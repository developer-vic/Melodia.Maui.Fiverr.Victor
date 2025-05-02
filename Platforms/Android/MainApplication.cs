using Android.App;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace MelodiaTherapy;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
		AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
	}

	protected override MauiApp CreateMauiApp()
	{ 
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CustomEntry", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
			handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent); 
            handler.PlatformView.SetTextColor(Android.Graphics.Color.Black); 
            handler.PlatformView.SetPadding(0, 0, 0, 0); 
		});
		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("CustomEditor", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
			handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent); 
            handler.PlatformView.SetTextColor(Android.Graphics.Color.Black); 
            handler.PlatformView.SetPadding(0, 0, 0, 0); 
		});
		Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("CustomPicker", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
			handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent); 
            handler.PlatformView.SetTextColor(Android.Graphics.Color.Black); 
            handler.PlatformView.SetPadding(0, 0, 0, 0); 
		});
		Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("CustomDatePicker", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
			handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent); 
            handler.PlatformView.SetTextColor(Android.Graphics.Color.Black); 
            handler.PlatformView.SetPadding(0, 0, 0, 0); 
		});
		Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("CustomTimePicker", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
			handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent); 
            handler.PlatformView.SetTextColor(Android.Graphics.Color.Black); 
            handler.PlatformView.SetPadding(0, 0, 0, 0); 
		});
		return MauiProgram.CreateMauiApp();
	}
}
