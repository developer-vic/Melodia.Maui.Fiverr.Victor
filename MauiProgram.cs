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
		builder.Services.AddTransient<LanguageController>();
		builder.Services.AddSingleton<MyAudioHandler>();
		builder.Services.AddTransient<MelodiaController>();
		builder.Services.AddTransient<TreatmentController>();
		builder.Services.AddTransient<AmbianceController>();
		builder.Services.AddTransient<ListenTypeController>();
		builder.Services.AddTransient<ListenDurationController>();
		builder.Services.AddTransient<ThemeController>();
		builder.Services.AddTransient<DownloadController>();
	

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
