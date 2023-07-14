// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Design;

/// <summary>
/// Provides design-time functionality for ImageList.
/// </summary>
internal class ImageListDesigner : ComponentDesigner
{
    // The designer keeps a backup copy of all the images in the image list.  Unlike the image list,
    // we don't lose any information about size and color depth.
    private OriginalImageCollection? _originalImageCollection;
    private DesignerActionListCollection? _actionLists;

    /// <summary>
    /// Accessor method for the ColorDepth property on ImageList.
    /// We shadow this property at design time.
    /// </summary>
    private ColorDepth ColorDepth
    {
        get
        {
            return ImageList.ColorDepth;
        }
        set
        {
            ImageList.Images.Clear();
            ImageList.ColorDepth = value;
            Images.PopulateHandle();
        }
    }

    private bool ShouldSerializeColorDepth() => Images.Count == 0;

    /// <summary>
    /// Accessor method for the Images property on ImageList.
    /// We shadow this property at design time.
    /// </summary>
    private OriginalImageCollection Images
    {
        get
        {
            _originalImageCollection ??= new OriginalImageCollection(this);
            return _originalImageCollection;
        }
    }

    internal ImageList ImageList => (ImageList)Component;

    /// <summary>
    /// Accessor method for the ImageSize property on ImageList.
    /// We shadow this property at design time.
    /// </summary>
    private Size ImageSize
    {
        get
        {
            return ImageList.ImageSize;
        }
        set
        {
            ImageList.Images.Clear();
            ImageList.ImageSize = value;
            Images.PopulateHandle();
        }
    }

    private bool ShouldSerializeImageSize() => Images.Count == 0;

    private Color TransparentColor
    {
        get
        {
            return ImageList.TransparentColor;
        }
        set
        {
            ImageList.Images.Clear();
            ImageList.TransparentColor = value;
            Images.PopulateHandle();
        }
    }

    private bool ShouldSerializeTransparentColor() => !TransparentColor.Equals(Color.LightGray);

    /// <summary>
    /// Accessor method for the ImageStream property on ImageList.
    /// We shadow this property at design time.
    /// </summary>
    private ImageListStreamer? ImageStream
    {
        get
        {
            return ImageList.ImageStream;
        }
        set
        {
            ImageList.ImageStream = value;
            Images.ReloadFromImageList();
        }
    }

    /// <summary>
    /// Provides an opportunity for the designer to filter the properties.
    /// </summary>
    /// <param name="properties"></param>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        // Handle shadowed properties
        string[] shadowProps = new string[]
        {
            "ColorDepth",
            "ImageSize",
            "ImageStream",
            "TransparentColor"
        };

        for (int i = 0; i < shadowProps.Length; i++)
        {
            PropertyDescriptor? prop = (PropertyDescriptor?)properties[shadowProps[i]];
            if (prop is not null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ImageListDesigner), prop, Array.Empty<Attribute>());
            }
        }

        // replace this one seperately because it is of a different type (OriginalImageCollection) than
        // the original property (ImageCollection)
        PropertyDescriptor? imageProp = (PropertyDescriptor?)properties["Images"];
        if (imageProp is not null)
        {
            Attribute[] attrs = new Attribute[imageProp.Attributes.Count];
            imageProp.Attributes.CopyTo(attrs, 0);
            properties["Images"] = TypeDescriptor.CreateProperty(typeof(ImageListDesigner), "Images", typeof(OriginalImageCollection), attrs);
        }
    }

    public override DesignerActionListCollection ActionLists
    {
        get
        {
            _actionLists ??= new DesignerActionListCollection
            {
                new ImageListActionList(this)
            };
            return _actionLists;
        }
    }

    //  Shadow ImageList.Images to allow arbitrary handle recreation.
    internal class OriginalImageCollection : IList
    {
        private readonly ImageListDesigner _owner;
        private readonly List<object> _items = new List<object>();

        internal OriginalImageCollection(ImageListDesigner owner)
        {
            _owner = owner;
            // just in case it's got images
            ReloadFromImageList();
        }

        private void AssertInvariant()
        {
            Debug.Assert(_owner is not null, "OriginalImageCollection has no owner (ImageListDesigner)");
            Debug.Assert(_items is not null, "OriginalImageCollection has no list (ImageListDesigner)");
        }

        public int Count
        {
            get
            {
                AssertInvariant();
                return _items.Count;
            }
        }

        public bool IsReadOnly => false;

        bool IList.IsFixedSize => false;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageListImage this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(string.Format(SR.InvalidArgument,
                                                              "index",
                                                              index.ToString(CultureInfo.CurrentCulture)));
                return (ImageListImage)_items[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(string.Format(SR.InvalidArgument,
                                                              "index",
                                                              index.ToString(CultureInfo.CurrentCulture)));

                if (value is null)
                    throw new ArgumentException(string.Format(SR.InvalidArgument,
                                                              "value",
                                                              "null"));

                AssertInvariant();
                _items[index] = value;
                RecreateHandle();
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = value is ImageListImage image ? image : throw new ArgumentException();
        }

        public void SetKeyName(int index, string name)
        {
            this[index].Name = name;
            _owner.ImageList.Images.SetKeyName(index, name);
        }

        /// <summary>
        /// Add the given image to the ImageList.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(ImageListImage value)
        {
            _items.Add(value);
            if (value.Name is not null)
            {
                _owner.ImageList.Images.Add(value.Name, value.Image);
            }
            else
            {
                _owner.ImageList.Images.Add(value.Image);
            }

            return _items.IndexOf(value);
        }

        public void AddRange(ImageListImage[] values)
        {
            ArgumentNullException.ThrowIfNull(values);
            foreach (ImageListImage value in values)
            {
                if (value is not null)
                {
                    Add(value);
                }
            }
        }

        int IList.Add(object? value) => value is ImageListImage image ? Add(image) : throw new ArgumentException();

        // Called when reloading the form.  In this case, we have no "originals" list,
        // so we make one out of the image list.
        internal void ReloadFromImageList()
        {
            _items.Clear();
            StringCollection imageKeys = _owner.ImageList.Images.Keys;
            for (int i = 0; i < _owner.ImageList.Images.Count; i++)
            {
                _items.Add(new ImageListImage((Bitmap)_owner.ImageList.Images[i], imageKeys[i]));
            }
        }

        /// <summary>
        /// Remove all images and masks from the ImageList.
        /// </summary>
        public void Clear()
        {
            AssertInvariant();
            _items.Clear();
            _owner.ImageList.Images.Clear();
        }

        public bool Contains(ImageListImage value) => _items.Contains(value);

        bool IList.Contains(object? value) => value is ImageListImage image && Contains(image);

        public IEnumerator GetEnumerator() => _items.GetEnumerator();

        public int IndexOf(Image? value) => value is Image image ? _items.IndexOf(image) : -1;

        int IList.IndexOf(object? value) => value is Image image ? IndexOf(image) : -1;

        void IList.Insert(int index, object? value) => throw new NotSupportedException();

        internal void PopulateHandle()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                ImageListImage imageListImage = (ImageListImage)_items[i];
                _owner.ImageList.Images.Add(imageListImage.Name, imageListImage.Image);
            }
        }

        private void RecreateHandle()
        {
            _owner.ImageList.Images.Clear();
            PopulateHandle();
        }

        public void Remove(Image value)
        {
            AssertInvariant();
            _items.Remove(value);
            RecreateHandle();
        }

        void IList.Remove(object? value)
        {
            if (value is Image image)
            {
                Remove(image);
            }
        }

        public void RemoveAt(int index)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(index));
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(string.Format(SR.InvalidArgument,
                                                          "index",
                                                          index.ToString(CultureInfo.CurrentCulture)));

            AssertInvariant();
            _items.RemoveAt(index);
            RecreateHandle();
        }

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((ICollection)_items).SyncRoot;

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
