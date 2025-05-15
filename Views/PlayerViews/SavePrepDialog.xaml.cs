using Newtonsoft.Json; 
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Views.PlayerViews;

public partial class SavePrepDialog : ContentPage
{
    TreatmentModel? treatment;
    AmbianceModel? atmosphere;
    ListenTypeModel? listeningMode;
    ListenDurationModel? duration;
    ThemeModel? theme;
    double treatmentVolume, atmosphereVolume, themeVolume;

    public SavePrepDialog(
        TreatmentModel? treatment,
        AmbianceModel? atmosphere,
        ListenTypeModel? listeningMode,
        ListenDurationModel? duration,
        ThemeModel? theme,
        double treatmentVolume,
        double atmosphereVolume,
        double themeVolume)
    {
        InitializeComponent();
        this.treatment = treatment;
        this.atmosphere = atmosphere;
        this.listeningMode = listeningMode;
        this.duration = duration;
        this.theme = theme;
        this.treatmentVolume = treatmentVolume;
        this.atmosphereVolume = atmosphereVolume;
        this.themeVolume = themeVolume;
        TitleEntry.Text = "Untitled1";
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(title))
        {
            ErrorLabel.Text = "You can't submit with an empty input";
            ErrorLabel.IsVisible = true;
            return;
        }

        var newEntry = new Dictionary<string, object>
        {
            [title] = new
            {
                Treat = treatment,
                Atmosphere = atmosphere,
                Theme = theme,
                ListenMode = listeningMode,
                Duration = duration,
                TreatmentVolume = treatmentVolume,
                AtmosphereVolume = atmosphereVolume,
                ThemeVolume = themeVolume
            }
        };

        try
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "saved_session.json");
            List<Dictionary<string, object>> settingsList = new();

            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                settingsList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content) ?? [];
            }

            bool exists = settingsList.Any(entry => entry.ContainsKey(title));
            if (exists)
            {
                ErrorLabel.Text = "This session already exists, try another title";
                ErrorLabel.IsVisible = true;
                return;
            }

            settingsList.Add(newEntry);
            File.WriteAllText(path, JsonConvert.SerializeObject(settingsList, Formatting.Indented));

            await DisplayAlert("Success", "Settings saved successfully", "OK");
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save settings: {ex.Message}", "OK");
        }
    }

    async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
