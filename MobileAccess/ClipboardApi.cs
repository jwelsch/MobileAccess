using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileAccess
{
   public static class ClipboardApi
   {
      #region DllImports

      [DllImport( "user32.dll" )]
      private static extern bool OpenClipboard( IntPtr hWndNewOwner );

      [DllImport( "user32.dll" )]
      private static extern bool CloseClipboard();

      [DllImport( "user32.dll" )]
      private static extern bool SetClipboardData( ClipboardFormat format, IntPtr data );

      [DllImport( "user32.dll" )]
      private static extern IntPtr GetClipboardData( ClipboardFormat format );

      [DllImport( "user32.dll" )]
      private static extern bool EmptyClipboard();

      #endregion

      #region ClipboardFormat

      public enum ClipboardFormat : uint
      {
         CF_BITMAP = 2,
         CF_DIB = 8,
         CF_DIBV5 = 17,
         CF_DIF = 5,
         CF_DSPBITMAP = 0x0082,
         CF_DSPENHMETAFILE = 0x008E,
         CF_DSPMETAFILEPICT = 0x0083,
         CF_DSPTEXT = 0x0081,
         CF_ENHMETAFILE = 14,
         CF_GDIOBJFIRST = 0x0300,
         CF_GDIOBJLAST = 0x03FF,
         CF_HDROP = 15,
         CF_LOCALE = 16,
         CF_METAFILEPICT = 3,
         CF_OEMTEXT = 7,
         CF_OWNERDISPLAY = 0x0080,
         CF_PALETTE = 9,
         CF_PENDATA = 10,
         CF_PRIVATEFIRST = 0x0200,
         CF_PIRVATELAST = 0x02FF,
         CF_RIFF = 11,
         CF_SYLK = 4,
         CF_TEXT = 1,
         CF_TIFF = 6,
         CF_UNICODETEXT = 13,
         CF_WAVE = 12
      }

      #endregion

      public static void Copy( string text, bool unicode )
      {
         if ( !ClipboardApi.OpenClipboard( IntPtr.Zero ) )
         {
            throw new Win32Exception( Marshal.GetLastWin32Error() );
         }

         try
         {
            if ( unicode )
            {
               var unicodeBytes = Encoding.Unicode.GetBytes( text );
               var memory = Marshal.AllocHGlobal( unicodeBytes.Length );

               Marshal.Copy( unicodeBytes, 0, memory, unicodeBytes.Length );
               ClipboardApi.SetClipboardData( ClipboardFormat.CF_UNICODETEXT, memory );
            }
            else
            {
               var unicodeBytes = Encoding.Unicode.GetBytes( text );
               var memory = Marshal.AllocHGlobal( unicodeBytes.Length );

               var asciiBytes = Encoding.Convert( Encoding.Unicode, Encoding.ASCII, unicodeBytes );

               Marshal.Copy( asciiBytes, 0, memory, asciiBytes.Length );
               ClipboardApi.SetClipboardData( ClipboardFormat.CF_TEXT, memory );
            }
         }
         finally
         {
            ClipboardApi.CloseClipboard();
         }
      }
   }
}
