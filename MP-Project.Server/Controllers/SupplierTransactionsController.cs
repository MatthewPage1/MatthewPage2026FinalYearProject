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
	public async Task<ActionResult<List<PurchaseDto>>> GetTransactions()
	{
		var purchases = await (
			from Transaction in _context.SupplierTransaction
			join Products in _context.products
				on Transaction.ProductID equals Products.ProductId
			join Supplier in _context.Supplier
				on Transaction.SupplierID equals Supplier.SupplierID
			orderby Transaction.TransactionID descending
			select new PurchaseDto
			{
				TransactionID = Transaction.TransactionID,
				SupplierName = Supplier.Name,
				SupplierEmail = Supplier.Email,
				SupplierPhone = Supplier.Phone,
				ProductName = Products.ProductName,
				Quantity = Transaction.Quantity,
				TotalPrice = Transaction.TotalPrice,
				DeliveryDate = Transaction.DeliveryDate,
				Processed = Transaction.Processed,
				CheckedIn = Transaction.CheckedIn
			}
		).ToListAsync();

		return Ok(purchases);
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
			.Where(t => t.DeliveryDate.Date <= DateTime.Today && !t.Processed)
			.ToListAsync();

		foreach (var delivery in deliveries)
		{
			delivery.Processed = true;
		}

		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpPost("checkInDelivery")]
	public async Task<IActionResult> CheckInDelivery([FromBody] int transactionId)
	{
		var delivery = await _context.SupplierTransaction
			.FirstOrDefaultAsync(t => t.TransactionID == transactionId);

		if (delivery == null || !delivery.Processed || delivery.CheckedIn)
			return BadRequest();

		var product = await _context.products
			.FirstOrDefaultAsync(p => p.ProductId == delivery.ProductID);

		if (product != null)
		{
			product.StockCount += delivery.Quantity;
		}

		delivery.CheckedIn = true;

		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpPost("checkInAllDeliveries")]
	public async Task<IActionResult> CheckInAllDeliveries()
	{
		var deliveries = await _context.SupplierTransaction
			.Where(t => t.Processed && !t.CheckedIn)
			.ToListAsync();

		foreach (var delivery in deliveries)
		{
			var product = await _context.products
				.FirstOrDefaultAsync(p => p.ProductId == delivery.ProductID);

			if (product != null)
			{
				product.StockCount += delivery.Quantity;
			}

			delivery.CheckedIn = true;
		}

		await _context.SaveChangesAsync();

		return Ok();
	}


}