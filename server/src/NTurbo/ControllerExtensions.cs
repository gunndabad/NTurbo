using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NTurbo.SignalR;

namespace NTurbo
{
    /// <summary>
    /// Static class for <see cref="Controller"/> extension methods.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Renders a partial view and sends the result to the specified <paramref name="hubClient"/>.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="hubClient">The client to send the rendered partial view to.</param>
        /// <returns>A <see cref="Task"/> that completes when the rendered partial view has been sent to the client.</returns>
        public static Task SendPartialViewToHubClients(
            this Controller controller,
            ITurboStreamClient hubClient)
        {
            return SendPartialViewToHubClients(controller, hubClient, viewName: null, model: null);
        }

        /// <summary>
        /// Renders a partial view by specifying a <paramref name="viewName"/>
        /// and sends the result to the specified <paramref name="hubClient"/>.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="hubClient">The client to send the rendered partial view to.</param>
        /// <param name="viewName">The name or path of the partial view that is rendered to the response.</param>
        /// <returns>A <see cref="Task"/> that completes when the rendered partial view has been sent to the client.</returns>
        public static Task SendPartialViewToHubClients(
            this Controller controller,
            ITurboStreamClient hubClient,
            string? viewName)
        {
            return SendPartialViewToHubClients(controller, hubClient, viewName, model: null);
        }

        /// <summary>
        /// Renders a partial view by specifying the <paramref name="model"/>
        /// and sends the result to the specified <paramref name="hubClient"/>.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="hubClient">The client to send the rendered partial view to.</param>
        /// <param name="model">The model that is rendered by the partial view.</param>
        /// <returns>A <see cref="Task"/> that completes when the rendered partial view has been sent to the client.</returns>
        public static Task SendPartialViewToHubClients(
            this Controller controller,
            ITurboStreamClient hubClient,
            object? model)
        {
            return SendPartialViewToHubClients(controller, hubClient, viewName: null, model);
        }

        /// <summary>
        /// Renders a partial view by specifying a <paramref name="viewName"/> and <paramref name="model"/>
        /// and sends the result to the specified <paramref name="hubClient"/>.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="hubClient">The client to send the rendered partial view to.</param>
        /// <param name="viewName">The name or path of the partial view that is rendered to the response.</param>
        /// <param name="model">The model that is rendered by the partial view.</param>
        /// <returns>A <see cref="Task"/> that completes when the rendered partial view has been sent to the client.</returns>
        public static Task SendPartialViewToHubClients(
            this Controller controller,
            ITurboStreamClient hubClient,
            string? viewName,
            object? model)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (hubClient == null)
            {
                throw new ArgumentNullException(nameof(hubClient));
            }

            // TODO Consider providing a better message if this call fails to prompt user to `services.AddNTurbo()`
            var viewHelper = controller.HttpContext.RequestServices.GetRequiredService<TurboStreamViewHelper>();

            return viewHelper.SendPartialViewElement(controller, hubClient, viewName, model);
        }

        /// <summary>
        /// Creates a <see cref="TurboStreamPartialViewResult"/> object that renders a partial view to the response.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <returns>The created <see cref="TurboStreamPartialViewResult"/> object for the response.</returns>
        public static TurboStreamPartialViewResult TurboStreamPartialView(this Controller controller)
        {
            return TurboStreamPartialView(controller, viewName: null);
        }

        /// <summary>
        /// Creates a <see cref="TurboStreamPartialViewResult"/> object by specifying a <paramref name="viewName"/>.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="viewName">The name or path of the partial view that is rendered to the response.</param>
        /// <returns>The created <see cref="TurboStreamPartialViewResult"/> object for the response.</returns>
        public static TurboStreamPartialViewResult TurboStreamPartialView(
            this Controller controller,
            string? viewName)
        {
            return TurboStreamPartialView(controller, viewName, model: null);
        }

        /// <summary>
        /// Creates a <see cref="TurboStreamPartialViewResult"/> object by specifying
        /// the <paramref name="model"/> to be rendered by the partial view.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="model">The model that is rendered by the partial view.</param>
        /// <returns>The created <see cref="TurboStreamPartialViewResult"/> object for the response.</returns>
        public static TurboStreamPartialViewResult TurboStreamPartialView(
            this Controller controller,
            object? model)
        {
            return TurboStreamPartialView(controller, viewName: null, model);
        }

        /// <summary>
        /// Creates a <see cref="TurboStreamPartialViewResult"/> object by specifying a <paramref name="viewName"/>
        /// and the <paramref name="model"/> to be rendered by the partial view.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="viewName">The name or path of the partial view that is rendered to the response.</param>
        /// <param name="model">The model that is rendered by the partial view.</param>
        /// <returns>The created <see cref="TurboStreamPartialViewResult"/> object for the response.</returns>
        public static TurboStreamPartialViewResult TurboStreamPartialView(
            this Controller controller,
            string? viewName,
            object? model)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            controller.ViewData.Model = model;

            return new TurboStreamPartialViewResult()
            {
                ViewName = viewName,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }
    }
}
