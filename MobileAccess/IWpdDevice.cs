using System;

namespace MobileAccess
{
   public interface IWpdDevice : IDisposable
   {
      string Name
      {
         get;
      }

      string DeviceID
      {
         get;
      }

      WpdPropertyCollection Properties
      {
         get;
      }

      void StartEnumerate();
      IWpdObject[] GetChildren();
      IWpdObject ObjectFromPath( string path, bool createPath );
   }
}
