using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace NTurbo.SignalR
{
    internal class TurboStreamViewHelper
    {
        private static readonly ActionDescriptor _emptyActionDescriptor = new ActionDescriptor();

        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IOptions<MvcViewOptions> _viewOptions;

        public TurboStreamViewHelper(
            ICompositeViewEngine viewEngine,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
            IOptions<MvcViewOptions> viewOptions)
        {
            _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
            _tempDataDictionaryFactory = tempDataDictionaryFactory ?? throw new ArgumentNullException(nameof(tempDataDictionaryFactory));
            _viewOptions = viewOptions ?? throw new ArgumentNullException(nameof(viewOptions));
        }

        public Task SendPartialViewElement(
            Controller controller,
            ITurboStreamClient client,
            string? viewName,
            object? model)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var actionContext = controller.ControllerContext;
            var tempData = controller.TempData;

            var viewData = new ViewDataDictionary(controller.ViewData)
            {
                Model = model
            };

            return SendPartialViewElement(actionContext, viewData, tempData, client, viewName);
        }

        public Task SendPartialViewElement(
            HubCallerContext hubCallerContext,
            ITurboStreamClient client,
            string viewName,
            object? model)
        {
            if (hubCallerContext == null)
            {
                throw new ArgumentNullException(nameof(hubCallerContext));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (viewName == null)
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            var httpContext = hubCallerContext.GetHttpContext();
            var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), _emptyActionDescriptor);

            var viewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);

            return SendPartialViewElement(actionContext, viewData, tempData, client, viewName);
        }

        private async Task SendPartialViewElement(
            ActionContext actionContext,
            ViewDataDictionary viewData,
            ITempDataDictionary tempData,
            ITurboStreamClient client,
            string? viewName)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            if (tempData == null)
            {
                throw new ArgumentNullException(nameof(tempData));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var viewEngineResult = _viewEngine
                .FindView(actionContext, viewName, isMainPage: false)
                .EnsureSuccessful(originalLocations: null);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewEngineResult.View,
                    viewData,
                    tempData,
                    writer,
                    _viewOptions.Value.HtmlHelperOptions);

                var view = viewEngineResult.View;
                await view.RenderAsync(viewContext);
            }

            var element = sb.ToString();

            await client.ReceiveStreamElement(element);
        }
    }
}
