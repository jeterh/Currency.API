using Currency.API.Application.Coindesk;
using Currency.API.Application.Currency;
using Microsoft.AspNetCore.Mvc;

namespace Currency.API.Controllers
{
	public class CurrencyController : BaseAPIController
	{
		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] GetCurrencysInput input)
		{
			if (input != null && string.IsNullOrEmpty(input.Language))
			{
				input.Language = "en-us";
			}

			var response = await Mediator.Send(input);

			return Ok(response);
		}
	}
}
