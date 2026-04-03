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

public class UsersController : ControllerBase
{
	private readonly IConfiguration _config;

	public UsersController(IConfiguration config)
	{
		_config = config;
	}

	[HttpPut("{id}/display-name")]
	public IActionResult UpdateDisplayName(int id, [FromBody] UpdateDisplayName request)
	{
		if (string.IsNullOrWhiteSpace(request.DisplayName))
			return BadRequest("Display name cannot be empty");

		var connString = _config.GetConnectionString("DefaultConnection");

		using var conn = new MySqlConnection(connString);

		var rows = conn.Execute(
			"UPDATE Users SET DisplayName = @DisplayName WHERE UserId = @UserId",
			new { DisplayName = request.DisplayName, UserId = id }
		);

		if (rows == 0)
			return NotFound();

		
		return Ok();
	}
}
