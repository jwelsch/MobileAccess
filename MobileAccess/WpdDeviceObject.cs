﻿using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public class WpdDeviceObject : IWpdDeviceObject
   {
      public string ObjectID
      {
         get;
         private set;
      }

      public IWpdDeviceObject Parent
      {
         get;
         private set;
      }

      public IPortableDeviceContent Content
      {
         get;
         private set;
      }

      private string name;
      public string Name
      {
         get { return this.name; }
      }

      private string originalFileName;
      public string OriginalFileName
      {
         get { return this.originalFileName; }
      }

      private bool isContainer;
      public bool IsContainer
      {
         get { return this.isContainer; }
      }

      private ulong size;
      public ulong Size
      {
         get { return this.size; }
      }

      public WpdDeviceObject( string objectID, IWpdDeviceObject parent, IPortableDeviceContent content )
      {
         this.ObjectID = objectID;
         this.Parent = parent;
         this.Content = content;

         IPortableDeviceProperties properties = null;
         this.Content.Properties( out properties );

         PortableDeviceApiLib.IPortableDeviceValues values = null;
         properties.GetValues( this.ObjectID, null, out values );

         var property = PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE;
         Guid value;
         values.GetGuidValue( ref property, out value );
         this.isContainer = value == PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER;

         IPortableDeviceKeyCollection keys;
         properties.GetSupportedProperties( this.ObjectID, out keys );
         var count = 0U;
         keys.GetCount( ref count );
         var propertiesRead = 0;
         var maxProperties = this.IsContainer ? 2 : 3;

         for ( var i = 0U; i < count && propertiesRead < maxProperties; i++ )
         {
            _tagpropertykey key = new _tagpropertykey();
            keys.GetAt( i, ref key );

            property = PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME;
            if ( PortableDevicePKeys.Equals( key, property ) )
            {
               values.GetStringValue( ref property, out this.originalFileName );
               propertiesRead++;
            }

            property = PortableDevicePKeys.WPD_OBJECT_NAME;
            if ( PortableDevicePKeys.Equals( key, property ) )
            {
               values.GetStringValue( ref property, out this.name );
               propertiesRead++;
            }

            property = PortableDevicePKeys.WPD_OBJECT_SIZE;
            if ( PortableDevicePKeys.Equals( key, property ) )
            {
               values.GetUnsignedLargeIntegerValue( ref property, out this.size );
               propertiesRead++;
            }
         }
      }

      public IWpdDeviceObject[] GetChildren()
      {
         IEnumPortableDeviceObjectIDs pEnum;
         this.Content.EnumObjects( 0, this.ObjectID, null, out pEnum );

         var objects = new List<WpdDeviceObject>();
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

      public string GetPath()
      {
         var name = this.OriginalFileName == null ? this.Name : this.OriginalFileName;

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
         IPortableDeviceProperties properties;
         this.Content.Properties( out properties );

         IPortableDeviceKeyCollection keys;
         IPortableDeviceValues values;
         var count = 0U;

         properties.GetSupportedProperties( this.ObjectID, out keys );
         properties.GetValues( this.ObjectID, keys, out values );

         values.GetCount( ref count );

         for ( var i = 0U; i < count; i++ )
         {
            _tagpropertykey key = new _tagpropertykey();
            tag_inner_PROPVARIANT variant = new tag_inner_PROPVARIANT();

            values.GetAt( i, ref key, ref variant );

            var output = String.Format( "[{0}] {1}", i, PortableDevicePKeys.FindKeyName( key ) );
            System.Diagnostics.Trace.WriteLine( output );
            Console.WriteLine( output );
         }
      }
   }
}
