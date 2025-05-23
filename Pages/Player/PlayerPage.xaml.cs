using System.Windows.Input;
using MelodiaTherapy.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MelodiaTherapy.Controllers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Enums;
using MelodiaTherapy.Views.PlayerViews;

namespace MelodiaTherapy.Pages;

public partial class PlayerPage : ContentPage
{
	public PlayerPage()
	{
		InitializeComponent();
		BindingContext = new PlayerPageViewModel();
	}

	private async void imgBackTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
	{
		await NavigationService.GoBackAsync();
	}
}

public sealed class PlayerPageViewModel : BaseViewModel
{
	#region fields & ctor
	private MelodiaController? _controller;
	private MyAudioHandler? _audioHandler;

	public SoundDownloadController? ThemeDownloadController { get; set; }
	public SoundDownloadController? AmbianceDownloadController { get; set; }
	public SoundDownloadController? TreatmentDownloadController { get; set; }

    public string? BGImagePath { get => bGImagePath; set { SetProperty(ref bGImagePath, value); } }

    public PlayerPageViewModel()
	{
		PlayCommand = new Command(OnPlay);
		SaveSessionCommand = new Command(OnSaveSession);

		Task.Factory.StartNew(() =>
		{
			InitData();
		});
	}

	private void InitData()
	{
		_controller = ServiceHelper.GetService<MelodiaController>();
		_audioHandler = ServiceHelper.GetService<MyAudioHandler>();

		if (_controller == null || _audioHandler == null)
			return;

		BGImagePath = ServiceHelper.FixMalformedUrl(_controller.SelectedTheme.ImageUrl);

		ThemeDownloadController = new SoundDownloadController(
			_controller.SelectedTheme.SongGuid,
			_controller.SelectedListeningDuration.Duration,
			DataType.Themes);

		AmbianceDownloadController = new SoundDownloadController(
			_controller.SelectedAmbiance.SongGuid,
			_controller.SelectedListeningDuration.Duration,
			DataType.Ambiances);

		var tGuid = _controller.SelectedTreatment.Guid == string.Empty
					? string.Empty
					: _controller.SelectedTreatment.TreatmentUrls?
								  .Find(x => x.ListenTypeGuidId
											 == _controller.SelectedListeningMode.Guid)?
								  .SongGuid;

		TreatmentDownloadController = new SoundDownloadController(
			tGuid,
			_controller.SelectedListeningDuration.Duration,
			DataType.Treatments);

		InitStreams();
		_ = StartDownloadsAsync();
	}
	#endregion

	#region observable properties
	public ThemeModel? SelectedTheme { get => _controller?.SelectedTheme; }
	private AmbianceModel? _selectedAmbiance => _controller?.SelectedAmbiance;
	private TreatmentModel? _selectedTreatment => _controller?.SelectedTreatment;
	private ListenDurationModel? _selectedDuration => _controller?.SelectedListeningDuration;
	private ListenTypeModel? _selectedMode => _controller?.SelectedListeningMode;

	private bool _readyToPlay;
	public bool ReadyToPlay
	{
		get => _readyToPlay;
		set { SetField(ref _readyToPlay, value); }
	}

	private bool _isPlaying;
    private string? bGImagePath;

    public bool IsPlaying
	{
		get => _isPlaying;
		private set
		{
			if (SetField(ref _isPlaying, value))
				OnPropertyChanged(nameof(PlayIcon));
		}
	}

	public string PlayIcon { get => IsPlaying ? "pause_icon.png" : "play_icon.png"; }

	public bool ShowTreatment => _selectedTreatment != TreatmentController.DefaultTreatmentModel;
	public bool ShowAmbiance => _selectedAmbiance != AmbianceController.DefaultAmbianceModel;
	public bool ShowTheme => SelectedTheme != ThemeController.DefaultThemeModel;
	#endregion

	#region commands
	public ICommand PlayCommand { get; }
	public ICommand SaveSessionCommand { get; }
	#endregion

	#region InitStreams
	private void InitStreams()
	{
		if (ThemeDownloadController == null || AmbianceDownloadController == null
			|| TreatmentDownloadController == null || _audioHandler == null) return;

		// download-completion callbacks
		ThemeDownloadController.ReadyChanged += OnDownloadReadyChanged;
		AmbianceDownloadController.ReadyChanged += OnDownloadReadyChanged;
		TreatmentDownloadController.ReadyChanged += OnDownloadReadyChanged;

		UpdateReadyState();

		// audio player state
		_audioHandler.OnPlaybackStateChanged += (state) =>
		{
			IsPlaying = state.Playing;
		};
	}

	private void OnDownloadReadyChanged()
	{
		UpdateReadyState();
	}

	private async void UpdateReadyState()
	{
		if (ThemeDownloadController == null || AmbianceDownloadController == null
			|| TreatmentDownloadController == null || _controller == null) return;

		ReadyToPlay = ThemeDownloadController.Ready &&
					  AmbianceDownloadController.Ready &&
					  TreatmentDownloadController.Ready;

		if (ReadyToPlay)
			await _controller.InitSounds();
	}
	#endregion

	#region downloads
	private async Task StartDownloadsAsync()
	{
		if (await InternetService.IsConnectedAsync())
		{
			if (ThemeDownloadController == null || AmbianceDownloadController == null
				|| TreatmentDownloadController == null) return;

			ThemeDownloadController.StartDownload();
			AmbianceDownloadController.StartDownload();
			TreatmentDownloadController.StartDownload();
		}
	}
	#endregion

	#region actions
	private async void OnPlay()
	{
		if (!ReadyToPlay) return;
		await PremiumService.CheckPremium();

		if (_controller == null) return;
		await _controller.PlaySounds();
	}

	private async void OnSaveSession()
	{
		if (_controller == null) return;

		var popup = new SavePrepDialog(
			_selectedTreatment,
			_selectedAmbiance,
			_selectedMode,
			_selectedDuration,
			SelectedTheme,
			_controller.TreatmentSound,
			_controller.AmbianceSound,
			_controller.ThemeSound);

		await NavigationService.ShowPopupAsync(popup);

		// fallback:
		//await NavigationService.DisplayAlert("Save Session", "Session saved.", "OK");
	}
	#endregion

	#region INotifyPropertyChanged
	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged([CallerMemberName] string? name = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	private bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
	{
		if (Equals(field, value)) return false;
		field = value;
		OnPropertyChanged(name);
		return true;
	}
	#endregion
}
