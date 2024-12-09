using Currency.API.Application.Coindesk;
using Currency.API.AttributeFilter;
using Microsoft.AspNetCore.Mvc;

namespace Currency.API.Controllers
{
	[ApplicationAuthenticationFilter]
	public class CoindeskController : BaseAPIController
	{
		private readonly IConfiguration _configuration;

		public CoindeskController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] GetCoindeskInfoInput input)
		{
			if (input != null && input.TimeInfoId == null)
			{
				input.TimeInfoId = Guid.Parse(_configuration["CurrencyConfig:TimeInfoId"]!);
			}

			var response = await Mediator.Send(input);

			return Ok(response);
		}
	}
}
