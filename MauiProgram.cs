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
				fonts.AddFont("fa-solid-900.otf", "FontAwesome");
			});

		// Register your services
		builder.Services.AddSingleton<LanguageController>();
		builder.Services.AddSingleton<MyAudioHandler>();
		builder.Services.AddSingleton<MelodiaController>();
		builder.Services.AddSingleton<TreatmentController>();
		builder.Services.AddSingleton<AmbianceController>();
		builder.Services.AddSingleton<ListenTypeController>();
		builder.Services.AddSingleton<ListenDurationController>();
		builder.Services.AddSingleton<ThemeController>();
		builder.Services.AddSingleton<DownloadController>();
		builder.Services.AddSingleton<DataController>();
	

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
