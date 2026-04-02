
namespace MP_Project.Shared
{
	public class SimulationHistory
	{
		public int Id { get; set; }
		public int Day { get; set; }
		public decimal Balance { get; set; }
		public decimal Revenue { get; set; }
		public decimal Costs { get; set; }
		public DateTime Timestamp { get; set; }
		public int UserId { get; set; }
	}
}