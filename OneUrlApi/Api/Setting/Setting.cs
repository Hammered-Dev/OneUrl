using OneUrlApi.Models;

namespace OneUrlApi.Api.Setting;

public static class Setting
{
    public static void MapSettingEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/setting").WithTags(["Settings"]);

        group.MapPost("/", () => "H");
        group.MapGet("/", SettingsUtil.GetSettings)
            .Produces<SettingsModel>();
    }
}