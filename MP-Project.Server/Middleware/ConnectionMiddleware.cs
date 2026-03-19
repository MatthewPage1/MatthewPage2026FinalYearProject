namespace MP_Project.Server.Middleware
{
	public class ConnectionMiddleware
	{
		private readonly RequestDelegate _next;

		public ConnectionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			var conn = context.Session.GetString("ConnString");

			if (!string.IsNullOrEmpty(conn))
			{
				context.Items["ConnString"] = conn.ToString();
			}

			await _next(context);
		}
	}
}