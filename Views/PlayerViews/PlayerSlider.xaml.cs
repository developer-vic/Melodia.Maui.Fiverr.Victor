using MelodiaTherapy.Models;

namespace MelodiaTherapy.Views.PlayerViews
{
    public partial class PlayerSlider : ContentView
    {
        public PlayerSlider()
        {
            InitializeComponent();
        }

        private void OnSliderDragStarted(object sender, EventArgs e)
        {
            if (BindingContext is MelodiaViewModel vm)
            {
                vm.IsDragging = true;
            }
        }

        private void OnSliderDragCompleted(object sender, EventArgs e)
        {
            if (BindingContext is MelodiaViewModel vm)
            {
                vm.IsDragging = false;
                vm.UpdatePositionFromSlider();
            }
        }
    }
    public class MelodiaViewModel : BaseViewModel
    {
        private double _sliderValue;
        public double SliderValue
        {
            get => _sliderValue;
            set
            {
                if (SetProperty(ref _sliderValue, value) && !IsDragging)
                {
                    UpdateTimeFromSlider();
                }
            }
        }

        public double MaxDuration => SelectedListeningDuration?.Duration.TotalSeconds ?? 0;

        public string FormattedSliderValue =>
            TimeSpan.FromSeconds(SliderValue).ToString(@"mm\:ss");

        public string FormattedMaxDuration =>
            SelectedListeningDuration?.Duration.ToString(@"mm\:ss") ?? "00:00";

        public bool IsDragging { get; set; }

        public ListenDurationModel? SelectedListeningDuration { get; set; }
        private TimeSpan _position;
        public TimeSpan Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        // Timer to simulate audio playback
        private System.Timers.Timer? _playbackTimer;

        public void SmartPlay()
        {
            _playbackTimer?.Stop();
            _playbackTimer = new System.Timers.Timer(1000); // 1 second interval
            _playbackTimer.Elapsed += (s, e) =>
            {
                if (!IsDragging)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Position = Position.Add(TimeSpan.FromSeconds(1));
                        SliderValue = Position.TotalSeconds;

                        if (Position.TotalSeconds >= MaxDuration - 10)
                        {
                            _playbackTimer.Stop();
                            IsDragging = true;
                            SliderValue = 1;
                            IsDragging = false;
                            Position = TimeSpan.Zero;
                            SmartPlay(); // restart
                        }
                    });
                }
            };
            _playbackTimer.Start();
        }

        private void UpdateTimeFromSlider()
        {
            if (!IsDragging)
            {
                var seconds = (int)SliderValue;
                Position = TimeSpan.FromSeconds(seconds);

                // Loop reset logic
                if (seconds >= MaxDuration - 10)
                {
                    IsDragging = true;
                    SliderValue = 1;
                    IsDragging = false;
                    Position = TimeSpan.Zero;
                    SmartPlay();
                }
            }
        }

        public void UpdatePositionFromSlider()
        {
            Position = TimeSpan.FromSeconds((int)SliderValue);
        }
    }

}