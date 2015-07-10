using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public abstract class DeviceCommandDataCopy : DeviceCommand
   {
      protected event DataCopyStartedHandler DataCopyStarted;
      protected event DataCopiedHandler DataCopied;
      protected event DataCopyEndedHandler DataCopyEnded;
      protected event DataCopyErrorHandler DataCopyError;

      public DeviceCommandDataCopy( CLArguments arguments )
         : base( arguments )
      {
      }

      protected override void PreExecuteCommand( IMessageWriter writer )
      {
         this.DataCopyStarted += ( sender, e ) =>
         {
            writer.WriteLine( "\r{0}", e.SourcePath );
         };
         this.DataCopied += ( sender, e ) =>
         {
            var percent = 100.0 * ( (double) e.CopiedBytes / (double) e.MaxBytes );
            writer.Write( "\r  {0}/{1} bytes ({2}%)", e.CopiedBytes, e.MaxBytes, percent.ToString( "G3" ) );
         };
         this.DataCopyEnded += ( sender, e ) =>
         {
         };
         this.DataCopyError += ( sender, e ) =>
         {
            writer.WriteLine( "\r" + e.Exception.Message );
         };
         writer.WriteLine();
      }

      protected override void PostExecuteCommand( IMessageWriter writer )
      {
         writer.WriteLine( "\rCompleted                                                                     " );
      }
   }
}
