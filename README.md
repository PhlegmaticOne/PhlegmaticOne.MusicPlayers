# PhlegmaticOne.MusicPlayers

![Logo - Copy](https://user-images.githubusercontent.com/73738250/192877116-bf055039-8220-4ec7-bec0-66c1e269910f.png)
##

### Nuget package
[PhlegmaticOne.MusicPlayers](https://www.nuget.org/packages/PhlegmaticOne.MusicPlayers/)
##

### Installation
```
PM> NuGet\Install-Package PhlegmaticOne.MusicPlayers -Version 1.0.2
```

## Usage

### Basic Usage
```csharp
GlobalPlayerSettings.StartVolume = 0.1f;
GlobalPlayerSettings.UpdatePlayerTimelineDefaultTime = TimeSpan.FromMilliseconds(500);
var player = new NAudioMusicPlayer();
player.Play("MyMusicFile.mp3");
```

### More advanced usage
See sample :)
