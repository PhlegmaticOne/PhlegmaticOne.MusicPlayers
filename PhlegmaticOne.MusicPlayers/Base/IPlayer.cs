namespace PhlegmaticOne.MusicPlayers.Base;

/// <summary>
/// Contracts for music players
/// </summary>
public interface IPlayer : IDisposable
{
    /// <summary>
    /// Invoked when player time changed
    /// </summary>
    public event EventHandler<TimeSpan>? TimeChanged;
    /// <summary>
    /// Invoked when player volume changed
    /// </summary>
    public event EventHandler<float>? VolumeChanged;
    /// <summary>
    /// Invoked when player state changed
    /// </summary>
    public event EventHandler<PlayerState>? PlayerStateChanged;
    /// <summary>
    /// Invoked when song in player ended
    /// </summary>
    public event EventHandler? SongEnded;
    /// <summary>
    /// Player volume
    /// </summary>
    public float Volume { get; set; }
    /// <summary>
    /// The time interval after which the time in the player is updated
    /// </summary>
    public TimeSpan UpdatePlayerTimelineTime { get; set; }
    /// <summary>
    /// Player state
    /// </summary>
    public PlayerState PlayerState { get; }
    /// <summary>
    /// Current time in player
    /// </summary>
    public TimeSpan CurrentTime { get; }
    /// <summary>
    /// Plays music file
    /// </summary>
    /// <param name="fileName">Path to file</param>
    public void Play(string fileName);
    /// <summary>
    /// Pauses or unpauses song player
    /// </summary>
    public void PauseOrUnpause();
    /// <summary>
    /// Stops current song in player
    /// </summary>
    public void Stop();
    /// <summary>
    /// Rewinds current song in player
    /// </summary>
    /// <param name="timeStamp">Timestamp in song where to rewind to</param>
    public void Rewind(TimeSpan timeStamp);
}