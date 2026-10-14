// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Private.Windows.Ole;

internal static unsafe partial class EmfNativeMethods
{
    internal static nint CreateDeterministicEnhMetafile()
    {
        // A single rectangle produces a small, deterministic EMF with a nontrivial drawing record.
        nint recordingDc = CreateEnhMetaFile(
            hdcRef: 0,
            fileName: null,
            rectangle: 0,
            description: null);
        recordingDc.Should().NotBe(0);

        nint metafile = 0;
        bool success = false;
        try
        {
            Rectangle(recordingDc, 10, 10, 100, 100).Should().NotBe(0);
            metafile = CloseEnhMetaFile(recordingDc);
            recordingDc = 0;
            metafile.Should().NotBe(0);
            GetBits(metafile).Length.Should().BeGreaterThanOrEqualTo(88);
            success = true;

            return metafile;
        }
        finally
        {
            if (recordingDc != 0)
            {
                // Closing an unfinished recording still returns an owned metafile handle that must be deleted.
                nint unfinishedMetafile = CloseEnhMetaFile(recordingDc);
                if (unfinishedMetafile != 0)
                {
                    DeleteEnhMetaFile(unfinishedMetafile);
                }
            }

            if (metafile != 0 && !success)
            {
                DeleteEnhMetaFile(metafile);
            }
        }
    }

    internal static byte[] GetBits(nint handle)
    {
        uint size = GetEnhMetaFileBits(handle, 0, null);
        if (size == 0)
        {
            return [];
        }

        byte[] bits = GC.AllocateUninitializedArray<byte>(checked((int)size));
        fixed (byte* buffer = bits)
        {
            GetEnhMetaFileBits(handle, size, buffer).Should().Be(size);
        }

        return bits;
    }

    internal static bool IsValid(nint handle) => handle != 0 && GetEnhMetaFileBits(handle, 0, null) != 0;

    internal static bool Delete(nint handle) => DeleteEnhMetaFile(handle) != 0;

    [LibraryImport(Libraries.Gdi32, EntryPoint = "CreateEnhMetaFileW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateEnhMetaFile(
        nint hdcRef,
        string? fileName,
        nint rectangle,
        string? description);

    [LibraryImport(Libraries.Gdi32)]
    private static partial nint CloseEnhMetaFile(nint hdc);

    [LibraryImport(Libraries.Gdi32)]
    private static partial int Rectangle(nint hdc, int left, int top, int right, int bottom);

    [LibraryImport(Libraries.Gdi32)]
    private static partial uint GetEnhMetaFileBits(nint hemf, uint bufferSize, byte* buffer);

    [LibraryImport(Libraries.Gdi32)]
    private static partial int DeleteEnhMetaFile(nint hemf);
}
