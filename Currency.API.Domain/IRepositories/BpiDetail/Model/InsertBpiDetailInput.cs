
namespace Currency.API.Domain.IRepositories.BpiDetail.Model
{
	public class InsertBpiDetailInput
	{
        public int BpiId { get; set; }
        public String Language { get; set; } = null!;
        public String Description { get; set; } = null!;
    }
}
