using OneUrlApi.Models;

namespace OneUrlApi.Api.Manage;

public static class Manage
{
    public static void MapManageEnpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/manage");

        group.MapGet("/urls", () => ManageEndpointUtils.GetRecords())
            .Produces<UrlRecord?[]>();
        group.MapPost("/urls", (UrlRecord record) => ManageEndpointUtils.ConfigureUrl(record))
            .Produces(204);
        group.MapDelete("/urls/{id}", (string id) => id);

    }
}