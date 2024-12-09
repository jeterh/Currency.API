using Currency.API.Domain.IRepositories.Content.Model;
using Currency.API.Domain.IRepositories.TimeInfo.Model;
using Currency.API.Domain.IRepositories.View.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.API.Domain.IRepositories.View
{
	public interface IViewRepository
	{
		Task<IEnumerable<GetCurrencyInfoOutput>> GetCurrencyInfoAsync(Guid TimeInfoId);
		Task<IEnumerable<GetCurrencysOutput>> GetCurrencysAsync(Guid timeInfoId);
	}
}
