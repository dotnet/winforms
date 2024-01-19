// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public unsafe sealed class GraphicsPathIterator : MarshalByRefObject, IDisposable
{
    // handle to native path iterator object
    internal GpPathIterator* _nativeIterator;

    public GraphicsPathIterator(GraphicsPath? path)
    {
        GpPathIterator* iterator;
        PInvoke.GdipCreatePathIter(&iterator, path.Pointer()).ThrowIfFailed();
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
                PInvoke.GdipDeletePathIter(_nativeIterator);
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
            PInvoke.GdipPathIterNextSubpath(_nativeIterator, &resultCount, s, e, &tempIsClosed).ThrowIfFailed();
            isClosed = tempIsClosed;
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public int NextSubpath(GraphicsPath path, out bool isClosed)
    {
        int resultCount;
        BOOL tempIsClosed;
        PInvoke.GdipPathIterNextSubpathPath(_nativeIterator, &resultCount, path.Pointer(), &tempIsClosed).ThrowIfFailed();
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
            PInvoke.GdipPathIterNextPathType(_nativeIterator, &resultCount, pt, s, e).ThrowIfFailed();
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public int NextMarker(out int startIndex, out int endIndex)
    {
        int resultCount;

        fixed (int* s = &startIndex, e = &endIndex)
        {
            PInvoke.GdipPathIterNextMarker(_nativeIterator, &resultCount, s, e).ThrowIfFailed();
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public int NextMarker(GraphicsPath path)
    {
        int resultCount;
        PInvoke.GdipPathIterNextMarkerPath(_nativeIterator, &resultCount, path.Pointer()).ThrowIfFailed();
        GC.KeepAlive(this);
        return resultCount;
    }

    public int Count
    {
        get
        {
            int resultCount;
            PInvoke.GdipPathIterGetCount(_nativeIterator, &resultCount).ThrowIfFailed();
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public int SubpathCount
    {
        get
        {
            int resultCount;
            PInvoke.GdipPathIterGetSubpathCount(_nativeIterator, &resultCount).ThrowIfFailed();
            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public bool HasCurve()
    {
        BOOL hasCurve;
        PInvoke.GdipPathIterHasCurve(_nativeIterator, &hasCurve).ThrowIfFailed();
        GC.KeepAlive(this);
        return hasCurve;
    }

    public void Rewind()
    {
        PInvoke.GdipPathIterRewind(_nativeIterator).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public unsafe int Enumerate(ref PointF[] points, ref byte[] types)
    {
        if (points.Length != types.Length)
            throw Status.InvalidParameter.GetException();

        if (points.Length == 0)
            return 0;

        fixed (PointF* p = points)
        fixed (byte* t = types)
        {
            int resultCount;
            PInvoke.GdipPathIterEnumerate(
                _nativeIterator,
                &resultCount,
                (GdiPlus.PointF*)p,
                t,
                points.Length).ThrowIfFailed();

            GC.KeepAlive(this);
            return resultCount;
        }
    }

    public unsafe int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
    {
        if ((points.Length != types.Length) || (endIndex - startIndex + 1 > points.Length))
            throw Status.InvalidParameter.GetException();

        fixed (PointF* p = points)
        fixed (byte* t = types)
        {
            int resultCount;
            PInvoke.GdipPathIterCopyData(
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
