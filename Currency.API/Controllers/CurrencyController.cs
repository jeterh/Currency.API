using Currency.API.Application.Coindesk;
using Currency.API.Application.Currency;
using Currency.API.AttributeFilter;
using Microsoft.AspNetCore.Mvc;

namespace Currency.API.Controllers
{
	[ApplicationAuthenticationFilter]
	public class CurrencyController : BaseAPIController
	{
		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] GetCurrencysInput input)
		{
			if (input != null && string.IsNullOrEmpty(input.Language))
			{
				input.Language = "en-us";
			}

			var response = await Mediator.Send(input!);

			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateCurrencyBpiLanguageInput request)
		{
			var response = await Mediator.Send(request);

			return Ok(response);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] UpdateCurrencyBpiInput request)
		{
			var response = await Mediator.Send(request);

			return Ok(response);
		}

		[HttpDelete]
		public async Task<IActionResult> Delete([FromQuery] DeleteCurrencyBpiLanguageInput request)
		{
			var response = await Mediator.Send(request);

			return Ok(response);
		}

		[HttpPost("CreateContent")]
		public async Task<IActionResult> CreateContent([FromBody] CreateCurrencyContentInput request)
		{
			var response = await Mediator.Send(request);

			return Ok(response);
		}

		[HttpPut("UpdateContent")]
		public async Task<IActionResult> UpdateContent([FromBody] UpdateCurrencyContentInput request)
		{
			var response = await Mediator.Send(request);

			return Ok(response);
		}

		[HttpDelete("DeleteContent")]
		public async Task<IActionResult> DeleteContent([FromQuery] DeleteCurrencyContentInput request)
		{
			var response = await Mediator.Send(request);

			return Ok(response);
		}
	}
}
