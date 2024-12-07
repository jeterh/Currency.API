using Currency.API.Domain.IRepositories.TimeInfo.Model;

namespace Currency.API.Domain.IRepositories.TimeInfo
{
	public interface ITimeInfoRepository
	{
		Task<GetTimeInfoOutput> GetTimeInfoAsync(Guid timeInfoId);
		Task<bool> InsertTimeInfoAsync(InsertTimeInfoInput input);
		Task<bool> DeleteTimeInfoAsync(int timeInfoId);
		Task<bool> UpdateTimeInfoAsync(UpdateTimeInfoInput input);

	}
}
