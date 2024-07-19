// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing;

public partial class ImageConverter
{
    // As defined by Access
    [StructLayout(LayoutKind.Sequential)]
    private struct OLEOBJHEADER
    {
        public const ushort OLEOBJID = 0x1c15; // ^L^E

        public ushort typ;          // Type signature (Always OLEOBJID)
        public ushort cbHdr;        // Size of header
        public OleObjectType oot;   // OLE object type code
        public ushort cchName;      // Count of characters in object name
        public ushort cchClass;     // Count of characters in class name
        public ushort ibName;       // Offset of object name in structure
        public ushort ibClass;      // Offset of class name in structure
        public POINTS ptSize;       // Original size of object (MM_HITMETRIC) (-1, -1 to express Access 2.0 and later)
    }
}
