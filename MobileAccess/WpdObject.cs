using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace MobileAccess
{
   public class WpdObject : IWpdObject
   {
      public string ObjectID
      {
         get;
         private set;
      }

      public IWpdObject Parent
      {
         get;
         private set;
      }

      public IPortableDeviceContent Content
      {
         get;
         private set;
      }

      public string Name
      {
         get;
         private set;
      }

      public string OriginalFileName
      {
         get;
         private set;
      }

      public bool IsContainer
      {
         get;
         private set;
      }

      public ulong Size
      {
         get;
         private set;
      }

      public WpdPropertyCollection Properties
      {
         get;
         private set;
      }

      public WpdObject( string objectID, IWpdObject parent, IPortableDeviceContent content )
      {
         this.ObjectID = objectID;
         this.Parent = parent;
         this.Content = content;
         this.Properties = new WpdPropertyCollection( this.Content, this.ObjectID );
         this.Properties.Refresh();

         var name = this.Properties.Find<string>( PortableDevicePKeys.WPD_OBJECT_NAME, false );
         this.Name = name == null ? string.Empty : name.Value;

         var originalFileName = this.Properties.Find<string>( PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, false );
         this.OriginalFileName = originalFileName == null ? string.Empty : originalFileName.Value;

         var size = this.Properties.Find<ulong>( PortableDevicePKeys.WPD_OBJECT_SIZE, false );
         this.Size = size == null ? 0UL : size.Value;

         var value = this.Properties.Find<Guid>( PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, false );
         this.IsContainer = value == null ? false : value.Value == PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER;
      }

      public IWpdObject[] GetChildren()
      {
         IEnumPortableDeviceObjectIDs pEnum;
         this.Content.EnumObjects( 0, this.ObjectID, null, out pEnum );

         var objects = new List<WpdObject>();
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

      public string GetPath()
      {
         var name = String.IsNullOrEmpty( this.OriginalFileName ) ? this.Name : this.OriginalFileName;

         if ( this.Parent == null )
         {
            return name;
         }

         return this.Parent.GetPath() + Path.DirectorySeparatorChar + name;
      }

      public string GetNameOnDevice()
      {
         return this.Name == null ? this.OriginalFileName : this.Name;
      }

      public string[] GetChildPaths( string searchPattern, bool recursive )
      {
         var children = this.GetChildren();

         if ( children.Length == 0 )
         {
            var path = this.GetPath();
            return WildcardSearch.Match( searchPattern, path ) ? new string[] { path } : new string[0];
         }

         var paths = new List<string>();
         foreach ( var child in children )
         {
            if ( !child.IsContainer || recursive )
            {
               var childPaths = child.GetChildPaths( searchPattern, recursive );
               paths.AddRange( childPaths );
            }
         }

         return paths.ToArray();
      }

      public void DumpSupportedResources()
      {
         IPortableDeviceResources resources;
         IPortableDeviceKeyCollection keys;
         var count = 0U;

         this.Content.Transfer( out resources );
         resources.GetSupportedResources( this.ObjectID, out keys );
         keys.GetCount( ref count );

         for ( var i = 0U; i < count; i++ )
         {
            _tagpropertykey key = new _tagpropertykey();
            keys.GetAt( i, ref key );
            var output = String.Format( "[{0}] {1}", i, PortableDevicePKeys.FindKeyName( key ) );
            System.Diagnostics.Trace.WriteLine( output );
            Console.WriteLine( output );
         }
      }

      public void DumpProperties()
      {
         for ( var i = 0; i < this.Properties.Count; i++ )
         {
            var output = String.Format( "{0} {1} ({2}): {3}", i.ToString( "D" + this.Properties.Count.CountDigits().ToString() ), PortableDevicePKeys.FindKeyName( this.Properties[i].Key ), this.Properties[i].Type, this.Properties[i].Value );
            System.Diagnostics.Trace.WriteLine( output );
            Console.WriteLine( output );
         }
      }
   }
}
