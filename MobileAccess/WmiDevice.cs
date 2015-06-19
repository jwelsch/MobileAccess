using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public class WmiDevice : IDevice
   {
      public string Name
      {
         get;
         set;
      }

      public string DeviceID
      {
         get;
         set;
      }

      #region IDisposable implementation

      public void Dispose()
      {
         this.Dispose( true );
         GC.SuppressFinalize( this );
      }

      private void Dispose( bool disposing )
      {
         if ( disposing )
         {
            // Free managed objects here.
         }

         // Free unmanaged objects here.
      }

      #endregion
   }
}
