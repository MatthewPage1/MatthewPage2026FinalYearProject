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


	[HttpGet("history/byuser")]
	public async Task<ActionResult<List<SimulationHistory>>> GetHistoryByUser(int currentUserId)
	{
		return await _context.SimulationHistory
			.Where(x => x.UserId == currentUserId)
			.OrderBy(x => x.Day)
			.ToListAsync();
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
