namespace MelodiaTherapy.Widgets;

public class VerticalSpacer : ContentView
{
	public VerticalSpacer()
	{

	}
	public VerticalSpacer(double height)
	{
		HeightRequest = height; // Set the height
	}
}
public class HorizontalSpacer : ContentView
{
	public HorizontalSpacer()
	{

	}
	public HorizontalSpacer(double width)
	{
		WidthRequest = width; // Set the width
	}
}