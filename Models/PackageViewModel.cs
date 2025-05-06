using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.Models
{
    public class Package
    {
        public string Identifier { get; set; }
        public string OfferingIdentifier { get; set; }
        public double Price { get; set; }
        public string PriceFormatted { get; set; }
        public string CurrencyCode { get; set; }
        public string SubscriptionPeriod { get; set; }
    }

    public class Subscription
    {
        public string SubscriptionCode { get; set; }
        public string Description { get; set; }
        public string ParentSubscriptionCode { get; set; }
    }

    public class PurchaseResult
    {
        public bool IsSuccessful { get; set; }
        public bool IsPremiumActive { get; set; }
        public string ErrorMessage { get; set; }
    }
    
    public class PackageViewModel : BaseViewModel
    {
        private Package _package;
        private string _description;
        private double? _oldPrice;
        private bool _isSelected;
        private bool _isPopular;
        private double _discount;
        private string _currencyCode;

        public Package Package
        {
            get => _package;
            set => SetProperty(ref _package, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public double? OldPrice
        {
            get => _oldPrice;
            set => SetProperty(ref _oldPrice, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsPopular
        {
            get => _isPopular;
            set => SetProperty(ref _isPopular, value);
        }

        public double Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        public string CurrencyCode
        {
            get => _currencyCode;
            set => SetProperty(ref _currencyCode, value);
        }

        public string PriceFormatted => Package?.PriceFormatted;

        public string OldPriceFormatted => OldPrice.HasValue ?
            OldPrice.Value.ToString("F2", CultureInfo.CurrentCulture) :
            string.Empty;

        public bool HasDiscount => OldPrice.HasValue;

        public string DiscountText => $"  - {Discount:F2} {CurrencyCode}";
    }

}