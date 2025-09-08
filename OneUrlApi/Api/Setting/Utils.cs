using System.Net;
using System.Text.Json;
using OneUrlApi.Models;
using OneUrlApi.Services;

namespace OneUrlApi.Api.Setting;

static public class SettingsUtil
{
    public static SettingsModel GetSettings()
    {
        var data = RedisService.GetHash("settings:general");
        var serilized = JsonSerializer.Serialize(data);
        var settings = JsonSerializer.Deserialize<SettingsModel>(serilized) ??
            throw new HttpRequestException(
                "Invalid value",
                inner: null,
                statusCode: HttpStatusCode.InternalServerError
            );
        return settings;
    }

    public static IResult SaveSettings(SettingsModel settings)
    {
        Dictionary<string, string> settingDict = [];

        foreach (var prop in settings.GetType().GetProperties())
        {
            settingDict.Add(prop.Name, (prop.GetValue(settings) ?? "").ToString() ?? "");
        }

        RedisService.SaveHash("settings:general", settingDict);

        return Results.NoContent();
    }
}