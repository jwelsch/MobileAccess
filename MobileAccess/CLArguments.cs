using System;
using System.Collections.Generic;
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

      [Switch( "Find", Ordinal = 1, Optional = true, Groups = new int[] { 2 }, CaseSensitive = false, Description = "Finds the specified MTP-capable device." )]
      public bool CommandFind
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

      //
      // Arguments for FIND command.
      //

      [StringValue( 2, Groups = new int[] { 2 }, Description = "Name of the device to find." )]
      public string FindDeviceName
      {
         get;
         private set;
      }

      [Switch( "-copyId", Optional = true, CaseSensitive = false, Groups = new int[] { 2 }, Description = "Copies the device ID to the clipboard." )]
      public bool FindCopyID
      {
         get;
         private set;
      }

      //
      // Arguments for UPLOAD command.
      //

      [StringValue( 2, Groups = new int[] { 3 }, Description = "Source path of file or folder to upload." )]
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
      // Arguments for multiple commands.
      //

      [Switch( "-deviceID", Optional = true, CaseSensitive = false, Groups = new int[] { 1, 2 }, Description = "Include to show device IDs." )]
      public bool ShowDeviceID
      {
         get;
         private set;
      }

      [Switch( "-overwrite", Optional = true, CaseSensitive = false, Groups = new int[] { 3 }, Description = "Include to allow files to be overwritten." )]
      public bool Overwrite
      {
         get;
         private set;
      }

      [Switch( "-createPath", Optional = true, CaseSensitive = false, Groups = new int[] { 3 }, Description = "Include to automatically create any missing directories in the path." )]
      public bool CreatePath
      {
         get;
         private set;
      }
   }
}
