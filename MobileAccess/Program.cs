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

            if ( arguments.CommandDiscover )
            {
               //var devices = WmiDeviceCollection.Create();
               var devices = WpdDeviceCollection.Create();

               if ( devices.Count == 0 )
               {
                  Console.WriteLine( "No devices found." );
               }
               else
               {
                  foreach ( var device in devices )
                  {
                     Console.WriteLine( "\"{0}\"{1}", device.Name, arguments.ShowDeviceID ? " " + device.DeviceID : string.Empty );
                  }
               }
            }
            else if ( arguments.CommandFind )
            {
               //var devices = WmiDeviceCollection.Create();
               var devices = WpdDeviceCollection.Create();
               var device = devices.First( ( item ) =>
                  {
                     return ( String.Compare( arguments.DeviceName, item.Name ) == 0 );
                  } );
               if ( device == null )
               {
                  Console.WriteLine( "No device found with the name \"{0}\".", arguments.DeviceName );
               }
               else
               {
                  Console.WriteLine( "Found device" );
                  Console.WriteLine( "  Name: \"{0}\"", device.Name );
                  if ( arguments.ShowDeviceID )
                  {
                     Console.WriteLine( "  DeviceID: {0}", device.DeviceID );
                  }
               }
            }
            else if ( arguments.CommandCopy )
            {
               Console.WriteLine( "Copying..." );
               var device = WpdDevice.Create( @"\\TSC-1114\root\cimv2:Win32_PnPEntity.DeviceID=""USB\\VID_04E8&PID_6860&MS_COMP_MTP&SAMSUNG_ANDROID\\6&38109ED&1&0000""" );
            }
            else
            {
               Console.WriteLine( commandLine.Help() );
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

      private static void WaitForKey( ConsoleKey key )
      {
         var keyInfo = new ConsoleKeyInfo();

         Console.WriteLine( "Press {0} to quit.", key );

         do
         {
            keyInfo = Console.ReadKey();
         }
         while ( keyInfo.Key != key );

         Console.WriteLine();
      }
   }
}
