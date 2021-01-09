using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace NTurbo.SignalR
{
    /// <summary>
    /// Static class for <see cref="Hub"/> extension methods.
    /// </summary>
    public static class HubExtensions
    {
        /// <summary>
        /// Renders a partial view by specifying a <paramref name="viewName"/>
        /// and sends the result to the specified <paramref name="hubClient"/>.
        /// </summary>
        /// <param name="hub">The <see cref="Hub"/>.</param>
        /// <param name="hubClient">The client to send the rendered partial view to.</param>
        /// <param name="viewName">The name or path of the partial view that is rendered to the response.</param>
        /// <returns>A <see cref="Task"/> that completes when the rendered partial view has been sent to the client.</returns>
        public static Task SendPartialViewToHubClients<T>(
            this Hub<T> hub,
            ITurboStreamClient hubClient,
            string viewName)
            where T : class, ITurboStreamClient
        {
            return SendPartialViewToHubClients<T>(hub, hubClient, viewName, model: null);
        }

        /// <summary>
        /// Renders a partial view by specifying a <paramref name="viewName"/> and <paramref name="model"/>
        /// and sends the result to the specified <paramref name="hubClient"/>.
        /// </summary>
        /// <param name="hub">The <see cref="Hub"/>.</param>
        /// <param name="hubClient">The client to send the rendered partial view to.</param>
        /// <param name="viewName">The name or path of the partial view that is rendered to the response.</param>
        /// <param name="model">The model that is rendered by the partial view.</param>
        /// <returns>A <see cref="Task"/> that completes when the rendered partial view has been sent to the client.</returns>
        public static Task SendPartialViewToHubClients<T>(
            this Hub<T> hub,
            ITurboStreamClient hubClient,
            string viewName,
            object? model)
            where T : class, ITurboStreamClient
        {
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }

            if (hubClient == null)
            {
                throw new ArgumentNullException(nameof(hubClient));
            }

            if (viewName == null)
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            var httpContext = hub.Context.GetHttpContext();

            if (httpContext == null)
            {
                throw new InvalidOperationException("Connection is not associated with an HTTP request.");
            }

            // TODO Consider providing a better message if this call fails to prompt user to `services.AddNTurbo()`
            var viewHelper = httpContext.RequestServices.GetRequiredService<TurboStreamViewHelper>();

            return viewHelper.SendPartialViewElement(hub.Context, hubClient, viewName, model);
        }
    }
}
