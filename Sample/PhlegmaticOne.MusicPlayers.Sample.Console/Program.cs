using PhlegmaticOne.MusicPlayers.Base;
using PhlegmaticOne.MusicPlayers.Models;

namespace PhlegmaticOne.MusicPlayers.Sample.Console;

public class Program
{
    private const float DVolume = 0.02f;
    private static readonly TimeSpan DTime = TimeSpan.FromSeconds(10);
    private readonly static Dictionary<ConsoleKey, Action> _keyActions = new()
    {
        { ConsoleKey.Escape, () => _isExitRequested = true },
        { ConsoleKey.MediaStop,
            () =>
            {
                if(_player.PlayerState is PlayerState.Playing or PlayerState.Paused)
                {
                    _player.Stop();
                }
            }
        },
        { ConsoleKey.MediaPlay,
            () =>
            {
                if(_player.PlayerState == PlayerState.Stopped)
                {
                    PlayTrack();
                }
                else
                {
                    _player.PauseOrUnpause();
                }
            }
        },
        { ConsoleKey.MediaNext,
            () =>
            {
                _player.Stop();
                IncreaseIndex();
                PlayTrack();
            }
        },
        { ConsoleKey.MediaPrevious,
            () =>
            {
                _player.Stop();
                DecreaseIndex();
                PlayTrack();
            }
        },
        { ConsoleKey.VolumeUp, () => _player.Volume += DVolume },
        { ConsoleKey.VolumeDown, () => _player.Volume -= DVolume },
        { ConsoleKey.LeftArrow, () =>
            {
                var playerTime = _player.CurrentTime;
                var rewindTime = playerTime - DTime;
                try
                {
                    _player.Rewind(rewindTime);
                }
                catch { }
            } 
        },
        { ConsoleKey.RightArrow, () =>
            {
                var playerTime = _player.CurrentTime;
                var rewindTime = playerTime + DTime;
                try
                {
                    _player.Rewind(rewindTime);
                }
                catch { }
            }
        },
    };

    private readonly static List<string> Tracks = new()
    {
        "https://musify.club/track/dl/635394/paysage-dhiver-die-baumfrau.mp3",
        "https://musify.club/track/dl/635395/paysage-dhiver-der-baummann.mp3",
        "https://musify.club/track/dl/635396/paysage-dhiver-der-baum.mp3",
        //here can be located local paths (D:\Music\MyMusicFile.mp3)
    };

    private static bool _isExitRequested;

    private static int _playingTrackIndex;
    private static IPlayer _player = null!;

    public static void Main(string[] _)
    {
        Logger.LogTo = System.Console.WriteLine;
        Logger.ClearAction = System.Console.Clear;

        _playingTrackIndex = 0;
        _player = CreatePlayer();
        PlayTrack();


        while(_isExitRequested == false)
        {
            var keyPressed = System.Console.ReadKey();
            if(_keyActions.TryGetValue(keyPressed.Key, out var action))
            {
                action();
            }
        }

        _player.Dispose();
    }

    static IPlayer CreatePlayer()
    {
        GlobalPlayerSettings.StartVolume = 0.1f;
        GlobalPlayerSettings.UpdatePlayerTimelineDefaultTime = TimeSpan.FromMilliseconds(500);
        var player = new NAudioMusicPlayer();
        player.PlayerStateChanged += Player_PlayerStateChanged;
        player.VolumeChanged += Player_VolumeChanged;
        player.TimeChanged += Player_TimeChanged;
        player.SongEnded += Player_SongEnded;
        return player;
    }

    static void PlayTrack()
    {
        var newTrackPath = Tracks[_playingTrackIndex];
        _player.Play(newTrackPath);
    }

    static void Player_SongEnded(object? sender, string endedTrackUri)
    {
        Logger.Log("Ended: " + endedTrackUri);

        IncreaseIndex();
        PlayTrack();
    }

    static void IncreaseIndex()
    {
        _playingTrackIndex = _playingTrackIndex == Tracks.Count - 1 ? 0 : ++_playingTrackIndex;
    }

    static void DecreaseIndex()
    {
        _playingTrackIndex = _playingTrackIndex == 0 ? Tracks.Count - 1 : --_playingTrackIndex;
    }

    static void Player_TimeChanged(object? sender, TimeSpan currentTimeInPlayer)
    {
        ClearAndLogPlayerInfo();
    }

    static void Player_VolumeChanged(object? sender, float newPlayerVolume)
    {
        Logger.Log(newPlayerVolume);
    }

    static void Player_PlayerStateChanged(object? sender, PlayerState newPlayerState)
    {
        Logger.Log(newPlayerState);
    }

    static void ClearAndLogPlayerInfo()
    {
        Logger.Clear();
        Logger.Log(_player.ToString());
    }
}