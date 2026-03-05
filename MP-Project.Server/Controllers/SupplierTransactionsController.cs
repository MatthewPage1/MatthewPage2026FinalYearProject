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

	[HttpPost("addSupplierTransaction")]
	public async Task<IActionResult> AddSupplierTransaction([FromBody] Supplier transaction)
	{
		using var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

		string sql = @"INSERT INTO suppliertransaction
                      (Quantity, CostPrice, TotalPrice, TransactionDate, SupplierID, ProductID)
                      VALUES
                      (@Quantity, @CostPrice, @TotalPrice, @TransactionDate, @SupplierID, @ProductID)";

		await connection.ExecuteAsync(sql, transaction);

		return Ok();
	}
}