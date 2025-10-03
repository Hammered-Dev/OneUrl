using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OneUrlApi.Api.Manage;
using OneUrlApi.Api.Redirect;
using OneUrlApi.Api.Setting;
using Scalar.AspNetCore;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.Authority = Environment.GetEnvironmentVariable("AUTH_ISSUER");
        jwtOptions.Audience = Environment.GetEnvironmentVariable("AUTH_AUDIENCE");

        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireClaim("scope").Build());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");
app.MapManageEnpoint();
app.MapRedirectEndpoint();
app.MapSettingEndpoint();

app.Run();