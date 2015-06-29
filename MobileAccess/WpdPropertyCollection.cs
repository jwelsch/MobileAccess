using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public class WpdPropertyCollection : Collection<IWpdProperty>
   {
      private IPortableDeviceContent content;
      private string objectID;

      public WpdPropertyCollection( IPortableDeviceContent content, string objectID )
      {
         this.content = content;
         this.objectID = objectID;
      }

      public void Refresh()
      {
         if ( this.Count > 0 )
         {
            this.Clear();
         }

         IPortableDeviceProperties properties = null;
         this.content.Properties( out properties );

         IPortableDeviceValues values = null;
         properties.GetValues( this.objectID, null, out values );

         var count = 0U;
         values.GetCount( ref count );

         var key = new _tagpropertykey();
         var propVar = new tag_inner_PROPVARIANT();

         for ( var i = 0U; i < count; i++ )
         {
            values.GetAt( i, ref key, ref propVar );
            this.Add( WpdProperty.Create( key, propVar ) );
         }
      }

      public IWpdProperty Find( _tagpropertykey key, bool throwNotFound = true )
      {
         foreach ( var item in this )
         {
            if ( PortableDevicePKeys.Equals( key, item.Key ) )
            {
               return item;
            }
         }

         if ( throwNotFound )
         {
            throw new KeyNotFoundException();
         }

         return null;
      }

      public WpdProperty<T> Find<T>( _tagpropertykey key, bool throwNotFound = true )
      {
         var property = this.Find( key, throwNotFound );

         if ( property == null )
         {
            return null;
         }

         return (WpdProperty<T>) property;
      }
   }
}
