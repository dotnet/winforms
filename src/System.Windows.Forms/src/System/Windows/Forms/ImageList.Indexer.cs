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
    }
}
