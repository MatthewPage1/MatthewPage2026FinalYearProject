using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MP_Project.Server.Data;
using MP_Project.Shared;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
	private readonly AppDbContext _context;

	public ProductsController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<List<Product>>> GetProducts()
	{

		return await _context.products.ToListAsync();
	}

	[HttpPut("{id}/decrease-stock")]
	public async Task<IActionResult> DecreaseStock(int id, int quantity)
	{
		var product = await _context.products.FindAsync(id);

		if (product == null)
			return NotFound("Product not found.");

		if (product.StockCount < quantity)
			return BadRequest("Not enough stock.");

		product.StockCount -= quantity;

		// LOG THE STOCK MOVEMENT
		var movement = new StockMovement
		{
			ProductId = product.ProductId,
			ChangeAmount = -quantity,
			MovementType = "Decrease",
			CreatedAt = DateTime.UtcNow
		};

		_context.StockMovements.Add(movement);

		await _context.SaveChangesAsync();

		return Ok(product);
	}


	[HttpPut("{id}/increase-stock")]
	public async Task<IActionResult> IncreaseStock(int id, int quantity)
	{
		var product = await _context.products.FindAsync(id);

		if (product == null)
			return NotFound("Product not found.");

		if (quantity <= 0)
			return BadRequest("Quantity must be greater than zero.");

		product.StockCount += quantity;

		// Update availability
		if (product.StockCount > 0)
		{
			product.Availability = "InStock";
		}


		_context.StockMovements.Add(new StockMovement
		{
			ProductId = product.ProductId,
			ChangeAmount = -quantity,
			MovementType = "Sale",
			CreatedAt = DateTime.UtcNow
		});


		await _context.SaveChangesAsync();

		return Ok(product);
	}

	[HttpGet("dashboard")]
	public async Task<IActionResult> GetDashboard()
	{
		var bestSellers = await _context.StockMovements
			.Where(m => m.ChangeAmount < 0)
			.GroupBy(m => m.ProductId)
			.Select(g => new
			{
				ProductId = g.Key,
				TotalSold = g.Sum(x => -x.ChangeAmount)
			})
			.OrderByDescending(x => x.TotalSold)
			.Take(5)
			.Join(_context.products,
				  g => g.ProductId,
				  p => p.ProductId,
				  (g, p) => p)
			.ToListAsync();

		var totalStockByGroup = await _context.products
			.Where(p => p.ProductGroup1 != null)
			.GroupBy(p => p.ProductGroup1)
			.Select(g => new
			{
				ProductGroup = g.Key,
				TotalStock = g.Sum(p => p.StockCount)
			})
			.OrderByDescending(x => x.TotalStock)
			.ToListAsync();

		var recentlyOrdered = await _context.StockMovements
			.Where(m => m.ChangeAmount > 0)
			.OrderByDescending(m => m.CreatedAt)
			.Take(5)
			.Join(_context.products,
				  m => m.ProductId,
				  p => p.ProductId,
				  (m, p) => p)
			.ToListAsync();

		return Ok(new
		{
			BestSellers = bestSellers,
			TotalStockByGroup = totalStockByGroup,
			RecentlyOrdered = recentlyOrdered,
			Underperforming = new List<Product>()
		});
	}
}






