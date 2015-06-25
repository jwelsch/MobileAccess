using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   //
   // From:
   //   https://cgeers.wordpress.com/?s=wpd
   //   http://blogs.msdn.com/b/dimeby8/archive/tags/c_2300_/
   //

   public class WpdDeviceCollection : Collection<IDevice>
   {
      private WpdDeviceCollection()
      {
      }

      public static WpdDeviceCollection Create()
      {
         var deviceManager = new PortableDeviceManager();

         var deviceIds = new string[1];
         var deviceIdCount = 1U;
         deviceManager.GetDevices( ref deviceIds[0], ref deviceIdCount );

         var collection = new WpdDeviceCollection();

         if ( deviceIdCount > 0 )
         {
            deviceIds = new string[deviceIdCount];
            deviceManager.GetDevices( ref deviceIds[0], ref deviceIdCount );

            foreach ( var deviceId in deviceIds )
            {
               collection.Add( WpdDevice.Create( deviceId ) );
            }
         }

         return collection;
      }

      public IDevice Find( string deviceName, bool caseSensitive )
      {
         foreach ( var item in this )
         {
            if ( String.Compare( deviceName, item.Name, !caseSensitive ) == 0 )
            {
               return item;
            }
         }

         return null;
      }

      public IDevice Find( string deviceID )
      {
         foreach ( var item in this )
         {
            if ( String.Compare( deviceID, item.DeviceID, true ) == 0 )
            {
               return item;
            }
         }

         return null;
      }
   }
}
