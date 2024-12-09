using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.API.Domain.IRepositories.View.Model
{
	public class GetCurrencysOutput
	{
        public string CurrencyCode { get; set; } = null!;
        public string Symbol { get; set; } = null!;
		public string Rate { get; set; } = null!;
		public decimal RateFloat { get; set; }
		public string Language { get; set; } = null!;
		public string Description { get; set; } = null!;
	}
}
