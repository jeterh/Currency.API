using System.Data;
using System.Text;
using Currency.API.Domain.IRepositories.BpiDetail;
using Dapper;
using Currency.API.Domain.IRepositories.BpiDetail.Model;

namespace Currency.API.Infrastructure.Repositories
{
	public class BpiDetailRepository : IBpiDetailRepository
	{
		private readonly IDbConnection _con;

		public BpiDetailRepository(IDbConnection con)
		{
			_con = con;
		}

		public async Task<IEnumerable<GetBpiDetailOutput>> GetBpiDetailAsync(int bpiId)
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine(@"SELECT [Id],[BpiId],[Language],[Description] FROM [dbo].[BpiDetail] WITH(NOLOCK)"); ;
			sql.AppendLine(" WHERE BpiId = @BpiId");

			var data = await _con.QueryAsync<GetBpiDetailOutput>(sql.ToString(), new
			{
				bpiId
			});

			return data;
		}

		public async Task<bool> InsertBpiDetailAsync(InsertBpiDetailInput input)
		{
			string sql = @"INSERT INTO [dbo].[BpiDetail]
			   ([BpiId]
			   ,[Language] 
			   ,[Description])
				VALUES
			   (@BpiId
			   ,@Language 
			   ,@Description)";

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

		public async Task<bool> DeleteBpiDetailAsync(int id)
		{
			string sql = @"DELETE FROM [dbo].[BpiDetail]
                           WHERE Id = @id";
			try
			{
				int rowEffectiveCounts = await _con.ExecuteAsync(sql, new { id });
				return rowEffectiveCounts == 1;
			}
			catch
			{
				throw;
			}
		}

		public async Task<bool> UpdateBpiDetailAsync(UpdateBpiDetailInput input)
		{
			string sql = @"UPDATE [dbo].[BpiDetail]
                           SET [Description] = @Description
                         WHERE BpiId = @BpiId and Language = @Language";

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
