using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public static class UsbManager
   {
      public static List<UsbDeviceInfo> GetUSBDevices()
      {
         var devices = new List<UsbDeviceInfo>();

         using ( var searcher = new ManagementObjectSearcher( @"Select * From Win32_PnPEntity" ) )
         //using ( var searcher = new ManagementObjectSearcher( @"Select * From Win32_USBControllerDevice" ) )
         //using ( var searcher = new ManagementObjectSearcher( @"Select * From Win32_USBHub" ) )
         {
            using ( var collection = searcher.Get() )
            {
               foreach ( var device in collection )
               {
                  devices.Add( new UsbDeviceInfo(
                  (string) device.GetPropertyValue( "DeviceID" ),
                  (string) device.GetPropertyValue( "Description" ),
                  (string) device.GetPropertyValue( "PNPDeviceID" ),
                  (string) device.GetPropertyValue( "Name" ),
                  (string) device.GetPropertyValue( "Status" )
                  ) );
               }
            }
         }

         return devices;
      }
   }
}
