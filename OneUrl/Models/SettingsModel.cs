namespace OneUrl.Models;

public class SettingsModel
{
    public int Id { get; set; }
    required public int RedirectDelay { get; set; }

    public bool IsCollectingInteractCount { get; set; }
}