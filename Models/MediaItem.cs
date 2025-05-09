using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.Models
{
    public class MediaItem
    {
        public string? Id { get; set; } // File path or URL
        public string? Title { get; set; }
        public string? Artist { get; set; }
        public TimeSpan Duration { get; set; }
        public Uri? ArtUri { get; set; }
    }

    public class PlaybackState
    {
        public bool Playing { get; set; }
        public TimeSpan Position { get; set; }
        public TimeSpan Duration { get; set; }
    }

}