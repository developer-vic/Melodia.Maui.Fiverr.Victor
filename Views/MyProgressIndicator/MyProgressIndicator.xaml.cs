using MelodiaTherapy.Controllers;

namespace MelodiaTherapy.Views;

public partial class MyProgressIndicator : ContentView
{
	public MelodiaController? Controller { get; set; }

	public MyProgressIndicator()
	{
		InitializeComponent();
	}

	public MyProgressIndicator(MelodiaController controller)
	{
		InitializeComponent();
		Controller = controller;
		Controller.PropertyChanged += (s, e) => UpdateUI();
		UpdateUI();
	}

	private void UpdateUI()
	{
		ProgressGrid.Children.Clear();

		if (Controller?.SelectedPage <= 4)
		{
			for (int i = 0; i <= 4; i++)
			{
				ProgressGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				ProgressGrid.Children.Add(new ProgressNumber(i, Controller)); //, i * 2, 0

				if (i < 4)
				{
					ProgressGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
					ProgressGrid.Children.Add(new ProgressLine(i, Controller)); //, i * 2 + 1, 0
				}
			}
		}
	}
}