using Currency.API.Application.Common;
using Currency.API.Utilities.Extensions;

namespace Currency.API.Middlewares
{
	public class ExceptionCatchMiddleware : IMiddleware
	{

		public ExceptionCatchMiddleware()
		{
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				var errorAction = new ResponseModel();
				errorAction.IsSuccess = false;
				errorAction.Message = ex.Message;
				errorAction.ReturnCode = ReturnCodeEnum.UnKnown.GetFourDigitReturnCode();

				context.Response.StatusCode = StatusCodes.Status200OK;
				await context.Response.WriteAsJsonAsync(errorAction);
			}
		}
	}
}
