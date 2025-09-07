using System.Net;
using StackExchange.Redis;

namespace OneUrlApi.Services;

public static class RedisService
{
    private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
    private static readonly IDatabase database = redis.GetDatabase();

    public static void SaveString(string key, string value)
    {
        database.StringSet(key, value);
    }

    public static string GetString(string key)
    {
        var value = database.StringGet(key);

        if (value == RedisValue.Null || value.IsNull)
        {
            throw new HttpRequestException("Key not found", inner: null, statusCode: HttpStatusCode.NotFound);
        }

        return value.ToString();
    }

    public static void SaveHash(string key, Dictionary<string, string> keyValuePairs)
    {
        List<HashEntry> entries = [];
        foreach (var item in keyValuePairs)
        {
            entries.Add(new HashEntry(item.Key, item.Value));
        }
        database.HashSet(key, [.. entries]);
    }

    public static Dictionary<string, string> GetHash(string key)
    {
        var data = database.HashGetAll(key);
        Dictionary<string, string> keyValuePairs = [];
        foreach (var item in data)
        {
            keyValuePairs.Add(item.Name.ToString(), item.Value.ToString());
        }

        return keyValuePairs;
    }
}