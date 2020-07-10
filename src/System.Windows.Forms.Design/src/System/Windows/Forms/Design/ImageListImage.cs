// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;

namespace System.Windows.Forms.Design
{
    [Editor(typeof(ImageListImageEditor), typeof(UITypeEditor))]
    internal class ImageListImage
    {
        private string _name;

        public ImageListImage(Image image)
        {
            Image = image;
        }

        public ImageListImage(Image image, string name)
        {
            Image = image;
            Name = name;
        }

        public string Name
        {
            get => _name ?? string.Empty;
            set => _name = value;
        }

        [Browsable(false)]
        public Image Image { get; set; }

        // Add properties to make this object "look" like Image in the Collection editor
        public float HorizontalResolution => Image.HorizontalResolution;

        public float VerticalResolution => Image.VerticalResolution;

        public PixelFormat PixelFormat => Image.PixelFormat;

        public ImageFormat RawFormat => Image.RawFormat;

        public Size Size => Image.Size;

        public SizeF PhysicalDimension => Image.Size;

        public static ImageListImage ImageListImageFromStream(Stream stream, bool imageIsIcon)
        {
            if (imageIsIcon)
            {
                return new ImageListImage((new Icon(stream)).ToBitmap());
            }

            return new ImageListImage((Bitmap)Bitmap.FromStream(stream));
        }
    }
}
