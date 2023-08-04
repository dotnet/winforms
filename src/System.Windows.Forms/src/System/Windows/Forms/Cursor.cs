﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using static Interop;

namespace System.Windows.Forms;

/// <summary>
///  Represents the image used to paint the mouse pointer. Different cursor shapes are used to inform the user
///  what operation the mouse will have.
/// </summary>
[TypeConverter(typeof(CursorConverter))]
[Editor($"System.Drawing.Design.CursorEditor, {AssemblyRef.SystemDrawingDesign}", typeof(UITypeEditor))]
public sealed class Cursor : IDisposable, ISerializable, IHandle<HICON>, IHandle<HANDLE>
{
    private static Size s_cursorSize = Size.Empty;

    private readonly byte[]? _cursorData;
    private HCURSOR _handle;
    private readonly Flags _flags;
    private readonly PCWSTR _resourceId;

    private bool OwnHandle => (_flags & Flags.OwnHandle) != 0;
    private bool IsWellKnown => (_flags & Flags.InternalCursor) != 0;

    /// <summary>
    ///  Private constructor. If you want a standard system cursor, use one of the
    ///  definitions in the Cursors class.
    /// </summary>
    internal unsafe Cursor(PCWSTR nResourceId)
    {
        // We don't delete stock cursors.
        _flags &= ~Flags.OwnHandle;
        _flags |= Flags.InternalCursor;
        _resourceId = nResourceId;
        _handle = PInvoke.LoadCursor((HINSTANCE)0, nResourceId);
        if (_handle.IsNull)
        {
            throw new Win32Exception(string.Format(SR.FailedToLoadCursor, Marshal.GetLastWin32Error()));
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Cursor"/> class from the specified <paramref name="handle"/>.
    /// </summary>
    public Cursor(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException(string.Format(SR.InvalidGDIHandle, (typeof(Cursor)).Name), nameof(handle));
        }

        _handle = (HCURSOR)handle;
        _flags &= ~Flags.OwnHandle;
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Cursor"/> class with the specified <paramref name="fileName"/>.
    /// </summary>
    public Cursor(string fileName)
    {
        _cursorData = File.ReadAllBytes(fileName);
        _flags |= Flags.OwnHandle;
        LoadPicture(
            new Ole32.GPStream(new MemoryStream(_cursorData)),
            nameof(fileName));
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Cursor"/> class from the specified <paramref name="resource"/>.
    /// </summary>
    public Cursor(Type type, string resource)
        : this((type.OrThrowIfNull()).Module.Assembly.GetManifestResourceStream(type, resource)!)
    {
        _flags |= Flags.OwnHandle;

        if (type == typeof(Cursor))
        {
            _flags |= Flags.InternalCursor;
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Cursor"/> class from the
    ///  specified data <paramref name="stream"/>.
    /// </summary>
    public Cursor(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        MemoryStream memoryStream = new();
        // reset stream position to start, there are no gaurantees it is at the start.
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        stream.CopyTo(memoryStream);
        _cursorData = memoryStream.ToArray();
        _flags |= Flags.OwnHandle;

        // stream.CopyTo causes both streams to advance. So reset it for LoadPicture.
        memoryStream.Position = 0;
        LoadPicture(
            new Ole32.GPStream(memoryStream),
            nameof(stream));
    }

    /// <summary>
    ///  Gets or sets a <see cref="Rectangle"/> that represents the current clipping
    ///  rectangle for this <see cref="Cursor"/> in screen coordinates.
    /// </summary>
    public static unsafe Rectangle Clip
    {
        get
        {
            PInvoke.GetClipCursor(out RECT rect);
            return rect;
        }
        set
        {
            if (value.IsEmpty)
            {
                PInvoke.ClipCursor((RECT*)null);
            }
            else
            {
                RECT rect = value;
                PInvoke.ClipCursor(&rect);
            }
        }
    }

    /// <summary>
    ///  Gets or sets a <see cref="Cursor"/> that represents the current mouse cursor.
    ///  The value is <see langword="null"/> if the current mouse cursor is not visible.
    /// </summary>
    public static Cursor? Current
    {
        get
        {
            HCURSOR cursor = PInvoke.GetCursor();
            return cursor.IsNull ? null : new Cursor(cursor);
        }
        set => PInvoke.SetCursor(value?._handle ?? HCURSOR.Null);
    }

    /// <summary>
    ///  Gets the Win32 handle for this <see cref="Cursor"/>.
    /// </summary>
    public IntPtr Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_handle.IsNull, this);
            return (nint)_handle;
        }
    }

    /// <summary>
    ///  Returns the "hot" location of the cursor.
    /// </summary>
    public Point HotSpot
    {
        get
        {
            using ICONINFO info = PInvoke.GetIconInfo(this);
            return new Point((int)info.xHotspot, (int)info.yHotspot);
        }
    }

    /// <summary>
    ///  Gets or sets a <see cref="Point"/> that specifies the current cursor position in screen coordinates.
    /// </summary>
    public static Point Position
    {
        get
        {
            PInvoke.GetCursorPos(out Point p);
            return p;
        }
        set => PInvoke.SetCursorPos(value.X, value.Y);
    }

    /// <summary>
    ///  Gets the size of this <see cref="Cursor"/> object.
    /// </summary>
    public Size Size
    {
        get
        {
            if (s_cursorSize.IsEmpty)
            {
                s_cursorSize = SystemInformation.CursorSize;
            }

            return s_cursorSize;
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag { get; set; }

    HICON IHandle<HICON>.Handle => (HICON)Handle;

    HANDLE IHandle<HANDLE>.Handle => (HANDLE)Handle;

    /// <summary>
    ///  Duplicates this the Win32 handle of this <see cref="Cursor"/>.
    /// </summary>
    public IntPtr CopyHandle()
    {
        Size sz = Size;
        return PInvoke.CopyImage(this, GDI_IMAGE_TYPE.IMAGE_CURSOR, sz.Width, sz.Height, IMAGE_FLAGS.LR_DEFAULTCOLOR);
    }

    /// <summary>
    ///  Cleans up the resources allocated by this object. Once called, the cursor object is no longer useful.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_handle.IsNull && OwnHandle)
        {
            PInvoke.DestroyCursor(_handle);
            _handle = HCURSOR.Null;
        }
    }

    /// <summary>
    ///  Draws this image to a graphics object.  The drawing command originates on the graphics
    ///  object, but a graphics object generally has no idea how to render a given image.  So,
    ///  it passes the call to the actual image.  This version crops the image to the given
    ///  dimensions and allows the user to specify a rectangle within the image to draw.
    /// </summary>
    private void DrawImageCore(Graphics graphics, Rectangle imageRect, Rectangle targetRect, bool stretch)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        // Support GDI+ Translate method
        targetRect.X += (int)graphics.Transform.OffsetX;
        targetRect.Y += (int)graphics.Transform.OffsetY;

        using DeviceContextHdcScope dc = new(graphics, applyGraphicsState: false);

        int imageX = 0;
        int imageY = 0;
        int imageWidth;
        int imageHeight;
        int targetX = 0;
        int targetY = 0;
        int targetWidth = 0;
        int targetHeight = 0;

        Size cursorSize = Size;

        // compute the dimensions of the icon, if needed
        if (!imageRect.IsEmpty)
        {
            imageX = imageRect.X;
            imageY = imageRect.Y;
            imageWidth = imageRect.Width;
            imageHeight = imageRect.Height;
        }
        else
        {
            imageWidth = cursorSize.Width;
            imageHeight = cursorSize.Height;
        }

        if (!targetRect.IsEmpty)
        {
            targetX = targetRect.X;
            targetY = targetRect.Y;
            targetWidth = targetRect.Width;
            targetHeight = targetRect.Height;
        }
        else
        {
            targetWidth = cursorSize.Width;
            targetHeight = cursorSize.Height;
        }

        int drawWidth, drawHeight;
        int clipWidth, clipHeight;

        if (stretch)
        {
            // Short circuit the simple case of blasting an icon to the screen
            if (targetWidth == imageWidth && targetHeight == imageHeight
                && imageX == 0 && imageY == 0
                && imageWidth == cursorSize.Width && imageHeight == cursorSize.Height)
            {
                PInvoke.DrawIcon(dc, targetX, targetY, this);
                return;
            }

            drawWidth = cursorSize.Width * targetWidth / imageWidth;
            drawHeight = cursorSize.Height * targetHeight / imageHeight;
            clipWidth = targetWidth;
            clipHeight = targetHeight;
        }
        else
        {
            // Short circuit the simple case of blasting an icon to the screen
            if (imageX == 0 && imageY == 0
                && cursorSize.Width <= targetWidth && cursorSize.Height <= targetHeight
                && cursorSize.Width == imageWidth && cursorSize.Height == imageHeight)
            {
                PInvoke.DrawIcon(dc, targetX, targetY, this);
                return;
            }

            drawWidth = cursorSize.Width;
            drawHeight = cursorSize.Height;
            clipWidth = targetWidth < imageWidth ? targetWidth : imageWidth;
            clipHeight = targetHeight < imageHeight ? targetHeight : imageHeight;
        }

        // The ROP is SRCCOPY, so we can be simple here and take advantage of clipping regions.
        // Drawing the cursor is merely a matter of offsetting and clipping.
        PInvoke.IntersectClipRect(dc, targetX, targetY, targetX + clipWidth, targetY + clipHeight);
        PInvoke.DrawIconEx(
            (HDC)dc,
            targetX - imageX,
            targetY - imageY,
            this,
            drawWidth,
            drawHeight);

        // Let GDI+ restore clipping
        return;
    }

    /// <summary>
    ///  Draws this <see cref="Cursor"/> to a <see cref="Graphics"/>.
    /// </summary>
    public void Draw(Graphics g, Rectangle targetRect)
    {
        DrawImageCore(g, Rectangle.Empty, targetRect, stretch: false);
    }

    /// <summary>
    ///  Draws this <see cref="Cursor"/> to a <see cref="Graphics"/>.
    /// </summary>
    public void DrawStretched(Graphics g, Rectangle targetRect)
    {
        DrawImageCore(g, Rectangle.Empty, targetRect, stretch: true);
    }

    /// <summary>
    ///  Cleans up Windows resources for this object.
    /// </summary>
    ~Cursor()
    {
        Dispose(disposing: false);
    }

    /// <summary>
    ///  ISerializable private implementation
    /// </summary>
    void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Hides the cursor. For every call to Cursor.hide() there must be a balancing call to Cursor.show().
    /// </summary>
    public static void Hide() => PInvoke.ShowCursor(bShow: false);

    private Size GetIconSize(HICON iconHandle)
    {
        // this code is adapted from Icon.GetIconSize please take this into account when changing this

        using ICONINFO info = PInvoke.GetIconInfo(iconHandle);
        if (!info.hbmColor.IsNull)
        {
            PInvoke.GetObject(info.hbmColor, out BITMAP bitmap);
            return new Size(bitmap.bmWidth, bitmap.bmHeight);
        }
        else if (!info.hbmMask.IsNull)
        {
            PInvoke.GetObject(info.hbmMask, out BITMAP bitmap);
            return new Size(bitmap.bmWidth, bitmap.bmHeight / 2);
        }
        else
        {
            return Size;
        }
    }

    /// <summary>
    ///  Loads a picture from the requested stream.
    /// </summary>
    private unsafe void LoadPicture(IStream.Interface stream, string paramName)
    {
        Debug.Assert(stream is not null, "Stream should be validated before this method is called.");

        try
        {
            using ComScope<IPicture> picture = new(null);
            PInvoke.OleCreatePictureIndirect(lpPictDesc: null, IID.Get<IPicture>(), fOwn: true, picture).ThrowOnFailure();

            using ComScope<IPersistStream> persist = new(null);
            picture.Value->QueryInterface(IID.Get<IPersistStream>(), persist).ThrowOnFailure();

            using var pStream = ComHelpers.GetComScope<IStream>(stream);
            persist.Value->Load(pStream);

            if (picture.Value->Type == PICTYPE.PICTYPE_ICON)
            {
                HICON cursorHandle = (HICON)picture.Value->Handle;
                Size picSize = GetIconSize(cursorHandle);
                if (DpiHelper.IsScalingRequired)
                {
                    picSize = DpiHelper.LogicalToDeviceUnits(picSize);
                }

                _handle = (HCURSOR)PInvoke.CopyImage(
                    (HANDLE)cursorHandle.Value,
                    GDI_IMAGE_TYPE.IMAGE_CURSOR,
                    picSize.Width,
                    picSize.Height,
                    IMAGE_FLAGS.LR_DEFAULTCOLOR).Value;

                if (_handle.IsNull)
                {
                    throw new Win32Exception(string.Format(SR.FailedToLoadCursor, Marshal.GetLastWin32Error()));
                }
            }
            else
            {
                throw new ArgumentException(string.Format(SR.InvalidPictureType, nameof(picture), nameof(Cursor)), paramName);
            }
        }
        catch (COMException e)
        {
            throw new ArgumentException(SR.InvalidPictureFormat, paramName, e);
        }
    }

    /// <summary>
    ///  Saves a picture from the requested stream.
    /// </summary>
    internal unsafe byte[] GetData()
    {
        if (_resourceId.Value is not null)
        {
            throw new FormatException(SR.CursorCannotCovertToBytes);
        }

        if (_cursorData is null)
        {
            throw new InvalidOperationException(SR.InvalidPictureFormat);
        }

        return (byte[])_cursorData.Clone();
    }

    internal bool IsValid() => _handle != IntPtr.Zero;

    /// <summary>
    ///  Displays the cursor. For every call to Cursor.show() there must have been
    ///  a previous call to Cursor.hide().
    /// </summary>
    public static void Show() => PInvoke.ShowCursor(bShow: true);

    /// <summary>
    ///  Retrieves a human readable string representing this <see cref="Cursor"/>.
    /// </summary>
    public override string ToString()
    {
        string? cursorName = IsWellKnown || !OwnHandle ? TypeDescriptor.GetConverter(typeof(Cursor)).ConvertToString(this) : base.ToString();
        return $"[Cursor: {cursorName}]";
    }

    public static bool operator ==(Cursor? left, Cursor? right)
    {
        return right is null || left is null ? left is null && right is null : left._handle == right._handle;
    }

    public static bool operator !=(Cursor? left, Cursor? right) => !(left == right);

    public override int GetHashCode() => (int)_handle.Value;

    public override bool Equals(object? obj) => obj is Cursor cursor && this == cursor;

    [Flags]
    private enum Flags : byte
    {
        None = 0,
        OwnHandle = 1 << 0,
        InternalCursor = 1 << 1,
    }
}
