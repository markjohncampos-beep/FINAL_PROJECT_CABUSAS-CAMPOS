using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;


namespace CampusITAssetTracker.Models
{
    public class SoftwareLicense : CampusAsset
    {
        [JsonInclude]
        public int SubscriptionMonthsRemaining { get; private set; }

        public SoftwareLicense(
            string assetTag,
            decimal purchasePrice,
            int subscriptionMonthsRemaining)
            : base(assetTag, purchasePrice)
        {
            if (subscriptionMonthsRemaining < 0)
                throw new ArgumentException(
                    "Subscription months cannot be negative.");

            SubscriptionMonthsRemaining = subscriptionMonthsRemaining;
        }

        public override string GetMaintenanceStatus()
        {
            if (SubscriptionMonthsRemaining == 0)
                return "Software license has expired.";

            if (SubscriptionMonthsRemaining <= 3)
                return "Software license is expiring soon.";

            return "Software license is active.";
        }

        public override string GetDetails()
        {
            return base.GetDetails() +
                   $" | Months Remaining: {SubscriptionMonthsRemaining}";
        }
    }
}
