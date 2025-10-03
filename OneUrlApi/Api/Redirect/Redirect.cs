using OneUrlApi.Models;

namespace OneUrlApi.Api.Redirect;

public static class Redirect
{
    public static void MapRedirectEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/redirect").WithTags(["Redirect"]);

        group.MapGet("/", RedirectUtils.GetRecord)
            .Produces<UrlRecord>()
            .ProducesProblem(404)
            .AllowAnonymous();
    }
}