using Currency.API.Domain.IRepositories.Content;
using Currency.API.Domain.IRepositories.Content.Model;
using Dapper;
using System.Data;
using System.Text;

namespace Currency.API.Infrastructure.Repositories
{
	public class ContentRepository : IContentRepository
	{
		private readonly IDbConnection _con;

		public ContentRepository(IDbConnection con)
		{
			_con = con;
		}

		public async Task<IEnumerable<GetContentOutput>> GetContentAsync(Guid timeInfoId)
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine(@"SELECT [Id],[TimeInfoId],[ContentKey],[Language],[Content] FROM [dbo].[Content] WITH(NOLOCK)"); ;
			sql.AppendLine(" WHERE TimeInfoId = @timeInfoId");

			var data = await _con.QueryAsync<GetContentOutput>(sql.ToString(), new
			{
				timeInfoId
			});

			return data;
		}

		public async Task<bool> InsertContentAsync(InsertContentInput input)
		{
			string sql = @"INSERT INTO [dbo].[Content]
			   ([TimeInfoId]
			   ,[ContentKey] 
			   ,[Language]
			   ,[Content])
				VALUES
			   (@TimeInfoId
			   ,@ContentKey 
			   ,@Language
			   ,@Content)";

			bool result = false;
			try
			{
				int rowEffectiveCounts = await _con.ExecuteAsync(sql, input);
				if (rowEffectiveCounts == 1)
				{
					result = true;
				}
			}
			catch
			{
				throw;
			}

			return result;
		}

		public async Task<bool> DeleteContentAsync(DeleteContentInput input)
		{
			string sql = @"DELETE FROM [dbo].[Content]
                           WHERE TimeInfoId = @timeInfoId and ContentKey = @ContentKey and Language = @Language ";
			try
			{
				int rowEffectiveCounts = await _con.ExecuteAsync(sql, input);
				return rowEffectiveCounts == 1;
			}
			catch
			{
				throw;
			}
		}

		public async Task<bool> UpdateContentAsync(UpdateContentInput input)
		{
			string sql = @"UPDATE [dbo].[Content]
                           SET [Content] = @Content
                         WHERE TimeInfoId = @TimeInfoId and ContentKey = @ContentKey and Language = @Language";

			bool result = false;
			try
			{
				int rowEffectiveCounts = await _con.ExecuteAsync(sql, input);
				if (rowEffectiveCounts == 1)
				{
					result = true;
				}
			}
			catch
			{
				throw;
			}

			return result;
		}
	}
}
