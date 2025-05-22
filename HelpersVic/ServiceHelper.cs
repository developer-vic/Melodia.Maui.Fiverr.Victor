namespace MelodiaTherapy.Helpers
{
    public static class ServiceHelper
    {
        public static T? GetService<T>() where T : class =>
            Application.Current?.Handler?.MauiContext?.Services?.GetService<T>();

        public static string FixMalformedUrl(string? urlSent)
        {
            if (string.IsNullOrWhiteSpace(urlSent)) return "image_not_supported.png";

            urlSent = urlSent.ToLower(); // Fix common mistake: duplicated 'therapy'
            string correctUrl = urlSent.Replace("melodiatherapytherapy.com", "melodiatherapy.com");

            correctUrl = Uri.EscapeUriString(correctUrl);
            return correctUrl;
        }
    }

}