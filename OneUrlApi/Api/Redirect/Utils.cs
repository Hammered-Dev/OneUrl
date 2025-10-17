using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OneUrlApi.Models;
using OneUrlApi.Services;

namespace OneUrlApi.Api.Redirect;

static public class RedirectUtils
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public static IResult GetRecord(string id)
    {
        try
        {
            var data = RedisService.GetHash($"records:{id}");
            var json = JsonSerializer.Serialize(data);
            var deserilized = JsonSerializer.Deserialize<UrlRecord>(json, options) ?? throw new HttpRequestException(HttpRequestError.InvalidResponse, "Record not found");
            return Results.Ok(deserilized);
        }
        catch
        {
            ProblemDetails details = new()
            {
                Type = "target-not-found",
                Title = "Target not found.",
                Status = 404,
                Instance = $"/redirect?id={id}",
                Detail = $"Cannot found record for {id}"
            };
            return Results.NotFound(details);
        }
    }
}