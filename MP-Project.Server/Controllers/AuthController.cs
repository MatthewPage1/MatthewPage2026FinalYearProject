using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using MP_Project.Shared;

namespace MP_Project.Server.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		[HttpPost("login")]
		public IActionResult Login([FromBody] MP_Project.Shared.LoginRequest request)
		{
			try
			{
				var builder = new MySqlConnectionStringBuilder
				{
					Server = "localhost",             
					Database = "FinalYearProject",  
					UserID = request.Username,
					Password = request.Password,
					SslMode = MySqlSslMode.None       
				};

				var connString = builder.ConnectionString;

				using var conn = new MySqlConnection(connString);
				conn.Open();

				HttpContext.Session.SetString("ConnString", connString);

				return Ok(new
				{
					success = true,
					connection = connString
				});
			}
			catch
			{
				return Unauthorized(new
				{
					success = false,
					message = "Invalid login"
				});
			}
		}

		[HttpPost("logout")]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();

			return Ok(new
			{
				success = true,
				message = "Logged out"
			});
		}

	}
}