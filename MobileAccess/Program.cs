using System;
using System.Text;
using System.Collections.Generic;
using CommandLineLib;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;

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

            using ( var fileStream = new FileStream( "C:\\Temp\\usb.txt", FileMode.Create, FileAccess.Write, FileShare.Write ) )
            {
               using ( var writer = new StreamWriter( fileStream, Encoding.UTF8, 1024000, true ) )
               {
                  var devices = UsbManager.GetUSBDevices();
                  foreach ( var device in devices )
                  {
                     var builder = new StringBuilder();
                     builder.AppendFormat( "Device ID: {0}\n", device.DeviceId );
                     builder.AppendFormat( "  PNP Device ID: {0}\n", device.PnpDeviceId );
                     builder.AppendFormat( "  Name: {0}\n", device.Name );
                     builder.AppendFormat( "  Description: {0}\n", device.Description );
                     builder.AppendFormat( "  Status: {0}\n", device.Status );
                     Console.Write( builder.ToString() );
                     writer.Write( builder.ToString() );
                  }
               }
            }

            // Get an HIDP_CAPS struct
            // http://stackoverflow.com/questions/11691619/cannot-communicate-successfully-with-usb-hid-device-using-writefile
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
   }
}
