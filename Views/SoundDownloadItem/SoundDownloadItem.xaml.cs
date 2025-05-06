using MelodiaTherapy.Models;

namespace MelodiaTherapy.Views;

public partial class SoundDownloadItem : ContentView
{
	public static readonly BindableProperty NameProperty =
		BindableProperty.Create(nameof(Name), typeof(string), typeof(SoundDownloadItem), propertyChanged: OnPropertyChanged);

	public static readonly BindableProperty DurationProperty =
		BindableProperty.Create(nameof(Duration), typeof(string), typeof(SoundDownloadItem), propertyChanged: OnPropertyChanged);

	public static readonly BindableProperty SongGuidProperty =
		BindableProperty.Create(nameof(SongGuid), typeof(string), typeof(SoundDownloadItem), propertyChanged: OnPropertyChanged);

	public static readonly BindableProperty TypeProperty =
		BindableProperty.Create(nameof(Type), typeof(DataType), typeof(SoundDownloadItem), propertyChanged: OnPropertyChanged);

	public string Name
	{
		get => (string)GetValue(NameProperty);
		set => SetValue(NameProperty, value);
	}

	public string Duration
	{
		get => (string)GetValue(DurationProperty);
		set => SetValue(DurationProperty, value);
	}

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

	public SoundDownloadItem()
	{
		InitializeComponent();
	}

	static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (SoundDownloadItem)bindable;
		control.UpdateView();
	}

	void UpdateView()
	{
		NameLabel.Text = $"{Name} ";
		DurationLabel.Text = $"({Duration})";
		DownloadButton.Initialize(SongGuid, Type, Duration);
	}
}