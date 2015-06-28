using System;

namespace MobileAccess
{
   public interface IMessageWriter
   {
      void Write( string text, params object[] arguments );
      void WriteLine();
      void WriteLine( string text, params object[] arguments );
   }
}
