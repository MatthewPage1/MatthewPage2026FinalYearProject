using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MP_Project.Server.Data;
using MP_Project.Shared;

namespace MP_Project.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierTransactionsController : ControllerBase
{
	private readonly AppDbContext _context;

	public SupplierTransactionsController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<List<PurchaseDto>>> GetPurchases()
	{
		var purchases = await _context.SupplierTransactions
			.Include(t => t.Supplier)
			.Include(t => t.Items)
				.ThenInclude(i => i.Product)
			.Select(t => new PurchaseDto
			{
				SupplierTransactionID = t.SupplierTransactionID,
				SupplierName = t.Supplier!.Name,
				ContactInfo = $"{t.Supplier.Phone} | {t.Supplier.Email}",
				Address = t.Supplier.Address,
				TransactionDate = t.TransactionDate,
				TotalPrice = t.TotalPrice,
				Items = t.Items!.Select(i => new PurchaseItemDto
				{
					ProductName = i.Product!.ProductName,
					Quantity = i.Quantity,
					CostPrice = i.CostPrice,
					Total = i.Total
				}).ToList()
			})
			.ToListAsync();

		return purchases;
	}

	[HttpPost]
	public async Task<IActionResult> CreatePurchase([FromBody] PurchaseDto purchase)
	{
		var transaction = new SupplierTransaction
		{
			SupplierID = purchase.SupplierID,
			TransactionDate = purchase.TransactionDate,
			TotalPrice = purchase.TotalPrice,
			Items = purchase.Items.Select(i => new SupplierTransactionItem
			{
				ProductId = i.ProductID,
				Quantity = i.Quantity,
				CostPrice = i.CostPrice,
				Total = i.Total
			}).ToList()
		};

		_context.SupplierTransactions.Add(transaction);

		await _context.SaveChangesAsync();

		return Ok();
	}

}