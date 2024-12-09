
namespace Currency.API.Domain.IRepositories.Bpi.Model
{
	public class GetBpiOutput
	{
        public int Id { get; set; }
        public Guid TimeInfoId { get; set; }
        public string CurrencyCode { get; set; } = null!;
		public string Symbol { get; set; } = null!;
		public string Rate { get; set; } = null!;
		public double RateFloat { get; set; }
	}
}
