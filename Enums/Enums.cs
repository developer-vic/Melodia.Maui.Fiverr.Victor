namespace MelodiaTherapy.Enums
{
    public enum Enums
    {

    }
    public enum SoundStatus
    {
        Premium,
        Free
    }

    // public enum SubscriptionStatus
    // {
    //     Active,
    //     Expired
    // }

    public enum DataType
    {
        Data,
        Treatments,
        Ambiances,
        Themes,
        ListenTypes,
        DurationsNew,
        Isochrones,
        Binaurales
    }

    public enum DownloadStatus
    {
        NotDownloaded,
        FetchingDownload,
        Downloading,
        Downloaded,
        Ondownloand,
        Failed
    }


    public enum Environment
    {
        Production,
        Development
    }

}