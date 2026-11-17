
using System;
using System.Text.Json.Serialization;

namespace CampusITAssetTracker.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "AssetType")]
    [JsonDerivedType(typeof(Workstation), "Workstation")]
    [JsonDerivedType(typeof(NetworkingHardware), "NetworkingHardware")]
    [JsonDerivedType(typeof(Peripheral), "Peripheral")]
    [JsonDerivedType(typeof(SoftwareLicense), "SoftwareLicense")]
    public abstract class CampusAsset
    {
        [JsonInclude]
        public string AssetTag { get; private set; }

        [JsonInclude]
        public decimal PurchasePrice { get; private set; }

        [JsonInclude]
        public string Status { get; private set; }

        protected CampusAsset(string assetTag, decimal purchasePrice)
        {
            AssetTag = assetTag;
            PurchasePrice = purchasePrice;
            Status = "Available";
        }

        public void UpdatePurchasePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException(
                    "Price must be greater than zero.");

            PurchasePrice = newPrice;
        }

        public void MarkForRepair()
        {
            if (Status == "Borrowed")
                throw new InvalidOperationException(
                    "Return the asset before marking it for repair.");

            Status = "In Repair";
        }

        public void MarkAvailable()
        {
            if (Status == "Borrowed")
                throw new InvalidOperationException(
                    "Return the asset before marking it available.");

            Status = "Available";
        }

        protected void SetStatus(string newStatus)
        {
            Status = newStatus;
        }

        public abstract string GetMaintenanceStatus();

        public virtual string GetDetails()
        {
            return $"Tag: {AssetTag} | " +
                   $"Price: {PurchasePrice:C} | " +
                   $"Status: {Status}";
        }
    }
}
