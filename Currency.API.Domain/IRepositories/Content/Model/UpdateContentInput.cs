
namespace Currency.API.Domain.IRepositories.Content.Model
{
	public class UpdateContentInput
	{
		public Guid TimeInfoId { get; set; }
		public string ContentnKey { get; set; }
		public string Language { get; set; }
		public string Content { get; set; }
	}
}
