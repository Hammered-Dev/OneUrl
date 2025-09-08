using System.Net;
using System.Text.Json;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using OneUrlApi.Models;
using StackExchange.Redis;

namespace OneUrlApi.Services;

public static class RedisService
{
    private static readonly ConfigurationOptions conf = new()
    {
        EndPoints = { { "localhost", 6379 }, },
    };
    private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(conf);
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

    public static void Delete(string key)
    {
        database.KeyDelete(key);
    }

    public static UrlRecord?[] IndexUrl()
    {
        try
        {
            database.FT().Info("idx:urls");
        }
        catch
        {
            var scheme = new Schema()
                .AddTextField("Location")
                .AddTextField("Target");

            FTCreateParams createParams = new FTCreateParams()
                .AddPrefix("records:")
                .On(IndexDataType.HASH);

            database.FT().Create(
                "idx:urls",
                createParams,
                scheme
            );
        }
        List<string> result = database.FT().Search("idx:urls", new("*")).ToJson();
        List<UrlRecord?> records = [];

        foreach (var item in result)
        {
            if (item != null)
            {
                records.Add(JsonSerializer.Deserialize<UrlRecord>(item));
            }
        }

        return [.. records];
    }
}