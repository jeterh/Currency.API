using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.API.Domain.IRepositories.View.Model
{
	public class GetCurrencyInfoOutput
	{
		public string Updated { get; set; } = null!;
		public string DisclaimerContent { get; set; } = null!;
		public string ChartNameContent { get; set; } = null!;
		public string Language { get; set; } = null!;
	}
}
