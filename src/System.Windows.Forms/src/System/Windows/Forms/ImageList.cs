// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  The ImageList is an object that stores a collection of Images, most
    ///  commonly used by other controls, such as the ListView, TreeView, or
    ///  Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    ///  other controls will be able to use the Images as they desire.
    /// </summary>
    [Designer("System.Windows.Forms.Design.ImageListDesigner, " + AssemblyRef.SystemDesign)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [DefaultProperty(nameof(Images))]
    [TypeConverter(typeof(ImageListConverter))]
    [DesignerSerializer("System.Windows.Forms.Design.ImageListCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionImageList))]
    public sealed partial class ImageList : Component, IHandle
    {
        private static readonly Color s_fakeTransparencyColor = Color.FromArgb(0x0d, 0x0b, 0x0c);
        private static readonly Size s_defaultImageSize = new Size(16, 16);

        private const int MaxDimension = 256;
        private static int s_maxImageWidth = MaxDimension;
        private static int s_maxImageHeight = MaxDimension;
        private static bool s_isScalingInitialized;

        private NativeImageList _nativeImageList;

        private ColorDepth _colorDepth = ColorDepth.Depth8Bit;
        private Size _imageSize = s_defaultImageSize;

        private ImageCollection _imageCollection;

        // The usual handle virtualization problem, with a new twist: image
        // lists are lossy. At runtime, we delay handle creation as long as possible, and store
        // away the original images until handle creation (and hope no one disposes of the images!). At design time, we keep the originals around indefinitely.
        // This variable will become null when the original images are lost.
        private IList _originals = new ArrayList();
        private EventHandler _recreateHandler;
        private EventHandler _changeHandler;

        private bool _inAddRange;

        /// <summary>
        ///  Creates a new ImageList Control with a default image size of 16x16
        ///  pixels
        /// </summary>
        public ImageList()
        {
            if (!s_isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    s_maxImageWidth = DpiHelper.LogicalToDeviceUnitsX(MaxDimension);
                    s_maxImageHeight = DpiHelper.LogicalToDeviceUnitsY(MaxDimension);
                }
                s_isScalingInitialized = true;
            }
        }

        /// <summary>
        ///  Creates a new ImageList Control with a default image size of 16x16
        ///  pixels and adds the ImageList to the passed in container.
        /// </summary>
        public ImageList(IContainer container) : this()
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        /// <summary>
        ///  Retrieves the color depth of the imagelist.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ImageListColorDepthDescr))]
        public ColorDepth ColorDepth
        {
            get => _colorDepth;
            set
            {
                // ColorDepth is not conitguous - list the members instead.
                if (!ClientUtils.IsEnumValid_NotSequential(value,
                                                     (int)value,
                                                    (int)ColorDepth.Depth4Bit,
                                                    (int)ColorDepth.Depth8Bit,
                                                    (int)ColorDepth.Depth16Bit,
                                                    (int)ColorDepth.Depth24Bit,
                                                    (int)ColorDepth.Depth32Bit))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ColorDepth));
                }

                if (_colorDepth == value)
                {
                    return;
                }

                _colorDepth = value;
                PerformRecreateHandle(nameof(ColorDepth));
            }
        }

        private bool ShouldSerializeColorDepth() => Images.Count == 0;

        private void ResetColorDepth() => ColorDepth = ColorDepth.Depth8Bit;

        /// <summary>
        ///  The handle of the ImageList object. This corresponds to a win32
        ///  HIMAGELIST Handle.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ImageListHandleDescr))]
        public IntPtr Handle
        {
            get
            {
                if (_nativeImageList is null)
                {
                    CreateHandle();
                }
                return _nativeImageList.Handle;
            }
        }

        internal IntPtr CreateUniqueHandle()
        {
            if (_nativeImageList is null)
            {
                CreateHandle();
            }

            using var iml = _nativeImageList.Duplicate();
            return iml.TransferOwnership();
        }

        /// <summary>
        ///  Whether or not the underlying Win32 handle has been created.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ImageListHandleCreatedDescr))]
        public bool HandleCreated => _nativeImageList != null;

        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ImageListImagesDescr))]
        [MergableProperty(false)]
        public ImageCollection Images => _imageCollection ??= new ImageCollection(this);

        /// <summary>
        ///  Returns the size of the images in the ImageList
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [Localizable(true)]
        [SRDescription(nameof(SR.ImageListSizeDescr))]
        public Size ImageSize
        {
            get => _imageSize;
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(ImageSize), "Size.Empty"), nameof(value));
                }

                // ImageList appears to consume an exponential amount of memory
                // based on image size x bpp. Restrict this to a reasonable maximum
                // to keep people's systems from crashing.
                if (value.Width <= 0 || value.Width > s_maxImageWidth)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, "ImageSize.Width", value.Width, 1, s_maxImageWidth));
                }

                if (value.Height <= 0 || value.Height > s_maxImageHeight)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, "ImageSize.Height", value.Height, 1, s_maxImageHeight));
                }

                if (_imageSize.Width != value.Width || _imageSize.Height != value.Height)
                {
                    _imageSize = new Size(value.Width, value.Height);
                    PerformRecreateHandle(nameof(ImageSize));
                }
            }
        }

        private bool ShouldSerializeImageSize() => Images.Count == 0;

        /// <summary>
        ///  Returns an ImageListStreamer, or null if the image list is empty.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.ImageListImageStreamDescr))]
        public ImageListStreamer ImageStream
        {
            get
            {
                if (Images.Empty)
                {
                    return null;
                }

                // No need for us to create the handle, because any serious attempts to use the
                // ImageListStreamer will do it for us.
                return new ImageListStreamer(this);
            }
            set
            {
                if (value is null)
                {
                    DestroyHandle();
                    Images.Clear();
                    return;
                }

                NativeImageList himl = value.GetNativeImageList();
                if (himl != null && himl != _nativeImageList)
                {
                    bool recreatingHandle = HandleCreated; // We only need to fire RecreateHandle if there was a previous handle
                    DestroyHandle();
                    _originals = null;
                    _nativeImageList = himl.Duplicate();
                    if (ComCtl32.ImageList.GetIconSize(new HandleRef(this, _nativeImageList.Handle), out int x, out int y).IsTrue())
                    {
                        _imageSize = new Size(x, y);
                    }

                    // need to get the image bpp
                    var imageInfo = new ComCtl32.IMAGEINFO();
                    if (ComCtl32.ImageList.GetImageInfo(new HandleRef(this, _nativeImageList.Handle), 0, ref imageInfo).IsTrue())
                    {
                        Gdi32.GetObjectW(imageInfo.hbmImage, out Gdi32.BITMAP bmp);
                        _colorDepth = bmp.bmBitsPixel switch
                        {
                            4 => ColorDepth.Depth4Bit,
                            8 => ColorDepth.Depth8Bit,
                            16 => ColorDepth.Depth16Bit,
                            24 => ColorDepth.Depth24Bit,
                            32 => ColorDepth.Depth32Bit,
                            _ => _colorDepth
                        };
                    }

                    Images.ResetKeys();
                    if (recreatingHandle)
                    {
                        OnRecreateHandle(EventArgs.Empty);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        /// <summary>
        ///  The color to treat as transparent.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ImageListTransparentColorDescr))]
        public Color TransparentColor { get; set; } = Color.Transparent;

        private bool UseTransparentColor => TransparentColor.A > 0;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SRDescription(nameof(SR.ImageListOnRecreateHandleDescr))]
        public event EventHandler RecreateHandle
        {
            add => _recreateHandler += value;
            remove => _recreateHandler -= value;
        }

        internal event EventHandler ChangeHandle
        {
            add => _changeHandler += value;
            remove => _changeHandler -= value;
        }

        private Bitmap CreateBitmap(Original original, out bool ownsBitmap)
        {
            Color transparent = TransparentColor;
            ownsBitmap = false;
            if ((original._options & OriginalOptions.CustomTransparentColor) != 0)
            {
                transparent = original._customTransparentColor;
            }

            Bitmap bitmap;
            if (original._image is Bitmap)
            {
                bitmap = (Bitmap)original._image;
            }
            else if (original._image is Icon)
            {
                bitmap = ((Icon)original._image).ToBitmap();
                ownsBitmap = true;
            }
            else
            {
                bitmap = new Bitmap((Image)original._image);
                ownsBitmap = true;
            }

            if (transparent.A > 0)
            {
                // ImageList_AddMasked doesn't work on high color bitmaps,
                // so we always create the mask ourselves
                Bitmap source = bitmap;
                bitmap = (Bitmap)bitmap.Clone();
                bitmap.MakeTransparent(transparent);
                if (ownsBitmap)
                {
                    source.Dispose();
                }

                ownsBitmap = true;
            }

            Size size = bitmap.Size;
            if ((original._options & OriginalOptions.ImageStrip) != 0)
            {
                // strip width must be a positive multiple of image list width
                if (size.Width == 0 || (size.Width % _imageSize.Width) != 0)
                {
                    throw new ArgumentException(SR.ImageListStripBadWidth, nameof(original));
                }

                if (size.Height != _imageSize.Height)
                {
                    throw new ArgumentException(SR.ImageListImageTooShort, nameof(original));
                }
            }
            else if (!size.Equals(ImageSize))
            {
                Bitmap source = bitmap;
                bitmap = new Bitmap(source, ImageSize);
                if (ownsBitmap)
                {
                    source.Dispose();
                }

                ownsBitmap = true;
            }

            return bitmap;
        }

        private int AddIconToHandle(Original original, Icon icon)
        {
            try
            {
                Debug.Assert(HandleCreated, "Calling AddIconToHandle when there is no handle");
                int index = ComCtl32.ImageList.ReplaceIcon(this, -1, new HandleRef(icon, icon.Handle));
                if (index == -1)
                {
                    throw new InvalidOperationException(SR.ImageListAddFailed);
                }

                return index;
            }
            finally
            {
                if ((original._options & OriginalOptions.OwnsImage) != 0)
                {
                    // This is to handle the case were we clone the icon (see why below)
                    icon.Dispose();
                }
            }
        }

        /// <summary>
        ///  Add the given <paramref name="bitmap"/> to the <see cref="ImageList"/> handle.
        /// </summary>
        private int AddToHandle(Bitmap bitmap)
        {
            Debug.Assert(HandleCreated, "Calling AddToHandle when there is no handle");

            // Calls GDI to create Bitmap.
            Gdi32.HBITMAP hMask = (Gdi32.HBITMAP)ControlPaint.CreateHBitmapTransparencyMask(bitmap);

            // Calls GDI+ to create Bitmap
            Gdi32.HBITMAP hBitmap = (Gdi32.HBITMAP)ControlPaint.CreateHBitmapColorMask(bitmap, (IntPtr)hMask);

            int index;
            try
            {
                index = ComCtl32.ImageList.Add(this, hBitmap, hMask);
            }
            finally
            {
                Gdi32.DeleteObject(hBitmap);
                Gdi32.DeleteObject(hMask);
            }

            if (index == -1)
            {
                throw new InvalidOperationException(SR.ImageListAddFailed);
            }

            return index;
        }

        /// <summary>
        ///  Creates the underlying HIMAGELIST handle, and sets up all the
        ///  appropriate values with it. Inheriting classes overriding this method
        ///  should not forget to call base.createHandle();
        /// </summary>
        private void CreateHandle()
        {
            Debug.Assert(_nativeImageList is null, "Handle already created, this may be a source of temporary GDI leaks");

            ComCtl32.ILC flags = ComCtl32.ILC.MASK;
            switch (_colorDepth)
            {
                case ColorDepth.Depth4Bit:
                    flags |= ComCtl32.ILC.COLOR4;
                    break;
                case ColorDepth.Depth8Bit:
                    flags |= ComCtl32.ILC.COLOR8;
                    break;
                case ColorDepth.Depth16Bit:
                    flags |= ComCtl32.ILC.COLOR16;
                    break;
                case ColorDepth.Depth24Bit:
                    flags |= ComCtl32.ILC.COLOR24;
                    break;
                case ColorDepth.Depth32Bit:
                    flags |= ComCtl32.ILC.COLOR32;
                    break;
                default:
                    Debug.Fail("Unknown color depth in ImageList");
                    break;
            }

            // We enclose the imagelist handle create in a theming scope.
            IntPtr userCookie = ThemingScope.Activate(Application.UseVisualStyles);

            try
            {
                ComCtl32.InitCommonControls();

                if (_nativeImageList != null)
                {
                    _nativeImageList.Dispose();
                    _nativeImageList = null;
                }

                _nativeImageList = new NativeImageList(_imageSize, flags);
            }
            finally
            {
                ThemingScope.Deactivate(userCookie);
            }

            ComCtl32.ImageList.SetBkColor(this, ComCtl32.CLR.NONE);

            Debug.Assert(_originals != null, "Handle not yet created, yet original images are gone");
            for (int i = 0; i < _originals.Count; i++)
            {
                Original original = (Original)_originals[i];
                if (original._image is Icon)
                {
                    AddIconToHandle(original, (Icon)original._image);
                    // NOTE: if we own the icon (it's been created by us) this WILL dispose the icon to avoid a GDI leak
                    // **** original.image is NOT LONGER VALID AFTER THIS POINT ***
                }
                else
                {
                    Bitmap bitmapValue = CreateBitmap(original, out bool ownsBitmap);
                    AddToHandle(bitmapValue);
                    if (ownsBitmap)
                    {
                        bitmapValue.Dispose();
                    }
                }
            }
            _originals = null;
        }

        // Don't merge this function into Dispose() -- that base.Dispose() will damage the design time experience
        private void DestroyHandle()
        {
            if (HandleCreated)
            {
                _nativeImageList.Dispose();
                _nativeImageList = null;
                _originals = new ArrayList();
            }
        }

        /// <summary>
        ///  Releases the unmanaged resources used by the <see cref="ImageList" />
        ///  and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///  <see langword="true" /> to release both managed and unmanaged resources;
        ///  <see langword="false" /> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_originals != null)
                {
                    // we might own some of the stuff that's not been created yet
                    foreach (Original original in _originals)
                    {
                        if ((original._options & OriginalOptions.OwnsImage) != 0)
                        {
                            ((IDisposable)original._image).Dispose();
                        }
                    }
                }

                ImageStream?.Dispose();

                DestroyHandle();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Draw the image indicated by the given index on the given Graphics
        ///  at the given location.
        /// </summary>
        public void Draw(Graphics g, Point pt, int index) => Draw(g, pt.X, pt.Y, index);

        /// <summary>
        ///  Draw the image indicated by the given index on the given Graphics
        ///  at the given location.
        /// </summary>
        public void Draw(Graphics g, int x, int y, int index) => Draw(g, x, y, _imageSize.Width, _imageSize.Height, index);

        /// <summary>
        ///  Draw the image indicated by the given index using the location, size
        ///  and raster op code specified. The image is stretched or compressed as
        ///  necessary to fit the bounds provided.
        /// </summary>
        public void Draw(Graphics g, int x, int y, int width, int height, int index)
        {
            if (index < 0 || index >= Images.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            IntPtr dc = g.GetHdc();
            try
            {
                ComCtl32.ImageList.DrawEx(
                    this,
                    index,
                    new HandleRef(g, dc),
                    x,
                    y,
                    width,
                    height,
                    ComCtl32.CLR.NONE,
                    ComCtl32.CLR.NONE,
                    ComCtl32.ILD.TRANSPARENT);
            }
            finally
            {
                g.ReleaseHdcInternal(dc);
            }
        }

        private unsafe void CopyBitmapData(BitmapData sourceData, BitmapData targetData)
        {
            Debug.Assert(Image.GetPixelFormatSize(sourceData.PixelFormat) == 32);
            Debug.Assert(Image.GetPixelFormatSize(sourceData.PixelFormat) == Image.GetPixelFormatSize(targetData.PixelFormat));
            Debug.Assert(targetData.Width == sourceData.Width);
            Debug.Assert(targetData.Height == sourceData.Height);
            Debug.Assert(targetData.Stride == targetData.Width * 4);

            // do the actual copy
            int offsetSrc = 0;
            int offsetDest = 0;
            for (int i = 0; i < targetData.Height; i++)
            {
                IntPtr srcPtr = sourceData.Scan0 + offsetSrc;
                IntPtr destPtr = targetData.Scan0 + offsetDest;
                int length = Math.Abs(targetData.Stride);
                Buffer.MemoryCopy(srcPtr.ToPointer(), destPtr.ToPointer(), length, length);
                offsetSrc += sourceData.Stride;
                offsetDest += targetData.Stride;
            }
        }

        private static bool BitmapHasAlpha(BitmapData bmpData)
        {
            if (bmpData.PixelFormat != PixelFormat.Format32bppArgb && bmpData.PixelFormat != PixelFormat.Format32bppRgb)
            {
                return false;
            }
            bool hasAlpha = false;
            unsafe
            {
                for (int i = 0; i < bmpData.Height; i++)
                {
                    int offsetRow = i * bmpData.Stride;
                    for (int j = 3; j < bmpData.Width * 4; j += 4)
                    { // *4 is safe since we know PixelFormat is ARGB
                        unsafe
                        {
                            byte* candidate = ((byte*)bmpData.Scan0.ToPointer()) + offsetRow + j;
                            if (*candidate != 0)
                            {
                                hasAlpha = true;
                                goto Found; // gotos are not the best, but it's the best thing here...
                            }
                        }
                    }
                }
            Found:
                return hasAlpha;
            }
        }

        /// <summary>
        ///  Returns the image specified by the given index. The bitmap returned is a
        ///  copy of the original image.
        /// </summary>
        // NOTE: forces handle creation, so doesn't return things from the original list

        private Bitmap GetBitmap(int index)
        {
            if (index < 0 || index >= Images.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            Bitmap result = null;

            // if the imagelist is 32bpp, if the image slot at index
            // has valid alpha information (not all zero... which is cause by windows just painting RGB values
            // and not touching the alpha byte for images < 32bpp painted to a 32bpp imagelist)
            // we're not using the mask. That means that
            // we can just get the whole image strip, cut out the piece that we want
            // and return that, that way we don't flatten the alpha by painting the value with the alpha... (ie using the alpha)

            if (ColorDepth == ColorDepth.Depth32Bit)
            {
                var imageInfo = new ComCtl32.IMAGEINFO();
                if (ComCtl32.ImageList.GetImageInfo(new HandleRef(this, Handle), index, ref imageInfo).IsTrue())
                {
                    Bitmap tmpBitmap = null;
                    BitmapData bmpData = null;
                    BitmapData targetData = null;
                    try
                    {
                        tmpBitmap = Bitmap.FromHbitmap((IntPtr)imageInfo.hbmImage);

                        bmpData = tmpBitmap.LockBits(new Rectangle(imageInfo.rcImage.left, imageInfo.rcImage.top, imageInfo.rcImage.right - imageInfo.rcImage.left, imageInfo.rcImage.bottom - imageInfo.rcImage.top), ImageLockMode.ReadOnly, tmpBitmap.PixelFormat);

                        int offset = bmpData.Stride * _imageSize.Height * index;
                        // we need do the following if the image has alpha because otherwise the image is fully transparent even though it has data
                        if (BitmapHasAlpha(bmpData))
                        {
                            result = new Bitmap(_imageSize.Width, _imageSize.Height, PixelFormat.Format32bppArgb);
                            targetData = result.LockBits(new Rectangle(0, 0, _imageSize.Width, _imageSize.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                            CopyBitmapData(bmpData, targetData);
                        }
                    }
                    finally
                    {
                        if (tmpBitmap != null)
                        {
                            if (bmpData != null)
                            {
                                tmpBitmap.UnlockBits(bmpData);
                            }

                            tmpBitmap.Dispose();
                        }
                        if (result != null && targetData != null)
                        {
                            result.UnlockBits(targetData);
                        }
                    }
                }
            }

            if (result is null)
            {
                // Paint with the mask but no alpha.
                result = new Bitmap(_imageSize.Width, _imageSize.Height);

                Graphics graphics = Graphics.FromImage(result);
                try
                {
                    IntPtr dc = graphics.GetHdc();
                    try
                    {
                        ComCtl32.ImageList.DrawEx(
                            this,
                            index,
                            new HandleRef(graphics, dc),
                            0,
                            0,
                            _imageSize.Width,
                            _imageSize.Height,
                            ComCtl32.CLR.NONE,
                            ComCtl32.CLR.NONE,
                            ComCtl32.ILD.TRANSPARENT);
                    }
                    finally
                    {
                        graphics.ReleaseHdcInternal(dc);
                    }
                }
                finally
                {
                    graphics.Dispose();
                }
            }

            // See Icon for description of fakeTransparencyColor
            if (result.RawFormat.Guid != ImageFormat.Icon.Guid)
            {
                result.MakeTransparent(s_fakeTransparencyColor);
            }

            return result;
        }

        /// <summary>
        ///  Called when the Handle property changes.
        /// </summary>
        private void OnRecreateHandle(EventArgs eventargs) => _recreateHandler?.Invoke(this, eventargs);

        private void OnChangeHandle(EventArgs eventargs) => _changeHandler?.Invoke(this, eventargs);

        // PerformRecreateHandle doesn't quite do what you would suspect.
        // Any existing images in the imagelist will NOT be copied to the
        // new image list -- they really should.

        // The net effect is that if you add images to an imagelist, and
        // then e.g. change the ImageSize any existing images will be lost
        // and you will have to add them back. This is probably a corner case
        // but it should be mentioned.
        //
        // The fix isn't as straightforward as you might think, i.e. we
        // cannot just blindly store off the images and copy them into
        // the newly created imagelist. E.g. say you change the ColorDepth
        // from 8-bit to 32-bit. Just copying the 8-bit images would be wrong.
        // Therefore we are going to leave this as is. Users should make sure
        // to set these properties before actually adding the images.

        // The Designer works around this by shadowing any Property that ends
        // up calling PerformRecreateHandle (ImageSize, ColorDepth, ImageStream).

        // Thus, if you add a new Property to ImageList which ends up calling
        // PerformRecreateHandle, you must shadow the property in ImageListDesigner.
        private void PerformRecreateHandle(string reason)
        {
            if (!HandleCreated)
            {
                return;
            }

            if (_originals is null || Images.Empty)
            {
                // spoof it into thinking this is the first CreateHandle
                _originals = new ArrayList();
            }

            DestroyHandle();
            CreateHandle();
            OnRecreateHandle(EventArgs.Empty);
        }

        private void ResetImageSize() => ImageSize = s_defaultImageSize;

        private void ResetTransparentColor() => TransparentColor = Color.LightGray;

        private bool ShouldSerializeTransparentColor() => !TransparentColor.Equals(Color.LightGray);

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            if (Images != null)
            {
                return s + " Images.Count: " + Images.Count.ToString(CultureInfo.CurrentCulture) + ", ImageSize: " + ImageSize.ToString();
            }

            return s;
        }
    }
}
