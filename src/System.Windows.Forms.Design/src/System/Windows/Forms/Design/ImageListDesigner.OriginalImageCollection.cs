// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal partial class ImageListDesigner
{
    // Shadow ImageList.Images to allow arbitrary handle recreation.
    [Editor($"System.Windows.Forms.Design.ImageCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    internal class OriginalImageCollection : IList
    {
        private readonly ImageListDesigner _owner;
        private readonly List<object> _items = [];

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
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

                return (ImageListImage)_items[index];
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
                ArgumentNullException.ThrowIfNull(value);

                AssertInvariant();
                _items[index] = value;
                RecreateHandle();
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = value is ImageListImage image ? image : throw new ArgumentException(null, nameof(index));
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

        int IList.Add(object? value) => value is ImageListImage image ? Add(image) : throw new ArgumentException(null, nameof(value));

        // Called when reloading the form. In this case, we have no "originals" list,
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

        public int IndexOf(ImageListImage? value) => value is ImageListImage imageListImage ? _items.IndexOf(imageListImage) : -1;

        int IList.IndexOf(object? value) => value is ImageListImage image ? IndexOf(image) : -1;

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

        public void Remove(ImageListImage value)
        {
            AssertInvariant();
            _items.Remove(value);
            RecreateHandle();
        }

        void IList.Remove(object? value)
        {
            if (value is ImageListImage image)
            {
                Remove(image);
            }
        }

        public void RemoveAt(int index)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(index));
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

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
