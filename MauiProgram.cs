using CommunityToolkit.Maui;
using MelodiaTherapy.Controllers;
using MelodiaTherapy.Services;
using Microsoft.Extensions.Logging;
using Sharpnado.Tabs;

namespace MelodiaTherapy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseSharpnadoTabs(loggerEnable: false)
			.UseMauiCommunityToolkitMediaElement()
			//.UseAudio()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register your services
		builder.Services.AddSingleton<MyAudioHandler>();
		builder.Services.AddTransient<PageController>();
		builder.Services.AddTransient<MelodiaController>();


#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
