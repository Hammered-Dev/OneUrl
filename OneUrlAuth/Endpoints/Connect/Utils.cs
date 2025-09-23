using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Primitives;


namespace OneUrlAuth.Endpoints.Connect;

public static class ConnectEndpointUtils
{
    public static async Task<IResult> Authorize(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest();
        if (request is null)
        {
            return Results.BadRequest();
        }

        var result = await context.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (!result.Succeeded)
        {
            return Results.Challenge(
                authenticationSchemes: [IdentityConstants.ApplicationScheme],
                properties: new AuthenticationProperties
                {
                    RedirectUri =
                        context.Request.PathBase +
                        context.Request.Path +
                        context.Request.QueryString
                }
            );
        }

        var principal = result.Principal;

        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            Claims.Subject,
            Claims.Role
        );
        identity.AddClaim(Claims.Subject, principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        identity.AddClaims(principal.Claims);

        var newPricipal = new ClaimsPrincipal(identity);
        return Results.SignIn(newPricipal, null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    public static async Task<IResult> Exchange(
        HttpContext context,
        IOpenIddictApplicationManager applicationManager
    )
    {
        var request = context.GetOpenIddictServerRequest();

        if (request!.IsAuthorizationCodeGrantType())
        {
            var application = await applicationManager.FindByClientIdAsync(request?.ClientId ?? "") ??
                throw new InvalidOperationException("Application not found");

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType);

            identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application));
            identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));

            identity.SetDestinations(static claim => claim.Type switch
            {
                Claims.Name when claim.Subject!.HasScope(Scopes.Profile)
                    => [Destinations.AccessToken, Destinations.IdentityToken],

                _ => [Destinations.AccessToken]
            });

            return Results.SignIn(new ClaimsPrincipal(identity), properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new NotImplementedException("The specified grant is not implemented.");
    }
}