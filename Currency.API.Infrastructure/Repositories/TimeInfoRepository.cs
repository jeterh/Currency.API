using System.Data;
using System.Text;
using Dapper;
using Currency.API.Domain.IRepositories.TimeInfo;
using Currency.API.Domain.IRepositories.TimeInfo.Model;

namespace Currency.API.Infrastructure.Repositories
{

	public class TimeInfoRepository : ITimeInfoRepository
	{
		private readonly IDbConnection _con;

		public TimeInfoRepository(IDbConnection con)
		{
			_con = con;
		}

		public async Task<GetTimeInfoOutput> GetTimeInfoAsync(Guid timeInfoId)
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine(@"SELECT [Id],[Updated],[UpdatedISO],[UpdatedUK],[UpdatedAt] FROM [dbo].[TimeInfo] WITH(NOLOCK)");
			sql.AppendLine(" WHERE Id = @timeInfoId");

			var data = await _con.QueryFirstOrDefaultAsync<GetTimeInfoOutput>(sql.ToString(), new
			{
				timeInfoId
			});

			return data;
		}

		public async Task<bool> InsertTimeInfoAsync(InsertTimeInfoInput input)
		{
			string sql = @"INSERT INTO [dbo].[TimeInfo]
			   ([Id]
			   ,[Updated] 
			   ,[UpdatedISO]
			   ,[UpdatedUK]
			   ,[UpdatedAt]
				)
				VALUES
			   (@Id
			   ,@Updated 
			   ,@UpdatedISO
			   ,@UpdatedUK
			   ,@UpdatedAt)";

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

		public async Task<bool> UpdateTimeInfoAsync(UpdateTimeInfoInput input)
		{
			string sql = @"UPDATE [dbo].[TimeInfo]
                           SET [Updated] = @Updated
                              ,[UpdatedISO] = @UpdatedISO
                              ,[UpdatedUK] = @UpdatedUK
                              ,[UpdatedAt] = @UpdatedAt
                         WHERE Id = @Id";

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
