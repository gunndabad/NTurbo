using System;
using Microsoft.AspNetCore.Mvc;

namespace NTurbo
{
    /// <summary>
    /// Static class for <see cref="Controller"/> extension methods.
    /// </summary>
    public static class ControllerExtensions
    {
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
