using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NTurbo.SignalR;
using NTurbo.Tests.Infrastructure;
using Xunit;

namespace NTurbo.Tests
{
    public class TurboStreamViewHelperAcceptanceTests : IClassFixture<TurboStreamViewHelperTestsFixture>
    {
        public TurboStreamViewHelperAcceptanceTests(TurboStreamViewHelperTestsFixture fixture)
        {
            Fixture = fixture;
            TurboStreamViewHelper = Fixture.Host.Services.GetRequiredService<TurboStreamViewHelper>();
        }

        private TurboStreamViewHelperTestsFixture Fixture { get; }

        private HttpClient HttpClient => Fixture.HttpClient;

        private TurboStreamViewHelper TurboStreamViewHelper { get; }

        [Fact]
        public async Task SendPartialViewElement_FromController_SendsViewToClients()
        {
            // Arrange
            var elements = new List<string>();
            var receivedMessage = new SemaphoreSlim(0);

            await using var connection = Fixture.CreateHubConnection();

            connection.On<string>(nameof(ITurboStreamClient.ReceiveStreamElement), msg =>
            {
                elements.Add(msg);
                receivedMessage.Release();
            });

            await connection.StartAsync();

            var message = "test message";

            // Act
            await HttpClient.GetAsync($"TurboStreamViewHelperAcceptanceTests?model={UrlEncoder.Default.Encode(message)}");

            // Assert
            await receivedMessage.WaitAsync(millisecondsTimeout: 100);

            Assert.Collection(
                elements,
                element =>
                {
                    Assert.Contains("<turbo-stream", element);
                    Assert.Contains($@"<div data-testid=""message"">{message}</div>", element);
                });
        }

        [Fact]
        public async Task SendPartialViewElement_FromHub_SendsViewToClients()
        {
            // Arrange
            var elements = new List<string>();
            var receivedMessage = new SemaphoreSlim(0);

            await using var connection = Fixture.CreateHubConnection();

            connection.On<string>(nameof(ITurboStreamClient.ReceiveStreamElement), msg =>
            {
                elements.Add(msg);
                receivedMessage.Release();
            });

            await connection.StartAsync();

            var message = "test message";

            // Act
            await connection.InvokeAsync("SendMessage", message);

            // Assert
            await receivedMessage.WaitAsync(millisecondsTimeout: 100);

            Assert.Collection(
                elements,
                element =>
                {
                    Assert.Contains("<turbo-stream", element);
                    Assert.Contains($@"<div data-testid=""message"">{message}</div>", element);
                });
        }
    }

    public class TurboStreamViewHelperTestsFixture : IDisposable
    {
        public const string HubEndpoint = "hubs/turbo-stream";

        public TurboStreamViewHelperTestsFixture()
        {
            ViewFileProvider = new DictionaryFileProvider();

            AddTestViews();

            Host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services
                                .AddMvc()
                                .AddRazorRuntimeCompilation(options =>
                                {
                                    options.FileProviders.Add(ViewFileProvider);
                                });

                            services.AddSignalR();
                            services.AddNTurbo();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                                endpoints.MapHub<TurboStreamHub>("/" + HubEndpoint);
                            });
                        });
                })
                .Start();

            HttpClient = Host.GetTestClient();
        }

        private void AddTestViews()
        {
            var testViewContents = @"
@model string

<turbo-stream target=""messages"" action=""append"">
    <template>
        <div data-testid=""message"">@Model</div>
    </template>
</turbo-stream>";

            ViewFileProvider.Add("/Views/Shared/_Test.cshtml", testViewContents);
        }

        public HubConnection CreateHubConnection()
        {
            var testServer = Host.GetTestServer();

            return new HubConnectionBuilder()
                .WithUrl(
                    $"{testServer.BaseAddress}{HubEndpoint}",
                    o => o.HttpMessageHandlerFactory = _ => testServer.CreateHandler())
                .Build();
        }

        public DictionaryFileProvider ViewFileProvider { get; }

        public IHost Host { get; }

        public HttpClient HttpClient { get; }

        public void Dispose()
        {
            HttpClient.Dispose();
            Host.Dispose();
        }
    }

    public class TurboStreamHub : Hub<ITurboStreamClient>
    {
        public async Task SendMessage(string message)
        {
            await this.SendPartialViewToHubClients(Clients.All, "_Test", message);
        }
    }

    [Route("TurboStreamViewHelperAcceptanceTests")]
    public class TurboStreamViewHelperAcceptanceTestsController : Controller
    {
        private readonly IHubContext<TurboStreamHub, ITurboStreamClient> _hubContext;

        public TurboStreamViewHelperAcceptanceTestsController(
            IHubContext<TurboStreamHub, ITurboStreamClient> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> SendView([FromQuery] string model)
        {
            await this.SendPartialViewToHubClients(
                _hubContext.Clients.All,
                "_Test",
                model);

            return Ok();
        }
    }
}
