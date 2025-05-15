using System.ComponentModel;
using MelodiaTherapy.Globals;
using MelodiaTherapy.Services;
using System.Runtime.CompilerServices;
using MelodiaTherapy.Enums;

namespace MelodiaTherapy.Controllers;

public class SoundDownloadController : INotifyPropertyChanged
{
    private const string Ext = ".mp3";
    private static readonly string BaseUrl = Config.ApiUrl;
    private readonly string? _songGuid;
    private readonly DataType _type;
    private readonly string _url;
    private double _progress;
    private bool _ready;
    private bool _isDownloading;
    private DownloadStatus _downloadStatus;

    public event PropertyChangedEventHandler? PropertyChanged;

    public SoundDownloadController(string? songGuid, TimeSpan duration, DataType type, double progress = 0.0)
    {
        _songGuid = songGuid;
        _type = type;
        _progress = progress;

        string durationMinutes = ((int)duration.TotalMinutes).ToString();
        _url = $"{BaseUrl}{type}/download/{songGuid}/{durationMinutes}";
        _downloadStatus = CheckDownloadStatus(type, songGuid, durationMinutes);
        _ready = _downloadStatus == DownloadStatus.Downloaded;
    }

    public bool Ready
    {
        get => _ready;
        set => SetProperty(ref _ready, value);
    }

    public DownloadStatus DownloadStatus
    {
        get => _downloadStatus;
        set
        {
            SetProperty(ref _downloadStatus, value);
            ReadyChanged?.Invoke();
        }
    }

    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }
    public Action? ReadyChanged { get; internal set; }

    public void StartDownload()
    {
        if (_downloadStatus == DownloadStatus.NotDownloaded)
        {
            if (!string.IsNullOrEmpty(_songGuid))
            {
                _ = DoDownloadAsync(_url);
            }
            else
            {
                DownloadStatus = DownloadStatus.Downloaded;
                Ready = true;
            }
        }
    }

    public void StopDownload()
    {
        if (_isDownloading)
        {
            _isDownloading = false;
            DownloadStatus = DownloadStatus.NotDownloaded;
            Progress = 0.0;
        }
    }

    public void CancelDownload() => StopDownload();

    private async Task DoDownloadAsync(string url)
    {
        if (!await InternetService.IsConnectedAsync())
        {
            return;
        }

        var fileName = url.Substring(url.LastIndexOf("/") + 1) + Ext;
        var tempFileName = fileName.Replace(".mp3", ".temp");

        _isDownloading = true;
        DownloadStatus = DownloadStatus.FetchingDownload;

        try
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            var contentLength = response.Content.Headers.ContentLength ?? 0;
            if (!_isDownloading || contentLength < 1000)
                return;

            DownloadStatus = DownloadStatus.Downloading;

            var file = CreateFileAsync(_type, tempFileName);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.OpenWrite(file.FullName);

            var buffer = new byte[8192];
            int bytesRead;
            long totalRead = 0;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0 && _isDownloading)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalRead += bytesRead;
                Progress = (double)totalRead / contentLength;
            }

            if (!_isDownloading)
            {
                file.Delete();
                return;
            }

            DownloadStatus = DownloadStatus.Downloaded;
            Ready = true;
            file.MoveTo(Path.Combine(Config.InternalPath, "sounds", _type.ToString(), fileName));
            _isDownloading = false;
        }
        catch (Exception ex)
        {
            DownloadStatus = DownloadStatus.Failed;
            _isDownloading = false;
            Console.WriteLine($"Download failed: {ex.Message}");
        }
    }

    private FileInfo CreateFileAsync(DataType type, string fileName)
    {
        var dirPath = Path.Combine(Config.InternalPath, "sounds", type.ToString());
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, fileName);
        return new FileInfo(filePath);
    }

    private DownloadStatus CheckDownloadStatus(DataType type, string? guid, string duration)
    {
        if (string.IsNullOrEmpty(guid)) return DownloadStatus.Downloaded;

        var dirPath = Path.Combine(Config.InternalPath, "sounds", type.ToString());
        var fileName = $"{guid}_{duration}{Ext}";
        return File.Exists(Path.Combine(dirPath, fileName)) ? DownloadStatus.Downloaded : DownloadStatus.NotDownloaded;
    }

    private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
    {
        if (!Equals(storage, value))
        {
            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
