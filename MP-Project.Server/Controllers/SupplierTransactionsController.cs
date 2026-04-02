using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MP_Project.Server.Data;
using MP_Project.Shared;
using MySqlConnector;
using Dapper;
using Microsoft.AspNetCore.WebUtilities;

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
	public async Task<ActionResult<List<PurchaseDto>>> GetTransactions(int currentUserId)
	{
		var purchases = await (
			from Transaction in _context.SupplierTransaction
			join Products in _context.products
				on Transaction.ProductID equals Products.ProductId
			join Supplier in _context.Supplier
				on Transaction.SupplierID equals Supplier.SupplierID
			where Transaction.UserId == currentUserId
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
        (Quantity, CostPrice, TotalPrice, TransactionDate, DeliveryDate, Processed, SupplierID, ProductID, UserId)
        VALUES
        (@Quantity, @CostPrice, @TotalPrice, @TransactionDate, @DeliveryDate, @Processed, @SupplierID, @ProductID, @UserId)";

		await connection.ExecuteAsync(sql, transaction);

		return Ok();
	}

	[HttpPost("processDeliveries")]
	public async Task<IActionResult> ProcessDeliveries(int currentUserId)
	{
		var deliveries = await _context.SupplierTransaction
			.Where(t => t.UserId == currentUserId &&
						t.DeliveryDate.Date <= DateTime.Today &&
						!t.Processed)
			.ToListAsync();

		foreach (var delivery in deliveries)
			delivery.Processed = true;

		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpPost("checkInDelivery")]
	public async Task<IActionResult> CheckInDelivery(int transactionId, int currentUserId)
	{
		var delivery = await _context.SupplierTransaction
			.FirstOrDefaultAsync(t => t.TransactionID == transactionId && t.UserId == currentUserId);

		if (delivery == null || !delivery.Processed || delivery.CheckedIn)
			return BadRequest();

		var product = await _context.products
			.FirstOrDefaultAsync(p => p.ProductId == delivery.ProductID && p.UserId == currentUserId);

		if (product != null)
			product.StockCount += delivery.Quantity;

		delivery.CheckedIn = true;

		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpPost("checkInAllDeliveries")]
	public async Task<IActionResult> CheckInAllDeliveries(int currentUserId)
	{
		var deliveries = await _context.SupplierTransaction
			.Where(t => t.UserId == currentUserId &&
						t.Processed &&
						!t.CheckedIn)
			.ToListAsync();

		foreach (var delivery in deliveries)
		{
			var product = await _context.products
				.FirstOrDefaultAsync(p => p.ProductId == delivery.ProductID && p.UserId == currentUserId);

			if (product != null)
				product.StockCount += delivery.Quantity;

			delivery.CheckedIn = true;
		}

		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpPost("checkInPendingDeliveries")]
	public async Task<IActionResult> CheckInPendingDeliveries(int currentUserId)
	{
		var deliveries = await _context.SupplierTransaction
			.Where(t => t.UserId == currentUserId && !t.CheckedIn)
			.ToListAsync();

		foreach (var delivery in deliveries)
		{
			var product = await _context.products
				.FirstOrDefaultAsync(p => p.ProductId == delivery.ProductID && p.UserId == currentUserId);

			if (product != null)
				product.StockCount += delivery.Quantity;

			delivery.Processed = true;
			delivery.CheckedIn = true;
			delivery.DeliveryDate = DateTime.Today;
		}

		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpGet("delivery-cost-today")]
	public async Task<ActionResult<decimal>> GetDeliveryCostToday(int day, int currentUserId)
	{
		var simulatedDate = DateTime.Today.AddDays(day);

		var totalCost = await _context.SupplierTransaction
			.Where(t => t.UserId == currentUserId &&
						t.CheckedIn &&
						t.DeliveryDate.Date == simulatedDate)
			.SumAsync(t => (decimal?)t.TotalPrice) ?? 0;

		return Ok(totalCost);
	}
}