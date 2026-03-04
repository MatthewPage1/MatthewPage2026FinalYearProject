using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MP_Project.Shared;

namespace MP_Project.Server.Data
{
	[Table("suppliertransaction")]
	public class SupplierTransaction
	{
		[Key]
		public int SupplierTransactionID { get; set; }

		public decimal TotalPrice { get; set; }

		public DateTime TransactionDate { get; set; }

		public string? Invoice { get; set; }

		public int SupplierID { get; set; }   // FK

		public Supplier? Supplier { get; set; }

		public List<SupplierTransactionItem>? Items { get; set; } = new();
	}
}