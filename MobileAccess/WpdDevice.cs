using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public class WpdDevice : IDevice, IWpdDeviceObject
   {
      private PortableDeviceClass device = null;

      private WpdDevice()
      {
      }

      ~WpdDevice()
      {
         this.CleanUp();
      }

      private string name;
      public string Name
      {
         get { return this.name; }
      }

      public string OriginalFileName
      {
         get { return this.name; }
      }

      public bool IsContainer
      {
         get { return true; }
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

      public IWpdDeviceObject Parent
      {
         get { return null; }
      }

      public string DeviceID
      {
         get;
         private set;
      }

      public static WpdDevice Create( string deviceId )
      {
         var clientInfo = new PortableDeviceTypesLib.PortableDeviceValuesClass();
         var device = new WpdDevice();
         device.device = new PortableDeviceClass();
         device.device.Open( deviceId, (IPortableDeviceValues) clientInfo );
         device.DeviceID = deviceId;
         device.device.Content( out device.content );

         IPortableDeviceContent content = null;
         IPortableDeviceProperties properties = null;
         device.device.Content( out content );
         content.Properties( out properties );

         IPortableDeviceValues values = null;
         properties.GetValues( device.ObjectID, null, out values );

         var property = PortableDevicePKeys.WPD_DEVICE_FRIENDLY_NAME;
         values.GetStringValue( ref property, out device.name );

         return device;
      }

      public void DisplayProperties()
      {
         //
         // Retrieve IPortableDeviceProperties interface required
         // to get all the properties
         //
         IPortableDeviceProperties pProperties;
         this.Content.Properties( out pProperties );

         //
         // Call the GetValues API, we specify null to indicate we
         // want to retrieve all properties
         //
         IPortableDeviceValues pPropValues;
         pProperties.GetValues( this.ObjectID, null, out pPropValues );

         //
         // Get count of properties
         //
         var cPropValues = 0U;
         pPropValues.GetCount( ref cPropValues );
         Console.WriteLine( "Received " + cPropValues.ToString() + " properties" );

         for ( var i = 0U; i < cPropValues; i++ )
         {
            //
            // Retrieve the property at index 'i'
            //
            var propKey = new PortableDeviceApiLib._tagpropertykey();
            var ipValue = new PortableDeviceApiLib.tag_inner_PROPVARIANT();
            pPropValues.GetAt( i, ref propKey, ref ipValue );

            //
            // Allocate memory for the intermediate marshalled object
            // and marshal it as a pointer
            //
            var ptrValue = Marshal.AllocHGlobal( Marshal.SizeOf( ipValue ) );
            Marshal.StructureToPtr( ipValue, ptrValue, false );

            //
            // Marshal the pointer into our C# object
            //
            var pvValue = (PropVariant) Marshal.PtrToStructure( ptrValue, typeof( PropVariant ) );

            //
            // Display the property if it a string (VT_LPWSTR is decimal 31)
            //
            if ( pvValue.variantType == VariantType.VT_LPWSTR )
            {
               Console.WriteLine( "{0}: Value is \"{1}\"", ( i + 1 ).ToString(), Marshal.PtrToStringUni( pvValue.pointerValue ) );
            }
            else
            {
               Console.WriteLine( "{0}: Vartype is {1}", ( i + 1 ).ToString(), pvValue.variantType.ToString() );
            }
         }
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

      public IWpdDeviceObject[] GetChildren()
      {
         IEnumPortableDeviceObjectIDs pEnum;
         this.Content.EnumObjects( 0, this.ObjectID, null, out pEnum );

         var objects = new List<IWpdDeviceObject>();
         var cFetched = 0U;
         do
         {
            string objectID;
            pEnum.Next( 1, out objectID, ref cFetched );

            if ( cFetched > 0 )
            {
               objects.Add( new WpdDeviceObject( objectID, this, this.Content ) );
            }
         }
         while ( cFetched > 0 );

         return objects.ToArray();
      }

      public IWpdDeviceObject ObjectFromPath( string path, bool createPath )
      {
         var directories = path.Split( new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries );

         IWpdDeviceObject final = this;
         var found = false;
         var children = this.GetChildren();
         for ( var i = 0; i < directories.Length; i++ )
         {
            found = false;
            foreach ( var child in children )
            {
               if ( String.Compare( child.Name, directories[i], true ) == 0 )
               {
                  found = true;
                  final = child;
                  if ( i < directories.Length - 1 )
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
                     if ( String.Compare( child.OriginalFileName, directories[i], true ) == 0 )
                     {
                        found = true;
                        final = child;
                        if ( i < directories.Length - 1 )
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
                     var commander = new DeviceCommander();
                     final = commander.CreateDirectory( final, directories[i] );
                     children = new IWpdDeviceObject[0];
                  }
                  else
                  {
                     var errorDirectories = new string[i + 2];
                     errorDirectories[0] = this.Name;
                     for ( var j = 0; j <= i; j++ )
                     {
                        errorDirectories[j + 1] = directories[j];
                     }

                     var errorPath = errorDirectories.DelimitedString( "\\" );
                     throw new DirectoryNotFoundException( String.Format( "An object with the path \"{0}\" was not found.", errorPath ) );
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
