using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public class VolumeInformer
   {
      public void Inform()
      {
         var driveStrings = VolumeManagement.GetLogicalDriveStrings();
         Console.WriteLine( "GetLogicalDriveStrings returned:" );
         foreach ( var drive in driveStrings )
         {
            Console.WriteLine( "  {0}", drive );
         }

         var volumes = VolumeManagement.EnumerateVolumes();
         Console.WriteLine( "Enumerated volumes (count = {0}):", volumes.Length );
         foreach ( var volume in volumes )
         {
            Console.WriteLine( "  {0}", volume );
            string[] volumeMountPoints = null;
            try
            {
               Console.WriteLine( "  Enumerated volume mount points:" );
               volumeMountPoints = VolumeManagement.EnumerateVolumeMountPoints( volume );
               foreach ( var volumeMountPoint in volumeMountPoints )
               {
                  Console.WriteLine( "    {0}", volumeMountPoint );
               }
            }
            catch ( Exception ex )
            {
               Console.WriteLine( "    {0}", ex.Message );
            }
            Console.WriteLine( "  Volume information:" );
            try
            {
               var vi = VolumeManagement.GetVolumeInformation( volume );
               Console.WriteLine( "    Root Path Name: {0}", vi.RootPathName );
               Console.WriteLine( "    Volume Name: {0}", vi.VolumeName );
               Console.WriteLine( "    Serial Number: {0}", vi.SerialNumber );
               Console.WriteLine( "    Maximum Component Length: {0}", vi.MaximumComponentLength );
               Console.WriteLine( "    File System Flags: {0}", vi.FileSystemFlags.ToStringFlags() );
               Console.WriteLine( "    File System Name: {0}", vi.FileSystemName );
            }
            catch ( Exception ex )
            {
               Console.WriteLine( "    {0}", ex.Message );
            }
         }
      }
   }
}
