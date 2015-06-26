using PortableDeviceApiLib;
using System;
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

      private IWpdDevice device;

      public DeviceCommander( IWpdDevice device )
      {
         this.device = device;
      }

      public void Upload( IWpdObject containerObject, string sourceFilePath, bool overwrite )
      {
         var fileInfo = new FileInfo( sourceFilePath );
         if ( ( fileInfo.Attributes & FileAttributes.Directory ) == FileAttributes.Directory )
         {
            this.Upload( containerObject, sourceFilePath, overwrite, "*", true );
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

      public void Upload( IWpdObject containerObject, string[] sourceFilePaths, bool overwrite )
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

      public void Upload( IWpdObject containerObject, string sourceDirectoryPath, bool overwrite, string searchPattern, bool recursive )
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

         if ( sourceDirectoryPath[sourceDirectoryPath.Length - 1] != '\\' )
         {
            sourceDirectoryPath += '\\';
         }

         foreach ( var sourceFilePath in sourceFilePaths )
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
               var targetObject = this.device.ObjectFromPath( targetObjectPath, true );

               this.Upload( targetObject, sourceFilePath, overwrite );
            }
            else
            {
               this.Upload( containerObject, sourceFilePath, overwrite );
            }
         }
      }

      public void Delete( IWpdObject deleteObject )
      {
         var variant = PropVariant.StringToPropVariant( deleteObject.ObjectID );
         var objectIds = (IPortableDevicePropVariantCollection) new PortableDeviceTypesLib.PortableDevicePropVariantCollection();
         objectIds.Add( variant );

         IPortableDevicePropVariantCollection results = null;

         deleteObject.Content.Delete( 0, objectIds, ref results );
      }

      public IWpdObject CreateDirectory( IWpdObject containerObject, string directoryName )
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

         return new WpdObject( objectID, containerObject, containerObject.Content );
      }

      public void Download( IWpdObject sourceObject, string targetDirectoryPath, bool overwrite )
      {
         if ( sourceObject.IsContainer )
         {
            var children = sourceObject.GetChildren();
            this.Download( children, targetDirectoryPath, overwrite );
            return;
         }

         var targetFilePath = Path.Combine( targetDirectoryPath, sourceObject.OriginalFileName );
         var sourceFilePath = sourceObject.GetPath();

         if ( this.DataCopyStarted != null )
         {
            this.DataCopyStarted( this, new DataCopyStartedArgs( sourceFilePath, targetFilePath ) );
         }

         IPortableDeviceResources resources;
         sourceObject.Content.Transfer( out resources );

         var key = PortableDevicePKeys.WPD_RESOURCE_DEFAULT;
         var optimalBufferSize = 0U;
         IStream sourceStream = null;

         resources.GetStream( sourceObject.ObjectID, ref key, PortableDeviceResourceAccessModes.STGM_READ, ref optimalBufferSize, out sourceStream );

         try
         {
            using ( var targetStream = new FileStream( targetFilePath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write ) )
            {
               var buffer = new byte[optimalBufferSize];
               var bytesRead = 0U;
               var totalBytesRead = 0U;

               do
               {
                  sourceStream.RemoteRead( out buffer[0], optimalBufferSize, out bytesRead );
                  if ( bytesRead > 0 )
                  {
                     targetStream.Write( buffer, 0, (int) bytesRead );
                     totalBytesRead += bytesRead;

                     if ( this.DataCopied != null )
                     {
                        this.DataCopied( this, new DataCopiedArgs( sourceFilePath, targetFilePath, sourceObject.Size, totalBytesRead, bytesRead ) );
                     }
                  }
               }
               while ( bytesRead > 0 );

               targetStream.Flush();
            }
         }
         catch ( Exception ex )
         {
            System.Diagnostics.Trace.WriteLine( String.Format( "Failed to download file \"{0}\": {1}", sourceObject.GetPath(), ex ) );

            if ( this.DataCopyError != null )
            {
               this.DataCopyError( this, new DataCopyErrorArgs( sourceObject.GetPath(), targetFilePath, ex ) );
            }

            throw ex;
         }
         finally
         {
            if ( sourceStream != null )
            {
               Marshal.ReleaseComObject( sourceStream );
            }
         }

         if ( this.DataCopyEnded != null )
         {
            this.DataCopyEnded( this, new DataCopyEndedArgs( sourceFilePath, targetFilePath ) );
         }
      }

      public void Download( IWpdObject sourceObject, string targetDirectoryPath, bool overwrite, string searchPattern, bool recursive )
      {
         if ( !sourceObject.IsContainer )
         {
            this.Download( sourceObject, targetDirectoryPath, overwrite );
            return;
         }

         var sourceDirectoryPath = sourceObject.GetPath();
         var sourceFilePaths = sourceObject.GetChildPaths( searchPattern, recursive );

         sourceDirectoryPath = sourceDirectoryPath.EnsureLastCharacter( '\\', true );

         foreach ( var sourceFilePath in sourceFilePaths )
         {
            var slash = sourceFilePath.LastIndexOf( '\\' );

            if ( slash < 0 )
            {
               continue;
            }

            var sourceFileObject = this.device.ObjectFromPath( sourceFilePath, true );

            if ( slash != sourceDirectoryPath.Length - 1 )
            {
               var extra = sourceFilePath.Substring( sourceDirectoryPath.Length, slash - sourceDirectoryPath.Length );

               var targetDirectoryExtraPath = targetDirectoryPath + Path.DirectorySeparatorChar + extra;

               if ( !Directory.Exists( targetDirectoryExtraPath ) )
               {
                  Directory.CreateDirectory( targetDirectoryExtraPath );
               }

               this.Download( sourceFileObject, targetDirectoryExtraPath, overwrite );
            }
            else
            {
               this.Download( sourceFileObject, targetDirectoryPath, overwrite );
            }
         }
      }

      public void Download( IWpdObject[] sourceObjects, string targetDirectoryPath, bool overwrite )
      {
         foreach ( var sourceObject in sourceObjects )
         {
            var name = sourceObject.GetNameOnDevice();

            this.Download( sourceObject, targetDirectoryPath, overwrite );
         }
      }
   }
}
