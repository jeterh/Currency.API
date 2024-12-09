
namespace Currency.API.Domain.IRepositories.Content.Model
{
	public class GetContentOutput
	{
		public int Id { get; set; }

		public Guid TimeInfoId { get; set; }

		public string ContentKey { get; set; } = null!;

		public string Language { get; set; } = null!;

		public string Content { get; set; } = null!;

		public DateTime CreatedAt { get; set; }
	}
}
