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
               if ( devices.Count == 0 )
               {
                  Console.WriteLine( "No devices found." );
               }
               else
               {
                  var device = devices.First( ( item ) =>
                     {
                        return ( String.Compare( arguments.FindDeviceName, item.Name ) == 0 );
                     } );
                  if ( device == null )
                  {
                     Console.WriteLine( "No device found with the name \"{0}\".", arguments.FindDeviceName );
                  }
                  else
                  {
                     if ( arguments.FindCopyID )
                     {
                        ClipboardApi.Copy( device.DeviceID, true );
                     }

                     Console.WriteLine( "Found device" );
                     Console.WriteLine( "  Name: \"{0}\"", device.Name );
                     if ( arguments.ShowDeviceID )
                     {
                        Console.WriteLine( "  DeviceID: {0}", device.DeviceID );
                     }
                  }
               }
            }
            else if ( arguments.CommandUpload )
            {
               var devices = WpdDeviceCollection.Create();
               if ( devices.Count == 0 )
               {
                  Console.WriteLine( "No devices found." );
               }
               else
               {
                  var device = devices.First( ( item ) =>
                     {
                        return ( String.Compare( arguments.UploadDeviceName, item.Name ) == 0 );
                     } );
                  if ( device == null )
                  {
                     Console.WriteLine( "No device found with the name \"{0}\".", arguments.FindDeviceName );
                  }
                  else
                  {
                     Console.WriteLine( "Copying..." );
                     var deviceObject = device.ObjectFromPath( arguments.UploadTargetPath, arguments.CreatePath );
                     Console.WriteLine( "[{0}] {1}", deviceObject.ObjectID, deviceObject.Name );
                     var commander = new DeviceCommander();
                     commander.DataCopied += ( sender, e ) =>
                        {
                           var percent = 100.0 * ( (double) e.CopiedBytes / (double) e.MaxBytes );
                           Console.Write( "\r{0}: {1}/{2} bytes ({3}%)", e.SourcePath, e.CopiedBytes, e.MaxBytes, percent.ToString( "G3" ) );
                        };
                     commander.DataCopyEnded += ( sender, e ) =>
                        {
                           Console.WriteLine();
                        };
                     commander.DataCopyError += ( sender, e ) =>
                        {
                           Console.WriteLine( e.Exception.Message );
                        };
                     commander.Download( deviceObject, arguments.UploadSourcePath, arguments.Overwrite );
                  }
               }
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
