using System;

namespace MobileAccess
{
   public class ConsoleWriter : IMessageWriter
   {
      public void Write( string text, params object[] arguments )
      {
         Console.Write( text, arguments );
      }

      public void WriteLine()
      {
         Console.WriteLine();
      }

      public void WriteLine( string text, params object[] arguments )
      {
         Console.WriteLine( text, arguments );
      }
   }
}
