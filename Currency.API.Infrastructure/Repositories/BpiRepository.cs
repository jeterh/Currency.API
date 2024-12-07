using System.Data;
using System.Text;
using Dapper;
using Currency.API.Domain.IRepositories.Bpi.Model;
using Currency.API.Domain.IRepositories.Bpi;

namespace Currency.API.Infrastructure.Repositories
{

	public class BpiRepository : IBpiRepository
	{
		private readonly IDbConnection _con;

		public BpiRepository(IDbConnection con)
		{
			_con = con;
		}

		public async Task<IEnumerable<GetBpiOutput>> GetBpiAsync(Guid timeInfoId)
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine(@"SELECT [Id],[TimeInfoId],[CurrencyCode],[Symbol],[Rate],[RateFloat] FROM [dbo].[Bpi] WITH(NOLOCK)"); ;
			sql.AppendLine(" WHERE TimeInfoId = @timeInfoId");

			var data = await _con.QueryAsync<GetBpiOutput>(sql.ToString(), new
			{
				timeInfoId
			});

			return data;
		}

		public async Task<int?> InsertBpiAsync(InsertBpiInput input)
		{
			string sql = @"INSERT INTO [dbo].[Bpi]
			   ([TimeInfoId]
			   ,[CurrencyCode] 
			   ,[Symbol]
			   ,[Rate]
			   ,[RateFloat])
				VALUES
			   (@TimeInfoId
			   ,@CurrencyCode 
			   ,@Symbol
			   ,@Rate
			   ,@RateFloat);
			   SELECT SCOPE_IDENTITY() AS InsertedId;";

			try
			{
				var insertedId = await _con.QuerySingleAsync<int>(sql, input);

				if (insertedId > 0)
				{
					return insertedId;
				}

				return -1;
			}
			catch
			{
				return null;
			}
		}

		public async Task<bool> DeleteBpiAsync(int timeInfoId)
		{
			string sql = @"DELETE FROM [dbo].[Bpi]
                           WHERE TimeInfoId = @timeInfoId";
			try
			{
				int rowEffectiveCounts = await _con.ExecuteAsync(sql, new { timeInfoId });
				return rowEffectiveCounts == 1;
			}
			catch
			{
				throw;
			}
		}

		public async Task<bool> UpdateBpiAsync(UpdateBpiInput input)
		{
			string sql = @"UPDATE [dbo].[Bpi]
                           SET [Symbol] = @Symbol
                              ,[Rate] = @Rate
							  ,[RateFloat] = @RateFloat
                         WHERE TimeInfoId = @TimeInfoId and CurrencyCode = @Code";

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
