using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace NTurbo.TagHelpers
{
    /// <summary>
    /// A <see cref="ITagHelper"/> implementation targeting &lt;turbo-frame&gt; elements.
    /// </summary>
    [HtmlTargetElement(TagName, Attributes = ActionAttributeName)]
    [HtmlTargetElement(TagName, Attributes = ControllerAttributeName)]
    [HtmlTargetElement(TagName, Attributes = AreaAttributeName)]
    [HtmlTargetElement(TagName, Attributes = PageAttributeName)]
    [HtmlTargetElement(TagName, Attributes = PageHandlerAttributeName)]
    [HtmlTargetElement(TagName, Attributes = HostAttributeName)]
    [HtmlTargetElement(TagName, Attributes = ProtocolAttributeName)]
    [HtmlTargetElement(TagName, Attributes = RouteAttributeName)]
    [HtmlTargetElement(TagName, Attributes = RouteValuesDictionaryName)]
    [HtmlTargetElement(TagName, Attributes = RouteValuesPrefix + "*")]
    public class TurboFrameTagHelper : TagHelper
    {
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private const string HostAttributeName = "asp-host";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string RouteAttributeName = "asp-route";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";
        private const string IdAttributeName = "id";
        private const string SrcAttributeName = "src";
        private const string TargetAttributeName = "target";
        private const string TagName = "turbo-frame";

        private readonly IUrlHelperFactory _urlHelperFactory;

        private IDictionary<string, string>? _routeValues;

        /// <summary>
        /// Creates a new <see cref="TurboFrameTagHelper"/>.
        /// </summary>
        /// <param name="urlHelperFactory">The <see cref="IUrlHelperFactory"/>.</param>
        public TurboFrameTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory ?? throw new ArgumentNullException(nameof(urlHelperFactory));
        }

        /// <summary>
        /// The name of the action method.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> or <see cref="Page"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(ActionAttributeName)]
        public string? Action { get; set; }

        /// <summary>
        /// The name of the controller.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> or <see cref="Page"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(ControllerAttributeName)]
        public string? Controller { get; set; }

        /// <summary>
        /// The name of the area.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(AreaAttributeName)]
        public string? Area { get; set; }

        /// <summary>
        /// The name of the page.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> or <see cref="Action"/>, <see cref="Controller"/>
        /// is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(PageAttributeName)]
        public string? Page { get; set; }

        /// <summary>
        /// The name of the page handler.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> or <see cref="Action"/>, <see cref="Controller"/>
        /// is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(PageHandlerAttributeName)]
        public string? PageHandler { get; set; }

        /// <summary>
        /// The protocol for the URL, such as &quot;http&quot; or &quot;https&quot;.
        /// </summary>
        [HtmlAttributeName(ProtocolAttributeName)]
        public string? Protocol { get; set; }

        /// <summary>
        /// The host name.
        /// </summary>
        [HtmlAttributeName(HostAttributeName)]
        public string? Host { get; set; }

        /// <summary>
        /// Name of the route.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if one of <see cref="Action"/>, <see cref="Controller"/>, <see cref="Area"/> 
        /// or <see cref="Page"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(RouteAttributeName)]
        public string? Route { get; set; }

        /// <summary>
        /// Additional parameters for the route.
        /// </summary>
        [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string>? RouteValues
        {
            get
            {
                if (_routeValues == null)
                {
                    _routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                return _routeValues;
            }
            set
            {
                _routeValues = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ViewContext"/> for the current request.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        [DisallowNull]
        public ViewContext? ViewContext { get; set; }

        /// <inheritdoc/>
        public override int Order => -1000;

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (output.Attributes.ContainsName(SrcAttributeName))
            {
                if (Action != null ||
                    Controller != null ||
                    Area != null ||
                    Page != null ||
                    PageHandler != null ||
                    Route != null ||
                    Protocol != null ||
                    Host != null ||
                    (_routeValues != null && _routeValues.Count > 0))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Cannot override the '{0}' attribute for {1}. An {1} with a specified '{0}' must not have attributes starting with '{2}' or an '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', or '{10}' attribute.",
                            SrcAttributeName,
                            $"<{TagName}>",
                            RouteValuesPrefix,
                            ActionAttributeName,
                            ControllerAttributeName,
                            AreaAttributeName,
                            RouteAttributeName,
                            ProtocolAttributeName,
                            HostAttributeName,
                            PageAttributeName,
                            PageHandlerAttributeName));
                }

                return;
            }

            var routeLink = Route != null;
            var actionLink = Controller != null || Action != null;
            var pageLink = Page != null || PageHandler != null;

            if ((routeLink && actionLink) || (routeLink && pageLink) || (actionLink && pageLink))
            {
                var message = string.Join(
                    Environment.NewLine,
                    string.Format(
                        "Cannot determine the '{0}' attribute for {1}. The following attributes are mutually exclusive:",
                        SrcAttributeName,
                        $"<{TagName}>"),
                    RouteAttributeName,
                    ControllerAttributeName + ", " + ActionAttributeName,
                    PageAttributeName + ", " + PageHandlerAttributeName);

                throw new InvalidOperationException(message);
            }

            RouteValueDictionary? routeValues = null;
            if (_routeValues != null && _routeValues.Count > 0)
            {
                routeValues = new RouteValueDictionary(_routeValues);
            }

            if (Area != null)
            {
                routeValues ??= new RouteValueDictionary();
                routeValues["area"] = Area;
            }

            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            string src;

            if (pageLink)
            {
                src = urlHelper.PageLink(Page, PageHandler, routeValues, Protocol, Host);
            }
            else if (routeLink)
            {
                src = urlHelper.RouteUrl(Route, routeValues, Protocol, Host);
            }
            else  //actionLink
            {
                src = urlHelper.Action(Action, Controller, routeValues, Protocol, Host);
            }

            output.Attributes.Add(SrcAttributeName, src);
        }
    }
}
