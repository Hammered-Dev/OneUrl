namespace OneUrlApi.Api.Manage;

using OneUrlApi.Models;
using OneUrlApi.Services;

public static class ManageEndpointUtils
{
    private static readonly string RedisPrefix = "records:";
    public static IResult ConfigureUrl(UrlRecord record)
    {
        Dictionary<string, string> pairs = new()
        {
            { "Target", record.Target },
            { "Location", record.Location }
        };
        RedisService.SaveHash($"{RedisPrefix}{record.Target}", pairs);
        return Results.NoContent();
    }

    public static UrlRecord?[] GetRecords()
    {
        return RedisService.IndexUrl();
    }

    public static IResult Delete(string id)
    {
        RedisService.Delete($"{RedisPrefix}{id}");
        return Results.NoContent();
    }
}