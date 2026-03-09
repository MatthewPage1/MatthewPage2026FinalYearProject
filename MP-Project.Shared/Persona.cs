using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP_Project.Shared
{
	public class Persona
	{
		public string Name { get; set; }
		public List<ShoppingItem> ShoppingList { get; set; }
	}

	public class ShoppingItem
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
	}
}
