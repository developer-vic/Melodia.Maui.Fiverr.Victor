using System.Globalization;

namespace MelodiaTherapy.Models
{
    public class MobileLanguageVM
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class MobileAmbianceVM
    {
        public virtual Guid Guid { get; set; }
        public Guid SongGuid { get; set; }
        public bool IsPremium { get; set; }
        public string IconCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MobileDurationVM
    {
        public string MobileIconCode { get; set; }
        public int Length { get; set; }
        public bool IsPremium { get; set; } = true;
        public string Description { get; set; }
    }

    public class MobileTreatmentVM
    {
        public virtual Guid Guid { get; set; }
        public bool IsPremium { get; set; } = true;
        public string IconCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<MobileTreatmentUrlVM> TreatmentUrls { get; set; } = new List<MobileTreatmentUrlVM>();
    }

    public class MobileTreatmentUrlVM
    {
        public Guid ListenTypeGuidID { get; set; }
        public Guid SongGuid { get; set; }
    }

    public class MobileListenTypeVM
    {
        public virtual Guid Guid { get; set; }
        public string IconCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MobileThemeVM
    {
        public Guid Guid { get; set; }
        public Guid SongGuid { get; set; }
        public bool IsPremium { get; set; } = true;
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        //public string Description { get; set; }
    }
}
