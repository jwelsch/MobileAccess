using PortableDeviceApiLib;
using System;
using System.Collections.ObjectModel;

namespace MobileAccess
{
   //
   // From:
   //   https://cgeers.wordpress.com/?s=wpd
   //   http://blogs.msdn.com/b/dimeby8/archive/tags/c_2300_/
   //

   public class WpdDeviceCollection : Collection<IWpdDevice>, IDisposable
   {
      private WpdDeviceCollection()
      {
      }

      ~WpdDeviceCollection()
      {
         this.DisposeItems();
      }

      public static WpdDeviceCollection Create()
      {
         var deviceManager = new PortableDeviceManager();

         var deviceIdCount = 0U;
         deviceManager.GetDevices( null, ref deviceIdCount );

         var collection = new WpdDeviceCollection();

         if ( deviceIdCount > 0 )
         {
            var deviceIds = new string[deviceIdCount];
            deviceManager.GetDevices( deviceIds, ref deviceIdCount );

            foreach ( var deviceId in deviceIds )
            {
               collection.Add( WpdDevice.Create( deviceId ) );
            }
         }

         return collection;
      }

      public IWpdDevice Find( string deviceName, bool caseSensitive )
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

      public IWpdDevice Find( string deviceID )
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

      private void DisposeItems()
      {
         foreach ( var item in this )
         {
            item.Dispose();
         }
      }

      #region IDisposable implementation

      public void Dispose()
      {
         this.Dispose( true );
         GC.SuppressFinalize( this );
      }

      private void Dispose( bool disposing )
      {
         if ( disposing )
         {
            // Free managed objects here.
            this.DisposeItems();
         }

         // Free unmanaged objects here.
      }

      #endregion
   }
}
