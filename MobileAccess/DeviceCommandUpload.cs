using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public class DeviceCommandUpload : DeviceCommandDataCopy
   {
      public DeviceCommandUpload( CLArguments arguments )
         : base( arguments )
      {
      }

      protected override void ExecuteCommand( IMessageWriter writer )
      {
         var components = WildcardSearch.SplitPattern( this.Arguments.UploadSourcePath );
         var targetObject = this.Device.ObjectFromPath( this.Arguments.UploadTargetPath, this.Arguments.CreatePath );

         if ( String.IsNullOrEmpty( components.Item2 ) )
         {
            this.Upload( targetObject, components.Item1, this.Arguments.Overwrite );
         }
         else
         {
            this.Upload( targetObject, components.Item1, this.Arguments.Overwrite, components.Item2, this.Arguments.Recursive, this.Arguments.Flatten );
         }
      }

      protected void Upload( IWpdObject containerObject, string sourceFilePath, bool overwrite )
      {
         var fileInfo = new FileInfo( sourceFilePath );
         if ( ( fileInfo.Attributes & FileAttributes.Directory ) == FileAttributes.Directory )
         {
            this.Upload( containerObject, sourceFilePath, overwrite, "*", true, false );
            return;
         }

         var fileName = Path.GetFileName( sourceFilePath );
         var fileNameWithoutExtension = Path.GetFileNameWithoutExtension( sourceFilePath );
         var targetPath = containerObject.GetPath() + Path.DirectorySeparatorChar + fileName;

         var children = containerObject.GetChildren();
         foreach ( var child in children )
         {
            if ( ( String.Compare( child.OriginalFileName, fileName, true ) == 0 ) )
            {
               if ( overwrite )
               {
                  this.Delete( child );
                  break;
               }
               else
               {
                  var ex = new IOException( String.Format( "A file with the path \"{0}\" already exists on the device.", targetPath ) );

                  if ( this.DataCopyError != null )
                  {
                     this.DataCopyError( this, new DataCopyErrorArgs( sourceFilePath, targetPath, ex ) );
                  }

                  throw ex;
               }
            }
         }

         if ( this.DataCopyStarted != null )
         {
            this.DataCopyStarted( this, new DataCopyStartedArgs( sourceFilePath, targetPath ) );
         }

         var values = new PortableDeviceTypesLib.PortableDeviceValues() as IPortableDeviceValues;

         // Parent ID of the new object.
         values.SetStringValue( ref PortableDevicePKeys.WPD_OBJECT_PARENT_ID, containerObject.ObjectID );

         // Size in bytes of the new object.
         values.SetUnsignedLargeIntegerValue( PortableDevicePKeys.WPD_OBJECT_SIZE, (ulong) fileInfo.Length );

         // The original file name of the object.
         values.SetStringValue( PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, fileName );

         // The name of the object on the device.
         values.SetStringValue( PortableDevicePKeys.WPD_OBJECT_NAME, fileNameWithoutExtension );

         IStream targetStream = null;
         var optimalTransferSizeBytes = 0U;
         containerObject.Content.CreateObjectWithPropertiesAndData( values, out targetStream, ref optimalTransferSizeBytes, null );

         try
         {
            using ( var sourceStream = new FileStream( sourceFilePath, FileMode.Open, FileAccess.Read ) )
            {
               var buffer = new byte[optimalTransferSizeBytes];
               var bytesRead = 0;
               var totalWritten = 0UL;

               do
               {
                  bytesRead = sourceStream.Read( buffer, 0, (int) optimalTransferSizeBytes );
                  if ( bytesRead > 0 )
                  {
                     var written = 0U;
                     targetStream.RemoteWrite( ref buffer[0], (uint) bytesRead, out written );
                     totalWritten += (ulong) written;

                     if ( this.DataCopied != null )
                     {
                        this.DataCopied( this, new DataCopiedArgs( sourceFilePath, targetPath, (ulong) fileInfo.Length, totalWritten, (ulong) written ) );
                     }
                  }
               }
               while ( bytesRead > 0 );
            }
            targetStream.Commit( 0 );
         }
         catch ( Exception ex )
         {
            System.Diagnostics.Trace.WriteLine( String.Format( "Failed to upload file \"{0}\": {1}", sourceFilePath, ex ) );

            if ( this.DataCopyError != null )
            {
               this.DataCopyError( this, new DataCopyErrorArgs( sourceFilePath, targetPath, ex ) );
            }
         }
         finally
         {
            if ( targetStream != null )
            {
               Marshal.ReleaseComObject( targetStream );
            }
         }

         if ( this.DataCopyEnded != null )
         {
            this.DataCopyEnded( this, new DataCopyEndedArgs( sourceFilePath, targetPath ) );
         }
      }

      protected void Upload( IWpdObject containerObject, string[] sourceFilePaths, bool overwrite )
      {
         foreach ( var sourceFilePath in sourceFilePaths )
         {
            try
            {
               this.Upload( containerObject, sourceFilePath, overwrite );
            }
            catch ( Exception )
            {
               // Hide exception and continue copy operation.
            }
         }
      }

      protected void Upload( IWpdObject containerObject, string sourceDirectoryPath, bool overwrite, string searchPattern, bool recursive, bool flatten )
      {
         var fileInfo = new FileInfo( sourceDirectoryPath );
         if ( ( fileInfo.Attributes & FileAttributes.Directory ) != FileAttributes.Directory )
         {
            throw new InvalidOperationException( "Source path must be a directory." );
         }

         if ( String.IsNullOrEmpty( searchPattern ) )
         {
            var directoryName = Path.GetFileName( sourceDirectoryPath );

            var found = false;
            var children = containerObject.GetChildren();
            IWpdObject directoryObject = null;

            foreach ( var child in children )
            {
               if ( String.Compare( child.Name, directoryName, true ) == 0 )
               {
                  found = true;
                  directoryObject = child;
                  break;
               }
            }

            if ( found )
            {
               if ( !overwrite )
               {
                  throw new IOException( String.Format( "The directory \"{0}\" already exists.", directoryObject.GetPath() ) );
               }

               containerObject = directoryObject;
            }
            else
            {
               containerObject = this.CreateDirectory( containerObject, directoryName );
            }
         }

         var sourceFilePaths = Directory.GetFiles( sourceDirectoryPath,
            String.IsNullOrEmpty( searchPattern ) ? "*" : searchPattern,
            recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );

         sourceDirectoryPath = sourceDirectoryPath.EnsureLastCharacter( '\\', true );

         IWpdObject targetObject = null;

         foreach ( var sourceFilePath in sourceFilePaths )
         {
            if ( flatten )
            {
               targetObject = containerObject;
            }
            else
            {
               var slash = sourceFilePath.LastIndexOf( '\\' );

               if ( slash < 0 )
               {
                  continue;
               }

               if ( slash != sourceDirectoryPath.Length - 1 )
               {
                  var extra = sourceFilePath.Substring( sourceDirectoryPath.Length, slash - sourceDirectoryPath.Length );

                  var targetObjectPath = containerObject.GetPath() + Path.DirectorySeparatorChar + extra;
                  targetObject = this.device.ObjectFromPath( targetObjectPath, true );
               }
               else
               {
                  targetObject = containerObject;
               }
            }

            this.Upload( targetObject, sourceFilePath, overwrite );
         }
      }
   }
}
