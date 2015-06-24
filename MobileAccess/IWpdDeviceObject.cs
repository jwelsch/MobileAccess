using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public interface IWpdDeviceObject
   {
      string ObjectID
      {
         get;
      }

      IWpdDeviceObject Parent
      {
         get;
      }

      IPortableDeviceContent Content
      {
         get;
      }

      string Name
      {
         get;
      }

      string OriginalFileName
      {
         get;
      }

      bool IsContainer
      {
         get;
      }

      IWpdDeviceObject[] GetChildren();
      string GetPath();
   }
}
