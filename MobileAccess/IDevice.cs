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

      void DisplayProperties();
      void StartEnumerate();
      IWpdDeviceObject[] GetChildren();
      IWpdDeviceObject ObjectFromPath( string path, bool createPath );
   }
}
