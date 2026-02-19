namespace MP_Project.Shared
{
	public class Product
	{
		public int ProductId { get; set; }
		public string? ProductName { get; set; }
		public int StockCount { get; set; }
		public int ReorderLevel { get; set; }
		public string? Availability { get; set; }
	}
}
