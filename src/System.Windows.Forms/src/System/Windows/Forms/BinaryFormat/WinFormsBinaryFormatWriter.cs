﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Writer that writes Windows Forms specific types in binary format without using the BinaryFormatter.
/// </summary>
internal static class WinFormsBinaryFormatWriter
{
    private static readonly string[] s_dataMemberName = ["Data"];

    private static readonly string s_currentWinFormsFullName = typeof(WinFormsBinaryFormatWriter).Assembly.FullName!;

    public static unsafe void WriteBitmap(Stream stream, Bitmap bitmap)
    {
        using MemoryStream memoryStream = new();
        bitmap.Save(memoryStream);

        bool success = memoryStream.TryGetBuffer(out ArraySegment<byte> data);
        Debug.Assert(success);

        using BinaryFormatWriterScope writer = new(stream);
        new BinaryLibrary(2, AssemblyRef.SystemDrawing).Write(writer);
        new ClassWithMembersAndTypes(
            new ClassInfo(1, typeof(Bitmap).FullName!, s_dataMemberName),
            libraryId: 2,
            new MemberTypeInfo((BinaryType.PrimitiveArray, PrimitiveType.Byte)),
            new MemberReference(3)).Write(writer);

        new ArraySinglePrimitive<byte>(3, data).Write(writer);
    }

    public static void WriteImageListStreamer(Stream stream, ImageListStreamer streamer)
    {
        byte[] data = streamer.Serialize();

        using BinaryFormatWriterScope writer = new(stream);

        new BinaryLibrary(2, s_currentWinFormsFullName).Write(writer);
        new ClassWithMembersAndTypes(
            new ClassInfo(1, typeof(ImageListStreamer).FullName!, s_dataMemberName),
            libraryId: 2,
            new MemberTypeInfo((BinaryType.PrimitiveArray, PrimitiveType.Byte)),
            new MemberReference(3)).Write(writer);

        new ArraySinglePrimitive<byte>(3, data).Write(writer);
    }

    /// <summary>
    ///  Writes the given <paramref name="value"/> if supported.
    /// </summary>
    public static bool TryWriteObject(Stream stream, object value)
    {
        // Framework types are more likely to be written, so check them first.
        return BinaryFormatWriter.TryWriteFrameworkObject(stream, value)
            || BinaryFormatWriter.TryWrite(Write, stream, value);

        static bool Write(Stream stream, object value)
        {
            if (value is ImageListStreamer streamer)
            {
                WriteImageListStreamer(stream, streamer);
                return true;
            }
            else if (value is Bitmap bitmap)
            {
                WriteBitmap(stream, bitmap);
                return true;
            }

            return false;
        }
    }
}
