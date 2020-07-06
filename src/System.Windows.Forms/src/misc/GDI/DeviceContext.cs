﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Represents a Win32 device context.  Provides operations for setting some of the properties
    ///  of a device context.  It's the managed wrapper for an HDC.
    ///
    ///  This class is divided into two files separating the code that needs to be compiled into
    ///  reatail builds and debugging code.
    /// </summary>
    /// <remarks>
    ///  This class is a wrapper to a Win32 device context, and the Hdc property is the way to get a
    ///  handle to it.
    ///
    ///  The hDc is released/deleted only when owned by the object, meaning it was created internally;
    ///  in this case, the object is responsible for releasing/deleting it.
    ///  In the case the object is created from an exisiting hdc, it is not released; this is consistent
    ///  with the Win32 guideline that says if you call GetDC/CreateDC/CreatIC/CreateEnhMetafile, you are
    ///  responsible for calling ReleaseDC/DeleteDC/DeleteEnhMetafile respectivelly.
    ///
    ///  This class implements some of the operations commonly performed on the properties of a dc  in WinForms,
    ///  specially for interacting with GDI+, like clipping and coordinate transformation.
    ///  Several properties are not persisted in the dc but instead they are set/reset during a more comprehensive
    ///  operation like text rendering or painting; for instance text alignment is set and reset during DrawText (GDI),
    ///  DrawString (GDI+).
    ///
    ///  Other properties are persisted from operation to operation until they are reset, like clipping,
    ///  one can make several calls to Graphics or WindowsGraphics obect after setting the dc clip area and
    ///  before resetting it; these kinds of properties are the ones implemented in this class.
    ///  This kind of properties place an extra chanllenge in the scenario where a DeviceContext is obtained
    ///  from a Graphics object that has been used with GDI+, because GDI+ saves the hdc internally, rendering the
    ///  DeviceContext underlying hdc out of sync.  DeviceContext needs to support these kind of properties to
    ///  be able to keep the GDI+ and GDI HDCs in sync.
    ///
    ///  A few other persisting properties have been implemented in DeviceContext2, among them:
    ///
    ///   1. Window origin.
    ///   2. Bounding rectangle.
    ///   3. DC origin.
    ///   4. View port extent.
    ///   5. View port origin.
    ///   6. Window extent
    ///
    ///  Other non-persisted properties just for information: Background/Forground color, Palette, Color adjustment,
    ///  Color space, ICM mode and profile, Current pen position, Binary raster op (not supported by GDI+),
    ///  Background mode, Logical Pen, DC pen color, ARc direction, Miter limit, Logical brush, DC brush color,
    ///  Brush origin, Polygon filling mode, Bitmap stretching mode, Logical font, Intercharacter spacing,
    ///  Font mapper flags, Text alignment, Test justification, Layout, Path, Meta region.
    ///  See book "Windows Graphics Programming - Feng Yuang", P315 - Device Context Attributes.
    /// </remarks>
    internal sealed partial class DeviceContext : MarshalByRefObject, IDeviceContext, IDisposable, IHandle
    {
        private Gdi32.HDC _hDC;
        public event EventHandler? Disposing;
        private bool _disposed;

        // We cache the hWnd when creating the dc from one, to provide support forIDeviceContext.GetHdc/ReleaseHdc.
        // This hWnd could be null, in such case it is referring to the screen.
        private readonly IntPtr _hWnd = (IntPtr)(-1); // Unlikely to be a valid hWnd.

        private Gdi32.HGDIOBJ _hInitialPen;
        private Gdi32.HGDIOBJ _hInitialBrush;
        private Gdi32.HGDIOBJ _hInitialBmp;
        private Gdi32.HGDIOBJ _hInitialFont;

        private Gdi32.HGDIOBJ _hCurrentPen;
        private Gdi32.HGDIOBJ _hCurrentBrush;
        private Gdi32.HGDIOBJ _hCurrentBmp;
        private Gdi32.HGDIOBJ _hCurrentFont;

        private Stack<GraphicsState>? _contextStack;

        /// <summary>
        ///  The device type the context refers to.
        /// </summary>
        public DeviceContextType DeviceContextType { get; }

        IntPtr IHandle.Handle => (IntPtr)Hdc;

        /// <summary>
        ///  This object's hdc.  If this property is called, then the object will be used as an HDC wrapper,
        ///  so the hdc is cached and calls to GetHdc/ReleaseHdc won't PInvoke into GDI.
        ///  Call Dispose to properly release the hdc.
        /// </summary>
        public Gdi32.HDC Hdc
        {
            get
            {
                if (_hDC.IsNull)
                {
                    if (DeviceContextType == DeviceContextType.Display)
                    {
                        Debug.Assert(!_disposed, "Accessing a disposed DC, forcing recreation of HDC - this will generate a Handle leak!");

                        // Note: ReleaseDC must be called from the same thread. This applies only to HDC obtained
                        // from calling GetDC. This means Display DeviceContext objects should never be finalized.
                        _hDC = (Gdi32.HDC)((IDeviceContext)this).GetHdc();  // this.hDC will be released on call to Dispose.
                        CacheInitialState();
                    }
                }

                return _hDC;
            }
        }

        // Due to a problem with calling DeleteObject() on currently selected GDI objects,
        // we now track the initial set of objects when a DeviceContext is created.  Then,
        // we also track which objects are currently selected in the DeviceContext.  When
        // a currently selected object is disposed, it is first replaced in the DC and then
        // deleted.

        private void CacheInitialState()
        {
            _hCurrentPen = _hInitialPen = Gdi32.GetCurrentObject(this, Gdi32.ObjectType.OBJ_PEN);
            _hCurrentBrush = _hInitialBrush = Gdi32.GetCurrentObject(this, Gdi32.ObjectType.OBJ_BRUSH);
            _hCurrentBmp = _hInitialBmp = Gdi32.GetCurrentObject(this, Gdi32.ObjectType.OBJ_BITMAP);
            _hCurrentFont = _hInitialFont = Gdi32.GetCurrentObject(this, Gdi32.ObjectType.OBJ_FONT);
        }

        public void DeleteObject(Gdi32.HGDIOBJ handle, GdiObjectType type)
        {
            Gdi32.HGDIOBJ handleToDelete = default;
            switch (type)
            {
                case GdiObjectType.Pen:
                    if (handle == _hCurrentPen)
                    {
                        Gdi32.HGDIOBJ currentPen = Gdi32.SelectObject(this, _hInitialPen);
                        Debug.Assert(currentPen == _hCurrentPen, "DeviceContext thinks a different pen is selected than the HDC");
                        _hCurrentPen = default;
                    }
                    handleToDelete = handle;
                    break;
                case GdiObjectType.Brush:
                    if (handle == _hCurrentBrush)
                    {
                        Gdi32.HGDIOBJ currentBrush = Gdi32.SelectObject(this, _hInitialBrush);
                        Debug.Assert(currentBrush == _hCurrentBrush, "DeviceContext thinks a different brush is selected than the HDC");
                        _hCurrentBrush = default;
                    }
                    handleToDelete = handle;
                    break;
                case GdiObjectType.Bitmap:
                    if (handle == _hCurrentBmp)
                    {
                        Gdi32.HGDIOBJ currentBmp = Gdi32.SelectObject(this, _hInitialBmp);
                        Debug.Assert(currentBmp == _hCurrentBmp, "DeviceContext thinks a different brush is selected than the HDC");
                        _hCurrentBmp = default;
                    }
                    handleToDelete = handle;
                    break;
            }

            Gdi32.DeleteObject(handleToDelete);
        }

        /// <summary>
        ///  Constructor to contruct a DeviceContext object from an window handle.
        /// </summary>
        private DeviceContext(IntPtr hWnd)
        {
            _hWnd = hWnd;
            DeviceContextType = DeviceContextType.Display;

            DeviceContexts.AddDeviceContext(this);

            // the hDc will be created on demand.
        }

        /// <summary>
        ///  Constructor to contruct a DeviceContext object from an existing Win32 device context handle.
        /// </summary>
        private DeviceContext(Gdi32.HDC hDC, DeviceContextType dcType)
        {
            _hDC = hDC;
            DeviceContextType = dcType;

            CacheInitialState();
            DeviceContexts.AddDeviceContext(this);

            if (dcType == DeviceContextType.Display)
            {
                _hWnd = User32.WindowFromDC(this);
            }
        }

        /// <summary>
        ///  Creates a DeviceContext object wrapping a memory DC compatible with the specified device.
        /// </summary>
        public static DeviceContext FromCompatibleDC(Gdi32.HDC hdc)
        {
            // If hdc is null, the function creates a memory DC compatible with the application's current screen.
            // In this case the thread that calls CreateCompatibleDC owns the HDC that is created. When this thread is destroyed,
            // the HDC is no longer valid.

            Gdi32.HDC compatibleDc = Gdi32.CreateCompatibleDC(hdc);
            return new DeviceContext(compatibleDc, DeviceContextType.Memory);
        }

        /// <summary>
        ///  Used for wrapping an existing hdc.  In this case, this object doesn't own the hdc
        ///  so calls to GetHdc/ReleaseHdc don't PInvoke into GDI.
        /// </summary>
        public static DeviceContext FromHdc(Gdi32.HDC hdc) => new DeviceContext(hdc, DeviceContextType.Unknown);

        /// <summary>
        ///  When hwnd is null, we are getting the screen DC.
        /// </summary>
        public static DeviceContext FromHwnd(IntPtr hwnd) => new DeviceContext(hwnd);

        ~DeviceContext() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Only fire if we're not coming in on the finalizer
                Disposing?.Invoke(this, EventArgs.Empty);
            }

            _disposed = true;

            DisposeFont(disposing);

            switch (DeviceContextType)
            {
                case DeviceContextType.Display:
                    Debug.Assert(disposing, "WARNING: Finalizing a Display DeviceContext.\r\nReleaseDC may fail when not called from the same thread GetDC was called from.");

                    ((IDeviceContext)this).ReleaseHdc();
                    break;

                case DeviceContextType.Information:
                case DeviceContextType.NamedDevice:

                    // CreateDC and CreateIC add an HDC handle to the HandleCollector; to remove it properly we need
                    // to call DeleteHDC.
                    Gdi32.DeleteDC(_hDC);
                    _hDC = default;
                    break;

                case DeviceContextType.Memory:

                    // CreatCompatibleDC adds a GDI handle to HandleCollector, to remove it properly we need to call
                    // DeleteDC.
                    Gdi32.DeleteDC(_hDC);
                    _hDC = default;
                    break;

                case DeviceContextType.Unknown:
                default:
                    return;
                    // do nothing, the hdc is not owned by this object.
                    // in this case it is ok if disposed throught finalization.
            }
        }

        /// <summary>
        ///  Explicit interface method implementation to hide them a bit for usability reasons so the object is seen
        ///  as a wrapper around an hdc that is always available, and for performance reasons since it caches the hdc
        ///  if used in this way.
        /// </summary>
        IntPtr IDeviceContext.GetHdc()
        {
            if (_hDC.IsNull)
            {
                Debug.Assert(DeviceContextType == DeviceContextType.Display, "Calling GetDC from a non display/window device.");

                // Note: for common DCs, GetDC assigns default attributes to the DC each time it is retrieved.
                // For example, the default font is System.
                _hDC = User32.GetDC(new HandleRef(this, _hWnd));
            }

            return (IntPtr)_hDC;
        }

        /// <summary>
        ///  If the object was created from a DC, this object doesn't 'own' the dc so we just ignore
        ///  this call.
        /// </summary>
        void IDeviceContext.ReleaseHdc()
        {
            if (!_hDC.IsNull && DeviceContextType == DeviceContextType.Display)
            {
                User32.ReleaseDC(new HandleRef(this, _hWnd), _hDC);
                _hDC = default;
            }
        }

        /// <summary>
        ///  Restores the device context to the specified state. The DC is restored by popping state information off a
        ///  stack created by earlier calls to the SaveHdc function.
        ///  The stack can contain the state information for several instances of the DC. If the state specified by the
        ///  specified parameter is not at the top of the stack, RestoreDC deletes all state information between the top
        ///  of the stack and the specified instance.
        ///  Specifies the saved state to be restored. If this parameter is positive, nSavedDC represents a specific
        ///  instance of the state to be restored. If this parameter is negative, nSavedDC represents an instance relative
        ///  to the current state. For example, -1 restores the most recently saved state.
        ///  See MSDN for more info.
        /// </summary>
        public void RestoreHdc()
        {
            // Note: Don't use the Hdc property here, it would force handle creation.
            Gdi32.RestoreDC(_hDC, -1);
            Debug.Assert(_contextStack != null, "Someone is calling RestoreHdc() before SaveHdc()");

            if (_contextStack != null)
            {
                GraphicsState g = _contextStack.Pop();

                _hCurrentBmp = g.hBitmap;
                _hCurrentBrush = g.hBrush;
                _hCurrentPen = g.hPen;
                _hCurrentFont = g.hFont;

                if (g.font != null && g.font.IsAlive)
                {
                    ActiveFont = g.font.Target as WindowsFont;
                }
                else
                {
                    WindowsFont? previousFont = ActiveFont;
                    ActiveFont = null;
                    if (previousFont != null && MeasurementDCInfo.IsMeasurementDC(this))
                    {
                        previousFont.Dispose();
                    }
                }
            }

            // in this case, GDI will copy back the previously saved font into the DC.
            // we dont actually know what the font is in our measurement DC so
            // we need to clear it off.
            MeasurementDCInfo.ResetIfIsMeasurementDC(_hDC);
        }

        /// <summary>
        ///  Saves the current state of the device context by copying data describing selected objects and graphic
        ///  modes (such as the bitmap, brush, palette, font, pen, region, drawing mode, and mapping mode) to a
        ///  context stack.
        ///  The SaveDC function can be used any number of times to save any number of instances of the DC state.
        ///  A saved state can be restored by using the RestoreHdc method.
        ///  See MSDN for more details.
        /// </summary>
        public int SaveHdc()
        {
            int state = Gdi32.SaveDC(this);

            var g = new GraphicsState
            {
                hBitmap = _hCurrentBmp,
                hBrush = _hCurrentBrush,
                hPen = _hCurrentPen,
                hFont = _hCurrentFont,
                font = new WeakReference(ActiveFont)
            };
            _contextStack ??= new Stack<GraphicsState>();
            _contextStack.Push(g);

            return state;
        }

        /// <summary>
        ///  Modifies the viewport origin for a device context using the specified horizontal and vertical offsets in logical units.
        /// </summary>
        public void TranslateTransform(int dx, int dy)
        {
            var origin = new Point();
            Gdi32.OffsetViewportOrgEx(this, dx, dy, ref origin);
        }

        public override bool Equals(object? obj)
        {
            DeviceContext? other = obj as DeviceContext;

            if (other == this)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            // Note: Use property instead of field so the HDC is initialized.  Also, this avoid serialization issues (the obj could be a proxy that does not have access to private fields).
            return other.Hdc.Handle == Hdc.Handle;
        }

        /// <summary>
        ///  This allows collections to treat DeviceContext objects wrapping the same HDC as the same objects.
        /// </summary>
        public override int GetHashCode() => Hdc.GetHashCode();

        internal struct GraphicsState
        {
            internal Gdi32.HGDIOBJ hBrush;
            internal Gdi32.HGDIOBJ hFont;
            internal Gdi32.HGDIOBJ hPen;
            internal Gdi32.HGDIOBJ hBitmap;
            internal WeakReference font;
        }
    }
}
