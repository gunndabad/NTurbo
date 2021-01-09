using System;
using Microsoft.Extensions.DependencyInjection;
using NTurbo.SignalR;

namespace NTurbo
{
    /// <summary>
    /// Extension methods for setting up NTurbo services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds NTurbo services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddNTurbo(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<TurboStreamViewHelper>();

            return services;
        }
    }
}
