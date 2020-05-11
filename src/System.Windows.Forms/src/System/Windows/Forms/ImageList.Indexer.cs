// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {
        // This class is for classes that want to support both an ImageIndex
        // and ImageKey.  We want to toggle between using keys or indexes.
        // Default is to use the integer index.
        internal class Indexer
        {
            private string _key = string.Empty;
            private int _index = -1;
            private bool _useIntegerIndex = true;

            public virtual ImageList ImageList { get; set; }

            public virtual string Key
            {
                get { return _key; }
                set
                {
                    _index = -1;
                    _key = (value ?? string.Empty);
                    _useIntegerIndex = false;
                }
            }

            public virtual int Index
            {
                get { return _index; }
                set
                {
                    _key = string.Empty;
                    _index = value;
                    _useIntegerIndex = true;
                }
            }

            public virtual int ActualIndex
            {
                get
                {
                    if (_useIntegerIndex)
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
    }
}
