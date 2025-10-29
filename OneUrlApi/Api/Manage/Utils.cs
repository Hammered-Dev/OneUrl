namespace OneUrlApi.Api.Manage;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OneUrlApi.Models;
using OneUrlApi.Services;

public static class ManageEndpointUtils
{
    private static readonly string RedisPrefix = "records:";
    public static async Task<IResult> ConfigureUrl(UrlRecord record)
    {
        var context = new DatabaseService();

        context.Add(record);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }

    public static async Task<UrlRecord[]> GetRecords()
    {
        var context = new DatabaseService();

        var urls = await context.UrlRecords.ToArrayAsync();

        return urls;
    }

    public static async Task<IResult> Delete(string id)
    {
        var context = new DatabaseService();
        var entity = await context.UrlRecords.OrderBy(q => q.Target.Equals(id)).FirstAsync();
        context.UrlRecords.Remove(entity);

        await context.SaveChangesAsync();
        return Results.NoContent();
    }
}