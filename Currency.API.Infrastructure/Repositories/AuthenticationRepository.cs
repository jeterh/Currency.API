using Currency.API.Domain.IRepositories.Authentication.Model;
using Currency.API.Domain.IRepositories.Authentication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Currency.API.Infrastructure.Repositories
{
	public class AuthenticationRepository : IAuthenticationRepository
	{
		private readonly IDbConnection _con;

		public AuthenticationRepository(IDbConnection con)
		{
			_con = con;
		}

		public async Task<GetApplicationOutput?> GetApplication(string appName)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT [Name],[SecretKey],[Enabled] FROM [dbo].[Application] WHERE Name = @AppName");

			var data = await _con.QuerySingleOrDefaultAsync<GetApplicationOutput>(sql.ToString(), new
			{
				AppName = appName
			});

			return data;
		}
	}
}
