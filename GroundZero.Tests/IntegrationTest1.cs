using System.Net.Http.Json;
using Projects;

namespace GroundZero.Tests
{
    public class IntegrationTest1
    {
        // Instructions:
        // 1. Add a project reference to the target AppHost project, e.g.:
        //
        //    <ItemGroup>
        //        <ProjectReference Include="../MyAspireApp.AppHost/MyAspireApp.AppHost.csproj" />
        //    </ItemGroup>
        //
        // 2. Uncomment the following example test and update 'Projects.MyAspireApp_AppHost' to match your AppHost project:
        //
        [Fact]
        public async Task LoginWithAdminUser()
        {
            // Arrange
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<GroundZero_AppHost>();
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

            await using var app = await appHost.BuildAsync();
            var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
            await app.StartAsync();

            // Act
            var httpClient = app.CreateHttpClient("api");
            await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
            var response = await httpClient.PostAsJsonAsync("/login", new
            {
                email = "admin@groundzero.dev",
                password = "admin"
            });

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
