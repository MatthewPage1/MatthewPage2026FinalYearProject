using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MP_Project.Shared;

namespace MP_Project.Server.Data
{
	[Table("suppliertransactionitem")]
	public class SupplierTransactionItem
	{
		[Key]
		public int SupplierTransactionItemID { get; set; }

		public int SupplierTransactionID { get; set; }

		public SupplierTransaction? SupplierTransaction { get; set; }

		public int ProductId { get; set; }

		public Product? Product { get; set; }

		public int Quantity { get; set; }

		public decimal CostPrice { get; set; }

		public decimal Total { get; set; }
	}
}
