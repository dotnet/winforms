// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Label
{
    /// <summary>
    ///  Override ImageList.Indexer to support Label's ImageList semantics.
    /// </summary>
    internal class LabelImageIndexer : ImageList.Indexer
    {
        private readonly Label _owner;
        private bool _useIntegerIndex = true;

        public LabelImageIndexer(Label owner)
        {
            Debug.Assert(owner is not null, $"{nameof(owner)} should not be null.");
            _owner = owner;
        }

        public override ImageList? ImageList
        {
            get { return _owner.ImageList; }
            set { Debug.Assert(false, "Setting the image list in this class is not supported"); }
        }

        [AllowNull]
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
                if (ImageList is null)
                {
                    return -1;
                }

                if (_useIntegerIndex)
                {
                    // The behavior of label is to return the last item in the Images collection
                    // if the index is currently set higher than the count.
                    return (Index < ImageList.Images.Count) ? Index : ImageList.Images.Count - 1;
                }

                return ImageList.Images.IndexOfKey(Key);
            }
        }
    }
}
