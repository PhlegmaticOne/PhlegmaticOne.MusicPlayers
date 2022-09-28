using NAudio.Wave;
using PhlegmaticOne.MusicPlayers;
using PhlegmaticOne.MusicPlayers.Base;
using PhlegmaticOne.MusicPlayers.Exceptions;

namespace PhlegmaticOne.MusicPlayers.Models;

public class NAudioMusicPlayer : IPlayer
{
    private float _volume;
    private bool _isDisposed;
    private bool _isUserStopped;
    private MediaFoundationReader _mediaFoundationReader = null!;
    private WasapiOut _wasapiOut = null!;
    private Task? _songTask;
    public NAudioMusicPlayer()
    {
        PlayerState = PlayerState.Stopped;
        _isDisposed = true;
        _volume = GlobalPlayerSettings.StartVolume;
        UpdatePlayerTimelineTime = GlobalPlayerSettings.UpdatePlayerTimelineDefaultTime;
    }

    public NAudioMusicPlayer(TimeSpan updatePlayerTimelineTime) : this() => UpdatePlayerTimelineTime = updatePlayerTimelineTime;


    public event EventHandler<TimeSpan>? TimeChanged;
    public event EventHandler? SongEnded;
    public event EventHandler<PlayerState>? PlayerStateChanged;
    public event EventHandler<float>? VolumeChanged;

    public float Volume
    {
        get => _wasapiOut?.Volume ?? _volume;
        set
        {
            if (value <= 1)
            {
                _volume = value;
            }
            if (_isDisposed == false)
            {
                _wasapiOut.Volume = _volume;
            }
            InvokeVolumeChanged();
        }
    }

    public TimeSpan UpdatePlayerTimelineTime { get; set; }
    public PlayerState PlayerState { get; private set; }
    public TimeSpan CurrentTime { get; private set; }

    /// <summary>
    /// Plays music file from URL-link or Local file path
    /// </summary>
    /// <param name="fileName"></param>
    /// <exception cref="ArgumentException"></exception>
    public void Play(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name can't be null or empty", nameof(fileName));
        }
        _songTask?.Wait();
        _songTask = new Task(() => PlaySong(fileName));
        _songTask.GetAwaiter().OnCompleted(() =>
        {
            if (_isUserStopped == false)
            {
                InvokeSongEnded();
            }

            _isUserStopped = false;
        });
        _songTask.Start();
    }

    public void PauseOrUnpause()
    {
        if (PlayerState == PlayerState.Paused)
        {
            _wasapiOut.Play();
            SetState(PlayerState.Playing);
        }
        else
        {
            _wasapiOut.Pause();
            SetState(PlayerState.Paused);
        }
    }
    public void Stop()
    {
        try
        {
            _wasapiOut.Stop();
            SetState(PlayerState.Stopped);
            _isUserStopped = true;
        }
        catch { }
    }

    public void Rewind(TimeSpan timeStamp)
    {
        if (timeStamp <= TimeSpan.Zero || timeStamp > _mediaFoundationReader.TotalTime)
        {
            throw new RewindException(timeStamp, _mediaFoundationReader.TotalTime,
                "Requested time to rewind to is less than 0 or greater than total track length");
        }

        var position = _mediaFoundationReader.Length * timeStamp.Ticks / _mediaFoundationReader.TotalTime.Ticks;
        _mediaFoundationReader.Position = position;
        SetTime(timeStamp);
    }

    public void Dispose() => TryDispose();
    private void PlaySong(string fileName)
    {
        TryDispose();

        _mediaFoundationReader = new MediaFoundationReader(fileName);
        _wasapiOut = new WasapiOut();

        _isDisposed = false;
        Volume = _volume;

        SetState(PlayerState.Playing);

        _wasapiOut.Init(_mediaFoundationReader);
        _wasapiOut.Play();

        while (_wasapiOut.PlaybackState is PlaybackState.Playing or PlaybackState.Paused)
        {
            if (_wasapiOut.PlaybackState == PlaybackState.Paused) continue;

            Thread.Sleep(UpdatePlayerTimelineTime);
            SetTime(CurrentTime += UpdatePlayerTimelineTime);
        }

        TryDispose();

        SetTime(TimeSpan.Zero);
        SetState(PlayerState.Stopped);
    }

    private void SetTime(TimeSpan newTime)
    {
        CurrentTime = newTime;
        InvokeTimeChanged();
    }

    private void SetState(PlayerState playerState)
    {
        PlayerState = playerState;
        InvokeStateChanged();
    }

    private void InvokeTimeChanged() => TimeChanged?.Invoke(this, CurrentTime);
    private void InvokeStateChanged() => PlayerStateChanged?.Invoke(this, PlayerState);
    private void InvokeSongEnded() => SongEnded?.Invoke(this, EventArgs.Empty);
    private void InvokeVolumeChanged() => VolumeChanged?.Invoke(this, _volume);
    private void TryDispose()
    {
        try
        {
            _mediaFoundationReader?.Dispose();
            _wasapiOut?.Dispose();
            _isDisposed = true;
        }
        catch { }
    }
}