using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MP_Project.Server.Data;
using MP_Project.Shared;
using MySqlConnector;

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

		/*
		private readonly AppDbContext _context;

		public AuthController(AppDbContext context)
		{
			_context = context;
		}
		*/



		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] MP_Project.Shared.LoginRequest request)
		{
			try
			{
				using var conn = new MySqlConnection(_connString);
				await conn.OpenAsync();

				using var cmd = new MySqlCommand(
					"SELECT UserId, Username, PasswordHash FROM Users WHERE Username = @username",
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
				Console.WriteLine($"User {username} logged in with ID {userId}");	
				Console.WriteLine($"Password hash from DB: {passwordHash}, Password from request: {request.Password}");	


				if (passwordHash != request.Password)
					return Unauthorized(new
					{
						success = false,
						message = "Invalid login"
					});

				return Ok(new
				{
					success = true,
					message = "Login is successful",
					currentUserId = userId,   //userId,
				});

				/*
				 * var result = await _context.Users
					.FirstOrDefaultAsync(u =>
						u.Username == request.Username &&
						u.PasswordHash == request.Password);
						Console.Write($"result" , result);

				

					var connString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

					using var conn = new MySqlConnection();
					conn.Open();

					var cmd = new MySqlCommand(
						"SELECT UserId, PasswordHash FROM Users WHERE Username = @username",
						conn
					);

					cmd.Parameters.AddWithValue("@username", request.Username);

					using var reader = cmd.ExecuteReader();

				
				if (!result)
					{
						return Unauthorized(new
						{
							success = false,
							message = "Invalid login"
						});
					}
				
				//var userId = reader.GetInt32("UserId");
				//var storedPassword = reader.GetString("PasswordHash");
				//Console.WriteLine($"User {request.Username} logged in with ID {userId}");

				//if (storedPassword != request.Password)
				//{
				//	return Unauthorized(new
				//	{
				//		success = false,
				//		message = "Invalid login"
				//	});
				//}

				return Ok(new
				{
					success = true,
					message = "Login is successful",
					currentUserId = 2,   //userId,
				});
				*/
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
			return Ok(new
			{
				success = true,
				message = "Logged out"
			});

		}
		
	}
}