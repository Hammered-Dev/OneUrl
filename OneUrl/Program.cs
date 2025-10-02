using OneUrl.Components;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddBlazorBootstrap();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = Environment.GetEnvironmentVariable("AUTH_DOMAIN")!;
    options.ClientId = Environment.GetEnvironmentVariable("AUTH_CLIENTID")!;
    options.ClientSecret = Environment.GetEnvironmentVariable("AUTH_CLIENT_SECRECT");
    options.CallbackPath = new PathString("/Auth/Login");

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ResponseType = OpenIdConnectResponseType.Code;

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.MapInboundClaims = false;
    options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
    options.TokenValidationParameters.RoleClaimType = "role";

    options.Scope.Clear();
    options.Scope.Add(OpenIdConnectScope.OpenId);
    options.Scope.Add(OpenIdConnectScope.Profile);
});

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
