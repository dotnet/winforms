// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents the image used to paint the mouse pointer.
    ///  Different cursor shapes are used to inform the user what operation the mouse will
    ///  have.
    /// </summary>
    [TypeConverter(typeof(CursorConverter))]
    [Editor("System.Drawing.Design.CursorEditor, " + AssemblyRef.SystemDrawingDesign, typeof(UITypeEditor))]
    public sealed class Cursor : IDisposable, ISerializable, IHandle
    {
        private static Size s_cursorSize = Size.Empty;

        private readonly byte[]? _cursorData;
        private IntPtr _handle = IntPtr.Zero;       // handle to loaded image
        private bool _ownHandle = true;
        private readonly int _resourceId;

        /// <summary>
        ///  Private constructor. If you want a standard system cursor, use one of the
        ///  definitions in the Cursors class.
        /// </summary>
        internal Cursor(int nResourceId)
        {
            // We don't delete stock cursors.
            _ownHandle = false;
            _resourceId = nResourceId;
            _handle = User32.LoadCursorW(IntPtr.Zero, (IntPtr)nResourceId);
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Cursor'/> class with the specified handle.
        /// </summary>
        public Cursor(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException(string.Format(SR.InvalidGDIHandle, (typeof(Cursor)).Name), nameof(handle));
            }

            _handle = handle;
            _ownHandle = false;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Cursor'/>
        ///  class with the specified filename.
        /// </summary>
        public Cursor(string fileName)
        {
            _cursorData = File.ReadAllBytes(fileName);
            LoadPicture(
                new Ole32.GPStream(new MemoryStream(_cursorData)),
                nameof(fileName));
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Cursor'/> class from the specified resource.
        /// </summary>
        public Cursor(Type type, string resource)
            : this((type ?? throw new ArgumentNullException(nameof(type))).Module.Assembly.GetManifestResourceStream(type, resource)!)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Cursor'/> class from the
        ///  specified data stream.
        /// </summary>
        public Cursor(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            int length = checked((int)stream.Length);
            _cursorData = new byte[length];
            stream.Read(_cursorData, 0, length);
            LoadPicture(
                new Ole32.GPStream(new MemoryStream(_cursorData)),
                nameof(stream));
        }

        /// <summary>
        ///  Gets or sets a <see cref='Rectangle'/> that represents the current clipping
        ///  rectangle for this <see cref='Cursor'/> in screen coordinates.
        /// </summary>
        public unsafe static Rectangle Clip
        {
            get
            {
                User32.GetClipCursor(out RECT rect);
                return rect;
            }
            set
            {
                if (value.IsEmpty)
                {
                    User32.ClipCursor(null);
                }
                else
                {
                    RECT rect = value;
                    User32.ClipCursor(&rect);
                }
            }
        }

        /// <summary>
        ///  Gets or sets a <see cref='Cursor'/> that represents the current mouse cursor.
        ///  The value is <see langword="null"/> if the current mouse cursor is not visible.
        /// </summary>
        public static Cursor? Current
        {
            get
            {
                IntPtr curHandle = User32.GetCursor();
                if (curHandle == IntPtr.Zero)
                {
                    return null;
                }

                return new Cursor(curHandle);
            }
            set => User32.SetCursor(value);
        }

        /// <summary>
        ///  Gets the Win32 handle for this <see cref='Cursor'/>.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                if (_handle == IntPtr.Zero)
                {
                    throw new ObjectDisposedException(string.Format(SR.ObjectDisposed, GetType().Name));
                }
                return _handle;
            }
        }

        /// <summary>
        ///  Returns the "hot" location of the cursor.
        /// </summary>
        public Point HotSpot
        {
            get
            {
                using User32.ICONINFO info = User32.GetIconInfo(this);
                return new Point((int)info.xHotspot, (int)info.yHotspot);
            }
        }

        /// <summary>
        ///  Gets or sets a <see cref='Point'/> that specifies the current cursor
        ///  position in screen coordinates.
        /// </summary>
        public static Point Position
        {
            get
            {
                User32.GetCursorPos(out Point p);
                return p;
            }
            set => User32.SetCursorPos(value.X, value.Y);
        }

        /// <summary>
        ///  Gets the size of this <see cref='Cursor'/> object.
        /// </summary>
        public Size Size
        {
            get
            {
                if (s_cursorSize.IsEmpty)
                {
                    s_cursorSize = new Size(
                        User32.GetSystemMetrics(User32.SystemMetric.SM_CXCURSOR),
                        User32.GetSystemMetrics(User32.SystemMetric.SM_CYCURSOR));
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

        /// <summary>
        ///  Duplicates this the Win32 handle of this <see cref='Cursor'/>.
        /// </summary>
        public IntPtr CopyHandle()
        {
            Size sz = Size;
            return User32.CopyImage(this, User32.IMAGE.CURSOR, sz.Width, sz.Height, User32.LR.DEFAULTCOLOR);
        }

        /// <summary>
        ///  Cleans up the resources allocated by this object.  Once called, the cursor
        ///  object is no longer useful.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_handle != IntPtr.Zero)
            {
                if (_ownHandle)
                {
                    User32.DestroyCursor(_handle);
                }
                _handle = IntPtr.Zero;
            }
        }

        /// <summary>
        ///  Draws this image to a graphics object.  The drawing command originates on the graphics
        ///  object, but a graphics object generally has no idea how to render a given image.  So,
        ///  it passes the call to the actual image.  This version crops the image to the given
        ///  dimensions and allows the user to specify a rectangle within the image to draw.
        /// </summary>
        // This method is way more powerful than what we expose, but I'll leave it in place.
        private void DrawImageCore(Graphics graphics, Rectangle imageRect, Rectangle targetRect, bool stretch)
        {
            if (graphics is null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            // Support GDI+ Translate method
            targetRect.X += (int)graphics.Transform.OffsetX;
            targetRect.Y += (int)graphics.Transform.OffsetY;

            IntPtr dc = graphics.GetHdc();

            // want finally clause to release dc
            try
            {
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
                        User32.DrawIcon(dc, targetX, targetY, this);
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
                        User32.DrawIcon(dc, targetX, targetY, this);
                        return;
                    }

                    drawWidth = cursorSize.Width;
                    drawHeight = cursorSize.Height;
                    clipWidth = targetWidth < imageWidth ? targetWidth : imageWidth;
                    clipHeight = targetHeight < imageHeight ? targetHeight : imageHeight;
                }

                // The ROP is SRCCOPY, so we can be simple here and take
                // advantage of clipping regions.  Drawing the cursor
                // is merely a matter of offsetting and clipping.
                Gdi32.IntersectClipRect(this, targetX, targetY, targetX + clipWidth, targetY + clipHeight);
                User32.DrawIconEx(
                    (Gdi32.HDC)dc,
                    targetX - imageX,
                    targetY - imageY,
                    this,
                    drawWidth,
                    drawHeight);

                // Let GDI+ restore clipping
                return;
            }
            finally
            {
                graphics.ReleaseHdcInternal(dc);
            }
        }

        /// <summary>
        ///  Draws this <see cref='Cursor'/> to a <see cref='Graphics'/>.
        /// </summary>
        public void Draw(Graphics g, Rectangle targetRect)
        {
            DrawImageCore(g, Rectangle.Empty, targetRect, false);
        }

        /// <summary>
        ///  Draws this <see cref='Cursor'/> to a <see cref='Graphics'/>.
        /// </summary>
        public void DrawStretched(Graphics g, Rectangle targetRect)
        {
            DrawImageCore(g, Rectangle.Empty, targetRect, true);
        }

        /// <summary>
        ///  Cleans up Windows resources for this object.
        /// </summary>
        ~Cursor()
        {
            Dispose(false);
        }

        /// <summary>
        ///  ISerializable private implementation
        /// </summary>
        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Hides the cursor. For every call to Cursor.hide() there must be a
        ///  balancing call to Cursor.show().
        /// </summary>
        public static void Hide() => User32.ShowCursor(BOOL.FALSE);

        // this code is adapted from Icon.GetIconSize please take this into account when changing this
        private Size GetIconSize(IntPtr iconHandle)
        {
            using User32.ICONINFO info = User32.GetIconInfo(iconHandle);
            if (!info.hbmColor.IsNull)
            {
                Gdi32.GetObjectW(info.hbmColor, out Gdi32.BITMAP bitmap);
                return new Size(bitmap.bmWidth, bitmap.bmHeight);
            }
            else if (!info.hbmMask.IsNull)
            {
                Gdi32.GetObjectW(info.hbmMask, out Gdi32.BITMAP bitmap);
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
        private void LoadPicture(Ole32.IStream stream, string paramName)
        {
            Debug.Assert(stream != null, "Stream should be validated before this method is called.");

            try
            {
                Guid iid = typeof(Ole32.IPicture).GUID;
                Ole32.IPicture picture = (Ole32.IPicture)Ole32.OleCreatePictureIndirect(ref iid);
                Ole32.IPersistStream ipictureAsIPersist = (Ole32.IPersistStream)picture;
                ipictureAsIPersist.Load(stream);

                if (picture != null && picture.Type == (short)Ole32.PICTYPE.ICON)
                {
                    IntPtr cursorHandle = (IntPtr)picture.Handle;
                    Size picSize = GetIconSize(cursorHandle);
                    if (DpiHelper.IsScalingRequired)
                    {
                        picSize = DpiHelper.LogicalToDeviceUnits(picSize);
                    }

                    _handle = User32.CopyImage(
                        cursorHandle,
                        User32.IMAGE.CURSOR,
                        picSize.Width,
                        picSize.Height,
                        User32.LR.DEFAULTCOLOR);

                    _ownHandle = true;
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
        internal byte[] GetData()
        {
            if (_resourceId != 0)
            {
                throw new FormatException(SR.CursorCannotCovertToBytes);
            }
            if (_cursorData is null)
            {
                throw new InvalidOperationException(SR.InvalidPictureFormat);
            }

            return (byte[])_cursorData.Clone();
        }

        /// <summary>
        ///  Displays the cursor. For every call to Cursor.show() there must have been
        ///  a previous call to Cursor.hide().
        /// </summary>
        public static void Show() => User32.ShowCursor(BOOL.TRUE);

        /// <summary>
        ///  Retrieves a human readable string representing this <see cref='Cursor'/>.
        /// </summary>
        public override string ToString()
        {
            string? s = null;

            if (!_ownHandle)
            {
                s = TypeDescriptor.GetConverter(typeof(Cursor)).ConvertToString(this);
            }
            else
            {
                s = base.ToString();
            }

            return $"[Cursor: {s}]";
        }

        public static bool operator ==(Cursor left, Cursor right)
        {
            if (right is null)
            {
                return left is null;
            }

            if (left is null)
            {
                return false;
            }

            return left._handle == right._handle;
        }

        public static bool operator !=(Cursor left, Cursor right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            // Handle is a 64-bit value in 64-bit machines, uncheck here to avoid overflow exceptions.
            return unchecked((int)_handle);
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Cursor))
            {
                return false;
            }
            return (this == (Cursor)obj);
        }
    }
}
