
namespace Currency.API.Domain.IRepositories.Bpi.Model
{
	public class InsertBpiInput
	{
		public Guid TimeInfoId { get; set; }
		public string CurrencyCode { get; set; } = null!;
		public string Symbol { get; set; } = null!;
		public string Rate { get; set; } = null!;
		public decimal RateFloat { get; set; }
	}
}
