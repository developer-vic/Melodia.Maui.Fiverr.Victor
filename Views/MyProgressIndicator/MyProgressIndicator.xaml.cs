using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;

namespace MelodiaTherapy.Views;

public partial class MyProgressIndicator : ContentView
{
	public MelodiaController? Controller { get; set; }

	public MyProgressIndicator()
	{
		InitializeComponent();
		Controller = ServiceHelper.GetService<MelodiaController>();
		if (Controller == null)
			return;

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
				int colIndex = i * 2;
				var progressNumber = new ProgressNumber(i, Controller);
				Grid.SetColumn(progressNumber, colIndex);
				ProgressGrid.Children.Add(progressNumber);

				if (i < 4)
				{
					ProgressGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
					colIndex = colIndex + 1;
					var progressLine = new ProgressLine(i, Controller);
					Grid.SetColumn(progressLine, colIndex);
					ProgressGrid.Children.Add(progressLine);
				}
			}
		}
	}
}
