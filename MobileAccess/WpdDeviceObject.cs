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

         var hasOriginalFileName = false;
         var hasObjectName = false;
         IPortableDeviceKeyCollection keys;
         properties.GetSupportedProperties( this.ObjectID, out keys );
         var count = 0U;
         keys.GetCount( ref count );
         for ( var i = 0U; i < count; i++ )
         {
            _tagpropertykey key = new _tagpropertykey();
            keys.GetAt( i, ref key );

            if ( PortableDevicePKeys.Equals( key, PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME ) )
            {
               hasOriginalFileName = true;
            }

            if ( PortableDevicePKeys.Equals( key, PortableDevicePKeys.WPD_OBJECT_NAME ) )
            {
               hasObjectName = true;
            }
         }

         if ( hasOriginalFileName )
         {
            property = PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME;
            values.GetStringValue( ref property, out this.originalFileName );
         }

         if ( hasObjectName )
         {
            property = PortableDevicePKeys.WPD_OBJECT_NAME;
            values.GetStringValue( ref property, out this.name );
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
         if ( this.Parent == null )
         {
            return this.Name;
         }

         return this.Parent.GetPath() + Path.DirectorySeparatorChar + this.Name;
      }
   }
}
