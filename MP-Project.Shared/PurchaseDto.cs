namespace MP_Project.Shared;

public class PurchaseDto
{
	public int SupplierTransactionID { get; set; }

	public string? SupplierName { get; set; }
	public string? ContactInfo { get; set; }
	public string? Address { get; set; }

	public DateTime TransactionDate { get; set; }

	public decimal TotalPrice { get; set; }

	public List<PurchaseItemDto> Items { get; set; } = new();
}