using System;
using System.Collections.Generic;
using System.Text;


namespace CampusITAssetTracker.Interfaces
{
    public interface INetworkable
    {
        string IPAddress { get; set; }
        string MACAddress { get; set; }
    }
}
