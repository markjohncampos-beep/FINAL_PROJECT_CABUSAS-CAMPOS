using CampusITAssetTracker.Interfaces;
using CampusITAssetTracker.Validators;
using System;
using System.Collections.Generic;
using System.Text;


namespace CampusITAssetTracker.Models
{
    public class Workstation : CampusAsset, IBorrowable
    {
        public string BorrowerId { get; private set; }

        public Workstation(string assetTag, decimal purchasePrice)
            : base(assetTag, purchasePrice)
        {
            BorrowerId = "";
        }


        public void CheckOut(string borrowerId)
        {

            if (Status != "Available")
                throw new InvalidOperationException(
                    "Only available assets can be checked out.");

            if (!AssetValidator.IsValidBorrowerId(borrowerId))
                throw new ArgumentException(
                    "Invalid Borrower ID! Use format 2026-00125.");

            BorrowerId = borrowerId;
            SetStatus("Borrowed");
        }


        public void ReturnAsset()
        {
            if (Status != "Borrowed")
                throw new InvalidOperationException(
                    "This workstation is not borrowed.");

            BorrowerId = "";
            SetStatus("Available");
        }

        public override string GetMaintenanceStatus()
        {
            return Status == "In Repair"
                ? "Workstation requires repair."
                : "Workstation maintenance is normal.";
        }

        public override string GetDetails()
        {
            return base.GetDetails() +
                   $" | Borrower: {(BorrowerId == "" ? "None" : BorrowerId)}";
        }
    }
}
