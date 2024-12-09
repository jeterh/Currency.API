
namespace Currency.API.Domain.IRepositories.Bpi.Model
{
	public class UpdateBpiInput
	{
		public Guid TimeInfoId { get; set; }
		public string Code { get; set; } = null!;
		public string? Symbol { get; set; }
		public string? Rate { get; set; }
		public decimal? RateFloat { get; set; }
	}
}
