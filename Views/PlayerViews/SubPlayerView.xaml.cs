using CommunityToolkit.Mvvm.ComponentModel;
using MelodiaTherapy.Controllers;
using MelodiaTherapy.Enums;
using MelodiaTherapy.Helpers;

namespace MelodiaTherapy.Views.PlayerViews;

public partial class SubPlayerView : ContentView
{
    public SubPlayerView()
    {
        InitializeComponent();
        BindingContext = new SubPlayerViewModel();
    }
}

public partial class SubPlayerViewModel : ObservableObject
{
    private readonly DataController? _dataController;
    private readonly DownloadController? _downloadController;
    private readonly MelodiaController? _melodia;

    public SubPlayerViewModel()
    {
        _dataController = ServiceHelper.GetService<DataController>();
        _downloadController = ServiceHelper.GetService<DownloadController>();
        _melodia = ServiceHelper.GetService<MelodiaController>();

        UpdateBindings();
    }

    [ObservableProperty]
    private string? soundTitle;

    [ObservableProperty]
    private double soundValue;

    [ObservableProperty]
    private bool isReady;

    [ObservableProperty]
    private double downloadProgress;

    private void UpdateBindings()
    {
        if (_dataController == null || _melodia == null || _downloadController == null)
            return;

        var type = _dataController.Type;

        SoundTitle = type switch
        {
            DataType.Treatments => _melodia.SelectedTreatment.Name,
            DataType.Ambiances => _melodia.SelectedAmbiance.Name,
            DataType.Themes => _melodia.SelectedTheme.Name,
            _ => "Aucun nom"
        };

        SoundValue = type switch
        {
            DataType.Treatments => _melodia.TreatmentSound,
            DataType.Ambiances => _melodia.AmbianceSound,
            DataType.Themes => _melodia.ThemeSound, 
            _ => 0.0
        };

        IsReady = _downloadController.Ready;
        DownloadProgress = _downloadController.Progress;
    }
}

