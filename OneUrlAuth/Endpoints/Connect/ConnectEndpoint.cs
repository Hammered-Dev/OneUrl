namespace OneUrlAuth.Endpoints.Connect;

public static class ConnectEndpoint
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/connect");

        group.MapGet("/authorize", ConnectEndpointUtils.Authorize);
    }
}