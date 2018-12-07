// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define GRAYSCALE_DISABLED



namespace System.Windows.Forms {
    using System.ComponentModel;

    using System.Diagnostics;
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Forms.Layout;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;
    using System.Drawing.Text;
    using System.Drawing.Imaging;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Internal;
    using System.Runtime.Versioning;

    /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint"]/*' />
    /// <devdoc>
    ///      The ControlPaint class provides a series of methods that can be used to
    ///      paint common Windows UI pieces.  Many windows forms controls use this class to paint
    ///      their UI elements.
    /// </devdoc>
    public sealed class ControlPaint {
        [ThreadStatic]
        private static Bitmap       checkImage;         // image used to render checkmarks
        [ThreadStatic]
        private static Pen          focusPen;           // pen used to draw a focus rectangle
        [ThreadStatic]
        private static Pen          focusPenInvert;     // pen used to draw a focus rectangle
        [ThreadStatic]
        private static Color        focusPenColor;      // the last background color the focus pen was created with
        [ThreadStatic]
        private static bool         hcFocusPen;         // cached focus pen intended for high contrast mode
        private static Pen          grabPenPrimary;     // pen used for primary grab handles
        private static Pen          grabPenSecondary;   // pen used for secondary grab handles
        private static Brush        grabBrushPrimary;   // brush used for primary grab handles
        private static Brush        grabBrushSecondary; // brush used for secondary grab handles
        [ThreadStatic]
        private static Brush        frameBrushActive;   // brush used for the active selection frame
        private static Color        frameColorActive;   // color of active frame brush
        [ThreadStatic]
        private static Brush        frameBrushSelected; // brush used for the inactive selection frame
        private static Color        frameColorSelected; // color of selected frame brush
        [ThreadStatic]
        private static Brush        gridBrush;          // brush used to draw a grid
        private static Size         gridSize;           // the dimensions of the grid dots
        private static bool         gridInvert;         // true if the grid color is inverted
        [ThreadStatic]
        private static ImageAttributes disabledImageAttr; // ImageAttributes used to render disabled images

        //use these value to signify ANY of the right, top, left, center, or bottom alignments with the ContentAlignment enum.
        private static readonly ContentAlignment anyRight  = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;
        private static readonly ContentAlignment anyBottom = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
        private static readonly ContentAlignment anyCenter = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
        private static readonly ContentAlignment anyMiddle = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
        
        // not creatable...
        //
        private ControlPaint() {
        }

        internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout) {

           Rectangle result = bounds;

           if (backgroundImage != null) {
               switch (imageLayout) {
                   case ImageLayout.Stretch:
                       result.Size = bounds.Size;
                       break;

                   case ImageLayout.None:
                       result.Size = backgroundImage.Size;
                       break;

                   case ImageLayout.Center:
                       result.Size = backgroundImage.Size;
                       Size szCtl = bounds.Size;
                       
                       if (szCtl.Width > result.Width) {
                           result.X = (szCtl.Width - result.Width) / 2;
                       }
                       if (szCtl.Height > result.Height) {
                           result.Y = (szCtl.Height - result.Height) / 2;
                       }
                       break;

                   case ImageLayout.Zoom:
                        Size imageSize = backgroundImage.Size;
                        float xRatio = (float)bounds.Width / (float)imageSize.Width;
                        float yRatio = (float)bounds.Height / (float)imageSize.Height;
                        if (xRatio < yRatio) {
                            //width should fill the entire bounds.
                            result.Width = bounds.Width;
                            // preserve the aspect ratio by multiplying the xRatio by the height
                            // adding .5 to round to the nearest pixel
                            result.Height = (int) ((imageSize.Height * xRatio) +.5);
                            if (bounds.Y >= 0)
                            {                               
                                result.Y = (bounds.Height - result.Height) /2;
                            }
                        }
                        else {
                            // width should fill the entire bounds
                            result.Height = bounds.Height;                       
                            // preserve the aspect ratio by multiplying the xRatio by the height
                            // adding .5 to round to the nearest pixel
                            result.Width = (int) ((imageSize.Width * yRatio) +.5);
                            if (bounds.X >= 0)
                            {
                                result.X = (bounds.Width - result.Width) /2;
                            }
                        }
                                              
                        break;
               }
           }
           return result;
       }


        // a color appropriate for certain elements that are ControlDark in normal color schemes,
        // but for which ControlDark does not work in high contrast color schemes
        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.ContrastControlDark"]/*' />
        public static Color ContrastControlDark {
            get {
                return SystemInformation.HighContrast ? SystemColors.WindowFrame : SystemColors.ControlDark;
            }
        }

        // Returns address of a BITMAPINFO for use by CreateHBITMAP16Bit.
        // The caller is resposible for freeing the memory returned by this method.
        // 


        private static IntPtr CreateBitmapInfo(Bitmap bitmap, IntPtr hdcS) {
            NativeMethods.BITMAPINFOHEADER header = new NativeMethods.BITMAPINFOHEADER();
            header.biSize = Marshal.SizeOf(header);
            header.biWidth = bitmap.Width;
            header.biHeight = bitmap.Height;
            header.biPlanes = 1;
            header.biBitCount = 16;
            header.biCompression = NativeMethods.BI_RGB;
            // leave everything else 0

            // Set up color table --
            int entryCount = 0;
            IntPtr palette = SafeNativeMethods.CreateHalftonePalette(new HandleRef(null, hdcS));
            UnsafeNativeMethods.GetObject(new HandleRef(null, palette), 2, ref entryCount);
            int[] entries = new int[entryCount];
            SafeNativeMethods.GetPaletteEntries(new HandleRef(null, palette), 0, entryCount, entries);
            int[] colors = new int[entryCount];
            for (int i = 0; i < entryCount; i++) {
                int entry = entries[i];
                colors[i]
                = (entry & unchecked((int)0xff000000)) >> 6 // red
                  + (entry & 0x00ff0000) >> 4 // blue
                  + (entry & 0x0000ff00) >> 2; // green
            }
            SafeNativeMethods.DeleteObject(new HandleRef(null, palette));

            IntPtr address = Marshal.AllocCoTaskMem(Marshal.SizeOf(header) + entryCount*4);
            Marshal.StructureToPtr(header, address, false);
            Marshal.Copy(colors, 0, (IntPtr)((long)address + Marshal.SizeOf(header)), entryCount);
            return address;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.CreateHBitmap16Bit"]/*' />
        /// <devdoc>
        ///     Creates a 16-bit color bitmap.
        ///     Sadly, this must be public for the designer to get at it.
        ///     From MSDN: 
        ///       This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public static IntPtr CreateHBitmap16Bit(Bitmap bitmap, Color background) {
            IntPtr hBitmap;
            Size size = bitmap.Size;

            using( DeviceContext screenDc = DeviceContext.ScreenDC ){
                IntPtr hdcS = screenDc.Hdc;

                using( DeviceContext compatDc = DeviceContext.FromCompatibleDC( hdcS ) ){
                    IntPtr dc = compatDc.Hdc;

                    byte[] enoughBits = new byte[bitmap.Width * bitmap.Height];
                    IntPtr bitmapInfo = CreateBitmapInfo(bitmap, hdcS);
                    hBitmap = SafeNativeMethods.CreateDIBSection(new HandleRef(null, hdcS), new HandleRef(null, bitmapInfo), NativeMethods.DIB_RGB_COLORS,
                                                                        enoughBits, IntPtr.Zero, 0);

                    Marshal.FreeCoTaskMem(bitmapInfo);

                    if (hBitmap == IntPtr.Zero) {
                        throw new Win32Exception();
                    }
                    
                    try {
                        IntPtr previousBitmap = SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, hBitmap));
                        if (previousBitmap == IntPtr.Zero) {
                            throw new Win32Exception();
                        }

                        SafeNativeMethods.DeleteObject( new HandleRef( null, previousBitmap ) );

                        using( Graphics graphics = Graphics.FromHdcInternal(dc) ) {
                            using( Brush brush = new SolidBrush(background) ) {
                                graphics.FillRectangle(brush, 0, 0, size.Width, size.Height);
                            }
                            graphics.DrawImage(bitmap, 0, 0, size.Width, size.Height);
                        }
                    }
                    catch{
                        SafeNativeMethods.DeleteObject( new HandleRef( null, hBitmap ) );
                        throw;
                    }
                }
            }

            return hBitmap;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.CreateHBitmapTransparencyMask"]/*' />
        /// <devdoc>
        ///     Creates a Win32 HBITMAP out of the image.  You are responsible for
        ///     de-allocating the HBITMAP with Windows.DeleteObject(handle).
        ///     If the image uses transparency, the background will be filled with the specified color.
        ///     From MSDN:
        ///         This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public static IntPtr CreateHBitmapTransparencyMask(Bitmap bitmap) {
            if (bitmap == null) {
                throw new ArgumentNullException(nameof(bitmap));
            }
            Size size = bitmap.Size;
            int width = bitmap.Width;
            int height = bitmap.Height;

            int monochromeStride = width / 8;
            if ((width % 8) != 0) // wanted division to round up, not down
                monochromeStride++;
            // must be multiple of two -- i.e., bitmap 
            // scanlines must fall on double-byte boundaries
            if ((monochromeStride % 2) != 0)
                monochromeStride++;

            byte[] bits = new byte[monochromeStride * height];
            BitmapData data = bitmap.LockBits(new Rectangle(0,0, width, height),
                                              ImageLockMode.ReadOnly,
                                              PixelFormat.Format32bppArgb);

            Debug.Assert(data.Scan0 != IntPtr.Zero, "BitmapData.Scan0 is null; check marshalling");

            for (int y = 0; y < height; y++) {
                IntPtr scan = (IntPtr)((long)data.Scan0 + y * data.Stride);
                for (int x = 0; x < width; x++) {
                    int color = Marshal.ReadInt32(scan, x*4);
                    if (color >> 24 == 0) {
                        // pixel is transparent; set bit to 1
                        int index = monochromeStride * y + x / 8;
                        bits[index] |= (byte) (0x80 >> (x % 8));
                    }
                }
            }

            bitmap.UnlockBits(data);

            IntPtr mask = SafeNativeMethods.CreateBitmap(size.Width, size.Height, 1, /* 1bpp */ 1, bits);

            return mask;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.CreateHBitmapColorMask"]/*' />
        /// <devdoc>
        ///     Creates a Win32 HBITMAP out of the image.  You are responsible for
        ///     de-allocating the HBITMAP with Windows.DeleteObject(handle).
        ///     If the image uses transparency, the background will be filled with the specified color.
        ///     From MSDN:
        ///       This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public static IntPtr CreateHBitmapColorMask(Bitmap bitmap, IntPtr monochromeMask) {
            Size size = bitmap.Size;

            IntPtr colorMask = bitmap.GetHbitmap();
            IntPtr hdcS = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
            IntPtr source = UnsafeNativeMethods.CreateCompatibleDC(new HandleRef(null, hdcS));
            IntPtr target = UnsafeNativeMethods.CreateCompatibleDC(new HandleRef(null, hdcS));
            UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, hdcS));
            IntPtr previousSourceBitmap = SafeNativeMethods.SelectObject(new HandleRef(null, source), new HandleRef(null, monochromeMask));
            IntPtr previousTargetBitmap = SafeNativeMethods.SelectObject(new HandleRef(null, target), new HandleRef(null, colorMask));

            // Now the trick is to make colorBitmap black wherever the transparent
            // color is located, but keep the original color everywhere else.
            // We've already got the original bitmap, so all we need to do is
            // to and with the inverse of the mask (ROP DSna).  When going from
            // monochrome to color, Windows sets all 1 bits to the background
            // color, and all 0 bits to the foreground color.
            //
            SafeNativeMethods.SetBkColor(new HandleRef(null, target), 0x00ffffff); // white
            SafeNativeMethods.SetTextColor(new HandleRef(null, target), 0); // black
            SafeNativeMethods.BitBlt(new HandleRef(null, target), 0, 0, size.Width, size.Height, new HandleRef(null, source),
                                     0, 0, 0x220326); // RasterOp.SOURCE.Invert().AndWith(RasterOp.TARGET).GetRop());

            SafeNativeMethods.SelectObject(new HandleRef(null, source), new HandleRef(null, previousSourceBitmap));
            SafeNativeMethods.SelectObject(new HandleRef(null, target), new HandleRef(null, previousTargetBitmap));
            UnsafeNativeMethods.DeleteCompatibleDC(new HandleRef(null, source));
            UnsafeNativeMethods.DeleteCompatibleDC(new HandleRef(null, target));

            return  System.Internal.HandleCollector.Add(colorMask, NativeMethods.CommonHandles.GDI);;
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Machine | ResourceScope.Process, ResourceScope.Machine)]
        internal static IntPtr CreateHalftoneHBRUSH() {
            short[] grayPattern = new short[8];
            for (int i = 0; i < 8; i++)
                grayPattern[i] = (short)(0x5555 << (i & 1));
            IntPtr hBitmap = SafeNativeMethods.CreateBitmap(8, 8, 1, 1, grayPattern);

            NativeMethods.LOGBRUSH lb = new NativeMethods.LOGBRUSH();
            lb.lbColor = ColorTranslator.ToWin32(Color.Black);
            lb.lbStyle = NativeMethods.BS_PATTERN;
            lb.lbHatch = hBitmap;
            IntPtr brush = SafeNativeMethods.CreateBrushIndirect(lb);

            SafeNativeMethods.DeleteObject(new HandleRef(null, hBitmap));
            return brush;
        }

        // roughly the same code as in Graphics.cs
        internal static void CopyPixels(IntPtr sourceHwnd, IDeviceContext targetDC, Point sourceLocation, Point destinationLocation, Size blockRegionSize, CopyPixelOperation copyPixelOperation) {
            int destWidth = blockRegionSize.Width;
            int destHeight = blockRegionSize.Height;

            DeviceContext dc = DeviceContext.FromHwnd(sourceHwnd);
            HandleRef targetHDC = new HandleRef( null, targetDC.GetHdc());
            HandleRef screenHDC = new HandleRef( null, dc.Hdc );
            
            try {
                bool result = SafeNativeMethods.BitBlt(targetHDC, destinationLocation.X, destinationLocation.Y, destWidth, destHeight, 
                                                      screenHDC,
                                                      sourceLocation.X, sourceLocation.Y, (int) copyPixelOperation);
                
                //a zero result indicates a win32 exception has been thrown
                if (!result) {
                    throw new Win32Exception();
                }
            }
            finally {
                targetDC.ReleaseHdc();
                dc.Dispose();
            }
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.BorderStyleToDashStyle"]/*' />
        /// <devdoc>
        ///      Draws a border of the specified style and color to the given graphics.
        /// </devdoc>
        private static DashStyle BorderStyleToDashStyle(ButtonBorderStyle borderStyle) {
            switch (borderStyle) {
                case ButtonBorderStyle.Dotted: return DashStyle.Dot;
                case ButtonBorderStyle.Dashed: return DashStyle.Dash;
                case ButtonBorderStyle.Solid: return DashStyle.Solid;
                default:
                    Debug.Fail("border style has no corresponding dash style");
                    return DashStyle.Solid;
            }
        } 

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.Dark"]/*' />
        /// <devdoc>
        ///      Creates a new color that is a object of the given color.
        /// </devdoc>
        public static Color Dark(Color baseColor, float percOfDarkDark) {
            return new HLSColor(baseColor).Darker(percOfDarkDark);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.Dark1"]/*' />
        /// <devdoc>
        ///      Creates a new color that is a object of the given color.
        /// </devdoc>
        public static Color Dark(Color baseColor) {
            return new HLSColor(baseColor).Darker(0.5f);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DarkDark"]/*' />
        /// <devdoc>
        ///      Creates a new color that is a object of the given color.
        /// </devdoc>
        public static Color DarkDark(Color baseColor) {
            return new HLSColor(baseColor).Darker(1.0f);
        }

        //returns true if the luminosity of c1 is less than c2.
        internal static bool IsDarker(Color c1, Color c2) {
            HLSColor hc1 = new HLSColor(c1);
            HLSColor hc2 = new HLSColor(c2);
            return (hc1.Luminosity < hc2.Luminosity);
        }

        /// <devdoc>
        ///     Used by PrintToMetaFileRecursive overrides (Label, Panel) to manually
        ///     paint borders for UserPaint controls that were relying on
        ///     their window style to provide their borders.
        /// </devdoc>
        internal static void PrintBorder(Graphics graphics, Rectangle bounds, BorderStyle style, Border3DStyle b3dStyle) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            switch (style) {
                case BorderStyle.FixedSingle:
                    ControlPaint.DrawBorder(graphics, bounds, Color.FromKnownColor(KnownColor.WindowFrame), ButtonBorderStyle.Solid);
                    break;
                case BorderStyle.Fixed3D:
                    ControlPaint.DrawBorder3D(graphics, bounds, b3dStyle);
                    break;
                case BorderStyle.None:
                    break;
                default:
                    Debug.Fail("Unsupported border style.");
                    break;
            }
        }
        internal static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect) {
              DrawBackgroundImage(g, backgroundImage, backColor, backgroundImageLayout, bounds, clipRect, Point.Empty, RightToLeft.No);
        }
        internal static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect,  Point scrollOffset) {
              DrawBackgroundImage(g, backgroundImage, backColor, backgroundImageLayout, bounds, clipRect, scrollOffset, RightToLeft.No);
        }
        
        internal static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect,  Point scrollOffset, RightToLeft rightToLeft) {
            if (g == null) {
                throw new ArgumentNullException(nameof(g));
            }
         
            if(backgroundImageLayout == ImageLayout.Tile) {
                // tile
                
                using (TextureBrush textureBrush = new TextureBrush(backgroundImage,WrapMode.Tile)) {
                      // Make sure the brush origin matches the display rectangle, not the client rectangle,
                      // so the background image scrolls on AutoScroll forms.
                      if (scrollOffset != Point.Empty) {
                          Matrix transform = textureBrush.Transform;
                          transform.Translate(scrollOffset.X,scrollOffset.Y);
                          textureBrush.Transform = transform;
                      }
                      g.FillRectangle(textureBrush, clipRect);
                }
            }
  
            else {
                // Center, Stretch, Zoom
                
                Rectangle imageRectangle = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);

                //flip the coordinates only if we don't do any layout, since otherwise the image should be at the center of the
                //displayRectangle anyway.
                
                if (rightToLeft == RightToLeft.Yes && backgroundImageLayout == ImageLayout.None) {
                    imageRectangle.X += clipRect.Width - imageRectangle.Width;
                }

                // We fill the entire cliprect with the backcolor in case the image is transparent.
                // Also, if gdi+ can't quite fill the rect with the image, they will interpolate the remaining
                // pixels, and make them semi-transparent. This is another reason why we need to fill the entire rect.
                // If we didn't where ever the image was transparent, we would get garbage.
                using (SolidBrush brush = new SolidBrush(backColor)) {
                    g.FillRectangle(brush, clipRect);
                }

                if (!clipRect.Contains(imageRectangle)) {
                    if (backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Zoom) {
                        imageRectangle.Intersect(clipRect);
                        g.DrawImage(backgroundImage, imageRectangle);
                    }
                    else if (backgroundImageLayout == ImageLayout.None) {
                        imageRectangle.Offset(clipRect.Location);
                        Rectangle imageRect = imageRectangle;
                        imageRect.Intersect(clipRect);
                        Rectangle partOfImageToDraw = new Rectangle(Point.Empty, imageRect.Size);
                        g.DrawImage(backgroundImage, imageRect, partOfImageToDraw.X, partOfImageToDraw.Y, partOfImageToDraw.Width,
                            partOfImageToDraw.Height, GraphicsUnit.Pixel);
                    }
                    else {
                        Rectangle imageRect = imageRectangle;
                        imageRect.Intersect(clipRect);
                        Rectangle partOfImageToDraw = new Rectangle(new Point(imageRect.X - imageRectangle.X, imageRect.Y - imageRectangle.Y)
                                    , imageRect.Size);

                        g.DrawImage(backgroundImage, imageRect, partOfImageToDraw.X, partOfImageToDraw.Y, partOfImageToDraw.Width,
                            partOfImageToDraw.Height, GraphicsUnit.Pixel);
                    }
                }
                else {
                    ImageAttributes imageAttrib = new ImageAttributes();
                    imageAttrib.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(backgroundImage, imageRectangle, 0, 0, backgroundImage.Width, backgroundImage.Height, GraphicsUnit.Pixel, imageAttrib);
                    imageAttrib.Dispose();
                    
                }
                 
            }
            
        }


        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawBorder"]/*' />
        public static void DrawBorder(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style) {
            // Optimized version
            switch (style) {
                case ButtonBorderStyle.None:
                    // nothing
                    break;
                
                case ButtonBorderStyle.Dotted:
                case ButtonBorderStyle.Dashed:
                case ButtonBorderStyle.Solid:
                    DrawBorderSimple(graphics, bounds, color, style);
                    break;

                case ButtonBorderStyle.Inset:
                case ButtonBorderStyle.Outset:
                    DrawBorderComplex(graphics, bounds, color, style);
                    break;

                default:
                    Debug.Fail("Unknown border style");
                    break;
            }
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawBorder1"]/*' />
        /// <devdoc>
        ///      Draws a border of the specified style and color to the given graphics.
        /// </devdoc>
        public static void DrawBorder(Graphics graphics, Rectangle bounds,
                                      Color leftColor, int leftWidth, ButtonBorderStyle leftStyle,
                                      Color topColor, int topWidth, ButtonBorderStyle topStyle,
                                      Color rightColor, int rightWidth, ButtonBorderStyle rightStyle,
                                      Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle) {
            // Very general, and very slow
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }

            int[] topLineLefts = new int[topWidth];
            int[] topLineRights = new int[topWidth];
            int[] leftLineTops = new int[leftWidth];
            int[] leftLineBottoms = new int[leftWidth];
            int[] bottomLineLefts = new int[bottomWidth];
            int[] bottomLineRights = new int[bottomWidth];
            int[] rightLineTops = new int[rightWidth];
            int[] rightLineBottoms = new int[rightWidth];

            float topToLeft = 0.0f;
            float bottomToLeft = 0.0f;
            if (leftWidth > 0) {
                topToLeft = ((float)topWidth)/((float)leftWidth);
                bottomToLeft = ((float)bottomWidth)/((float)leftWidth);
            }
            float topToRight = 0.0f;
            float bottomToRight = 0.0f;
            if (rightWidth > 0) {
                topToRight = ((float)topWidth)/((float)rightWidth);
                bottomToRight = ((float)bottomWidth)/((float)rightWidth);
            }

            HLSColor topHLSColor = new HLSColor(topColor);
            HLSColor leftHLSColor = new HLSColor(leftColor);
            HLSColor bottomHLSColor = new HLSColor(bottomColor);
            HLSColor rightHLSColor = new HLSColor(rightColor);

            if (topWidth > 0) {
                int i=0;
                for (; i<topWidth; i++) {
                    int leftOffset = 0;
                    if (topToLeft > 0) {
                        leftOffset = (int)(((float)i) / topToLeft);
                    }
                    int rightOffset = 0;
                    if (topToRight > 0) {
                        rightOffset = (int)(((float)i) / topToRight);
                    }
                    topLineLefts[i] = bounds.X + leftOffset;
                    topLineRights[i] = bounds.X + bounds.Width - rightOffset - 1;
                    if (leftWidth > 0) {
                        leftLineTops[leftOffset] = bounds.Y + i + 1;
                    }
                    if (rightWidth > 0) {
                        rightLineTops[rightOffset] = bounds.Y + i;
                    }
                }
                for (int j=i; j<leftWidth; j++) {
                    leftLineTops[j] = bounds.Y + i + 1;
                }
                for (int j=i; j<rightWidth; j++) {
                    rightLineTops[j] = bounds.Y + i;
                }
            }
            else {
                for (int i=0; i<leftWidth; i++) {
                    leftLineTops[i] = bounds.Y;
                }
                for (int i=0; i<rightWidth; i++) {
                    rightLineTops[i] = bounds.Y;
                }
            }

            if (bottomWidth > 0) {
                int i=0;
                for (; i<bottomWidth; i++) {
                    int leftOffset = 0;
                    if (bottomToLeft > 0) {
                        leftOffset = (int)(((float)i) / bottomToLeft);
                    }
                    int rightOffset = 0;
                    if (bottomToRight > 0) {
                        rightOffset = (int)(((float)i) / bottomToRight);
                    }
                    bottomLineLefts[i] = bounds.X + leftOffset;
                    bottomLineRights[i] = bounds.X + bounds.Width - rightOffset - 1;
                    if (leftWidth > 0) {
                        leftLineBottoms[leftOffset] = bounds.Y + bounds.Height - i - 1;
                    }
                    if (rightWidth > 0) {
                        rightLineBottoms[rightOffset] = bounds.Y + bounds.Height - i - 1;
                    }
                }
                for (int j=i; j<leftWidth; j++) {
                    leftLineBottoms[j] = bounds.Y + bounds.Height - i - 1;
                }
                for (int j=i; j<rightWidth; j++) {
                    rightLineBottoms[j] = bounds.Y + bounds.Height - i - 1;
                }
            }
            else {
                for (int i=0; i<leftWidth; i++) {
                    leftLineBottoms[i] = bounds.Y + bounds.Height - 1;
                }
                for (int i=0; i<rightWidth; i++) {
                    rightLineBottoms[i] = bounds.Y + bounds.Height - 1;
                }
            }

            Pen pen;

            // draw top line
            switch (topStyle) {
                case ButtonBorderStyle.None:
                    // nothing
                    break;
                case ButtonBorderStyle.Dotted:
                    pen = new Pen(topColor);
                    pen.DashStyle = DashStyle.Dot;
                    for (int i=0; i<topWidth; i++) {
                        graphics.DrawLine(pen, topLineLefts[i], bounds.Y + i, topLineRights[i], bounds.Y + i);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Dashed:
                    pen = new Pen(topColor);
                    pen.DashStyle = DashStyle.Dash;
                    for (int i=0; i<topWidth; i++) {
                        graphics.DrawLine(pen, topLineLefts[i], bounds.Y + i, topLineRights[i], bounds.Y + i);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Solid:
                    pen = new Pen(topColor);
                    pen.DashStyle = DashStyle.Solid;
                    for (int i=0; i<topWidth; i++) {
                        graphics.DrawLine(pen, topLineLefts[i], bounds.Y + i, topLineRights[i], bounds.Y + i);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Inset: {
                        float inc = InfinityToOne(1.0f/(float)(topWidth-1));
                        for (int i=0; i<topWidth; i++) {
                            pen = new Pen(topHLSColor.Darker(1.0f - ((float)i)*inc));
                            pen.DashStyle = DashStyle.Solid;
                            graphics.DrawLine(pen, topLineLefts[i], bounds.Y + i, topLineRights[i], bounds.Y + i);
                            pen.Dispose();
                        }
                        break;
                    }
                case ButtonBorderStyle.Outset: {
                        float inc = InfinityToOne(1.0f/(float)(topWidth-1));

                        for (int i=0; i<topWidth; i++) {
                            pen = new Pen(topHLSColor.Lighter(1.0f - ((float)i)*inc));
                            pen.DashStyle = DashStyle.Solid;
                            graphics.DrawLine(pen, topLineLefts[i], bounds.Y + i, topLineRights[i], bounds.Y + i);
                            pen.Dispose();
                        }
                        break;
                    }
            }

            // Assertion: pen has been disposed
            pen = null;

            // draw left line
            switch (leftStyle) {
                case ButtonBorderStyle.None:
                    // nothing
                    break;
                case ButtonBorderStyle.Dotted:
                    pen = new Pen(leftColor);
                    pen.DashStyle = DashStyle.Dot;
                    for (int i=0; i<leftWidth; i++) {
                        graphics.DrawLine(pen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i]);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Dashed:
                    pen = new Pen(leftColor);
                    pen.DashStyle = DashStyle.Dash;
                    for (int i=0; i<leftWidth; i++) {
                        graphics.DrawLine(pen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i]);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Solid:
                    pen = new Pen(leftColor);
                    pen.DashStyle = DashStyle.Solid;
                    for (int i=0; i<leftWidth; i++) {
                        graphics.DrawLine(pen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i]);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Inset: {
                        float inc = InfinityToOne(1.0f/(float)(leftWidth-1));
                        for (int i=0; i<leftWidth; i++) {
                            pen = new Pen(leftHLSColor.Darker(1.0f - ((float)i)*inc));
                            pen.DashStyle = DashStyle.Solid;
                            graphics.DrawLine(pen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i]);
                            pen.Dispose();
                        }
                        break;
                    }
                case ButtonBorderStyle.Outset: {
                        float inc = InfinityToOne(1.0f/(float)(leftWidth-1));
                        for (int i=0; i<leftWidth; i++) {
                            pen = new Pen(leftHLSColor.Lighter(1.0f - ((float)i)*inc));
                            pen.DashStyle = DashStyle.Solid;
                            graphics.DrawLine(pen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i]);
                            pen.Dispose();
                        }
                        break;
                    }
            }

            // Assertion: pen has been disposed
            pen = null;

            // draw bottom line
            switch (bottomStyle) {
                case ButtonBorderStyle.None:
                    // nothing
                    break;
                case ButtonBorderStyle.Dotted:
                    pen = new Pen(bottomColor);
                    pen.DashStyle = DashStyle.Dot;
                    for (int i=0; i<bottomWidth; i++) {
                        graphics.DrawLine(pen, bottomLineLefts[i], bounds.Y + bounds.Height - 1 - i, bottomLineRights[i], bounds.Y + bounds.Height - 1 - i);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Dashed:
                    pen = new Pen(bottomColor);
                    pen.DashStyle = DashStyle.Dash;
                    for (int i=0; i<bottomWidth; i++) {
                        graphics.DrawLine(pen, bottomLineLefts[i], bounds.Y + bounds.Height - 1 - i, bottomLineRights[i], bounds.Y + bounds.Height - 1 - i);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Solid:
                    pen = new Pen(bottomColor);
                    pen.DashStyle = DashStyle.Solid;
                    for (int i=0; i<bottomWidth; i++) {
                        graphics.DrawLine(pen, bottomLineLefts[i], bounds.Y + bounds.Height - 1 - i, bottomLineRights[i], bounds.Y + bounds.Height - 1 - i);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Inset: {
                        float inc = InfinityToOne(1.0f/(float)(bottomWidth-1));
                        for (int i=0; i<bottomWidth; i++) {
                            pen = new Pen(bottomHLSColor.Lighter(1.0f - ((float)i)*inc));
                            pen.DashStyle = DashStyle.Solid;
                            graphics.DrawLine(pen, bottomLineLefts[i], bounds.Y + bounds.Height - 1 - i, bottomLineRights[i], bounds.Y + bounds.Height - 1 - i);
                            pen.Dispose();
                        }
                        break;
                    }
                case ButtonBorderStyle.Outset: {
                        float inc = InfinityToOne(1.0f/(float)(bottomWidth-1));

                        for (int i=0; i<bottomWidth; i++) {
                            pen = new Pen(bottomHLSColor.Darker(1.0f - ((float)i)*inc));
                            pen.DashStyle = DashStyle.Solid;
                            graphics.DrawLine(pen, bottomLineLefts[i], bounds.Y + bounds.Height - 1 - i, bottomLineRights[i], bounds.Y + bounds.Height - 1 - i);
                            pen.Dispose();
                        }
                        break;
                    }
            }

            // Assertion: pen has been disposed
            pen = null;

            // draw right line
            switch (rightStyle) {
                case ButtonBorderStyle.None:
                    // nothing
                    break;
                case ButtonBorderStyle.Dotted:
                    pen = new Pen(rightColor);
                    pen.DashStyle = DashStyle.Dot;
                    for (int i=0; i<rightWidth; i++) {
                        graphics.DrawLine(pen, bounds.X + bounds.Width - 1 - i, rightLineTops[i], bounds.X + bounds.Width - 1 - i, rightLineBottoms[i]);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Dashed:
                    pen = new Pen(rightColor);
                    pen.DashStyle = DashStyle.Dash;
                    for (int i=0; i<rightWidth; i++) {
                        graphics.DrawLine(pen, bounds.X + bounds.Width - 1 - i, rightLineTops[i], bounds.X + bounds.Width - 1 - i, rightLineBottoms[i]);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Solid:
                    pen = new Pen(rightColor);
                    pen.DashStyle = DashStyle.Solid;
                    for (int i=0; i<rightWidth; i++) {
                        graphics.DrawLine(pen, bounds.X + bounds.Width - 1 - i, rightLineTops[i], bounds.X + bounds.Width - 1 - i, rightLineBottoms[i]);
                    }
                    pen.Dispose();
                    break;
                case ButtonBorderStyle.Inset: {
                        float inc = InfinityToOne(1.0f/(float)(rightWidth-1));
                        for (int i=0; i<rightWidth; i++) {
                            pen = new Pen(rightHLSColor.Lighter(1.0f - ((float)i)*inc));
                            pen.DashStyle = DashStyle.Solid;
                            graphics.DrawLine(pen, bounds.X + bounds.Width - 1 - i, rightLineTops[i], bounds.X + bounds.Width - 1 - i, rightLineBottoms[i]);
                            pen.Dispose();
                        }
                        break;
                    }
                case ButtonBorderStyle.Outset: {
                        float inc = InfinityToOne(1.0f/(float)(rightWidth-1));

                        for (int i=0; i<rightWidth; i++) {
                            pen = new Pen(rightHLSColor.Darker(1.0f - ((float)i)*inc));
                            pen.DashStyle = DashStyle.Solid;
                            graphics.DrawLine(pen, bounds.X + bounds.Width - 1 - i, rightLineTops[i], bounds.X + bounds.Width - 1 - i, rightLineBottoms[i]);
                            pen.Dispose();
                        }

                        break;
                    }
            }
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawBorder3D"]/*' />
        /// <devdoc>
        ///     Draws a 3D style border at the given rectangle.  The default 3D style of
        ///     Etched is used.
        /// </devdoc>
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle) {
            DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, Border3DStyle.Etched,
                         Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawBorder3D1"]/*' />
        /// <devdoc>
        ///     Draws a 3D style border at the given rectangle.  You may specify the style
        ///     of the 3D appearance.
        /// </devdoc>
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style) {
            DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, style,
                         Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawBorder3D2"]/*' />
        /// <devdoc>
        ///     Draws a 3D style border at the given rectangle.  You may specify the style
        ///     of the 3D appearance, and which sides of the 3D rectangle you wish to
        ///     draw.
        /// </devdoc>
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style, Border3DSide sides) {
            DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, style, sides);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawBorder3D3"]/*' />
        /// <devdoc>
        ///     Draws a 3D style border at the given rectangle.  The default 3D style of
        ///     ETCHED is used.
        /// </devdoc>
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height) {
            DrawBorder3D(graphics, x, y, width, height, Border3DStyle.Etched,
                         Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawBorder3D4"]/*' />
        /// <devdoc>
        ///     Draws a 3D style border at the given rectangle.  You may specify the style
        ///     of the 3D appearance.
        /// </devdoc>
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style) {
            DrawBorder3D(graphics, x, y, width, height, style,
                         Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawBorder3D5"]/*' />
        /// <devdoc>
        ///     Draws a 3D style border at the given rectangle.  You may specify the style
        ///     of the 3D appearance, and which sides of the 3D rectangle you wish to
        ///     draw.
        /// </devdoc>
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style, Border3DSide sides) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }

            int edge = ((int)style) & 0x0F;
            int flags = ((int)sides) | (((int)style) & ~0x0F);

            NativeMethods.RECT rc = NativeMethods.RECT.FromXYWH(x, y, width, height);

            // Windows just draws the border to size, and then
            // shrinks the rectangle so the user can paint the
            // client area.  We can't really do that, so we do
            // the opposite:  We precalculate the size of the border
            // and enlarge the rectangle so the client size is
            // preserved.
            //
            if ((flags & (int)Border3DStyle.Adjust) == (int)Border3DStyle.Adjust) {
                Size sz = SystemInformation.Border3DSize;
                rc.left -= sz.Width;
                rc.right += sz.Width;
                rc.top -= sz.Height;
                rc.bottom += sz.Height;
                flags &= ~((int)Border3DStyle.Adjust);
            }

            using( WindowsGraphics wg = WindowsGraphics.FromGraphics(graphics)) { // Get Win32 dc with Graphics properties applied to it.
                SafeNativeMethods.DrawEdge(new HandleRef(wg, wg.DeviceContext.Hdc), ref rc, edge, flags);
            }
        }
        
        /// <devdoc>
        ///     Helper function that draws a more complex border.  This is used by DrawBorder for less common
        ///     rendering cases.  We split DrawBorder into DrawBorderSimple and DrawBorderComplex so we maximize
        ///     the % of the function call.  It is less performant to have large functions that do many things.
        /// </devdoc>
        private static void DrawBorderComplex(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (style == ButtonBorderStyle.Inset) { // button being pushed
                HLSColor hls = new HLSColor(color);

                // top + left
                Pen pen = new Pen(hls.Darker(1.0f));
                graphics.DrawLine(pen, bounds.X, bounds.Y, 
                                  bounds.X + bounds.Width - 1, bounds.Y);
                graphics.DrawLine(pen, bounds.X, bounds.Y, 
                                  bounds.X, bounds.Y + bounds.Height - 1);

                // bottom + right
                pen.Color = hls.Lighter(1.0f);
                graphics.DrawLine(pen, bounds.X, bounds.Y + bounds.Height - 1, 
                                  bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
                graphics.DrawLine(pen, bounds.X + bounds.Width - 1, bounds.Y, 
                                  bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);

                // Top + left inset
                pen.Color = hls.Lighter(0.5f);
                graphics.DrawLine(pen, bounds.X + 1, bounds.Y + 1,
                                      bounds.X + bounds.Width - 2, bounds.Y + 1);
                graphics.DrawLine(pen, bounds.X + 1, bounds.Y + 1,
                                  bounds.X + 1, bounds.Y + bounds.Height - 2);

                // bottom + right inset
                if (color.ToKnownColor() == SystemColors.Control.ToKnownColor()) {
                    pen.Color = SystemColors.ControlLight;
                    graphics.DrawLine(pen, bounds.X + 1, bounds.Y + bounds.Height - 2,
                                              bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                    graphics.DrawLine(pen, bounds.X + bounds.Width - 2, bounds.Y + 1,
                                      bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                }

                pen.Dispose();
            }
            else { // Standard button
                Debug.Assert(style == ButtonBorderStyle.Outset, "Caller should have known how to use us.");
                
                bool stockColor = color.ToKnownColor() == SystemColors.Control.ToKnownColor();
                HLSColor hls = new HLSColor(color);

                // top + left
                Pen pen = stockColor ? SystemPens.ControlLightLight : new Pen(hls.Lighter(1.0f));
                graphics.DrawLine(pen, bounds.X, bounds.Y,
                                            bounds.X + bounds.Width - 1, bounds.Y);
                graphics.DrawLine(pen, bounds.X, bounds.Y, 
                                  bounds.X, bounds.Y + bounds.Height - 1);
                // bottom + right
                if (stockColor) {
                    pen = SystemPens.ControlDarkDark;
                }
                else {
                    pen.Color = hls.Darker(1.0f);
                }
                graphics.DrawLine(pen, bounds.X, bounds.Y + bounds.Height - 1, 
                                  bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
                graphics.DrawLine(pen, bounds.X + bounds.Width - 1, bounds.Y, 
                                  bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
                // top + left inset
                if (stockColor) {
                    if (SystemInformation.HighContrast) {
                        pen = SystemPens.ControlLight;
                    }
                    else {
                        pen = SystemPens.Control;
                    }
                }
                else {
                    pen.Color = color;
                }
                graphics.DrawLine(pen, bounds.X + 1, bounds.Y + 1,
                                  bounds.X + bounds.Width - 2, bounds.Y + 1);
                graphics.DrawLine(pen, bounds.X + 1, bounds.Y + 1,
                                  bounds.X + 1, bounds.Y + bounds.Height - 2);

                // Bottom + right inset                        
                if (stockColor) {
                    pen = SystemPens.ControlDark;
                }
                else {
                    pen.Color = hls.Darker(0.5f);
                }

                graphics.DrawLine(pen, bounds.X + 1, bounds.Y + bounds.Height - 2,
                                  bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                graphics.DrawLine(pen, bounds.X + bounds.Width - 2, bounds.Y + 1,
                                  bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                                  
                if (!stockColor) {
                    pen.Dispose();
                }
            }
        }
        
        /// <devdoc>
        ///     Helper function that draws a simple border.  This is used by DrawBorder for the most common rendering cases.
        /// </devdoc>
        private static void DrawBorderSimple(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style) {
            // Common case: system color with solid pen
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            bool stockBorder = (style == ButtonBorderStyle.Solid && color.IsSystemColor);
            Pen pen;
            if (stockBorder) {
                pen = SystemPens.FromSystemColor(color);
            }
            else  {
                pen = new Pen(color);
                if (style != ButtonBorderStyle.Solid) {
                    pen.DashStyle = BorderStyleToDashStyle(style);
                }
            }
                
            graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            
            if (!stockBorder) {
                pen.Dispose();
            }
         }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawButton"]/*' />
        /// <devdoc>
        ///     Draws a Win32 button control in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawButton(Graphics graphics, Rectangle rectangle, ButtonState state) {
            DrawButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawButton1"]/*' />
        /// <devdoc>
        ///     Draws a Win32 button control in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawButton(Graphics graphics, int x, int y, int width, int height, ButtonState state) {
            DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_BUTTON,
                             NativeMethods.DFCS_BUTTONPUSH | (int) state, Color.Empty, Color.Empty);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawCaptionButton"]/*' />
        /// <devdoc>
        ///     Draws a Win32 window caption button in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawCaptionButton(Graphics graphics, Rectangle rectangle, CaptionButton button, ButtonState state) {
            DrawCaptionButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, button, state);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawCaptionButton1"]/*' />
        /// <devdoc>
        ///     Draws a Win32 window caption button in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawCaptionButton(Graphics graphics, int x, int y, int width, int height, CaptionButton button, ButtonState state) {
            DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_CAPTION,
                             (int) button | (int) state, Color.Empty, Color.Empty);
        }


        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawCheckBox"]/*' />
        /// <devdoc>
        ///     Draws a Win32 checkbox control in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state) {
            DrawCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawCheckBox1"]/*' />
        /// <devdoc>
        ///     Draws a Win32 checkbox control in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState state) {
            // We overwrite the windows checkbox
            if ((state & ButtonState.Flat) == ButtonState.Flat) {
                DrawFlatCheckBox(graphics, new Rectangle(x, y, width, height), state);
            }
            else {
                DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_BUTTON,
                                 NativeMethods.DFCS_BUTTONCHECK | (int) state, Color.Empty, Color.Empty);
            }
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawComboButton"]/*' />
        /// <devdoc>
        ///     Draws the drop down button of a Win32 combo box in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawComboButton(Graphics graphics, Rectangle rectangle, ButtonState state) {
            DrawComboButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawComboButton1"]/*' />
        /// <devdoc>
        ///     Draws the drop down button of a Win32 combo box in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawComboButton(Graphics graphics, int x, int y, int width, int height, ButtonState state) {
            DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_SCROLL,
                             NativeMethods.DFCS_SCROLLCOMBOBOX | (int) state, Color.Empty, Color.Empty);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawContainerGrabHandle"]/*' />
        /// <devdoc>
        ///     Draws a container control grab handle glyph inside the given rectangle.
        /// </devdoc>
        public static void DrawContainerGrabHandle(Graphics graphics, Rectangle bounds) {

            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            Brush brush = Brushes.White;
            Pen pen = Pens.Black;

            graphics.FillRectangle(brush, bounds.Left + 1, bounds.Top + 1, bounds.Width - 2, bounds.Height - 2);
            
            //draw the bounding rect w/o the four corners
            graphics.DrawLine(pen, bounds.X + 1, bounds.Y, bounds.Right -2, bounds.Y);
            graphics.DrawLine(pen, bounds.X + 1, bounds.Bottom - 1, bounds.Right -2, bounds.Bottom - 1);
            graphics.DrawLine(pen, bounds.X, bounds.Y + 1, bounds.X, bounds.Bottom -2);
            graphics.DrawLine(pen, bounds.Right - 1, bounds.Y + 1, bounds.Right - 1, bounds.Bottom -2);

            int midx = bounds.X + bounds.Width/2;
            int midy = bounds.Y + bounds.Height/2;

            // vert line
            graphics.DrawLine(pen, midx, bounds.Y, midx, bounds.Bottom - 2);

            // horiz line
            graphics.DrawLine(pen, bounds.X, midy, bounds.Right - 2, midy);

            // top hash
            graphics.DrawLine(pen, midx - 1, bounds.Y+2, midx+1, bounds.Y+2);
            graphics.DrawLine(pen, midx - 2, bounds.Y+3, midx+2, bounds.Y+3);

            // left hash
            graphics.DrawLine(pen, bounds.X+2, midy - 1, bounds.X + 2, midy + 1);
            graphics.DrawLine(pen, bounds.X+3, midy - 2, bounds.X + 3, midy + 2);            

            // right hash
            graphics.DrawLine(pen, bounds.Right - 3, midy - 1, bounds.Right - 3, midy + 1);
            graphics.DrawLine(pen, bounds.Right - 4, midy - 2, bounds.Right - 4, midy + 2);

            // bottom hash
            graphics.DrawLine(pen, midx - 1, bounds.Bottom - 3, midx+1, bounds.Bottom - 3);
            graphics.DrawLine(pen, midx - 2, bounds.Bottom - 4, midx+2, bounds.Bottom - 4);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawFlatCheckBox"]/*' />
        /// <devdoc>
        ///     Draws a flat checkbox.
        /// </devdoc>
        private static void DrawFlatCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state) {
            // Background color of checkbox
            //
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            Brush background = ((state & ButtonState.Inactive) == ButtonState.Inactive) ?
                               SystemBrushes.Control :
                               SystemBrushes.Window;
            Color foreground = ((state & ButtonState.Inactive) == ButtonState.Inactive) ?
                               ((SystemInformation.HighContrast && AccessibilityImprovements.Level1) ? SystemColors.GrayText : SystemColors.ControlDark) :
                               SystemColors.ControlText;
            DrawFlatCheckBox(graphics, rectangle, foreground, background, state);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawFlatCheckBox1"]/*' />
        /// <devdoc>
        ///     Draws a Win32 checkbox control in the given rectangle with the given state.  This
        ///     draws a flat looking check box that is suitable for use in list boxes, etc. We
        ///     custom draw this because the windows version is soooo ugly.
        /// </devdoc>
        /// <internalonly/>
        private static void DrawFlatCheckBox(Graphics graphics, Rectangle rectangle, Color foreground, Brush background, ButtonState state) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (rectangle.Width < 0 || rectangle.Height < 0) {
                throw new ArgumentOutOfRangeException(nameof(rectangle));
            }

            Rectangle offsetRectangle = new Rectangle(rectangle.X + 1, rectangle.Y + 1,
                                                      rectangle.Width - 2, rectangle.Height - 2);
            graphics.FillRectangle(background, offsetRectangle);

            // Checkmark
            //
            if ((state & ButtonState.Checked) == ButtonState.Checked) {
                if (checkImage == null || checkImage.Width != rectangle.Width || checkImage.Height != rectangle.Height) {

                    if (checkImage != null) {
                        checkImage.Dispose();
                        checkImage = null;
                    }

                    // We draw the checkmark slightly off center to eliminate 3-D border artifacts,
                    // and compensate below
                    NativeMethods.RECT rcCheck = NativeMethods.RECT.FromXYWH(0, 0, rectangle.Width, rectangle.Height);
                    Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
                    using (Graphics g2 = Graphics.FromImage(bitmap)) {
                        g2.Clear(Color.Transparent);
                        IntPtr dc = g2.GetHdc();
                        try {
                            SafeNativeMethods.DrawFrameControl(new HandleRef(null, dc), ref rcCheck,
                                                               NativeMethods.DFC_MENU, NativeMethods.DFCS_MENUCHECK);
                        }
                        finally {
                            g2.ReleaseHdcInternal(dc);
                        }
                    }
                    bitmap.MakeTransparent();
                    checkImage = bitmap;
                }

                rectangle.X += 1;
                DrawImageColorized(graphics, checkImage, rectangle, foreground);
                rectangle.X -= 1;
            }

            // Surrounding border.  We inset this by one pixel so we match how
            // the 3D checkbox is drawn.
            //
            Pen pen = SystemPens.ControlDark;
            graphics.DrawRectangle(pen, offsetRectangle.X, offsetRectangle.Y, offsetRectangle.Width - 1, offsetRectangle.Height - 1);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawFocusRectangle"]/*' />
        /// <devdoc>
        ///      Draws a focus rectangle.  A focus rectangle is a dotted rectangle that Windows
        ///      uses to indicate what control has the current keyboard focus.
        /// </devdoc>
        public static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle) {
            DrawFocusRectangle(graphics, rectangle, SystemColors.ControlText, SystemColors.Control);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawFocusRectangle1"]/*' />
        /// <devdoc>
        ///      Draws a focus rectangle.  A focus rectangle is a dotted rectangle that Windows
        ///      uses to indicate what control has the current keyboard focus.
        /// </devdoc>
        public static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color foreColor, Color backColor) {
            DrawFocusRectangle(graphics, rectangle, backColor, false);
        }

        internal static void DrawHighContrastFocusRectangle(Graphics graphics, Rectangle rectangle, Color color) {
            DrawFocusRectangle(graphics, rectangle, color, true);
        }

        private static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color color, bool highContrast) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            rectangle.Width--;
            rectangle.Height--;
            graphics.DrawRectangle(GetFocusPen(color,
                // we want the corner to be penned
                // see GetFocusPen for more explanation
                (rectangle.X + rectangle.Y) % 2 == 1, 
                highContrast),
                    rectangle);
        }


        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawFrameControl"]/*' />
        /// <devdoc>
        ///     Draws a win32 frame control.
        /// </devdoc>
        /// <internalonly/>
        private static void DrawFrameControl(Graphics graphics, int x, int y, int width, int height, 
                                             int kind, int state, Color foreColor, Color backColor) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (width < 0) {
                throw new ArgumentOutOfRangeException(nameof(width));
            }
            if (height < 0) {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            NativeMethods.RECT rcFrame = NativeMethods.RECT.FromXYWH(0, 0, width, height);
            using (Bitmap bitmap = new Bitmap(width, height)) {
                using( Graphics g2 = Graphics.FromImage(bitmap) ) {
                    g2.Clear(Color.Transparent);

                    using( WindowsGraphics wg = WindowsGraphics.FromGraphics(g2) ){ // Get Win32 dc with Graphics properties applied to it.
                        SafeNativeMethods.DrawFrameControl(new HandleRef(wg, wg.DeviceContext.Hdc), ref rcFrame, kind, (int) state);
                    }
                
                    if (foreColor == Color.Empty || backColor == Color.Empty) {
                        graphics.DrawImage(bitmap, x, y);
                    }
                    else {
                        // Replace black/white with foreColor/backColor.
                        ImageAttributes attrs = new ImageAttributes();
                        ColorMap cm1 = new ColorMap();
                        cm1.OldColor = Color.Black;
                        cm1.NewColor = foreColor;
                        ColorMap cm2 = new ColorMap();
                        cm2.OldColor = Color.White;
                        cm2.NewColor = backColor;
                        attrs.SetRemapTable(new ColorMap[2] { cm1, cm2 }, ColorAdjustType.Bitmap);
                        graphics.DrawImage(bitmap, new Rectangle(x, y, width, height), 0, 0, width, height, GraphicsUnit.Pixel, attrs, null, IntPtr.Zero);
                    }
                }
            }
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawGrabHandle"]/*' />
        /// <devdoc>
        ///      Draws a standard selection grab handle with the given dimensions.  Grab
        ///      handles are used by components to indicate to the user that they can
        ///      be directly maniupulated.
        /// </devdoc>
        public static void DrawGrabHandle(Graphics graphics, Rectangle rectangle, bool primary, bool enabled) {
            Pen pen;
            Brush brush;
            
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (primary) {
                if (null == grabPenPrimary) {
                    grabPenPrimary = Pens.Black;
                }
                pen = grabPenPrimary;

                if (enabled) {
                    if (null == grabBrushPrimary) {
                        grabBrushPrimary = Brushes.White;
                    }
                    brush = grabBrushPrimary;
                }
                else {
                    brush = SystemBrushes.Control;
                }
            }
            else {
                if (null == grabPenSecondary) {
                    grabPenSecondary = Pens.White;
                }
                pen = grabPenSecondary;

                if (enabled) {
                    if (null == grabBrushSecondary) {
                        grabBrushSecondary = Brushes.Black;
                    }
                    brush = grabBrushSecondary;
                }
                else {
                    brush = SystemBrushes.Control;
                }
            }

            Rectangle fillRect = new Rectangle(rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 1, rectangle.Height - 1);
            graphics.FillRectangle(brush, fillRect);
            rectangle.Width --;
            rectangle.Height--;
            graphics.DrawRectangle(pen, rectangle);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawGrid"]/*' />
        /// <devdoc>
        ///      Draws a grid of one pixel dots in the given rectangle.
        /// </devdoc>
        public static void DrawGrid(Graphics graphics, Rectangle area, Size pixelsBetweenDots, Color backColor) {

            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (pixelsBetweenDots.Width <= 0 || pixelsBetweenDots.Height <= 0) {
                throw new ArgumentOutOfRangeException(nameof(pixelsBetweenDots));
            }

            float intensity = backColor.GetBrightness();
            bool invert = (intensity < .5);
            
            if (gridBrush == null || gridSize.Width != pixelsBetweenDots.Width 
                || gridSize.Height != pixelsBetweenDots.Height || invert != gridInvert) {

                if (gridBrush != null) {
                    gridBrush.Dispose();
                    gridBrush = null;
                }

                gridSize = pixelsBetweenDots;
                int idealSize = 16;
                gridInvert = invert;
                Color foreColor = (gridInvert) ? Color.White : Color.Black;

                // Round size to a multiple of pixelsBetweenDots
                int width = ((idealSize / pixelsBetweenDots.Width) + 1) * pixelsBetweenDots.Width;
                int height = ((idealSize / pixelsBetweenDots.Height) + 1) * pixelsBetweenDots.Height;

                Bitmap bitmap = new Bitmap(width, height);

                // draw the dots
                for (int x = 0; x < width; x += pixelsBetweenDots.Width)
                    for (int y = 0; y < height; y += pixelsBetweenDots.Height)
                        bitmap.SetPixel(x, y, foreColor);

                gridBrush = new TextureBrush(bitmap);
                bitmap.Dispose();
            }

            graphics.FillRectangle(gridBrush, area);
        }

        /* Unused
        // Takes a black and white image, and paints it in color
        internal static void DrawImageColorized(Graphics graphics, Image image, Rectangle destination, 
                                                Color replaceBlack, Color replaceWhite) {
            DrawImageColorized(graphics, image, destination, 
                               RemapBlackAndWhiteAndTransparentMatrix(replaceBlack, replaceWhite));
        }
        */

        // Takes a black and transparent image, turns black pixels into some other color, and leaves transparent pixels alone
        internal static void DrawImageColorized(Graphics graphics, Image image, Rectangle destination, 
                                                Color replaceBlack) {
            DrawImageColorized(graphics, image, destination, 
                               RemapBlackAndWhitePreserveTransparentMatrix(replaceBlack, Color.White));
        }

        internal static bool IsImageTransparent(Image backgroundImage) {
            if (backgroundImage != null && (backgroundImage.Flags & (int)ImageFlags.HasAlpha) > 0) {
                return true;
            }
            return false;
        }

        // takes an image and replaces all the pixels of oldColor with newColor, drawing the new image into the rectangle on
        // the supplied Graphics object.
        internal static void DrawImageReplaceColor(Graphics g, Image image, Rectangle dest, Color oldColor, Color newColor) {
            ImageAttributes attrs = new ImageAttributes();

            ColorMap cm = new ColorMap();
            cm.OldColor = oldColor;
            cm.NewColor = newColor;

            attrs.SetRemapTable(new ColorMap[]{cm}, ColorAdjustType.Bitmap);

            g.DrawImage(image, dest, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attrs, null, IntPtr.Zero);
            attrs.Dispose();
        }

        // Takes a black and white image, and paints it in color
        private static void DrawImageColorized(Graphics graphics, Image image, Rectangle destination, 
                                               ColorMatrix matrix) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix);
            graphics.DrawImage(image, destination, 0,0, image.Width, image.Height,
                               GraphicsUnit.Pixel, attributes, null, IntPtr.Zero);
            attributes.Dispose();
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawImageDisabled"]/*' />
        /// <devdoc>
        ///     Draws an image and makes it look disabled.
        /// </devdoc>
        public static void DrawImageDisabled(Graphics graphics, Image image, int x, int y, Color background) {
            DrawImageDisabled(graphics, image, new Rectangle(x, y, image.Width, image.Height), background, false);
        }

        /// <devdoc>
        ///     Draws an image and makes it look disabled.
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]        
        internal static void DrawImageDisabled(Graphics graphics, Image image, Rectangle imageBounds, Color background, bool unscaledImage) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (image == null) {
                throw new ArgumentNullException(nameof(image));
            }
#if GRAYSCALE_DISABLED
            Size imageSize = image.Size;

            if (disabledImageAttr == null) {
                // This is how I came up with this somewhat random ColorMatrix.
                // Its set to resemble Office10 commandbars, but still be able to
                // deal with hi-color (256+) icons and images.
                //
                // The idea is to scale everything down (more than just a grayscale does,
                // therefore the small numbers in the scaling part of matrix)
                // White -> some shade of gray &
                // Black -> Black
                //
                // Second part of the matrix is to translate everything, so all colors are
                // a bit brigher.
                // Grays become lighter and washed out looking
                // Black becomes a shade of gray as well.
                //
                // btw, if you do come up with something better let me know - Microsoft
                
                float[][] array = new float[5][];
                    array[0] = new float[5] {0.2125f, 0.2125f, 0.2125f, 0, 0};
                array[1] = new float[5] {0.2577f, 0.2577f, 0.2577f, 0, 0};
                array[2] = new float[5] {0.0361f, 0.0361f, 0.0361f, 0, 0};
                array[3] = new float[5] {0,       0,       0,       1, 0};
                array[4] = new float[5] {0.38f,   0.38f,   0.38f,   0, 1};

                ColorMatrix grayMatrix = new ColorMatrix(array);

                disabledImageAttr = new ImageAttributes();
                disabledImageAttr.ClearColorKey();
                disabledImageAttr.SetColorMatrix(grayMatrix);
            }


            if (unscaledImage) {
                using (Bitmap bmp = new Bitmap(image.Width, image.Height)) {
                    using (Graphics g = Graphics.FromImage(bmp)) {
                        g.DrawImage(image, 
                                   new Rectangle(0, 0, imageSize.Width, imageSize.Height),
                                   0, 0, imageSize.Width, imageSize.Height,
                                   GraphicsUnit.Pixel, 
                                   disabledImageAttr);
                    }
                    graphics.DrawImageUnscaled(bmp, imageBounds);
                }
            }
            else {
                graphics.DrawImage(image, 
                                   imageBounds, 
                                   0, 0, imageSize.Width, imageSize.Height,
                                   GraphicsUnit.Pixel, 
                                   disabledImageAttr);
            }
#else


            // This is remarkably simple -- make a monochrome version of the image, draw once
            // with the button highlight color, then a second time offset by one pixel
            // and in the button shadow color.
            // Technique borrowed from comctl Toolbar.

            Bitmap bitmap;
            bool disposeBitmap = false;
            if (image is Bitmap)
                bitmap = (Bitmap) image;
            else {
                // metafiles can have extremely high resolutions,
                // so if we naively turn them into bitmaps, the performance will be very poor.
                // bitmap = new Bitmap(image);

                GraphicsUnit units = GraphicsUnit.Display;
                RectangleF bounds = image.GetBounds(ref units);
                bitmap = new Bitmap((int) (bounds.Width * graphics.DpiX / image.HorizontalResolution),
                                    (int) (bounds.Height * graphics.DpiY / image.VerticalResolution));

                Graphics bitmapGraphics = Graphics.FromImage(bitmap);
                bitmapGraphics.Clear(Color.Transparent);
                bitmapGraphics.DrawImage(image, 0, 0, image.Size.Width, image.Size.Height);
                bitmapGraphics.Dispose();

                disposeBitmap = true;
            }
            
            Color highlight = ControlPaint.LightLight(background);
            Bitmap monochrome = MakeMonochrome(bitmap, highlight);
            graphics.DrawImage(monochrome, new Rectangle(imageBounds.X + 1, imageBounds.Y + 1, imageBounds.Width, imageBounds.Height));
            monochrome.Dispose();

            Color shadow = ControlPaint.Dark(background);
            monochrome = MakeMonochrome(bitmap, shadow);
            graphics.DrawImage(monochrome, imageBounds);
            monochrome.Dispose();

            if (disposeBitmap)
                bitmap.Dispose();
#endif
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawLockedFrame"]/*' />
        /// <devdoc>
        ///     Draws a locked selection frame around the given rectangle.
        /// </devdoc>
        public static void DrawLockedFrame(Graphics graphics, Rectangle rectangle, bool primary) {
            Pen pen;

            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (primary) {
                pen = Pens.White;
            }
            else {
                pen = Pens.Black;
            }

            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
            rectangle.Inflate(-1, -1);
            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);

            if (primary) {
                pen = Pens.Black;
            }
            else {
                pen = Pens.White;
            }
            rectangle.Inflate(-1, -1);
            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawMenuGlyph"]/*' />
        /// <devdoc>
        ///     Draws a menu glyph for a Win32 menu in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawMenuGlyph(Graphics graphics, Rectangle rectangle, MenuGlyph glyph) {
            DrawMenuGlyph(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, glyph);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawMenuGlyph0"]/*' />
        /// <devdoc>
        ///     Draws a menu glyph for a Win32 menu in the given rectangle with the given state.
        ///     White color is replaced with backColor, Black is replaced with foreColor.
        /// </devdoc>
        public static void DrawMenuGlyph(Graphics graphics, Rectangle rectangle, MenuGlyph glyph, Color foreColor, Color backColor)
        {
            DrawMenuGlyph(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, glyph, foreColor, backColor);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawMenuGlyph1"]/*' />
        /// <devdoc>
        ///     Draws a menu glyph for a Win32 menu in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawMenuGlyph(Graphics graphics, int x, int y, int width, int height, MenuGlyph glyph) {
            DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_MENU, 
                             (int) glyph, Color.Empty, Color.Empty);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawMenuGlyph2"]/*' />
        /// <devdoc>
        ///     Draws a menu glyph for a Win32 menu in the given rectangle with the given state.
        ///     White color is replaced with backColor, Black is replaced with foreColor.
        /// </devdoc>
        public static void DrawMenuGlyph(Graphics graphics, int x, int y, int width, int height, MenuGlyph glyph, Color foreColor, Color backColor)
        {
            DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_MENU, (int)glyph, foreColor, backColor);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawMixedCheckBox"]/*' />
        /// <devdoc>
        ///     Draws a Win32 3-state checkbox control in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawMixedCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state) {
            DrawMixedCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawMixedCheckBox1"]/*' />
        public static void DrawMixedCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState state) {
            DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_BUTTON,
                             NativeMethods.DFCS_BUTTON3STATE | (int) state, Color.Empty, Color.Empty);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawRadioButton"]/*' />
        /// <devdoc>
        ///     Draws a Win32 radio button in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawRadioButton(Graphics graphics, Rectangle rectangle, ButtonState state) {
            DrawRadioButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawRadioButton1"]/*' />
        /// <devdoc>
        ///     Draws a Win32 radio button in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawRadioButton(Graphics graphics, int x, int y, int width, int height, ButtonState state) {
            DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_BUTTON,
                             NativeMethods.DFCS_BUTTONRADIO | ((int)state), Color.Empty, Color.Empty);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawReversibleFrame"]/*' />
        /// <devdoc>
        ///      Draws a rectangular frame on the screen.  The operation of this can be
        ///      "reversed" by drawing the same rectangle again.  This is similar to
        ///      inverting a region of the screen except that it behaves better for
        ///      a wider variety of colors.
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        public static void DrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style) {
            int rop2;
            Color graphicsColor;

            if (backColor.GetBrightness() < .5) {
                rop2 = 0xA; // RasterOp.PEN.Invert().XorWith(RasterOp.TARGET);
                graphicsColor = Color.White;
            }
            else {
                rop2 = 0x7; // RasterOp.PEN.XorWith(RasterOp.TARGET);
                graphicsColor = Color.Black;
            }

            IntPtr dc = UnsafeNativeMethods.GetDCEx(new HandleRef(null, UnsafeNativeMethods.GetDesktopWindow()), NativeMethods.NullHandleRef, NativeMethods.DCX_WINDOW | NativeMethods.DCX_LOCKWINDOWUPDATE | NativeMethods.DCX_CACHE);
            IntPtr pen;

            switch (style) {
                case FrameStyle.Dashed:
                    pen = SafeNativeMethods.CreatePen(NativeMethods.PS_DOT, 1, ColorTranslator.ToWin32(backColor));
                    break;

                case FrameStyle.Thick:
                default:
                    pen = SafeNativeMethods.CreatePen(NativeMethods.PS_SOLID, 2, ColorTranslator.ToWin32(backColor));
                    break;
            }

            int prevRop2 = SafeNativeMethods.SetROP2(new HandleRef(null, dc), rop2);
            IntPtr oldBrush = SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, UnsafeNativeMethods.GetStockObject(NativeMethods.HOLLOW_BRUSH)));
            IntPtr oldPen = SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, pen));
            SafeNativeMethods.SetBkColor(new HandleRef(null, dc), ColorTranslator.ToWin32(graphicsColor));
            SafeNativeMethods.Rectangle(new HandleRef(null, dc), rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);

            SafeNativeMethods.SetROP2(new HandleRef(null, dc), prevRop2);
            SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, oldBrush));
            SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, oldPen));

            if (pen != IntPtr.Zero)
            { 
                SafeNativeMethods.DeleteObject(new HandleRef(null, pen));
            }

            UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, dc));
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawReversibleLine"]/*' />
        /// <devdoc>
        ///      Draws a reversible line on the screen.  A reversible line can
        ///      be erased by just drawing over it again.
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        public static void DrawReversibleLine(Point start, Point end, Color backColor) {
            int rop2 = GetColorRop(backColor, 
                                   0xA, // RasterOp.PEN.Invert().XorWith(RasterOp.TARGET), 
                                   0x7); //RasterOp.PEN.XorWith(RasterOp.TARGET));

            IntPtr dc = UnsafeNativeMethods.GetDCEx(new HandleRef(null, UnsafeNativeMethods.GetDesktopWindow()), NativeMethods.NullHandleRef, NativeMethods.DCX_WINDOW | NativeMethods.DCX_LOCKWINDOWUPDATE | NativeMethods.DCX_CACHE);
            IntPtr pen = SafeNativeMethods.CreatePen(NativeMethods.PS_SOLID, 1, ColorTranslator.ToWin32(backColor));

            int prevRop2 = SafeNativeMethods.SetROP2(new HandleRef(null, dc), rop2);
            IntPtr oldBrush = SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, UnsafeNativeMethods.GetStockObject(NativeMethods.HOLLOW_BRUSH)));
            IntPtr oldPen = SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, pen));


            SafeNativeMethods.MoveToEx(new HandleRef(null, dc), start.X, start.Y, null);
            SafeNativeMethods.LineTo(new HandleRef(null, dc), end.X, end.Y);

            SafeNativeMethods.SetROP2(new HandleRef(null, dc), prevRop2);
            SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, oldBrush));
            SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, oldPen));
            SafeNativeMethods.DeleteObject(new HandleRef(null, pen));
            UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, dc));
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawScrollButton"]/*' />
        /// <devdoc>
        ///     Draws a button for a Win32 scroll bar in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawScrollButton(Graphics graphics, Rectangle rectangle, ScrollButton button, ButtonState state) {
            DrawScrollButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, button, state);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawScrollButton1"]/*' />
        /// <devdoc>
        ///     Draws a button for a Win32 scroll bar in the given rectangle with the given state.
        /// </devdoc>
        public static void DrawScrollButton(Graphics graphics, int x, int y, int width, int height, ScrollButton button, ButtonState state) {
            DrawFrameControl(graphics, x, y, width, height, NativeMethods.DFC_SCROLL,
                             (int)button | (int)state, Color.Empty, Color.Empty);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawSelectionFrame"]/*' />
        /// <devdoc>
        ///      Draws a standard selection frame.  A selection frame is a frame that is
        ///      drawn around a selected component at design time.
        /// </devdoc>
        public static void DrawSelectionFrame(Graphics graphics, bool active, Rectangle outsideRect, Rectangle insideRect, Color backColor) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }

            Brush frameBrush;            
            if (active) {
                frameBrush = GetActiveBrush(backColor);
            }
            else {
                frameBrush = GetSelectedBrush(backColor);
            }

            Region clip = graphics.Clip;
            graphics.ExcludeClip(insideRect);
            graphics.FillRectangle(frameBrush, outsideRect);
            graphics.Clip = clip;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawSizeGrip"]/*' />
        /// <devdoc>
        ///      Draws a size grip at the given location.  The color of the size grip is based
        ///      on the given background color.
        /// </devdoc>
        public static void DrawSizeGrip(Graphics graphics, Color backColor, Rectangle bounds) {
            DrawSizeGrip(graphics, backColor, bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawSizeGrip1"]/*' />
        /// <devdoc>
        ///      Draws a size grip at the given location.  The color of the size grip is based
        ///      on the given background color.
        /// </devdoc>
        public static void DrawSizeGrip(Graphics graphics, Color backColor, int x, int y, int width, int height) {

            // Note: We don't paint any background to facilitate transparency, background images, etc...
            //
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }

            using( Pen bright = new Pen(LightLight(backColor)) ) {
                using ( Pen dark = new Pen(Dark(backColor)) ) {

                    int minDim = Math.Min(width, height);
                    int right = x+width-1;
                    int bottom = y+height-2;

                    for (int i=0; i<minDim - 4; i+= 4) {
                        graphics.DrawLine(dark, right - (i + 1) - 2, bottom, right, bottom - (i + 1) - 2);
                        graphics.DrawLine(dark, right - (i + 2) - 2, bottom, right, bottom - (i + 2) - 2);
                        graphics.DrawLine(bright, right - (i + 3) - 2, bottom, right, bottom - (i + 3) - 2);
                    }
                }
            }
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawStringDisabled"]/*' />
        /// <devdoc>
        ///     Draws a string in the style appropriate for disabled items.
        /// </devdoc>
        public static void DrawStringDisabled(Graphics graphics, string s, Font font,
                                              Color color, RectangleF layoutRectangle,
                                              StringFormat format) {

            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (SystemInformation.HighContrast && AccessibilityImprovements.Level1) {
                // Ignore the foreground color argument and don't do shading in high contrast, 
                // as colors should match the OS-defined ones.
                graphics.DrawString(s, font, SystemBrushes.GrayText, layoutRectangle, format);
            }
            else {
                layoutRectangle.Offset(1, 1);
                using (SolidBrush brush = new SolidBrush(LightLight(color))) {
                    graphics.DrawString(s, font, brush, layoutRectangle, format);

                    layoutRectangle.Offset(-1, -1);
                    color = Dark(color);
                    brush.Color = color;
                    graphics.DrawString(s, font, brush, layoutRectangle, format);
                }
            }
        }

        /// <devdoc>
        ///     Draws a string in the style appropriate for disabled items, using GDI-based TextRenderer.
        /// </devdoc>
        public static void DrawStringDisabled(IDeviceContext dc, string s, Font font, 
                                              Color color, Rectangle layoutRectangle,
                                              TextFormatFlags format) {
            if (dc == null) {
                throw new ArgumentNullException(nameof(dc));
            }

            if (SystemInformation.HighContrast && AccessibilityImprovements.Level1) {
                TextRenderer.DrawText(dc, s, font, layoutRectangle, SystemColors.GrayText, format);
            }
            else {
                layoutRectangle.Offset(1, 1);
                Color paintcolor = LightLight(color);
           
                TextRenderer.DrawText(dc, s, font, layoutRectangle, paintcolor, format);
                layoutRectangle.Offset(-1, -1);
                paintcolor = Dark(color);
                TextRenderer.DrawText(dc, s, font, layoutRectangle, paintcolor, format);
            }
        }


        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.DrawVisualStyleBorder"]/*' />
        /// <devdoc>
        ///     Draws a string in the style appropriate for disabled items.
        /// </devdoc>
        public static void DrawVisualStyleBorder(Graphics graphics, Rectangle bounds) {
            if (graphics == null) {
                throw new ArgumentNullException(nameof(graphics));
            }
            using (Pen borderPen = new Pen(System.Windows.Forms.VisualStyles.VisualStyleInformation.TextControlBorder)) {
                graphics.DrawRectangle(borderPen, bounds);
            }
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.FillReversibleRectangle"]/*' />
        /// <devdoc>
        ///      Draws a filled rectangle on the screen.  The operation of this can be
        ///      "reversed" by drawing the same rectangle again.  This is similar to
        ///      inverting a region of the screen except that it behaves better for
        ///      a wider variety of colors.
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        public static void FillReversibleRectangle(Rectangle rectangle, Color backColor) {
            int rop3 = GetColorRop(backColor, 
                                   0xa50065, // RasterOp.BRUSH.Invert().XorWith(RasterOp.TARGET), 
                                   0x5a0049); // RasterOp.BRUSH.XorWith(RasterOp.TARGET));
            int rop2 = GetColorRop(backColor, 
                                   0x6, // RasterOp.BRUSH.Invert().XorWith(RasterOp.TARGET), 
                                   0x6); // RasterOp.BRUSH.XorWith(RasterOp.TARGET));

            IntPtr dc = UnsafeNativeMethods.GetDCEx(new HandleRef(null, UnsafeNativeMethods.GetDesktopWindow()), NativeMethods.NullHandleRef, NativeMethods.DCX_WINDOW | NativeMethods.DCX_LOCKWINDOWUPDATE | NativeMethods.DCX_CACHE);
            IntPtr brush = SafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(backColor));

            int prevRop2 = SafeNativeMethods.SetROP2(new HandleRef(null, dc), rop2);
            IntPtr oldBrush = SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, brush));

            // PatBlt must be the only Win32 function that wants height in width rather than x2,y2.
            SafeNativeMethods.PatBlt(new HandleRef(null, dc), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, rop3);

            SafeNativeMethods.SetROP2(new HandleRef(null, dc), prevRop2);
            SafeNativeMethods.SelectObject(new HandleRef(null, dc), new HandleRef(null, oldBrush));
            SafeNativeMethods.DeleteObject(new HandleRef(null, brush));
            UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, dc));
        }

        // Converts the font into one where Font.Unit = Point.
        // If the original font is in device-dependent units (and it usually is),
        // we interpret the size relative to the screen.
        //
        // This is not really a general-purpose function -- when used on something
        // not obtained from ChooseFont, it may round away some precision.
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        internal static Font FontInPoints(Font font) {
            return new Font(font.FontFamily, font.SizeInPoints, font.Style, GraphicsUnit.Point, font.GdiCharSet, font.GdiVerticalFont);
        }

        // Returns whether or not target was changed
        internal static bool FontToIFont(Font source, UnsafeNativeMethods.IFont target) {
            bool changed = false; 

            // we need to go through all the pain of the diff here because
            // it looks like setting them all has different results based on the
            // order and each individual IFont implementor...
            //
            string fontName = target.GetName();
            if (!source.Name.Equals(fontName)) {
                target.SetName(source.Name);
                changed = true;
            }

            // Microsoft, Review: this always seems to come back as
            // the point size * 10000 (HIMETRIC?), regardless
            // or ratio or mapping mode, and despite what
            // the documentation says...
            //
            // Either figure out what's going on here or
            // do the process that the windows forms FONT object does here
            // or, worse case, just create another Font object
            // from the handle, but that's pretty heavy...
            //
            float fontSize = (float)target.GetSize() / 10000;

            // size must be in points
            float winformsSize = source.SizeInPoints;
            if (winformsSize != fontSize) {
                target.SetSize((long)(winformsSize * 10000));
                changed = true;
            }

            NativeMethods.LOGFONT logfont = new NativeMethods.LOGFONT();

            IntSecurity.ObjectFromWin32Handle.Assert();
            try {
                source.ToLogFont(logfont);
            }
            finally {
                CodeAccessPermission.RevertAssert();
            }

            short fontWeight = target.GetWeight();
            if (fontWeight != logfont.lfWeight) {
                target.SetWeight((short)logfont.lfWeight);
                changed = true;
            }

            bool fontBold = target.GetBold();
            if (fontBold != (logfont.lfWeight >= 700)) {
                target.SetBold(logfont.lfWeight >= 700);
                changed = true;
            }

            bool fontItalic = target.GetItalic();
            if (fontItalic != (0 != logfont.lfItalic)) {
                target.SetItalic(0 != logfont.lfItalic);
                changed = true;
            }

            bool fontUnderline = target.GetUnderline();
            if (fontUnderline != (0 != logfont.lfUnderline)) {
                target.SetUnderline(0 != logfont.lfUnderline);
                changed = true;
            }

            bool fontStrike = target.GetStrikethrough();
            if (fontStrike != (0 != logfont.lfStrikeOut)) {
                target.SetStrikethrough(0 != logfont.lfStrikeOut);
                changed = true;
            }

            short fontCharset = target.GetCharset();
            if (fontCharset != logfont.lfCharSet) {
                target.SetCharset(logfont.lfCharSet);
                changed = true;
            }

            return changed;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.GetColorRop"]/*' />
        /// <devdoc>
        ///     This makes a choice from a set of raster op codes, based on the color given.  If the
        ///     color is considered to be "dark", the raster op provided by dark will be returned.
        /// </devdoc>
        private static int GetColorRop(Color color, int darkROP, int lightROP) {
            if (color.GetBrightness() < .5) {
                return darkROP;
            }
            return lightROP;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.GetActiveBrush"]/*' />
        /// <devdoc>
        ///      Retrieves the brush used to draw active objects.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process | ResourceScope.Machine, ResourceScope.Machine)]
        private static Brush GetActiveBrush(Color backColor) {
            Color brushColor;

            if (backColor.GetBrightness() <= .5) {
                brushColor = SystemColors.ControlLight;
            }
            else {
                brushColor = SystemColors.ControlDark;
            }
            
            if (frameBrushActive == null ||
                !frameColorActive.Equals(brushColor)) {

                if (frameBrushActive != null) {
                    frameBrushActive.Dispose();
                    frameBrushActive = null;
                }

                frameColorActive = brushColor;

                int patternSize = 8;

                Bitmap bitmap = new Bitmap(patternSize, patternSize);

                // gpr : bitmap does not initialize itself to be zero?
                //
                for (int x = 0; x < patternSize; x++) {
                    for (int y = 0; y < patternSize; y++) {
                        bitmap.SetPixel(x, y, Color.Transparent);
                    }
                }

                for (int y = 0; y < patternSize; y++) {
                    for (int x = -y; x < patternSize; x += 4) {
                        if (x >= 0) {
                            bitmap.SetPixel(x, y, brushColor);
                        }
                    }
                }

                frameBrushActive = new TextureBrush(bitmap);
                bitmap.Dispose();
            }

            return frameBrushActive;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.GetFocusPen"]/*' />
        /// <devdoc>
        ///      Retrieves the pen used to draw a focus rectangle around a control.  The focus
        ///      rectangle is typically drawn when the control has keyboard focus.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process | ResourceScope.Machine, ResourceScope.Machine)]
        private static Pen GetFocusPen(Color baseColor, bool odds, bool highContrast) {
            if (focusPen == null ||
                (!highContrast && focusPenColor.GetBrightness() <= .5 && baseColor.GetBrightness() <= .5) ||
                focusPenColor.ToArgb() != baseColor.ToArgb() ||
                hcFocusPen != highContrast) {

                if (focusPen != null) {
                    focusPen.Dispose();
                    focusPen = null;
                    focusPenInvert.Dispose();
                    focusPenInvert = null;
                }

                focusPenColor = baseColor;
                hcFocusPen = highContrast;

                Bitmap b = new Bitmap(2,2);
                Color color1 = Color.Transparent;
                Color color2;
                if (highContrast) {
                    // in highcontrast mode "baseColor" itself is used as the focus pen color
                    color2 = baseColor;
                }
                else {
                    // in non-highcontrast mode "baseColor" is used to calculate the focus pen colors
                    // in this mode "baseColor" is expected to contain background color of the control to do this calculation properly
                    color2 = Color.Black;

                    if (baseColor.GetBrightness() <= .5) {
                        color1 = color2;
                        color2 = InvertColor(baseColor);
                    }
                    else if (baseColor == Color.Transparent) {
                        color1 = Color.White;
                    }
                }

                b.SetPixel(1, 0, color2);
                b.SetPixel(0, 1, color2);
                b.SetPixel(0, 0, color1);
                b.SetPixel(1, 1, color1);

                Brush brush = new TextureBrush(b);
                focusPen = new Pen(brush, 1);
                brush.Dispose(); // The Pen constructor copies what it needs from the brush

                b.SetPixel(1, 0, color1);
                b.SetPixel(0, 1, color1);
                b.SetPixel(0, 0, color2);
                b.SetPixel(1, 1, color2);

                brush = new TextureBrush(b);
                focusPenInvert = new Pen(brush, 1);
                brush.Dispose();

                b.Dispose();
            }

            return odds ? focusPen : focusPenInvert;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.GetSelectedBrush"]/*' />
        /// <devdoc>
        ///      Retrieves the brush used to draw selected objects.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process | ResourceScope.Machine, ResourceScope.Machine)]
        private static Brush GetSelectedBrush(Color backColor) {
            Color brushColor;

            if (backColor.GetBrightness() <= .5) {
                brushColor = SystemColors.ControlLight;
            }
            else {
                brushColor = SystemColors.ControlDark;
            }
            if (frameBrushSelected == null ||
                !frameColorSelected.Equals(brushColor)) {

                if (frameBrushSelected != null) {
                    frameBrushSelected.Dispose();
                    frameBrushSelected = null;
                }

                frameColorSelected = brushColor;

                int patternSize = 8;

                Bitmap bitmap = new Bitmap(patternSize, patternSize);

                // gpr : bitmap does not initialize itself to be zero?
                //
                for (int x = 0; x < patternSize; x++) {
                    for (int y = 0; y < patternSize; y++) {
                        bitmap.SetPixel(x, y, Color.Transparent);
                    }
                }

                int start = 0;

                for (int x = 0; x < patternSize; x += 2) {
                    for (int y = start; y < patternSize; y += 2) {
                        bitmap.SetPixel(x, y, brushColor);
                    }

                    start ^= 1;
                }

                frameBrushSelected = new TextureBrush(bitmap);
                bitmap.Dispose();
            }

            return frameBrushSelected;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.InfinityToOne"]/*' />
        /// <devdoc>
        ///      Converts an infinite value to "1".
        /// </devdoc>
        private static float InfinityToOne(float value) {
            if (value == Single.NegativeInfinity || value == Single.PositiveInfinity) {
                return 1.0f;
            }
            return value;
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.InvertColor"]/*' />
        /// <devdoc>
        ///      Inverts the given color.
        /// </devdoc>
        private static Color InvertColor(Color color) {
            return Color.FromArgb(color.A, (byte)~color.R, (byte)~color.G, (byte)~color.B);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.Light"]/*' />
        /// <devdoc>
        ///      Creates a new color that is a object of the given color.
        /// </devdoc>
        public static Color Light(Color baseColor, float percOfLightLight) {
            return new HLSColor(baseColor).Lighter(percOfLightLight);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.Light1"]/*' />
        /// <devdoc>
        ///      Creates a new color that is a object of the given color.
        /// </devdoc>
        public static Color Light(Color baseColor) {
            return new HLSColor(baseColor).Lighter(0.5f);
        }

        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.LightLight"]/*' />
        /// <devdoc>
        ///      Creates a new color that is a object of the given color.
        /// </devdoc>
        public static Color LightLight(Color baseColor) {
            return new HLSColor(baseColor).Lighter(1.0f);
        }

#if !GRAYSCALE_DISABLED
        // Returns a monochrome bitmap based on the input.
        private static Bitmap MakeMonochrome(Bitmap input, Color color) {
            Bitmap output = new Bitmap(input.Width, input.Height);
            output.SetResolution(input.HorizontalResolution, input.VerticalResolution);        
            Size size = input.Size;
            int width = input.Width;
            int height = input.Height;

            BitmapData inputData = input.LockBits(new Rectangle(0,0, width, height),
                                                  ImageLockMode.ReadOnly,
                                                  PixelFormat.Format32bppArgb);
            BitmapData outputData = output.LockBits(new Rectangle(0,0, width, height),
                                                    ImageLockMode.WriteOnly,
                                                    PixelFormat.Format32bppArgb);

            Debug.Assert(inputData.Scan0 != IntPtr.Zero && outputData.Scan0 != IntPtr.Zero, "BitmapData.Scan0 is null; check marshalling");

            int colorARGB = color.ToArgb();
            for (int y = 0; y < height; y++) {
                IntPtr inputScan = (IntPtr)((long)inputData.Scan0 + y * inputData.Stride);
                IntPtr outputScan = (IntPtr)((long)outputData.Scan0 + y * outputData.Stride);
                for (int x = 0; x < width; x++) {
                    int pixel = Marshal.ReadInt32(inputScan,x*4);
                    if (pixel >> 24 == 0)
                        Marshal.WriteInt32(outputScan, x*4, 0); // transparent
                    else
                        Marshal.WriteInt32(outputScan, x*4, colorARGB);
                }
            }
            input.UnlockBits(inputData);
            output.UnlockBits(outputData);

            return output;
        }
#endif

        internal static ColorMatrix MultiplyColorMatrix(float[][] matrix1, float[][] matrix2) {
            int size = 5; // multiplies 2 5x5 matrices.

            // build up an empty 5x5 array for results
            float[][] result = new float[size][];
            for (int row = 0; row < size; row++){
                 result[row] = new float[size];
            }
           
            float[] column = new float[size];
            for (int j = 0; j < size; j++) {
                for (int k = 0; k < size; k++) {
                    column[k] = matrix1[k][j];
                }
                for (int i = 0; i < size; i++) {
                    float[] row = matrix2[i];
                    float s = 0;
                    for (int k = 0; k < size; k++) {
                        s += row[k] * column[k];
                    }
                    result[i][j] = s;
                 } 
            }
            
            return new ColorMatrix(result);

        }
        //paint the border of the table
        internal static void PaintTableControlBorder(TableLayoutPanelCellBorderStyle borderStyle, Graphics g, Rectangle bound) {
            int x = bound.X;
            int y = bound.Y;
            int right = bound.Right;
            int bottom  = bound.Bottom;
            //draw the outside bounding rectangle
            switch(borderStyle) {
                case TableLayoutPanelCellBorderStyle.None:
                case TableLayoutPanelCellBorderStyle.Single:
                    break;
                    
                case TableLayoutPanelCellBorderStyle.Inset:
                case TableLayoutPanelCellBorderStyle.InsetDouble:
                    g.DrawLine(SystemPens.ControlDark, x, y, right - 1, y);
                    g.DrawLine(SystemPens.ControlDark, x, y, x, bottom - 1);
                    using (Pen pen = new Pen(SystemColors.Window)) {
                        g.DrawLine(pen, right - 1, y, right - 1, bottom - 1);
                        g.DrawLine(pen, x, bottom - 1, right - 1, bottom - 1);
                    }
                    break;
                    
                case TableLayoutPanelCellBorderStyle.Outset:
                case TableLayoutPanelCellBorderStyle.OutsetDouble:
                case TableLayoutPanelCellBorderStyle.OutsetPartial:
                    using (Pen pen = new Pen(SystemColors.Window)) {
                        g.DrawLine(pen, x, y, right - 1, y);
                        g.DrawLine(pen, x, y, x, bottom - 1);
                    }
                    g.DrawLine(SystemPens.ControlDark, right - 1, y, right - 1, bottom - 1);
                    g.DrawLine(SystemPens.ControlDark, x, bottom - 1, right - 1, bottom - 1);
                    break; 
             }            
        }

        //paint individual cell of the table
        internal static void PaintTableCellBorder(TableLayoutPanelCellBorderStyle borderStyle, Graphics g, Rectangle bound) {
            
            //next, paint the cell border
            switch (borderStyle) {
                case TableLayoutPanelCellBorderStyle.None :
                    break;

                case TableLayoutPanelCellBorderStyle.Single :
                    g.DrawRectangle(SystemPens.ControlDark, bound);
                    break;

                case TableLayoutPanelCellBorderStyle.Inset :
                    using (Pen pen = new Pen(SystemColors.Window)) {
                        g.DrawLine(pen, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                        g.DrawLine(pen, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);
                    }

                    g.DrawLine(SystemPens.ControlDark, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    break;

                case TableLayoutPanelCellBorderStyle.InsetDouble :
                    g.DrawRectangle(SystemPens.Control, bound);

                    //draw the shadow
                    bound = new Rectangle(bound.X + 1, bound.Y + 1, bound.Width - 1, bound.Height - 1);
                    using (Pen pen = new Pen(SystemColors.Window)) {
                        g.DrawLine(pen, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                        g.DrawLine(pen, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);
                    }

                    g.DrawLine(SystemPens.ControlDark, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    break;

                case TableLayoutPanelCellBorderStyle.Outset :
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);
                    using (Pen pen = new Pen(SystemColors.Window)) {
                        g.DrawLine(pen, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                        g.DrawLine(pen, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    }

                    break;

                case TableLayoutPanelCellBorderStyle.OutsetDouble :
                case TableLayoutPanelCellBorderStyle.OutsetPartial :
                    g.DrawRectangle(SystemPens.Control, bound);

                    //draw the shadow
                    bound = new Rectangle(bound.X + 1, bound.Y + 1, bound.Width - 1, bound.Height - 1);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);
                    using (Pen pen = new Pen(SystemColors.Window)) {
                        g.DrawLine(pen, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                        g.DrawLine(pen, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    }

                    break;
            }
        }

        /* Unused
        // Takes a black and white image, and replaces those colors with the colors of your choice.
        // The Alpha channel of the source bitmap will be ignored, meaning pixels with Color.Transparent
        // (really transparent black) will be mapped to the replaceBlack color.
        private static ColorMatrix RemapBlackAndWhiteAndTransparentMatrix(Color replaceBlack, Color replaceWhite) {
            // Normalize the colors to 1.0.

            float normBlackRed   = ((float)replaceBlack.R)/(float)255.0;
            float normBlackGreen = ((float)replaceBlack.G)/(float)255.0;
            float normBlackBlue  = ((float)replaceBlack.B)/(float)255.0;
            float normBlackAlpha = ((float)replaceBlack.A)/(float)255.0;

            float normWhiteRed   = ((float)replaceWhite.R)/(float)255.0;
            float normWhiteGreen = ((float)replaceWhite.G)/(float)255.0;
            float normWhiteBlue  = ((float)replaceWhite.B)/(float)255.0;
            float normWhiteAlpha = ((float)replaceWhite.A)/(float)255.0;

            // Set up a matrix that will map white to replaceWhite and 
            // black and transparent black to replaceBlack.
            //
            //                | -B  -B  -B  -B   0 |
            //                |   r   g   b   a    |
            //                |                    |
            //                |  W   W   W   W   0 |
            //                |   r   g   b   a    |
            //                |                    |
            //  [ R G B A ] * |  0   0   0   0   0 | = [ R' G' B' A' ]
            //                |                    |
            //                |                    |
            //                |  0   0   0   0   0 |
            //                |                    |
            //                |                    |
            //                |  B   B   B   B   1 |
            //                |   r   g   b   a    |

            ColorMatrix matrix = new ColorMatrix();

            matrix.Matrix00 = -normBlackRed;
            matrix.Matrix01 = -normBlackGreen;
            matrix.Matrix02 = -normBlackBlue;
            matrix.Matrix03 = -normBlackAlpha;

            matrix.Matrix10 =  normWhiteRed;
            matrix.Matrix11 =  normWhiteGreen;
            matrix.Matrix12 =  normWhiteBlue;
            matrix.Matrix13 =  normWhiteAlpha;

            matrix.Matrix40 =  normBlackRed;
            matrix.Matrix41 =  normBlackGreen;
            matrix.Matrix42 =  normBlackBlue;
            matrix.Matrix43 =  normBlackAlpha;
            matrix.Matrix44 =  1.0f;

            return matrix;
        }
        */

        // Takes a black and white image, and replaces those colors with the colors of your choice.
        // The replaceBlack and replaceWhite colors must have alpha = 255, because the alpha value
        // of the bitmap is preserved.
        private static ColorMatrix RemapBlackAndWhitePreserveTransparentMatrix(Color replaceBlack, Color replaceWhite) {
            Debug.Assert(replaceBlack.A == 255, "replaceBlack.Alpha is ignored, so please set it to 255 so I know you know what you're doing");
            Debug.Assert(replaceWhite.A == 255, "replaceWhite.Alpha is ignored, so please set it to 255 so I know you know what you're doing");

            // Normalize the colors to 1.0.

            float normBlackRed   = ((float)replaceBlack.R)/(float)255.0;
            float normBlackGreen = ((float)replaceBlack.G)/(float)255.0;
            float normBlackBlue  = ((float)replaceBlack.B)/(float)255.0;
            float normBlackAlpha = ((float)replaceBlack.A)/(float)255.0;

            float normWhiteRed   = ((float)replaceWhite.R)/(float)255.0;
            float normWhiteGreen = ((float)replaceWhite.G)/(float)255.0;
            float normWhiteBlue  = ((float)replaceWhite.B)/(float)255.0;
            float normWhiteAlpha = ((float)replaceWhite.A)/(float)255.0;

            // Set up a matrix that will map white to replaceWhite and 
            // black to replaceBlack, using the source bitmap's alpha value for the output
            //
            //                | -B  -B  -B   0   0 |
            //                |   r   g   b        |
            //                |                    |
            //                |  W   W   W   0   0 |
            //                |   r   g   b        |
            //                |                    |
            //  [ R G B A ] * |  0   0   0   0   0 | = [ R' G' B' A ]
            //                |                    |
            //                |                    |
            //                |  0   0   0   1   0 |
            //                |                    |
            //                |                    |
            //                |  B   B   B   0   1 |
            //                |   r   g   b        |

            ColorMatrix matrix = new ColorMatrix();

            matrix.Matrix00 = -normBlackRed;
            matrix.Matrix01 = -normBlackGreen;
            matrix.Matrix02 = -normBlackBlue;

            matrix.Matrix10 =  normWhiteRed;
            matrix.Matrix11 =  normWhiteGreen;
            matrix.Matrix12 =  normWhiteBlue;

            matrix.Matrix33 =  1.0f;

            matrix.Matrix40 =  normBlackRed;
            matrix.Matrix41 =  normBlackGreen;
            matrix.Matrix42 =  normBlackBlue;
            matrix.Matrix44 =  1.0f;

            return matrix;
        }
        
        /* Unused
        internal static StringAlignment TranslateAlignment(HorizontalAlignment align) {
            StringAlignment result;
            switch (align) {
                case HorizontalAlignment.Right:
                    result = StringAlignment.Far;
                    break;
                case HorizontalAlignment.Center:
                    result = StringAlignment.Center;
                    break;
                case HorizontalAlignment.Left:
                default:
                    result = StringAlignment.Near;
                    break;
            }

            return result;
        }
        */

        internal static TextFormatFlags TextFormatFlagsForAlignmentGDI(ContentAlignment align) {
            TextFormatFlags output = new TextFormatFlags();
            output |= TranslateAlignmentForGDI(align);
            output |= TranslateLineAlignmentForGDI(align);
            return output;
        }

        internal static StringAlignment TranslateAlignment(ContentAlignment align) {
            StringAlignment result;
            if ((align & anyRight) != 0)
                result = StringAlignment.Far;
            else if ((align & anyCenter) != 0)
                result = StringAlignment.Center;
            else
                result = StringAlignment.Near;
            return result;
        }


        internal static  TextFormatFlags TranslateAlignmentForGDI(ContentAlignment align) {
            TextFormatFlags result;
            if ((align & anyBottom) != 0)
                result =  TextFormatFlags.Bottom;
            else if ((align & anyMiddle) != 0)
                result =  TextFormatFlags.VerticalCenter;
            else
                result =  TextFormatFlags.Top;
            return result;
        }

     
        internal static StringAlignment TranslateLineAlignment(ContentAlignment align) {
            StringAlignment result;
            if ((align & anyBottom) != 0) {
                result = StringAlignment.Far;
            }
            else if ((align & anyMiddle) != 0) {
                result = StringAlignment.Center;
            }
            else {
                result = StringAlignment.Near;
            }
            return result;
        }

        internal static  TextFormatFlags TranslateLineAlignmentForGDI(ContentAlignment align) {
             TextFormatFlags result;
            if ((align & anyRight) != 0)
                result =  TextFormatFlags.Right;
            else if ((align & anyCenter) != 0)
                result =  TextFormatFlags.HorizontalCenter;
            else
                result =  TextFormatFlags.Left;
            return result;
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        internal static StringFormat StringFormatForAlignment(ContentAlignment align) {
            StringFormat output = new StringFormat();
            output.Alignment = TranslateAlignment(align);
            output.LineAlignment = TranslateLineAlignment(align);
            return output;
        }

        /* Unused
        internal static StringFormat StringFormatForAlignment(HorizontalAlignment align) {
            StringFormat output = new StringFormat();
            output.Alignment = TranslateAlignment(align);
            return output;
        }
        */

        /// <devdoc>
        ///     Get StringFormat object for rendering text using GDI+ (Graphics).
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        internal static StringFormat CreateStringFormat( Control ctl, ContentAlignment textAlign, bool showEllipsis, bool useMnemonic ) {

            StringFormat stringFormat = ControlPaint.StringFormatForAlignment( textAlign );

            // make sure that the text is contained within the label
            // 


            // Adjust string format for Rtl controls
            if( ctl.RightToLeft == RightToLeft.Yes ) {
                stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            if( showEllipsis ) {
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
            }

            if( !useMnemonic ) {
                stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
            }
            else if( ctl.ShowKeyboardCues ) {
                stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
            }
            else {
                stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
            }

            if( ctl.AutoSize ) {
                stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            }

            return stringFormat;
        }

        /// <devdoc>
        ///     Get TextFormatFlags flags for rendering text using GDI (TextRenderer).
        /// </devdoc>
        internal static TextFormatFlags CreateTextFormatFlags(Control ctl, ContentAlignment textAlign, bool showEllipsis, bool useMnemonic ) {

            textAlign = ctl.RtlTranslateContent( textAlign );
            TextFormatFlags flags = ControlPaint.TextFormatFlagsForAlignmentGDI( textAlign );

            // The effect of the TextBoxControl flag is that in-word line breaking will occur if needed, this happens when AutoSize 
            // is false and a one-word line still doesn't fit the binding box (width).  The other effect is that partially visible 
            // lines are clipped; this is how GDI+ works by default.
            flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

            if( showEllipsis ) {
                flags |= TextFormatFlags.EndEllipsis;
            }

            // Adjust string format for Rtl controls
            if( ctl.RightToLeft == RightToLeft.Yes ) {
                flags |= TextFormatFlags.RightToLeft;
            }

            //if we don't use mnemonic, set formatFlag to NoPrefix as this will show the ampersand
            if( !useMnemonic ) {
                flags |= TextFormatFlags.NoPrefix;
            }
            //else if we don't show keyboard cues, set formatFlag to HidePrefix as this will hide
            //the ampersand if we don't press down the alt key
            else if( !ctl.ShowKeyboardCues ) {
                flags |= TextFormatFlags.HidePrefix;
            }

            return flags;
        }


        /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor"]/*' />
        /// <devdoc>
        ///     Logic copied from Win2K sources to copy the lightening and
        ///     darkening of colors.
        /// </devdoc>
        private struct HLSColor {
            private const int ShadowAdj         = -333;
            private const int HilightAdj        = 500;
            private const int WatermarkAdj      = -50;

            private const int Range = 240;
            private const int HLSMax = Range;
            private const int RGBMax = 255;
            private const int Undefined = HLSMax*2/3;

            private int hue;
            private int saturation;
            private int luminosity;

            private bool isSystemColors_Control;

            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.HLSColor"]/*' />
            /// <devdoc>
            /// </devdoc>
            public HLSColor(Color color) {
                isSystemColors_Control = (color.ToKnownColor() == SystemColors.Control.ToKnownColor());
                int r = color.R;
                int g = color.G;
                int b = color.B;
                int max, min;        /* max and min RGB values */
                int sum, dif;
                int  Rdelta,Gdelta,Bdelta;  /* intermediate value: % of spread from max */

                /* calculate lightness */
                max = Math.Max( Math.Max(r,g), b);
                min = Math.Min( Math.Min(r,g), b);
                sum = max + min;

                luminosity = (((sum * HLSMax) + RGBMax)/(2*RGBMax));

                dif = max - min;
                if (dif == 0) {       /* r=g=b --> achromatic case */
                    saturation = 0;                         /* saturation */
                    hue = Undefined;                 /* hue */
                }
                else {                           /* chromatic case */
                    /* saturation */
                    if (luminosity <= (HLSMax/2))
                        saturation = (int) (((dif * (int) HLSMax) + (sum / 2) ) / sum);
                    else
                        saturation = (int) ((int) ((dif * (int) HLSMax) + (int)((2*RGBMax-sum)/2) )
                                            / (2*RGBMax-sum));
                    /* hue */
                    Rdelta = (int) (( ((max-r)*(int)(HLSMax/6)) + (dif / 2) ) / dif);
                    Gdelta = (int) (( ((max-g)*(int)(HLSMax/6)) + (dif / 2) ) / dif);
                    Bdelta = (int) (( ((max-b)*(int)(HLSMax/6)) + (dif / 2) ) / dif);

                    if ((int) r == max)
                        hue = Bdelta - Gdelta;
                    else if ((int)g == max)
                        hue = (HLSMax/3) + Rdelta - Bdelta;
                    else /* B == cMax */
                        hue = ((2*HLSMax)/3) + Gdelta - Rdelta;

                    if (hue < 0)
                        hue += HLSMax;
                    if (hue > HLSMax)
                        hue -= HLSMax;
                }
            }

            /* Unused
            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.Hue"]/*' />
            /// <devdoc>
            /// </devdoc>
            public int Hue {
                get {
                    return hue;
                }
            }
            */

            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.Luminosity"]/*' />
            /// <devdoc>
            /// </devdoc>
            public int Luminosity {
                get {
                    return luminosity;
                }
            }

            /* Unused
            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.Saturation"]/*' />
            /// <devdoc>
            /// </devdoc>
            public int Saturation {
                get {
                    return saturation;
                }
            }
            */

            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.Darker"]/*' />
            /// <devdoc>
            /// </devdoc>
            public Color Darker(float percDarker) {
                if (isSystemColors_Control) {
                    // With the usual color scheme, ControlDark/DarkDark is not exactly
                    // what we would otherwise calculate
                    if (percDarker == 0.0f) {
                        return SystemColors.ControlDark;
                    }
                    else if (percDarker == 1.0f) {
                        return SystemColors.ControlDarkDark;
                    }
                    else {
                        Color dark = SystemColors.ControlDark;
                        Color darkDark = SystemColors.ControlDarkDark;

                        int dr = dark.R - darkDark.R;
                        int dg = dark.G - darkDark.G;
                        int db = dark.B - darkDark.B;

                        return Color.FromArgb((byte)(dark.R - (byte)(dr * percDarker)),
                                              (byte)(dark.G - (byte)(dg * percDarker)),
                                              (byte)(dark.B - (byte)(db * percDarker)));
                    }
                }
                else {
                    int oneLum = 0;
                    int zeroLum = NewLuma(ShadowAdj, true);

                    /*                                        
                    if (luminosity < 40) {
                        zeroLum = NewLuma(120, ShadowAdj, true);
                    }
                    else {
                        zeroLum = NewLuma(ShadowAdj, true);
                    }
                    */

                    return ColorFromHLS(hue, zeroLum - (int)((zeroLum - oneLum) * percDarker), saturation);
                }
            }
            
            public override bool Equals(object o) {
                if (!(o is HLSColor)) {
                    return false;
                }
                
                HLSColor c = (HLSColor)o;
                return hue == c.hue && 
                       saturation == c.saturation && 
                       luminosity == c.luminosity && 
                       isSystemColors_Control == c.isSystemColors_Control;
            }

            public static bool operator ==(HLSColor a, HLSColor b) {            
                return a.Equals(b);
            }

            public static bool operator !=(HLSColor a, HLSColor b) {
                return !a.Equals(b);
            }

            public override int GetHashCode() {
                return hue << 6 | saturation << 2 | luminosity;
            }
    
            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.Lighter"]/*' />
            /// <devdoc>
            /// </devdoc>
            public Color Lighter(float percLighter) {
                if (isSystemColors_Control) {
                    // With the usual color scheme, ControlLight/LightLight is not exactly
                    // what we would otherwise calculate
                    if (percLighter == 0.0f) {
                        return SystemColors.ControlLight;
                    }
                    else if (percLighter == 1.0f) {
                        return SystemColors.ControlLightLight;
                    }
                    else {
                        Color light = SystemColors.ControlLight;
                        Color lightLight = SystemColors.ControlLightLight;

                        int dr = light.R - lightLight.R;
                        int dg = light.G - lightLight.G;
                        int db = light.B - lightLight.B;

                        return Color.FromArgb((byte)(light.R - (byte)(dr * percLighter)),
                                              (byte)(light.G - (byte)(dg * percLighter)),
                                              (byte)(light.B - (byte)(db * percLighter)));
                    }
                }
                else {
                    int zeroLum = luminosity;
                    int oneLum = NewLuma(HilightAdj, true);

                    /*
                    if (luminosity < 40) {
                        zeroLum = 120;
                        oneLum = NewLuma(120, HilightAdj, true);
                    }
                    else {
                        zeroLum = luminosity;
                        oneLum = NewLuma(HilightAdj, true);
                    }
                    */
                    
                    return ColorFromHLS(hue, zeroLum + (int)((oneLum - zeroLum) * percLighter), saturation);
                }
            }

            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.NewLuma"]/*' />
            /// <devdoc>
            /// </devdoc>
            private int NewLuma(int n, bool scale) {
                return NewLuma(luminosity, n, scale);
            }

            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.NewLuma1"]/*' />
            /// <devdoc>
            /// </devdoc>
            private int NewLuma(int luminosity, int n, bool scale) {
                if (n == 0)
                    return luminosity;

                if (scale) {
                    if (n > 0) {
                        return(int)(((int)luminosity * (1000 - n) + (Range + 1L) * n) / 1000);
                    }
                    else {
                        return(int)(((int)luminosity * (n + 1000)) / 1000);
                    }
                }

                int newLum = luminosity;
                newLum += (int)((long)n * Range / 1000);

                if (newLum < 0)
                    newLum = 0;
                if (newLum > HLSMax)
                    newLum = HLSMax;

                return newLum;
            }

            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.ColorFromHLS"]/*' />
            /// <devdoc>
            /// </devdoc>
            private Color ColorFromHLS(int hue, int luminosity, int saturation) {
                byte r,g,b;                      /* RGB component values */
                int  magic1,magic2;       /* calculated magic numbers (really!) */

                if (saturation == 0) {                /* achromatic case */
                    r = g = b = (byte)((luminosity * RGBMax) / HLSMax);
                    if (hue != Undefined) {
                        /* ERROR */
                    }
                }
                else {                         /* chromatic case */
                    /* set up magic numbers */
                    if (luminosity <= (HLSMax/2))
                        magic2 = (int)((luminosity * ((int)HLSMax + saturation) + (HLSMax/2))/HLSMax);
                    else
                        magic2 = luminosity + saturation - (int)(((luminosity*saturation) + (int)(HLSMax/2))/HLSMax);
                    magic1 = 2*luminosity-magic2;

                    /* get RGB, change units from HLSMax to RGBMax */
                    r = (byte)(((HueToRGB(magic1,magic2,(int)(hue+(int)(HLSMax/3)))*(int)RGBMax + (HLSMax/2))) / (int)HLSMax);
                    g = (byte)(((HueToRGB(magic1,magic2,hue)*(int)RGBMax + (HLSMax/2))) / HLSMax);
                    b = (byte)(((HueToRGB(magic1,magic2,(int)(hue-(int)(HLSMax/3)))*(int)RGBMax + (HLSMax/2))) / (int)HLSMax);
                }
                return Color.FromArgb(r,g,b);
            }

            /// <include file='doc\ControlPaint.uex' path='docs/doc[@for="ControlPaint.HLSColor.HueToRGB"]/*' />
            /// <devdoc>
            /// </devdoc>
            private int HueToRGB(int n1, int n2, int hue) {
                /* range check: note values passed add/subtract thirds of range */

                /* The following is redundant for WORD (unsigned int) */
                if (hue < 0)
                    hue += HLSMax;

                if (hue > HLSMax)
                    hue -= HLSMax;

                /* return r,g, or b value from this tridrant */
                if (hue < (HLSMax/6))
                    return( n1 + (((n2-n1)*hue+(HLSMax/12))/(HLSMax/6)) );
                if (hue < (HLSMax/2))
                    return( n2 );
                if (hue < ((HLSMax*2)/3))
                    return( n1 + (((n2-n1)*(((HLSMax*2)/3)-hue)+(HLSMax/12)) / (HLSMax/6)) );
                else
                    return( n1 );

            }

        }
    }
}
