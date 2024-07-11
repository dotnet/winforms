// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

#pragma warning disable CS8618,CS0169,CA1725,CA1821,CA1823,CA1066,IDE0001,IDE0002,IDE1006,IDE0034,IDE0044,IDE0051,IDE0055,IDE1006

namespace System.Drawing
{
    [System.ComponentModel.EditorAttribute("System.Drawing.Design.BitmapEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public sealed partial class Bitmap : System.Drawing.Image
    {
        public Bitmap(System.Drawing.Image original) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(System.Drawing.Image original, System.Drawing.Size newSize) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(System.Drawing.Image original, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(int width, int height, System.Drawing.Graphics g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(int width, int height, System.Drawing.Imaging.PixelFormat format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(int width, int height, int stride, System.Drawing.Imaging.PixelFormat format, System.IntPtr scan0) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(System.IO.Stream stream) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(System.IO.Stream stream, bool useIcm) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(string filename) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(string filename, bool useIcm) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Bitmap(System.Type type, string resource) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Bitmap Clone(System.Drawing.Rectangle rect, System.Drawing.Imaging.PixelFormat format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Bitmap Clone(System.Drawing.RectangleF rect, System.Drawing.Imaging.PixelFormat format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Bitmap FromHicon(System.IntPtr hicon) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Bitmap FromResource(System.IntPtr hinstance, string bitmapName) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IntPtr GetHbitmap() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IntPtr GetHbitmap(System.Drawing.Color background) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IntPtr GetHicon() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Color GetPixel(int x, int y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Imaging.BitmapData LockBits(System.Drawing.Rectangle rect, System.Drawing.Imaging.ImageLockMode flags, System.Drawing.Imaging.PixelFormat format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Imaging.BitmapData LockBits(System.Drawing.Rectangle rect, System.Drawing.Imaging.ImageLockMode flags, System.Drawing.Imaging.PixelFormat format, System.Drawing.Imaging.BitmapData bitmapData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MakeTransparent() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MakeTransparent(System.Drawing.Color transparentColor) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetPixel(int x, int y, System.Drawing.Color color) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetResolution(float xDpi, float yDpi) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void UnlockBits(System.Drawing.Imaging.BitmapData bitmapdata) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.AttributeUsageAttribute(System.AttributeTargets.Assembly)]
    public partial class BitmapSuffixInSameAssemblyAttribute : System.Attribute
    {
        public BitmapSuffixInSameAssemblyAttribute() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.AttributeUsageAttribute(System.AttributeTargets.Assembly)]
    public partial class BitmapSuffixInSatelliteAssemblyAttribute : System.Attribute
    {
        public BitmapSuffixInSatelliteAssemblyAttribute() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public abstract partial class Brush : System.MarshalByRefObject, System.ICloneable, System.IDisposable
    {
        protected Brush() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public abstract object Clone();
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        ~Brush() { }
        protected internal void SetNativeBrush(System.IntPtr brush) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public static partial class Brushes
    {
        public static System.Drawing.Brush AliceBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush AntiqueWhite { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Aqua { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Aquamarine { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Azure { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Beige { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Bisque { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Black { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush BlanchedAlmond { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Blue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush BlueViolet { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Brown { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush BurlyWood { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush CadetBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Chartreuse { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Chocolate { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Coral { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush CornflowerBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Cornsilk { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Crimson { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Cyan { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkCyan { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkGoldenrod { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkKhaki { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkMagenta { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkOliveGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkOrange { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkOrchid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkSalmon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkSeaGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkSlateBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkSlateGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkTurquoise { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DarkViolet { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DeepPink { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DeepSkyBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DimGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush DodgerBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Firebrick { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush FloralWhite { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ForestGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Fuchsia { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Gainsboro { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush GhostWhite { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Gold { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Goldenrod { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Gray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Green { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush GreenYellow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Honeydew { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush HotPink { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush IndianRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Indigo { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Ivory { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Khaki { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Lavender { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LavenderBlush { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LawnGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LemonChiffon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightCoral { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightCyan { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightGoldenrodYellow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightPink { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightSalmon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightSeaGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightSkyBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightSlateGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightSteelBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LightYellow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Lime { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush LimeGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Linen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Magenta { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Maroon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumAquamarine { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumOrchid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumPurple { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumSeaGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumSlateBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumSpringGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumTurquoise { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MediumVioletRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MidnightBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MintCream { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MistyRose { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Moccasin { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush NavajoWhite { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Navy { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush OldLace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Olive { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush OliveDrab { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Orange { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush OrangeRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Orchid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush PaleGoldenrod { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush PaleGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush PaleTurquoise { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush PaleVioletRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush PapayaWhip { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush PeachPuff { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Peru { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Pink { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Plum { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush PowderBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Purple { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Red { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush RosyBrown { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush RoyalBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SaddleBrown { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Salmon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SandyBrown { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SeaGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SeaShell { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Sienna { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Silver { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SkyBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SlateBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SlateGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Snow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SpringGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush SteelBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Tan { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Teal { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Thistle { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Tomato { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Transparent { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Turquoise { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Violet { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Wheat { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush White { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush WhiteSmoke { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Yellow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush YellowGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public sealed partial class BufferedGraphics : System.IDisposable
    {
        internal BufferedGraphics() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Graphics Graphics { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public void Dispose() { }
        public void Render() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Render(System.Drawing.Graphics? target) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Render(System.IntPtr targetDC) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class BufferedGraphicsContext : System.IDisposable
    {
        public BufferedGraphicsContext() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Size MaximumBuffer { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.BufferedGraphics Allocate(System.Drawing.Graphics targetGraphics, System.Drawing.Rectangle targetRectangle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.BufferedGraphics Allocate(System.IntPtr targetDC, System.Drawing.Rectangle targetRectangle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        ~BufferedGraphicsContext() { }
        public void Invalidate() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public static partial class BufferedGraphicsManager
    {
        public static System.Drawing.BufferedGraphicsContext Current { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public partial struct CharacterRange
    {
        private int _dummyPrimitive;
        public CharacterRange(int First, int Length) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int First { readonly get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Length { readonly get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override readonly bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override readonly int GetHashCode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static bool operator ==(System.Drawing.CharacterRange cr1, System.Drawing.CharacterRange cr2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static bool operator !=(System.Drawing.CharacterRange cr1, System.Drawing.CharacterRange cr2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public static partial class ColorTranslator
    {
        public static System.Drawing.Color FromHtml(string htmlColor) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Color FromOle(int oleColor) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Color FromWin32(int win32Color) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static string ToHtml(System.Drawing.Color c) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static int ToOle(System.Drawing.Color c) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static int ToWin32(System.Drawing.Color c) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.ComponentModel.EditorAttribute("System.Drawing.Design.ContentAlignmentEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public enum ContentAlignment
    {
        TopLeft = 1,
        TopCenter = 2,
        TopRight = 4,
        MiddleLeft = 16,
        MiddleCenter = 32,
        MiddleRight = 64,
        BottomLeft = 256,
        BottomCenter = 512,
        BottomRight = 1024,
    }

    public enum CopyPixelOperation
    {
        NoMirrorBitmap = -2147483648,
        Blackness = 66,
        NotSourceErase = 1114278,
        NotSourceCopy = 3342344,
        SourceErase = 4457256,
        DestinationInvert = 5570569,
        PatInvert = 5898313,
        SourceInvert = 6684742,
        SourceAnd = 8913094,
        MergePaint = 12255782,
        MergeCopy = 12583114,
        SourceCopy = 13369376,
        SourcePaint = 15597702,
        PatCopy = 15728673,
        PatPaint = 16452105,
        Whiteness = 16711778,
        CaptureBlt = 1073741824,
    }

    [System.ComponentModel.EditorAttribute("System.Drawing.Design.FontEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.Drawing.FontConverter))]
    public sealed partial class Font : System.MarshalByRefObject, System.ICloneable, System.IDisposable, System.Runtime.Serialization.ISerializable
    {
        public Font(System.Drawing.Font prototype, System.Drawing.FontStyle newStyle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(System.Drawing.FontFamily family, float emSize) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(System.Drawing.FontFamily family, float emSize, System.Drawing.FontStyle style) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(System.Drawing.FontFamily family, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(System.Drawing.FontFamily family, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(System.Drawing.FontFamily family, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(System.Drawing.FontFamily family, float emSize, System.Drawing.GraphicsUnit unit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(string familyName, float emSize) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(string familyName, float emSize, System.Drawing.FontStyle style) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(string familyName, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(string familyName, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(string familyName, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Font(string familyName, float emSize, System.Drawing.GraphicsUnit unit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Bold { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public System.Drawing.FontFamily FontFamily { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public byte GdiCharSet { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool GdiVerticalFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public int Height { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public bool IsSystemFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Italic { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        [System.ComponentModel.EditorAttribute("System.Drawing.Design.FontNameEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [System.ComponentModel.TypeConverterAttribute(typeof(System.Drawing.FontConverter.FontNameConverter))]
        public string Name { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public string? OriginalFontName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Size { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public float SizeInPoints { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Strikeout { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public System.Drawing.FontStyle Style { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public string SystemFontName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Underline { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.TypeConverterAttribute(typeof(System.Drawing.FontConverter.FontUnitConverter))]
        public System.Drawing.GraphicsUnit Unit { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        ~Font() { }
        public static System.Drawing.Font FromHdc(System.IntPtr hdc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Font FromHfont(System.IntPtr hfont) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Font FromLogFont(object lf) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Font FromLogFont(object lf, System.IntPtr hdc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override int GetHashCode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public float GetHeight() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public float GetHeight(System.Drawing.Graphics graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public float GetHeight(float dpi) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.IntPtr ToHfont() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ToLogFont(object logFont) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ToLogFont(object logFont, System.Drawing.Graphics graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class FontConverter : System.ComponentModel.TypeConverter
    {
        public FontConverter() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] System.Type? destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext? context, System.Collections.IDictionary propertyValues) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [Diagnostics.CodeAnalysis.RequiresUnreferencedCode("The Type of value cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
        public override System.ComponentModel.PropertyDescriptorCollection? GetProperties(System.ComponentModel.ITypeDescriptorContext? context, object? value, System.Attribute[]? attributes) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public sealed partial class FontNameConverter : System.ComponentModel.TypeConverter, System.IDisposable
        {
            public FontNameConverter() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            void System.IDisposable.Dispose() { }
        }

        public partial class FontUnitConverter : System.ComponentModel.EnumConverter
        {
            public FontUnitConverter() : base (default(System.Type)) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        }
    }

    public sealed partial class FontFamily : System.MarshalByRefObject, System.IDisposable
    {
        public FontFamily(System.Drawing.Text.GenericFontFamilies genericFamily) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public FontFamily(string name) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public FontFamily(string name, System.Drawing.Text.FontCollection? fontCollection) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.FontFamily[] Families { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.FontFamily GenericMonospace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.FontFamily GenericSansSerif { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.FontFamily GenericSerif { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string Name { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public void Dispose() { }
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        ~FontFamily() { }
        public int GetCellAscent(System.Drawing.FontStyle style) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int GetCellDescent(System.Drawing.FontStyle style) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int GetEmHeight(System.Drawing.FontStyle style) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ObsoleteAttribute("FontFamily.GetFamilies has been deprecated. Use Families instead.")]
        public static System.Drawing.FontFamily[] GetFamilies(System.Drawing.Graphics graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override int GetHashCode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int GetLineSpacing(System.Drawing.FontStyle style) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public string GetName(int language) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsStyleAvailable(System.Drawing.FontStyle style) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.FlagsAttribute]
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8,
    }

    public sealed partial class Graphics : System.MarshalByRefObject, System.Drawing.IDeviceContext, System.IDisposable
    {
        internal Graphics() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Region Clip { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.RectangleF ClipBounds { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.CompositingMode CompositingMode { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.CompositingQuality CompositingQuality { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float DpiX { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float DpiY { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.InterpolationMode InterpolationMode { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool IsClipEmpty { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool IsVisibleClipEmpty { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float PageScale { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.GraphicsUnit PageUnit { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.PixelOffsetMode PixelOffsetMode { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Point RenderingOrigin { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int TextContrast { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Text.TextRenderingHint TextRenderingHint { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.Matrix Transform { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.RectangleF VisibleClipBounds { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public void AddMetafileComment(byte[] data) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.GraphicsContainer BeginContainer() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.GraphicsContainer BeginContainer(System.Drawing.Rectangle dstrect, System.Drawing.Rectangle srcrect, System.Drawing.GraphicsUnit unit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.GraphicsContainer BeginContainer(System.Drawing.RectangleF dstrect, System.Drawing.RectangleF srcrect, System.Drawing.GraphicsUnit unit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Clear(System.Drawing.Color color) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void CopyFromScreen(System.Drawing.Point upperLeftSource, System.Drawing.Point upperLeftDestination, System.Drawing.Size blockRegionSize) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void CopyFromScreen(System.Drawing.Point upperLeftSource, System.Drawing.Point upperLeftDestination, System.Drawing.Size blockRegionSize, System.Drawing.CopyPixelOperation copyPixelOperation) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, System.Drawing.Size blockRegionSize) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, System.Drawing.Size blockRegionSize, System.Drawing.CopyPixelOperation copyPixelOperation) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        public void DrawArc(System.Drawing.Pen pen, System.Drawing.Rectangle rect, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawArc(System.Drawing.Pen pen, System.Drawing.RectangleF rect, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawArc(System.Drawing.Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawArc(System.Drawing.Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawBezier(System.Drawing.Pen pen, System.Drawing.Point pt1, System.Drawing.Point pt2, System.Drawing.Point pt3, System.Drawing.Point pt4) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawBezier(System.Drawing.Pen pen, System.Drawing.PointF pt1, System.Drawing.PointF pt2, System.Drawing.PointF pt3, System.Drawing.PointF pt4) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawBezier(System.Drawing.Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawBeziers(System.Drawing.Pen pen, System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawBeziers(System.Drawing.Pen pen, System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawClosedCurve(System.Drawing.Pen pen, System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawClosedCurve(System.Drawing.Pen pen, System.Drawing.PointF[] points, float tension, System.Drawing.Drawing2D.FillMode fillmode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawClosedCurve(System.Drawing.Pen pen, System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawClosedCurve(System.Drawing.Pen pen, System.Drawing.Point[] points, float tension, System.Drawing.Drawing2D.FillMode fillmode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawCurve(System.Drawing.Pen pen, System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawCurve(System.Drawing.Pen pen, System.Drawing.PointF[] points, int offset, int numberOfSegments) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawCurve(System.Drawing.Pen pen, System.Drawing.PointF[] points, int offset, int numberOfSegments, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawCurve(System.Drawing.Pen pen, System.Drawing.PointF[] points, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawCurve(System.Drawing.Pen pen, System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawCurve(System.Drawing.Pen pen, System.Drawing.Point[] points, int offset, int numberOfSegments, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawCurve(System.Drawing.Pen pen, System.Drawing.Point[] points, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawEllipse(System.Drawing.Pen pen, System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawEllipse(System.Drawing.Pen pen, System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawEllipse(System.Drawing.Pen pen, int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawEllipse(System.Drawing.Pen pen, float x, float y, float width, float height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawIcon(System.Drawing.Icon icon, System.Drawing.Rectangle targetRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawIcon(System.Drawing.Icon icon, int x, int y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawIconUnstretched(System.Drawing.Icon icon, System.Drawing.Rectangle targetRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Point point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.PointF point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.PointF[] destPoints) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttr, System.Drawing.Graphics.DrawImageAbort? callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttr, System.Drawing.Graphics.DrawImageAbort? callback, int callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Point[] destPoints) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Point[] destPoints, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Point[] destPoints, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Point[] destPoints, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttr, System.Drawing.Graphics.DrawImageAbort? callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Point[] destPoints, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttr, System.Drawing.Graphics.DrawImageAbort? callback, int callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, System.Drawing.GraphicsUnit srcUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttr, System.Drawing.Graphics.DrawImageAbort? callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttrs, System.Drawing.Graphics.DrawImageAbort? callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, System.Drawing.GraphicsUnit srcUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttrs) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttrs, System.Drawing.Graphics.DrawImageAbort? callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Imaging.ImageAttributes? imageAttrs, System.Drawing.Graphics.DrawImageAbort? callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, System.Drawing.RectangleF destRect, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, int x, int y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, int x, int y, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, float x, float y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, float x, float y, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImage(System.Drawing.Image image, float x, float y, float width, float height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImageUnscaled(System.Drawing.Image image, System.Drawing.Point point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImageUnscaled(System.Drawing.Image image, System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImageUnscaled(System.Drawing.Image image, int x, int y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImageUnscaled(System.Drawing.Image image, int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawImageUnscaledAndClipped(System.Drawing.Image image, System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawLine(System.Drawing.Pen pen, System.Drawing.Point pt1, System.Drawing.Point pt2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawLine(System.Drawing.Pen pen, System.Drawing.PointF pt1, System.Drawing.PointF pt2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawLine(System.Drawing.Pen pen, int x1, int y1, int x2, int y2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawLine(System.Drawing.Pen pen, float x1, float y1, float x2, float y2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawLines(System.Drawing.Pen pen, System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawLines(System.Drawing.Pen pen, System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawPath(System.Drawing.Pen pen, System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawPie(System.Drawing.Pen pen, System.Drawing.Rectangle rect, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawPie(System.Drawing.Pen pen, System.Drawing.RectangleF rect, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawPie(System.Drawing.Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawPie(System.Drawing.Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawPolygon(System.Drawing.Pen pen, System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawPolygon(System.Drawing.Pen pen, System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawRectangle(System.Drawing.Pen pen, System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawRectangle(System.Drawing.Pen pen, int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawRectangle(System.Drawing.Pen pen, float x, float y, float width, float height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawRectangles(System.Drawing.Pen pen, System.Drawing.RectangleF[] rects) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawRectangles(System.Drawing.Pen pen, System.Drawing.Rectangle[] rects) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawString(string? s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.PointF point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawString(string? s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.PointF point, System.Drawing.StringFormat? format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawString(string? s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.RectangleF layoutRectangle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawString(string? s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.RectangleF layoutRectangle, System.Drawing.StringFormat? format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawString(string? s, System.Drawing.Font font, System.Drawing.Brush brush, float x, float y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void DrawString(string? s, System.Drawing.Font font, System.Drawing.Brush brush, float x, float y, System.Drawing.StringFormat? format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EndContainer(System.Drawing.Drawing2D.GraphicsContainer container) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point destPoint, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point destPoint, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point destPoint, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point destPoint, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point destPoint, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point destPoint, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit unit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF destPoint, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF destPoint, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF destPoint, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF destPoint, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF destPoint, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF destPoint, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit unit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF[] destPoints, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF[] destPoints, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF[] destPoints, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit unit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point[] destPoints, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point[] destPoints, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point[] destPoints, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point[] destPoints, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point[] destPoints, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Point[] destPoints, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit unit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Rectangle destRect, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Rectangle destRect, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Rectangle destRect, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Rectangle destRect, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Rectangle destRect, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.Rectangle destRect, System.Drawing.Rectangle srcRect, System.Drawing.GraphicsUnit unit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.RectangleF destRect, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.RectangleF destRect, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.RectangleF destRect, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.RectangleF destRect, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.RectangleF destRect, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit srcUnit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void EnumerateMetafile(System.Drawing.Imaging.Metafile metafile, System.Drawing.RectangleF destRect, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit unit, System.Drawing.Graphics.EnumerateMetafileProc callback, System.IntPtr callbackData, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ExcludeClip(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ExcludeClip(System.Drawing.Region region) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillClosedCurve(System.Drawing.Brush brush, System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillClosedCurve(System.Drawing.Brush brush, System.Drawing.PointF[] points, System.Drawing.Drawing2D.FillMode fillmode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillClosedCurve(System.Drawing.Brush brush, System.Drawing.PointF[] points, System.Drawing.Drawing2D.FillMode fillmode, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillClosedCurve(System.Drawing.Brush brush, System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillClosedCurve(System.Drawing.Brush brush, System.Drawing.Point[] points, System.Drawing.Drawing2D.FillMode fillmode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillClosedCurve(System.Drawing.Brush brush, System.Drawing.Point[] points, System.Drawing.Drawing2D.FillMode fillmode, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillEllipse(System.Drawing.Brush brush, System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillEllipse(System.Drawing.Brush brush, System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillEllipse(System.Drawing.Brush brush, int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillEllipse(System.Drawing.Brush brush, float x, float y, float width, float height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillPath(System.Drawing.Brush brush, System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillPie(System.Drawing.Brush brush, System.Drawing.Rectangle rect, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillPie(System.Drawing.Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillPie(System.Drawing.Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillPolygon(System.Drawing.Brush brush, System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillPolygon(System.Drawing.Brush brush, System.Drawing.PointF[] points, System.Drawing.Drawing2D.FillMode fillMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillPolygon(System.Drawing.Brush brush, System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillPolygon(System.Drawing.Brush brush, System.Drawing.Point[] points, System.Drawing.Drawing2D.FillMode fillMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillRectangle(System.Drawing.Brush brush, System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillRectangle(System.Drawing.Brush brush, System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillRectangle(System.Drawing.Brush brush, int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillRectangle(System.Drawing.Brush brush, float x, float y, float width, float height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillRectangles(System.Drawing.Brush brush, System.Drawing.RectangleF[] rects) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillRectangles(System.Drawing.Brush brush, System.Drawing.Rectangle[] rects) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void FillRegion(System.Drawing.Brush brush, System.Drawing.Region region) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        ~Graphics() { }
        public void Flush() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Flush(System.Drawing.Drawing2D.FlushIntention intention) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Drawing.Graphics FromHdc(System.IntPtr hdc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Drawing.Graphics FromHdc(System.IntPtr hdc, System.IntPtr hdevice) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Drawing.Graphics FromHdcInternal(System.IntPtr hdc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Drawing.Graphics FromHwnd(System.IntPtr hwnd) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Drawing.Graphics FromHwndInternal(System.IntPtr hwnd) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Graphics FromImage(System.Drawing.Image image) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.Runtime.Versioning.SupportedOSPlatformAttribute("windows")]
        public object GetContextInfo() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported); }
        public static System.IntPtr GetHalftonePalette() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.IntPtr GetHdc() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Color GetNearestColor(System.Drawing.Color color) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void IntersectClip(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void IntersectClip(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void IntersectClip(System.Drawing.Region region) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.Point point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.PointF point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(int x, int y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(float x, float y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(float x, float y, float width, float height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Region[] MeasureCharacterRanges(string? text, System.Drawing.Font font, System.Drawing.RectangleF layoutRect, System.Drawing.StringFormat? stringFormat) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.SizeF MeasureString(string? text, System.Drawing.Font font) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.SizeF MeasureString(string? text, System.Drawing.Font font, System.Drawing.PointF origin, System.Drawing.StringFormat? stringFormat) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.SizeF MeasureString(string? text, System.Drawing.Font font, System.Drawing.SizeF layoutArea) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.SizeF MeasureString(string? text, System.Drawing.Font font, System.Drawing.SizeF layoutArea, System.Drawing.StringFormat? stringFormat) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.SizeF MeasureString(string? text, System.Drawing.Font font, System.Drawing.SizeF layoutArea, System.Drawing.StringFormat? stringFormat, out int charactersFitted, out int linesFilled) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.SizeF MeasureString(string? text, System.Drawing.Font font, int width) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.SizeF MeasureString(string? text, System.Drawing.Font font, int width, System.Drawing.StringFormat? format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ReleaseHdc() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void ReleaseHdc(System.IntPtr hdc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public void ReleaseHdcInternal(System.IntPtr hdc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ResetClip() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ResetTransform() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Restore(System.Drawing.Drawing2D.GraphicsState gstate) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.GraphicsState Save() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Drawing2D.CombineMode combineMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.Graphics g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.Graphics g, System.Drawing.Drawing2D.CombineMode combineMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.Rectangle rect, System.Drawing.Drawing2D.CombineMode combineMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.RectangleF rect, System.Drawing.Drawing2D.CombineMode combineMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetClip(System.Drawing.Region region, System.Drawing.Drawing2D.CombineMode combineMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TransformPoints(System.Drawing.Drawing2D.CoordinateSpace destSpace, System.Drawing.Drawing2D.CoordinateSpace srcSpace, System.Drawing.PointF[] pts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TransformPoints(System.Drawing.Drawing2D.CoordinateSpace destSpace, System.Drawing.Drawing2D.CoordinateSpace srcSpace, System.Drawing.Point[] pts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateClip(int dx, int dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateClip(float dx, float dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public delegate bool DrawImageAbort(System.IntPtr callbackdata);
        public delegate bool EnumerateMetafileProc(System.Drawing.Imaging.EmfPlusRecordType recordType, int flags, int dataSize, System.IntPtr data, System.Drawing.Imaging.PlayRecordCallback? callbackData);
    }

    public enum GraphicsUnit
    {
        World = 0,
        Display = 1,
        Pixel = 2,
        Point = 3,
        Inch = 4,
        Document = 5,
        Millimeter = 6,
    }

    [System.ComponentModel.EditorAttribute("System.Drawing.Design.IconEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.Drawing.IconConverter))]
    public sealed partial class Icon : System.MarshalByRefObject, System.ICloneable, System.IDisposable, System.Runtime.Serialization.ISerializable
    {
        public Icon(System.Drawing.Icon original, System.Drawing.Size size) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Icon(System.Drawing.Icon original, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Icon(System.IO.Stream stream) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Icon(System.IO.Stream stream, System.Drawing.Size size) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Icon(System.IO.Stream stream, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Icon(string fileName) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Icon(string fileName, System.Drawing.Size size) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Icon(string fileName, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Icon(System.Type type, string resource) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.BrowsableAttribute(false)]
        public System.IntPtr Handle { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public int Height { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Size Size { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public int Width { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        public static System.Drawing.Icon? ExtractAssociatedIcon(string filePath) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        ~Icon() { }
        public static System.Drawing.Icon FromHandle(System.IntPtr handle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Save(System.IO.Stream outputStream) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Bitmap ToBitmap() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class IconConverter : System.ComponentModel.ExpandableObjectConverter
    {
        public IconConverter() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] System.Type? destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial interface IDeviceContext : System.IDisposable
    {
        System.IntPtr GetHdc();
        void ReleaseHdc();
    }

    [System.ComponentModel.EditorAttribute("System.Drawing.Design.ImageEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [System.ComponentModel.ImmutableObjectAttribute(true)]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.Drawing.ImageConverter))]
    public abstract partial class Image : System.MarshalByRefObject, System.ICloneable, System.IDisposable, System.Runtime.Serialization.ISerializable
    {
        internal Image() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.BrowsableAttribute(false)]
        public int Flags { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public System.Guid[] FrameDimensionsList { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        [System.ComponentModel.DefaultValueAttribute(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int Height { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float HorizontalResolution { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public System.Drawing.Imaging.ColorPalette Palette { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.SizeF PhysicalDimension { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Imaging.PixelFormat PixelFormat { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public int[] PropertyIdList { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        public System.Drawing.Imaging.PropertyItem[] PropertyItems { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Imaging.ImageFormat RawFormat { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Size Size { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        [System.ComponentModel.LocalizableAttribute(false)]
        public object? Tag { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float VerticalResolution { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        [System.ComponentModel.DefaultValueAttribute(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int Width { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        ~Image() { }
        public static System.Drawing.Image FromFile(string filename) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Image FromFile(string filename, bool useEmbeddedColorManagement) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Bitmap FromHbitmap(System.IntPtr hbitmap) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Bitmap FromHbitmap(System.IntPtr hbitmap, System.IntPtr hpalette) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Image FromStream(System.IO.Stream stream) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Image FromStream(System.IO.Stream stream, bool useEmbeddedColorManagement) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Image FromStream(System.IO.Stream stream, bool useEmbeddedColorManagement, bool validateImageData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.RectangleF GetBounds(ref System.Drawing.GraphicsUnit pageUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Imaging.EncoderParameters? GetEncoderParameterList(System.Guid encoder) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int GetFrameCount(System.Drawing.Imaging.FrameDimension dimension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static int GetPixelFormatSize(System.Drawing.Imaging.PixelFormat pixfmt) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Imaging.PropertyItem? GetPropertyItem(int propid) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Image GetThumbnailImage(int thumbWidth, int thumbHeight, System.Drawing.Image.GetThumbnailImageAbort? callback, System.IntPtr callbackData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static bool IsAlphaPixelFormat(System.Drawing.Imaging.PixelFormat pixfmt) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static bool IsCanonicalPixelFormat(System.Drawing.Imaging.PixelFormat pixfmt) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static bool IsExtendedPixelFormat(System.Drawing.Imaging.PixelFormat pixfmt) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RemovePropertyItem(int propid) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateFlip(System.Drawing.RotateFlipType rotateFlipType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Save(System.IO.Stream stream, System.Drawing.Imaging.ImageCodecInfo encoder, System.Drawing.Imaging.EncoderParameters? encoderParams) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Save(System.IO.Stream stream, System.Drawing.Imaging.ImageFormat format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Save(string filename) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Save(string filename, System.Drawing.Imaging.ImageCodecInfo encoder, System.Drawing.Imaging.EncoderParameters? encoderParams) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Save(string filename, System.Drawing.Imaging.ImageFormat format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SaveAdd(System.Drawing.Image image, System.Drawing.Imaging.EncoderParameters? encoderParams) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SaveAdd(System.Drawing.Imaging.EncoderParameters? encoderParams) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int SelectActiveFrame(System.Drawing.Imaging.FrameDimension dimension, int frameIndex) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetPropertyItem(System.Drawing.Imaging.PropertyItem propitem) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public delegate bool GetThumbnailImageAbort();
    }

    public sealed partial class ImageAnimator
    {
        internal ImageAnimator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static void Animate(System.Drawing.Image image, System.EventHandler onFrameChangedHandler) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static bool CanAnimate([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] System.Drawing.Image? image) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static void StopAnimate(System.Drawing.Image image, System.EventHandler onFrameChangedHandler) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static void UpdateFrames() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static void UpdateFrames(System.Drawing.Image? image) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class ImageConverter : System.ComponentModel.TypeConverter
    {
        public ImageConverter() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] System.Type? destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [Diagnostics.CodeAnalysis.RequiresUnreferencedCode("The Type of value cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
        public override System.ComponentModel.PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext? context, object? value, System.Attribute[]? attributes) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class ImageFormatConverter : System.ComponentModel.TypeConverter
    {
        public ImageFormatConverter() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type? sourceType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] System.Type? destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class Pen : System.MarshalByRefObject, System.ICloneable, System.IDisposable
    {
        public Pen(System.Drawing.Brush brush) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Pen(System.Drawing.Brush brush, float width) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Pen(System.Drawing.Color color) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Pen(System.Drawing.Color color, float width) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.PenAlignment Alignment { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Brush Brush { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Color Color { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float[] CompoundArray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.CustomLineCap CustomEndCap { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.CustomLineCap CustomStartCap { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.DashCap DashCap { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float DashOffset { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float[] DashPattern { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.DashStyle DashStyle { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.LineCap EndCap { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.LineJoin LineJoin { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float MiterLimit { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.PenType PenType { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.LineCap StartCap { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.Matrix Transform { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Width { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        ~Pen() { }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ResetTransform() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetLineCap(System.Drawing.Drawing2D.LineCap startCap, System.Drawing.Drawing2D.LineCap endCap, System.Drawing.Drawing2D.DashCap dashCap) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public static partial class Pens
    {
        public static System.Drawing.Pen AliceBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen AntiqueWhite { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Aqua { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Aquamarine { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Azure { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Beige { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Bisque { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Black { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen BlanchedAlmond { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Blue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen BlueViolet { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Brown { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen BurlyWood { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen CadetBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Chartreuse { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Chocolate { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Coral { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen CornflowerBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Cornsilk { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Crimson { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Cyan { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkCyan { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkGoldenrod { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkKhaki { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkMagenta { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkOliveGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkOrange { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkOrchid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkSalmon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkSeaGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkSlateBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkSlateGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkTurquoise { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DarkViolet { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DeepPink { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DeepSkyBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DimGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen DodgerBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Firebrick { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen FloralWhite { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ForestGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Fuchsia { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Gainsboro { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen GhostWhite { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Gold { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Goldenrod { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Gray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Green { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen GreenYellow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Honeydew { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen HotPink { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen IndianRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Indigo { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Ivory { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Khaki { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Lavender { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LavenderBlush { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LawnGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LemonChiffon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightCoral { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightCyan { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightGoldenrodYellow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightPink { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightSalmon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightSeaGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightSkyBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightSlateGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightSteelBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LightYellow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Lime { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen LimeGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Linen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Magenta { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Maroon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumAquamarine { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumOrchid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumPurple { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumSeaGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumSlateBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumSpringGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumTurquoise { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MediumVioletRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MidnightBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MintCream { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MistyRose { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Moccasin { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen NavajoWhite { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Navy { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen OldLace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Olive { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen OliveDrab { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Orange { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen OrangeRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Orchid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen PaleGoldenrod { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen PaleGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen PaleTurquoise { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen PaleVioletRed { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen PapayaWhip { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen PeachPuff { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Peru { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Pink { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Plum { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen PowderBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Purple { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Red { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen RosyBrown { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen RoyalBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SaddleBrown { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Salmon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SandyBrown { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SeaGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SeaShell { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Sienna { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Silver { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SkyBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SlateBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SlateGray { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Snow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SpringGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen SteelBlue { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Tan { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Teal { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Thistle { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Tomato { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Transparent { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Turquoise { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Violet { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Wheat { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen White { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen WhiteSmoke { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Yellow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen YellowGreen { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public sealed partial class Region : System.MarshalByRefObject, System.IDisposable
    {
        public Region() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Region(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Region(System.Drawing.Drawing2D.RegionData rgnData) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Region(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Region(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Region Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Complement(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Complement(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Complement(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Complement(System.Drawing.Region region) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        public bool Equals(System.Drawing.Region region, System.Drawing.Graphics g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Exclude(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Exclude(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Exclude(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Exclude(System.Drawing.Region region) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        ~Region() { }
        public static System.Drawing.Region FromHrgn(System.IntPtr hrgn) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.RectangleF GetBounds(System.Drawing.Graphics g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.IntPtr GetHrgn(System.Drawing.Graphics g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.RegionData? GetRegionData() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.RectangleF[] GetRegionScans(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Intersect(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Intersect(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Intersect(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Intersect(System.Drawing.Region region) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsEmpty(System.Drawing.Graphics g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsInfinite(System.Drawing.Graphics g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.Point point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.Point point, System.Drawing.Graphics? g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.PointF point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.PointF point, System.Drawing.Graphics? g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.Rectangle rect, System.Drawing.Graphics? g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.RectangleF rect, System.Drawing.Graphics? g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(int x, int y, System.Drawing.Graphics? g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(int x, int y, int width, int height, System.Drawing.Graphics? g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(float x, float y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(float x, float y, System.Drawing.Graphics? g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(float x, float y, float width, float height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(float x, float y, float width, float height, System.Drawing.Graphics? g) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MakeEmpty() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MakeInfinite() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ReleaseHrgn(System.IntPtr regionHandle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Transform(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Translate(int dx, int dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Translate(float dx, float dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Union(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Union(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Union(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Union(System.Drawing.Region region) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Xor(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Xor(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Xor(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Xor(System.Drawing.Region region) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum RotateFlipType
    {
        Rotate180FlipXY = 0,
        RotateNoneFlipNone = 0,
        Rotate270FlipXY = 1,
        Rotate90FlipNone = 1,
        Rotate180FlipNone = 2,
        RotateNoneFlipXY = 2,
        Rotate270FlipNone = 3,
        Rotate90FlipXY = 3,
        Rotate180FlipY = 4,
        RotateNoneFlipX = 4,
        Rotate270FlipY = 5,
        Rotate90FlipX = 5,
        Rotate180FlipX = 6,
        RotateNoneFlipY = 6,
        Rotate270FlipX = 7,
        Rotate90FlipY = 7,
    }

    public sealed partial class SolidBrush : System.Drawing.Brush
    {
        public SolidBrush(System.Drawing.Color color) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Color Color { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        protected override void Dispose(bool disposing) { }
    }

    public enum StringAlignment
    {
        Near = 0,
        Center = 1,
        Far = 2,
    }

    public enum StringDigitSubstitute
    {
        User = 0,
        None = 1,
        National = 2,
        Traditional = 3,
    }

    public sealed partial class StringFormat : System.MarshalByRefObject, System.ICloneable, System.IDisposable
    {
        public StringFormat() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public StringFormat(System.Drawing.StringFormat format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public StringFormat(System.Drawing.StringFormatFlags options) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public StringFormat(System.Drawing.StringFormatFlags options, int language) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.StringAlignment Alignment { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int DigitSubstitutionLanguage { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.StringDigitSubstitute DigitSubstitutionMethod { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.StringFormatFlags FormatFlags { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.StringFormat GenericDefault { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.StringFormat GenericTypographic { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Text.HotkeyPrefix HotkeyPrefix { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.StringAlignment LineAlignment { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.StringTrimming Trimming { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        ~StringFormat() { }
        public float[] GetTabStops(out float firstTabOffset) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetDigitSubstitution(int language, System.Drawing.StringDigitSubstitute substitute) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetMeasurableCharacterRanges(System.Drawing.CharacterRange[] ranges) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetTabStops(float firstTabOffset, float[] tabStops) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.FlagsAttribute]
    public enum StringFormatFlags
    {
        DirectionRightToLeft = 1,
        DirectionVertical = 2,
        FitBlackBox = 4,
        DisplayFormatControl = 32,
        NoFontFallback = 1024,
        MeasureTrailingSpaces = 2048,
        NoWrap = 4096,
        LineLimit = 8192,
        NoClip = 16384,
    }

    public enum StringTrimming
    {
        None = 0,
        Character = 1,
        Word = 2,
        EllipsisCharacter = 3,
        EllipsisWord = 4,
        EllipsisPath = 5,
    }

    public enum StringUnit
    {
        World = 0,
        Display = 1,
        Pixel = 2,
        Point = 3,
        Inch = 4,
        Document = 5,
        Millimeter = 6,
        Em = 32,
    }

    public static partial class SystemBrushes
    {
        public static System.Drawing.Brush ActiveBorder { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ActiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ActiveCaptionText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush AppWorkspace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ButtonFace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ButtonHighlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ButtonShadow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Control { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ControlDark { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ControlDarkDark { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ControlLight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ControlLightLight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ControlText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Desktop { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush GradientActiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush GradientInactiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush GrayText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Highlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush HighlightText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush HotTrack { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush InactiveBorder { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush InactiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush InactiveCaptionText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Info { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush InfoText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Menu { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MenuBar { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MenuHighlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush MenuText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush ScrollBar { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush Window { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush WindowFrame { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush WindowText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Brush FromSystemColor(System.Drawing.Color c) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public static partial class SystemColors
    {
        public static System.Drawing.Color ActiveBorder { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ActiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ActiveCaptionText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color AppWorkspace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ButtonFace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ButtonHighlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ButtonShadow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color Control { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ControlDark { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ControlDarkDark { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ControlLight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ControlLightLight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ControlText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color Desktop { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color GradientActiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color GradientInactiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color GrayText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color Highlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color HighlightText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color HotTrack { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color InactiveBorder { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color InactiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color InactiveCaptionText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color Info { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color InfoText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color Menu { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color MenuBar { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color MenuHighlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color MenuText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color ScrollBar { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color Window { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color WindowFrame { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Color WindowText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public static partial class SystemFonts
    {
        public static System.Drawing.Font? CaptionFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Font DefaultFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Font DialogFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Font? IconTitleFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Font? MenuFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Font? MessageBoxFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Font? SmallCaptionFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Font? StatusFont { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Font? GetFontByName(string systemFontName) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public static partial class SystemIcons
    {
        public static System.Drawing.Icon Application { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon Asterisk { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon Error { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon Exclamation { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon Hand { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon Information { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon Question { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon Shield { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon Warning { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Icon WinLogo { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public static partial class SystemPens
    {
        public static System.Drawing.Pen ActiveBorder { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ActiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ActiveCaptionText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen AppWorkspace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ButtonFace { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ButtonHighlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ButtonShadow { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Control { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ControlDark { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ControlDarkDark { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ControlLight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ControlLightLight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ControlText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Desktop { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen GradientActiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen GradientInactiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen GrayText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Highlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen HighlightText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen HotTrack { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen InactiveBorder { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen InactiveCaption { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen InactiveCaptionText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Info { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen InfoText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Menu { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MenuBar { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MenuHighlight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen MenuText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen ScrollBar { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen Window { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen WindowFrame { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen WindowText { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Pen FromSystemColor(System.Drawing.Color c) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class TextureBrush : System.Drawing.Brush
    {
        public TextureBrush(System.Drawing.Image bitmap) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public TextureBrush(System.Drawing.Image image, System.Drawing.Drawing2D.WrapMode wrapMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public TextureBrush(System.Drawing.Image image, System.Drawing.Drawing2D.WrapMode wrapMode, System.Drawing.Rectangle dstRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public TextureBrush(System.Drawing.Image image, System.Drawing.Drawing2D.WrapMode wrapMode, System.Drawing.RectangleF dstRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public TextureBrush(System.Drawing.Image image, System.Drawing.Rectangle dstRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public TextureBrush(System.Drawing.Image image, System.Drawing.Rectangle dstRect, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public TextureBrush(System.Drawing.Image image, System.Drawing.RectangleF dstRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public TextureBrush(System.Drawing.Image image, System.Drawing.RectangleF dstRect, System.Drawing.Imaging.ImageAttributes? imageAttr) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Image Image { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.Matrix Transform { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.WrapMode WrapMode { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ResetTransform() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.AttributeUsageAttribute(System.AttributeTargets.Class)]
    public partial class ToolboxBitmapAttribute : System.Attribute
    {
        public static readonly System.Drawing.ToolboxBitmapAttribute Default;
        public ToolboxBitmapAttribute(string imageFile) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public ToolboxBitmapAttribute(System.Type t) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public ToolboxBitmapAttribute(System.Type t, string name) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override int GetHashCode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Image? GetImage(object? component) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Image? GetImage(object? component, bool large) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Image? GetImage(System.Type type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Image? GetImage(System.Type type, bool large) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Image? GetImage(System.Type type, string? imgName, bool large) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Image? GetImageFromResource(System.Type t, string? imageName, bool large) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }
}

namespace System.Drawing.Design
{
    public sealed partial class CategoryNameCollection : System.Collections.ReadOnlyCollectionBase
    {
        public CategoryNameCollection(System.Drawing.Design.CategoryNameCollection value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public CategoryNameCollection(string[] value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public string this[int index] { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool Contains(string value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void CopyTo(string[] array, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int IndexOf(string value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }
}

namespace System.Drawing.Drawing2D
{
    public sealed partial class AdjustableArrowCap : System.Drawing.Drawing2D.CustomLineCap
    {
        public AdjustableArrowCap(float width, float height) : base (default(System.Drawing.Drawing2D.GraphicsPath), default(System.Drawing.Drawing2D.GraphicsPath)) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public AdjustableArrowCap(float width, float height, bool isFilled) : base (default(System.Drawing.Drawing2D.GraphicsPath), default(System.Drawing.Drawing2D.GraphicsPath)) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool Filled { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Height { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float MiddleInset { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Width { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public sealed partial class Blend
    {
        public Blend() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Blend(int count) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public float[] Factors { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float[] Positions { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public sealed partial class ColorBlend
    {
        public ColorBlend() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public ColorBlend(int count) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Color[] Colors { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float[] Positions { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public enum CombineMode
    {
        Replace = 0,
        Intersect = 1,
        Union = 2,
        Xor = 3,
        Exclude = 4,
        Complement = 5,
    }

    public enum CompositingMode
    {
        SourceOver = 0,
        SourceCopy = 1,
    }

    public enum CompositingQuality
    {
        Invalid = -1,
        Default = 0,
        HighSpeed = 1,
        HighQuality = 2,
        GammaCorrected = 3,
        AssumeLinear = 4,
    }

    public enum CoordinateSpace
    {
        World = 0,
        Page = 1,
        Device = 2,
    }

    public partial class CustomLineCap : System.MarshalByRefObject, System.ICloneable, System.IDisposable
    {
        public CustomLineCap(System.Drawing.Drawing2D.GraphicsPath? fillPath, System.Drawing.Drawing2D.GraphicsPath? strokePath) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public CustomLineCap(System.Drawing.Drawing2D.GraphicsPath? fillPath, System.Drawing.Drawing2D.GraphicsPath? strokePath, System.Drawing.Drawing2D.LineCap baseCap) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public CustomLineCap(System.Drawing.Drawing2D.GraphicsPath? fillPath, System.Drawing.Drawing2D.GraphicsPath? strokePath, System.Drawing.Drawing2D.LineCap baseCap, float baseInset) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.LineCap BaseCap { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float BaseInset { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.LineJoin StrokeJoin { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float WidthScale { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        ~CustomLineCap() { }
        public void GetStrokeCaps(out System.Drawing.Drawing2D.LineCap startCap, out System.Drawing.Drawing2D.LineCap endCap) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetStrokeCaps(System.Drawing.Drawing2D.LineCap startCap, System.Drawing.Drawing2D.LineCap endCap) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum DashCap
    {
        Flat = 0,
        Round = 2,
        Triangle = 3,
    }

    public enum DashStyle
    {
        Solid = 0,
        Dash = 1,
        Dot = 2,
        DashDot = 3,
        DashDotDot = 4,
        Custom = 5,
    }

    public enum FillMode
    {
        Alternate = 0,
        Winding = 1,
    }

    public enum FlushIntention
    {
        Flush = 0,
        Sync = 1,
    }

    public sealed partial class GraphicsContainer : System.MarshalByRefObject
    {
        internal GraphicsContainer() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class GraphicsPath : System.MarshalByRefObject, System.ICloneable, System.IDisposable
    {
        public GraphicsPath() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public GraphicsPath(System.Drawing.Drawing2D.FillMode fillMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public GraphicsPath(System.Drawing.PointF[] pts, byte[] types) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public GraphicsPath(System.Drawing.PointF[] pts, byte[] types, System.Drawing.Drawing2D.FillMode fillMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public GraphicsPath(System.Drawing.Point[] pts, byte[] types) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public GraphicsPath(System.Drawing.Point[] pts, byte[] types, System.Drawing.Drawing2D.FillMode fillMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.FillMode FillMode { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.PathData PathData { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.PointF[] PathPoints { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public byte[] PathTypes { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int PointCount { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public void AddArc(System.Drawing.Rectangle rect, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddArc(System.Drawing.RectangleF rect, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddBezier(System.Drawing.Point pt1, System.Drawing.Point pt2, System.Drawing.Point pt3, System.Drawing.Point pt4) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddBezier(System.Drawing.PointF pt1, System.Drawing.PointF pt2, System.Drawing.PointF pt3, System.Drawing.PointF pt4) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddBeziers(System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddBeziers(params System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddClosedCurve(System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddClosedCurve(System.Drawing.PointF[] points, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddClosedCurve(System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddClosedCurve(System.Drawing.Point[] points, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddCurve(System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddCurve(System.Drawing.PointF[] points, int offset, int numberOfSegments, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddCurve(System.Drawing.PointF[] points, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddCurve(System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddCurve(System.Drawing.Point[] points, int offset, int numberOfSegments, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddCurve(System.Drawing.Point[] points, float tension) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddEllipse(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddEllipse(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddEllipse(int x, int y, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddEllipse(float x, float y, float width, float height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddLine(System.Drawing.Point pt1, System.Drawing.Point pt2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddLine(System.Drawing.PointF pt1, System.Drawing.PointF pt2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddLine(int x1, int y1, int x2, int y2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddLine(float x1, float y1, float x2, float y2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddLines(System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddLines(System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddPath(System.Drawing.Drawing2D.GraphicsPath addingPath, bool connect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddPie(System.Drawing.Rectangle rect, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddPolygon(System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddPolygon(System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddRectangle(System.Drawing.Rectangle rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddRectangle(System.Drawing.RectangleF rect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddRectangles(System.Drawing.RectangleF[] rects) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddRectangles(System.Drawing.Rectangle[] rects) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddString(string s, System.Drawing.FontFamily family, int style, float emSize, System.Drawing.Point origin, System.Drawing.StringFormat? format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddString(string s, System.Drawing.FontFamily family, int style, float emSize, System.Drawing.PointF origin, System.Drawing.StringFormat? format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddString(string s, System.Drawing.FontFamily family, int style, float emSize, System.Drawing.Rectangle layoutRect, System.Drawing.StringFormat? format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddString(string s, System.Drawing.FontFamily family, int style, float emSize, System.Drawing.RectangleF layoutRect, System.Drawing.StringFormat? format) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearMarkers() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void CloseAllFigures() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void CloseFigure() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        ~GraphicsPath() { }
        public void Flatten() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Flatten(System.Drawing.Drawing2D.Matrix? matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Flatten(System.Drawing.Drawing2D.Matrix? matrix, float flatness) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.RectangleF GetBounds() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.RectangleF GetBounds(System.Drawing.Drawing2D.Matrix? matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.RectangleF GetBounds(System.Drawing.Drawing2D.Matrix? matrix, System.Drawing.Pen? pen) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.PointF GetLastPoint() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsOutlineVisible(System.Drawing.Point point, System.Drawing.Pen pen) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsOutlineVisible(System.Drawing.Point pt, System.Drawing.Pen pen, System.Drawing.Graphics? graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsOutlineVisible(System.Drawing.PointF point, System.Drawing.Pen pen) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsOutlineVisible(System.Drawing.PointF pt, System.Drawing.Pen pen, System.Drawing.Graphics? graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsOutlineVisible(int x, int y, System.Drawing.Pen pen) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsOutlineVisible(int x, int y, System.Drawing.Pen pen, System.Drawing.Graphics? graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsOutlineVisible(float x, float y, System.Drawing.Pen pen) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsOutlineVisible(float x, float y, System.Drawing.Pen pen, System.Drawing.Graphics? graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.Point point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.Point pt, System.Drawing.Graphics? graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.PointF point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(System.Drawing.PointF pt, System.Drawing.Graphics? graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(int x, int y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(int x, int y, System.Drawing.Graphics? graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(float x, float y) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsVisible(float x, float y, System.Drawing.Graphics? graphics) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Reset() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Reverse() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetMarkers() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void StartFigure() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Transform(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Warp(System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Warp(System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.Drawing2D.Matrix? matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Warp(System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.Drawing2D.Matrix? matrix, System.Drawing.Drawing2D.WarpMode warpMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Warp(System.Drawing.PointF[] destPoints, System.Drawing.RectangleF srcRect, System.Drawing.Drawing2D.Matrix? matrix, System.Drawing.Drawing2D.WarpMode warpMode, float flatness) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Widen(System.Drawing.Pen pen) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Widen(System.Drawing.Pen pen, System.Drawing.Drawing2D.Matrix? matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Widen(System.Drawing.Pen pen, System.Drawing.Drawing2D.Matrix? matrix, float flatness) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class GraphicsPathIterator : System.MarshalByRefObject, System.IDisposable
    {
        public GraphicsPathIterator(System.Drawing.Drawing2D.GraphicsPath? path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int SubpathCount { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int CopyData(ref System.Drawing.PointF[] points, ref byte[] types, int startIndex, int endIndex) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        public int Enumerate(ref System.Drawing.PointF[] points, ref byte[] types) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        ~GraphicsPathIterator() { }
        public bool HasCurve() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int NextMarker(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int NextMarker(out int startIndex, out int endIndex) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int NextPathType(out byte pathType, out int startIndex, out int endIndex) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int NextSubpath(System.Drawing.Drawing2D.GraphicsPath path, out bool isClosed) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Rewind() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class GraphicsState : System.MarshalByRefObject
    {
        internal GraphicsState() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class HatchBrush : System.Drawing.Brush
    {
        public HatchBrush(System.Drawing.Drawing2D.HatchStyle hatchstyle, System.Drawing.Color foreColor) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public HatchBrush(System.Drawing.Drawing2D.HatchStyle hatchstyle, System.Drawing.Color foreColor, System.Drawing.Color backColor) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Color BackgroundColor { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Color ForegroundColor { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.HatchStyle HatchStyle { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum HatchStyle
    {
        Horizontal = 0,
        Min = 0,
        Vertical = 1,
        ForwardDiagonal = 2,
        BackwardDiagonal = 3,
        Cross = 4,
        LargeGrid = 4,
        Max = 4,
        DiagonalCross = 5,
        Percent05 = 6,
        Percent10 = 7,
        Percent20 = 8,
        Percent25 = 9,
        Percent30 = 10,
        Percent40 = 11,
        Percent50 = 12,
        Percent60 = 13,
        Percent70 = 14,
        Percent75 = 15,
        Percent80 = 16,
        Percent90 = 17,
        LightDownwardDiagonal = 18,
        LightUpwardDiagonal = 19,
        DarkDownwardDiagonal = 20,
        DarkUpwardDiagonal = 21,
        WideDownwardDiagonal = 22,
        WideUpwardDiagonal = 23,
        LightVertical = 24,
        LightHorizontal = 25,
        NarrowVertical = 26,
        NarrowHorizontal = 27,
        DarkVertical = 28,
        DarkHorizontal = 29,
        DashedDownwardDiagonal = 30,
        DashedUpwardDiagonal = 31,
        DashedHorizontal = 32,
        DashedVertical = 33,
        SmallConfetti = 34,
        LargeConfetti = 35,
        ZigZag = 36,
        Wave = 37,
        DiagonalBrick = 38,
        HorizontalBrick = 39,
        Weave = 40,
        Plaid = 41,
        Divot = 42,
        DottedGrid = 43,
        DottedDiamond = 44,
        Shingle = 45,
        Trellis = 46,
        Sphere = 47,
        SmallGrid = 48,
        SmallCheckerBoard = 49,
        LargeCheckerBoard = 50,
        OutlinedDiamond = 51,
        SolidDiamond = 52,
    }

    public enum InterpolationMode
    {
        Invalid = -1,
        Default = 0,
        Low = 1,
        High = 2,
        Bilinear = 3,
        Bicubic = 4,
        NearestNeighbor = 5,
        HighQualityBilinear = 6,
        HighQualityBicubic = 7,
    }

    public sealed partial class LinearGradientBrush : System.Drawing.Brush
    {
        public LinearGradientBrush(System.Drawing.Point point1, System.Drawing.Point point2, System.Drawing.Color color1, System.Drawing.Color color2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public LinearGradientBrush(System.Drawing.PointF point1, System.Drawing.PointF point2, System.Drawing.Color color1, System.Drawing.Color color2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public LinearGradientBrush(System.Drawing.Rectangle rect, System.Drawing.Color color1, System.Drawing.Color color2, System.Drawing.Drawing2D.LinearGradientMode linearGradientMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public LinearGradientBrush(System.Drawing.Rectangle rect, System.Drawing.Color color1, System.Drawing.Color color2, float angle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public LinearGradientBrush(System.Drawing.Rectangle rect, System.Drawing.Color color1, System.Drawing.Color color2, float angle, bool isAngleScaleable) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public LinearGradientBrush(System.Drawing.RectangleF rect, System.Drawing.Color color1, System.Drawing.Color color2, System.Drawing.Drawing2D.LinearGradientMode linearGradientMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public LinearGradientBrush(System.Drawing.RectangleF rect, System.Drawing.Color color1, System.Drawing.Color color2, float angle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public LinearGradientBrush(System.Drawing.RectangleF rect, System.Drawing.Color color1, System.Drawing.Color color2, float angle, bool isAngleScaleable) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.Blend? Blend { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool GammaCorrection { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.ColorBlend InterpolationColors { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Color[] LinearColors { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.RectangleF Rectangle { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.Matrix Transform { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.WrapMode WrapMode { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ResetTransform() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetBlendTriangularShape(float focus) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetBlendTriangularShape(float focus, float scale) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetSigmaBellShape(float focus) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetSigmaBellShape(float focus, float scale) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum LinearGradientMode
    {
        Horizontal = 0,
        Vertical = 1,
        ForwardDiagonal = 2,
        BackwardDiagonal = 3,
    }

    public enum LineCap
    {
        Flat = 0,
        Square = 1,
        Round = 2,
        Triangle = 3,
        NoAnchor = 16,
        SquareAnchor = 17,
        RoundAnchor = 18,
        DiamondAnchor = 19,
        ArrowAnchor = 20,
        AnchorMask = 240,
        Custom = 255,
    }

    public enum LineJoin
    {
        Miter = 0,
        Bevel = 1,
        Round = 2,
        MiterClipped = 3,
    }

    public sealed partial class Matrix : System.MarshalByRefObject, System.IDisposable
    {
        public Matrix() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Matrix(System.Drawing.Rectangle rect, System.Drawing.Point[] plgpts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Matrix(System.Drawing.RectangleF rect, System.Drawing.PointF[] plgpts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Matrix(float m11, float m12, float m21, float m22, float dx, float dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public float[] Elements { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool IsIdentity { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool IsInvertible { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float OffsetX { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float OffsetY { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.Matrix Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        ~Matrix() { }
        public override int GetHashCode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Invert() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Multiply(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Multiply(System.Drawing.Drawing2D.Matrix matrix, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Reset() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Rotate(float angle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Rotate(float angle, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateAt(float angle, System.Drawing.PointF point) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateAt(float angle, System.Drawing.PointF point, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Scale(float scaleX, float scaleY) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Scale(float scaleX, float scaleY, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Shear(float shearX, float shearY) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Shear(float shearX, float shearY, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TransformPoints(System.Drawing.PointF[] pts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TransformPoints(System.Drawing.Point[] pts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TransformVectors(System.Drawing.PointF[] pts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TransformVectors(System.Drawing.Point[] pts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Translate(float offsetX, float offsetY) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Translate(float offsetX, float offsetY, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void VectorTransformPoints(System.Drawing.Point[] pts) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum MatrixOrder
    {
        Prepend = 0,
        Append = 1,
    }

    public sealed partial class PathData
    {
        public PathData() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.PointF[]? Points { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public byte[]? Types { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public sealed partial class PathGradientBrush : System.Drawing.Brush
    {
        public PathGradientBrush(System.Drawing.Drawing2D.GraphicsPath path) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public PathGradientBrush(System.Drawing.PointF[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public PathGradientBrush(System.Drawing.PointF[] points, System.Drawing.Drawing2D.WrapMode wrapMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public PathGradientBrush(System.Drawing.Point[] points) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public PathGradientBrush(System.Drawing.Point[] points, System.Drawing.Drawing2D.WrapMode wrapMode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Drawing2D.Blend Blend { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Color CenterColor { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.PointF CenterPoint { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.PointF FocusScales { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.ColorBlend InterpolationColors { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.RectangleF Rectangle { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Color[] SurroundColors { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.Matrix Transform { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Drawing2D.WrapMode WrapMode { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ResetTransform() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void RotateTransform(float angle, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ScaleTransform(float sx, float sy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetBlendTriangularShape(float focus) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetBlendTriangularShape(float focus, float scale) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetSigmaBellShape(float focus) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetSigmaBellShape(float focus, float scale) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void TranslateTransform(float dx, float dy, System.Drawing.Drawing2D.MatrixOrder order) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum PathPointType
    {
        Start = 0,
        Line = 1,
        Bezier = 3,
        Bezier3 = 3,
        PathTypeMask = 7,
        DashMode = 16,
        PathMarker = 32,
        CloseSubpath = 128,
    }

    public enum PenAlignment
    {
        Center = 0,
        Inset = 1,
        Outset = 2,
        Left = 3,
        Right = 4,
    }

    public enum PenType
    {
        SolidColor = 0,
        HatchFill = 1,
        TextureFill = 2,
        PathGradient = 3,
        LinearGradient = 4,
    }

    public enum PixelOffsetMode
    {
        Invalid = -1,
        Default = 0,
        HighSpeed = 1,
        HighQuality = 2,
        None = 3,
        Half = 4,
    }

    public enum QualityMode
    {
        Invalid = -1,
        Default = 0,
        Low = 1,
        High = 2,
    }

    public sealed partial class RegionData
    {
        internal RegionData() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public byte[] Data { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public enum SmoothingMode
    {
        Invalid = -1,
        Default = 0,
        HighSpeed = 1,
        HighQuality = 2,
        None = 3,
        AntiAlias = 4,
    }

    public enum WarpMode
    {
        Perspective = 0,
        Bilinear = 1,
    }

    public enum WrapMode
    {
        Tile = 0,
        TileFlipX = 1,
        TileFlipY = 2,
        TileFlipXY = 3,
        Clamp = 4,
    }
}

namespace System.Drawing.Imaging
{
    public sealed partial class BitmapData
    {
        public BitmapData() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int Height { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Imaging.PixelFormat PixelFormat { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Reserved { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.IntPtr Scan0 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Stride { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Width { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public enum ColorAdjustType
    {
        Default = 0,
        Bitmap = 1,
        Brush = 2,
        Pen = 3,
        Text = 4,
        Count = 5,
        Any = 6,
    }

    public enum ColorChannelFlag
    {
        ColorChannelC = 0,
        ColorChannelM = 1,
        ColorChannelY = 2,
        ColorChannelK = 3,
        ColorChannelLast = 4,
    }

    public sealed partial class ColorMap
    {
        public ColorMap() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Color NewColor { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Color OldColor { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public enum ColorMapType
    {
        Default = 0,
        Brush = 1,
    }

    public sealed partial class ColorMatrix
    {
        public ColorMatrix() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.CLSCompliantAttribute(false)]
        public ColorMatrix(float[][] newColorMatrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public float this[int row, int column] { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix00 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix01 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix02 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix03 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix04 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix10 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix11 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix12 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix13 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix14 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix20 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix21 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix22 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix23 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix24 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix30 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix31 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix32 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix33 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix34 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix40 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix41 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix42 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix43 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float Matrix44 { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public enum ColorMatrixFlag
    {
        Default = 0,
        SkipGrays = 1,
        AltGrays = 2,
    }

    public enum ColorMode
    {
        Argb32Mode = 0,
        Argb64Mode = 1,
    }

    public sealed partial class ColorPalette
    {
        internal ColorPalette() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Color[] Entries { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Flags { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public enum EmfPlusRecordType
    {
        EmfHeader = 1,
        EmfMin = 1,
        EmfPolyBezier = 2,
        EmfPolygon = 3,
        EmfPolyline = 4,
        EmfPolyBezierTo = 5,
        EmfPolyLineTo = 6,
        EmfPolyPolyline = 7,
        EmfPolyPolygon = 8,
        EmfSetWindowExtEx = 9,
        EmfSetWindowOrgEx = 10,
        EmfSetViewportExtEx = 11,
        EmfSetViewportOrgEx = 12,
        EmfSetBrushOrgEx = 13,
        EmfEof = 14,
        EmfSetPixelV = 15,
        EmfSetMapperFlags = 16,
        EmfSetMapMode = 17,
        EmfSetBkMode = 18,
        EmfSetPolyFillMode = 19,
        EmfSetROP2 = 20,
        EmfSetStretchBltMode = 21,
        EmfSetTextAlign = 22,
        EmfSetColorAdjustment = 23,
        EmfSetTextColor = 24,
        EmfSetBkColor = 25,
        EmfOffsetClipRgn = 26,
        EmfMoveToEx = 27,
        EmfSetMetaRgn = 28,
        EmfExcludeClipRect = 29,
        EmfIntersectClipRect = 30,
        EmfScaleViewportExtEx = 31,
        EmfScaleWindowExtEx = 32,
        EmfSaveDC = 33,
        EmfRestoreDC = 34,
        EmfSetWorldTransform = 35,
        EmfModifyWorldTransform = 36,
        EmfSelectObject = 37,
        EmfCreatePen = 38,
        EmfCreateBrushIndirect = 39,
        EmfDeleteObject = 40,
        EmfAngleArc = 41,
        EmfEllipse = 42,
        EmfRectangle = 43,
        EmfRoundRect = 44,
        EmfRoundArc = 45,
        EmfChord = 46,
        EmfPie = 47,
        EmfSelectPalette = 48,
        EmfCreatePalette = 49,
        EmfSetPaletteEntries = 50,
        EmfResizePalette = 51,
        EmfRealizePalette = 52,
        EmfExtFloodFill = 53,
        EmfLineTo = 54,
        EmfArcTo = 55,
        EmfPolyDraw = 56,
        EmfSetArcDirection = 57,
        EmfSetMiterLimit = 58,
        EmfBeginPath = 59,
        EmfEndPath = 60,
        EmfCloseFigure = 61,
        EmfFillPath = 62,
        EmfStrokeAndFillPath = 63,
        EmfStrokePath = 64,
        EmfFlattenPath = 65,
        EmfWidenPath = 66,
        EmfSelectClipPath = 67,
        EmfAbortPath = 68,
        EmfReserved069 = 69,
        EmfGdiComment = 70,
        EmfFillRgn = 71,
        EmfFrameRgn = 72,
        EmfInvertRgn = 73,
        EmfPaintRgn = 74,
        EmfExtSelectClipRgn = 75,
        EmfBitBlt = 76,
        EmfStretchBlt = 77,
        EmfMaskBlt = 78,
        EmfPlgBlt = 79,
        EmfSetDIBitsToDevice = 80,
        EmfStretchDIBits = 81,
        EmfExtCreateFontIndirect = 82,
        EmfExtTextOutA = 83,
        EmfExtTextOutW = 84,
        EmfPolyBezier16 = 85,
        EmfPolygon16 = 86,
        EmfPolyline16 = 87,
        EmfPolyBezierTo16 = 88,
        EmfPolylineTo16 = 89,
        EmfPolyPolyline16 = 90,
        EmfPolyPolygon16 = 91,
        EmfPolyDraw16 = 92,
        EmfCreateMonoBrush = 93,
        EmfCreateDibPatternBrushPt = 94,
        EmfExtCreatePen = 95,
        EmfPolyTextOutA = 96,
        EmfPolyTextOutW = 97,
        EmfSetIcmMode = 98,
        EmfCreateColorSpace = 99,
        EmfSetColorSpace = 100,
        EmfDeleteColorSpace = 101,
        EmfGlsRecord = 102,
        EmfGlsBoundedRecord = 103,
        EmfPixelFormat = 104,
        EmfDrawEscape = 105,
        EmfExtEscape = 106,
        EmfStartDoc = 107,
        EmfSmallTextOut = 108,
        EmfForceUfiMapping = 109,
        EmfNamedEscpae = 110,
        EmfColorCorrectPalette = 111,
        EmfSetIcmProfileA = 112,
        EmfSetIcmProfileW = 113,
        EmfAlphaBlend = 114,
        EmfSetLayout = 115,
        EmfTransparentBlt = 116,
        EmfReserved117 = 117,
        EmfGradientFill = 118,
        EmfSetLinkedUfis = 119,
        EmfSetTextJustification = 120,
        EmfColorMatchToTargetW = 121,
        EmfCreateColorSpaceW = 122,
        EmfMax = 122,
        EmfPlusRecordBase = 16384,
        Invalid = 16384,
        Header = 16385,
        Min = 16385,
        EndOfFile = 16386,
        Comment = 16387,
        GetDC = 16388,
        MultiFormatStart = 16389,
        MultiFormatSection = 16390,
        MultiFormatEnd = 16391,
        Object = 16392,
        Clear = 16393,
        FillRects = 16394,
        DrawRects = 16395,
        FillPolygon = 16396,
        DrawLines = 16397,
        FillEllipse = 16398,
        DrawEllipse = 16399,
        FillPie = 16400,
        DrawPie = 16401,
        DrawArc = 16402,
        FillRegion = 16403,
        FillPath = 16404,
        DrawPath = 16405,
        FillClosedCurve = 16406,
        DrawClosedCurve = 16407,
        DrawCurve = 16408,
        DrawBeziers = 16409,
        DrawImage = 16410,
        DrawImagePoints = 16411,
        DrawString = 16412,
        SetRenderingOrigin = 16413,
        SetAntiAliasMode = 16414,
        SetTextRenderingHint = 16415,
        SetTextContrast = 16416,
        SetInterpolationMode = 16417,
        SetPixelOffsetMode = 16418,
        SetCompositingMode = 16419,
        SetCompositingQuality = 16420,
        Save = 16421,
        Restore = 16422,
        BeginContainer = 16423,
        BeginContainerNoParams = 16424,
        EndContainer = 16425,
        SetWorldTransform = 16426,
        ResetWorldTransform = 16427,
        MultiplyWorldTransform = 16428,
        TranslateWorldTransform = 16429,
        ScaleWorldTransform = 16430,
        RotateWorldTransform = 16431,
        SetPageTransform = 16432,
        ResetClip = 16433,
        SetClipRect = 16434,
        SetClipPath = 16435,
        SetClipRegion = 16436,
        OffsetClip = 16437,
        DrawDriverString = 16438,
        Max = 16438,
        Total = 16439,
        WmfRecordBase = 65536,
        WmfSaveDC = 65566,
        WmfRealizePalette = 65589,
        WmfSetPalEntries = 65591,
        WmfCreatePalette = 65783,
        WmfSetBkMode = 65794,
        WmfSetMapMode = 65795,
        WmfSetROP2 = 65796,
        WmfSetRelAbs = 65797,
        WmfSetPolyFillMode = 65798,
        WmfSetStretchBltMode = 65799,
        WmfSetTextCharExtra = 65800,
        WmfRestoreDC = 65831,
        WmfInvertRegion = 65834,
        WmfPaintRegion = 65835,
        WmfSelectClipRegion = 65836,
        WmfSelectObject = 65837,
        WmfSetTextAlign = 65838,
        WmfResizePalette = 65849,
        WmfDibCreatePatternBrush = 65858,
        WmfSetLayout = 65865,
        WmfDeleteObject = 66032,
        WmfCreatePatternBrush = 66041,
        WmfSetBkColor = 66049,
        WmfSetTextColor = 66057,
        WmfSetTextJustification = 66058,
        WmfSetWindowOrg = 66059,
        WmfSetWindowExt = 66060,
        WmfSetViewportOrg = 66061,
        WmfSetViewportExt = 66062,
        WmfOffsetWindowOrg = 66063,
        WmfOffsetViewportOrg = 66065,
        WmfLineTo = 66067,
        WmfMoveTo = 66068,
        WmfOffsetCilpRgn = 66080,
        WmfFillRegion = 66088,
        WmfSetMapperFlags = 66097,
        WmfSelectPalette = 66100,
        WmfCreatePenIndirect = 66298,
        WmfCreateFontIndirect = 66299,
        WmfCreateBrushIndirect = 66300,
        WmfPolygon = 66340,
        WmfPolyline = 66341,
        WmfScaleWindowExt = 66576,
        WmfScaleViewportExt = 66578,
        WmfExcludeClipRect = 66581,
        WmfIntersectClipRect = 66582,
        WmfEllipse = 66584,
        WmfFloodFill = 66585,
        WmfRectangle = 66587,
        WmfSetPixel = 66591,
        WmfFrameRegion = 66601,
        WmfAnimatePalette = 66614,
        WmfTextOut = 66849,
        WmfPolyPolygon = 66872,
        WmfExtFloodFill = 66888,
        WmfRoundRect = 67100,
        WmfPatBlt = 67101,
        WmfEscape = 67110,
        WmfCreateRegion = 67327,
        WmfArc = 67607,
        WmfPie = 67610,
        WmfChord = 67632,
        WmfBitBlt = 67874,
        WmfDibBitBlt = 67904,
        WmfExtTextOut = 68146,
        WmfStretchBlt = 68387,
        WmfDibStretchBlt = 68417,
        WmfSetDibToDev = 68915,
        WmfStretchDib = 69443,
    }

    public enum EmfType
    {
        EmfOnly = 3,
        EmfPlusOnly = 4,
        EmfPlusDual = 5,
    }

    public sealed partial class Encoder
    {
        public static readonly System.Drawing.Imaging.Encoder ChrominanceTable;
        public static readonly System.Drawing.Imaging.Encoder ColorDepth;
        public static readonly System.Drawing.Imaging.Encoder ColorSpace;
        public static readonly System.Drawing.Imaging.Encoder Compression;
        public static readonly System.Drawing.Imaging.Encoder ImageItems;
        public static readonly System.Drawing.Imaging.Encoder LuminanceTable;
        public static readonly System.Drawing.Imaging.Encoder Quality;
        public static readonly System.Drawing.Imaging.Encoder RenderMethod;
        public static readonly System.Drawing.Imaging.Encoder SaveAsCmyk;
        public static readonly System.Drawing.Imaging.Encoder SaveFlag;
        public static readonly System.Drawing.Imaging.Encoder ScanMethod;
        public static readonly System.Drawing.Imaging.Encoder Transformation;
        public static readonly System.Drawing.Imaging.Encoder Version;
        public Encoder(System.Guid guid) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Guid Guid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public sealed partial class EncoderParameter : System.IDisposable
    {
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, byte value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, byte value, bool undefined) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, byte[] value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, byte[] value, bool undefined) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, short value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, short[] value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, int numberValues, System.Drawing.Imaging.EncoderParameterValueType type, System.IntPtr value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, int numerator, int denominator) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ObsoleteAttribute("This constructor has been deprecated. Use EncoderParameter(Encoder encoder, int numberValues, EncoderParameterValueType type, IntPtr value) instead.")]
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, int NumberOfValues, int Type, int Value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, int numerator1, int demoninator1, int numerator2, int demoninator2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, int[] numerator, int[] denominator) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, long value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, long rangebegin, long rangeend) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, long[] value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, long[] rangebegin, long[] rangeend) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameter(System.Drawing.Imaging.Encoder encoder, string value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Imaging.Encoder Encoder { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int NumberOfValues { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Imaging.EncoderParameterValueType Type { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Imaging.EncoderParameterValueType ValueType { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public void Dispose() { }
        ~EncoderParameter() { }
    }

    public sealed partial class EncoderParameters : System.IDisposable
    {
        public EncoderParameters() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public EncoderParameters(int count) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Imaging.EncoderParameter[] Param { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public void Dispose() { }
    }

    public enum EncoderParameterValueType
    {
        ValueTypeByte = 1,
        ValueTypeAscii = 2,
        ValueTypeShort = 3,
        ValueTypeLong = 4,
        ValueTypeRational = 5,
        ValueTypeLongRange = 6,
        ValueTypeUndefined = 7,
        ValueTypeRationalRange = 8,
        ValueTypePointer = 9,
    }

    public enum EncoderValue
    {
        ColorTypeCMYK = 0,
        ColorTypeYCCK = 1,
        CompressionLZW = 2,
        CompressionCCITT3 = 3,
        CompressionCCITT4 = 4,
        CompressionRle = 5,
        CompressionNone = 6,
        ScanMethodInterlaced = 7,
        ScanMethodNonInterlaced = 8,
        VersionGif87 = 9,
        VersionGif89 = 10,
        RenderProgressive = 11,
        RenderNonProgressive = 12,
        TransformRotate90 = 13,
        TransformRotate180 = 14,
        TransformRotate270 = 15,
        TransformFlipHorizontal = 16,
        TransformFlipVertical = 17,
        MultiFrame = 18,
        LastFrame = 19,
        Flush = 20,
        FrameDimensionTime = 21,
        FrameDimensionResolution = 22,
        FrameDimensionPage = 23,
    }

    public sealed partial class FrameDimension
    {
        public FrameDimension(System.Guid guid) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Guid Guid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.FrameDimension Page { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.FrameDimension Resolution { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.FrameDimension Time { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? o) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override int GetHashCode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class ImageAttributes : System.ICloneable, System.IDisposable
    {
        public ImageAttributes() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearBrushRemapTable() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearColorKey() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearColorKey(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearColorMatrix() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearColorMatrix(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearGamma() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearGamma(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearNoOp() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearNoOp(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearOutputChannel() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearOutputChannel(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearOutputChannelColorProfile() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearOutputChannelColorProfile(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearRemapTable() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearRemapTable(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearThreshold() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void ClearThreshold(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Dispose() { }
        ~ImageAttributes() { }
        public void GetAdjustedPalette(System.Drawing.Imaging.ColorPalette palette, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetBrushRemapTable(System.Drawing.Imaging.ColorMap[] map) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetColorKey(System.Drawing.Color colorLow, System.Drawing.Color colorHigh) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetColorKey(System.Drawing.Color colorLow, System.Drawing.Color colorHigh, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetColorMatrices(System.Drawing.Imaging.ColorMatrix newColorMatrix, System.Drawing.Imaging.ColorMatrix? grayMatrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetColorMatrices(System.Drawing.Imaging.ColorMatrix newColorMatrix, System.Drawing.Imaging.ColorMatrix? grayMatrix, System.Drawing.Imaging.ColorMatrixFlag flags) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetColorMatrices(System.Drawing.Imaging.ColorMatrix newColorMatrix, System.Drawing.Imaging.ColorMatrix? grayMatrix, System.Drawing.Imaging.ColorMatrixFlag mode, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetColorMatrix(System.Drawing.Imaging.ColorMatrix newColorMatrix) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetColorMatrix(System.Drawing.Imaging.ColorMatrix newColorMatrix, System.Drawing.Imaging.ColorMatrixFlag flags) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetColorMatrix(System.Drawing.Imaging.ColorMatrix newColorMatrix, System.Drawing.Imaging.ColorMatrixFlag mode, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetGamma(float gamma) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetGamma(float gamma, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetNoOp() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetNoOp(System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetOutputChannel(System.Drawing.Imaging.ColorChannelFlag flags) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetOutputChannel(System.Drawing.Imaging.ColorChannelFlag flags, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetOutputChannelColorProfile(string colorProfileFilename) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetOutputChannelColorProfile(string colorProfileFilename, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetRemapTable(System.Drawing.Imaging.ColorMap[] map) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetRemapTable(System.Drawing.Imaging.ColorMap[] map, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetThreshold(float threshold) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetThreshold(float threshold, System.Drawing.Imaging.ColorAdjustType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetWrapMode(System.Drawing.Drawing2D.WrapMode mode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetWrapMode(System.Drawing.Drawing2D.WrapMode mode, System.Drawing.Color color) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetWrapMode(System.Drawing.Drawing2D.WrapMode mode, System.Drawing.Color color, bool clamp) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.FlagsAttribute]
    public enum ImageCodecFlags
    {
        Encoder = 1,
        Decoder = 2,
        SupportBitmap = 4,
        SupportVector = 8,
        SeekableEncode = 16,
        BlockingDecode = 32,
        Builtin = 65536,
        System = 131072,
        User = 262144,
    }

    public sealed partial class ImageCodecInfo
    {
        internal ImageCodecInfo() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Guid Clsid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string? CodecName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string? DllName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string? FilenameExtension { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Imaging.ImageCodecFlags Flags { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string? FormatDescription { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Guid FormatID { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string? MimeType { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.CLSCompliantAttribute(false)]
        public byte[][]? SignatureMasks { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.CLSCompliantAttribute(false)]
        public byte[][]? SignaturePatterns { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Version { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageCodecInfo[] GetImageDecoders() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Imaging.ImageCodecInfo[] GetImageEncoders() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.FlagsAttribute]
    public enum ImageFlags
    {
        None = 0,
        Scalable = 1,
        HasAlpha = 2,
        HasTranslucent = 4,
        PartiallyScalable = 8,
        ColorSpaceRgb = 16,
        ColorSpaceCmyk = 32,
        ColorSpaceGray = 64,
        ColorSpaceYcbcr = 128,
        ColorSpaceYcck = 256,
        HasRealDpi = 4096,
        HasRealPixelSize = 8192,
        ReadOnly = 65536,
        Caching = 131072,
    }

    [System.ComponentModel.TypeConverterAttribute(typeof(System.Drawing.ImageFormatConverter))]
    public sealed partial class ImageFormat
    {
        public ImageFormat(System.Guid guid) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Imaging.ImageFormat Bmp { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat Emf { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat Exif { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat Gif { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Guid Guid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat Icon { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat Jpeg { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat MemoryBmp { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat Png { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat Tiff { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Imaging.ImageFormat Wmf { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? o) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override int GetHashCode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum ImageLockMode
    {
        ReadOnly = 1,
        WriteOnly = 2,
        ReadWrite = 3,
        UserInputBuffer = 4,
    }

    [System.ComponentModel.EditorAttribute("System.Drawing.Design.MetafileEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public sealed partial class Metafile : System.Drawing.Image
    {
        public Metafile(System.IntPtr henhmetafile, bool deleteEmf) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.Imaging.EmfType emfType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.Imaging.EmfType emfType, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr hmetafile, System.Drawing.Imaging.WmfPlaceableFileHeader wmfHeader) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr hmetafile, System.Drawing.Imaging.WmfPlaceableFileHeader wmfHeader, bool deleteWmf) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type, string? desc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.Imaging.EmfType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.Imaging.EmfType type, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(System.IO.Stream stream, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string filename) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.Imaging.EmfType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.Imaging.EmfType type, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.Rectangle frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type, string? description) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Metafile(string fileName, System.IntPtr referenceHdc, System.Drawing.RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, string? desc) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.IntPtr GetHenhmetafile() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Imaging.MetafileHeader GetMetafileHeader() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Imaging.MetafileHeader GetMetafileHeader(System.IntPtr henhmetafile) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Imaging.MetafileHeader GetMetafileHeader(System.IntPtr hmetafile, System.Drawing.Imaging.WmfPlaceableFileHeader wmfHeader) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Imaging.MetafileHeader GetMetafileHeader(System.IO.Stream stream) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Imaging.MetafileHeader GetMetafileHeader(string fileName) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void PlayRecord(System.Drawing.Imaging.EmfPlusRecordType recordType, int flags, int dataSize, byte[] data) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum MetafileFrameUnit
    {
        Pixel = 2,
        Point = 3,
        Inch = 4,
        Document = 5,
        Millimeter = 6,
        GdiCompatible = 7,
    }

    public sealed partial class MetafileHeader
    {
        internal MetafileHeader() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Rectangle Bounds { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float DpiX { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float DpiY { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int EmfPlusHeaderSize { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int LogicalDpiX { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int LogicalDpiY { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int MetafileSize { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Imaging.MetafileType Type { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Version { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Imaging.MetaHeader WmfHeader { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool IsDisplay() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsEmf() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsEmfOrEmfPlus() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsEmfPlus() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsEmfPlusDual() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsEmfPlusOnly() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsWmf() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsWmfPlaceable() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum MetafileType
    {
        Invalid = 0,
        Wmf = 1,
        WmfPlaceable = 2,
        Emf = 3,
        EmfPlusOnly = 4,
        EmfPlusDual = 5,
    }

    public sealed partial class MetaHeader
    {
        public MetaHeader() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public short HeaderSize { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int MaxRecord { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short NoObjects { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short NoParameters { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Size { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short Type { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short Version { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    [System.FlagsAttribute]
    public enum PaletteFlags
    {
        HasAlpha = 1,
        GrayScale = 2,
        Halftone = 4,
    }

    public enum PixelFormat
    {
        DontCare = 0,
        Undefined = 0,
        Max = 15,
        Indexed = 65536,
        Gdi = 131072,
        Format16bppRgb555 = 135173,
        Format16bppRgb565 = 135174,
        Format24bppRgb = 137224,
        Format32bppRgb = 139273,
        Format1bppIndexed = 196865,
        Format4bppIndexed = 197634,
        Format8bppIndexed = 198659,
        Alpha = 262144,
        Format16bppArgb1555 = 397319,
        PAlpha = 524288,
        Format32bppPArgb = 925707,
        Extended = 1048576,
        Format16bppGrayScale = 1052676,
        Format48bppRgb = 1060876,
        Format64bppPArgb = 1851406,
        Canonical = 2097152,
        Format32bppArgb = 2498570,
        Format64bppArgb = 3424269,
    }

    public delegate void PlayRecordCallback(System.Drawing.Imaging.EmfPlusRecordType recordType, int flags, int dataSize, System.IntPtr recordData);
    public sealed partial class PropertyItem
    {
        internal PropertyItem() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int Id { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Len { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short Type { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public byte[]? Value { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public sealed partial class WmfPlaceableFileHeader
    {
        public WmfPlaceableFileHeader() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public short BboxBottom { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short BboxLeft { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short BboxRight { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short BboxTop { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short Checksum { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short Hmf { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short Inch { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Key { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Reserved { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }
}

namespace System.Drawing.Printing
{
    public enum Duplex
    {
        Default = -1,
        Simplex = 1,
        Vertical = 2,
        Horizontal = 3,
    }

    public partial class InvalidPrinterException : System.SystemException
    {
        public InvalidPrinterException(System.Drawing.Printing.PrinterSettings settings) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        protected InvalidPrinterException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.ComponentModel.TypeConverterAttribute(typeof(System.Drawing.Printing.MarginsConverter))]
    public partial class Margins : System.ICloneable
    {
        public Margins() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public Margins(int left, int right, int top, int bottom) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int Bottom { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Left { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Right { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Top { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override int GetHashCode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static bool operator ==(System.Drawing.Printing.Margins? m1, System.Drawing.Printing.Margins? m2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static bool operator !=(System.Drawing.Printing.Margins? m1, System.Drawing.Printing.Margins? m2) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class MarginsConverter : System.ComponentModel.ExpandableObjectConverter
    {
        public MarginsConverter() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] System.Type? destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext? context, System.Collections.IDictionary propertyValues) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext? context) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class PageSettings : System.ICloneable
    {
        public PageSettings() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public PageSettings(System.Drawing.Printing.PrinterSettings printerSettings) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Rectangle Bounds { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool Color { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float HardMarginX { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public float HardMarginY { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool Landscape { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.Margins Margins { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PaperSize PaperSize { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PaperSource PaperSource { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.RectangleF PrintableArea { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PrinterResolution PrinterResolution { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PrinterSettings PrinterSettings { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void CopyToHdevmode(System.IntPtr hdevmode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetHdevmode(System.IntPtr hdevmode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum PaperKind
    {
        Custom = 0,
        Letter = 1,
        LetterSmall = 2,
        Tabloid = 3,
        Ledger = 4,
        Legal = 5,
        Statement = 6,
        Executive = 7,
        A3 = 8,
        A4 = 9,
        A4Small = 10,
        A5 = 11,
        B4 = 12,
        B5 = 13,
        Folio = 14,
        Quarto = 15,
        Standard10x14 = 16,
        Standard11x17 = 17,
        Note = 18,
        Number9Envelope = 19,
        Number10Envelope = 20,
        Number11Envelope = 21,
        Number12Envelope = 22,
        Number14Envelope = 23,
        CSheet = 24,
        DSheet = 25,
        ESheet = 26,
        DLEnvelope = 27,
        C5Envelope = 28,
        C3Envelope = 29,
        C4Envelope = 30,
        C6Envelope = 31,
        C65Envelope = 32,
        B4Envelope = 33,
        B5Envelope = 34,
        B6Envelope = 35,
        ItalyEnvelope = 36,
        MonarchEnvelope = 37,
        PersonalEnvelope = 38,
        USStandardFanfold = 39,
        GermanStandardFanfold = 40,
        GermanLegalFanfold = 41,
        IsoB4 = 42,
        JapanesePostcard = 43,
        Standard9x11 = 44,
        Standard10x11 = 45,
        Standard15x11 = 46,
        InviteEnvelope = 47,
        LetterExtra = 50,
        LegalExtra = 51,
        TabloidExtra = 52,
        A4Extra = 53,
        LetterTransverse = 54,
        A4Transverse = 55,
        LetterExtraTransverse = 56,
        APlus = 57,
        BPlus = 58,
        LetterPlus = 59,
        A4Plus = 60,
        A5Transverse = 61,
        B5Transverse = 62,
        A3Extra = 63,
        A5Extra = 64,
        B5Extra = 65,
        A2 = 66,
        A3Transverse = 67,
        A3ExtraTransverse = 68,
        JapaneseDoublePostcard = 69,
        A6 = 70,
        JapaneseEnvelopeKakuNumber2 = 71,
        JapaneseEnvelopeKakuNumber3 = 72,
        JapaneseEnvelopeChouNumber3 = 73,
        JapaneseEnvelopeChouNumber4 = 74,
        LetterRotated = 75,
        A3Rotated = 76,
        A4Rotated = 77,
        A5Rotated = 78,
        B4JisRotated = 79,
        B5JisRotated = 80,
        JapanesePostcardRotated = 81,
        JapaneseDoublePostcardRotated = 82,
        A6Rotated = 83,
        JapaneseEnvelopeKakuNumber2Rotated = 84,
        JapaneseEnvelopeKakuNumber3Rotated = 85,
        JapaneseEnvelopeChouNumber3Rotated = 86,
        JapaneseEnvelopeChouNumber4Rotated = 87,
        B6Jis = 88,
        B6JisRotated = 89,
        Standard12x11 = 90,
        JapaneseEnvelopeYouNumber4 = 91,
        JapaneseEnvelopeYouNumber4Rotated = 92,
        Prc16K = 93,
        Prc32K = 94,
        Prc32KBig = 95,
        PrcEnvelopeNumber1 = 96,
        PrcEnvelopeNumber2 = 97,
        PrcEnvelopeNumber3 = 98,
        PrcEnvelopeNumber4 = 99,
        PrcEnvelopeNumber5 = 100,
        PrcEnvelopeNumber6 = 101,
        PrcEnvelopeNumber7 = 102,
        PrcEnvelopeNumber8 = 103,
        PrcEnvelopeNumber9 = 104,
        PrcEnvelopeNumber10 = 105,
        Prc16KRotated = 106,
        Prc32KRotated = 107,
        Prc32KBigRotated = 108,
        PrcEnvelopeNumber1Rotated = 109,
        PrcEnvelopeNumber2Rotated = 110,
        PrcEnvelopeNumber3Rotated = 111,
        PrcEnvelopeNumber4Rotated = 112,
        PrcEnvelopeNumber5Rotated = 113,
        PrcEnvelopeNumber6Rotated = 114,
        PrcEnvelopeNumber7Rotated = 115,
        PrcEnvelopeNumber8Rotated = 116,
        PrcEnvelopeNumber9Rotated = 117,
        PrcEnvelopeNumber10Rotated = 118,
    }

    public partial class PaperSize
    {
        public PaperSize() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public PaperSize(string name, int width, int height) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public int Height { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PaperKind Kind { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string PaperName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int RawKind { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Width { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class PaperSource
    {
        public PaperSource() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Printing.PaperSourceKind Kind { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int RawKind { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string SourceName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum PaperSourceKind
    {
        Upper = 1,
        Lower = 2,
        Middle = 3,
        Manual = 4,
        Envelope = 5,
        ManualFeed = 6,
        AutomaticFeed = 7,
        TractorFeed = 8,
        SmallFormat = 9,
        LargeFormat = 10,
        LargeCapacity = 11,
        Cassette = 14,
        FormSource = 15,
        Custom = 257,
    }

    public sealed partial class PreviewPageInfo
    {
        public PreviewPageInfo(System.Drawing.Image image, System.Drawing.Size physicalSize) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Image Image { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Size PhysicalSize { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public partial class PreviewPrintController : System.Drawing.Printing.PrintController
    {
        public PreviewPrintController() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override bool IsPreview { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public virtual bool UseAntiAlias { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PreviewPageInfo[] GetPreviewPageInfo() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override void OnEndPage(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintPageEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override void OnEndPrint(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override System.Drawing.Graphics OnStartPage(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintPageEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override void OnStartPrint(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum PrintAction
    {
        PrintToFile = 0,
        PrintToPreview = 1,
        PrintToPrinter = 2,
    }

    public abstract partial class PrintController
    {
        protected PrintController() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public virtual bool IsPreview { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public virtual void OnEndPage(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintPageEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public virtual void OnEndPrint(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public virtual System.Drawing.Graphics? OnStartPage(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintPageEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public virtual void OnStartPrint(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    [System.ComponentModel.DefaultEventAttribute("PrintPage")]
    [System.ComponentModel.DefaultPropertyAttribute("DocumentName")]
    public partial class PrintDocument : System.ComponentModel.Component
    {
        public PrintDocument() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        [System.ComponentModel.BrowsableAttribute(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Printing.PageSettings DefaultPageSettings { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DefaultValueAttribute("document")]
        public string DocumentName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool OriginAtMargins { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Printing.PrintController PrintController { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        [System.ComponentModel.BrowsableAttribute(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Printing.PrinterSettings PrinterSettings { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public event System.Drawing.Printing.PrintEventHandler BeginPrint { add { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } remove { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public event System.Drawing.Printing.PrintEventHandler EndPrint { add { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } remove { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public event System.Drawing.Printing.PrintPageEventHandler PrintPage { add { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } remove { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public event System.Drawing.Printing.QueryPageSettingsEventHandler QueryPageSettings { add { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } remove { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        protected internal virtual void OnBeginPrint(System.Drawing.Printing.PrintEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        protected internal virtual void OnEndPrint(System.Drawing.Printing.PrintEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        protected internal virtual void OnPrintPage(System.Drawing.Printing.PrintPageEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        protected internal virtual void OnQueryPageSettings(System.Drawing.Printing.QueryPageSettingsEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void Print() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class PrinterResolution
    {
        public PrinterResolution() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Printing.PrinterResolutionKind Kind { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int X { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int Y { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public enum PrinterResolutionKind
    {
        High = -4,
        Medium = -3,
        Low = -2,
        Draft = -1,
        Custom = 0,
    }

    public partial class PrinterSettings : System.ICloneable
    {
        public PrinterSettings() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool CanDuplex { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool Collate { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public short Copies { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PageSettings DefaultPageSettings { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.Duplex Duplex { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int FromPage { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public static System.Drawing.Printing.PrinterSettings.StringCollection InstalledPrinters { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool IsDefaultPrinter { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool IsPlotter { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool IsValid { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int LandscapeAngle { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int MaximumCopies { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int MaximumPage { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int MinimumPage { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PrinterSettings.PaperSizeCollection PaperSizes { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PrinterSettings.PaperSourceCollection PaperSources { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string PrinterName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PrinterSettings.PrinterResolutionCollection PrinterResolutions { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public string PrintFileName { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PrintRange PrintRange { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool PrintToFile { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool SupportsColor { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public int ToPage { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public object Clone() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Graphics CreateMeasurementGraphics() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Graphics CreateMeasurementGraphics(bool honorOriginAtMargins) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Graphics CreateMeasurementGraphics(System.Drawing.Printing.PageSettings pageSettings) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Graphics CreateMeasurementGraphics(System.Drawing.Printing.PageSettings pageSettings, bool honorOriginAtMargins) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.IntPtr GetHdevmode() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.IntPtr GetHdevmode(System.Drawing.Printing.PageSettings pageSettings) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.IntPtr GetHdevnames() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsDirectPrintingSupported(System.Drawing.Image image) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool IsDirectPrintingSupported(System.Drawing.Imaging.ImageFormat imageFormat) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetHdevmode(System.IntPtr hdevmode) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void SetHdevnames(System.IntPtr hdevnames) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override string ToString() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public partial class PaperSizeCollection : System.Collections.ICollection, System.Collections.IEnumerable
        {
            public PaperSizeCollection(System.Drawing.Printing.PaperSize[] array) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public int Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            public virtual System.Drawing.Printing.PaperSize this[int index] { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            int System.Collections.ICollection.Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            bool System.Collections.ICollection.IsSynchronized { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            object System.Collections.ICollection.SyncRoot { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            public int Add(System.Drawing.Printing.PaperSize paperSize) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public void CopyTo(System.Drawing.Printing.PaperSize[] paperSizes, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public System.Collections.IEnumerator GetEnumerator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        }

        public partial class PaperSourceCollection : System.Collections.ICollection, System.Collections.IEnumerable
        {
            public PaperSourceCollection(System.Drawing.Printing.PaperSource[] array) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public int Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            public virtual System.Drawing.Printing.PaperSource this[int index] { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            int System.Collections.ICollection.Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            bool System.Collections.ICollection.IsSynchronized { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            object System.Collections.ICollection.SyncRoot { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            public int Add(System.Drawing.Printing.PaperSource paperSource) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public void CopyTo(System.Drawing.Printing.PaperSource[] paperSources, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public System.Collections.IEnumerator GetEnumerator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        }

        public partial class PrinterResolutionCollection : System.Collections.ICollection, System.Collections.IEnumerable
        {
            public PrinterResolutionCollection(System.Drawing.Printing.PrinterResolution[] array) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public int Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            public virtual System.Drawing.Printing.PrinterResolution this[int index] { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            int System.Collections.ICollection.Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            bool System.Collections.ICollection.IsSynchronized { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            object System.Collections.ICollection.SyncRoot { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            public int Add(System.Drawing.Printing.PrinterResolution printerResolution) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public void CopyTo(System.Drawing.Printing.PrinterResolution[] printerResolutions, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public System.Collections.IEnumerator GetEnumerator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        }

        public partial class StringCollection : System.Collections.ICollection, System.Collections.IEnumerable
        {
            public StringCollection(string[] array) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public int Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            public virtual string this[int index] { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            int System.Collections.ICollection.Count { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            bool System.Collections.ICollection.IsSynchronized { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            object System.Collections.ICollection.SyncRoot { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            public int Add(string value) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public void CopyTo(string[] strings, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            public System.Collections.IEnumerator GetEnumerator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        }
    }

    public enum PrinterUnit
    {
        Display = 0,
        ThousandthsOfAnInch = 1,
        HundredthsOfAMillimeter = 2,
        TenthsOfAMillimeter = 3,
    }

    public sealed partial class PrinterUnitConvert
    {
        internal PrinterUnitConvert() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static double Convert(double value, System.Drawing.Printing.PrinterUnit fromUnit, System.Drawing.Printing.PrinterUnit toUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Point Convert(System.Drawing.Point value, System.Drawing.Printing.PrinterUnit fromUnit, System.Drawing.Printing.PrinterUnit toUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Printing.Margins Convert(System.Drawing.Printing.Margins value, System.Drawing.Printing.PrinterUnit fromUnit, System.Drawing.Printing.PrinterUnit toUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Rectangle Convert(System.Drawing.Rectangle value, System.Drawing.Printing.PrinterUnit fromUnit, System.Drawing.Printing.PrinterUnit toUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static System.Drawing.Size Convert(System.Drawing.Size value, System.Drawing.Printing.PrinterUnit fromUnit, System.Drawing.Printing.PrinterUnit toUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public static int Convert(int value, System.Drawing.Printing.PrinterUnit fromUnit, System.Drawing.Printing.PrinterUnit toUnit) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public partial class PrintEventArgs : System.ComponentModel.CancelEventArgs
    {
        public PrintEventArgs() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Printing.PrintAction PrintAction { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public delegate void PrintEventHandler(object sender, System.Drawing.Printing.PrintEventArgs e);
    public partial class PrintPageEventArgs : System.EventArgs
    {
        public PrintPageEventArgs(System.Drawing.Graphics? graphics, System.Drawing.Rectangle marginBounds, System.Drawing.Rectangle pageBounds, System.Drawing.Printing.PageSettings pageSettings) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public bool Cancel { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Graphics? Graphics { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public bool HasMorePages { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Rectangle MarginBounds { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Rectangle PageBounds { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public System.Drawing.Printing.PageSettings PageSettings { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public delegate void PrintPageEventHandler(object sender, System.Drawing.Printing.PrintPageEventArgs e);
    public enum PrintRange
    {
        AllPages = 0,
        Selection = 1,
        SomePages = 2,
        CurrentPage = 4194304,
    }

    public partial class QueryPageSettingsEventArgs : System.Drawing.Printing.PrintEventArgs
    {
        public QueryPageSettingsEventArgs(System.Drawing.Printing.PageSettings pageSettings) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.Printing.PageSettings PageSettings { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } set { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
    }

    public delegate void QueryPageSettingsEventHandler(object sender, System.Drawing.Printing.QueryPageSettingsEventArgs e);
    public partial class StandardPrintController : System.Drawing.Printing.PrintController
    {
        public StandardPrintController() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override void OnEndPage(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintPageEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override void OnEndPrint(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override System.Drawing.Graphics OnStartPage(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintPageEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public override void OnStartPrint(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintEventArgs e) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }
}

namespace System.Drawing.Text
{
    public abstract partial class FontCollection : System.IDisposable
    {
        internal FontCollection() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public System.Drawing.FontFamily[] Families { get { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  } }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        ~FontCollection() { }
    }

    public enum GenericFontFamilies
    {
        Serif = 0,
        SansSerif = 1,
        Monospace = 2,
    }

    public enum HotkeyPrefix
    {
        None = 0,
        Show = 1,
        Hide = 2,
    }

    public sealed partial class InstalledFontCollection : System.Drawing.Text.FontCollection
    {
        public InstalledFontCollection() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
    }

    public sealed partial class PrivateFontCollection : System.Drawing.Text.FontCollection
    {
        public PrivateFontCollection() { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddFontFile(string filename) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        public void AddMemoryFont(System.IntPtr memory, int length) { throw new System.PlatformNotSupportedException(System.SR.SystemDrawingCommon_PlatformNotSupported);  }
        protected override void Dispose(bool disposing) { }
    }

    public enum TextRenderingHint
    {
        SystemDefault = 0,
        SingleBitPerPixelGridFit = 1,
        SingleBitPerPixel = 2,
        AntiAliasGridFit = 3,
        AntiAlias = 4,
        ClearTypeGridFit = 5,
    }
}

#pragma warning restore CS8618,CS0169,CA1725,CA1821,CA1823,CA1066,IDE0001,IDE0002,IDE1006,IDE0034,IDE0044,IDE0051,IDE0055,IDE1006
