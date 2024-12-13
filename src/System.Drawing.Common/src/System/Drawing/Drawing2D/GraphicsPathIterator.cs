// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public sealed unsafe class GraphicsPathIterator : MarshalByRefObject, IDisposable
{
    // handle to native path iterator object
    internal GpPathIterator* _nativeIterator;

    public GraphicsPathIterator(GraphicsPath? path)
    {
        GpPathIterator* iterator;
        PInvokeGdiPlus.GdipCreatePathIter(&iterator, path.Pointer()).ThrowIfFailed();
        GC.KeepAlive(path);
        _nativeIterator = iterator;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_nativeIterator is not null)
        {
            try
            {
#if DEBUG
                Status status = !Gdip.Initialized ? Status.Ok :
#endif
                PInvokeGdiPlus.GdipDeletePathIter(_nativeIterator);
#if DEBUG
                Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
#endif
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }

                Debug.Fail($"Exception thrown during Dispose: {ex}");
            }
            finally
            {
                _nativeIterator = null;
            }
        }
    }

    ~GraphicsPathIterator() => Dispose(false);

    public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
    {
        int resultCount;
        BOOL tempIsClosed;

        fixed (int* s = &startIndex, e = &endIndex)
        {
            PInvokeGdiPlus.GdipPathIterNextSubpath(_nativeIterator, &resultCount, s, e, &tempIsClosed).ThrowIfFailed();
            isClosed = tempIsClosed;
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public int NextSubpath(GraphicsPath path, out bool isClosed)
    {
        int resultCount;
        BOOL tempIsClosed;
        PInvokeGdiPlus.GdipPathIterNextSubpathPath(_nativeIterator, &resultCount, path.Pointer(), &tempIsClosed).ThrowIfFailed();
        isClosed = tempIsClosed;
        GC.KeepAlive(this);
        return resultCount;
    }

    public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
    {
        int resultCount;

        fixed (byte* pt = &pathType)
        fixed (int* s = &startIndex, e = &endIndex)
        {
            PInvokeGdiPlus.GdipPathIterNextPathType(_nativeIterator, &resultCount, pt, s, e).ThrowIfFailed();
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public int NextMarker(out int startIndex, out int endIndex)
    {
        int resultCount;

        fixed (int* s = &startIndex, e = &endIndex)
        {
            PInvokeGdiPlus.GdipPathIterNextMarker(_nativeIterator, &resultCount, s, e).ThrowIfFailed();
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public int NextMarker(GraphicsPath path)
    {
        int resultCount;
        PInvokeGdiPlus.GdipPathIterNextMarkerPath(_nativeIterator, &resultCount, path.Pointer()).ThrowIfFailed();
        GC.KeepAlive(this);
        return resultCount;
    }

    public int Count
    {
        get
        {
            int resultCount;
            PInvokeGdiPlus.GdipPathIterGetCount(_nativeIterator, &resultCount).ThrowIfFailed();
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public int SubpathCount
    {
        get
        {
            int resultCount;
            PInvokeGdiPlus.GdipPathIterGetSubpathCount(_nativeIterator, &resultCount).ThrowIfFailed();
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public bool HasCurve()
    {
        BOOL hasCurve;
        PInvokeGdiPlus.GdipPathIterHasCurve(_nativeIterator, &hasCurve).ThrowIfFailed();
        GC.KeepAlive(this);
        return hasCurve;
    }

    public void Rewind()
    {
        PInvokeGdiPlus.GdipPathIterRewind(_nativeIterator).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <inheritdoc cref="CopyData(ref PointF[], ref byte[], int, int)"/>
    public unsafe int Enumerate(ref PointF[] points, ref byte[] types)
        => Enumerate(points.OrThrowIfNull().AsSpan(), types.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="CopyData(ref PointF[], ref byte[], int, int)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    unsafe int Enumerate(Span<PointF> points, Span<byte> types)
    {
        if (points.Length != types.Length
            || points.Length < Count)
        {
            throw Status.InvalidParameter.GetException();
        }

        if (points.Length == 0)
        {
            return 0;
        }

        fixed (PointF* p = points)
        fixed (byte* t = types)
        {
            int resultCount;
            PInvokeGdiPlus.GdipPathIterEnumerate(
                _nativeIterator,
                &resultCount,
                (GdiPlus.PointF*)p,
                t,
                points.Length).ThrowIfFailed();

            GC.KeepAlive(this);
            return resultCount;
        }
    }

    /// <summary>
    ///  Copies the <see cref="GraphicsPath.PathPoints"/> property and <see cref="GraphicsPath.PathTypes"/> property data
    ///  of the associated <see cref="GraphicsPath"/>.
    /// </summary>
    /// <param name="points">Upon return, contains <see cref="PointF"/> structures that represent the points in the path.</param>
    /// <param name="types">Upon return, contains bytes that represent the types of points in the path.</param>
    /// <param name="startIndex">The index of the first point to copy.</param>
    /// <param name="endIndex">The index of the last point to copy.</param>
    /// <returns>The number of points copied.</returns>
    public unsafe int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
        => CopyData(points.OrThrowIfNull().AsSpan(), types.OrThrowIfNull().AsSpan(), startIndex, endIndex);

    /// <inheritdoc cref="CopyData(ref PointF[], ref byte[], int, int)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    unsafe int CopyData(Span<PointF> points, Span<byte> types, int startIndex, int endIndex)
    {
        int count = endIndex - startIndex + 1;

        if ((points.Length != types.Length)
            || endIndex < 0
            || startIndex < 0
            || endIndex < startIndex
            || count > points.Length
            || endIndex >= Count)
        {
            throw Status.InvalidParameter.GetException();
        }

        fixed (PointF* p = points)
        fixed (byte* t = types)
        {
            int resultCount;
            PInvokeGdiPlus.GdipPathIterCopyData(
                _nativeIterator,
                &resultCount,
                (GdiPlus.PointF*)p,
                t,
                startIndex,
                endIndex).ThrowIfFailed();

            GC.KeepAlive(this);
            return resultCount;
        }
    }
}
