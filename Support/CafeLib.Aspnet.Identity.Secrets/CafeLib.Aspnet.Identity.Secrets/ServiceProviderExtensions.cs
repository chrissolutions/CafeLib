using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CafeLib.Aspnet.Identity.Secrets
{
	public static class ServiceProviderExtensions
	{
		public static void AddPasswordHasher(this IServiceCollection services)
		{
			services.AddScoped<IPasswordHasher, PasswordHasher>(x =>
			{
				var options = x.GetRequiredService<IOptions<PasswordHasherOptions>>();
				return new PasswordHasher(options, new BytesEqualityComparer());
			});
		}
	}
}
