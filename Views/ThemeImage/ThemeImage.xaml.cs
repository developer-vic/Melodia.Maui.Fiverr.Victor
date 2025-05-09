using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;

namespace MelodiaTherapy.Views;

public partial class ThemeImage : ContentView
{
	private readonly MelodiaController? _controller;

	public ThemeImage()
	{
		InitializeComponent();
		_controller = ServiceHelper.GetService<MelodiaController>();
		BindingContext = _controller;

		if (_controller != null)
			_controller.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(_controller.SelectedTheme) ||
					e.PropertyName == nameof(_controller.SelectedPage))
				{
					UpdateImage();
				}
			};

		UpdateImage();
	}

	private void UpdateImage()
	{
		if (_controller != null && _controller.SelectedPage == 5)
		{
			var imagePath = ThemeController.GetLocalImagePath(_controller.SelectedTheme);
			if (File.Exists(imagePath))
			{
				ThemeBackgroundImage.Source = ImageSource.FromFile(imagePath);
				ThemeBackgroundImage.IsVisible = true;
			}
			else
			{
				ThemeBackgroundImage.IsVisible = false;
			}
		}
		else
		{
			ThemeBackgroundImage.IsVisible = false;
		}
	}
}