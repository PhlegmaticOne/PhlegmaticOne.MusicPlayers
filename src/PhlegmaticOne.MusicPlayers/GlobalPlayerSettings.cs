namespace PhlegmaticOne.MusicPlayers;

public static class GlobalPlayerSettings
{
    public static float StartVolume { get; set; } = 0.2f;
    public static TimeSpan UpdatePlayerTimelineDefaultTime { get; set; } = TimeSpan.FromMilliseconds(500);
}