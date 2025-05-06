using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.Services
{
    public static class PremiumService
    {
        internal static bool IsEntitlementActive = false;

        public static Task CheckPremium()
        {
            //TODO Simulate check
            return Task.CompletedTask;
        }
    }
}