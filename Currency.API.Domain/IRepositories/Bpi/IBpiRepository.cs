using Currency.API.Domain.IRepositories.Bpi.Model;

namespace Currency.API.Domain.IRepositories.Bpi
{
	public interface IBpiRepository
	{
		Task<IEnumerable<GetBpiOutput>> GetBpiAsync(Guid timeInfoId);
		Task<int?> InsertBpiAsync(InsertBpiInput input);
		Task<bool> UpdateBpiAsync(UpdateBpiInput input);
	}
}
