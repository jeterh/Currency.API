using Currency.API.Domain.IRepositories.Content.Model;

namespace Currency.API.Domain.IRepositories.Content
{
	public interface IContentRepository
	{
		Task<IEnumerable<GetContentOutput>> GetContentAsync(Guid TimeInfoId);
		Task<bool> InsertContentAsync(InsertContentInput input);
		Task<bool> DeleteContentAsync(DeleteContentInput input);
		Task<bool> UpdateContentAsync(UpdateContentInput input);
	}
}
