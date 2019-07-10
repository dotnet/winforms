//------------------------------------------------------------------------------
// <copyright file="ImageListImage.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Windows.Forms.Design {
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.IO;   
    using System.Drawing;
    using System.Drawing.Imaging;
    

    [Editor(typeof(System.Windows.Forms.Design.ImageListImageEditor), typeof(UITypeEditor))]
    internal class ImageListImage {

        public ImageListImage(Bitmap image) {
            Image = image;
        }

        public ImageListImage (Bitmap image, string name) {
            Image = image;
            Name = name;
        }
            
        private string _name = null;
        private Bitmap _image = null;
        
        public string Name {
            get { return (_name == null) ? "" : _name; }
            set {
                _name = value; 
            }
        }

        [Browsable(false)]
        public Bitmap Image {
            get { return _image; }
            set { _image = value; }
        }

       // Add properties to make this object "look" like Image in the Collection editor
       public float HorizontalResolution {
            get { return _image.HorizontalResolution; }
       }

        public float VerticalResolution {
            get { return _image.VerticalResolution; }
        }

        public PixelFormat PixelFormat {
            get { return _image.PixelFormat; }
        }
        
        public ImageFormat RawFormat {
            get { return _image.RawFormat; }
        }

        public Size Size {
            get { return _image.Size; }
        }

        public SizeF PhysicalDimension {
            get { return _image.Size; }
        }
                
        public static ImageListImage ImageListImageFromStream(Stream stream, bool imageIsIcon) {
            if(imageIsIcon) {
                return new ImageListImage((new Icon(stream)).ToBitmap());
            } else {
                return new ImageListImage((Bitmap)Bitmap.FromStream(stream));
            }
        }
    }
}
