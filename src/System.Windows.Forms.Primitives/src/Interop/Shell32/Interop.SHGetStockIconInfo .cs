// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        public enum SHSTOCKICONID
        {
            DOCNOASSOC = 0,
            DOCASSOC = 1,
            APPLICATION = 2,
            FOLDER = 3,
            FOLDEROPEN = 4,
            DRIVE525 = 5,
            DRIVE35 = 6,
            DRIVEREMOVE = 7,
            DRIVEFIXED = 8,
            DRIVENET = 9,
            DRIVENETDISABLED = 10,
            DRIVECD = 11,
            DRIVERAM = 12,
            WORLD = 13,
            SERVER = 15,
            PRINTER = 16,
            MYNETWORK = 17,
            FIND = 22,
            HELP = 23,
            SHARE = 28,
            LINK = 29,
            SLOWFILE = 30,
            RECYCLER = 31,
            RECYCLERFULL = 32,
            MEDIACDAUDIO = 40,
            LOCK = 47,
            AUTOLIST = 49,
            PRINTERNET = 50,
            SERVERSHARE = 51,
            PRINTERFAX = 52,
            PRINTERFAXNET = 53,
            PRINTERFILE = 54,
            STACK = 55,
            MEDIASVCD = 56,
            STUFFEDFOLDER = 57,
            DRIVEUNKNOWN = 58,
            DRIVEDVD = 59,
            MEDIADVD = 60,
            MEDIADVDRAM = 61,
            MEDIADVDRW = 62,
            MEDIADVDR = 63,
            MEDIADVDROM = 64,
            MEDIACDAUDIOPLUS = 65,
            MEDIACDRW = 66,
            MEDIACDR = 67,
            MEDIACDBURN = 68,
            MEDIABLANKCD = 69,
            MEDIACDROM = 70,
            AUDIOFILES = 71,
            IMAGEFILES = 72,
            VIDEOFILES = 73,
            MIXEDFILES = 74,
            FOLDERBACK = 75,
            FOLDERFRONT = 76,
            SHIELD = 77,
            WARNING = 78,
            INFO = 79,
            ERROR = 80,
            KEY = 81,
            SOFTWARE = 82,
            RENAME = 83,
            DELETE = 84,
            MEDIAAUDIODVD = 85,
            MEDIAMOVIEDVD = 86,
            MEDIAENHANCEDCD = 87,
            MEDIAENHANCEDDVD = 88,
            MEDIAHDDVD = 89,
            MEDIABLURAY = 90,
            MEDIAVCD = 91,
            MEDIADVDPLUSR = 92,
            MEDIADVDPLUSRW = 93,
            DESKTOPPC = 94,
            MOBILEPC = 95,
            USERS = 96,
            MEDIASMARTMEDIA = 97,
            MEDIACOMPACTFLASH = 98,
            DEVICECELLPHONE = 99,
            DEVICECAMERA = 100,
            DEVICEVIDEOCAMERA = 101,
            DEVICEAUDIOPLAYER = 102,
            NETWORKCONNECT = 103,
            INTERNET = 104,
            ZIPFILE = 105,
            SETTINGS = 106,
            DRIVEHDDVD = 132,
            DRIVEBD = 133,
            MEDIAHDDVDROM = 134,
            MEDIAHDDVDR = 135,
            MEDIAHDDVDRAM = 136,
            MEDIABDROM = 137,
            MEDIABDR = 138,
            MEDIABDRE = 139,
            CLUSTEREDDRIVE = 140,
            MAX_ICONS = 181
        }

        public enum SHGSI
        {
            ICONLOCATION = 0,
            ICON = 0x000000100,
            SYSICONINDEX = 0x000004000,
            LINKOVERLAY = 0x000008000,
            SELECTED = 0x000010000,
#pragma warning disable CA1069 // Enums values should not be duplicated
            LARGEICON = 0x000000000,
#pragma warning restore CA1069 // Enums values should not be duplicated
            SMALLICON = 0x000000001,
            SHELLICONSIZE = 0x000000004

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysImageIndex;
            public int iIcon;
            private fixed char _szPath[Kernel32.MAX_PATH];

            private ReadOnlySpan<char> szPath
            {
                get { fixed (char* c = _szPath) { return new ReadOnlySpan<char>(c, Kernel32.MAX_PATH); } }
            }

            public ReadOnlySpan<char> Path
            {
                get { return szPath.SliceAtFirstNull(); }
            }
        }

        [DllImport(Libraries.Shell32, ExactSpelling = true)]
        public static unsafe extern HRESULT SHGetStockIconInfo(SHSTOCKICONID siid,
                                                               SHGSI uFlags,
                                                               SHSTOCKICONINFO* psii);
    }
}
