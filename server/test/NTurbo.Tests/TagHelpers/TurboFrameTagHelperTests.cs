using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using NTurbo.TagHelpers;
using Xunit;

namespace NTurbo.Tests.TagHelpers
{
    public class TurboFrameTagHelperTests
    {
        [Theory]
        [InlineData("action", null, null, null, null, null, null, null, null)]
        [InlineData(null, "controller", null, null, null, null, null, null, null)]
        [InlineData(null, null, "area", null, null, null, null, null, null)]
        [InlineData(null, null, null, "host", null, null, null, null, null)]
        [InlineData(null, null, null, null, "page", null, null, null, null)]
        [InlineData(null, null, null, null, null, "pageHandler", null, null, null)]
        [InlineData(null, null, null, null, null, null, "protocol", null, null)]
        [InlineData(null, null, null, null, null, null, null, "route", null)]
        public async Task ProcessAsync_AlreadyHaveSrcAttributeAndRouteAttributeSpecified_ThrowsInvalidOperationException(
            string action,
            string controller,
            string area,
            string host,
            string page,
            string pageHandler,
            string protocol,
            string route,
            IDictionary<string, string> routeValues)
        {
            // Arrange
            var context = new TagHelperContext(
                tagName: "turbo-frame",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var output = new TagHelperOutput(
                "turbo-frame",
                attributes: new TagHelperAttributeList()
                {
                    { "src", "/src" }
                },
                getChildContentAsync: (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            var urlHelperMock = new Mock<IUrlHelper>();

            var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
            urlHelperFactoryMock
                .Setup(mock => mock.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelperMock.Object);

            var viewContext = new ViewContext();

            var tagHelper = new TurboFrameTagHelper(urlHelperFactoryMock.Object)
            {
                Action = action,
                Area = area,
                Controller = controller,
                Host = host,
                Page = page,
                PageHandler = pageHandler,
                Protocol = protocol,
                Route = route,
                RouteValues = routeValues,
                ViewContext = viewContext
            };

            // Act
            var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

            // Assert
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal(
                "Cannot override the 'src' attribute for <turbo-frame>. An <turbo-frame> with a specified 'src' must not have attributes starting with 'asp-route-' or an 'asp-action', 'asp-controller', 'asp-area', 'asp-route', 'asp-protocol', 'asp-host', 'asp-page', or 'asp-page-handler' attribute.",
                ex.Message);
        }
    }
}
