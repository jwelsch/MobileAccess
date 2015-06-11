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
                  var handle = IntPtr.Zero;
                  try
                  {
                     var diskGuid = SetupApi.GUID_DEVINTERFACE_DISK;

                     // Start at the "root" of the device tree and look for all devices that match the interface GUID of a disk.
                     handle = SetupApi.SetupDiGetClassDevs( ref diskGuid, IntPtr.Zero, IntPtr.Zero, SetupApi.DiGetClassFlags.DIGCF_PRESENT | SetupApi.DiGetClassFlags.DIGCF_DEVICEINTERFACE );
                     if ( handle != Win32Defines.INVALID_HANDLE_VALUE )
                     {
                        var success = true;
                        var i = 0;

                        while ( success )
                        {
                           var dia = new SetupApi.SP_DEVICE_INTERFACE_DATA();
                           dia.cbSize = Marshal.SizeOf( dia );

                           // Start the enumeration.
                           success = SetupApi.SetupDiEnumDeviceInterfaces( handle, IntPtr.Zero, ref diskGuid, (uint) i, ref dia );
                           if ( success )
                           {
                              var da = new SetupApi.SP_DEVINFO_DATA();
                              da.cbSize = Marshal.SizeOf( da );

                              var didd = new SetupApi.SP_DEVICE_INTERFACE_DETAIL_DATA();
                              didd.cbSize = Marshal.SizeOf( didd );

                              // Get detailed information on the device.
                              var requiredSize = 0U;
                              var bytes = SetupApi.SP_DEVICE_INTERFACE_DETAIL_DATA_BUFFER_SIZE;
                              if ( SetupApi.SetupDiGetDeviceInterfaceDetail( handle, ref dia, ref didd, bytes, out requiredSize, ref da ) )
                              {
                                 // Current InstanceID is at the "USBSTOR" level, so "move up" one level to get to the "USB" level.
                                 var previous = 0U;
                                 var result = SetupApi.CM_Get_Parent( out previous, (uint) da.devInst, 0 );
                                 if ( result == Win32Defines.ERROR_SUCCESS )
                                 {
                                    var instanceBuffer = Marshal.AllocHGlobal( (int) bytes );
                                    result = SetupApi.CM_Get_Device_ID( previous, instanceBuffer, (int) bytes, 0 );
                                    if ( result == Win32Defines.ERROR_SUCCESS )
                                    {
                                       var instanceID = Marshal.PtrToStringAuto( instanceBuffer );
                                       Console.WriteLine( instanceID );
                                       Marshal.FreeHGlobal( instanceBuffer );
                                    }
                                 }
                              }
                           }
                           i++;
                        }
                     }
                  }
                  finally
                  {
                     if ( handle != Win32Defines.INVALID_HANDLE_VALUE )
                     {
                        SetupApi.SetupDiDestroyDeviceInfoList( handle );
                     }
                  }
               }
            }

            // Get an HIDP_CAPS struct
            // http://stackoverflow.com/questions/11691619/cannot-communicate-successfully-with-usb-hid-device-using-writefile

            // http://www.developerfusion.com/article/84338/making-usb-c-friendly/
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
