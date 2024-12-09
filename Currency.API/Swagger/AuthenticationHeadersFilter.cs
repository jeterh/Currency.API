using Currency.API.AttributeFilter;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Currency.API.Swagger
{
	public class AuthenticationHeadersFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (context.ApiDescription.CustomAttributes().Any(attr => attr.GetType() == typeof(ApplicationAuthenticationFilter)))
			{
				if (operation.Parameters == null)
					operation.Parameters = new List<OpenApiParameter>();

				operation.Parameters.Add(new OpenApiParameter
				{
					Name = "Client-Id",
					In = ParameterLocation.Header,
					Required = true,
					Example = new Microsoft.OpenApi.Any.OpenApiString("DemoApp")
				});

				operation.Parameters.Add(new OpenApiParameter
				{
					Name = "Client-Secret",
					In = ParameterLocation.Header,
					Required = true,
					Example = new Microsoft.OpenApi.Any.OpenApiString("9de27add-6415-49a0-b39f-b17a6a93cf77")
				});
			}
		}
	}
}
