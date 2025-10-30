using System.ComponentModel.DataAnnotations;

namespace OneUrlApi.Models;

public class SettingsModel
{
    [Key]
    public int Id { get; set; }
    required public int RedirectDelay { get; set; }
}