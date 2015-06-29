using System;
using PortableDeviceApiLib;

namespace MobileAccess
{
   public interface IWpdObject
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

      ulong Size
      {
         get;
      }

      WpdPropertyCollection Properties
      {
         get;
      }

      IWpdObject[] GetChildren();
      string GetPath();
      string GetNameOnDevice();
      string[] GetChildPaths( string searchPattern, bool recursive );
   }
}
