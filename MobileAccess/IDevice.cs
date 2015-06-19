using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public interface IDevice : IDisposable
   {
      string Name
      {
         get;
      }

      string DeviceID
      {
         get;
      }
   }
}
