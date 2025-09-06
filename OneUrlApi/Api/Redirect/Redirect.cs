namespace OneUrlApi.Api.Redirect;

public static class Redirect
{
    public static void MapRedirectEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/redirect");

        group.MapGet("/", () => "Hello");
    }
}