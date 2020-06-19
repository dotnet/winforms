// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using static Interop;

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {
        // Everything other than set_All, Add, and Clear will force handle creation.
        [Editor("System.Windows.Forms.Design.ImageCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        public sealed class ImageCollection : IList
        {
            private readonly ImageList _owner;
            private readonly ArrayList _imageInfoCollection = new ArrayList();

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            private int _lastAccessedIndex = -1;

            /// <summary>
            ///  Returns the keys in the image list - images without keys return String.Empty.
            /// </summary>
            public StringCollection Keys
            {
                get
                {
                    // pass back a copy of the current state.
                    StringCollection keysCollection = new StringCollection();

                    for (int i = 0; i < _imageInfoCollection.Count; i++)
                    {
                        if ((_imageInfoCollection[i] is ImageInfo image) && (image.Name != null) && (image.Name.Length != 0))
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
                _owner = owner;
            }

            internal void ResetKeys()
            {
                _imageInfoCollection?.Clear();

                for (int i = 0; i < Count; i++)
                {
                    _imageInfoCollection.Add(new ImageInfo());
                }
            }

            [Conditional("DEBUG")]
            private void AssertInvariant()
            {
                Debug.Assert(_owner != null, "ImageCollection has no owner (ImageList)");
                Debug.Assert((_owner._originals is null) == (_owner.HandleCreated), " Either we should have the original images, or the handle should be created");
            }

            [Browsable(false)]
            public int Count
            {
                get
                {
                    Debug.Assert(_owner != null, "ImageCollection has no owner (ImageList)");

                    if (_owner.HandleCreated)
                    {
                        return ComCtl32.ImageList.GetImageCount(_owner);
                    }
                    else
                    {
                        int count = 0;
                        foreach (Original original in _owner._originals)
                        {
                            if (original != null)
                            {
                                count += original._nImages;
                            }
                        }
                        return count;
                    }
                }
            }

            object ICollection.SyncRoot => this;

            bool ICollection.IsSynchronized => false;

            bool IList.IsFixedSize => false;

            public bool IsReadOnly => false;

            /// <summary>
            ///  Determines if the ImageList has any images, without forcing a handle creation.
            /// </summary>
            public bool Empty => Count == 0;

            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Image this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return _owner.GetBitmap(index);
                }
                set
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    if (value is null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    if (!(value is Bitmap bitmap))
                    {
                        throw new ArgumentException(SR.ImageListBitmap);
                    }

                    AssertInvariant();
                    bool ownsImage = false;
                    if (_owner.UseTransparentColor && bitmap.RawFormat.Guid != ImageFormat.Icon.Guid)
                    {
                        // Since there's no ImageList_ReplaceMasked, we need to generate
                        // a transparent bitmap
                        Bitmap source = bitmap;
                        bitmap = (Bitmap)bitmap.Clone();
                        bitmap.MakeTransparent(_owner.TransparentColor);
                        ownsImage = true;
                    }

                    try
                    {
                        IntPtr hMask = ControlPaint.CreateHBitmapTransparencyMask(bitmap);
                        IntPtr hBitmap = ControlPaint.CreateHBitmapColorMask(bitmap, hMask);
                        bool ok;
                        try
                        {
                            ok = ComCtl32.ImageList.Replace(_owner, index, hBitmap, hMask).IsTrue();
                        }
                        finally
                        {
                            Gdi32.DeleteObject(hBitmap);
                            Gdi32.DeleteObject(hMask);
                        }

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
                get => this[index];
                set
                {
                    if (!(value is Image image))
                    {
                        throw new ArgumentException(SR.ImageListBadImage, nameof(value));
                    }

                    this[index] = (Image)value;
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
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (!IsValidIndex(index))
                    {
                        return null;
                    }

                    return this[index];
                }
            }

            /// <summary>
            ///  Adds an image to the end of the image list with a key accessor.
            /// </summary>
            public void Add(string key, Image image)
            {
                Debug.Assert((Count == _imageInfoCollection.Count), "The count of these two collections should be equal.");

                // Store off the name.
                var imageInfo = new ImageInfo
                {
                    Name = key
                };

                // Add the image to the IList
                var original = new Original(image, OriginalOptions.Default);
                Add(original, imageInfo);
            }

            /// <summary>
            ///  Adds an icon to the end of the image list with a key accessor.
            /// </summary>
            public void Add(string key, Icon icon)
            {
                Debug.Assert((Count == _imageInfoCollection.Count), "The count of these two collections should be equal.");

                // Store off the name.
                var imageInfo = new ImageInfo
                {
                    Name = key
                };

                // Add the image to the IList
                var original = new Original(icon, OriginalOptions.Default);
                Add(original, imageInfo);
            }

            int IList.Add(object value)
            {
                if (!(value is Image image))
                {
                    throw new ArgumentException(SR.ImageListBadImage, nameof(value));
                }

                Add((Image)value);
                return Count - 1;
            }

            public void Add(Icon value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                // Don't clone it now is a breaking change, so we have to keep track of this specific icon and dispose that
                Add(new Original(value.Clone(), OriginalOptions.OwnsImage), null);
            }

            /// <summary>
            ///  Add the given image to the ImageList.
            /// </summary>
            public void Add(Image value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                var original = new Original(value, OriginalOptions.Default);
                Add(original, null);
            }

            /// <summary>
            ///  Add the given image to the ImageList, using the given color
            ///  to generate the mask. The number of images to add is inferred from
            ///  the width of the given image.
            /// </summary>
            public int Add(Image value, Color transparentColor)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                var original = new Original(value, OriginalOptions.CustomTransparentColor, transparentColor);
                return Add(original, null);
            }

            private int Add(Original original, ImageInfo imageInfo)
            {
                if (original is null || original._image is null)
                {
                    throw new ArgumentNullException(nameof(original));
                }

                int index = -1;

                AssertInvariant();

                if (original._image is Bitmap)
                {
                    if (_owner._originals != null)
                    {
                        index = _owner._originals.Add(original);
                    }

                    if (_owner.HandleCreated)
                    {
                        Bitmap bitmapValue = _owner.CreateBitmap(original, out bool ownsBitmap);
                        index = _owner.AddToHandle(bitmapValue);
                        if (ownsBitmap)
                        {
                            bitmapValue.Dispose();
                        }
                    }
                }
                else if (original._image is Icon)
                {
                    if (_owner._originals != null)
                    {
                        index = _owner._originals.Add(original);
                    }
                    if (_owner.HandleCreated)
                    {
                        index = _owner.AddIconToHandle(original, (Icon)original._image);
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
                if ((original._options & OriginalOptions.ImageStrip) != 0)
                {
                    for (int i = 0; i < original._nImages; i++)
                    {
                        _imageInfoCollection.Add(new ImageInfo());
                    }
                }
                else
                {
                    imageInfo ??= new ImageInfo();
                    _imageInfoCollection.Add(imageInfo);
                }

                if (!_owner._inAddRange)
                {
                    _owner.OnChangeHandle(EventArgs.Empty);
                }

                return index;
            }

            public void AddRange(Image[] images)
            {
                if (images is null)
                {
                    throw new ArgumentNullException(nameof(images));
                }

                _owner._inAddRange = true;
                foreach (Image image in images)
                {
                    Add(image);
                }

                _owner._inAddRange = false;
                _owner.OnChangeHandle(EventArgs.Empty);
            }

            /// <summary>
            ///  Add an image strip the given image to the ImageList. A strip is a single Image
            ///  which is treated as multiple images arranged side-by-side.
            /// </summary>
            public int AddStrip(Image value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                // strip width must be a positive multiple of image list width
                if (value.Width == 0 || (value.Width % _owner.ImageSize.Width) != 0)
                {
                    throw new ArgumentException(SR.ImageListStripBadWidth, nameof(value));
                }

                if (value.Height != _owner.ImageSize.Height)
                {
                    throw new ArgumentException(SR.ImageListImageTooShort, nameof(value));
                }

                int nImages = value.Width / _owner.ImageSize.Width;

                var original = new Original(value, OriginalOptions.ImageStrip, nImages);

                return Add(original, null);
            }

            /// <summary>
            ///  Remove all images and masks from the ImageList.
            /// </summary>
            public void Clear()
            {
                AssertInvariant();
                if (_owner._originals != null)
                {
                    _owner._originals.Clear();
                }

                _imageInfoCollection.Clear();

                if (_owner.HandleCreated)
                {
                    ComCtl32.ImageList.Remove(_owner, -1);
                }

                _owner.OnChangeHandle(EventArgs.Empty);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool Contains(Image image) => throw new NotSupportedException();

            bool IList.Contains(object value)
            {
                if (!(value is Image image))
                {
                    return false;
                }

                return Contains(image);
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public bool ContainsKey(string key) => IsValidIndex(IndexOfKey(key));

            [EditorBrowsable(EditorBrowsableState.Never)]
            public int IndexOf(Image image) => throw new NotSupportedException();

            int IList.IndexOf(object value)
            {
                if (!(value is Image image))
                {
                    return -1;
                }

                return IndexOf(image);
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase,
            ///  if found; otherwise, -1.
            /// </summary>
            public int IndexOfKey(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    // We dont support empty or null keys.
                    return -1;
                }

                // Check the last cached item
                if (IsValidIndex(_lastAccessedIndex))
                {
                    if ((_imageInfoCollection[_lastAccessedIndex] != null) &&
                        (WindowsFormsUtils.SafeCompareStrings(((ImageInfo)_imageInfoCollection[_lastAccessedIndex]).Name, key, ignoreCase: true)))
                    {
                        return _lastAccessedIndex;
                    }
                }

                // Search for the item
                for (int i = 0; i < Count; i++)
                {
                    if ((_imageInfoCollection[i] != null) &&
                            (WindowsFormsUtils.SafeCompareStrings(((ImageInfo)_imageInfoCollection[i]).Name, key, ignoreCase: true)))
                    {
                        _lastAccessedIndex = i;
                        return i;
                    }
                }

                // We didn't find it. Invalidate the last accessed index and return -1.
                _lastAccessedIndex = -1;
                return -1;
            }

            void IList.Insert(int index, object value) => throw new NotSupportedException();

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index) => index >= 0 && index < Count;

            void ICollection.CopyTo(Array dest, int index)
            {
                AssertInvariant();
                for (int i = 0; i < Count; ++i)
                {
                    dest.SetValue(_owner.GetBitmap(i), index++);
                }
            }

            public IEnumerator GetEnumerator()
            {
                // Forces handle creation

                AssertInvariant();
                Image[] images = new Image[Count];
                for (int i = 0; i < images.Length; ++i)
                {
                    images[i] = _owner.GetBitmap(i);
                }

                return images.GetEnumerator();
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Remove(Image image) => throw new NotSupportedException();

            void IList.Remove(object value)
            {
                if (value is Image image)
                {
                    Remove(image);
                    _owner.OnChangeHandle(EventArgs.Empty);
                }
            }

            public void RemoveAt(int index)
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                AssertInvariant();
                bool ok = ComCtl32.ImageList.Remove(_owner, index).IsTrue();
                if (!ok)
                {
                    throw new InvalidOperationException(SR.ImageListRemoveFailed);
                }

                if ((_imageInfoCollection != null) && (index >= 0 && index < _imageInfoCollection.Count))
                {
                    _imageInfoCollection.RemoveAt(index);
                    _owner.OnChangeHandle(EventArgs.Empty);
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
                    throw new IndexOutOfRangeException();
                }

                if (_imageInfoCollection[index] is null)
                {
                    _imageInfoCollection[index] = new ImageInfo();
                }

                ((ImageInfo)_imageInfoCollection[index]).Name = name;
            }

            internal class ImageInfo
            {
                public string Name { get; set; }
            }
        }
    }
}
