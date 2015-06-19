using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public class DeviceEventListener : IDisposable
   {
      private ManagementEventWatcher insertWatcher = null;
      private ManagementEventWatcher removeWatcher = null;

      public DeviceEventListener()
      {
      }

      ~DeviceEventListener()
      {
         CleanUp();
      }

      public void StartListening()
      {
         var insertQuery = new WqlEventQuery( "SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'" );
         this.insertWatcher = new ManagementEventWatcher( insertQuery );
         this.insertWatcher.EventArrived += new EventArrivedEventHandler( DeviceInsertedEvent );
         this.insertWatcher.Start();

         var removeQuery = new WqlEventQuery( "SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'" );
         this.removeWatcher = new ManagementEventWatcher( removeQuery );
         this.removeWatcher.EventArrived += new EventArrivedEventHandler( DeviceRemovedEvent );
         this.removeWatcher.Start();
      }

      public void StopListening()
      {
         this.CleanUp();
      }

      private void CleanUp()
      {
         if ( insertWatcher != null )
         {
            insertWatcher.Stop();
            this.insertWatcher = null;
         }

         if ( removeWatcher != null )
         {
            removeWatcher.Stop();
            this.removeWatcher = null;
         }
      }

      private static void DeviceInsertedEvent( object sender, EventArrivedEventArgs e )
      {
         //var instance = (ManagementBaseObject) e.NewEvent["TargetInstance"];
         //foreach ( var property in instance.Properties )
         //{
         //   Console.WriteLine( property.Name + " = " + property.Value );
         //}
      }

      private static void DeviceRemovedEvent( object sender, EventArrivedEventArgs e )
      {
         //var instance = (ManagementBaseObject) e.NewEvent["TargetInstance"];
         //foreach ( var property in instance.Properties )
         //{
         //   Console.WriteLine( property.Name + " = " + property.Value );
         //}
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

            this.CleanUp();
         }

         // Free unmanaged objects here.
      }

      #endregion
   }
}
