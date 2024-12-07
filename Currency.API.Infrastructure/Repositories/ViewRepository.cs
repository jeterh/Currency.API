using Currency.API.Domain.IRepositories.TimeInfo.Model;
using Currency.API.Domain.IRepositories.TimeInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Currency.API.Domain.IRepositories.View;
using Dapper;
using Currency.API.Domain.IRepositories.View.Model;
using Currency.API.Domain.IRepositories.Bpi.Model;
using Currency.API.Domain.IRepositories.Content.Model;

namespace Currency.API.Infrastructure.Repositories
{
	public class ViewRepository : IViewRepository
	{
		private readonly IDbConnection _con;

		public ViewRepository(IDbConnection con)
		{
			_con = con;
		}

		public async Task<IEnumerable<GetCurrencyInfoOutput>> GetCurrencyInfoAsync(Guid timeInfoId)
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine(@"SELECT ti.[Updated],
				MAX(CASE WHEN c.ContentnKey = 'Disclaimer' THEN Content END) AS DisclaimerContent,
				MAX(CASE WHEN c.ContentnKey = 'ChartName' THEN Content END) AS ChartNameContent,
				c.Language
					FROM [dbo].[TimeInfo] AS ti WITH(NOLOCK)
					INNER JOIN Content AS c WITH(NOLOCK) ON ti.Id = c.TimeInfoId
			 WHERE ti.Id = @timeInfoId
			 GROUP BY ti.[Updated],c.Language");

			var data = await _con.QueryAsync<GetCurrencyInfoOutput>(sql.ToString(), new
			{
				timeInfoId
			});

			return data;
		}

		public async Task<IEnumerable<GetCurrencysOutput>> GetCurrencysAsync(Guid timeInfoId)
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine(@"SELECT b.[CurrencyCode]
				  ,b.[Symbol]
				  ,b.[Rate]
				  ,b.[RateFloat]
				  ,bd.Language
				 ,bd.Description
			  FROM [dbo].[Bpi] AS b WITH(NOLOCK)
			  INNER JOIN BpiDetail AS bd WITH(NOLOCK) ON b.Id = bd.BpiId
			 WHERE b.TimeInfoId = @timeInfoId");

			var data = await _con.QueryAsync<GetCurrencysOutput>(sql.ToString(), new
			{
				timeInfoId
			});

			return data;
		}
	}
}
