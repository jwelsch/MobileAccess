using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace MobileAccess
{
   public class WpdDevice : IWpdDevice, IWpdObject, IDisposable
   {
      private PortableDeviceClass device = null;

      private string name;
      public string Name
      {
         get
         {
            if ( this.name == null )
            {
               var property = this.Properties.Find<string>( PortableDevicePKeys.WPD_DEVICE_FRIENDLY_NAME );
               this.name = property.Value;
            }

            return this.name;
         }
      }

      public string OriginalFileName
      {
         get { return this.name; }
      }

      public bool IsContainer
      {
         get { return true; }
      }

      public ulong Size
      {
         get { return 0UL; }
      }

      public string ObjectID
      {
         get { return "DEVICE"; }
      }

      private IPortableDeviceContent content;
      public IPortableDeviceContent Content
      {
         get { return this.content; }
      }

      public IWpdObject Parent
      {
         get { return null; }
      }

      public string DeviceID
      {
         get;
         private set;
      }

      public WpdPropertyCollection Properties
      {
         get;
         private set;
      }

      public WpdDevice( string deviceId )
      {
         var clientInfo = new PortableDeviceTypesLib.PortableDeviceValuesClass();
         this.device = new PortableDeviceClass();
         this.device.Open( deviceId, (IPortableDeviceValues) clientInfo );
         this.DeviceID = deviceId;
         this.device.Content( out this.content );

         this.Properties = new WpdPropertyCollection( this.Content, this.ObjectID );
         this.Properties.Refresh();
      }

      ~WpdDevice()
      {
         this.CleanUp();
      }

      public string[] GetChildPaths( string searchPattern, bool recursive )
      {
         throw new NotImplementedException();
      }

      private void Enumerate( ref IPortableDeviceContent pContent, string parentID, string indent )
      {
         //
         // Output object ID
         //
         Console.WriteLine( indent + parentID );

         indent += "   ";

         //
         // Enumerate children (if any)
         //
         IEnumPortableDeviceObjectIDs pEnum;
         pContent.EnumObjects( 0, parentID, null, out pEnum );

         var cFetched = 0U;
         do
         {
            string objectID;
            pEnum.Next( 1, out objectID, ref cFetched );

            if ( cFetched > 0 )
            {
               //
               // Recurse into children
               //
               this.Enumerate( ref pContent, objectID, indent );
            }
         } while ( cFetched > 0 );
      }

      public void StartEnumerate()
      {
         var content = this.Content;
         this.Enumerate( ref this.content, this.ObjectID, "" );
      }

      public IWpdObject[] GetChildren()
      {
         IEnumPortableDeviceObjectIDs pEnum;
         this.Content.EnumObjects( 0, this.ObjectID, null, out pEnum );

         var objects = new List<IWpdObject>();
         var cFetched = 0U;
         do
         {
            string objectID;
            pEnum.Next( 1, out objectID, ref cFetched );

            if ( cFetched > 0 )
            {
               objects.Add( new WpdObject( objectID, this, this.Content ) );
            }
         }
         while ( cFetched > 0 );

         return objects.ToArray();
      }

      public IWpdObject ObjectFromPath( string path, bool createPath )
      {
         var entries = path.Split( new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries );

         var start = 0;

         if ( ( entries.Length > 0 ) && ( String.Compare( entries[0], this.Name ) == 0 ) )
         {
            start = 1;
         }

         IWpdObject final = this;
         var found = false;
         var children = this.GetChildren();
         for ( var i = start; i < entries.Length; i++ )
         {
            found = false;
            foreach ( var child in children )
            {
               if ( String.Compare( child.Name, entries[i], true ) == 0 )
               {
                  found = true;
                  final = child;
                  if ( i < entries.Length - 1 )
                  {
                     children = child.GetChildren();
                  }
                  break;
               }
            }

            if ( !found )
            {
               foreach ( var child in children )
               {
                  if ( child.OriginalFileName != null )
                  {
                     if ( String.Compare( child.OriginalFileName, entries[i], true ) == 0 )
                     {
                        found = true;
                        final = child;
                        if ( i < entries.Length - 1 )
                        {
                           children = child.GetChildren();
                        }
                        break;
                     }
                  }
               }

               if ( !found )
               {
                  if ( createPath )
                  {
                     var commander = new DeviceCommander( this );
                     final = commander.CreateDirectory( final, entries[i] );
                     children = new IWpdObject[0];
                  }
                  else
                  {
                     var errorEntries = new string[i + 2];
                     errorEntries[0] = this.Name;
                     for ( var j = 0; j <= i; j++ )
                     {
                        errorEntries[j + 1] = entries[j];
                     }

                     var errorPath = errorEntries.DelimitedString( "\\" );
                     throw new FileNotFoundException( String.Format( "An object with the path \"{0}\" was not found.", errorPath ) );
                  }
               }
            }
         }

         return final;
      }

      public string GetPath()
      {
         return this.Name;
      }

      public string GetNameOnDevice()
      {
         return this.Name;
      }

      public void CleanUp()
      {
         if ( this.device != null )
         {
            this.device.Close();
            this.device = null;
         }
      }

      #region IDisposable implementation

      public void Dispose()
      {
         this.Dispose( true );
         GC.SuppressFinalize( this );
      }

      private void Dispose( bool disposing )
      {
         if ( disposing )
         {
            // Free managed objects here.
         }

         this.CleanUp();
      }

      #endregion
   }
}
