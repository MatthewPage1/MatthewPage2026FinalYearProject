using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MP_Project.Server.Data;
using MP_Project.Shared;
using MySqlConnector;
using BCrypt.Net;

namespace MP_Project.Server.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly string _connString;

		public AuthController(IConfiguration config)
		{
			_connString = config.GetConnectionString("DefaultConnection");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] MP_Project.Shared.LoginRequest request)
		{
			try
			{
				using var conn = new MySqlConnection(_connString);
				await conn.OpenAsync();

				using var cmd = new MySqlCommand(
					"SELECT UserId, Username, PasswordHash, DisplayName FROM Users WHERE Username = @username",
					conn
				);

				cmd.Parameters.AddWithValue("@username", request.Username);

				using var reader = await cmd.ExecuteReaderAsync();

				if (!await reader.ReadAsync())
					return Unauthorized(new
					{
						success = false,
						message = "Invalid login"
					});

				var userId = reader.GetInt32("UserId");
				var username = reader.GetString("Username");
				var passwordHash = reader.GetString("PasswordHash");
				var displayName = reader.GetString("DisplayName");
				Console.WriteLine($"User {username} logged in with ID {userId}");

				// Use the following only to get the hash in the 1st place, then we'll store it in the DB and use that for comparison
				// Do not use this repeadedly as it will generate a new hash every time and won't match the stored hash.
				// Console.WriteLine($"hashed password = {BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12)}");


				// Now we compare the provided password with the stored hash using BCrypt's Verify method
												
				if (!BCrypt.Net.BCrypt.Verify(request.Password, passwordHash))
				{
					return Unauthorized(new
					{
						success = false,
						message = "Invalid login"
					});
				}

				return Ok(new
				{
					success = true,
					message = "Login is successful",
					currentUserId = userId,   //userId
					DisplayName = displayName
				});

			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					success = false,
					message = "Server error",
					error = ex.Message
				});
			}
		}

		[HttpPost("logout")]
		public IActionResult Logout()
		{

			/* Commented out as to replace with a different loggin system
			MySqlConnection.ClearAllPools();
			HttpContext.Session.Clear();

			*/
		//	currentUserId = 0;
			return Ok(new
			{
				success = true,
				message = "Logged out"
			});

		}
		
	}
}