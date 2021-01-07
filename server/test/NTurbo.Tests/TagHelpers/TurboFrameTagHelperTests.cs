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
        [Fact]
        public async Task ProcessAsync_PassesIdToOutput()
        {
            // Arrange
            var context = new TagHelperContext(
                tagName: "turbo-frame",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var output = new TagHelperOutput(
                "turbo-frame",
                attributes: new TagHelperAttributeList(),
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
                Id = "test",
                ViewContext = viewContext
            };

            // Act
            await tagHelper.ProcessAsync(context, output);

            // Assert
            Assert.True(output.Attributes.ContainsName("id"));
            Assert.Equal("test", output.Attributes["id"].Value);
        }

        [Fact]
        public async Task ProcessAsync_WithNoSrcAttribute_DoesNotSetSrcAttributeOnOutput()
        {
            // Arrange
            var context = new TagHelperContext(
                tagName: "turbo-frame",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var output = new TagHelperOutput(
                "turbo-frame",
                attributes: new TagHelperAttributeList(),
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
                Id = "test",
                ViewContext = viewContext
            };

            // Act
            await tagHelper.ProcessAsync(context, output);

            // Assert
            Assert.False(output.Attributes.ContainsName("src"));
        }

        [Fact]
        public async Task ProcessAsync_WithSrcAttribute_PassesSrcToOutput()
        {
            // Arrange
            var context = new TagHelperContext(
                tagName: "turbo-frame",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var output = new TagHelperOutput(
                "turbo-frame",
                attributes: new TagHelperAttributeList(),
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
                Id = "test",
                Src = "/src",
                ViewContext = viewContext
            };

            // Act
            await tagHelper.ProcessAsync(context, output);

            // Assert
            Assert.True(output.Attributes.ContainsName("src"));
            Assert.Equal("/src", output.Attributes["src"].Value);
        }

        [Fact]
        public async Task ProcessAsync_MissingId_ThrowsInvalidOperationException()
        {
            // Arrange
            var context = new TagHelperContext(
                tagName: "turbo-frame",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var output = new TagHelperOutput(
                "turbo-frame",
                attributes: new TagHelperAttributeList(),
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
                ViewContext = viewContext
            };

            // Act
            var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

            // Assert
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("The 'id' attribute cannot be empty.", ex.Message);
        }
    }
}
