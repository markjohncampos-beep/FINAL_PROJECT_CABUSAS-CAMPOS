using System;
using System.Collections.Generic;
using System.Text;


using CampusITAssetTracker.Interfaces;

namespace CampusITAssetTracker.Models
{
    public class NetworkingHardware : CampusAsset, INetworkable
    {
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }

        public NetworkingHardware(
            string assetTag,
            decimal purchasePrice,
            string ipAddress,
            string macAddress)
            : base(assetTag, purchasePrice)
        {
            IPAddress = ipAddress;
            MACAddress = macAddress;
        }

        public override string GetMaintenanceStatus()
        {
            return Status == "In Repair"
                ? "Networking hardware requires repair."
                : "Networking hardware is operational.";
        }

        public override string GetDetails()
        {
            return base.GetDetails() +
                   $" | IP: {IPAddress} | MAC: {MACAddress}";
        }
    }
}

