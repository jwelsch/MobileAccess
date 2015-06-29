using PortableDeviceApiLib;
using System;
using System.Runtime.InteropServices;

namespace MobileAccess
{
   [StructLayout( LayoutKind.Sequential )]
   public struct PROPVARIANT
   {
      public ushort vt;
      public ushort wReserved1;
      public ushort wReserved2;
      public ushort wReserved3;
      public IntPtr p;
      public int p2;

      public byte[] GetDataBytes()
      {
         var ret = new byte[IntPtr.Size + sizeof( int )];

         if ( IntPtr.Size == 4 )
         {
            BitConverter.GetBytes( this.p.ToInt32() ).CopyTo( ret, 0 );
         }
         else if ( IntPtr.Size == 8 )
         {
            BitConverter.GetBytes( this.p.ToInt64() ).CopyTo( ret, 0 );
         }

         BitConverter.GetBytes( this.p2 ).CopyTo( ret, IntPtr.Size );

         return ret;
      }

      public byte AsByte()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_UI1 )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return this.GetDataBytes()[0];
      }

      public sbyte AsSByte()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_I1 )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return (sbyte) this.GetDataBytes()[0];
      }

      public ushort AsUInt16()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_UI2 )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return BitConverter.ToUInt16( this.GetDataBytes(), 0 );
      }

      public short AsInt16()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_I2 )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return BitConverter.ToInt16( this.GetDataBytes(), 0 );
      }

      public uint AsUInt32()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_UI4 && ( (VarEnum) this.vt ) != VarEnum.VT_UINT )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return BitConverter.ToUInt32( this.GetDataBytes(), 0 );
      }

      public int AsInt32()
      {
         return this.AsInt32( true );
      }

      private int AsInt32( bool checkType )
      {
         if ( checkType )
         {
            if ( ( (VarEnum) this.vt ) != VarEnum.VT_I4 && ( (VarEnum) this.vt ) != VarEnum.VT_INT && ( (VarEnum) this.vt ) != VarEnum.VT_ERROR )
            {
               throw new InvalidCastException( "The value cannot be converted to the specified type." );
            }
         }

         return BitConverter.ToInt32( this.GetDataBytes(), 0 );
      }

      public ulong AsUInt64()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_UI8 )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return BitConverter.ToUInt64( this.GetDataBytes(), 0 );
      }

      public long AsInt64()
      {
         return this.AsInt64( true );
      }

      private long AsInt64( bool checkType )
      {
         if ( checkType )
         {
            if ( ( (VarEnum) this.vt ) != VarEnum.VT_I8 )
            {
               throw new InvalidCastException( "The value cannot be converted to the specified type." );
            }
         }

         return BitConverter.ToInt64( this.GetDataBytes(), 0 );
      }

      public float AsFloat()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_R4 )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return BitConverter.ToSingle( this.GetDataBytes(), 0 );
      }

      public double AsDouble()
      {
         return this.AsDouble( true );
      }

      private double AsDouble( bool checkType )
      {
         if ( checkType )
         {
            if ( ( (VarEnum) this.vt ) != VarEnum.VT_R8 )
            {
               throw new InvalidCastException( "The value cannot be converted to the specified type." );
            }
         }

         return BitConverter.ToDouble( this.GetDataBytes(), 0 );
      }

      public bool AsBool()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_BOOL )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return this.AsInt32( false ) == 0 ? false : true;
      }

      public DateTime AsDateTime()
      {
         if ( ( (VarEnum) this.vt ) == VarEnum.VT_DATE )
         {
            return DateTime.FromOADate( this.AsDouble( false ) );
         }
         else if ( ( (VarEnum) this.vt ) == VarEnum.VT_FILETIME )
         {
            return DateTime.FromFileTime( this.AsInt64( false ) );
         }

         throw new InvalidCastException( "The value cannot be converted to the specified type." );
      }

      public string AsString()
      {
         if ( ( (VarEnum) this.vt ) == VarEnum.VT_LPSTR )
         {
            return Marshal.PtrToStringAnsi( this.p );
         }
         else if ( ( (VarEnum) this.vt ) == VarEnum.VT_LPWSTR )
         {
            return Marshal.PtrToStringUni( this.p );
         }
         else if ( ( (VarEnum) this.vt ) == VarEnum.VT_BSTR )
         {
            return Marshal.PtrToStringBSTR( this.p );
         }

         throw new InvalidCastException( "The value cannot be converted to the specified type." );
      }

      public byte[] AsByteBuffer()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_BLOB )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         var blobData = new byte[this.AsInt32( false )];
         var pBlobData = IntPtr.Zero;

         if ( IntPtr.Size == 4 )
         {
            pBlobData = new IntPtr( this.p2 );
         }
         else if ( IntPtr.Size == 8 )
         {
            // In this case, we need to derive a pointer at offset 12,
            // because the size of the blob is represented as a 4-byte int
            // but the pointer is immediately after that.
            pBlobData = new IntPtr( BitConverter.ToInt64( this.GetDataBytes(), sizeof( int ) ) );
         }
         else
         {
            throw new NotSupportedException();
         }

         Marshal.Copy( pBlobData, blobData, 0, this.AsInt32( false ) );

         return blobData;
      }

      public object AsIUnknown()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_UNKNOWN )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return Marshal.GetObjectForIUnknown( this.p );
      }

      public IntPtr AsIntPtr()
      {
         return this.p;
      }

      public decimal AsDecimal()
      {
         if ( ( (VarEnum) this.vt ) != VarEnum.VT_CY )
         {
            throw new InvalidCastException( "The value cannot be converted to the specified type." );
         }

         return decimal.FromOACurrency( this.AsInt64() );
      }

      private Guid AsGuid( bool checkType )
      {
         if ( checkType )
         {
            if ( ( (VarEnum) this.vt ) != VarEnum.VT_CLSID )
            {
               throw new InvalidCastException( "The value cannot be converted to the specified type." );
            }
         }

         return (Guid) Marshal.PtrToStructure( this.p, typeof( Guid ) );
      }

      public Guid AsGuid()
      {
         return this.AsGuid( true );
      }

      public static tag_inner_PROPVARIANT Create_tag_inner_PROPVARIANT( string variantValue )
      {
         tag_inner_PROPVARIANT variant;
         var pValues = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
         pValues.SetStringValue( ref PortableDevicePKeys.WPD_OBJECT_ID, variantValue );
         pValues.GetValue( ref PortableDevicePKeys.WPD_OBJECT_ID, out variant );

         return variant;
      }

      public static tag_inner_PROPVARIANT Create_tag_inner_PROPVARIANT( UInt64 variantValue )
      {
         tag_inner_PROPVARIANT variant;
         var pValues = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
         pValues.SetUnsignedLargeIntegerValue( ref PortableDevicePKeys.WPD_OBJECT_SIZE, variantValue );
         pValues.GetValue( ref PortableDevicePKeys.WPD_OBJECT_SIZE, out variant );

         return variant;
      }

      public static tag_inner_PROPVARIANT Create_tag_inner_PROPVARIANT( bool variantValue )
      {
         tag_inner_PROPVARIANT variant;
         var pValues = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
         pValues.SetBoolValue( ref PortableDevicePKeys.WPD_OBJECT_ISHIDDEN, variantValue ? -1 : 0 );
         pValues.GetValue( ref PortableDevicePKeys.WPD_OBJECT_ISHIDDEN, out variant );

         return variant;
      }

      public static tag_inner_PROPVARIANT Create_tag_inner_PROPVARIANT( Guid variantValue )
      {
         tag_inner_PROPVARIANT variant;
         var pValues = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
         pValues.SetGuidValue( ref PortableDevicePKeys.WPD_OBJECT_FORMAT, ref variantValue );
         pValues.GetValue( ref PortableDevicePKeys.WPD_OBJECT_FORMAT, out variant );

         return variant;
      }

      public static tag_inner_PROPVARIANT Create_tag_inner_PROPVARIANT( UInt32 variantValue )
      {
         tag_inner_PROPVARIANT variant;
         var pValues = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
         pValues.SetUnsignedIntegerValue( ref PortableDevicePKeys.WPD_API_OPTION_IOCTL_ACCESS, variantValue );
         pValues.GetValue( ref PortableDevicePKeys.WPD_API_OPTION_IOCTL_ACCESS, out variant );

         return variant;
      }

      public static tag_inner_PROPVARIANT Create_tag_inner_PROPVARIANT( Int32 variantValue )
      {
         tag_inner_PROPVARIANT variant;
         var pValues = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
         pValues.SetSignedIntegerValue( ref PortableDevicePKeys.WPD_STILL_IMAGE_EXPOSURE_BIAS_COMPENSATION, variantValue );
         pValues.GetValue( ref PortableDevicePKeys.WPD_STILL_IMAGE_EXPOSURE_BIAS_COMPENSATION, out variant );

         return variant;
      }

      public static tag_inner_PROPVARIANT Create_tag_inner_PROPVARIANT( float variantValue )
      {
         tag_inner_PROPVARIANT variant;
         var pValues = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
         pValues.SetFloatValue( ref PortableDevicePKeys.WPD_AUDIO_CHANNEL_COUNT, variantValue );
         pValues.GetValue( ref PortableDevicePKeys.WPD_AUDIO_CHANNEL_COUNT, out variant );

         return variant;
      }
   }

   #region PropVariant

   //
   // Acknowledgements:
   // http://blogs.msdn.com/b/adamroot/archive/2008/04/11/interop-with-propvariants-in-net.aspx
   // https://onedrive.live.com/prev?cid=6f4c66b0ee56cd90&id=6F4C66B0EE56CD90%21197&v=TextFileEditor
   //

   /// <summary>
   /// Represents the OLE struct PROPVARIANT.
   /// </summary>
   /// <remarks>
   /// Must call Clear when finished to avoid memory leaks. If you get the value of
   /// a VT_UNKNOWN prop, an implicit AddRef is called, thus your reference will
   /// be active even after the PropVariant struct is cleared.
   /// </remarks>
   [StructLayout( LayoutKind.Sequential )]
   public struct PropVariant
   {
      #region Struct Fields

      // The layout of these elements needs to be maintained.
      //
      // NOTE: We could use LayoutKind.Explicit, but we want
      //       to maintain that the IntPtr may be 8 bytes on
      //       64-bit architectures, so we'll let the CLR keep
      //       us aligned.
      //
      // NOTE: In order to allow x64 compat, we need to allow for
      //       expansion of the IntPtr. However, the BLOB struct
      //       uses a 4-byte int, followed by an IntPtr, so
      //       although the p field catches most pointer values,
      //       we need an additional 4-bytes to get the BLOB
      //       pointer. The p2 field provides this, as well as
      //       the last 4-bytes of an 8-byte value on 32-bit
      //       architectures.

      // This is actually a VarEnum value, but the VarEnum type
      // shifts the layout of the struct by 4 bytes instead of the
      // expected 2.
      private ushort vt;
      private ushort wReserved1;
      private ushort wReserved2;
      private ushort wReserved3;
      private IntPtr p;
      private int p2;

      #endregion

      //private object value;

      #region Union Members

      private sbyte cVal // CHAR cVal;
      {
         get { return (sbyte) this.GetDataBytes()[0]; }
      }

      private byte bVal // UCHAR bVal;
      {
         get { return this.GetDataBytes()[0]; }
      }

      private short iVal // SHORT iVal;
      {
         get { return BitConverter.ToInt16( this.GetDataBytes(), 0 ); }
      }

      private ushort uiVal // USHORT uiVal;
      {
         get { return BitConverter.ToUInt16( this.GetDataBytes(), 0 ); }
      }

      private int lVal // LONG lVal;
      {
         get { return BitConverter.ToInt32( this.GetDataBytes(), 0 ); }
      }

      private uint ulVal // ULONG ulVal;
      {
         get { return BitConverter.ToUInt32( this.GetDataBytes(), 0 ); }
      }

      private long hVal // LARGE_INTEGER hVal;
      {
         get { return BitConverter.ToInt64( this.GetDataBytes(), 0 ); }
      }

      private ulong uhVal // ULARGE_INTEGER uhVal;
      {
         get { return BitConverter.ToUInt64( this.GetDataBytes(), 0 ); }
      }

      private float fltVal // FLOAT fltVal;
      {
         get { return BitConverter.ToSingle( this.GetDataBytes(), 0 ); }
      }

      private double dblVal // DOUBLE dblVal;
      {
         get { return BitConverter.ToDouble( this.GetDataBytes(), 0 ); }
      }

      private bool boolVal // VARIANT_BOOL boolVal;
      {
         get { return ( this.iVal == 0 ? false : true ); }
      }

      private int scode // SCODE scode;
      {
         get { return this.lVal; }
      }

      private decimal cyVal // CY cyVal;
      {
         get { return decimal.FromOACurrency( this.hVal ); }
      }

      private DateTime date // DATE date;
      {
         get { return DateTime.FromOADate( this.dblVal ); }
      }

      #endregion

      /// <summary>
      /// Gets a byte array containing the data bits of the struct.
      /// </summary>
      /// <returns>A byte array that is the combined size of the data bits.</returns>
      private byte[] GetDataBytes()
      {
         var ret = new byte[IntPtr.Size + sizeof( int )];

         if ( IntPtr.Size == 4 )
         {
            BitConverter.GetBytes( this.p.ToInt32() ).CopyTo( ret, 0 );
         }
         else if ( IntPtr.Size == 8 )
         {
            BitConverter.GetBytes( this.p.ToInt64() ).CopyTo( ret, 0 );
         }

         BitConverter.GetBytes( this.p2 ).CopyTo( ret, IntPtr.Size );

         return ret;
      }

      /// <summary>
      /// Called to properly clean up the memory referenced by a PropVariant instance.
      /// </summary>
      [DllImport( "ole32.dll" )]
      private extern static int PropVariantClear( ref PropVariant pvar );

      /// <summary>
      /// Called to clear the PropVariant's referenced and local memory.
      /// </summary>
      /// <remarks>
      /// You must call Clear to avoid memory leaks.
      /// </remarks>
      public void Clear()
      {
         // Can't pass "this" by ref, so make a copy to call PropVariantClear with
         PropVariant var = this;
         PropVariantClear( ref var );

         // Since we couldn't pass "this" by ref, we need to clear the member fields manually
         // NOTE: PropVariantClear already freed heap data for us, so we are just setting
         //       our references to null.
         this.vt = (ushort) VarEnum.VT_EMPTY;
         this.wReserved1 = this.wReserved2 = this.wReserved3 = 0;
         this.p = IntPtr.Zero;
         this.p2 = 0;
      }

      /// <summary>
      /// Gets the variant type.
      /// </summary>
      public VarEnum Type
      {
         get { return (VarEnum) vt; }
      }

      /// <summary>
      /// Gets the variant value.
      /// </summary>
      public object Value
      {
         get
         {
            // TODO: Add support for reference types (ie. VT_REF | VT_I1)
            // TODO: Add support for safe arrays

            switch ( (VarEnum) this.vt )
            {
               case VarEnum.VT_I1:
                  return this.cVal;
               case VarEnum.VT_UI1:
                  return this.bVal;
               case VarEnum.VT_I2:
                  return this.iVal;
               case VarEnum.VT_UI2:
                  return this.uiVal;
               case VarEnum.VT_I4:
               case VarEnum.VT_INT:
                  return this.lVal;
               case VarEnum.VT_UI4:
               case VarEnum.VT_UINT:
                  return this.ulVal;
               case VarEnum.VT_I8:
                  return this.hVal;
               case VarEnum.VT_UI8:
                  return this.uhVal;
               case VarEnum.VT_R4:
                  return this.fltVal;
               case VarEnum.VT_R8:
                  return this.dblVal;
               case VarEnum.VT_BOOL:
                  return this.boolVal;
               case VarEnum.VT_ERROR:
                  return this.scode;
               case VarEnum.VT_CY:
                  return this.cyVal;
               case VarEnum.VT_DATE:
                  return this.date;
               case VarEnum.VT_FILETIME:
                  return DateTime.FromFileTime( this.hVal );
               case VarEnum.VT_BSTR:
                  return Marshal.PtrToStringBSTR( this.p );
               case VarEnum.VT_BLOB:
                  var blobData = new byte[this.lVal];
                  var pBlobData = IntPtr.Zero;
                  if ( IntPtr.Size == 4 )
                  {
                     pBlobData = new IntPtr( this.p2 );
                  }
                  else if ( IntPtr.Size == 8 )
                  {
                     // In this case, we need to derive a pointer at offset 12,
                     // because the size of the blob is represented as a 4-byte int
                     // but the pointer is immediately after that.
                     pBlobData = new IntPtr( BitConverter.ToInt64( this.GetDataBytes(), sizeof( int ) ) );
                  }
                  else
                  {
                     throw new NotSupportedException();
                  }
                  Marshal.Copy( pBlobData, blobData, 0, this.lVal );
                  return blobData;
               case VarEnum.VT_LPSTR:
                  return Marshal.PtrToStringAnsi( this.p );
               case VarEnum.VT_LPWSTR:
                  return Marshal.PtrToStringUni( this.p );
               case VarEnum.VT_UNKNOWN:
                  return Marshal.GetObjectForIUnknown( this.p );
               case VarEnum.VT_DISPATCH:
                  return this.p;
               default:
                  throw new NotSupportedException( "The type of this variable is not supported ('" + this.vt.ToString() + "')." );
            }
         }
      }
   }

   #endregion
}
