using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Pages;

public partial class ProgressPage : ContentPage
{
	private readonly MelodiaController? _controller;
	private readonly Dictionary<int, View> _pages;

	public ProgressPage(object? navData = null)
	{
		InitializeComponent();

		_pages = new()
		{
			{ 0, new TreatmentPage() },
			{ 1, new AmbiancePage() },
			{ 2, new ListeningPage() },
			{ 3, new ListeningDurationPage() },
			{ 4, new ThemePage() }
			//{ 5, new PlayerPage() }
		};
		
		_controller = ServiceHelper.GetService<MelodiaController>();
		if (_controller != null)
		{
			_controller.PageChanged += OnPageChanged;
			progressIndicator.Controller = _controller;

			if (navData is Dictionary<string, object> data)
			{
				_controller.SelectedTheme = (ThemeModel)data["Theme"];
				_controller.SelectedTreatment = (TreatmentModel)data["Treat"];
				_controller.SelectedAmbiance = (AmbianceModel)data["Atmosphere"];
				_controller.SelectedListeningDuration = (ListenDurationModel)data["Duration"];
				_controller.SelectedListeningMode = (ListenTypeModel)data["Listen Mode"];
				_controller.SetSoundsValue(DataType.Ambiances, (double)data["Atmosphere Volume"]);
				_controller.SetSoundsValue(DataType.Treatments, (double)data["Treatment Volume"]);
				_controller.SetSoundsValue(DataType.Themes, (double)data["Theme Volume"]);

				MainThread.BeginInvokeOnMainThread(() =>
				{
					_controller.SelectedPage = 5;
					CurrentPageView.Content = _pages[5];
				});
			}
			else
			{
				_controller.SetSoundsValue(null, 100);
				_controller.SelectedPage = 0;
				CurrentPageView.Content = _pages[0];
			}
		}
	}

	private void OnPageChanged(int pageIndex)
	{
		if (_pages.TryGetValue(pageIndex, out var page))
		{
			CurrentPageView.Content = page;
		}
	}

	protected override bool OnBackButtonPressed()
	{
		GoBack();
		return true;
	}

	private async void GoBack()
	{
		if (_controller != null) await _controller.GoBack();
		else await NavigationService.GoBackAsync();
	}

	public void OnDrawerCloseClicked()
	{
		HideDrawer();
	}

	private void OnDrawerClickOut(object sender, EventArgs e)
	{
		HideDrawer();
	}

	private void ShowDrawer()
	{
		// Animate drawer to slide in from right
		menuDrawerContainer.TranslationX = this.Width;
		menuDrawerContainer.IsVisible = true;

		menuDrawerContainer.FadeTo(1, 250, Easing.CubicOut);
		menuDrawerContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
	}

	public async void HideDrawer()
	{
		// Animate drawer to slide out to right
		await Task.WhenAll(
			menuDrawerContainer.FadeTo(0, 250, Easing.CubicIn),
			menuDrawerContainer.TranslateTo(this.Width, 0, 250, Easing.CubicIn)
		);

		menuDrawerContainer.IsVisible = false;
	}

	private void OnOpenMenuClicked(object sender, EventArgs e)
	{
		ShowDrawer();
	}
}