namespace OneUrlApi.Api.Manage;

using System.Reflection;
using OneUrlApi.Models;
using OneUrlApi.Services;
using StackExchange.Redis;

public static class ManageEndpointUtils
{
    private static readonly string RedisPrefix = "records:";
    public static IResult ConfigureUrl(UrlRecord record)
    {
        Dictionary<string, string> pairs = new()
        {
            { "Target", record.Target },
            { "location", record.Location }
        };
        RedisService.SaveHash($"{RedisPrefix}{record.Target}", pairs);
        return Results.StatusCode(204);
    }

    public static UrlRecord?[] GetRecords()
    {
        return RedisService.IndexUrl();
    }
}