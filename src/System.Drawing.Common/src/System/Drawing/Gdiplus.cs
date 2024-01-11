// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Drawing;

internal static partial class SafeNativeMethods
{
    // We make this a nested class so that we don't have to initialize GDI+ to access SafeNativeMethods (mostly gdi/user32).
    internal static partial class Gdip
    {
        private static readonly nuint s_initToken = Init();

        [ThreadStatic]
        private static IDictionary<object, object>? t_threadData;

        private static unsafe nuint Init()
        {
            if (!OperatingSystem.IsWindows())
            {
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (_, _, _) =>
                    throw new PlatformNotSupportedException(SR.PlatformNotSupported_Unix));
            }

            Debug.Assert(s_initToken == 0, "GdiplusInitialization: Initialize should not be called more than once in the same domain!");

            // GDI+ ref counts multiple calls to Startup in the same process, so calls from multiple
            // domains are ok, just make sure to pair each w/GdiplusShutdown

            nuint token;
            GdiplusStartupInputEx startup = GdiplusStartupInputEx.GetDefault();
            Status status = PInvoke.GdiplusStartup(&token, (GdiplusStartupInput*)&startup, null);
            CheckStatus(status);
            return token;
        }

        /// <summary>
        /// Returns true if GDI+ has been started, but not shut down
        /// </summary>
        internal static bool Initialized => s_initToken != 0;

        /// <summary>
        /// This property will give us back a dictionary we can use to store all of our static brushes and pens on
        /// a per-thread basis. This way we can avoid 'object in use' crashes when different threads are
        /// referencing the same drawing object.
        /// </summary>
        internal static IDictionary<object, object> ThreadData => t_threadData ??= new Dictionary<object, object>();

        // Used to ensure static constructor has run.
        internal static void DummyFunction()
        {
        }

        //----------------------------------------------------------------------------------------
        // Status codes
        //----------------------------------------------------------------------------------------
        internal const int Ok = 0;
        internal const int GenericError = 1;
        internal const int InvalidParameter = 2;
        internal const int OutOfMemory = 3;
        internal const int ObjectBusy = 4;
        internal const int InsufficientBuffer = 5;
        internal const int NotImplemented = 6;
        internal const int Win32Error = 7;
        internal const int WrongState = 8;
        internal const int Aborted = 9;
        internal const int FileNotFound = 10;
        internal const int ValueOverflow = 11;
        internal const int AccessDenied = 12;
        internal const int UnknownImageFormat = 13;
        internal const int FontFamilyNotFound = 14;
        internal const int FontStyleNotFound = 15;
        internal const int NotTrueTypeFont = 16;
        internal const int UnsupportedGdiplusVersion = 17;
        internal const int GdiplusNotInitialized = 18;
        internal const int PropertyNotFound = 19;
        internal const int PropertyNotSupported = 20;

        internal static void CheckStatus(int status) => CheckStatus((Status)status);

        internal static Exception StatusException(int status) => StatusException((Status)status);

        internal static void CheckStatus(Status status) => status.ThrowIfFailed();

        internal static Exception StatusException(Status status) => status.GetException();
    }

    public const int
    DM_IN_BUFFER = 8,
    DM_OUT_BUFFER = 2,
    PD_ALLPAGES = 0x00000000,
    PD_SELECTION = 0x00000001,
    PD_PAGENUMS = 0x00000002,
    PD_CURRENTPAGE = 0x00400000,
    PD_RETURNDEFAULT = 0x00000400,
    DI_NORMAL = 0x0003,
    IMAGE_ICON = 1,
    IDI_APPLICATION = 32512,
    IDI_HAND = 32513,
    IDI_QUESTION = 32514,
    IDI_EXCLAMATION = 32515,
    IDI_ASTERISK = 32516,
    IDI_WINLOGO = 32517,
    IDI_WARNING = 32515,
    IDI_ERROR = 32513,
    IDI_INFORMATION = 32516,
    DEFAULT_CHARSET = 1;

    public const int ERROR_ACCESS_DENIED = 5;
    public const int ERROR_INVALID_PARAMETER = 87;
    public const int ERROR_PROC_NOT_FOUND = 127;
    public const int ERROR_CANCELLED = 1223;

    [StructLayout(LayoutKind.Sequential)]
    public struct ENHMETAHEADER
    {
        /// The ENHMETAHEADER structure is defined natively as a union with WmfHeader.
        /// Extreme care should be taken if changing the layout of the corresponding managed
        /// structures to minimize the risk of buffer overruns.  The affected managed classes
        /// are the following: ENHMETAHEADER, MetaHeader, MetafileHeaderWmf, MetafileHeaderEmf.
        public int iType;
        public int nSize;
        // rclBounds was a by-value RECTL structure
        public int rclBounds_left;
        public int rclBounds_top;
        public int rclBounds_right;
        public int rclBounds_bottom;
        // rclFrame was a by-value RECTL structure
        public int rclFrame_left;
        public int rclFrame_top;
        public int rclFrame_right;
        public int rclFrame_bottom;
        public int dSignature;
        public int nVersion;
        public int nBytes;
        public int nRecords;
        public short nHandles;
        public short sReserved;
        public int nDescription;
        public int offDescription;
        public int nPalEntries;
        // szlDevice was a by-value SIZE structure
        public int szlDevice_cx;
        public int szlDevice_cy;
        // szlMillimeters was a by-value SIZE structure
        public int szlMillimeters_cx;
        public int szlMillimeters_cy;
        public int cbPixelFormat;
        public int offPixelFormat;
        public int bOpenGL;
    }

    // https://devblogs.microsoft.com/oldnewthing/20101018-00/?p=12513
    // https://devblogs.microsoft.com/oldnewthing/20120720-00/?p=7083

    // Needs to be packed to 2 to get ICONDIRENTRY to follow immediately after idCount.
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct ICONDIR
    {
        // Must be 0
        public ushort idReserved;
        // Must be 1
        public ushort idType;
        // Count of entries
        public ushort idCount;
        // First entry (anysize array)
        public ICONDIRENTRY idEntries;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ICONDIRENTRY
    {
        // Width and height are 1 - 255 or 0 for 256
        public byte bWidth;
        public byte bHeight;
        public byte bColorCount;
        public byte bReserved;
        public ushort wPlanes;
        public ushort wBitCount;
        public uint dwBytesInRes;
        public uint dwImageOffset;
    }
}
