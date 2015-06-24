using PortableDeviceApiLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace MobileAccess
{
   public class DeviceCommander
   {
      public event DataCopyStartedHandler DataCopyStarted;
      public event DataCopiedHandler DataCopied;
      public event DataCopyEndedHandler DataCopyEnded;
      public event DataCopyErrorHandler DataCopyError;

      public void Download( IWpdDeviceObject containerObject, string sourceFilePath, bool overwrite )
      {
         var fileInfo = new FileInfo( sourceFilePath );
         if ( ( fileInfo.Attributes & FileAttributes.Directory ) == FileAttributes.Directory )
         {
            this.Download( containerObject, sourceFilePath, overwrite, "*", true );
            return;
            //throw new InvalidOperationException( "Source path cannot be a directory." );
         }

         var fileName = Path.GetFileName( sourceFilePath );
         var targetPath = containerObject.GetPath() + Path.DirectorySeparatorChar + fileName;

         var children = containerObject.GetChildren();
         foreach ( var child in children )
         {
            if ( ( String.Compare( child.Name, fileName, true ) == 0 ) )
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
         values.SetStringValue( PortableDevicePKeys.WPD_OBJECT_NAME, fileName );

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
            System.Diagnostics.Trace.WriteLine( String.Format( "Failed to download file \"{0}\": {1}", sourceFilePath, ex ) );

            if ( this.DataCopyError != null )
            {
               this.DataCopyError( this, new DataCopyErrorArgs( sourceFilePath, targetPath, ex ) );
            }
         }
         finally
         {
            Marshal.ReleaseComObject( targetStream );
         }

         if ( this.DataCopyEnded != null )
         {
            this.DataCopyEnded( this, new DataCopyEndedArgs( sourceFilePath, targetPath ) );
         }
      }

      public void Download( IWpdDeviceObject containerObject, string[] sourceFilePaths, bool overwrite )
      {
         foreach ( var sourceFilePath in sourceFilePaths )
         {
            try
            {
               this.Download( containerObject, sourceFilePath, overwrite );
            }
            catch ( Exception )
            {
               // Hide exception and continue copy operation.
            }
         }
      }

      public void Download( IWpdDeviceObject containerObject, string sourceDirectoryPath, bool overwrite, string searchPattern, bool recursive )
      {
         var fileInfo = new FileInfo( sourceDirectoryPath );
         if ( ( fileInfo.Attributes & FileAttributes.Directory ) != FileAttributes.Directory )
         {
            throw new InvalidOperationException( "Source path must be a directory." );
         }

         var sourceFilePaths = Directory.GetFiles( sourceDirectoryPath,
            String.IsNullOrEmpty( searchPattern ) ? "*" : searchPattern,
            recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );

         this.Download( containerObject, sourceFilePaths, overwrite );
      }

      public void Delete( IWpdDeviceObject deleteObject )
      {
         var variant = PropVariant.StringToPropVariant( deleteObject.ObjectID );
         var objectIds = (IPortableDevicePropVariantCollection) new PortableDeviceTypesLib.PortableDevicePropVariantCollection();
         objectIds.Add( variant );

         IPortableDevicePropVariantCollection results = null;

         deleteObject.Content.Delete( 0, objectIds, ref results );
      }

      public IWpdDeviceObject CreateDirectory( IWpdDeviceObject containerObject, string directoryName )
      {
         var values = new PortableDeviceTypesLib.PortableDeviceValues() as IPortableDeviceValues;

         values.SetGuidValue( ref PortableDevicePKeys.WPD_OBJECT_FORMAT, PortableDeviceGuids.WPD_OBJECT_FORMAT_PROPERTIES_ONLY );
         values.SetGuidValue( ref PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER );

         // Parent ID of the new object.
         values.SetStringValue( ref PortableDevicePKeys.WPD_OBJECT_PARENT_ID, containerObject.ObjectID );

         // The original file name of the object.
         values.SetStringValue( PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, directoryName );

         // The name of the object on the device.
         values.SetStringValue( PortableDevicePKeys.WPD_OBJECT_NAME, directoryName );

         var objectID = string.Empty;

         containerObject.Content.CreateObjectWithPropertiesOnly( values, ref objectID );

         return new WpdDeviceObject( objectID, containerObject, containerObject.Content );
      }
   }
}
