using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MP_Project.Shared
{
	public class Supplier
	{
		public int Quantity { get; set; }
		public string? CostPrice { get; set; }
		public string? TotalPrice { get; set; }
		public string? TransactionDate { get; set; }
		public int SupplierID { get; set; }
		public int ProductID { get; set; }
	}
}