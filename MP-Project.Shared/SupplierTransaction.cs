using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MP_Project.Shared
{
	public class SupplierTransaction
	{
		[Key]
		public int TransactionID { get; set; }
		public int Quantity { get; set; }
		public decimal CostPrice { get; set; }
		public decimal TotalPrice { get; set; }
		public DateTime TransactionDate { get; set; }
		public DateTime DeliveryDate { get; set; }
		public bool Processed { get; set; }
		public int SupplierID { get; set; }
		public int ProductID { get; set; }
	}
}