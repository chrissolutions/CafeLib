using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CafeLib.Aspnet.Identity.Secrets
{
	public static class ServiceProviderExtensions
	{
		public static void AddPasswordHasher(this IServiceCollection services)
		{
			services.AddScoped<IPasswordHash, PasswordHash>(x =>
			{
				var options = x.GetRequiredService<IOptions<PasswordHashOptions>>();
				return new PasswordHash(options, new BytesEqualityComparer());
			});
		}
	}
}
