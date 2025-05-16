using System.Windows.Input;

namespace MelodiaTherapy.Views.PlayerViews;

public partial class SoundSliderView : ContentView
{
    public static readonly BindableProperty SoundProperty =
        BindableProperty.Create(nameof(Sound), typeof(double), typeof(SoundSliderView), 0.0, BindingMode.TwoWay);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(SoundSliderView), "Aucun nom");

    public static readonly BindableProperty IsReadyProperty =
        BindableProperty.Create(nameof(IsReady), typeof(bool), typeof(SoundSliderView), false);

    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(SoundSliderView), 0.0);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public double Sound
    {
        get => (double)GetValue(SoundProperty);
        set => SetValue(SoundProperty, value);
    }

    public bool IsReady
    {
        get => (bool)GetValue(IsReadyProperty);
        set => SetValue(IsReadyProperty, value);
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public bool NotIsReady
    {
        get => !IsReady;
        set { }
    }
    public string SoundIcon
    {
        get => Sound > 0 ? "loud.png" : "mute.png";
        set { }
    }

    public ICommand ToggleSoundCommand { get; }

    public SoundSliderView()
    {
        InitializeComponent();

        ToggleSoundCommand = new Command(() =>
        {
            Sound = Sound > 0 ? 0 : 50;
            
        });

        BindingContext = this;
    }
}
