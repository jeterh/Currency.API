using Currency.API.Application.Coindesk;
using Currency.API.AttributeFilter;
using Microsoft.AspNetCore.Mvc;

namespace Currency.API.Controllers
{
	[ApplicationAuthenticationFilter]
	public class CoindeskController : BaseAPIController
	{
		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] GetCoindeskInfoInput query)
		{
			var response = await Mediator.Send(query);

			return Ok(response);
		}
	}
}
