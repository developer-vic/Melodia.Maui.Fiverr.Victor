using System.Collections.ObjectModel;
using MelodiaTherapy.Enums;
using MelodiaTherapy.Models;
using MelodiaTherapy.Services;

namespace MelodiaTherapy.Pages;

public partial class DownloadCenterPage : ContentPage
{
	public ObservableCollection<SoundDownloadItemModel> Ambiances { get; set; } = new();
	public ObservableCollection<SoundDownloadItemModel> Themes { get; set; } = new();
	public ObservableCollection<SoundDownloadItemModel> Isochrones { get; set; } = new();
	public ObservableCollection<SoundDownloadItemModel> Binaurales { get; set; } = new();

	public DownloadCenterPage()
	{
		InitializeComponent();

		LoadData();

		AmbiancesCollectionView.ItemsSource = Ambiances;
		ThemesCollectionView.ItemsSource = Themes;
		IsochronesCollectionView.ItemsSource = Isochrones;
		BinauralesCollectionView.ItemsSource = Binaurales;
	}

	void LoadData()
	{
		Task.Factory.StartNew(async () =>
		{
			var treatments = await MobileServices.GetData<MobileTreatmentVM>("treatments.json");
			var ambiances = await MobileServices.GetData<MobileAmbianceVM>("ambiances.json");
			var durationList = await MobileServices.GetData<MobileDurationVM>("durations.json");
			var themes = await MobileServices.GetData<MobileThemeVM>("themes.json");

			MainThread.BeginInvokeOnMainThread(() =>
			{
				ambiances.RemoveAt(ambiances.Count - 1);
				foreach (var ambiance in ambiances)
				{
					foreach (var duration in durationList)
					{
						Ambiances.Add(new SoundDownloadItemModel
						{
							Name = ambiance.Name,
							SongGuid = ambiance.SongGuid.ToString(),
							Duration = duration.Description,
							Type = DataType.Ambiances
						});
					}
				}

				themes.RemoveAt(themes.Count - 1);
				foreach (var theme in themes)
				{
					foreach (var duration in durationList)
					{
						Themes.Add(new SoundDownloadItemModel
						{
							Name = theme.Name,
							SongGuid = theme.SongGuid.ToString(),
							Duration = duration.Description,
							Type = DataType.Themes
						});
					}
				}

				treatments.RemoveAt(treatments.Count - 1);
				foreach (var treatment in treatments)
				{
					foreach (var duration in durationList)
					{
						Isochrones.Add(new SoundDownloadItemModel
						{
							Name = treatment.Name,
							SongGuid = treatment.TreatmentUrls.Last().SongGuid.ToString(),
							Duration = duration.Description,
							Type = DataType.Treatments
						});

						Binaurales.Add(new SoundDownloadItemModel
						{
							Name = treatment.Name,
							SongGuid = treatment.TreatmentUrls.First().SongGuid.ToString(),
							Duration = duration.Description,
							Type = DataType.Treatments
						});
					}
				}
			});
		});
	}
}