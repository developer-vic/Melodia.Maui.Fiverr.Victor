using MelodiaTherapy.Models;
using Plugin.InAppBilling;

namespace MelodiaTherapy.Services
{
    public class InAppPurchaseService
    {
        private  static string[] ProductIds = ["premium_access"];

        public static async Task<List<Package>> GetAvailablePackagesAsync()
        {
            var packages = new List<Package>();

            try
            {
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();

                if (!connected)
                    return packages;

                var products = await billing.GetProductInfoAsync(ItemType.Subscription, ProductIds);

                foreach (var product in products)
                {
                    packages.Add(new Package
                    {
                        Identifier = product.ProductId,
                        Price = double.Parse(product.LocalizedPrice),
                        PriceFormatted = product.LocalizedPrice,
                        CurrencyCode = product.CurrencyCode,
                        SubscriptionPeriod = "Subscription",
                    });
                }

                return packages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                throw;
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        public static async Task<PurchaseResult> PurchasePackageAsync(Package package)
        {
            try
            {
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();

                if (!connected)
                    return new PurchaseResult { IsSuccessful = false, ErrorMessage = "Connection failed" };

                var purchase = await billing.PurchaseAsync(package.Identifier, ItemType.Subscription);

                if (purchase != null && purchase.State == PurchaseState.Purchased)
                {
                    return new PurchaseResult
                    {
                        IsSuccessful = true,
                        IsPremiumActive = true
                    };
                }

                return new PurchaseResult
                {
                    IsSuccessful = false,
                    ErrorMessage = "Purchase failed or cancelled"
                };
            }
            catch (InAppBillingPurchaseException ex)
            {
                return new PurchaseResult
                {
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                };
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }
    }
}