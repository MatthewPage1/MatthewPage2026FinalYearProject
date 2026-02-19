using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MP_Project.Shared;


namespace MP_Project.Server.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) { }

		public DbSet<Product> productstest { get; set; }
	}
}
