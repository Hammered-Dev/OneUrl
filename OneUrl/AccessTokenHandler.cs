namespace OneUrl;

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

public class AccessTokenHandler(IHttpContextAccessor httpContextAccessor) : 
    DelegatingHandler
{
    private readonly IHttpContextAccessor contextAccessor = httpContextAccessor;
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (contextAccessor.HttpContext is null)
        {
            throw new Exception("HttpContext not available");
        }

        var accessToken = await contextAccessor.HttpContext
            .GetTokenAsync("access_token");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}