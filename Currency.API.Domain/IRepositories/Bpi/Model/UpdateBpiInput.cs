
namespace Currency.API.Domain.IRepositories.Bpi.Model
{
	public class UpdateBpiInput
	{
		public Guid TimeInfoId { get; set; }
		public string Code { get; set; } = null!;
		public string Symbol { get; set; } = null!;
		public string Rate { get; set; } = null!;
		public double RateFloat { get; set; }
	}
}
