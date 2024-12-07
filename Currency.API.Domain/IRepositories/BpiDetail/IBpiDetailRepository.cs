using Currency.API.Domain.IRepositories.BpiDetail.Model;

namespace Currency.API.Domain.IRepositories.BpiDetail
{
	public interface IBpiDetailRepository
	{
		Task<IEnumerable<GetBpiDetailOutput>> GetBpiDetailAsync(int bpiId);
		Task<bool> InsertBpiDetailAsync(InsertBpiDetailInput input);
		Task<bool> DeleteBpiDetailAsync(int id);
		Task<bool> UpdateBpiDetailAsync(UpdateBpiDetailInput input);
	}
}
