using System;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;

namespace MobileAccess
{
   public enum VariantType : ushort
   {
      //
      // https://msdn.microsoft.com/en-us/library/windows/desktop/aa380072(v=vs.85).aspx
      //

      VT_EMPTY = 0,
      VT_NULL = 1,
      VT_I1 = 16, // 1-byte signed integer.
      VT_UI1 = 17, // 1-byte unsigned integer.
      VT_I2 = 2, // 2-byte signed integer.
      VT_UI2 = 18, // 2-byte unsigned integer.
      VT_I4 = 3, // 4-byte signed integer.
      VT_UI4 = 19, // 4-byte unsigned integer.
      VT_INT = 22, // 4-byte signed integer.
      VT_UINT = 23, // 4-byte unsigned integer.
      VT_I8 = 20, // 8-byte signed integer.
      VT_UI8 = 21, // 8-byte unsigned integer.
      VT_R4 = 4, // 32-bit IEEE floating point value.
      VT_R8 = 5, // 64-bit IEEE floating point value.
      VT_BOOL = 11, // Boolean value (0 = FALSE, -1 = TRUE).
      VT_ERROR = 10, // DWORD that contains a status code.
      VT_CY = 6, // 8-byte two's compliment integer scaled by 10,000.
      VT_DATE = 7, // 64-bit floating point number representing days since 12/31/1899.
      VT_FILETIME = 64, // 64-bit FILETIME structure.
      VT_CLSID = 72, // Pointer to a GUID.
      VT_CF = 71, // Pointer to CLIPDATA structure.
      VT_BSTR = 8, // Pointer to NULL-terminated Unicode string.
      VT_BSTR_BLOB = 0xFFF, // For system use only.
      VT_BLOB = 65,
      VT_BLOBOBJECT = 70,
      VT_LPSTR = 30, // Pointer to NULL-terminated ANSI string.
      VT_LPWSTR = 31, // Pointer to a NULL-terminated Unicode string.
      VT_UNKNOWN = 13,
      VT_DISPATCH = 9,
      VT_STREAM = 66,
      VT_STREAMED_OBJECT = 68,
      VT_STORAGE = 67,
      VT_STORED_OBJECT = 69,
      VT_VERSIONED_STREAM = 73,
      VT_DECIMAL = 14,
      VT_VECTOR = 0x1000,
      VT_ARRAY = 0x2000,
      VT_BYREF = 0x4000,
      VT_VARIANT = 12,
      VT_TYPEMASK = 0xFFF
   }

   public class PropVariant
   {
      [StructLayout( LayoutKind.Explicit, Size = 16 )]
      private struct PROPVARIANT
      {
         [FieldOffset( 0 )]
         public VariantType variantType;
         #region Reserved
         [FieldOffset( 2 )]
         public ushort reserved1;
         [FieldOffset( 4 )]
         public ushort reserved2;
         [FieldOffset( 6 )]
         public ushort reserved3;
         #endregion
         [FieldOffset( 8 )]
         public IntPtr pointerValue;
         [FieldOffset( 8 )]
         public byte byteValue;
         [FieldOffset( 8 )]
         public sbyte sbyteValue;
         [FieldOffset( 8 )]
         public short shortValue;
         [FieldOffset( 8 )]
         public ushort ushortValue;
         [FieldOffset( 8 )]
         public int intValue;
         [FieldOffset( 8 )]
         public uint uintValue;
         [FieldOffset( 8 )]
         public long longValue;
         [FieldOffset( 8 )]
         public ulong ulongValue;
         [FieldOffset( 8 )]
         public DateTime dateValue;
         [FieldOffset( 8 )]
         public double doubleValue;
         [FieldOffset( 8 )]
         public float floatValue;
         [FieldOffset( 8 )]
         public int boolValue;
         [FieldOffset( 8 )]
         public Guid clsidValue;
         [FieldOffset( 8 )]
         public string stringValue;
      }

      private PROPVARIANT value;

      public PropVariant( IPortableDeviceContent content, string objectID, _tagpropertykey key )
      {
         IPortableDeviceProperties properties;
         content.Properties( out properties );

         IPortableDeviceValues values;
         properties.GetValues( objectID, null, out values );

         this.LoadData( values, key );
      }

      public PropVariant( IPortableDeviceValues values, _tagpropertykey key )
      {
         this.LoadData( values, key );
      }

      private void LoadData( IPortableDeviceValues values, _tagpropertykey key )
      {
         tag_inner_PROPVARIANT ipValue;
         values.GetValue( ref key, out ipValue );

         // Allocate memory for the intermediate marshalled object and marshal it as a pointer
         var ptrValue = Marshal.AllocHGlobal( Marshal.SizeOf( ipValue ) );

         try
         {
            Marshal.StructureToPtr( ipValue, ptrValue, false );

            // Marshal the pointer into our C# object
            var pv = MarshalToStructure<PROPVARIANT>( ptrValue );

            switch ( (VariantType) ipValue.vt )
            {
               case VariantType.VT_LPWSTR:
               {
                  this.value.stringValue = Marshal.PtrToStringUni( pv.pointerValue );
                  break;
               }
               case VariantType.VT_LPSTR:
               {
                  this.value.stringValue = Marshal.PtrToStringAnsi( pv.pointerValue );
                  break;
               }
               case VariantType.VT_BSTR:
               {
                  this.value.stringValue = Marshal.PtrToStringBSTR( pv.pointerValue );
                  break;
               }
               case VariantType.VT_BOOL:
               {
                  values.GetBoolValue( key, out this.value.boolValue );
                  break;
               }
               case VariantType.VT_DATE:
               {
                  this.value.dateValue = DateTime.FromOADate( MarshalToStructure<Double>( pv.pointerValue ) );
                  break;
               }
               case VariantType.VT_R4:
               {
                  values.GetFloatValue( key, out this.value.floatValue );
                  break;
               }
               case VariantType.VT_R8:
               {
                  this.value.doubleValue = MarshalToStructure<Double>( pv.pointerValue );
                  break;
               }
               case VariantType.VT_UI1:
               {
                  this.value.byteValue = MarshalToStructure<Byte>( pv.pointerValue );
                  break;
               }
               case VariantType.VT_I1:
               {
                  this.value.sbyteValue = MarshalToStructure<SByte>( pv.pointerValue );
                  break;
               }
               case VariantType.VT_UI2:
               {
                  this.value.ushortValue = MarshalToStructure<UInt16>( pv.pointerValue );
                  break;
               }
               case VariantType.VT_I2:
               {
                  this.value.shortValue = MarshalToStructure<Int16>( pv.pointerValue );
                  break;
               }
               case VariantType.VT_UI4:
               {
                  values.GetUnsignedIntegerValue( key, out this.value.uintValue );
                  break;
               }
               case VariantType.VT_I4:
               {
                  values.GetSignedIntegerValue( key, out this.value.intValue );
                  break;
               }
               case VariantType.VT_UI8:
               {
                  values.GetUnsignedLargeIntegerValue( key, out this.value.ulongValue );
                  break;
               }
               case VariantType.VT_I8:
               {
                  values.GetSignedLargeIntegerValue( key, out this.value.longValue );
                  break;
               }
               case VariantType.VT_CLSID:
               {
                  values.GetGuidValue( key, out this.value.clsidValue );
                  break;
               }
            }
         }
         finally
         {
            Marshal.FreeHGlobal( ptrValue );
         }
      }

      private static T MarshalToStructure<T>( IntPtr data ) where T : struct
      {
         return (T) Marshal.PtrToStructure( data, typeof( T ) );
      }

      public DateTime AsDateTime()
      {
         //if ( this.value. != VariantType.VT_DATE )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.dateValue;
      }

      public byte AsByte()
      {
         //if ( this.value.variantType != VariantType.VT_UI1 )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.byteValue;
      }

      public sbyte AsSByte()
      {
         //if ( this.value.variantType != VariantType.VT_I1 )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.sbyteValue;
      }

      public short AsShort()
      {
         //if ( this.value.variantType != VariantType.VT_I2 )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.shortValue;
      }

      public ushort AsUShort()
      {
         //if ( this.value.variantType != VariantType.VT_UI2 )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.ushortValue;
      }

      public int AsInt()
      {
         //if ( this.value.variantType != VariantType.VT_I4 && this.value.variantType != VariantType.VT_INT )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.intValue;
      }

      public uint AsUInt()
      {
         //if ( this.value.variantType != VariantType.VT_UI4 && this.value.variantType != VariantType.VT_UINT )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.uintValue;
      }

      public long AsLong()
      {
         //if ( this.value.variantType != VariantType.VT_I8 )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.longValue;
      }

      public float AsFloat()
      {
         //if ( this.value.variantType != VariantType.VT_R4 )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.floatValue;
      }

      public double AsDouble()
      {
         //if ( this.value.variantType != VariantType.VT_R8 )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         return this.value.doubleValue;
      }

      public bool AsBool()
      {
         //if ( this.value.variantType != VariantType.VT_BOOL )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         if ( this.value.boolValue == 0 )
         {
            return false;
         }
         else if ( this.value.boolValue == -1 )
         {
            return true;
         }

         throw new FormatException( String.Format( "Boolean value was \"{0}\".", this.value.boolValue ) );
      }

      public Guid AsClsid()
      {
         //if ( this.value.variantType != VariantType.VT_CLSID )
         //{
         //   throw new InvalidOperationException( "Cannot be converted to type." );
         //}

         //this.value.clsidValue = (Guid) Marshal.PtrToStructure( this.value.pointerValue, typeof( Guid ) );

         return this.value.clsidValue;
      }

      public override string ToString()
      {
         return base.ToString();
         //switch ( this.variantType )
         //{
         //   case VariantType.VT_LPWSTR:
         //   {
         //      return Marshal.PtrToStringUni( this.pointerValue );
         //   }
         //   case VariantType.VT_LPSTR:
         //   {
         //      return Marshal.PtrToStringAnsi( this.pointerValue );
         //   }
         //   case VariantType.VT_BSTR:
         //   {
         //      return Marshal.PtrToStringBSTR( this.pointerValue );
         //   }
         //   case VariantType.VT_BOOL:
         //   {
         //      return this.ToBool().ToString();
         //   }
         //   case VariantType.VT_DATE:
         //   {
         //      return this.ToDateTime().ToString();
         //   }
         //   case VariantType.VT_R4:
         //   {
         //      return this.floatValue.ToString();
         //   }
         //   case VariantType.VT_R8:
         //   {
         //      return this.doubleValue.ToString();
         //   }
         //   case VariantType.VT_UI1:
         //   {
         //      return this.byteValue.ToString();
         //   }
         //   case VariantType.VT_I1:
         //   {
         //      return this.sbyteValue.ToString();
         //   }
         //   case VariantType.VT_UI2:
         //   {
         //      return this.ushortValue.ToString();
         //   }
         //   case VariantType.VT_I2:
         //   {
         //      return this.shortValue.ToString();
         //   }
         //   case VariantType.VT_UI4:
         //   {
         //      return this.uintValue.ToString();
         //   }
         //   case VariantType.VT_I4:
         //   {
         //      return this.intValue.ToString();
         //   }
         //   case VariantType.VT_UI8:
         //   {
         //      return this.longValue.ToString();
         //   }
         //   case VariantType.VT_I8:
         //   {
         //      return this.ulongValue.ToString();
         //   }
         //}

         //return this.pointerValue.ToString();
      }

      //public static tag_inner_PROPVARIANT MarshalTo( PropVariant propVariant )
      //{
      //   var ptrValue = Marshal.AllocHGlobal( Marshal.SizeOf( propVariant ) );
      //   Marshal.StructureToPtr( propVariant, ptrValue, false );
      //   return (tag_inner_PROPVARIANT) Marshal.PtrToStructure( ptrValue, typeof( tag_inner_PROPVARIANT ) );
      //}

      //public static PropVariant MarshalFrom( tag_inner_PROPVARIANT propVariant )
      //{
      //   var ipValue = new tag_inner_PROPVARIANT();
      //   var ptrValue = Marshal.AllocHGlobal( Marshal.SizeOf( ipValue ) );
      //   Marshal.StructureToPtr( ipValue, ptrValue, false );
      //   return (PropVariant) Marshal.PtrToStructure( ptrValue, typeof( PropVariant ) );
      //}

      public static tag_inner_PROPVARIANT StringToPropVariant( string value )
      {
         // We'll use an IPortableDeviceValues object to transform the
         // string into a PROPVARIANT
         var pValues = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();

         // We insert the string value into the IPortableDeviceValues object
         // using the SetStringValue method
         pValues.SetStringValue( ref PortableDevicePKeys.WPD_OBJECT_ID, value );

         var variant = new tag_inner_PROPVARIANT();

         // We then extract the string into a PROPVARIANT by using the 
         // GetValue method
         pValues.GetValue( ref PortableDevicePKeys.WPD_OBJECT_ID, out variant );

         return variant;
      }
   }
}