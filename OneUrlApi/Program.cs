using OneUrlApi.Api.Manage;
using OneUrlApi.Api.Redirect;
using OneUrlApi.Api.Setting;
using OpenIddict.Validation.AspNetCore;
using Scalar.AspNetCore;

DotNetEnv.Env.Load();
Console.WriteLine(Environment.GetEnvironmentVariable("AUTH_ISSUER")?? "null........");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

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
    app.MapScalarApiReference("/docs");
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World");
app.MapManageEnpoint();
app.MapRedirectEndpoint();
app.MapSettingEndpoint();

app.Run();