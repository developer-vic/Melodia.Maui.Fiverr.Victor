using MelodiaTherapy.Enums;
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Views;

public partial class DownloadManagerButton : ContentView
{
	public enum DownloadStatus { NotDownloaded, Downloading, Downloaded }

	public static readonly BindableProperty SongGuidProperty =
		BindableProperty.Create(nameof(SongGuid), typeof(string), typeof(DownloadManagerButton));

	public static readonly BindableProperty TypeProperty =
		BindableProperty.Create(nameof(Type), typeof(DataType), typeof(DownloadManagerButton));

	public static readonly BindableProperty DurationProperty =
		BindableProperty.Create(nameof(Duration), typeof(string), typeof(DownloadManagerButton));

	private DownloadStatus _status = DownloadStatus.NotDownloaded;

	public string SongGuid
	{
		get => (string)GetValue(SongGuidProperty);
		set => SetValue(SongGuidProperty, value);
	}

	public DataType Type
	{
		get => (DataType)GetValue(TypeProperty);
		set => SetValue(TypeProperty, value);
	}

	public string Duration
	{
		get => (string)GetValue(DurationProperty);
		set => SetValue(DurationProperty, value);
	}

	public DownloadManagerButton()
	{
		InitializeComponent();
	}

	public void Initialize(string guid, DataType type, string duration)
	{
		SongGuid = guid;
		Type = type;
		Duration = duration;

		// Check existing status from storage
		// Example: _status = await StorageController.Instance.GetStatus(SongGuid);
		UpdateView();
	}

	private async void OnTapped(object sender, EventArgs e)
	{
		switch (_status)
		{
			case DownloadStatus.NotDownloaded:
				_status = DownloadStatus.Downloading;
				UpdateView();
				await StartDownload();
				break;

			case DownloadStatus.Downloading:
				_status = DownloadStatus.NotDownloaded;
				UpdateView();
				await CancelDownload();
				break;

			case DownloadStatus.Downloaded:
				_status = DownloadStatus.NotDownloaded;
				UpdateView();
				await DeleteDownload();
				break;
		}
	}

	private void UpdateView()
	{
		switch (_status)
		{
			case DownloadStatus.NotDownloaded:
				ProgressBar.IsVisible = false;
				ButtonText.Text = "Télécharger";
				ButtonGrid.BackgroundColor = Colors.Green;
				break;

			case DownloadStatus.Downloading:
				ProgressBar.IsVisible = true;
				ButtonText.Text = "";
				ButtonGrid.BackgroundColor = Colors.Transparent;
				break;

			case DownloadStatus.Downloaded:
				ProgressBar.IsVisible = false;
				ButtonText.Text = "Supprimer";
				ButtonGrid.BackgroundColor = Colors.Red;
				break;
		}
	}

	private async Task StartDownload()
	{
		// Simulate download
		for (int i = 0; i <= 100; i += 5)
		{
			await Task.Delay(100);
			ProgressBar.Progress = i / 100.0;
		}

		_status = DownloadStatus.Downloaded;
		UpdateView();
	}

	private Task CancelDownload()
	{
		// Add logic to cancel download
		return Task.CompletedTask;
	}

	private Task DeleteDownload()
	{
		// Add logic to delete download
		return Task.CompletedTask;
	}
}