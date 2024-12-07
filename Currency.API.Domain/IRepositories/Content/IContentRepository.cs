using Currency.API.Domain.IRepositories.Content.Model;

namespace Currency.API.Domain.IRepositories.Content
{
	public interface IContentRepository
	{
		Task<IEnumerable<GetContentOutput>> GetContentAsync(string TimeInfoId);
		Task<bool> InsertContentAsync(InsertContentInput input);
		Task<bool> DeleteContentAsync(int timeInfoId);
		Task<bool> UpdateContentAsync(UpdateContentInput input);
	}
}
