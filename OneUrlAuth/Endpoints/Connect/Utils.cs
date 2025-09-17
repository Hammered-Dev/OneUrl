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
    static IResult Deny()
    {
        return Results.Forbid(authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
    }

    static async Task<IResult> Accept(
        HttpContext context,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        UserManager<IdentityUser> userManager
    )
    {
        OpenIddictRequest? request = context.GetOpenIddictClientRequest() ??
            throw new InvalidOperationException("OpenId Connect request failed");

        IdentityUser? user = await userManager.GetUserAsync(context.User) ??
            throw new InvalidOperationException("Getting usr details failed");

        object? application = await applicationManager.FindByClientIdAsync(request.ClientId ?? "") ??
            throw new InvalidOperationException("Client id not found");

        IAsyncEnumerable<object> authorizationList = authorizationManager.FindAsync(
            subject: await userManager.GetUserIdAsync(user),
            client: await applicationManager.GetIdAsync(application),
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: request.GetScopes()
        );

        List<object> authorzations = [];

        await foreach (var auth in authorizationList)
        {
            authorzations.Add(auth);
        }

        if (authorzations.Count is 0 && await applicationManager.HasClientTypeAsync(application, ConsentTypes.External))
        {
            return Results.Forbid(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User is not allowed to access"
                }!)
            );
        }
        ClaimsIdentity identity = new(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role
        );

        identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
            .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
            .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
            .SetClaims(Claims.Role, [.. await userManager.GetRolesAsync(user)]);

        identity.SetScopes(request.GetScopes());

        IAsyncEnumerable<string>? scopeResources = scopeManager.ListResourcesAsync(identity.GetScopes());
        List<string> resources = [];

        await foreach (var resource in scopeResources)
        {
            resources.Add(resource);
        }

        identity.SetResources(resources);

        object? authorization = authorzations.LastOrDefault();
        authorization ??= await authorizationManager.CreateAsync(
            identity: identity,
            subject: await userManager.GetUserIdAsync(user),
            client: await applicationManager.GetClientIdAsync(application) ?? "",
            type: AuthorizationTypes.Permanent,
            scopes: identity.GetScopes()
        );

        identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));

        return Results.SignIn(
            new ClaimsPrincipal(identity),
            authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );
    }

    static async Task<IResult?> VerifyConsent(
        HttpContext context,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        UserManager<IdentityUser> userManager
    )
    {
        if (context.Request.Form.Where(parameter => parameter.Key == "submit.Accept").Any())
        {
            return await Accept(context, applicationManager, authorizationManager, scopeManager, userManager);
        }

        if (context.Request.Form.Where(parameter => parameter.Key == "submit.Deny").Any())
        {
            return Deny();
        }

        return null;
    }

    public static async Task<IResult> Authorize(
        HttpContext context,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        UserManager<IdentityUser> userManager
    )
    {
        var verifiedConsent = await VerifyConsent(context, applicationManager, authorizationManager, scopeManager, userManager);

        if (verifiedConsent is not null) return verifiedConsent;

        OpenIddictRequest request = context.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("Openid connect request failed");

        AuthenticateResult result = await context.AuthenticateAsync();
        if (result is null || !result.Succeeded || request.HasPromptValue(PromptValues.Login))
        {
            if (request.HasPromptValue(PromptValues.None))
            {
                return Results.Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Not logged in"
                    }!)
                );
            }

            string prompt = string.Join(" ", request.GetPromptValues().Remove(PromptValues.Login));

            List<KeyValuePair<string, StringValues>> parameters = context.Request.HasFormContentType ?
                [.. context.Request.Form.Where(parameter => parameter.Key != Parameters.Prompt)] :
                [.. context.Request.Query.Where(parameter => parameter.Key != Parameters.Prompt)];
            parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

            return Results.Challenge(new AuthenticationProperties
            {
                RedirectUri = context.Request.PathBase + context.Request.Path + QueryString.Create(parameters)
            });
        }

        IdentityUser user = await userManager.GetUserAsync(result.Principal) ??
            throw new InvalidOperationException("Getting user detail failed");
        object application = await applicationManager.FindByClientIdAsync(request.ClientId ?? "") ??
            throw new InvalidOperationException("Client application invalid");

        IAsyncEnumerable<object> authorizationList = authorizationManager.FindAsync(
            subject: await userManager.GetUserIdAsync(user),
            client: await applicationManager.GetIdAsync(application),
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: request.GetScopes()
        );

        List<object> authorizations = [];

        await foreach (var authorization in authorizationList)
        {
            authorizations.Add(authorization);
        }

        switch (await applicationManager.GetConsentTypeAsync(application))
        {
            case ConsentTypes.External when authorizations.Count is 0:
                return Results.Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }!)
                );

            case ConsentTypes.Explicit:
            case ConsentTypes.External when authorizations.Count is not 0:
            case ConsentTypes.Implicit when authorizations.Count is not 0 && !request.HasPromptValue(PromptValues.Consent):
                ClaimsIdentity identity = new(
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role
                );

                identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                    .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                    .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                    .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
                    .SetClaims(Claims.Role, [.. await userManager.GetRolesAsync(user)]);

                identity.SetScopes(request.GetScopes());

                IAsyncEnumerable<string>? scopeResources = scopeManager.ListResourcesAsync(identity.GetScopes());
                List<string> recources = [];
                await foreach (var recource in scopeResources)
                {
                    recources.Add(recource);
                }

                object? authorization = authorizations.LastOrDefault();

                authorization ??= await authorizationManager.CreateAsync(
                    identity: identity,
                    subject: await userManager.GetUserIdAsync(user),
                    client: await applicationManager.GetClientIdAsync(application) ?? "",
                    type: AuthorizationTypes.Permanent,
                    scopes: identity.GetScopes()
                );

                identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
                // identity.SetDestinations(GetDestinations)

                return Results.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
                return Results.Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Interactive user consent is required"
                    }!)
                );

            default:
                string jsonData = $"{{  \"applicationName\": \"{await applicationManager.GetLocalizedDisplayNameAsync(application)}\", \"scope\": \"{request.Scope}\"  }}";
                context.Session.SetString("ConsentData", jsonData);
                IEnumerable<KeyValuePair<string, StringValues>> parameters = context.Request.HasFormContentType ?
                    context.Request.Form : context.Request.Query;
                return Results.Redirect($"/Consent{QueryString.Create(parameters)}");
        }
    }
}