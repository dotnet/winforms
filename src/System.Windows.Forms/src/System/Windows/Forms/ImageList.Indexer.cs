// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {
        /// <summary>
        ///  This class is for classes that want to support both an ImageIndex
        ///  and ImageKey. We want to toggle between using keys or indexes.
        ///  Default is to use the integer index.
        /// </summary>
        internal class Indexer
        {
            internal const string DefaultKey = "";
            internal const int DefaultIndex = -1;
            private string _key = DefaultKey;
            private int _index = DefaultIndex;
            private bool _useIntegerIndex = true;

            public virtual ImageList? ImageList { get; set; }

            public virtual string Key
            {
                get => _key;
                set
                {
                    _index = DefaultIndex;
                    _key = value ?? DefaultKey;
                    _useIntegerIndex = false;
                }
            }

            public virtual int Index
            {
                get => _index;
                set
                {
                    _key = DefaultKey;
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

                    if (ImageList != null)
                    {
                        return ImageList.Images.IndexOfKey(Key);
                    }

                    return DefaultIndex;
                }
            }
        }
    }
}
