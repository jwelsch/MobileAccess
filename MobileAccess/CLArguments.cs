using System;
using System.Collections.Generic;
using CommandLineLib;

namespace MobileAccess
{
   public class CLArguments
   {
      // Commands

      [Switch( "Discover", Ordinal = 1, Optional = true, Groups = new int[] { 1 }, CaseSensitive = false, Description = "The command to execute." )]
      public bool CommandDiscover
      {
         get;
         private set;
      }

      [Switch( "Find", Ordinal = 1, Optional = true, Groups = new int[] { 2 }, CaseSensitive = false, Description = "The command to execute." )]
      public bool CommandFind
      {
         get;
         private set;
      }

      [Switch( "Copy", Ordinal = 1, Optional = true, Groups = new int[] { 3 }, CaseSensitive = false, Description = "The command to execute." )]
      public bool CommandCopy
      {
         get;
         private set;
      }

      // Arguments for commands

      [StringValue( 2, Groups = new int[] { 2 }, Description = "Name of the device to find." )]
      public string DeviceName
      {
         get;
         private set;
      }

      [StringValue( 2, Groups = new int[] { 3 }, Description = "Source path of file or folder to copy." )]
      public string SourcePath
      {
         get;
         private set;
      }

      [StringValue( 3, Groups = new int[] { 3 }, Description = "Path on the target device to copy the files to." )]
      public string TargetPath
      {
         get;
         private set;
      }

      [Switch( "-DeviceID", Optional = true, CaseSensitive = false, Groups = new int[] { 1, 2 }, Description = "Include to show device IDs." )]
      public bool ShowDeviceID
      {
         get;
         private set;
      }
   }
}
