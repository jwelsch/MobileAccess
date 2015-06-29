using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileAccess
{
   public interface IWpdProperty
   {
      _tagpropertykey Key
      {
         get;
      }

      tag_inner_PROPVARIANT Variant
      {
         get;
      }

      VarEnum Type
      {
         get;
      }

      object Value
      {
         get;
      }
   }

   public class WpdProperty
   {
      public static IWpdProperty Create( _tagpropertykey key, string variantValue )
      {
         return WpdProperty.Create( key, PROPVARIANT.Create_tag_inner_PROPVARIANT( variantValue ) );
      }

      public static IWpdProperty Create( _tagpropertykey key, UInt64 variantValue )
      {
         return WpdProperty.Create( key, PROPVARIANT.Create_tag_inner_PROPVARIANT( variantValue ) );
      }

      public static IWpdProperty Create( _tagpropertykey key, bool variantValue )
      {
         return WpdProperty.Create( key, PROPVARIANT.Create_tag_inner_PROPVARIANT( variantValue ) );
      }

      public static IWpdProperty Create( _tagpropertykey key, Guid variantValue )
      {
         return WpdProperty.Create( key, PROPVARIANT.Create_tag_inner_PROPVARIANT( variantValue ) );
      }

      public static IWpdProperty Create( _tagpropertykey key, UInt32 variantValue )
      {
         return WpdProperty.Create( key, PROPVARIANT.Create_tag_inner_PROPVARIANT( variantValue ) );
      }

      public static IWpdProperty Create( _tagpropertykey key, Int32 variantValue )
      {
         return WpdProperty.Create( key, PROPVARIANT.Create_tag_inner_PROPVARIANT( variantValue ) );
      }

      public static IWpdProperty Create( _tagpropertykey key, float variantValue )
      {
         return WpdProperty.Create( key, PROPVARIANT.Create_tag_inner_PROPVARIANT( variantValue ) );
      }

      public static IWpdProperty Create( _tagpropertykey key, tag_inner_PROPVARIANT variant )
      {
         var ptrValue = Marshal.AllocHGlobal( Marshal.SizeOf( variant ) );

         try
         {
            Marshal.StructureToPtr( variant, ptrValue, false );
            var pv = WpdProperty.MarshalToStructure<PROPVARIANT>( ptrValue );

            var ve = (VarEnum) pv.vt;

            switch ( ve )
            {
               case VarEnum.VT_I1:
                  return new WpdProperty<sbyte>( key, variant, ve, pv.AsSByte() );
               case VarEnum.VT_UI1:
                  return new WpdProperty<byte>( key, variant, ve, pv.AsByte() );
               case VarEnum.VT_I2:
                  return new WpdProperty<short>( key, variant, ve, pv.AsInt16() );
               case VarEnum.VT_UI2:
                  return new WpdProperty<ushort>( key, variant, ve, pv.AsUInt16() );
               case VarEnum.VT_I4:
               case VarEnum.VT_INT:
               case VarEnum.VT_ERROR:
                  return new WpdProperty<int>( key, variant, ve, pv.AsInt32() );
               case VarEnum.VT_UI4:
               case VarEnum.VT_UINT:
                  return new WpdProperty<uint>( key, variant, ve, pv.AsUInt32() );
               case VarEnum.VT_I8:
                  return new WpdProperty<long>( key, variant, ve, pv.AsInt64() );
               case VarEnum.VT_UI8:
                  return new WpdProperty<ulong>( key, variant, ve, pv.AsUInt64() );
               case VarEnum.VT_R4:
                  return new WpdProperty<float>( key, variant, ve, pv.AsFloat() );
               case VarEnum.VT_R8:
                  return new WpdProperty<double>( key, variant, ve, pv.AsDouble() );
               case VarEnum.VT_BOOL:
                  return new WpdProperty<bool>( key, variant, ve, pv.AsBool() );
               case VarEnum.VT_CY:
                  return new WpdProperty<decimal>( key, variant, ve, pv.AsDecimal() );
               case VarEnum.VT_DATE:
               case VarEnum.VT_FILETIME:
                  return new WpdProperty<DateTime>( key, variant, ve, pv.AsDateTime() );
               case VarEnum.VT_BSTR:
               case VarEnum.VT_LPSTR:
               case VarEnum.VT_LPWSTR:
                  return new WpdProperty<string>( key, variant, ve, pv.AsString() );
               case VarEnum.VT_BLOB:
                  return new WpdProperty<byte[]>( key, variant, ve, pv.AsByteBuffer() );
               case VarEnum.VT_UNKNOWN:
                  return new WpdProperty<object>( key, variant, ve, pv.AsIUnknown() );
               case VarEnum.VT_DISPATCH:
               case VarEnum.VT_PTR:
                  return new WpdProperty<IntPtr>( key, variant, ve, pv.AsIntPtr() );
               case VarEnum.VT_CLSID:
                  return new WpdProperty<Guid>( key, variant, ve, pv.AsGuid() );
               case VarEnum.VT_EMPTY:
                  return new WpdProperty<object>( key, variant, ve, null );
               default:
                  return new WpdProperty<object>( key, variant, ve, pv.AsIntPtr() );
            }
         }
         finally
         {
            Marshal.FreeHGlobal( ptrValue );
         }
      }

      private static U MarshalToStructure<U>( IntPtr data ) where U : struct
      {
         return (U) Marshal.PtrToStructure( data, typeof( U ) );
      }
   }

   public class WpdProperty<T> : IWpdProperty
   {
      public _tagpropertykey Key
      {
         get;
         private set;
      }

      public tag_inner_PROPVARIANT Variant
      {
         get;
         private set;
      }

      public T Value
      {
         get;
         private set;
      }

      public VarEnum Type
      {
         get;
         private set;
      }

      object IWpdProperty.Value
      {
         get { return this.Value; }
      }

      public WpdProperty( _tagpropertykey key, tag_inner_PROPVARIANT variant, VarEnum type, T value )
      {
         this.Key = key;
         this.Variant = variant;
         this.Type = type;
         this.Value = value;
      }

      public WpdProperty( _tagpropertykey key, tag_inner_PROPVARIANT variant, VarEnum type, object value )
         : this( key, variant, type, (T) value )
      {
      }
   }
}
