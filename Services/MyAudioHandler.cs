using MelodiaTherapy.Models;
using Plugin.Maui.Audio;

namespace MelodiaTherapy.Services;

public class MyAudioHandler
{
    public readonly IAudioPlayer PrimaryPlayer;
    public readonly IAudioPlayer SecondaryPlayer;
    public readonly IAudioPlayer ThirdyPlayer;
    public readonly IAudioManager _audioManager;

    public event Action<PlaybackState>? OnPlaybackStateChanged;

    public MyAudioHandler()
    {
        _audioManager = AudioManager.Current;
        PrimaryPlayer = _audioManager.CreatePlayer();
        SecondaryPlayer = _audioManager.CreatePlayer();
        ThirdyPlayer = _audioManager.CreatePlayer();

        // Simulate Playback State Subscription
        // Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        // {
        //     var state = GetCurrentPlaybackState();
        //     OnPlaybackStateChanged?.Invoke(state);
        //     return true;
        // });
    }

    public async Task ChangeMediaItem(MediaItem item)
    {
        PrimaryPlayer.Stop();
        
        if (!string.IsNullOrEmpty(item.Id))
            PrimaryPlayer.SetSource(await FileSystem.OpenAppPackageFileAsync(item.Id));
        // In Maui.Audio, only one file plays at a time per player instance
    }

    public void Play() => PlayersPlay();

    public void Pause() => PlayersPause();

    public Task Seek(TimeSpan position) => PlayersSeek(position);

    public void Stop() => PlayersStop();

    private PlaybackState GetCurrentPlaybackState()
    {
        return new PlaybackState
        {
            Playing = PrimaryPlayer.IsPlaying,
            Position = TimeSpan.FromSeconds(PrimaryPlayer.CurrentPosition),
            Duration = TimeSpan.FromSeconds(PrimaryPlayer.Duration)
        };
    }

    private void PlayersPlay()
    {
        PrimaryPlayer.Play();
        SecondaryPlayer.Play();
        ThirdyPlayer.Play();
    }

    private void PlayersPause()
    {
        PrimaryPlayer.Pause();
        SecondaryPlayer.Pause();
        ThirdyPlayer.Pause();
    }

    private void PlayersStop()
    {
        PrimaryPlayer.Stop();
        SecondaryPlayer.Stop();
        ThirdyPlayer.Stop();
    }

    private Task PlayersSeek(TimeSpan position)
    {
        PrimaryPlayer.Seek(position.TotalSeconds);
        SecondaryPlayer.Seek(position.TotalSeconds);
        ThirdyPlayer.Seek(position.TotalSeconds);
        return Task.CompletedTask;
    }

    public void DisposeAsync()
    {
        PrimaryPlayer.Stop();
        SecondaryPlayer.Stop();
        ThirdyPlayer.Stop();

        PrimaryPlayer.Dispose();
        SecondaryPlayer.Dispose();
        ThirdyPlayer.Dispose();

        Console.WriteLine("playersDispose");
    }
}
