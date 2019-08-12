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
    public sealed class Cursor : IDisposable, ISerializable
    {
        private static Size cursorSize = System.Drawing.Size.Empty;

        private readonly byte[] cursorData;
        private IntPtr handle = IntPtr.Zero;       // handle to loaded image
        private bool ownHandle = true;
        private int resourceId = 0;

        private object userData;

        /// <summary>
        ///  Private constructor. If you want a standard system cursor, use one of the
        ///  definitions in the Cursors class.
        /// </summary>
        //
        internal Cursor(int nResourceId, int dummy)
        {
            LoadFromResourceId(nResourceId, nameof(nResourceId));
        }

        // Private constructor.  We have a private constructor here for
        // static cursors that are loaded through resources.  The only reason
        // to use the private constructor is so that we can assert, rather
        // than throw, if the cursor couldn't be loaded.  Why?  Because
        // throwing in <clinit/> is really rude and will prevent any of windows forms
        // from initializing.  This seems extreme just because we fail to
        // load a cursor.
        internal Cursor(string resource, int dummy)
        {
            Stream stream = typeof(Cursor).Module.Assembly.GetManifestResourceStream(typeof(Cursor), resource);
            Debug.Assert(stream != null, "couldn't get stream for resource " + resource);
            cursorData = new byte[stream.Length];
            stream.Read(cursorData, 0, Convert.ToInt32(stream.Length)); // we assume that a cursor is less than 4gig big
            LoadPicture(new Ole32.GPStream(new MemoryStream(cursorData)), nameof(resource));
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

            this.handle = handle;
            ownHandle = false;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Cursor'/>
        ///  class with
        ///  the specified filename.
        /// </summary>
        public Cursor(string fileName)
        {
            //Filestream demands the correct FILEIO access here
            //
            FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                cursorData = new byte[f.Length];
                f.Read(cursorData, 0, Convert.ToInt32(f.Length)); // assume that a cursor is less than 4gig...
            }
            finally
            {
                f.Close();
            }
            LoadPicture(new Ole32.GPStream(new MemoryStream(cursorData)), nameof(fileName));
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Cursor'/> class from the specified resource.
        /// </summary>
        public Cursor(Type type, string resource) : this((type?? throw new ArgumentNullException(nameof(type))).Module.Assembly.GetManifestResourceStream(type, resource))
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Cursor'/> class from the
        ///  specified data stream.
        /// </summary>
        public Cursor(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            cursorData = new byte[stream.Length];
            stream.Read(cursorData, 0, Convert.ToInt32(stream.Length));// assume that a cursor is less than 4gig...
            LoadPicture(new Ole32.GPStream(new MemoryStream(cursorData)), nameof(stream));
        }

        /// <summary>
        ///  Gets or
        ///  sets a <see cref='Rectangle'/> that represents the current clipping rectangle for this <see cref='Cursor'/> in
        ///  screen coordinates.
        /// </summary>
        public static Rectangle Clip
        {
            get
            {
                return ClipInternal;
            }
            set
            {
                ClipInternal = value;
            }
        }

        /// <summary>
        ///  Implemented separately to be used internally from safe places.
        /// </summary>
        internal static Rectangle ClipInternal
        {
            get
            {
                RECT r = new RECT();
                SafeNativeMethods.GetClipCursor(ref r);
                return r;
            }
            set
            {
                if (value.IsEmpty)
                {
                    UnsafeNativeMethods.ClipCursor(null);
                }
                else
                {
                    RECT rcClip = value;
                    UnsafeNativeMethods.ClipCursor(ref rcClip);
                }
            }
        }

        /// <summary>
        ///  Gets or
        ///  sets a <see cref='Cursor'/> that
        ///  represents the current mouse cursor. The value is NULL if the current mouse cursor is not visible.
        /// </summary>
        public static Cursor Current
        {
            get
            {
                IntPtr curHandle = SafeNativeMethods.GetCursor();
                if (curHandle == IntPtr.Zero)
                {
                    return null;
                }

                return new Cursor(curHandle);
            }
            set
            {
                IntPtr handle = (value == null) ? IntPtr.Zero : value.handle;
                UnsafeNativeMethods.SetCursor(new HandleRef(value, handle));
            }
        }

        /// <summary>
        ///  Gets
        ///  the Win32 handle for this <see cref='Cursor'/> .
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                if (handle == IntPtr.Zero)
                {
                    throw new ObjectDisposedException(string.Format(SR.ObjectDisposed, GetType().Name));
                }
                return handle;
            }
        }

        /// <summary>
        ///  Returns the "hot" location of the cursor.
        /// </summary>
        public Point HotSpot
        {
            get
            {
                NativeMethods.ICONINFO info = default;
                SafeNativeMethods.GetIconInfo(new HandleRef(this, Handle), ref info);

                // GetIconInfo creates bitmaps for the hbmMask and hbmColor members of ICONINFO.
                // The calling application must manage these bitmaps and delete them when they are no longer necessary.

                if (info.hbmMask != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(info.hbmMask);
                    info.hbmMask = IntPtr.Zero;
                }

                if (info.hbmColor != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(info.hbmColor);
                    info.hbmColor = IntPtr.Zero;
                }

                return new Point(info.xHotspot, info.yHotspot);
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
                UnsafeNativeMethods.GetCursorPos(out Point p);
                return p;
            }
            set => UnsafeNativeMethods.SetCursorPos(value.X, value.Y);
        }

        /// <summary>
        ///  Gets
        ///  the size of this <see cref='Cursor'/> object.
        /// </summary>
        public Size Size
        {
            get
            {
                if (cursorSize.IsEmpty)
                {
                    cursorSize = new Size(
                        User32.GetSystemMetrics(User32.SystemMetric.SM_CXCURSOR),
                        User32.GetSystemMetrics(User32.SystemMetric.SM_CYCURSOR));
                }

                return cursorSize;
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag
        {
            get
            {
                return userData;
            }
            set
            {
                userData = value;
            }
        }

        /// <summary>
        ///  Duplicates this the Win32 handle of this <see cref='Cursor'/>.
        /// </summary>
        public IntPtr CopyHandle()
        {
            Size sz = Size;
            return SafeNativeMethods.CopyImage(new HandleRef(this, Handle), NativeMethods.IMAGE_CURSOR, sz.Width, sz.Height, 0);
        }

        /// <summary>
        ///  Destroys the Win32 handle of this <see cref='Cursor'/>, if the
        /// <see cref='Cursor'/>
        ///  owns the handle
        /// </summary>
        private void DestroyHandle()
        {
            if (ownHandle)
            {
                UnsafeNativeMethods.DestroyCursor(new HandleRef(this, handle));
            }
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
            if (handle != IntPtr.Zero)
            {
                DestroyHandle();
                handle = IntPtr.Zero;
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
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            // Support GDI+ Translate method
            targetRect.X += (int)graphics.Transform.OffsetX;
            targetRect.Y += (int)graphics.Transform.OffsetY;

            int rop = 0xcc0020; // RasterOp.SOURCE.GetRop();
            IntPtr dc = graphics.GetHdc();

            try
            { // want finally clause to release dc
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
                //
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
                    // Short circuit the simple case of blasting an icon to the
                    // screen
                    //
                    if (targetWidth == imageWidth && targetHeight == imageHeight
                        && imageX == 0 && imageY == 0 && rop == NativeMethods.SRCCOPY
                        && imageWidth == cursorSize.Width && imageHeight == cursorSize.Height)
                    {
                        SafeNativeMethods.DrawIcon(new HandleRef(graphics, dc), targetX, targetY, new HandleRef(this, handle));
                        return;
                    }

                    drawWidth = cursorSize.Width * targetWidth / imageWidth;
                    drawHeight = cursorSize.Height * targetHeight / imageHeight;
                    clipWidth = targetWidth;
                    clipHeight = targetHeight;
                }
                else
                {
                    // Short circuit the simple case of blasting an icon to the
                    // screen
                    //
                    if (imageX == 0 && imageY == 0 && rop == NativeMethods.SRCCOPY
                        && cursorSize.Width <= targetWidth && cursorSize.Height <= targetHeight
                        && cursorSize.Width == imageWidth && cursorSize.Height == imageHeight)
                    {
                        SafeNativeMethods.DrawIcon(new HandleRef(graphics, dc), targetX, targetY, new HandleRef(this, handle));
                        return;
                    }

                    drawWidth = cursorSize.Width;
                    drawHeight = cursorSize.Height;
                    clipWidth = targetWidth < imageWidth ? targetWidth : imageWidth;
                    clipHeight = targetHeight < imageHeight ? targetHeight : imageHeight;
                }

                if (rop == NativeMethods.SRCCOPY)
                {
                    // The ROP is SRCCOPY, so we can be simple here and take
                    // advantage of clipping regions.  Drawing the cursor
                    // is merely a matter of offsetting and clipping.
                    //
                    SafeNativeMethods.IntersectClipRect(new HandleRef(this, Handle), targetX, targetY, targetX + clipWidth, targetY + clipHeight);
                    SafeNativeMethods.DrawIconEx(new HandleRef(graphics, dc), targetX - imageX, targetY - imageY,
                                       new HandleRef(this, handle), drawWidth, drawHeight, 0, NativeMethods.NullHandleRef, NativeMethods.DI_NORMAL);
                    // Let GDI+ restore clipping
                    return;
                }

                Debug.Fail("Cursor.Draw does not support raster ops.  How did you even pass one in?");
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
        public static void Hide()
        {
            UnsafeNativeMethods.ShowCursor(false);
        }

        private void LoadFromResourceId(int nResourceId, string paramName)
        {
            // We don't delete stock cursors.
            ownHandle = false;

            // This constructor is used during class initialization. By catching this
            // exception we prevent WinForms from failing to initialize if an error occurs.
            try
            {
                resourceId = nResourceId;
                handle = User32.LoadCursorW(IntPtr.Zero, nResourceId);
            }
            catch
            {
                handle = IntPtr.Zero;
            }
        }

        // this code is adapted from Icon.GetIconSize please take this into account when changing this
        private Size GetIconSize(IntPtr iconHandle)
        {
            Size iconSize = Size;

            NativeMethods.ICONINFO info = default;
            SafeNativeMethods.GetIconInfo(new HandleRef(this, iconHandle), ref info);
            NativeMethods.BITMAP bmp = new NativeMethods.BITMAP();

            if (info.hbmColor != IntPtr.Zero)
            {
                UnsafeNativeMethods.GetObject(new HandleRef(null, info.hbmColor), Marshal.SizeOf<NativeMethods.BITMAP>(), bmp);
                Gdi32.DeleteObject(info.hbmColor);
                iconSize = new Size(bmp.bmWidth, bmp.bmHeight);
            }
            else if (info.hbmMask != IntPtr.Zero)
            {
                UnsafeNativeMethods.GetObject(new HandleRef(null, info.hbmMask), Marshal.SizeOf<NativeMethods.BITMAP>(), bmp);
                iconSize = new Size(bmp.bmWidth, bmp.bmHeight / 2);
            }

            if (info.hbmMask != IntPtr.Zero)
            {
                Gdi32.DeleteObject(info.hbmMask);
            }
            return iconSize;
        }

        /// <summary>
        ///  Loads a picture from the requested stream.
        /// </summary>
        private void LoadPicture(Ole32.IStream stream, string paramName)
        {
            Debug.Assert(stream != null, "Stream should be validated before this method is called.");

            try
            {
                Guid g = typeof(UnsafeNativeMethods.IPicture).GUID;
                UnsafeNativeMethods.IPicture picture = null;

                try
                {
                    picture = UnsafeNativeMethods.OleCreateIPictureIndirect(null, ref g, true);
                    Ole32.IPersistStream ipictureAsIPersist = (Ole32.IPersistStream)picture;
                    ipictureAsIPersist.Load(stream);

                    if (picture != null && picture.GetPictureType() == NativeMethods.Ole.PICTYPE_ICON)
                    {
                        IntPtr cursorHandle = picture.GetHandle();
                        Size picSize = GetIconSize(cursorHandle);
                        if (DpiHelper.IsScalingRequired)
                        {
                            picSize = DpiHelper.LogicalToDeviceUnits(picSize);
                        }

                        handle = SafeNativeMethods.CopyImage(
                            new HandleRef(this, cursorHandle),
                            NativeMethods.IMAGE_CURSOR,
                            picSize.Width,
                            picSize.Height,
                            0);

                        ownHandle = true;
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(SR.InvalidPictureType, nameof(picture), nameof(Cursor)), paramName);
                    }
                }
                finally
                {
                    // destroy the picture...
                    if (picture != null)
                    {
                        Marshal.ReleaseComObject(picture);
                    }
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
            if (resourceId != 0)
            {
                throw new FormatException(SR.CursorCannotCovertToBytes);
            }
            if (cursorData == null)
            {
                throw new InvalidOperationException(SR.InvalidPictureFormat);
            }

            return (byte[])cursorData.Clone();
        }

        /// <summary>
        ///  Displays the cursor. For every call to Cursor.show() there must have been
        ///  a previous call to Cursor.hide().
        /// </summary>
        public static void Show()
        {
            UnsafeNativeMethods.ShowCursor(true);
        }

        /// <summary>
        ///  Retrieves a human readable string representing this
        ///  <see cref='Cursor'/>
        ///  .
        /// </summary>
        public override string ToString()
        {
            string s = null;

            if (!ownHandle)
            {
                s = TypeDescriptor.GetConverter(typeof(Cursor)).ConvertToString(this);
            }
            else
            {
                s = base.ToString();
            }

            return "[Cursor: " + s + "]";
        }

        public static bool operator ==(Cursor left, Cursor right)
        {
            if (left is null != right is null)
            {
                return false;
            }

            if (!(left is null))
            {
                return (left.handle == right.handle);
            }
            else
            {
                return true;
            }
        }

        public static bool operator !=(Cursor left, Cursor right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            // Handle is a 64-bit value in 64-bit machines, uncheck here to avoid overflow exceptions.
            return unchecked((int)handle);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cursor))
            {
                return false;
            }
            return (this == (Cursor)obj);
        }
    }
}
