namespace MP_Project.Shared;

public class PurchaseItemDto
{
	public string? ProductName { get; set; }

	public int Quantity { get; set; }

	public decimal CostPrice { get; set; }

	public decimal Total { get; set; }
}