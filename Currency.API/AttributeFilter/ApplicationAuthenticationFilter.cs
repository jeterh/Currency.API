using Currency.API.Application.Authentication.Queries;
using Currency.API.Application.Common;
using Currency.API.Utilities.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Currency.API.AttributeFilter
{
	public class ApplicationAuthenticationFilter : Attribute, IAsyncAuthorizationFilter
	{
		private IMediator _mediator = null!;
		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			_mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();

			if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
			{
				return;
			}

			var headers = context.HttpContext.Request.Headers;
			if (!headers.ContainsKey("Client-Id") || !headers.ContainsKey("Client-Secret"))
			{
				Unauthorized(context);
				return;
			}

			var appName = headers["Client-Id"].First() ?? string.Empty;
			var secretKey = headers["Client-Secret"].First() ?? string.Empty;

			var data = await _mediator.Send(new GetAuthenticationQuery()
			{
				ApplicationName = appName,
				SecretKey = secretKey
			});

			if (data == null || !data!.VerificationStatus ) 
			{
				Unauthorized(context);
				return;
			}

			context.HttpContext.User = new ClaimsPrincipal(new[]
{
				new ClaimsIdentity(new List<Claim>()
				{
					new Claim("ApplicationName",  data.ApplicationName)
				})
			});
		}

		private static void Unauthorized(AuthorizationFilterContext context)
		{
			var response = new ResponseModel()
			{
				ReturnCode = ReturnCodeEnum.AuthenticationFail.GetFourDigitReturnCode(),
				Message = ReturnCodeEnum.AuthenticationFail.GetDescription(),
			};
			context.Result = new ObjectResult(response)
			{
				StatusCode = StatusCodes.Status401Unauthorized
			};
		}
	}
}
