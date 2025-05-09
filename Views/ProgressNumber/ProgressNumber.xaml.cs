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

        ProgressFrame.BackgroundColor = isCurrent ? Colors.LightBlue : isPast ? Colors.LightGray : Colors.Transparent;
        ProgressFrame.Stroke = isCurrent || isPast ? Colors.DarkBlue : Colors.LightBlue;
        IndexLabel.TextColor = isCurrent ? Colors.White : Colors.DarkBlue;
        IndexLabel.FontSize = 18;
        IndexLabel.FontAttributes = FontAttributes.Bold;
    }
} 