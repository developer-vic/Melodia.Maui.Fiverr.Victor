using MelodiaTherapy.Controllers;

namespace MelodiaTherapy.Views;

public partial class ProgressLine : ContentView
{ 
	public int Index { get; set; }
	public MelodiaController? Controller { get; set; }

	public ProgressLine(int index, MelodiaController controller)
	{
		InitializeComponent();
		Index = index;
		Controller = controller;
		Controller.PropertyChanged += (s, e) => DrawLine();
		DrawLine();
	}

	void DrawLine()
	{
		LineStack.Children.Clear();
		int lines = 5;
		for (int i = 0; i < lines * 2 - 1; i++)
		{
			bool isBar = i % 2 == 0;
			LineStack.Children.Add(new BoxView
			{
				WidthRequest = isBar ? 6 : 3,
				HeightRequest = 1,
				Opacity = 0.5,
				BackgroundColor = isBar || Controller?.SelectedPage > Index 
					? Colors.Transparent : Color.FromArgb("#17142b")
			});
		}
	}
}
