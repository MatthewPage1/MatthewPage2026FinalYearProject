using Microsoft.AspNetCore.Mvc;
using MP_Project.Server.Data;
using MP_Project.Shared;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
	private readonly AppDbContext _context;

	public SimulationController(AppDbContext context)
	{
		_context = context;
	}

	[HttpPost("history")]
	public async Task<IActionResult> AddHistory(SimulationHistory record)
	{
		if (record.Day % 7 == 0)
		{
			Console.WriteLine($"User {record.UserId}: Day {record.Day} is divisible by 7");

			var products = await _context.products
				.Where(p => p.UserId == record.UserId)
				.ToListAsync();

			if (products.Any())
			{
				var rand = Random.Shared;

				foreach (var product in products)
				{
					product.Promotion = false;
					product.SellingPrice = product.OriginalSellingPrice;
				}

				var promoProducts = products
					.OrderBy(x => rand.Next())
					.Take(Math.Max(1, (int)Math.Ceiling(products.Count * 0.3)));

				foreach (var product in promoProducts)
				{
					product.Promotion = true;

					var discountPercent = rand.Next(5, 51);
					var multiplier = 1 - (discountPercent / 100m);

					var discountedPrice = product.OriginalSellingPrice * multiplier;

					product.SellingPrice = Math.Max(product.CostPrice, discountedPrice);
				}
			}
		}

		_context.SimulationHistory.Add(record);
		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpGet("history")]
	public async Task<ActionResult<List<SimulationHistory>>> GetHistory()
	{
		return await _context.SimulationHistory
			.OrderBy(x => x.Day)
			.ToListAsync();
	}


	[HttpPut("history")]
	public async Task<IActionResult> UpdateHistory([FromBody] SimulationHistory updated)
	{
		var existing = await _context.SimulationHistory
			.FirstOrDefaultAsync(x => x.UserId == updated.UserId && x.Day == updated.Day);

		if (existing == null)
			return NotFound();

		existing.Costs = updated.Costs;
		existing.Revenue = updated.Revenue;
		existing.Balance = updated.Balance;
		existing.Timestamp = updated.Timestamp;

		await _context.SaveChangesAsync();

		return Ok(existing);
	}


	[HttpGet("history/byuser")]
	public async Task<ActionResult<List<SimulationHistory>>> GetHistoryByUser(int currentUserId)
	{
		return await _context.SimulationHistory
			.Where(x => x.UserId == currentUserId)
			.OrderBy(x => x.Day)
			.ToListAsync();
	}

	[HttpGet("history/by-user-day")]
	public async Task<IActionResult> GetByUserAndDay(int userId, int day)
	{
		var record = await _context.SimulationHistory
			.FirstOrDefaultAsync(x => x.UserId == userId && x.Day == day);

		if (record == null)
			return NotFound();

		return Ok(record);
	}

	[HttpGet("balance/byuser")]
	public async Task<ActionResult<decimal>> GetBalance(int currentUserId)
	{
		var latest = await _context.SimulationHistory
			.Where(x => x.UserId == currentUserId)
			.OrderByDescending(x => x.Day)
			.Select(x => x.Balance)
			.FirstOrDefaultAsync();

		return Ok(latest);
	}




}
