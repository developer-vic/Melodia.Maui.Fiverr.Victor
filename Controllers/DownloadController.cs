using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MelodiaTherapy.Enums;

namespace MelodiaTherapy.Controllers
{
    public class DownloadController //: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DownloadStatus DownloadStatus { get; }

        private bool _ready;
        public bool Ready
        {
            get => _ready;
            protected set
            {
                if (_ready != value)
                {
                    _ready = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _progress;
        public double Progress
        {
            get => _progress;
            protected set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        // public abstract void StartDownload();

        // public abstract void StopDownload();

        // public abstract void CancelDownload();

        public DownloadController()
        {

        }
    }
}