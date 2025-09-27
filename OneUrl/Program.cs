using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.IdentityModel.Tokens;
using OneUrl;
using OneUrl.Components;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AccessTokenHandler>();
builder.Services.AddHttpClient("Test", options => options.BaseAddress = new Uri(Environment.GetEnvironmentVariable("AUTH_SERVER")!))
    .AddHttpMessageHandler<AccessTokenHandler>();
builder.Services.AddBlazorBootstrap();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie("Cookies")
.AddOpenIdConnect(option =>
{
    option.Authority = Environment.GetEnvironmentVariable("AUTH_SERVER");
    option.ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");
    option.ResponseType = "code";
    option.Scope.Add("openid");
    option.Scope.Add("profile");
    option.Scope.Add("email");
    option.CallbackPath = "/Auth/login";

    option.SaveTokens = true;
    option.GetClaimsFromUserInfoEndpoint = true;

    option.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };
});

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
