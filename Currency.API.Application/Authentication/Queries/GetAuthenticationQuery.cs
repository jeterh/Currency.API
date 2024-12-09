using Currency.API.Application.Services;
using Currency.API.Domain.IRepositories.Authentication;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Currency.API.Application.Services.SecurityService;

namespace Currency.API.Application.Authentication.Queries
{
	public class GetAuthenticationQuery : IRequest<GetAuthentication>
	{
		public string ApplicationName { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
	}

	public class GetAuthentication
	{
		public string ApplicationName { get; set; } = null!;
        public bool VerificationStatus { get; set; }
	}

	public class GetAuthenticationQueryHandler : IRequestHandler<GetAuthenticationQuery, GetAuthentication?>
	{
		private readonly IAuthenticationRepository _authenticationRepository;
		private readonly ISecurityService _securityService;


		public GetAuthenticationQueryHandler(IAuthenticationRepository authenticationRepository, ISecurityService securityService)
		{
			_authenticationRepository = authenticationRepository;
			_securityService = securityService;
		}

		public async Task<GetAuthentication?> Handle(GetAuthenticationQuery request, CancellationToken cancellationToken)
		{

			var data = await _authenticationRepository.GetApplication(request.ApplicationName);

			if (data is null)
			{
				return new GetAuthentication
				{
					VerificationStatus = false
				};
			}
			
			var encryptSecretKey = _securityService.AESEncrypt(request.SecretKey);
			
			if (!string.Equals(data.SecretKey, encryptSecretKey))
			{
				return new GetAuthentication
				{
					VerificationStatus = false
				};
			}

			var response = new GetAuthentication
			{
				VerificationStatus = true,
				ApplicationName = data.Name
			};

			return response;
		}
	}
}
