using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileAccess
{
   public class UsbDeviceInfo
   {
      public string DeviceId
      {
         get;
         private set;
      }

      public string PnpDeviceId
      {
         get;
         private set;
      }

      public string Description
      {
         get;
         private set;
      }

      public string Name
      {
         get;
         private set;
      }

      public string Status
      {
         get;
         private set;
      }

      public UsbDeviceInfo( string deviceId, string description, string pnpDeviceId, string name, string status )
      {
         this.DeviceId = deviceId;
         this.Description = description;
         this.PnpDeviceId = pnpDeviceId;
         this.Name = name;
         this.Status = status;
      }

      public UsbDeviceInfo( string deviceId, string description, string pnpDeviceId )
         : this( deviceId, description, pnpDeviceId, String.Empty, String.Empty )
      {
      }

      public UsbDeviceInfo( string deviceId, string description )
         : this( deviceId, description, String.Empty, String.Empty, String.Empty )
      {
      }
   }
}
