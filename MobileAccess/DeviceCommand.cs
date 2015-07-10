using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public abstract class DeviceCommand : IDeviceCommand
   {
      protected CLArguments Arguments
      {
         get;
         private set;
      }

      protected WpdDeviceCollection Devices
      {
         get;
         private set;
      }

      public DeviceCommand( CLArguments arguments )
      {
         this.Arguments = arguments;
         this.Devices = WpdDeviceCollection.Create();
      }

      protected IWpdDevice Find( string deviceName, IMessageWriter writer )
      {
         if ( this.Devices.Count == 0 )
         {
            if ( writer != null )
            {
               writer.WriteLine( "No devices found." );
            }
         }
         else
         {
            var device = this.Devices.Find( deviceName, false );
            if ( device == null && writer != null )
            {
               writer.WriteLine( "No device found with the name \"{0}\".", deviceName );
            }

            return device;
         }

         return null;
      }

      public void Execute( IMessageWriter writer )
      {
         this.ExecuteCommand( writer );
      }

      protected virtual void PreExecuteCommand( IMessageWriter writer )
      {
      }

      protected abstract void ExecuteCommand( IMessageWriter writer );

      protected virtual void PostExecuteCommand( IMessageWriter writer )
      {
      }

      #region IDisposable implementation

      public virtual void Dispose()
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

         // Free unmanaged objects here.
         if ( this.Devices != null )
         {
            this.Devices.Dispose();
         }
      }

      #endregion
   }
}
