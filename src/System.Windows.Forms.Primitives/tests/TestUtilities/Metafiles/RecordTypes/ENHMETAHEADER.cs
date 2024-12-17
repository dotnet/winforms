// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct ENHMETAHEADER
{
    public uint iType;              // Record typeEMR_HEADER
    public uint nSize;              // Record size in bytes. This may be greater
                                    // than the sizeof(ENHMETAHEADER).
    public RECT rclBounds;          // Inclusive-inclusive bounds in device units
    public RECT rclFrame;           // Inclusive-inclusive Picture Frame of metafile in .01 mm units
    public uint dSignature;         // Signature. Must be ENHMETA_SIGNATURE.
    public uint nVersion;           // Version number
    public uint nBytes;             // Size of the metafile in bytes
    public uint nRecords;           // Number of records in the metafile
    public ushort nHandles;         // Number of handles in the handle table
                                    // Handle index zero is reserved.
    public ushort sReserved;        // Reserved. Must be zero.
    public uint nDescription;       // Number of chars in the unicode description string
                                    // This is 0 if there is no description string
    public uint offDescription;     // Offset to the metafile description record.
                                    // This is 0 if there is no description string
    public uint nPalEntries;        // Number of entries in the metafile palette.
    public Size szlDevice;          // Size of the reference device in pixels
    public Size szlMillimeters;     // Size of the reference device in millimeters

    // The next three bring the size up to 100 bytes
    public uint cbPixelFormat;      // Size of PIXELFORMATDESCRIPTOR information
                                    // This is 0 if no pixel format is set
    public uint offPixelFormat;     // Offset to PIXELFORMATDESCRIPTOR
                                    // This is 0 if no pixel format is set
    public BOOL bOpenGL;            // TRUE if OpenGL commands are present in
                                    // the metafile, otherwise FALSE

    // The next field brings the size to 108 bytes
    public Size szlMicrometers;     // Size of the reference device in micrometers

    public override readonly string ToString() => $@"[{nameof(ENHMETAHEADER)}] Bounds: {rclBounds} Device Size: {szlDevice}Header Size: {nSize}";
}
