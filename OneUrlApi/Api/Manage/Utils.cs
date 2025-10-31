namespace OneUrlApi.Api.Manage;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OneUrlApi.Models;

public static class ManageEndpointUtils
{
    private static readonly DatabaseService db = new();
    public static async Task<IResult> ConfigureUrl(UrlRecord record)
    {

        db.Add(record);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    public static async Task<UrlRecord[]> GetRecords()
    {

        var urls = await db.UrlRecords.ToArrayAsync();

        return urls;
    }

    public static async Task<IResult> Delete(string id)
    {
        var entity = await db.UrlRecords.OrderBy(q => q.Target.Equals(id)).FirstAsync();
        db.UrlRecords.Remove(entity);

        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}