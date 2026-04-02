using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP_Project.Shared
{
	public class Sale
	{
		public int SaleID { get; set; }
		public int Quantity { get; set; }
		public decimal SellingPrice { get; set; }
		public decimal TotalPrice { get; set; }
		public DateTime SaleDate { get; set; }
		public int ProductID { get; set; }
		public int UserId { get; set; }
	}
}
