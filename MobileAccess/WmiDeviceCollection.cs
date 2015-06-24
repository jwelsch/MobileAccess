using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   //public enum WmiDeviceTable
   //{
   //   PnPEntity,
   //   USBController,
   //   USBControllerDevice,
   //   USBHub
   //}

   //public class WmiDeviceCollection : Collection<IDevice>
   //{
   //   private WmiDeviceCollection()
   //   {
   //   }

   //   public static WmiDeviceCollection Create()
   //   {
   //      var table = WmiDeviceTable.PnPEntity;
   //      var tableString = string.Empty;

   //      if ( table >= WmiDeviceTable.PnPEntity && table <= WmiDeviceTable.USBHub )
   //      {
   //         tableString = "Win32_" + table.ToString();
   //      }

   //      var collection = new WmiDeviceCollection();

   //      var queryString = "SELECT * FROM " + tableString;

   //      using ( var searcher = new ManagementObjectSearcher( queryString ) )
   //      {
   //         using ( var devices = searcher.Get() )
   //         {
   //            foreach ( var device in devices )
   //            {
   //               collection.Add( new WmiDevice()
   //               {
   //                  DeviceID = (string) device.Properties["DeviceID"].Value,
   //                  Name = (string) device.Properties["Name"].Value
   //               } );
   //            }
   //         }
   //      }

   //      //using ( var file = new StreamWriter( table + ".txt" ) )
   //      //{
   //      //   foreach ( var device in collection )
   //      //   {
   //      //      foreach ( var property in device.Properties )
   //      //      {
   //      //         file.WriteLine( property.Name + " = " + property.Value );
   //      //         Console.WriteLine( property.Name + " = " + property.Value );
   //      //      }

   //      //      file.WriteLine();
   //      //      Console.WriteLine();
   //      //   }
   //      //}

   //      return collection;
   //   }
   //}
}
