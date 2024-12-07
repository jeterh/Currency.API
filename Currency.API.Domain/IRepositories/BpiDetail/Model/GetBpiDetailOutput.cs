
namespace Currency.API.Domain.IRepositories.BpiDetail.Model
{
	public class GetBpiDetailOutput
	{
        public int Id { get; set; }
		public int BpiId { get; set; }
		public String Language { get; set; } = null!;
        public String Description { get; set; } = null!;
	}
}
