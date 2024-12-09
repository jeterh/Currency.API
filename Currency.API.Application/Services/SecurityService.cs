using Currency.API.Utilities.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Currency.API.Application.Services.SecurityService;

namespace Currency.API.Application.Services
{
	public class SecurityService : ISecurityService
	{
		public interface ISecurityService
		{
			string? AESEncrypt(string content);
			string? AESDecrypt(string content);
		}

		private readonly IConfiguration _configuration;

		public SecurityService(IConfiguration configuration) 
		{
			_configuration = configuration;
		}

		public string? AESEncrypt(string content)
		{
			var aesKey = _configuration["AesConfig:Key"]!;
			var aesIV =_configuration["AesConfig:Iv"]!;

			return SecurityHelper.AESEncrypt(content, aesKey, aesIV);
		}

		public string? AESDecrypt(string content)
		{
			var aesKey = _configuration["AesConfig:Key"]!;
			var aesIV = _configuration["AesConfig:Iv"]!;

			return SecurityHelper.AESDecrypt(content, aesKey, aesIV);
		}
	}
}
