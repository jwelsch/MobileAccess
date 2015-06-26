using System;
using CommandLineLib;

namespace MobileAccess
{
   public class CLArguments
   {
      //
      // Commands
      //

      [Switch( "Discover", Ordinal = 1, Optional = true, Groups = new int[] { 1 }, CaseSensitive = false, Description = "Discovers MTP-capable devices." )]
      public bool CommandDiscover
      {
         get;
         private set;
      }

      [Switch( "Info", Ordinal = 1, Optional = true, Groups = new int[] { 2 }, CaseSensitive = false, Description = "Displays information about the specified MTP-capable device." )]
      public bool CommandInfo
      {
         get;
         private set;
      }

      [Switch( "Upload", Ordinal = 1, Optional = true, Groups = new int[] { 3 }, CaseSensitive = false, Description = "Uploads files to the device." )]
      public bool CommandUpload
      {
         get;
         private set;
      }

      [Switch( "Download", Ordinal = 2, Optional = true, Groups = new int[] { 4 }, CaseSensitive = false, Description = "Downloads files from the device." )]
      public bool CommandDownload
      {
         get;
         private set;
      }

      //
      // Arguments for INFO command.
      //

      [StringValue( 2, Groups = new int[] { 2 }, Description = "Name of the device to for which information will be displayed." )]
      public string InfoDeviceName
      {
         get;
         private set;
      }

      [Switch( "-copyId", Optional = true, CaseSensitive = false, Groups = new int[] { 2 }, Description = "Copies the device ID to the clipboard." )]
      public bool InfoCopyID
      {
         get;
         private set;
      }

      //
      // Arguments for UPLOAD command.
      //

      [StringValue( 2, Groups = new int[] { 3 }, Description = "Path on the local machine of file(s) to upload." )]
      public string UploadSourcePath
      {
         get;
         private set;
      }

      [StringValue( 3, Groups = new[] { 3 }, Description = "Name of the device to which the file(s) will be uploaded." )]
      public string UploadDeviceName
      {
         get;
         private set;
      }

      [StringValue( 4, Groups = new int[] { 3 }, Description = "Path on the target device where the file(s) will be uploaded." )]
      public string UploadTargetPath
      {
         get;
         private set;
      }

      //
      // Arguments for DOWNLOAD command.
      //

      [StringValue( 2, Groups = new[] { 4 }, Description = "Name of the device from which the file(s) will be downloaded." )]
      public string DownloadDeviceName
      {
         get;
         private set;
      }

      [StringValue( 3, Groups = new int[] { 4 }, Description = "Path on the source device from which the file(s) will be downloaded." )]
      public string DownloadSourcePath
      {
         get;
         private set;
      }

      [StringValue( 4, Groups = new int[] { 4 }, Description = "Path on the local machine where the file(s) will be downloaded." )]
      public string DownloadTargetPath
      {
         get;
         private set;
      }

      //
      // Arguments for multiple commands.
      //

      [Switch( "-deviceID", Optional = true, CaseSensitive = false, Groups = new int[] { 1, 2 }, Description = "Include to show device IDs." )]
      public bool ShowDeviceID
      {
         get;
         private set;
      }

      [Switch( "-overwrite", Optional = true, CaseSensitive = false, Groups = new int[] { 3, 4 }, Description = "Include to allow files to be overwritten." )]
      public bool Overwrite
      {
         get;
         private set;
      }

      [Switch( "-createPath", Optional = true, CaseSensitive = false, Groups = new int[] { 3, 4 }, Description = "Include to automatically create any missing directories in the path." )]
      public bool CreatePath
      {
         get;
         private set;
      }

      [Switch( "-recursive", Optional = true, CaseSensitive = false, Groups = new int[] { 3, 4 }, Description = "Include to recurse through sub directories." )]
      public bool Recursive
      {
         get;
         private set;
      }
   }
}
