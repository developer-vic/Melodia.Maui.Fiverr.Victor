using System.Windows.Input; 
using MelodiaTherapy.Enums;

namespace MelodiaTherapy.Views.PlayerViews
{
    public class DownloadButtonView : ContentView
    {
        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create(nameof(Status), typeof(DownloadStatus), typeof(DownloadButtonView), DownloadStatus.NotDownloaded, propertyChanged: OnStatusChanged);
 
        public static readonly BindableProperty DownloadProgressProperty =
            BindableProperty.Create(nameof(DownloadProgress), typeof(double), typeof(DownloadButtonView), 0.0);

        public static readonly BindableProperty OnDownloadCommandProperty =
            BindableProperty.Create(nameof(OnDownloadCommand), typeof(ICommand), typeof(DownloadButtonView));

        public static readonly BindableProperty OnCancelCommandProperty =
            BindableProperty.Create(nameof(OnCancelCommand), typeof(ICommand), typeof(DownloadButtonView));

        public DownloadStatus Status
        {
            get => (DownloadStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public double DownloadProgress
        {
            get => (double)GetValue(DownloadProgressProperty);
            set => SetValue(DownloadProgressProperty, value);
        }

        public ICommand OnDownloadCommand
        {
            get => (ICommand)GetValue(OnDownloadCommandProperty);
            set => SetValue(OnDownloadCommandProperty, value);
        }

        public ICommand OnCancelCommand
        {
            get => (ICommand)GetValue(OnCancelCommandProperty);
            set => SetValue(OnCancelCommandProperty, value);
        }

        private readonly Grid _mainLayout;
        private readonly Image _downloadIcon;
        private readonly ActivityIndicator _fetchingIndicator;
        private readonly ProgressBar _progressBar;

        public DownloadButtonView()
        {
            _mainLayout = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            _downloadIcon = new Image
            {
                Source = "download_icon.png",
                WidthRequest = 25,
                HeightRequest = 25,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            _fetchingIndicator = new ActivityIndicator
            {
                IsRunning = false,
                IsVisible = false,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            _progressBar = new ProgressBar
            {
                Progress = 0,
                IsVisible = false,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => HandleButtonTap();
            this.GestureRecognizers.Add(tapGesture);

            _mainLayout.Children.Add(_downloadIcon);
            _mainLayout.Children.Add(_fetchingIndicator);
            _mainLayout.Children.Add(_progressBar);

            Content = _mainLayout;
        }

        private void HandleButtonTap()
        {
            switch (Status)
            {
                case DownloadStatus.NotDownloaded:
                    OnDownloadCommand?.Execute(null);
                    break;
                case DownloadStatus.Downloading:
                    OnCancelCommand?.Execute(null);
                    break;
                case DownloadStatus.FetchingDownload:
                case DownloadStatus.Downloaded:
                case DownloadStatus.Ondownloand:
                case DownloadStatus.Failed:
                    // No action
                    break;
            }
        }

        private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is DownloadButtonView view && newValue is DownloadStatus newStatus)
            {
                view.UpdateVisualState(newStatus);
            }
        }

        private void UpdateVisualState(DownloadStatus status)
        {
            _downloadIcon.IsVisible = status == DownloadStatus.NotDownloaded;
            _fetchingIndicator.IsVisible = status == DownloadStatus.FetchingDownload;
            _fetchingIndicator.IsRunning = status == DownloadStatus.FetchingDownload;
            _progressBar.IsVisible = status == DownloadStatus.Downloading;
        }

        public void UpdateProgress(double progress)
        {
            _progressBar.Progress = progress;
        }
    }
}