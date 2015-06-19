using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public class WpdDevice : IDevice
   {
      private PortableDeviceClass device = null;

      private WpdDevice()
      {
      }

      ~WpdDevice()
      {
         this.CleanUp();
      }

      private string name;
      public string Name
      {
         get
         {
            if ( this.name == null )
            {
               if ( this.device == null )
               {
                  throw new InvalidOperationException( "The object has been disposed." );
               }

               IPortableDeviceContent content = null;
               IPortableDeviceProperties properties = null;
               this.device.Content( out content );
               content.Properties( out properties );

               PortableDeviceApiLib.IPortableDeviceValues values = null;
               properties.GetValues( "DEVICE", null, out values );

               var property = PortableDevicePKeys.WPD_DEVICE_FRIENDLY_NAME;
               values.GetStringValue( ref property, out this.name );
            }

            return this.name;
         }
      }

      public string DeviceID
      {
         get;
         private set;
      }

      public static WpdDevice Create( string deviceId )
      {
         var clientInfo = new PortableDeviceValuesClass();
         var device = new WpdDevice();
         device.device = new PortableDeviceClass();
         device.device.Open( deviceId, (PortableDeviceApiLib.IPortableDeviceValues) clientInfo );
         device.DeviceID = deviceId;

         return device;
      }

      public void CleanUp()
      {
         if ( this.device != null )
         {
            this.device.Close();
            this.device = null;
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
         }

         this.CleanUp();
      }

      #endregion
   }
}
