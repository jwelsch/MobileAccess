using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileAccess
{
   public static class VolumeManagement
   {
      #region Constants
      private const int MAX_PATH_EXT = 32767;
      private const int MAX_PATH_COMPONENT_EXT = 255;
      #endregion

      #region ThrowWin32Exception
      private static void ThrowWin32Exception( int errorCode, string message )
      {
         throw new Win32Exception( errorCode, String.Format( "{0} (Error: {1} \"{2}\")", message, errorCode, new Win32Exception( errorCode ).Message ) );
      }

      private static void ThrowWin32Exception( string message )
      {
         VolumeManagement.ThrowWin32Exception( Marshal.GetLastWin32Error(), message );
      }
      #endregion

      #region VolumeInformation
      public interface IVolumeInformation
      {
         string RootPathName
         {
            get;
         }

         string VolumeName
         {
            get;
         }

         uint SerialNumber
         {
            get;
         }

         uint MaximumComponentLength
         {
            get;
         }

         Kernel32.FileSystemFeature FileSystemFlags
         {
            get;
         }

         string FileSystemName
         {
            get;
         }
      }
      public class VolumeInformation : IVolumeInformation
      {
         public string RootPathName
         {
            get;
            set;
         }

         public string VolumeName
         {
            get;
            set;
         }

         public uint SerialNumber
         {
            get;
            set;
         }

         public uint MaximumComponentLength
         {
            get;
            set;
         }

         public Kernel32.FileSystemFeature FileSystemFlags
         {
            get;
            set;
         }

         public string FileSystemName
         {
            get;
            set;
         }
      }
      #endregion

      public static string[] GetLogicalDriveStrings()
      {
         var driveStrings = new List<string>();
         const int bufferLength = MAX_PATH_EXT;
         var buffer = new char[bufferLength];
         var code = Kernel32.GetLogicalDriveStrings( bufferLength, buffer );
         if ( ( code == 0 ) || ( code > bufferLength ) )
         {
            ThrowWin32Exception( "GetLogicalDriveStrings failed" );
         }
         else
         {
            var idx = 0;
            var counter = 0;
            var builder = new StringBuilder();
            while ( counter < 2 )
            {
               if ( buffer[idx] == '\0' )
               {
                  counter++;
                  if ( builder.Length > 0 )
                  {
                     driveStrings.Add( builder.ToString() );
                     builder.Clear();
                  }
               }
               else
               {
                  counter = 0;
                  builder.Append( buffer[idx] );
               }
               idx++;
            }
         }

         return driveStrings.ToArray();
      }

      public static string[] EnumerateVolumes()
      {
         var handle = IntPtr.Zero;

         try
         {
            const int bufferLength = MAX_PATH_EXT;
            var volume = new StringBuilder( bufferLength, bufferLength );
            var volumes = new List<string>();

            handle = Kernel32.FindFirstVolume( volume, bufferLength );
            if ( handle == Win32Defines.INVALID_HANDLE_VALUE )
            {
               ThrowWin32Exception( "FindFirstVolume failed" );
            }

            do
            {
               volumes.Add( volume.ToString() );
            }
            while ( Kernel32.FindNextVolume( handle, volume, bufferLength ) );

            return volumes.ToArray();
         }
         finally
         {
            if ( ( handle != IntPtr.Zero ) && ( handle != Win32Defines.INVALID_HANDLE_VALUE ) )
            {
               if ( !Kernel32.FindVolumeClose( handle ) )
               {
                  ThrowWin32Exception( "FindVolumeClose failed" );
               }
            }
         }
      }

      public static string[] EnumerateVolumeMountPoints( string rootPathName )
      {
         var handle = IntPtr.Zero;

         try
         {
            const int bufferLength = MAX_PATH_EXT;
            var volumeMountPoint = new StringBuilder( bufferLength, bufferLength );
            var volumeMountPoints = new List<string>();

            handle = Kernel32.FindFirstVolumeMountPoint( rootPathName, volumeMountPoint, bufferLength );
            if ( handle == Win32Defines.INVALID_HANDLE_VALUE )
            {
               ThrowWin32Exception( "FindFirstVolumeMountPoint failed" );
            }

            do
            {
               volumeMountPoints.Add( volumeMountPoint.ToString() );
            }
            while ( Kernel32.FindNextVolumeMountPoint( handle, volumeMountPoint, bufferLength ) );

            return volumeMountPoints.ToArray();
         }
         finally
         {
            if ( ( handle != IntPtr.Zero ) && ( handle != Win32Defines.INVALID_HANDLE_VALUE ) )
            {
               if ( !Kernel32.FindVolumeMountPointClose( handle ) )
               {
                  ThrowWin32Exception( "FindVolumeMountPointClose failed" );
               }
            }
         }
      }

      public static IVolumeInformation GetVolumeInformation( string rootPathName )
      {
         var volumeInformation = new VolumeInformation();
         var volumeName = new StringBuilder( MAX_PATH_EXT );
         var fileSystemName = new StringBuilder( MAX_PATH_EXT );
         uint serialNumber, maximumComponentLength;
         Kernel32.FileSystemFeature flags;
         if ( !Kernel32.GetVolumeInformation( rootPathName, volumeName, volumeName.Capacity, out serialNumber, out maximumComponentLength, out flags, fileSystemName, fileSystemName.Capacity ) )
         {
            ThrowWin32Exception( "GetVolumeInformation failed" );
         }
         volumeInformation.RootPathName = rootPathName;
         volumeInformation.VolumeName = volumeName.ToString();
         volumeInformation.SerialNumber = serialNumber;
         volumeInformation.MaximumComponentLength = maximumComponentLength;
         volumeInformation.FileSystemFlags = flags;
         volumeInformation.FileSystemName = fileSystemName.ToString();
         return volumeInformation;
      }
   }
}
