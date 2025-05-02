using Foundation;
using UIKit;

namespace MelodiaTherapy;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp()
	{
		var mauiApp = MauiProgram.CreateMauiApp();

		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CustomEntry", (handler, view) =>
		{
			handler.PlatformView.BackgroundColor = UIColor.Clear;
			handler.PlatformView.BorderStyle = UITextBorderStyle.None;
		});
		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("CustomEditor", (handler, view) =>
		{
			handler.PlatformView.BackgroundColor = UIColor.Clear;
			//handler.PlatformView.BorderStyle = UITextBorderStyle.None;
		});
		Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("CustomPicker", (handler, view) =>
		{
			handler.PlatformView.BackgroundColor = UIColor.Clear;
			handler.PlatformView.BorderStyle = UITextBorderStyle.None;
		});
		Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("CustomDatePicker", (handler, view) =>
		{
			handler.PlatformView.BackgroundColor = UIColor.Clear;
			handler.PlatformView.BorderStyle = UITextBorderStyle.None;
		});
		Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("CustomTimePicker", (handler, view) =>
		{
			handler.PlatformView.BackgroundColor = UIColor.Clear;
			handler.PlatformView.BorderStyle = UITextBorderStyle.None;
		});

		return mauiApp;
	}
}
