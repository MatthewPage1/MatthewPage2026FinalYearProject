using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MP_Project.Server.Data;
using MP_Project.Shared;
using MySqlConnector;
using Dapper;

namespace MP_Project.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierTransactionsController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly IConfiguration _config;

	public SupplierTransactionsController(AppDbContext context, IConfiguration config)
	{
		_context = context;
		_config = config;
	}

	[HttpGet]
	public async Task<ActionResult<List<SupplierTransaction>>> GetTransactions()
	{
		var transactions = await _context.SupplierTransaction.ToListAsync();
		return Ok(transactions);
	}

	[HttpPost("addSupplierTransaction")]
	public async Task<IActionResult> AddSupplierTransaction([FromBody] SupplierTransaction transaction)
	{
		using var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

		string sql = @"INSERT INTO suppliertransaction
		(Quantity, CostPrice, TotalPrice, TransactionDate, DeliveryDate, Processed, SupplierID, ProductID)
		VALUES
		(@Quantity, @CostPrice, @TotalPrice, @TransactionDate, @DeliveryDate, @Processed, @SupplierID, @ProductID)";

		await connection.ExecuteAsync(sql, transaction);

		return Ok();
	}

	[HttpPost("processDeliveries")]
	public async Task<IActionResult> ProcessDeliveries()
	{
		var deliveries = await _context.SupplierTransaction
			.Where(t => t.DeliveryDate <= DateTime.Today && !t.Processed)
			.ToListAsync();

		foreach (var delivery in deliveries)
		{
			var product = await _context.products
				.FirstOrDefaultAsync(p => p.ProductId == delivery.ProductID);

			if (product != null)
			{
				product.StockCount += delivery.Quantity;
				delivery.Processed = true;
			}
		}

		await _context.SaveChangesAsync();

		return Ok();
	}


}