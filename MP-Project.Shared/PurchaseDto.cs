using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP_Project.Shared
{
	public class PurchaseDto
	{
		public int TransactionID { get; set; }

		public string? SupplierName { get; set; }
		public string? SupplierEmail { get; set; }
		public string? SupplierPhone { get; set; }

		public string? ProductName { get; set; }

		public int Quantity { get; set; }

		public decimal TotalPrice { get; set; }

		public DateTime DeliveryDate { get; set; }

		public bool Processed { get; set; }

		public bool CheckedIn { get; set; }

		public bool IsChecked { get; set; }
	}
}
