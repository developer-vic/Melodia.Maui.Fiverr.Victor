using MelodiaTherapy.Controllers;

namespace MelodiaTherapy.Views;

public partial class ProgressNumber : ContentView
{
    public int Index { get; set; }
    public MelodiaController Controller { get; set; }

    public ProgressNumber(int index, MelodiaController controller)
    {
        InitializeComponent();
        Index = index;
        Controller = controller;
        Controller.PropertyChanged += (s, e) => UpdateVisual();
        UpdateVisual();

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => Controller._pageController.JumpToPage(Index);
        this.GestureRecognizers.Add(tapGesture);
    }

    void UpdateVisual()
    {
        IndexLabel.Text = (Index + 1).ToString();
        var isCurrent = Controller.SelectedPage == Index;
        var isPast = Controller.SelectedPage > Index;

        ProgressFrame.BackgroundColor = isCurrent || isPast ? Color.FromArgb("#17142b") : Colors.Transparent;
        ProgressFrame.StrokeThickness = isCurrent || isPast ? 2 : 1;
        ProgressFrame.Opacity = isCurrent || isPast ? 1 : 0.5;

        IndexLabel.TextColor = isCurrent ? Colors.White : Color.FromArgb("#76cec5");
        IndexLabel.FontSize = 18;
        IndexLabel.FontAttributes = FontAttributes.Bold;
    }
}