using MelodiaTherapy.Models;
using MelodiaTherapy.Services;

namespace MelodiaTherapy;

public partial class MainPage : ContentPage
{ 
	public MainPage()
	{
		InitializeComponent();

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var Languages = await MobileServices.GetData<MobileLanguageVM>("languages.json");
        var Treatments = await MobileServices.GetData<MobileTreatmentVM>("treatments.json");
        var Ambiances = await MobileServices.GetData<MobileAmbianceVM>("ambiances.json");
        var ListenTypes = await MobileServices.GetData<MobileListenTypeVM>("listentypes.json");
        var Durations = await MobileServices.GetData<MobileDurationVM>("durations.json");
        var Themes = await MobileServices.GetData<MobileThemeVM>("themes.json");
    }

}

