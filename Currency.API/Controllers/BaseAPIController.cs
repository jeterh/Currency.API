using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Currency.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BaseAPIController : ControllerBase
	{
		protected ISender Mediator => HttpContext.RequestServices.GetRequiredService<ISender>();
	}
}
