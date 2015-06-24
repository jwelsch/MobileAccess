using System;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;

namespace MobileAccess
{
   public enum VariantType : short
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

   [StructLayout( LayoutKind.Explicit, Size = 16 )]
   public struct PropVariant
   {
      [FieldOffset( 0 )]
      public VariantType variantType;
      [FieldOffset( 8 )]
      public IntPtr pointerValue;
      [FieldOffset( 8 )]
      public byte byteValue;
      [FieldOffset( 8 )]
      public long longValue;
      [FieldOffset( 8 )]
      public double dateValue;
      [FieldOffset( 8 )]
      public short boolValue;

      public DateTime ToDateTime()
      {
         if ( this.variantType != VariantType.VT_DATE )
         {
            throw new InvalidOperationException( "Cannot be converted to type." );
         }

         return DateTime.FromOADate( this.dateValue );
      }

      public void FromDateTime( DateTime dt )
      {
         if ( this.variantType != VariantType.VT_DATE )
         {
            throw new InvalidOperationException( "Cannot be converted to type." );
         }

         this.dateValue = dt.ToOADate();
      }

      public static tag_inner_PROPVARIANT MarshalTo( PropVariant propVariant )
      {
         var ptrValue = Marshal.AllocHGlobal( Marshal.SizeOf( propVariant ) );
         Marshal.StructureToPtr( propVariant, ptrValue, false );
         return (tag_inner_PROPVARIANT) Marshal.PtrToStructure( ptrValue, typeof( tag_inner_PROPVARIANT ) );
      }

      public static PropVariant MarshalFrom( tag_inner_PROPVARIANT propVariant )
      {
         var ipValue = new tag_inner_PROPVARIANT();
         var ptrValue = Marshal.AllocHGlobal( Marshal.SizeOf( ipValue ) );
         Marshal.StructureToPtr( ipValue, ptrValue, false );
         return (PropVariant) Marshal.PtrToStructure( ptrValue, typeof( PropVariant ) );
      }
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