namespace MP_Project.Shared
{
	public class Product
	{
		public int ProductId { get; set; }
		public string? ProductName { get; set; }
		public string? gtin13 { get; set; }
		public int StockCount { get; set; }
		public int ReorderLevel { get; set; }
		public string? Availability { get; set; }
		public string? ProductDescription { get; set; }
		public string? ProductGroup1 { get; set; }
		public string? ProductGroup2 { get; set; }
		public string? ProductGroup3 { get; set; }
		public string? Brand { get; set; }

		public decimal CostPrice { get; set; }
		public decimal SellingPrice { get; set; }
		public string? Image { get; set; }
		public int SupplierID { get; set; }
	}
}
