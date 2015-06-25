using System;
using PortableDeviceApiLib;

namespace MobileAccess
{
   public interface IWpdHeirarchicalObject
   {
      string ObjectID
      {
         get;
      }

      IWpdObject Parent
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

      IWpdHeirarchicalObject[] GetChildren();
   }
}
