namespace OneUrlApi.Api.Manage;

public static class Manage
{
    public static void MapManageEnpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/manage");

        group.MapGet("/urls", () => "H");
        group.MapPost("/urls", () => "H");
        group.MapPatch("/urls/{id}", (string id) => id);
        group.MapDelete("/urls/{id}", (string id) => id);

    }
}