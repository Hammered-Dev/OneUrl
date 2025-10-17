using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneUrlApi.Models;
using OneUrlApi.Services;

namespace OneUrlApi.Api.Setting;

static public class SettingsUtil
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
    public static SettingsModel GetSettings()
    {
        if (!RedisService.CheckKeyExists("settings:general"))
        {
            SettingsModel model = new()
            {
                RedirectDelay = 3000,
            };
            SaveSettings(model);
        }
        var data = RedisService.GetHash("settings:general");
        var serilized = JsonSerializer.Serialize(data);
        var settings = JsonSerializer.Deserialize<SettingsModel>(serilized, options) ??
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