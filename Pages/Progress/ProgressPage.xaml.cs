using System.Threading.Tasks;
using MelodiaTherapy.Controllers;
using MelodiaTherapy.Enums;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Pages;

public partial class ProgressPage : ContentPage, IMenuDrawerVM
{
	private MelodiaController? _controller;
	private Dictionary<int, View>? _pages;

	public ProgressPage(object? navData = null)
	{
		InitializeComponent();
		Task.Factory.StartNew(() =>
		{
			MainThread.BeginInvokeOnMainThread(() =>
				menuDrawerView.Content = new Views.MenuDrawerView()
			);
			InitData(navData);
		});
	}

	private void InitData(object? navData)
	{
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
			MainThread.BeginInvokeOnMainThread(() =>
			{
				if (_controller.PageChanged == null)
					_controller.PageChanged += OnPageChanged;
				else
				{
					_controller.PageChanged -= OnPageChanged;
					_controller.PageChanged = null;
					_controller.PageChanged += OnPageChanged;
					OnPageChanged(_controller.SelectedPage);
				}

				progressIndicator.Controller = _controller;
			});

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

				_controller.SelectedPage = 4;

				MainThread.BeginInvokeOnMainThread(() =>
				{
					CurrentPageView.Content = _pages[4];
				});
			}
			else
			{
				_controller.SetSoundsValue(null, 100);
				_controller.SelectedPage = 0;

				MainThread.BeginInvokeOnMainThread(() =>
				{
					CurrentPageView.Content = _pages[0];
				});
			}
		}
	}

	private void OnPageChanged(int pageIndex)
	{
		if (_pages == null) return;

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
		else NavigationService.NavigateToStartPageAsync();
	}

	public void OnDrawerCloseClicked()
	{
		HideDrawer();
	}
	public void ShowDrawer()
	{
		// Animate drawer to slide in from right
		menuDrawerContainer.TranslationX = this.Width;
		menuDrawerContainer.IsVisible = true;

		menuDrawerContainer.FadeTo(1, 250, Easing.CubicOut);
		menuDrawerContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
	}
	public async void HideDrawer()
	{
		await Task.WhenAll(
			menuDrawerContainer.FadeTo(0, 250, Easing.CubicIn),
			menuDrawerContainer.TranslateTo(this.Width, 0, 250, Easing.CubicIn)
		);

		menuDrawerContainer.IsVisible = false;
	}

	private void OnDrawerClickOut(object sender, EventArgs e)
	{
		HideDrawer();
	}
	private void OnOpenMenuClicked(object sender, EventArgs e)
	{
		ShowDrawer();
	}

	private void imgBackTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
	{
		NavigationService.NavigateToStartPageAsync();
	}
}