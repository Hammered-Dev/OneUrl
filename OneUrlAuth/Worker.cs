
using OneUrlAuth.Data;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OneUrlAuth;

public class Worker(IServiceProvider iServiceProvider) : IHostedService
{
    private readonly IServiceProvider serviceProvider = iServiceProvider;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync(Environment.GetEnvironmentVariable("CLIENT_ID")!, cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = Environment.GetEnvironmentVariable("CLIENT_ID"),
                ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                DisplayName = "Test App",
                Permissions = {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.ResponseTypes.Code,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                },
                RedirectUris = {
                    new Uri(Environment.GetEnvironmentVariable("REDIRECT_URIS")?? "")
                },
            }, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}