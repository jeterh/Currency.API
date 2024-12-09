using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.API.Domain.IRepositories.Authentication.Model
{
	public class GetApplicationOutput
	{
		public string Name { get; set; } = null!;

		public string SecretKey { get; set; } = null!;
	}
}
