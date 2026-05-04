using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using MP_Project.Shared;

namespace MP_Project.Server.Data
{
	public class AppDbContext : DbContext
	{
		private readonly IHttpContextAccessor _http;

		public AppDbContext(
			DbContextOptions<AppDbContext> options,
			IHttpContextAccessor http)
			: base(options)
		{
			_http = http;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			if (!options.IsConfigured)
			{
				var conn = _http.HttpContext?.Items["ConnString"]?.ToString();

				if (!string.IsNullOrEmpty(conn))
				{
					options.UseMySql(conn, ServerVersion.AutoDetect(conn));
				}
				else
				{
					throw new Exception("No connection string found");
				}
			}
		}

		public DbSet<Product> products { get; set; }
		public DbSet<StockMovement> StockMovements { get; set; }
		public DbSet<SupplierTransaction> SupplierTransaction { get; set; }
		public DbSet<Supplier> Supplier { get; set; }
		public DbSet<Sale> Sales { get; set; }
		public DbSet<SimulationHistory> SimulationHistory { get; set; }
	
	}
}