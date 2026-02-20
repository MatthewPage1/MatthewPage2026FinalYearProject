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

		if (product.StockCount == 0)
		{
			product.Availability = "OutOfStock";
		}
		else
		{
			product.Availability = "InStock";
		}

		if (product.StockCount == product.ReorderLevel)
		{
			Console.WriteLine("Reorder triggered!");
		}

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

		await _context.SaveChangesAsync();

		return Ok(product);
	}


}
