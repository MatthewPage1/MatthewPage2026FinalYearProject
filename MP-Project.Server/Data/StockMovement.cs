using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MP_Project.Shared;

namespace MP_Project.Server.Data
{
	public class StockMovement
	{
		[Key]
		public int MovementId { get; set; }

		[Required]
		public int ProductId { get; set; }

		[Required]
		public int ChangeAmount { get; set; }

		[Required]
		public string MovementType { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[ForeignKey("ProductId")]
		public Product? Product { get; set; }
	}
}