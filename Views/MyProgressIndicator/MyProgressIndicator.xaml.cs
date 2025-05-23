using MelodiaTherapy.Controllers;

namespace MelodiaTherapy.Views;

public partial class MyProgressIndicator : ContentView
{
	private MelodiaController? controller;

	public MelodiaController? Controller { get => controller; set { controller = value; UpdateController(); } }

	private void UpdateController()
	{
		if (Controller != null)
		{
			Controller.PropertyChanged += (s, e) => UpdateUI();
			UpdateUI();
		}
	}

	public MyProgressIndicator()
	{
		InitializeComponent();
	}

	private void UpdateUI()
	{
		Task.Factory.StartNew(() =>
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				if (ProgressGrid.Children.Count == 0 && Controller != null)
				{
					ProgressGrid.Children.Clear();
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
			});
		});
	}
}
