namespace OneUrl;

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

public class AccessTokenHandler(IHttpContextAccessor contextAccessor) : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor = contextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;

        var accessToken = await httpContext!.GetTokenAsync("access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        return await base.SendAsync(request, cancellationToken);
    }
}