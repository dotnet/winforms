// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Memory;

namespace System.IO;

internal static class StreamExtensions
{
    /// <summary>
    ///  Get a <see cref="IStream"/> wrapper around the given <paramref name="stream"/>. Use the return value
    ///  in a <see langword="using"/> scope.
    /// </summary>
    internal static ComScope<IStream> ToIStream(this Stream stream, bool makeSeekable = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return ComHelpers.GetComScope<IStream>(new ComManagedStream(stream, makeSeekable));
    }

    /// <summary>
    ///  Save the contents of the <paramref name="stream"/> to a new <see cref="HGLOBAL"/>.
    /// </summary>
    /// <param name="stream">The stream to save to the <see cref="HGLOBAL"/>.</param>
    /// <param name="hglobal">The memory handle if successful. If a non null value is passed in, it will be freed.</param>
    /// <returns><see cref="HRESULT.S_OK"/> if successful.</returns>
    internal static unsafe HRESULT SaveStreamToHGLOBAL(this Stream stream, ref HGLOBAL hglobal)
    {
        if (!hglobal.IsNull)
        {
            HGLOBAL freed = PInvokeCore.GlobalFree(hglobal);
            if (!freed.IsNull)
            {
                return HRESULT.E_OUTOFMEMORY;
            }
        }

        int size = checked((int)stream.Length);
        hglobal = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE, (uint)size);
        if (hglobal.IsNull)
        {
            return HRESULT.E_OUTOFMEMORY;
        }

        void* buffer = PInvokeCore.GlobalLock(hglobal);
        if (buffer is null)
        {
            return HRESULT.E_OUTOFMEMORY;
        }

        try
        {
            Span<byte> span = new(buffer, size);
            stream.Position = 0;
            stream.ReadExactly(span);
        }
        finally
        {
            PInvokeCore.GlobalUnlock(hglobal);
        }

        return HRESULT.S_OK;
    }
}
