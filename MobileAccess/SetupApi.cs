using System;
using System.Runtime.InteropServices;

namespace MobileAccess
{
   public class SetupApi
   {
      // P/Invoke: http://pinvoke.net/default.aspx/setupapi/SetupDiGetClassDevs.html

      public const int DIGCF_DEFAULT = 0x1;
      public const int DIGCF_PRESENT = 0x2;
      public const int DIGCF_ALLCLASSES = 0x4;
      public const int DIGCF_PROFILE = 0x8;
      public const int DIGCF_DEVICEINTERFACE = 0x10;
      public static readonly Guid GUID_DEVINTERFACE_DISK = new Guid( 0x53f56307, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x00, 0xa0, 0xc9, 0x1e, 0xfb, 0x8b );
      public const uint SP_DEVICE_INTERFACE_DETAIL_DATA_BUFFER_SIZE = 256;

      [Flags]
      public enum DiGetClassFlags : uint
      {
         DIGCF_DEFAULT = 0x00000001,  // only valid with DIGCF_DEVICEINTERFACE
         DIGCF_PRESENT = 0x00000002,
         DIGCF_ALLCLASSES = 0x00000004,
         DIGCF_PROFILE = 0x00000008,
         DIGCF_DEVICEINTERFACE = 0x00000010,
      }

      [Flags]
      public enum DeviceInterfaceDataFlags : uint
      {
         Active = 0x00000001,
         Default = 0x00000002,
         Removed = 0x00000004
      }

      [StructLayout( LayoutKind.Sequential )]
      public struct SP_DEVICE_INTERFACE_DATA
      {
         public Int32 cbSize; // Size of structure, in bytes.  Use Marshal.SizeOf()
         public Guid interfaceClassGuid;
         public DeviceInterfaceDataFlags flags;
         private UIntPtr reserved; // Do not use.
      }

      [StructLayout( LayoutKind.Sequential )]
      public struct SP_DEVINFO_DATA
      {
         public Int32 cbSize;  // Size of structure, in bytes.  Use Marshal.SizeOf()
         public Guid classGuid;
         public uint devInst;
         public IntPtr reserved;
      }

      [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
      public struct SP_DEVICE_INTERFACE_DETAIL_DATA
      {
         public int cbSize; // Size of structure, in bytes.  Use Marshal.SizeOf()
         [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int) SP_DEVICE_INTERFACE_DETAIL_DATA_BUFFER_SIZE )]
         public string DevicePath;
      }

      // Cannot pass "ClassGuid" as null.
      [DllImport( "setupapi.dll", CharSet = CharSet.Auto )]
      public static extern IntPtr SetupDiGetClassDevs(
         ref Guid ClassGuid,
         [MarshalAs( UnmanagedType.LPTStr )] string Enumerator,
         IntPtr hwndParent,
         DiGetClassFlags Flags
      );

      // Uses a ClassGuid only, with IntPtr.Zero (for C++ NULL) enumerator.
      [DllImport( "setupapi.dll", CharSet = CharSet.Auto )]
      public static extern IntPtr SetupDiGetClassDevs(
         ref Guid ClassGuid,
         IntPtr Enumerator,
         IntPtr hwndParent,
         DiGetClassFlags Flags
      );

      // Uses an Enumerator only, with IntPtr.Zero (for C++ NULL) ClassGuid.
      [DllImport( "setupapi.dll", CharSet = CharSet.Auto )]
      public static extern IntPtr SetupDiGetClassDevs(
         IntPtr ClassGuid,
         string Enumerator,
         IntPtr hwndParent,
         DiGetClassFlags Flags
      );

      // Uses an IntPtr.Zero (for C++ NULL) ClassGuid and Enumerator.
      [DllImport( "setupapi.dll", CharSet = CharSet.Auto )]
      public static extern IntPtr SetupDiGetClassDevs(
         IntPtr ClassGuid,
         IntPtr Enumerator,
         IntPtr hwndParent,
         DiGetClassFlags Flags
      );

      [DllImport( "setupapi.dll", SetLastError = true )]
      public static extern bool SetupDiDestroyDeviceInfoList( IntPtr DeviceInfoSet );

      [DllImport( "setupapi.dll", CharSet = CharSet.Auto, SetLastError = true )]
      public static extern Boolean SetupDiEnumDeviceInterfaces(
         IntPtr hDevInfo,
         ref SP_DEVINFO_DATA devInfo,
         ref Guid interfaceClassGuid,
         UInt32 memberIndex,
         ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
      );

      // Allows passing IntPtr.Zero (for C++ NULL) for devInfo.
      [DllImport( "setupapi.dll", CharSet = CharSet.Auto, SetLastError = true )]
      public static extern Boolean SetupDiEnumDeviceInterfaces(
         IntPtr hDevInfo,
         IntPtr devInfo,
         ref Guid interfaceClassGuid,
         UInt32 memberIndex,
         ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
      );

      [DllImport( "setupapi.dll", CharSet = CharSet.Auto, SetLastError = true )]
      public static extern Boolean SetupDiGetDeviceInterfaceDetail(
         IntPtr hDevInfo,
         ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
         ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
         UInt32 deviceInterfaceDetailDataSize,
         out UInt32 requiredSize,
         ref SP_DEVINFO_DATA deviceInfoData
      );

      [DllImport( "setupapi.dll" )]
      public static extern int CM_Get_Parent(
         out UInt32 pdnDevInst,
         UInt32 dnDevInst,  // DWORD, not pointer.
         int ulFlags
      );

      [DllImport( "setupapi.dll", CharSet = CharSet.Auto )]
      public static extern int CM_Get_Device_ID(
         UInt32 dnDevInst,
         IntPtr buffer,
         int bufferLen, // Call CM_Get_Device_ID_Size() and add to allow room for the terminating NULL.
         int flags // Must be zero.
      );

      [DllImport( "setupapi.dll", SetLastError = true )]
      public static extern int CM_Get_Device_ID_Size( ref int pulLen, int dnDevInst, int ulFlags );
   }
}
