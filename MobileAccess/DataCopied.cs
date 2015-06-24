using System;

namespace MobileAccess
{
   #region DataCopyStart

   public delegate void DataCopyStartedHandler( object sender, DataCopyStartedArgs e );

   public class DataCopyStartedArgs : EventArgs
   {
      public string SourcePath
      {
         get;
         private set;
      }

      public string TargetPath
      {
         get;
         private set;
      }

      public DataCopyStartedArgs( string sourcePath, string targetPath )
      {
         this.SourcePath = sourcePath;
         this.TargetPath = targetPath;
      }
   }

   #endregion

   #region DataCopied

   public delegate void DataCopiedHandler( object sender, DataCopiedArgs e );

   public class DataCopiedArgs : EventArgs
   {
      public string SourcePath
      {
         get;
         private set;
      }

      public string TargetPath
      {
         get;
         private set;
      }

      public ulong MaxBytes
      {
         get;
         private set;
      }

      /// <summary>
      /// Total copied bytes.
      /// </summary>
      public ulong CopiedBytes
      {
         get;
         private set;
      }

      /// <summary>
      /// Number of bytes copied since last event.
      /// </summary>
      public ulong DeltaBytes
      {
         get;
         private set;
      }

      public DataCopiedArgs( string sourcePath, string targetPath, ulong maxBytes, ulong copiedBytes, ulong deltaBytes )
      {
         this.SourcePath = sourcePath;
         this.TargetPath = targetPath;
         this.MaxBytes = maxBytes;
         this.CopiedBytes = copiedBytes;
         this.DeltaBytes = deltaBytes;
      }
   }

   #endregion

   #region DataCopyEnded

   public delegate void DataCopyEndedHandler( object sender, DataCopyEndedArgs e );

   public class DataCopyEndedArgs : EventArgs
   {
      public string SourcePath
      {
         get;
         private set;
      }

      public string TargetPath
      {
         get;
         private set;
      }

      public DataCopyEndedArgs( string sourcePath, string targetPath )
      {
         this.SourcePath = sourcePath;
         this.TargetPath = targetPath;
      }
   }

   #endregion

   #region DataCopyError

   public delegate void DataCopyErrorHandler( object sender, DataCopyErrorArgs e );

   public class DataCopyErrorArgs : EventArgs
   {
      public string SourcePath
      {
         get;
         private set;
      }

      public string TargetPath
      {
         get;
         private set;
      }

      public Exception Exception
      {
         get;
         private set;
      }

      public DataCopyErrorArgs( string sourcePath, string targetPath, Exception exception )
      {
         this.SourcePath = sourcePath;
         this.TargetPath = targetPath;
         this.Exception = exception;
      }
   }

   #endregion
}