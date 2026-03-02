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


	//GET to return all products within the database
	[HttpGet]
	public async Task<ActionResult<List<Product>>> GetProducts()
	{

		return await _context.products.ToListAsync();
	}


	//PUT to decrease the stock of a speicfic product and also log the movement of stock
	[HttpPut("{id}/decrease-stock")]
	public async Task<IActionResult> DecreaseStock(int id, int quantity)
	{
		var product = await _context.products.FindAsync(id);

		if (product == null)
			return NotFound("Product not found.");

		if (product.StockCount < quantity)
			return BadRequest("Not enough stock.");

		product.StockCount -= quantity;

		//log the stock decrease
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

	//PUT to increase the stock of a speicfic product and also log the movement of stock
	[HttpPut("{id}/increase-stock")]
	public async Task<IActionResult> IncreaseStock(int id, int quantity)
	{
		var product = await _context.products.FindAsync(id);

		if (product == null)
			return NotFound("Product not found.");

		if (quantity <= 0)
			return BadRequest("Quantity must be greater than zero.");

		product.StockCount += quantity;

		//update stock decrease
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
	
	//GET to return best selling, total stock per group, recently ordered and underperforming
	[HttpGet("dashboard")]
	public async Task<IActionResult> GetDashboard()
	{
		//best sellers by quantity sold
		var bestSellers = await _context.StockMovements
			.Where(stockMovement => stockMovement.ChangeAmount < 0)
			.GroupBy(stockMovement => stockMovement.ProductId)
			.Select(productGroup => new
			{
				ProductId = productGroup.Key,
				TotalSold = productGroup.Sum(stockMovement => -stockMovement.ChangeAmount)
			})
			.OrderByDescending(summary => summary.TotalSold)
			.Take(5)
			.Join(_context.products,
				  summary => summary.ProductId,
				  product => product.ProductId,
				  (summary, product) => product)
			.ToListAsync();

		//total stock in each product group
		var totalStockByGroup = await _context.products
			.Where(product => product.ProductGroup1 != null)
			.GroupBy(product => product.ProductGroup1)
			.Select(productGroup => new
			{
				ProductGroup = productGroup.Key,
				TotalStock = productGroup.Sum(product => product.StockCount)
			})
			.OrderByDescending(groupStock => groupStock.TotalStock)
			.ToListAsync();

		//recently ordered products
		var recentlyOrdered = await _context.StockMovements
			.Where(stockMovement => stockMovement.ChangeAmount > 0)
			.OrderByDescending(stockMovement => stockMovement.CreatedAt)
			.Take(5)
			.Join(_context.products,
				  stockMovement => stockMovement.ProductId,
				  product => product.ProductId,
				  (stockMovement, product) => product)
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






