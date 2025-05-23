using MelodiaTherapy.Pages;

namespace MelodiaTherapy;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();
		//MainPage = new MauiApp1.AppShell(); 
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new NavigationPage(new SplashScreen()));
	}
}