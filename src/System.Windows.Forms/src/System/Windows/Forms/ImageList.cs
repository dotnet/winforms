// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  The ImageList is an object that stores a collection of Images, most
    ///  commonly used by other controls, such as the ListView, TreeView, or
    ///  Toolbar.  You can add either bitmaps or Icons to the ImageList, and the
    ///  other controls will be able to use the Images as they desire.
    /// </summary>
    [
    Designer("System.Windows.Forms.Design.ImageListDesigner, " + AssemblyRef.SystemDesign),
    ToolboxItemFilter("System.Windows.Forms"),
    DefaultProperty(nameof(Images)),
    TypeConverter(typeof(ImageListConverter)),
    DesignerSerializer("System.Windows.Forms.Design.ImageListCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionImageList))
    ]
    public sealed class ImageList : Component
    {
        // gpr: Copied from Icon
        private static readonly Color fakeTransparencyColor = Color.FromArgb(0x0d, 0x0b, 0x0c);
        private static Size DefaultImageSize = new Size(16, 16);

        private const int INITIAL_CAPACITY = 4;
        private const int GROWBY = 4;

        private const int MAX_DIMENSION = 256;
        private static int maxImageWidth = MAX_DIMENSION;
        private static int maxImageHeight = MAX_DIMENSION;
        private static bool isScalingInitialized;

        private NativeImageList nativeImageList;

        // private int himlTemp;
        // private Bitmap temp = null;  // Used for drawing

        private ColorDepth colorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
        private Color transparentColor = Color.Transparent;
        private Size imageSize = DefaultImageSize;

        private ImageCollection imageCollection;

        private object userData;

        // The usual handle virtualization problem, with a new twist: image
        // lists are lossy.  At runtime, we delay handle creation as long as possible, and store
        // away the original images until handle creation (and hope no one disposes of the images!).  At design time, we keep the originals around indefinitely.
        // This variable will become null when the original images are lost.
        private IList /* of Original */ originals = new ArrayList();
        private EventHandler recreateHandler = null;
        private EventHandler changeHandler = null;

        private bool inAddRange = false;

        /// <summary>
        ///  Creates a new ImageList Control with a default image size of 16x16
        ///  pixels
        /// </summary>
        public ImageList()
        { // DO NOT DELETE -- AUTOMATION BP 1
            if (!isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    maxImageWidth = DpiHelper.LogicalToDeviceUnitsX(MAX_DIMENSION);
                    maxImageHeight = DpiHelper.LogicalToDeviceUnitsY(MAX_DIMENSION);
                }
                isScalingInitialized = true;
            }
        }

        /// <summary>
        ///  Creates a new ImageList Control with a default image size of 16x16
        ///  pixels and adds the ImageList to the passed in container.
        /// </summary>
        public ImageList(IContainer container) : this()
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        // This class is for classes that want to support both an ImageIndex
        // and ImageKey.  We want to toggle between using keys or indexes.
        // Default is to use the integer index.
        internal class Indexer
        {
            private string key = string.Empty;
            private int index = -1;
            private bool useIntegerIndex = true;
            private ImageList imageList = null;

            public virtual ImageList ImageList
            {
                get { return imageList; }
                set { imageList = value; }
            }

            public virtual string Key
            {
                get { return key; }
                set
                {
                    index = -1;
                    key = (value ?? string.Empty);
                    useIntegerIndex = false;
                }
            }

            public virtual int Index
            {
                get { return index; }
                set
                {
                    key = string.Empty;
                    index = value;
                    useIntegerIndex = true;
                }

            }

            public virtual int ActualIndex
            {
                get
                {
                    if (useIntegerIndex)
                    {
                        return Index;
                    }
                    else if (ImageList != null)
                    {
                        return ImageList.Images.IndexOfKey(Key);
                    }

                    return -1;
                }
            }
        }

        /// <summary>
        ///  Retrieves the color depth of the imagelist.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ImageListColorDepthDescr))
        ]
        public ColorDepth ColorDepth
        {
            get
            {
                return colorDepth;
            }
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

                if (colorDepth != value)
                {
                    colorDepth = value;
                    PerformRecreateHandle(nameof(ColorDepth));
                }
            }
        }

        private bool ShouldSerializeColorDepth()
        {
            return (Images.Count == 0);
        }
        private void ResetColorDepth()
        {
            ColorDepth = ColorDepth.Depth8Bit;
        }

        /// <summary>
        ///  The handle of the ImageList object.  This corresponds to a win32
        ///  HIMAGELIST Handle.
        /// </summary>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ImageListHandleDescr))
        ]
        public IntPtr Handle
        {
            get
            {
                if (nativeImageList == null)
                {
                    CreateHandle();
                }
                return nativeImageList.Handle;
            }
        }

        /// <summary>
        ///  Whether or not the underlying Win32 handle has been created.
        /// </summary>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ImageListHandleCreatedDescr))
        ]
        public bool HandleCreated
        {
            get { return nativeImageList != null; }
        }

        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(null),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ImageListImagesDescr)),
        MergableProperty(false)
        ]
        public ImageCollection Images
        {
            get
            {
                if (imageCollection == null)
                {
                    imageCollection = new ImageCollection(this);
                }

                return imageCollection;
            }
        }

        /// <summary>
        ///  Returns the size of the images in the ImageList
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ImageListSizeDescr))
        ]
        public Size ImageSize
        {
            get
            {
                return imageSize;
            }
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(ImageSize), "Size.Empty"), nameof(value));
                }

                // ImageList appears to consume an exponential amount of memory
                // based on image size x bpp.  Restrict this to a reasonable maximum
                // to keep people's systems from crashing.
                //
                if (value.Width <= 0 || value.Width > maxImageWidth)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, "ImageSize.Width", value.Width, 1, maxImageWidth));
                }

                if (value.Height <= 0 || value.Height > maxImageHeight)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, "ImageSize.Height", value.Height, 1, maxImageHeight));
                }

                if (imageSize.Width != value.Width || imageSize.Height != value.Height)
                {
                    imageSize = new Size(value.Width, value.Height);
                    PerformRecreateHandle(nameof(ImageSize));
                }
            }
        }

        private bool ShouldSerializeImageSize()
        {
            return (Images.Count == 0);
        }

        /// <summary>
        ///  Returns an ImageListStreamer, or null if the image list is empty.
        /// </summary>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DefaultValue(null),
        SRDescription(nameof(SR.ImageListImageStreamDescr))
        ]
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
                if (value != null)
                {

                    NativeImageList himl = value.GetNativeImageList();
                    if (himl != null && himl != nativeImageList)
                    {
                        bool recreatingHandle = HandleCreated;//We only need to fire RecreateHandle if there was a previous handle
                        DestroyHandle();
                        originals = null;
                        nativeImageList = new NativeImageList(SafeNativeMethods.ImageList_Duplicate(new HandleRef(himl, himl.Handle)));
                        if (SafeNativeMethods.ImageList_GetIconSize(new HandleRef(this, nativeImageList.Handle), out int x, out int y))
                        {
                            imageSize = new Size(x, y);
                        }
                        // need to get the image bpp
                        NativeMethods.IMAGEINFO imageInfo = new NativeMethods.IMAGEINFO(); // review? do I need to delete the mask and image?
                        if (SafeNativeMethods.ImageList_GetImageInfo(new HandleRef(this, nativeImageList.Handle), 0, imageInfo))
                        {
                            NativeMethods.BITMAP bmp = new NativeMethods.BITMAP();
                            UnsafeNativeMethods.GetObject(new HandleRef(null, imageInfo.hbmImage), Marshal.SizeOf(bmp), bmp);
                            switch (bmp.bmBitsPixel)
                            {
                                case 4:
                                    colorDepth = ColorDepth.Depth4Bit;
                                    break;
                                case 8:
                                    colorDepth = ColorDepth.Depth8Bit;
                                    break;
                                case 16:
                                    colorDepth = ColorDepth.Depth16Bit;
                                    break;
                                case 24:
                                    colorDepth = ColorDepth.Depth24Bit;
                                    break;
                                case 32:
                                    colorDepth = ColorDepth.Depth32Bit;
                                    break;
                                default:
                                    Debug.Fail("Unknown color depth");
                                    break;
                            }
                        }

                        Images.ResetKeys();
                        if (recreatingHandle)
                        {
                            OnRecreateHandle(EventArgs.Empty);
                        }
                    }
                }
                else
                {
                    DestroyHandle();
                    Images.Clear();
                }

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
        ///  The color to treat as transparent.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ImageListTransparentColorDescr))
        ]
        public Color TransparentColor
        {
            get
            {
                return transparentColor;
            }
            set
            {
                transparentColor = value;
            }
        }

        // Whether to use the transparent color, or rely on alpha instead
        private bool UseTransparentColor
        {
            get { return TransparentColor.A > 0; }
        }

        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        SRDescription(nameof(SR.ImageListOnRecreateHandleDescr))
        ]
        public event EventHandler RecreateHandle
        {
            add => recreateHandler += value;
            remove => recreateHandler -= value;
        }

        internal event EventHandler ChangeHandle
        {
            add => changeHandler += value;
            remove => changeHandler -= value;
        }

        //Creates a bitmap from the original image source..
        //

        private Bitmap CreateBitmap(Original original, out bool ownsBitmap)
        {
            Color transparent = transparentColor;
            ownsBitmap = false;
            if ((original.options & OriginalOptions.CustomTransparentColor) != 0)
            {
                transparent = original.customTransparentColor;
            }

            Bitmap bitmap;
            if (original.image is Bitmap)
            {
                bitmap = (Bitmap)original.image;
            }
            else if (original.image is Icon)
            {
                bitmap = ((Icon)original.image).ToBitmap();
                ownsBitmap = true;
            }
            else
            {
                bitmap = new Bitmap((Image)original.image);
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
            if ((original.options & OriginalOptions.ImageStrip) != 0)
            {
                // strip width must be a positive multiple of image list width
                if (size.Width == 0 || (size.Width % imageSize.Width) != 0)
                {
                    throw new ArgumentException(SR.ImageListStripBadWidth, "original");
                }

                if (size.Height != imageSize.Height)
                {
                    throw new ArgumentException(SR.ImageListImageTooShort, "original");
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
                int index = SafeNativeMethods.ImageList_ReplaceIcon(new HandleRef(this, Handle), -1, new HandleRef(icon, icon.Handle));
                if (index == -1)
                {
                    throw new InvalidOperationException(SR.ImageListAddFailed);
                }

                return index;
            }
            finally
            {
                if ((original.options & OriginalOptions.OwnsImage) != 0)
                { ///  this is to handle the case were we clone the icon (see WHY WHY WHY below)
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
            IntPtr hMask = ControlPaint.CreateHBitmapTransparencyMask(bitmap);   // Calls GDI to create Bitmap.
            IntPtr hBitmap = ControlPaint.CreateHBitmapColorMask(bitmap, hMask); // Calls GDI+ to create Bitmap. Need to add handle to HandleCollector.
            int index = SafeNativeMethods.ImageList_Add(new HandleRef(this, Handle), hBitmap, hMask);
            Gdi32.DeleteObject(hBitmap);
            Gdi32.DeleteObject(hMask);

            if (index == -1)
            {
                throw new InvalidOperationException(SR.ImageListAddFailed);
            }

            return index;
        }

        /// <summary>
        ///  Creates the underlying HIMAGELIST handle, and sets up all the
        ///  appropriate values with it.  Inheriting classes overriding this method
        ///  should not forget to call base.createHandle();
        /// </summary>
        private void CreateHandle()
        {
            Debug.Assert(nativeImageList == null, "Handle already created, this may be a source of temporary GDI leaks");

            int flags = NativeMethods.ILC_MASK;
            switch (colorDepth)
            {
                case ColorDepth.Depth4Bit:
                    flags |= NativeMethods.ILC_COLOR4;
                    break;
                case ColorDepth.Depth8Bit:
                    flags |= NativeMethods.ILC_COLOR8;
                    break;
                case ColorDepth.Depth16Bit:
                    flags |= NativeMethods.ILC_COLOR16;
                    break;
                case ColorDepth.Depth24Bit:
                    flags |= NativeMethods.ILC_COLOR24;
                    break;
                case ColorDepth.Depth32Bit:
                    flags |= NativeMethods.ILC_COLOR32;
                    break;
                default:
                    Debug.Fail("Unknown color depth in ImageList");
                    break;
            }

            // We enclose the imagelist handle create in a theming scope.
            IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();

            try
            {
                SafeNativeMethods.InitCommonControls();
                nativeImageList = new NativeImageList(SafeNativeMethods.ImageList_Create(imageSize.Width, imageSize.Height, flags, INITIAL_CAPACITY, GROWBY));
            }
            finally
            {
                UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
            }

            if (Handle == IntPtr.Zero)
            {
                throw new InvalidOperationException(SR.ImageListCreateFailed);
            }

            SafeNativeMethods.ImageList_SetBkColor(new HandleRef(this, Handle), NativeMethods.CLR_NONE);

            Debug.Assert(originals != null, "Handle not yet created, yet original images are gone");
            for (int i = 0; i < originals.Count; i++)
            {
                Original original = (Original)originals[i];
                if (original.image is Icon)
                {
                    AddIconToHandle(original, (Icon)original.image);
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
            originals = null;
        }

        // Don't merge this function into Dispose() -- that base.Dispose() will damage the design time experience
        private void DestroyHandle()
        {
            if (HandleCreated)
            {
                nativeImageList.Dispose();
                nativeImageList = null;
                originals = new ArrayList();
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
                if (originals != null)
                {
                    // we might own some of the stuff that's not been created yet
                    foreach (Original original in originals)
                    {
                        if ((original.options & OriginalOptions.OwnsImage) != 0)
                        {
                            ((IDisposable)original.image).Dispose();
                        }
                    }
                }
                DestroyHandle();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Draw the image indicated by the given index on the given Graphics
        ///  at the given location.
        /// </summary>
        public void Draw(Graphics g, Point pt, int index)
        {
            Draw(g, pt.X, pt.Y, index);
        }

        /// <summary>
        ///  Draw the image indicated by the given index on the given Graphics
        ///  at the given location.
        /// </summary>
        public void Draw(Graphics g, int x, int y, int index)
        {
            Draw(g, x, y, imageSize.Width, imageSize.Height, index);
        }

        /// <summary>
        ///  Draw the image indicated by the given index using the location, size
        ///  and raster op code specified.  The image is stretched or compressed as
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
                SafeNativeMethods.ImageList_DrawEx(new HandleRef(this, Handle), index, new HandleRef(g, dc), x, y,
                                       width, height, NativeMethods.CLR_NONE, NativeMethods.CLR_NONE, NativeMethods.ILD_TRANSPARENT);
            }
            finally
            {
                g.ReleaseHdcInternal(dc);
            }
        }

        private void CopyBitmapData(BitmapData sourceData, BitmapData targetData)
        {
            // do the actual copy
            int offsetSrc = 0;
            int offsetDest = 0;
            unsafe
            {
                for (int i = 0; i < targetData.Height; i++)
                {
                    IntPtr srcPtr, destPtr;
                    if (IntPtr.Size == 4)
                    {
                        srcPtr = new IntPtr(sourceData.Scan0.ToInt32() + offsetSrc);
                        destPtr = new IntPtr(targetData.Scan0.ToInt32() + offsetDest);
                    }
                    else
                    {
                        srcPtr = new IntPtr(sourceData.Scan0.ToInt64() + offsetSrc);
                        destPtr = new IntPtr(targetData.Scan0.ToInt64() + offsetDest);
                    }
                    UnsafeNativeMethods.CopyMemory(new HandleRef(this, destPtr), new HandleRef(this, srcPtr), Math.Abs(targetData.Stride));
                    offsetSrc += sourceData.Stride;
                    offsetDest += targetData.Stride;
                }
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
        ///  Returns the image specified by the given index.  The bitmap returned is a
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

                NativeMethods.IMAGEINFO imageInfo = new NativeMethods.IMAGEINFO(); // review? do I need to delete the mask and image inside of imageinfo?
                if (SafeNativeMethods.ImageList_GetImageInfo(new HandleRef(this, Handle), index, imageInfo))
                {
                    Bitmap tmpBitmap = null;
                    BitmapData bmpData = null;
                    BitmapData targetData = null;
                    try
                    {
                        tmpBitmap = Bitmap.FromHbitmap(imageInfo.hbmImage);
                        //

                        bmpData = tmpBitmap.LockBits(new Rectangle(imageInfo.rcImage_left, imageInfo.rcImage_top, imageInfo.rcImage_right - imageInfo.rcImage_left, imageInfo.rcImage_bottom - imageInfo.rcImage_top), ImageLockMode.ReadOnly, tmpBitmap.PixelFormat);

                        int offset = bmpData.Stride * imageSize.Height * index;
                        // we need do the following if the image has alpha because otherwise the image is fully transparent even though it has data
                        if (BitmapHasAlpha(bmpData))
                        {
                            result = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format32bppArgb);
                            targetData = result.LockBits(new Rectangle(0, 0, imageSize.Width, imageSize.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
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

            if (result == null)
            { // paint with the mask but no alpha...
                result = new Bitmap(imageSize.Width, imageSize.Height);

                Graphics graphics = Graphics.FromImage(result);
                try
                {
                    IntPtr dc = graphics.GetHdc();
                    try
                    {
                        SafeNativeMethods.ImageList_DrawEx(new HandleRef(this, Handle), index, new HandleRef(graphics, dc), 0, 0,
                                                imageSize.Width, imageSize.Height, NativeMethods.CLR_NONE, NativeMethods.CLR_NONE, NativeMethods.ILD_TRANSPARENT);

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

            // gpr: See Icon for description of fakeTransparencyColor
            if (result.RawFormat.Guid != ImageFormat.Icon.Guid)
            {
                result.MakeTransparent(fakeTransparencyColor);
            }
            return result;
        }

#if DEBUG_ONLY_APIS
        public Bitmap DebugOnly_GetMasterImage() {
            if (Images.Empty)
                return null;

            return Image.FromHBITMAP(GetImageInfo(0).hbmImage);
        }

        public Bitmap DebugOnly_GetMasterMask() {
            if (Images.Empty)
                return null;

            return Image.FromHBITMAP(GetImageInfo(0).hbmMask);
        }
#endif // DEBUG_ONLY_APIS

        /// <summary>
        ///  Called when the Handle property changes.
        /// </summary>
        private void OnRecreateHandle(EventArgs eventargs)
        {
            recreateHandler?.Invoke(this, eventargs);
        }

        private void OnChangeHandle(EventArgs eventargs)
        {
            changeHandler?.Invoke(this, eventargs);
        }

#if false
        /// <summary>
        ///  Copies the image at the specified index into the temporary Bitmap object.
        ///  The temporary Bitmap object is used for stuff that the Windows ImageList
        ///  control doesn't support, such as stretching images or copying images from
        ///  different image lists.  Since bitmap creation is expensive, the same instance
        ///  of the temporary Bitmap is reused.
        /// </summary>
        private void PutImageInTempBitmap(int index, bool useSnapshot) {
            Debug.Assert(!useSnapshot || himlTemp != 0, "Where's himlTemp?");

            IntPtr handleUse = (useSnapshot ? himlTemp : Handle);
            int count = SafeNativeMethods.ImageList_GetImageCount(handleUse);

            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));

            if (temp != null) {
                Size size = temp.Size;
                if (!temp.Size.Equals(imageSize)) {
                    temp.Dispose();
                    temp = null;
                }
            }
            if (temp == null) {
                temp = new Bitmap(imageSize.Width, imageSize.Height);
            }

            temp.Transparent = useMask;
            // OldGraphics gTemp = /*gpr useMask ? temp.ColorMask.GetGraphics() :*/ temp.GetGraphics();
            SafeNativeMethods.ImageList_DrawEx(handleUse, index, gTemp.Handle, 0, 0,
                                    imageSize.Width, imageSize.Height, useMask ? 0 : NativeMethods.CLR_DEFAULT, NativeMethods.CLR_NONE, NativeMethods.ILD_NORMAL);

            if (useMask) {
                gTemp = temp/*gpr .MonochromeMask*/.GetGraphics();
                SafeNativeMethods.ImageList_DrawEx(handleUse, index, gTemp.Handle, 0, 0, imageSize.Width, imageSize.Height, NativeMethods.CLR_DEFAULT, NativeMethods.CLR_NONE, NativeMethods.ILD_MASK);
            }
        }
#endif

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

            if (originals == null || Images.Empty)
            {
                originals = new ArrayList(); // spoof it into thinking this is the first CreateHandle
            }

            if (originals == null)
            {
                throw new InvalidOperationException(string.Format(SR.ImageListCantRecreate, reason));
            }

            DestroyHandle();
            CreateHandle();
            OnRecreateHandle(EventArgs.Empty);
        }

        private void ResetImageSize()
        {
            ImageSize = DefaultImageSize;
        }

        private void ResetTransparentColor()
        {
            TransparentColor = Color.LightGray;
        }

        private bool ShouldSerializeTransparentColor()
        {
            return !TransparentColor.Equals(Color.LightGray);
        }

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
            else
            {
                return s;
            }
        }

        internal class NativeImageList : IDisposable
        {
            private IntPtr himl;
#if DEBUG
            private readonly string callStack;
#endif

            internal NativeImageList(IntPtr himl)
            {
                this.himl = himl;
#if DEBUG
                callStack = Environment.StackTrace;
#endif
            }

            internal IntPtr Handle
            {
                get
                {
                    return himl;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void Dispose(bool disposing)
            {
                if (himl != IntPtr.Zero)
                {
                    SafeNativeMethods.ImageList_Destroy(new HandleRef(null, himl));
                    himl = IntPtr.Zero;
                }
            }

            ~NativeImageList()
            {
                Dispose(false);
            }

        }

        // An image before we add it to the image list, along with a few details about how to add it.
        private class Original
        {
            internal object image;
            internal OriginalOptions options;
            internal Color customTransparentColor = Color.Transparent;

            internal int nImages = 1;

            internal Original(object image, OriginalOptions options)
            : this(image, options, Color.Transparent)
            {
            }

            internal Original(object image, OriginalOptions options, int nImages)
            : this(image, options, Color.Transparent)
            {
                this.nImages = nImages;
            }

            internal Original(object image, OriginalOptions options, Color customTransparentColor)
            {
                Debug.Assert(image != null, "image is null");
                if (!(image is Icon) && !(image is Image))
                {
                    throw new InvalidOperationException(SR.ImageListEntryType);
                }
                this.image = image;
                this.options = options;
                this.customTransparentColor = customTransparentColor;
                if ((options & OriginalOptions.CustomTransparentColor) == 0)
                {
                    Debug.Assert(customTransparentColor.Equals(Color.Transparent),
                                 "Specified a custom transparent color then told us to ignore it");
                }
            }
        }

        [Flags]
        private enum OriginalOptions
        {
            Default = 0x00,

            ImageStrip = 0x01,
            CustomTransparentColor = 0x02,
            OwnsImage = 0x04
        }

        // Everything other than set_All, Add, and Clear will force handle creation.
        [
        Editor("System.Windows.Forms.Design.ImageCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        public sealed class ImageCollection : IList
        {
            private readonly ImageList owner;
            private readonly ArrayList imageInfoCollection = new ArrayList();

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /// <summary>
            ///  Returns the keys in the image list - images without keys return String.Empty.
                    /// </summary>
            public StringCollection Keys
            {
                get
                {
                    // pass back a copy of the current state.
                    StringCollection keysCollection = new StringCollection();

                    for (int i = 0; i < imageInfoCollection.Count; i++)
                    {
                        if ((imageInfoCollection[i] is ImageInfo image) && (image.Name != null) && (image.Name.Length != 0))
                        {
                            keysCollection.Add(image.Name);
                        }
                        else
                        {
                            keysCollection.Add(string.Empty);
                        }
                    }
                    return keysCollection;
                }
            }
            internal ImageCollection(ImageList owner)
            {
                this.owner = owner;
            }

            internal void ResetKeys()
            {
                if (imageInfoCollection != null)
                {
                    imageInfoCollection.Clear();
                }

                for (int i = 0; i < Count; i++)
                {
                    imageInfoCollection.Add(new ImageInfo());
                }
            }

            [Conditional("DEBUG")]
            private void AssertInvariant()
            {
                Debug.Assert(owner != null, "ImageCollection has no owner (ImageList)");
                Debug.Assert((owner.originals == null) == (owner.HandleCreated), " Either we should have the original images, or the handle should be created");
            }

            [Browsable(false)]
            public int Count
            {

                get
                {
                    Debug.Assert(owner != null, "ImageCollection has no owner (ImageList)");

                    if (owner.HandleCreated)
                    {
                        return SafeNativeMethods.ImageList_GetImageCount(new HandleRef(owner, owner.Handle));
                    }
                    else
                    {
                        int count = 0;
                        foreach (Original original in owner.originals)
                        {
                            if (original != null)
                            {
                                count += original.nImages;
                            }
                        }
                        return count;
                    }
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Determines if the ImageList has any images, without forcing a handle creation.
            /// </summary>
            public bool Empty
            {
                get
                {
                    return Count == 0;
                }
            }

            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Image this[int index]
            {

                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return owner.GetBitmap(index);
                }
                set
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    if (!(value is Bitmap))
                    {
                        throw new ArgumentException(SR.ImageListBitmap);
                    }

                    AssertInvariant();
                    Bitmap bitmap = (Bitmap)value;

                    bool ownsImage = false;
                    if (owner.UseTransparentColor && bitmap.RawFormat.Guid != ImageFormat.Icon.Guid)
                    {
                        // Since there's no ImageList_ReplaceMasked, we need to generate
                        // a transparent bitmap
                        Bitmap source = bitmap;
                        bitmap = (Bitmap)bitmap.Clone();
                        bitmap.MakeTransparent(owner.transparentColor);
                        ownsImage = true;
                    }

                    try
                    {
                        IntPtr hMask = ControlPaint.CreateHBitmapTransparencyMask(bitmap);
                        IntPtr hBitmap = ControlPaint.CreateHBitmapColorMask(bitmap, hMask);
                        bool ok = SafeNativeMethods.ImageList_Replace(new HandleRef(owner, owner.Handle), index, hBitmap, hMask);
                        Gdi32.DeleteObject(hBitmap);
                        Gdi32.DeleteObject(hMask);

                        if (!ok)
                        {
                            throw new InvalidOperationException(SR.ImageListReplaceFailed);
                        }
                    }
                    finally
                    {
                        if (ownsImage)
                        {
                            bitmap.Dispose();
                        }
                    }
                }
            }

            object IList.this[int index]
            {

                get
                {
                    return this[index];
                }
                set
                {
                    if (value is Image)
                    {
                        this[index] = (Image)value;
                    }
                    else
                    {
                        throw new ArgumentException(SR.ImageListBadImage, "value");
                    }
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public Image this[string key]
            {

                get
                {
                    // We do not support null and empty string as valid keys.
                    if ((key == null) || (key.Length == 0))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }

                }
            }

            /// <summary>
            ///  Adds an image to the end of the image list with a key accessor.
            /// </summary>
            public void Add(string key, Image image)
            {
                Debug.Assert((Count == imageInfoCollection.Count), "The count of these two collections should be equal.");

                // Store off the name.
                ImageInfo imageInfo = new ImageInfo
                {
                    Name = key
                };

                // Add the image to the IList
                Original original = new Original(image, OriginalOptions.Default);
                Add(original, imageInfo);

            }

            /// <summary>
            ///  Adds an icon to the end of the image list with a key accessor.
            /// </summary>
            public void Add(string key, Icon icon)
            {
                Debug.Assert((Count == imageInfoCollection.Count), "The count of these two collections should be equal.");

                // Store off the name.
                ImageInfo imageInfo = new ImageInfo
                {
                    Name = key
                };

                // Add the image to the IList
                Original original = new Original(icon, OriginalOptions.Default);
                Add(original, imageInfo);

            }

            int IList.Add(object value)
            {
                if (value is Image)
                {
                    Add((Image)value);
                    return Count - 1;
                }
                else
                {
                    throw new ArgumentException(SR.ImageListBadImage, "value");
                }
            }

            public void Add(Icon value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                Add(new Original(value.Clone(), OriginalOptions.OwnsImage), null); // WHY WHY WHY do we clone here...
                // changing it now is a breaking change, so we have to keep track of this specific icon and dispose that
            }

            /// <summary>
            ///  Add the given image to the ImageList.
            /// </summary>
            public void Add(Image value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                Original original = new Original(value, OriginalOptions.Default);
                Add(original, null);
            }

            /// <summary>
            ///  Add the given image to the ImageList, using the given color
            ///  to generate the mask. The number of images to add is inferred from
            ///  the width of the given image.
            /// </summary>
            public int Add(Image value, Color transparentColor)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                Original original = new Original(value, OriginalOptions.CustomTransparentColor,
                                                 transparentColor);
                return Add(original, null);
            }

            private int Add(Original original, ImageInfo imageInfo)
            {
                if (original == null || original.image == null)
                {
                    throw new ArgumentNullException(nameof(original));
                }

                int index = -1;

                AssertInvariant();

                if (original.image is Bitmap)
                {
                    if (owner.originals != null)
                    {
                        index = owner.originals.Add(original);
                    }

                    if (owner.HandleCreated)
                    {
                        Bitmap bitmapValue = owner.CreateBitmap(original, out bool ownsBitmap);
                        index = owner.AddToHandle(bitmapValue);
                        if (ownsBitmap)
                        {
                            bitmapValue.Dispose();
                        }
                    }
                }
                else if (original.image is Icon)
                {
                    if (owner.originals != null)
                    {
                        index = owner.originals.Add(original);
                    }
                    if (owner.HandleCreated)
                    {
                        index = owner.AddIconToHandle(original, (Icon)original.image);
                        // NOTE: if we own the icon (it's been created by us) this WILL dispose the icon to avoid a GDI leak
                        // **** original.image is NOT LONGER VALID AFTER THIS POINT ***
                    }
                }
                else
                {
                    throw new ArgumentException(SR.ImageListBitmap);
                }

                // update the imageInfoCollection
                // support AddStrip
                if ((original.options & OriginalOptions.ImageStrip) != 0)
                {
                    for (int i = 0; i < original.nImages; i++)
                    {
                        imageInfoCollection.Add(new ImageInfo());
                    }
                }
                else
                {
                    if (imageInfo == null)
                    {
                        imageInfo = new ImageInfo();
                    }

                    imageInfoCollection.Add(imageInfo);
                }

                if (!owner.inAddRange)
                {
                    owner.OnChangeHandle(EventArgs.Empty);
                }

                return index;
            }

            public void AddRange(Image[] images)
            {
                if (images == null)
                {
                    throw new ArgumentNullException(nameof(images));
                }
                owner.inAddRange = true;
                foreach (Image image in images)
                {
                    Add(image);
                }
                owner.inAddRange = false;
                owner.OnChangeHandle(EventArgs.Empty);
            }

            /// <summary>
            ///  Add an image strip the given image to the ImageList.  A strip is a single Image
            ///  which is treated as multiple images arranged side-by-side.
            /// </summary>
            public int AddStrip(Image value)
            {

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                // strip width must be a positive multiple of image list width
                //
                if (value.Width == 0 || (value.Width % owner.ImageSize.Width) != 0)
                {
                    throw new ArgumentException(SR.ImageListStripBadWidth, "value");
                }

                if (value.Height != owner.ImageSize.Height)
                {
                    throw new ArgumentException(SR.ImageListImageTooShort, "value");
                }

                int nImages = value.Width / owner.ImageSize.Width;

                Original original = new Original(value, OriginalOptions.ImageStrip, nImages);

                return Add(original, null);
            }

            /// <summary>
            ///  Remove all images and masks from the ImageList.
            /// </summary>
            public void Clear()
            {
                AssertInvariant();
                if (owner.originals != null)
                {
                    owner.originals.Clear();
                }

                imageInfoCollection.Clear();

                if (owner.HandleCreated)
                {
                    SafeNativeMethods.ImageList_Remove(new HandleRef(owner, owner.Handle), -1);
                }

                owner.OnChangeHandle(EventArgs.Empty);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool Contains(Image image)
            {
                throw new NotSupportedException();
            }

            bool IList.Contains(object image)
            {
                if (image is Image)
                {
                    return Contains((Image)image);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public int IndexOf(Image image)
            {
                throw new NotSupportedException();
            }

            int IList.IndexOf(object image)
            {
                if (image is Image)
                {
                    return IndexOf((Image)image);
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase,
            ///  if found; otherwise, -1.
            /// </summary>
            public int IndexOfKey(string key)
            {
                // Step 0 - Arg validation
                if ((key == null) || (key.Length == 0))
                {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if ((imageInfoCollection[lastAccessedIndex] != null) &&
                        (WindowsFormsUtils.SafeCompareStrings(((ImageInfo)imageInfoCollection[lastAccessedIndex]).Name, key, /* ignoreCase = */ true)))
                    {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if ((imageInfoCollection[i] != null) &&
                            (WindowsFormsUtils.SafeCompareStrings(((ImageInfo)imageInfoCollection[i]).Name, key, /* ignoreCase = */ true)))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return ((index >= 0) && (index < Count));
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                AssertInvariant();
                for (int i = 0; i < Count; ++i)
                {
                    dest.SetValue(owner.GetBitmap(i), index++);
                }
            }

            public IEnumerator GetEnumerator()
            {
                // Forces handle creation

                AssertInvariant();
                Image[] images = new Image[Count];
                for (int i = 0; i < images.Length; ++i)
                {
                    images[i] = owner.GetBitmap(i);
                }

                return images.GetEnumerator();
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Remove(Image image)
            {
                throw new NotSupportedException();
            }

            void IList.Remove(object image)
            {
                if (image is Image)
                {
                    Remove((Image)image);
                    owner.OnChangeHandle(EventArgs.Empty);
                }
            }

            public void RemoveAt(int index)
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                AssertInvariant();
                bool ok = SafeNativeMethods.ImageList_Remove(new HandleRef(owner, owner.Handle), index);
                if (!ok)
                {
                    throw new InvalidOperationException(SR.ImageListRemoveFailed);
                }
                else
                {
                    if ((imageInfoCollection != null) && (index >= 0 && index < imageInfoCollection.Count))
                    {
                        imageInfoCollection.RemoveAt(index);
                        owner.OnChangeHandle(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            ///  Removes the child control with the specified key.
            /// </summary>
            public void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            ///  Sets/Resets the key accessor for an image already in the image list.
            /// </summary>
            public void SetKeyName(int index, string name)
            {
                if (!IsValidIndex(index))
                {
                    throw new IndexOutOfRangeException(); //
                }

                if (imageInfoCollection[index] == null)
                {
                    imageInfoCollection[index] = new ImageInfo();
                }

                ((ImageInfo)imageInfoCollection[index]).Name = name;
            }

            internal class ImageInfo
            {
                private string name;
                public ImageInfo()
                {
                }

                public string Name
                {
                    get { return name; }
                    set { name = value; }
                }
            }

        } // end class ImageCollection
    }

    internal class ImageListConverter : ComponentConverter
    {
        public ImageListConverter() : base(typeof(ImageList))
        {
        }

        /// <summary>
        ///  Gets a value indicating
        ///  whether this object supports properties using the
        ///  specified context.
        /// </summary>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

