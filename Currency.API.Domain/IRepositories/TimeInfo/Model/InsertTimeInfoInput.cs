
namespace Currency.API.Domain.IRepositories.TimeInfo.Model
{
	public class InsertTimeInfoInput
	{
        public Guid Id { get; set; }
        public string Updated { get; set; }
        public string UpdatedISO { get; set; }
        public string UpdatedUK { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
