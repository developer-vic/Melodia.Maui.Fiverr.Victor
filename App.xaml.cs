using MelodiaTherapy.Pages;

namespace MelodiaTherapy;

public partial class App : Application
{
    internal static string InternalPath = "";

    public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new SplashScreen());
	}
}