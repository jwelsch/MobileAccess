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

      void DisplayProperties( IMessageWriter writer );
      void StartEnumerate();
      IWpdObject[] GetChildren();
      IWpdObject ObjectFromPath( string path, bool createPath );
   }
}
