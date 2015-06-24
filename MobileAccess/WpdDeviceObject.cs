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
         get
         {
            if ( this.name == null )
            {
               IPortableDeviceProperties properties = null;
               this.Content.Properties( out properties );

               PortableDeviceApiLib.IPortableDeviceValues values = null;
               properties.GetValues( this.ObjectID, null, out values );

               var property = PortableDevicePKeys.WPD_OBJECT_NAME;
               values.GetStringValue( ref property, out this.name );
            }

            return this.name;
         }
      }

      public WpdDeviceObject( string objectID, IWpdDeviceObject parent, IPortableDeviceContent content )
      {
         this.ObjectID = objectID;
         this.Parent = parent;
         this.Content = content;
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
