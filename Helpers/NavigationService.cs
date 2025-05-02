
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;

namespace MelodiaTherapy.Helpers
{
    public class NavigationService
    {
        internal static void CloseDrawer()
        {
            if (Application.Current != null)
            {
                var page = Application.Current.Windows[0].Page;
                if (page != null)
                {
                    ContentPage currentPage = (ContentPage)page;
                    IMenuDrawerVM drawerVM = (IMenuDrawerVM)currentPage;
                    drawerVM.OnDrawerCloseClicked();
                }
            }
        }

        internal async static void PushPage(ContentPage contentPage)
        {
            if (Application.Current?.Windows?.Count > 0)
            {
                var mainPage = Application.Current?.Windows[0].Page;
                if (mainPage is not null)
                {
                    await mainPage.Navigation.PushAsync(contentPage);
                }
            }
        }

        internal async static void OpenDialog(ContentPage modal)
        {
            if (Application.Current != null)
            {
                 await Application.Current.Windows[0].Navigation.PushModalAsync(modal);
            }
        }

        internal async static void ToastText(string message)
        {
            //await Toast.Make(message, ToastDuration.Short).Show();
            var snackbar = Snackbar.Make(message, null, "OK", TimeSpan.FromSeconds(3));
            await snackbar.Show();
        }

    }
}