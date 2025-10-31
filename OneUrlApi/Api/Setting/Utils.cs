using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OneUrlApi.Models;

namespace OneUrlApi.Api.Setting;

static public class SettingsUtil
{
    private static readonly DatabaseService db = new();
    public static async Task<SettingsModel> GetSettings()
    {
        var db = new DatabaseService();
        if (!await db.Settings.AnyAsync())
        {
            var settings = new SettingsModel
            {
                RedirectDelay = 3000
            };
            await CreateSettings(settings);
            return await GetSettings();
        }
        else
        {
            var settings = db.Settings.First();
            return settings;
        }
    }

    public static async Task<IResult> SaveSettings(SettingsModel settings)
    {
        var db = new DatabaseService();

        if (!await db.Settings.AnyAsync())
        {
            await CreateSettings(settings);
        }
        else
        {
            await UpdateSettings(settings);
        }

        return Results.NoContent();
    }

    private static async Task UpdateSettings(SettingsModel settings)
    {
        var setting = db.Settings.First();
        setting = settings;

        db.Update(setting);
        
        await db.SaveChangesAsync();
    }
    
    private static async Task CreateSettings(SettingsModel settings)
    {
        await db.Settings.AddAsync(settings);
        await db.SaveChangesAsync();
    }
}