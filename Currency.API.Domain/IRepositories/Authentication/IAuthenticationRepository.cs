using Currency.API.Domain.IRepositories.Authentication.Model;

namespace Currency.API.Domain.IRepositories.Authentication
{
	public interface IAuthenticationRepository
	{
		Task<GetApplicationOutput?> GetApplication(string appName);
	}
}
