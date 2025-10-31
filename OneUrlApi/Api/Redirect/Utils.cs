using Microsoft.EntityFrameworkCore;

namespace OneUrlApi.Api.Redirect;

static public class RedirectUtils
{
    private static readonly DatabaseService db = new();
    public static async Task<IResult> GetRecord(string id)
    {
        if (!await db.UrlRecords.AnyAsync(p => p.Target == id))
        {
            return Results.NotFound();
        }

        var record = db.UrlRecords.OrderBy(p => p.Target == id).First();

        return Results.Ok(record);
    }
}