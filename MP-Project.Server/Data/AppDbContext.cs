using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MP_Project.Shared;


namespace MP_Project.Server.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) { }

		public DbSet<Product> products { get; set; }
		public DbSet<StockMovement> StockMovements { get; set; }

		public DbSet<SupplierTransaction> SupplierTransactions { get; set; }
		public DbSet<SupplierTransactionItem> SupplierTransactionItems { get; set; }
		public DbSet<Supplier> Suppliers { get; set; }

	}
}
