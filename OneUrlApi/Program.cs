using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using OneUrlApi.Api.Manage;
using OneUrlApi.Api.Redirect;
using OneUrlApi.Api.Setting;
using OpenIddict.Validation.AspNetCore;
using Scalar.AspNetCore;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<SecuritySchemeTransformer>();
});

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(Environment.GetEnvironmentVariable("AUTH_ISSUER")!);
        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("DefaultScope", policy =>
    {
        policy.RequireAuthenticatedUser();
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", options =>
    {
        options.AddPreferredSecuritySchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
            .AddAuthorizationCodeFlow(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, flow =>
            {
                flow.ClientId = "console";
                flow.SelectedScopes = ["email", "openid", "profile"];
            });
    });
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World");
app.MapManageEnpoint();
app.MapRedirectEndpoint();
app.MapSettingEndpoint();

app.Run();

internal sealed class SecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                [OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }
    }
}