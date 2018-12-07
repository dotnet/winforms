// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System;
    using System.Collections.Specialized;
    using System.Collections;
    using System.Drawing;   
    using System.Drawing.Imaging;  
    using System.Drawing.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.IO;
    using System.ComponentModel.Design.Serialization;
    using System.Runtime.Versioning;

    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;
    using System.Globalization;

    /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList"]/*' />
    /// <devdoc>
    ///     The ImageList is an object that stores a collection of Images, most
    ///     commonly used by other controls, such as the ListView, TreeView, or
    ///     Toolbar.  You can add either bitmaps or Icons to the ImageList, and the
    ///     other controls will be able to use the Images as they desire.
    /// </devdoc>
    [
    Designer("System.Windows.Forms.Design.ImageListDesigner, " + AssemblyRef.SystemDesign),
    ToolboxItemFilter("System.Windows.Forms"),
    DefaultProperty(nameof(Images)),
    TypeConverter(typeof(ImageListConverter)),
    DesignerSerializer("System.Windows.Forms.Design.ImageListCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionImageList))
    ]
    public sealed class ImageList : Component {

        // gpr: Copied from Icon
        private static Color fakeTransparencyColor = Color.FromArgb(0x0d, 0x0b, 0x0c);
        private static Size  DefaultImageSize = new Size(16, 16);

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

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageList"]/*' />
        /// <devdoc>
        ///     Creates a new ImageList Control with a default image size of 16x16
        ///     pixels
        /// </devdoc>
        public ImageList() { // DO NOT DELETE -- AUTOMATION BP 1
            if (!isScalingInitialized) {
                if (DpiHelper.IsScalingRequired) {
                    maxImageWidth = DpiHelper.LogicalToDeviceUnitsX(MAX_DIMENSION);
                    maxImageHeight = DpiHelper.LogicalToDeviceUnitsY(MAX_DIMENSION);
                }
                isScalingInitialized = true;
            }
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageList1"]/*' />
        /// <devdoc>
        ///     Creates a new ImageList Control with a default image size of 16x16
        ///     pixels and adds the ImageList to the passed in container.
        /// </devdoc>
        public ImageList(IContainer container) : this() {
            if (container == null) {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        // This class is for classes that want to support both an ImageIndex
        // and ImageKey.  We want to toggle between using keys or indexes.
        // Default is to use the integer index.
        internal class Indexer {
            private string key = String.Empty;
            private int index = -1;
            private bool useIntegerIndex = true;
            private ImageList imageList = null;

            public virtual ImageList ImageList {
                get { return imageList; }
                set { imageList = value; }
            }

            public virtual string Key {
                get { return key; }
                set {
                    index = -1;
                    key = (value == null ? String.Empty : value);
                    useIntegerIndex = false;
                }
            }

            public virtual int Index {
                get { return index; }
                set {
                    key = String.Empty;
                    index = value;
                    useIntegerIndex = true;
                }

            }

            public virtual int ActualIndex {
                get {
                    if (useIntegerIndex) {
                        return Index;
                    }
                    else if (ImageList != null) {
                        return ImageList.Images.IndexOfKey(Key);
                    }

                    return -1;
                }
            }
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ColorDepth"]/*' />
        /// <devdoc>
        ///     Retrieves the color depth of the imagelist.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ImageListColorDepthDescr))
        ]
        public ColorDepth ColorDepth {
            get {
                return colorDepth;
            }
            set {
                // ColorDepth is not conitguous - list the members instead.
                if (!ClientUtils.IsEnumValid_NotSequential(value, 
                                                     (int)value,
                                                    (int)ColorDepth.Depth4Bit,
                                                    (int)ColorDepth.Depth8Bit,
                                                    (int)ColorDepth.Depth16Bit,
                                                    (int)ColorDepth.Depth24Bit,
                                                    (int)ColorDepth.Depth32Bit)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ColorDepth));
                }

                if (colorDepth != value) {
                    colorDepth = value;
                    PerformRecreateHandle("ColorDepth");
                }
            }
        }

        private bool ShouldSerializeColorDepth() {
            return (Images.Count==0);
        }
        private void ResetColorDepth() {
            ColorDepth = ColorDepth.Depth8Bit;
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.Handle"]/*' />
        /// <devdoc>
        ///     The handle of the ImageList object.  This corresponds to a win32
        ///     HIMAGELIST Handle.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ImageListHandleDescr))
        ]
        public IntPtr Handle {
            get {
                if (nativeImageList == null) {
                    CreateHandle();
                }
                return nativeImageList.Handle;
            }
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.HandleCreated"]/*' />
        /// <devdoc>
        ///     Whether or not the underlying Win32 handle has been created.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ImageListHandleCreatedDescr))
        ]
        public bool HandleCreated {
            get { return nativeImageList != null; }
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.Images"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(null),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ImageListImagesDescr)),
        MergableProperty(false)
        ]
        public ImageCollection Images {
            get {
                if (imageCollection == null)
                    imageCollection = new ImageCollection(this);
                return imageCollection;
            }
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageSize"]/*' />
        /// <devdoc>
        ///     Returns the size of the images in the ImageList
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ImageListSizeDescr))
        ]
        public Size ImageSize {
            get {
                return imageSize;
            }
            set {
                if (value.IsEmpty) {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "ImageSize", "Size.Empty"));
                }

                // ImageList appears to consume an exponential amount of memory
                // based on image size x bpp.  Restrict this to a reasonable maximum
                // to keep people's systems from crashing.
                //
                if (value.Width <= 0 || value.Width > maxImageWidth) {
                    throw new ArgumentOutOfRangeException(nameof(ImageSize), string.Format(SR.InvalidBoundArgument, "ImageSize.Width", value.Width.ToString(CultureInfo.CurrentCulture), (1).ToString(CultureInfo.CurrentCulture), maxImageWidth.ToString()));
                }

                if (value.Height <= 0 || value.Height > maxImageHeight) {
                    throw new ArgumentOutOfRangeException(nameof(ImageSize), string.Format(SR.InvalidBoundArgument, "ImageSize.Height", value.Height.ToString(CultureInfo.CurrentCulture), (1).ToString(CultureInfo.CurrentCulture), maxImageHeight.ToString()));
                }

                if (imageSize.Width != value.Width || imageSize.Height != value.Height) {
                    imageSize = new Size(value.Width, value.Height);
                    PerformRecreateHandle("ImageSize");
                }
            }
        }

        private bool ShouldSerializeImageSize() {
            return (Images.Count==0);
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageStream"]/*' />
        /// <devdoc>
        ///     Returns an ImageListStreamer, or null if the image list is empty.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DefaultValue(null),
        SRDescription(nameof(SR.ImageListImageStreamDescr))
        ]
        public ImageListStreamer ImageStream {
            get {
                if (Images.Empty)
                    return null;

                // No need for us to create the handle, because any serious attempts to use the
                // ImageListStreamer will do it for us.
                return new ImageListStreamer(this);
            }
            set {
                if (value != null) {

                    NativeImageList himl = value.GetNativeImageList();
                    if (himl != null && himl != this.nativeImageList) {
                        bool recreatingHandle = this.HandleCreated;//We only need to fire RecreateHandle if there was a previous handle
                        DestroyHandle();
                        originals = null;
                        this.nativeImageList = new NativeImageList(SafeNativeMethods.ImageList_Duplicate(new HandleRef(himl, himl.Handle)));
                        int x, y;
                        if(SafeNativeMethods.ImageList_GetIconSize(new HandleRef(this, this.nativeImageList.Handle), out x, out y)) {
                            imageSize = new Size(x,y);
                        }
                        // need to get the image bpp
                        NativeMethods.IMAGEINFO imageInfo = new NativeMethods.IMAGEINFO(); // review? do I need to delete the mask and image?
                        if(SafeNativeMethods.ImageList_GetImageInfo(new HandleRef(this, this.nativeImageList.Handle), 0, imageInfo)) {
                            NativeMethods.BITMAP bmp = new NativeMethods.BITMAP();
                            UnsafeNativeMethods.GetObject(new HandleRef(null, imageInfo.hbmImage), Marshal.SizeOf(bmp), bmp);
                            switch(bmp.bmBitsPixel) {
                                case 4:
                                    colorDepth =  ColorDepth.Depth4Bit;
                                    break;
                                case 8:
                                    colorDepth =  ColorDepth.Depth8Bit;
                                    break;
                                case 16:
                                    colorDepth =  ColorDepth.Depth16Bit;
                                    break;
                                case 24:
                                    colorDepth =  ColorDepth.Depth24Bit;
                                    break;
                                case 32:
                                    colorDepth =  ColorDepth.Depth32Bit;
                                    break;
                                default:
                                    Debug.Fail("Unknown color depth");
                                    break;
                            }
                        }

                        Images.ResetKeys();
                        if (recreatingHandle) {
                            OnRecreateHandle(new EventArgs());
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

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.TransparentColor"]/*' />
        /// <devdoc>
        ///     The color to treat as transparent.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ImageListTransparentColorDescr))
        ]
        public Color TransparentColor {
            get {
                return transparentColor;
            }
            set {
                transparentColor = value;
            }
        }

        // Whether to use the transparent color, or rely on alpha instead
        private bool UseTransparentColor {
            get { return TransparentColor.A > 0;}
        }


        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.RecreateHandle"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        SRDescription(nameof(SR.ImageListOnRecreateHandleDescr))
        ]
        public event EventHandler RecreateHandle {
            add {
                recreateHandler += value;
            }
            remove {
                recreateHandler -= value;
            }
        }

        internal event EventHandler ChangeHandle {
            add {
                changeHandler += value;
            }
            remove {
                changeHandler -= value;
            }
        }

        //Creates a bitmap from the original image source..
        //

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        private Bitmap CreateBitmap(Original original, out bool ownsBitmap) {
            Color transparent = transparentColor;
            ownsBitmap = false;
            if ((original.options & OriginalOptions.CustomTransparentColor) != 0)
                transparent = original.customTransparentColor;

            Bitmap bitmap;
            if (original.image is Bitmap) {
                bitmap = (Bitmap) original.image;
            }
            else if (original.image is Icon) {
                bitmap = ((Icon)original.image).ToBitmap();
                ownsBitmap = true;
            }
            else {
                bitmap = new Bitmap((Image)original.image);
                ownsBitmap = true;
            }

            if (transparent.A > 0) {
                // ImageList_AddMasked doesn't work on high color bitmaps,
                // so we always create the mask ourselves
                Bitmap source = bitmap;
                bitmap = (Bitmap) bitmap.Clone();
                bitmap.MakeTransparent(transparent);
                if(ownsBitmap)
                    source.Dispose();
                ownsBitmap = true;
            }

            Size size = bitmap.Size;
            if ((original.options & OriginalOptions.ImageStrip) != 0) {
                // strip width must be a positive multiple of image list width
                if (size.Width == 0 || (size.Width % imageSize.Width) != 0)
                    throw new ArgumentException(SR.ImageListStripBadWidth, "original");
                if (size.Height != imageSize.Height)
                    throw new ArgumentException(SR.ImageListImageTooShort, "original");
            }
            else if (!size.Equals(ImageSize)) {
                Bitmap source = bitmap;
                bitmap = new Bitmap(source, ImageSize);
                if(ownsBitmap)
                    source.Dispose();
                ownsBitmap = true;
            }
            return bitmap;

        }

        private int AddIconToHandle(Original original, Icon icon) {
            try {
                Debug.Assert(HandleCreated, "Calling AddIconToHandle when there is no handle");
                int index = SafeNativeMethods.ImageList_ReplaceIcon(new HandleRef(this, Handle), -1, new HandleRef(icon, icon.Handle));
                if (index == -1) throw new InvalidOperationException(SR.ImageListAddFailed);
                return index;
            } finally {
                if((original.options & OriginalOptions.OwnsImage) != 0) { /// this is to handle the case were we clone the icon (see WHY WHY WHY below)
                    icon.Dispose();
                }
            }
        }
        // Adds bitmap to the Imagelist handle...
        //
        private int AddToHandle(Original original, Bitmap bitmap) {

            Debug.Assert(HandleCreated, "Calling AddToHandle when there is no handle");
            IntPtr hMask = ControlPaint.CreateHBitmapTransparencyMask(bitmap);   // Calls GDI to create Bitmap.
            IntPtr hBitmap = ControlPaint.CreateHBitmapColorMask(bitmap, hMask); // Calls GDI+ to create Bitmap. Need to add handle to HandleCollector.
            int index = SafeNativeMethods.ImageList_Add(new HandleRef(this, Handle), new HandleRef(null, hBitmap), new HandleRef(null, hMask));
            SafeNativeMethods.DeleteObject(new HandleRef(null, hBitmap));
            SafeNativeMethods.DeleteObject(new HandleRef(null, hMask));

            if (index == -1) throw new InvalidOperationException(SR.ImageListAddFailed);
            return index;
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.CreateHandle"]/*' />
        /// <devdoc>
        ///     Creates the underlying HIMAGELIST handle, and sets up all the
        ///     appropriate values with it.  Inheriting classes overriding this method
        ///     should not forget to call base.createHandle();
        /// </devdoc>
        private void CreateHandle() {
            Debug.Assert(nativeImageList == null, "Handle already created, this may be a source of temporary GDI leaks");

            int flags = NativeMethods.ILC_MASK;
            switch (colorDepth) {
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

            try {
                SafeNativeMethods.InitCommonControls();
                nativeImageList = new NativeImageList(SafeNativeMethods.ImageList_Create(imageSize.Width, imageSize.Height, flags, INITIAL_CAPACITY, GROWBY));
            }
            finally {
                UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
            }

            if (Handle == IntPtr.Zero) throw new InvalidOperationException(SR.ImageListCreateFailed);
            SafeNativeMethods.ImageList_SetBkColor(new HandleRef(this, Handle), NativeMethods.CLR_NONE);

            Debug.Assert(originals != null, "Handle not yet created, yet original images are gone");
            for (int i = 0; i < originals.Count; i++) {
                Original original = (Original) originals[i];
                if (original.image is Icon) {
                    AddIconToHandle(original, (Icon)original.image);
                    // NOTE: if we own the icon (it's been created by us) this WILL dispose the icon to avoid a GDI leak
                    // **** original.image is NOT LONGER VALID AFTER THIS POINT ***
                }
                else {
                    bool ownsBitmap = false;
                    Bitmap bitmapValue = CreateBitmap(original, out ownsBitmap);
                    AddToHandle(original, bitmapValue);
                    if(ownsBitmap)
                        bitmapValue.Dispose();
                }
            }
            originals = null;
        }

        // Don't merge this function into Dispose() -- that base.Dispose() will damage the design time experience
        private void DestroyHandle() {
            if (HandleCreated) {
                nativeImageList.Dispose();
                nativeImageList = null;
                originals = new ArrayList();
            }
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.Dispose"]/*' />
        /// <devdoc>
        ///     Frees all resources assocaited with this component.
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if(originals != null) { // we might own some of the stuff that's not been created yet
                    foreach(Original original in originals) {
                        if((original.options & OriginalOptions.OwnsImage) != 0) {
                            ((IDisposable)original.image).Dispose();
                        }
                    }
                }
                DestroyHandle();
            }
            base.Dispose(disposing);
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.Draw"]/*' />
        /// <devdoc>
        ///     Draw the image indicated by the given index on the given Graphics
        ///     at the given location.
        /// </devdoc>
        public void Draw(Graphics g, Point pt, int index) {
            Draw(g, pt.X, pt.Y, index);
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.Draw1"]/*' />
        /// <devdoc>
        ///     Draw the image indicated by the given index on the given Graphics
        ///     at the given location.
        /// </devdoc>
        public void Draw(Graphics g, int x, int y, int index) {
            Draw(g, x, y, imageSize.Width, imageSize.Height, index);
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.Draw2"]/*' />
        /// <devdoc>
        ///     Draw the image indicated by the given index using the location, size
        ///     and raster op code specified.  The image is stretched or compressed as
        ///     necessary to fit the bounds provided.
        /// </devdoc>
        public void Draw(Graphics g, int x, int y, int width, int height, int index) {
            if (index < 0 || index >= Images.Count)
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));
            IntPtr dc = g.GetHdc();
            try {
                SafeNativeMethods.ImageList_DrawEx(new HandleRef(this, Handle), index, new HandleRef(g, dc), x, y,
                                       width, height, NativeMethods.CLR_NONE, NativeMethods.CLR_NONE, NativeMethods.ILD_TRANSPARENT);
            }
            finally {
                g.ReleaseHdcInternal(dc);
            }
        }


        private void CopyBitmapData(BitmapData sourceData, BitmapData targetData) {
            // do the actual copy
            int offsetSrc = 0;
            int offsetDest = 0;
            unsafe {
                for (int i = 0; i < targetData.Height; i++)
                {
                    IntPtr srcPtr, destPtr;
                    if (IntPtr.Size == 4) {
                        srcPtr = new IntPtr(sourceData.Scan0.ToInt32() + offsetSrc);
                        destPtr = new IntPtr(targetData.Scan0.ToInt32() + offsetDest);
                    } else {
                        srcPtr = new IntPtr(sourceData.Scan0.ToInt64() + offsetSrc);
                        destPtr = new IntPtr(targetData.Scan0.ToInt64() + offsetDest);
                    }
                    UnsafeNativeMethods.CopyMemory(new HandleRef(this, destPtr), new HandleRef(this, srcPtr), Math.Abs(targetData.Stride)); 
                    offsetSrc += sourceData.Stride;
                    offsetDest += targetData.Stride;
                }
            }
        }
        
        private static bool BitmapHasAlpha(BitmapData bmpData) {
            if(bmpData.PixelFormat != PixelFormat.Format32bppArgb && bmpData.PixelFormat != PixelFormat.Format32bppRgb) {
                return false;
            }
            bool hasAlpha = false;          
            unsafe {    
                for (int i = 0; i < bmpData.Height; i++) {
                    int offsetRow = i * bmpData.Stride;
                    for (int j = 3; j < bmpData.Width*4; j += 4) { // *4 is safe since we know PixelFormat is ARGB
                        unsafe {
                            byte* candidate = ((byte*)bmpData.Scan0.ToPointer()) + offsetRow + j;
                            if (*candidate != 0) {
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

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.GetBitmap"]/*' />
        /// <devdoc>
        ///     Returns the image specified by the given index.  The bitmap returned is a
        ///     copy of the original image.
        /// </devdoc>
        // NOTE: forces handle creation, so doesn't return things from the original list
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine | ResourceScope.Process, ResourceScope.Machine | ResourceScope.Process)]
        private Bitmap GetBitmap(int index) {
            if (index < 0 || index >= Images.Count)
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));

            Bitmap result=null;

            // if the imagelist is 32bpp, if the image slot at index
            // has valid alpha information (not all zero... which is cause by windows just painting RGB values
            // and not touching the alpha byte for images < 32bpp painted to a 32bpp imagelist)
            // we're not using the mask. That means that
            // we can just get the whole image strip, cut out the piece that we want
            // and return that, that way we don't flatten the alpha by painting the value with the alpha... (ie using the alpha)
         
            if(ColorDepth == ColorDepth.Depth32Bit) {

                NativeMethods.IMAGEINFO imageInfo = new NativeMethods.IMAGEINFO(); // review? do I need to delete the mask and image inside of imageinfo?
                if(SafeNativeMethods.ImageList_GetImageInfo(new HandleRef(this, this.Handle), index, imageInfo)) {
                    Bitmap tmpBitmap = null;
                    BitmapData bmpData = null;
                    BitmapData targetData = null;
                    IntSecurity.ObjectFromWin32Handle.Assert();
                    try {
                        tmpBitmap = Bitmap.FromHbitmap(imageInfo.hbmImage);
                        // 




                        bmpData = tmpBitmap.LockBits(new Rectangle(imageInfo.rcImage_left,imageInfo.rcImage_top, imageInfo.rcImage_right-imageInfo.rcImage_left, imageInfo.rcImage_bottom-imageInfo.rcImage_top), ImageLockMode.ReadOnly, tmpBitmap.PixelFormat);

                        int offset =  bmpData.Stride * imageSize.Height   * index;
                        // we need do the following if the image has alpha because otherwise the image is fully transparent even though it has data
                        if(BitmapHasAlpha(bmpData)) {
                            result = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format32bppArgb);
                            targetData = result.LockBits(new Rectangle(0, 0, imageSize.Width, imageSize.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                            CopyBitmapData(bmpData, targetData);
                        }                        
                    } finally {
                        CodeAccessPermission.RevertAssert();
                        if(tmpBitmap != null) {
                            if(bmpData != null) {
                                tmpBitmap.UnlockBits(bmpData);
                            }
                            tmpBitmap.Dispose();
                        }
                        if(result != null && targetData != null) {
                            result.UnlockBits(targetData);
                        }
                    }
                }                 
            } 

            if(result == null) { // paint with the mask but no alpha...
                result = new Bitmap(imageSize.Width, imageSize.Height);

                Graphics graphics = Graphics.FromImage(result);
                try {
                    IntPtr dc = graphics.GetHdc();
                    try {
                        SafeNativeMethods.ImageList_DrawEx(new HandleRef(this, Handle), index, new HandleRef(graphics, dc), 0, 0,
                                                imageSize.Width, imageSize.Height, NativeMethods.CLR_NONE, NativeMethods.CLR_NONE, NativeMethods.ILD_TRANSPARENT);

                    }
                    finally {
                        graphics.ReleaseHdcInternal(dc);
                    }
                }
                finally {
                    graphics.Dispose();
                }                
            }

            // gpr: See Icon for description of fakeTransparencyColor
            result.MakeTransparent(fakeTransparencyColor);
            return result;
        }



#if DEBUG_ONLY_APIS
        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.DebugOnly_GetMasterImage"]/*' />
        public Bitmap DebugOnly_GetMasterImage() {
            if (Images.Empty)
                return null;

            return Image.FromHBITMAP(GetImageInfo(0).hbmImage);
        }

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.DebugOnly_GetMasterMask"]/*' />
        public Bitmap DebugOnly_GetMasterMask() {
            if (Images.Empty)
                return null;

            return Image.FromHBITMAP(GetImageInfo(0).hbmMask);
        }
#endif // DEBUG_ONLY_APIS

        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.OnRecreateHandle"]/*' />
        /// <devdoc>
        ///     Called when the Handle property changes.
        /// </devdoc>
        private void OnRecreateHandle(EventArgs eventargs) {
            if (recreateHandler != null) {
                recreateHandler(this, eventargs);
            }
        }

        private void OnChangeHandle(EventArgs eventargs) {
            if (changeHandler != null) {
                changeHandler(this, eventargs);
            }
        }

#if false
        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.PutImageInTempBitmap"]/*' />
        /// <devdoc>
        ///     Copies the image at the specified index into the temporary Bitmap object.
        ///     The temporary Bitmap object is used for stuff that the Windows ImageList
        ///     control doesn't support, such as stretching images or copying images from
        ///     different image lists.  Since bitmap creation is expensive, the same instance
        ///     of the temporary Bitmap is reused.
        /// </devdoc>
        private void PutImageInTempBitmap(int index, bool useSnapshot) {
            Debug.Assert(!useSnapshot || himlTemp != 0, "Where's himlTemp?");

            IntPtr handleUse = (useSnapshot ? himlTemp : Handle);
            int count = SafeNativeMethods.ImageList_GetImageCount(handleUse);

            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString()));

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
        private void PerformRecreateHandle(string reason) {
            if (!HandleCreated) return;

            if (originals == null || Images.Empty)
                originals = new ArrayList(); // spoof it into thinking this is the first CreateHandle

            if (originals == null)
                throw new InvalidOperationException(string.Format(SR.ImageListCantRecreate, reason));

            DestroyHandle();
            CreateHandle();
            OnRecreateHandle(new EventArgs());
        }

        private void ResetImageSize() {
            ImageSize = DefaultImageSize;
        }

        private void ResetTransparentColor() {
            TransparentColor = Color.LightGray;
        }

        private bool ShouldSerializeTransparentColor() {
            return !TransparentColor.Equals(Color.LightGray);
        }


        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {
            string s = base.ToString();
            if (Images != null) {
                return s + " Images.Count: " + Images.Count.ToString(CultureInfo.CurrentCulture) + ", ImageSize: " + ImageSize.ToString();
            }
            else {
                return s;
            }
        }

        internal class NativeImageList : IDisposable {
            private IntPtr himl;
#if DEBUG
            private string callStack;
#endif

            internal NativeImageList(IntPtr himl) {
                this.himl = himl;
#if DEBUG
                new EnvironmentPermission(PermissionState.Unrestricted).Assert();
                try {
                    callStack = Environment.StackTrace;
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
#endif
            }

            internal IntPtr Handle
            {
                get
                {
                    return himl;
                }
            }

            public void Dispose() {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void Dispose(bool disposing) {
                if (himl != IntPtr.Zero) {
                    SafeNativeMethods.ImageList_Destroy(new HandleRef(null, himl));
                    himl = IntPtr.Zero;
                }
            }

            ~NativeImageList() {
                Dispose(false);
            }

        }

        // An image before we add it to the image list, along with a few details about how to add it.
        private class Original {
            internal object image;
            internal OriginalOptions options;
            internal Color customTransparentColor = Color.Transparent;

            internal int nImages = 1;

            internal Original(object image, OriginalOptions options)
            : this(image, options, Color.Transparent) {
            }

            internal Original(object image, OriginalOptions options, int nImages)
            : this(image, options, Color.Transparent) {
                this.nImages = nImages;
            }

            internal Original(object image, OriginalOptions options, Color customTransparentColor) {
                Debug.Assert(image != null, "image is null");
                if (!(image is Icon) && !(image is Image)) {
                    throw new InvalidOperationException(SR.ImageListEntryType);
                }
                this.image = image;
                this.options = options;
                this.customTransparentColor = customTransparentColor;
                if ((options & OriginalOptions.CustomTransparentColor) == 0) {
                    Debug.Assert(customTransparentColor.Equals(Color.Transparent),
                                 "Specified a custom transparent color then told us to ignore it");
                }
            }
        }

        [Flags]
        private enum OriginalOptions {
            Default                = 0x00,

            ImageStrip             = 0x01,
            CustomTransparentColor = 0x02,
            OwnsImage              = 0x04
        }





        // Everything other than set_All, Add, and Clear will force handle creation.
        /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection"]/*' />
        [
        Editor("System.Windows.Forms.Design.ImageCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        public sealed class ImageCollection : IList {
            private ImageList owner;
            private ArrayList imageInfoCollection = new ArrayList();

            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.Keys"]/*' />
            /// <devdoc>
            ///  <para>Returns the keys in the image list - images without keys return String.Empty.
            ///  </para>
            /// </devdoc>
            public StringCollection Keys {
                get {
                    // pass back a copy of the current state.
                    StringCollection keysCollection = new StringCollection();

                    for (int i = 0; i < imageInfoCollection.Count; i++) {
                        ImageInfo image = imageInfoCollection[i] as ImageInfo;
                        if ((image != null) && (image.Name != null) && (image.Name.Length != 0)) {
                             keysCollection.Add(image.Name);
                        } else {
                            keysCollection.Add(string.Empty);
                        }
                    }
                    return keysCollection;
                 }
            }
            internal ImageCollection(ImageList owner) {
                this.owner = owner;
            }

            internal void ResetKeys() {
                if (imageInfoCollection!= null)
                    imageInfoCollection.Clear();

                for (int i = 0; i < this.Count; i++) {
                   imageInfoCollection.Add(new ImageCollection.ImageInfo());
                }
            }

            [Conditional("DEBUG")]            
            private void AssertInvariant() {
                Debug.Assert(owner != null, "ImageCollection has no owner (ImageList)");
                Debug.Assert( (owner.originals == null) == (owner.HandleCreated), " Either we should have the original images, or the handle should be created");
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Count"]/*' />
            [Browsable(false)]
            public int Count {
                [ResourceExposure(ResourceScope.None)]
                get {
                    Debug.Assert(owner != null, "ImageCollection has no owner (ImageList)");

                    if (owner.HandleCreated) {
                        return SafeNativeMethods.ImageList_GetImageCount(new HandleRef(owner, owner.Handle));
                    }
                    else {
                        int count = 0;
                        foreach(Original original in owner.originals) {
                            if (original != null) {
                                count += original.nImages;
                            }
                        }
                        return count;
                    }
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return false;
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Empty"]/*' />
            /// <devdoc>
            ///      Determines if the ImageList has any images, without forcing a handle creation.
            /// </devdoc>
            public bool Empty {
                get  {
                    return Count == 0;
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.this"]/*' />
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Image this[int index] {
                [ResourceExposure(ResourceScope.Machine)]
                [ResourceConsumption(ResourceScope.Machine)]
                get {
                    if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));
                    return owner.GetBitmap(index);
                }
                set {
                    if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));

                    if (value == null) {
                        throw new ArgumentNullException(nameof(value));
                    }

                   if (!(value is Bitmap))
                        throw new ArgumentException(SR.ImageListBitmap);

                    AssertInvariant();
                    Bitmap bitmap = (Bitmap)value;

                    bool ownsImage = false;
                    if (owner.UseTransparentColor) {
                        // Since there's no ImageList_ReplaceMasked, we need to generate
                        // a transparent bitmap
                        Bitmap source = bitmap;
                        bitmap = (Bitmap) bitmap.Clone(); 
                        bitmap.MakeTransparent(owner.transparentColor);
                        ownsImage = true;
                    }

                    try {
                        IntPtr hMask = ControlPaint.CreateHBitmapTransparencyMask(bitmap);
                        IntPtr hBitmap = ControlPaint.CreateHBitmapColorMask(bitmap, hMask);
                        bool ok = SafeNativeMethods.ImageList_Replace(new HandleRef(owner, owner.Handle), index, new HandleRef(null, hBitmap), new HandleRef(null, hMask));
                        SafeNativeMethods.DeleteObject(new HandleRef(null, hBitmap));
                        SafeNativeMethods.DeleteObject(new HandleRef(null, hMask));

                        if (!ok)
                            throw new InvalidOperationException(SR.ImageListReplaceFailed);
                        
                    } finally {
                        if(ownsImage) {
                            bitmap.Dispose();
                        }
                    }
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                [ResourceExposure(ResourceScope.Machine)]
                [ResourceConsumption(ResourceScope.Machine)]
                get {
                    return this[index];
                }
                set {
                    if (value is Image) {
                        this[index] = (Image)value;
                    }
                    else {
                        throw new ArgumentException(SR.ImageListBadImage, "value");
                    }
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.this"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public Image this[string key] {
                [ResourceExposure(ResourceScope.Machine)]
                [ResourceConsumption(ResourceScope.Machine)]
                get {
                    // We do not support null and empty string as valid keys.
                    if ((key == null) || (key.Length == 0)){
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index)) {
                        return this[index];
                    }
                    else {
                        return null;
                    }

                }
            }


            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.Add4"]/*' />
            /// <devdoc>
            ///     <para>Adds an image to the end of the image list with a key accessor.</para>
            /// </devdoc>
            public void Add(string key, Image image) {
                Debug.Assert((this.Count == imageInfoCollection.Count), "The count of these two collections should be equal.");

                // Store off the name.
                ImageInfo imageInfo = new ImageInfo();
                imageInfo.Name = key;

                // Add the image to the IList
                Original original = new Original(image, OriginalOptions.Default);
                Add(original, imageInfo);

            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.Add3"]/*' />
            /// <devdoc>
            ///     <para>Adds an icon to the end of the image list with a key accessor.</para>
            /// </devdoc>
            public void Add(string key, Icon icon) {
                Debug.Assert((this.Count == imageInfoCollection.Count), "The count of these two collections should be equal.");

                // Store off the name.
                ImageInfo imageInfo = new ImageInfo();
                imageInfo.Name = key;

                // Add the image to the IList
                Original original = new Original(icon, OriginalOptions.Default);
                Add(original, imageInfo);


            }


            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                if (value is Image) {
                    Add((Image)value);
                    return Count - 1;
                }
                else {
                    throw new ArgumentException(SR.ImageListBadImage, "value");
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Add"]/*' />
            public void Add(Icon value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                Add(new Original(value.Clone(), OriginalOptions.OwnsImage), null); // WHY WHY WHY do we clone here...
                // changing it now is a breaking change, so we have to keep track of this specific icon and dispose that
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Add1"]/*' />
            /// <devdoc>
            ///     Add the given image to the ImageList.
            /// </devdoc>
            public void Add(Image value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                Original original = new Original(value, OriginalOptions.Default);
                Add(original, null);
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Add2"]/*' />
            /// <devdoc>
            ///     Add the given image to the ImageList, using the given color
            ///     to generate the mask. The number of images to add is inferred from
            ///     the width of the given image.
            /// </devdoc>
            public int Add(Image value, Color transparentColor) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                Original original = new Original(value, OriginalOptions.CustomTransparentColor,
                                                 transparentColor);
                return Add(original, null);
            }

            private int Add(Original original, ImageInfo imageInfo) {
                if (original == null || original.image == null) {
                    throw new ArgumentNullException(nameof(original));
                }

                int index = -1;

                AssertInvariant();

                if (original.image is Bitmap) {
                    if (owner.originals != null) {
                        index = owner.originals.Add(original);
                    }

                    if (owner.HandleCreated) {
                        bool ownsBitmap = false;
                        Bitmap bitmapValue = owner.CreateBitmap(original, out ownsBitmap);
                        index = owner.AddToHandle(original, bitmapValue);
                        if(ownsBitmap)
                            bitmapValue.Dispose();
                    }
                }
                else if (original.image is Icon) {
                    if (owner.originals != null) {
                        index = owner.originals.Add(original);
                    }
                    if (owner.HandleCreated) {
                        index = owner.AddIconToHandle(original, (Icon)original.image);
                        // NOTE: if we own the icon (it's been created by us) this WILL dispose the icon to avoid a GDI leak
                        // **** original.image is NOT LONGER VALID AFTER THIS POINT ***
                    }
                }
                else {
                    throw new ArgumentException(SR.ImageListBitmap);
                }

                // update the imageInfoCollection
                // support AddStrip
                if ((original.options & OriginalOptions.ImageStrip) != 0) {
                    for (int i = 0; i < original.nImages; i++) {
                        imageInfoCollection.Add(new ImageInfo());
                    }
                } else {
                    if (imageInfo == null)
                        imageInfo = new ImageInfo();
                    imageInfoCollection.Add(imageInfo);
                }

                if (!owner.inAddRange)
                    owner.OnChangeHandle(new EventArgs());

                return index;
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageListCollection.AddRange"]/*' />
            public void AddRange(Image[] images) {
                if (images == null) {
                    throw new ArgumentNullException(nameof(images));
                }
                owner.inAddRange = true;
                foreach(Image image in images) {
                    Add(image);
                }
                owner.inAddRange = false;
                owner.OnChangeHandle(new EventArgs());
             }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.AddStrip"]/*' />
            /// <devdoc>
            ///     Add an image strip the given image to the ImageList.  A strip is a single Image
            ///     which is treated as multiple images arranged side-by-side.
            /// </devdoc>
            public int AddStrip(Image value) {

                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                // strip width must be a positive multiple of image list width
                //
                if (value.Width == 0 || (value.Width % owner.ImageSize.Width) != 0)
                    throw new ArgumentException(SR.ImageListStripBadWidth, "value");
                if (value.Height != owner.ImageSize.Height)
                    throw new ArgumentException(SR.ImageListImageTooShort, "value");

                int nImages = value.Width / owner.ImageSize.Width;

                Original original = new Original(value, OriginalOptions.ImageStrip, nImages);

                return Add(original, null);
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Clear"]/*' />
            /// <devdoc>
            ///     Remove all images and masks from the ImageList.
            /// </devdoc>
            public void Clear() {
                AssertInvariant();
                if (owner.originals != null)
                    owner.originals.Clear();

                imageInfoCollection.Clear();

                if (owner.HandleCreated)
                    SafeNativeMethods.ImageList_Remove(new HandleRef(owner, owner.Handle), -1);

                owner.OnChangeHandle(new EventArgs());
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Contains"]/*' />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool Contains(Image image) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object image) {
                if (image is Image) {
                    return Contains((Image)image);
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.ContainsKey"]/*' />
            /// <devdoc>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </devdoc>
            public bool ContainsKey(string key) {
               return IsValidIndex(IndexOfKey(key));
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.IndexOf"]/*' />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int IndexOf(Image image) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object image) {
                if (image is Image) {
                    return IndexOf((Image)image);
                }
                else {
                    return -1;
                }
            }

           /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IndexOfKey"]/*' />
           /// <devdoc>
           ///     <para>The zero-based index of the first occurrence of value within the entire CollectionBase,
           ///           if found; otherwise, -1.</para>
           /// </devdoc>
           public int  IndexOfKey(String key) {
                // Step 0 - Arg validation
                if ((key == null) || (key.Length == 0)){
                    return -1; // we dont support empty or null keys.
                }


                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if ((imageInfoCollection[lastAccessedIndex] != null) &&
                        (WindowsFormsUtils.SafeCompareStrings(((ImageInfo)imageInfoCollection[lastAccessedIndex]).Name, key, /* ignoreCase = */ true))) {
                            return lastAccessedIndex;
                        }
                }

                // step 2 - search for the item
                for (int i = 0; i < this.Count; i ++) {
                    if ((imageInfoCollection[i] != null) &&
                            (WindowsFormsUtils.SafeCompareStrings(((ImageInfo)imageInfoCollection[i]).Name, key, /* ignoreCase = */ true))) {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }



            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IsValidIndex"]/*' />
            /// <devdoc>
            ///     <para>Determines if the index is valid for the collection.</para>
            /// </devdoc>
            /// <internalonly/>
            private bool IsValidIndex(int index) {
                return ((index >= 0) && (index < this.Count));
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array dest, int index) {
                AssertInvariant();
                for (int i = 0; i < Count; ++i) {
                    dest.SetValue(owner.GetBitmap(i), index++);
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.GetEnumerator"]/*' />
            public IEnumerator GetEnumerator() {
                // Forces handle creation

                AssertInvariant();
                Image[] images = new Image[Count];
                for (int i = 0; i < images.Length; ++i)
                    images[i] = owner.GetBitmap(i);

                return images.GetEnumerator();
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Remove"]/*' />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Remove(Image image) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object image) {
                if (image is Image) {
                    Remove((Image)image);
                    owner.OnChangeHandle(new EventArgs());
                }
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.RemoveAt"]/*' />
            public void RemoveAt(int index) {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));

                AssertInvariant();
                bool ok = SafeNativeMethods.ImageList_Remove(new HandleRef(owner, owner.Handle), index);
                if (!ok) {
                    throw new InvalidOperationException(SR.ImageListRemoveFailed);
                } else {
                    if ((imageInfoCollection != null) && (index >= 0  && index < imageInfoCollection.Count)) {
                         imageInfoCollection.RemoveAt(index);
                         owner.OnChangeHandle(new EventArgs());
                    }
                }
             }


          /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.RemoveByKey"]/*' />
          /// <devdoc>
          ///     <para>Removes the child control with the specified key.</para>
          /// </devdoc>
          public void RemoveByKey(string key) {
                int index = IndexOfKey(key);
                if (IsValidIndex(index)) {
                    RemoveAt(index);
                }
           }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.SetKeyName"]/*' />
            /// <devdoc>
            ///     <para>Sets/Resets the key accessor for an image already in the image list.</para>
            /// </devdoc>
            public void SetKeyName(int index, string name) {
                if (!IsValidIndex(index)) {
                    throw new IndexOutOfRangeException(); // 
                }

                if (imageInfoCollection[index] == null) {
                    imageInfoCollection[index] = new ImageInfo();
                }

                ((ImageInfo)imageInfoCollection[index]).Name = name;
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageInfo"]/*' />
            /// <internalonly/>
            internal class ImageInfo {
                private string name;
                public ImageInfo() {
                }

                public string Name {
                    get { return name; }
                    set { name = value; }
                }
            }

        } // end class ImageCollection
    }

    /// <include file='doc\ImageListConverter.uex' path='docs/doc[@for="ImageListConverter"]/*' />
    /// <internalonly/>
    internal class ImageListConverter : ComponentConverter {

        public ImageListConverter() : base(typeof(ImageList)) {
        }

        /// <include file='doc\ImageListConverter.uex' path='docs/doc[@for="ImageListConverter.GetPropertiesSupported"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Gets a value indicating
        ///       whether this object supports properties using the
        ///       specified context.</para>
        /// </devdoc>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
            return true;
        }
    }
}

