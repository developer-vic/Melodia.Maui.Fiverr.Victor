using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using Microsoft.Maui.Controls.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MelodiaTherapy.Models;
using MelodiaTherapy.Controllers;
using MelodiaTherapy.Services;
using MelodiaTherapy.Globals;
using MelodiaTherapy.Views;
using System.Timers;
using MelodiaTherapy.Helpers;
using System.Net.Http.Headers;
using MelodiaTherapy.Pages;

namespace MelodiaTherapy.Controllers
{
    public partial class MelodiaController : ObservableObject, IDisposable
    {
        private bool PlayStateIsPlaying;
        private readonly MyAudioHandler _audioHandler;
        public Action<int>? PageChanged { get; internal set; }

        [ObservableProperty]
        private int _selectedPage = 0;

        public int Length { get; private set; } = 6;

        [ObservableProperty]
        private TreatmentModel _selectedTreatment;

        [ObservableProperty]
        private AmbianceModel _selectedAmbiance;

        [ObservableProperty]
        private ListenTypeModel _selectedListeningMode;

        [ObservableProperty]
        private ListenDurationModel _selectedListeningDuration;

        [ObservableProperty]
        private ThemeModel _selectedTheme;

        // Audio players
        private IAudioPlayer? _themePlayer;
        private IAudioPlayer? _treatmentPlayer;
        private IAudioPlayer? _ambiancePlayer;

        [ObservableProperty]
        private TimeSpan _duration = TimeSpan.FromMinutes(30);

        [ObservableProperty]
        private TimeSpan _position = TimeSpan.Zero;

        [ObservableProperty]
        private double _treatmentSound = 100.0;

        [ObservableProperty]
        private double _ambianceSound = 100.0;

        [ObservableProperty]
        private double _themeSound = 100.0;

        [ObservableProperty]
        private bool _isDragging = false;

        private System.Timers.Timer? _inactivityTimer;

        public MelodiaController(MyAudioHandler audioHandler)
        {
            _audioHandler = audioHandler;

            _selectedTreatment = GetLastTreatment();
            _selectedAmbiance = GetLastAmbiance();
            _selectedListeningMode = GetLastListenType();
            _selectedListeningDuration = GetLastListenDuration();
            _selectedTheme = GetLastTheme();

            Initialize();
        }

        private void Initialize()
        {
            Console.WriteLine("MelodiaController initialized");
            Console.WriteLine($"primaryPlayerIsPlaying: {_audioHandler.PrimaryPlayer.IsPlaying}");
            Console.WriteLine($"secondaryPlayerIsPlaying: {_audioHandler.SecondaryPlayer.IsPlaying}");
            Console.WriteLine($"thirdyPlayerIsPlaying: {_audioHandler.ThirdyPlayer.IsPlaying}");

            // Console.WriteLine($"primaryPlayerIsBlank: {_audioHandler.PrimaryPlayer.IsBlank}");
            // Console.WriteLine($"secondaryPlayerIsBlank: {_audioHandler.SecondaryPlayer.IsBlank}");
            // Console.WriteLine($"thirdyPlayerIsBlank: {_audioHandler.ThirdyPlayer.IsBlank}");

            Console.WriteLine($"primaryPlayerDuration: {_audioHandler.PrimaryPlayer.Duration}");
            Console.WriteLine($"secondaryPlayerDuration: {_audioHandler.SecondaryPlayer.Duration}");
            Console.WriteLine($"thirdyPlayerDuration: {_audioHandler.ThirdyPlayer.Duration}");

            Console.WriteLine($"primaryPlayerPosition: {_audioHandler.PrimaryPlayer.CurrentPosition}");
            Console.WriteLine($"secondaryPlayerPosition: {_audioHandler.SecondaryPlayer.CurrentPosition}");
            Console.WriteLine($"thirdyPlayerPosition: {_audioHandler.ThirdyPlayer.CurrentPosition}");

            // Console.WriteLine($"audioHandlerPlaybackState: {_audioHandler.PlaybackState}");
            // Console.WriteLine($"audioHandlerMediaItem: {_audioHandler.MediaItem}");

            // Console.WriteLine($"processingState: {_audioHandler.PrimaryPlayer.ProcessingState}");
            // Console.WriteLine($"processingState: {_audioHandler.SecondaryPlayer.ProcessingState}");
            // Console.WriteLine($"processingState: {_audioHandler.ThirdyPlayer.ProcessingState}");
        }

        [RelayCommand]
        public async Task SmartPlay()
        {
            if (_themePlayer != null)
                _themePlayer.Volume = (ThemeSound / 100.0);
            if (_treatmentPlayer != null)
                _treatmentPlayer.Volume = (TreatmentSound / 100.0);
            if (_ambiancePlayer != null)
                _ambiancePlayer.Volume = (AmbianceSound / 100.0);

            if (PlayStateIsPlaying)
            {
                PausePlayer();
                Console.WriteLine("PlayPause Timer started");

                _inactivityTimer?.Dispose();
                _inactivityTimer = new System.Timers.Timer(Constants.InactivityDuration.TotalMilliseconds);
                _inactivityTimer.Elapsed += async (sender, args) =>
                {
                    Console.WriteLine("PlayPause Timer expired");

                    if (Application.Current?.Dispatcher != null)
                        await Application.Current.Dispatcher.DispatchAsync(async () =>
                        {
                            await NavigationService.DisplayAlert(
                                   "Session expirée",
                                   $"Vous avez été inactif pendant {Constants.InactivityDuration.TotalMinutes} minutes",
                                   "OK");

                            StopPlayers();
                            NavigationService.NavigateToStartPageAsync();
                        });
                };
                _inactivityTimer.AutoReset = false;
                _inactivityTimer.Start();
            }
            else
            {
                if (_audioHandler.PrimaryPlayer.CurrentPosition == 0)
                {
                    await DataService.SendStatistics(
                         SelectedTreatment,
                         SelectedAmbiance,
                         SelectedTheme,
                         SelectedListeningDuration,
                         SelectedListeningMode);
                }
                ResumePlayer();
            }
        }

        private void PausePlayer()
        {
            if (SelectedTheme != ThemeController.DefaultThemeModel)
            {
                _themePlayer?.Pause();
            }

            if (SelectedAmbiance != AmbianceController.DefaultAmbianceModel)
            {
                _ambiancePlayer?.Pause();
            }

            if (SelectedTreatment != TreatmentController.DefaultTreatmentModel)
            {
                _treatmentPlayer?.Pause();
            }

            PlayStateIsPlaying = false;
        }

        private void ResumePlayer()
        {
            _inactivityTimer?.Stop();
            _inactivityTimer?.Dispose();
            _inactivityTimer = null;
            Console.WriteLine("Timer canceled");

            if (SelectedTheme != ThemeController.DefaultThemeModel)
            {
                _themePlayer?.Play();
            }

            if (SelectedAmbiance != AmbianceController.DefaultAmbianceModel)
            {
                _ambiancePlayer?.Play();
            }

            if (SelectedTreatment != TreatmentController.DefaultTreatmentModel)
            {
                _treatmentPlayer?.Play();
            }

            PlayStateIsPlaying = true;
        }

        [RelayCommand]
        public void StopPlayers()
        {
            _audioHandler.Stop();
            Console.WriteLine("players stopped");

            PlayStateIsPlaying = false;
        }

        [RelayCommand]
        public async Task GoBack()
        {
            if (SelectedPage == 0)
            {
                Dispose();
                await NavigationService.GoBackAsync();
                return;
            }
            else if (SelectedPage == 5)
            {
                _inactivityTimer?.Stop();
                _inactivityTimer?.Dispose();
                _inactivityTimer = null;
                Console.WriteLine("Timer canceled");
            }

            PreviousPage();
        }

        [RelayCommand]
        public void NextPage()
        {
            //_controller.SetSoundsValue(null, 100);
            int nextIndex = SelectedPage < 5 ? SelectedPage + 1 : SelectedPage;

            SelectedPage = nextIndex;
            PageChanged?.Invoke(SelectedPage);
        }

        [RelayCommand]
        public void PreviousPage()
        {
            //_controller.SetSoundsValue(null, 100);
            int previousIndex = SelectedPage > 0 ? SelectedPage - 1 : SelectedPage;

            SelectedPage = previousIndex;
            PageChanged?.Invoke(SelectedPage);
        }

        internal void JumpToPage(int index)
        {
            //_controller.SetSoundsValue(null, 100);
            SelectedPage = index;
            PageChanged?.Invoke(SelectedPage);
        }

        public void SetSoundsValue(DataType? type, double value)
        {
            const double divisor = 100.0;

            if (type == DataType.Treatments && _treatmentPlayer != null)
            {
                TreatmentSound = value;
                _treatmentPlayer.Volume = (value / divisor);
            }
            else if (type == DataType.Themes && _themePlayer != null)
            {
                ThemeSound = value;
                _themePlayer.Volume = (value / divisor);
            }
            else if (type == DataType.Ambiances && _ambiancePlayer != null)
            {
                AmbianceSound = value;
                _ambiancePlayer.Volume = (value / divisor);
            }
            else if (type == null)
            {
                TreatmentSound = value;
                if (_treatmentPlayer != null)
                    _treatmentPlayer.Volume = (value / divisor);
                ThemeSound = value;
                if (_themePlayer != null)
                    _themePlayer.Volume = (value / divisor);
                AmbianceSound = value;
                if (_ambiancePlayer != null)
                    _ambiancePlayer.Volume = (value / divisor);
            }
            else
            {
                Console.WriteLine("Error: No sound type");
            }
        }

        [RelayCommand]
        public async Task PlaySounds()
        {
            await SmartPlay();
        }

        public void SetPositionValue(TimeSpan value)
        {
            if (IsDragging)
                return;

            if (value.TotalSeconds <= SelectedListeningDuration.Duration.TotalSeconds ||
                value.TotalSeconds <= Duration.TotalSeconds)
            {
                if (SelectedTheme != ThemeController.DefaultThemeModel && _themePlayer != null)
                {
                    _themePlayer.Seek(value.TotalSeconds);
                }

                if (SelectedAmbiance != AmbianceController.DefaultAmbianceModel && _ambiancePlayer != null)
                {
                    _ambiancePlayer.Seek(value.TotalSeconds);
                }

                if (SelectedTreatment != TreatmentController.DefaultTreatmentModel && _treatmentPlayer != null)
                {
                    _treatmentPlayer.Seek(value.TotalSeconds);
                }
            }
        }

        public void InitPlayers()
        {
            if ((_themePlayer?.IsPlaying == true) ||
                (_ambiancePlayer?.IsPlaying == true) ||
                (_treatmentPlayer?.IsPlaying == true))
            {
                Console.WriteLine("Players are already initialized");
                return;
            }

            Console.WriteLine("Initialization of players");

            if (SelectedTreatment != TreatmentController.DefaultTreatmentModel)
            {
                _treatmentPlayer = _audioHandler.PrimaryPlayer;

                if (SelectedAmbiance != AmbianceController.DefaultAmbianceModel)
                {
                    _ambiancePlayer = _audioHandler.SecondaryPlayer;

                    if (SelectedTheme != ThemeController.DefaultThemeModel)
                    {
                        _themePlayer = _audioHandler.ThirdyPlayer;
                    }
                }
                else if (SelectedTheme != ThemeController.DefaultThemeModel)
                {
                    _themePlayer = _audioHandler.SecondaryPlayer;
                }
            }
            else if (SelectedAmbiance != AmbianceController.DefaultAmbianceModel)
            {
                _ambiancePlayer = _audioHandler.PrimaryPlayer;

                if (SelectedTheme != ThemeController.DefaultThemeModel)
                {
                    _themePlayer = _audioHandler.SecondaryPlayer;
                }
            }
            else
            {
                _themePlayer = _audioHandler.PrimaryPlayer;
            }
        }

        private void InitPrincipalPlayerListener(IAudioPlayer player)
        {
            // // Duration listener
            // player.DurationChanged += (sender, duration) => Duration = duration;

            // // Position listener
            // player.PositionChanged += (sender, position) =>
            // {
            //     if ((position >= SelectedListeningDuration.Duration || position >= Duration))
            //     {
            //         Console.WriteLine($"Position {position} >= Duration {Duration} or SelectedListeningDuration {SelectedListeningDuration.Duration}");
            //         StopPlayers();
            //     }
            //     Position = position;
            // };

            // // Player state listener
            // player.StateChanged += (sender, state) => PlayState = state;
        }

        public void InitPlayersListeners()
        {
            Console.WriteLine("Initialization of players listeners");

            // Initialize listeners
            if (SelectedTreatment != TreatmentController.DefaultTreatmentModel && _treatmentPlayer != null)
            {
                InitPrincipalPlayerListener(_treatmentPlayer);
            }
            else if (SelectedAmbiance != AmbianceController.DefaultAmbianceModel && _ambiancePlayer != null)
            {
                InitPrincipalPlayerListener(_ambiancePlayer);
            }
            else if (SelectedTheme != ThemeController.DefaultThemeModel && _themePlayer != null)
            {
                InitPrincipalPlayerListener(_themePlayer);
            }

            // Sound selection change listeners
            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SelectedTreatment) ||
                    args.PropertyName == nameof(SelectedAmbiance) ||
                    args.PropertyName == nameof(SelectedTheme) ||
                    args.PropertyName == nameof(SelectedListeningMode) ||
                    args.PropertyName == nameof(SelectedListeningDuration))
                {
                    StopPlayers();
                }
            };
        }

        public async Task SetPlayersAudioSources()
        {
            Console.WriteLine("Setting players audio sources");

            if (SelectedTreatment != TreatmentController.DefaultTreatmentModel)
            {
                try
                {
                    Console.WriteLine($"selectedTreatment: {SelectedTreatment.Name}");
                    string treatmentPath = TreatmentController.GetLocalSongPath(
                        SelectedTreatment,
                        SelectedListeningMode,
                        SelectedListeningDuration.Duration);
                    Console.WriteLine($"treatmentPath: {treatmentPath}");
                    //Console.WriteLine($"treatmentState: {_treatmentPlayer.ProcessingState}");

                    await _audioHandler.ChangeMediaItem(new MediaItem
                    {
                        Id = treatmentPath,
                        Title = SelectedTreatment.Name,
                        Artist = SelectedAmbiance.Name,
                        Duration = SelectedListeningDuration.Duration,
                        ArtUri = new Uri(ThemeController.GetLocalImagePath(SelectedTheme))
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error setting audio source for treatment: {e}");
                }

                if (SelectedAmbiance != AmbianceController.DefaultAmbianceModel)
                {
                    try
                    {
                        Console.WriteLine($"selectedAmbiance: {SelectedAmbiance.Name}");
                        string ambiancePath = AmbianceController.GetLocalSongPath(
                            SelectedAmbiance,
                            SelectedListeningDuration.Duration);
                        Console.WriteLine($"ambiancePath: {ambiancePath}");
                        //Console.WriteLine($"ambianceState: {_ambiancePlayer.ProcessingState}");

                        if (_ambiancePlayer != null)
                            _ambiancePlayer.SetSource(await FileSystem.OpenAppPackageFileAsync(ambiancePath));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error setting audio source for ambiance: {e}");
                    }
                }

                if (SelectedTheme != ThemeController.DefaultThemeModel)
                {
                    try
                    {
                        Console.WriteLine($"selectedTheme: {SelectedTheme.Name}");
                        string themePath = ThemeController.GetLocalSongPath(
                            SelectedTheme,
                            SelectedListeningDuration.Duration);
                        Console.WriteLine($"themePath: {themePath}");
                        //Console.WriteLine($"themeState: {_themePlayer.ProcessingState}");

                        if (_themePlayer != null)
                            _themePlayer.SetSource(await FileSystem.OpenAppPackageFileAsync(themePath));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error setting audio source for theme: {e}");
                    }
                }
            }
            else if (SelectedAmbiance != AmbianceController.DefaultAmbianceModel)
            {
                string ambiancePath = AmbianceController.GetLocalSongPath(
                    SelectedAmbiance,
                    SelectedListeningDuration.Duration);

                await _audioHandler.ChangeMediaItem(new MediaItem
                {
                    Id = ambiancePath,
                    Title = SelectedTreatment.Name,
                    Artist = SelectedAmbiance.Name,
                    Duration = SelectedListeningDuration.Duration,
                    ArtUri = new Uri(ThemeController.GetLocalImagePath(SelectedTheme))
                });

                if (SelectedTheme != ThemeController.DefaultThemeModel)
                {
                    string themePath = ThemeController.GetLocalSongPath(
                        SelectedTheme,
                        SelectedListeningDuration.Duration);

                    if (_themePlayer != null)
                        _themePlayer.SetSource(await FileSystem.OpenAppPackageFileAsync(themePath));
                }
            }
            else if (SelectedTheme != ThemeController.DefaultThemeModel)
            {
                string themePath = ThemeController.GetLocalSongPath(
                    SelectedTheme,
                    SelectedListeningDuration.Duration);

                await _audioHandler.ChangeMediaItem(new MediaItem
                {
                    Id = themePath,
                    Title = SelectedTreatment.Name,
                    Artist = SelectedAmbiance.Name,
                    Duration = SelectedListeningDuration.Duration,
                    ArtUri = new Uri(ThemeController.GetLocalImagePath(SelectedTheme))
                });
            }

            Console.WriteLine("Players audio sources set");
        }

        [RelayCommand]
        public async Task InitSounds()
        {
            Console.WriteLine("Initialization of sounds");
            InitPlayers();
            InitPlayersListeners();

            await SetPlayersAudioSources();

            Console.WriteLine("PARAMETERS");
            Console.WriteLine($"position: {Position}");
            Console.WriteLine($"treatment: {_treatmentPlayer?.Duration}");
            Console.WriteLine(SelectedTreatment.Name);
            Console.WriteLine($"ambiance: {_ambiancePlayer?.Duration}");
            Console.WriteLine(SelectedAmbiance.Name);
            Console.WriteLine($"theme: {_themePlayer?.Duration}");
            Console.WriteLine(SelectedTheme.Name);
            Console.WriteLine($"selectedDuration: {SelectedListeningDuration.Duration}");
            Console.WriteLine($"selectedMode: {SelectedListeningMode.Name}");
            Console.WriteLine($"duration: {Duration}");

            Console.WriteLine("AUDIO HANDLERS:");
            Console.WriteLine($"1 {_audioHandler.PrimaryPlayer.Duration}");
            Console.WriteLine($"2 {_audioHandler.SecondaryPlayer.Duration}");
            Console.WriteLine($"3 {_audioHandler.ThirdyPlayer.Duration}");
        }

        private static TreatmentModel GetLastTreatment()
        {
            var controller = ServiceHelper.GetService<TreatmentController>();
            if (controller != null && Constants.TreatmentId != null)
            {
                return controller.Treatments?[Constants.TreatmentId.Value]
                    ?? TreatmentController.DefaultTreatmentModel;
            }
            else
            {
                return TreatmentController.DefaultTreatmentModel;
            }
        }

        private static AmbianceModel GetLastAmbiance()
        {
            var controller = ServiceHelper.GetService<AmbianceController>();
            if (controller != null && Constants.AmbianceId != null)
            {
                return controller.Ambiances?[Constants.AmbianceId.Value]
                    ?? AmbianceController.DefaultAmbianceModel;
            }
            else
            {
                return AmbianceController.DefaultAmbianceModel;
            }
        }

        private static ThemeModel GetLastTheme()
        {
            var controller = ServiceHelper.GetService<ThemeController>();
            if (controller != null && Constants.ThemeId != null)
            {
                return controller.Themes[Constants.ThemeId.Value];
            }
            else
            {
                return ThemeController.DefaultThemeModel;
            }
        }

        private static ListenTypeModel GetLastListenType()
        {
            var controller = ServiceHelper.GetService<ListenTypeController>();
            if (controller != null && Constants.ListenTypeId != null)
            {
                return controller.ListenTypes?[Constants.ListenTypeId.Value]
                    ?? ListenTypeController.DefaultListenTypeModel;
            }
            else
            {
                return ListenTypeController.DefaultListenTypeModel;
            }
        }

        private static ListenDurationModel GetLastListenDuration()
        {
            var controller = ServiceHelper.GetService<ListenDurationController>();
            Console.WriteLine($"listenDurationId: {Constants.ListenDurationId}");
            if (controller != null && Constants.ListenDurationId != null)
            {
                return controller.ListenDurations?[Constants.ListenDurationId.Value]
                    ?? ListenDurationController.DefaultListenDurationModel;
            }
            else
            {
                return ListenDurationController.DefaultListenDurationModel;
            }
        }

        [RelayCommand]
        public void SetIsDragging(bool value)
        {
            IsDragging = value;
        }

        public void Dispose()
        {
            Console.WriteLine("MelodiaController disposed");
            Console.WriteLine("Timer canceled");

            _inactivityTimer?.Stop();
            _inactivityTimer?.Dispose();
            _inactivityTimer = null;

            StopPlayers();

            PlayStateIsPlaying = false;

            GC.SuppressFinalize(this);
        }

        internal void GotoPlayerPage()
        {
            NavigationService.PushPage(new PlayerPage());
        }
    }
}