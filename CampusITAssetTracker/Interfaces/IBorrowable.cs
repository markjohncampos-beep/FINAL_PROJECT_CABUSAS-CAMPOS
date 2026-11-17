using System;
using System.Collections.Generic;
using System.Text;


namespace CampusITAssetTracker.Interfaces
{
    public interface IBorrowable
    {
        void CheckOut(string borrowerId);
        void ReturnAsset();
    }
}
