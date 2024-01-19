// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing;

public unsafe abstract class Brush : MarshalByRefObject, ICloneable, IDisposable
{
#if FINALIZATION_WATCH
    private string allocationSite = Graphics.GetAllocationStack();
#endif
    // Handle to native GDI+ brush object to be used on demand.
    private GpBrush* _nativeBrush;

    public abstract object Clone();

    protected internal void SetNativeBrush(IntPtr brush) => SetNativeBrushInternal((GpBrush*)brush);
    internal void SetNativeBrushInternal(GpBrush* brush) => _nativeBrush = brush;

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    internal GpBrush* NativeBrush => _nativeBrush;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
#if FINALIZATION_WATCH
        Debug.WriteLineIf(!disposing && _nativeBrush is not null, $"""
            **********************
            Disposed through finalization:
            {allocationSite}
            """);
#endif

        if (_nativeBrush is not null)
        {
            try
            {
#if DEBUG
                Status status = !Gdip.Initialized ? Status.Ok :
#endif
                PInvoke.GdipDeleteBrush(_nativeBrush);
#if DEBUG
                Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
#endif
            }
            catch (Exception ex) when (!ClientUtils.IsSecurityOrCriticalException(ex))
            {
                // Catch all non fatal exceptions. This includes exceptions like EntryPointNotFoundException, that is thrown
                // on Windows Nano.
            }
            finally
            {
                _nativeBrush = null;
            }
        }
    }

    ~Brush() => Dispose(disposing: false);
}
