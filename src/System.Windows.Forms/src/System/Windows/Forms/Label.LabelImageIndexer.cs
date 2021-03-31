// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class Label
    {
        /// <summary>
        ///  Override ImageList.Indexer to support Label's ImageList semantics.
        /// </summary>
        internal class LabelImageIndexer : ImageList.Indexer
        {
            private readonly Label _owner;
            private bool _useIntegerIndex = true;

            public LabelImageIndexer(Label owner) => _owner = owner;

            public override ImageList ImageList
            {
                get { return _owner?.ImageList; }
                set { Debug.Assert(false, "Setting the image list in this class is not supported"); }
            }

            public override string Key
            {
                get => base.Key;
                set
                {
                    base.Key = value;
                    _useIntegerIndex = false;
                }
            }

            public override int Index
            {
                get => base.Index;
                set
                {
                    base.Index = value;
                    _useIntegerIndex = true;
                }
            }

            public override int ActualIndex
            {
                get
                {
                    if (_useIntegerIndex)
                    {
                        // The behavior of label is to return the last item in the Images collection
                        // if the index is currently set higher than the count.
                        return (Index < ImageList.Images.Count) ? Index : ImageList.Images.Count - 1;
                    }
                    else if (ImageList is not null)
                    {
                        return ImageList.Images.IndexOfKey(Key);
                    }

                    return -1;
                }
            }
        }
    }
}
