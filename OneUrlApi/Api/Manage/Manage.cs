using OneUrlApi.Models;

namespace OneUrlApi.Api.Manage;

public static class Manage
{
    public static void MapManageEnpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/manage").WithTags(["Manage"]);

        group.MapGet("/urls", () => ManageEndpointUtils.GetRecords())
            .RequireAuthorization("DefaultScope")
            .Produces<UrlRecord?[]>();
        group.MapPost("/urls", (UrlRecord record) => ManageEndpointUtils.ConfigureUrl(record))
            .RequireAuthorization("DefaultScope")
            .Produces(204);
        group.MapDelete("/urls/{id}", (string id) => ManageEndpointUtils.Delete(id))
            .RequireAuthorization("DefaultScope")
            .Produces(204);

    }
}