namespace OneUrlAuth.Endpoints.Connect;

public static class ConnectEndpoint
{
    public static void MapConnectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/connect");

        group.MapGet("/authorize", (Delegate)ConnectEndpointUtils.Authorize);
        group.MapPost("/authorize", (Delegate)ConnectEndpointUtils.Authorize);
        group.MapPost("/token", ConnectEndpointUtils.Exchange);
    }
}