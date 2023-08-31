// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class TreeNode
{
    // We need a special way to defer to the TreeView's image
    // list for indexing purposes.
    internal partial class TreeNodeImageIndexer : ImageList.Indexer
    {
        private readonly TreeNode _owner;

        private readonly ImageListType _imageListType;

        public TreeNodeImageIndexer(TreeNode node, ImageListType imageListType)
        {
            Debug.Assert(node is not null, $"{nameof(node)} should not be null.");
            _owner = node;
            _imageListType = imageListType;
        }

        public override ImageList? ImageList
        {
            get
            {
                if (_owner.TreeView is not null)
                {
                    if (_imageListType == ImageListType.State)
                    {
                        return _owner.TreeView.StateImageList;
                    }
                    else
                    {
                        return _owner.TreeView.ImageList;
                    }
                }
                else
                {
                    return null;
                }
            }
            set { Debug.Assert(false, "We should never set the image list"); }
        }
    }
}
