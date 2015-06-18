using System;
using System.Text;
using System.Collections.Generic;
using CommandLineLib;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Management;

namespace MobileAccess
{
   class Program
   {
      static void Main( string[] args )
      {
         CommandLine<CLArguments> commandLine = null;

         try
         {
            commandLine = new CommandLine<CLArguments>();
            var arguments = commandLine.Parse( args );

            using ( var cancellation = new CancellationTokenSource() )
            {
               var task = Task.Factory.StartNew( () =>
                  {
                     var insertQuery = new WqlEventQuery( "SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'" );

                     var insertWatcher = new ManagementEventWatcher( insertQuery );
                     insertWatcher.EventArrived += new EventArrivedEventHandler( DeviceInsertedEvent );
                     insertWatcher.Start();

                     var removeQuery = new WqlEventQuery( "SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'" );
                     var removeWatcher = new ManagementEventWatcher( removeQuery );
                     removeWatcher.EventArrived += new EventArrivedEventHandler( DeviceRemovedEvent );
                     removeWatcher.Start();
                  }
                  , cancellation.Token );

               var keyInfo = new ConsoleKeyInfo();

               Console.WriteLine( "Press Q to quit." );
               Console.WriteLine( "Listening for events..." );

               do
               {
                  keyInfo = Console.ReadKey();
               }
               while ( keyInfo.Key != ConsoleKey.Q );

               Console.WriteLine();
               Console.WriteLine( "Exiting..." );

               cancellation.Cancel();
               task.Wait();
            }
         }
         catch ( CommandLineDeclarationException ex )
         {
            if ( commandLine != null )
            {
               Console.WriteLine( commandLine.Help() );
            }
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( ex );
         }
         catch ( CommandLineException ex )
         {
            if ( commandLine != null )
            {
               Console.WriteLine( commandLine.Help() );
            }
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( ex );
         }
         catch ( Exception ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( ex );
         }
      }

      private static void DeviceInsertedEvent( object sender, EventArrivedEventArgs e )
      {
         var instance = (ManagementBaseObject) e.NewEvent["TargetInstance"];
         foreach ( var property in instance.Properties )
         {
            Console.WriteLine( property.Name + " = " + property.Value );
         }
      }

      private static void DeviceRemovedEvent( object sender, EventArrivedEventArgs e )
      {
         var instance = (ManagementBaseObject) e.NewEvent["TargetInstance"];
         foreach ( var property in instance.Properties )
         {
            Console.WriteLine( property.Name + " = " + property.Value );
         }
      }
   }
}
