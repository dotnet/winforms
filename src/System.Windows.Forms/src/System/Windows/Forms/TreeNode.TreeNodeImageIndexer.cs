// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class TreeNode
    {
        // We need a special way to defer to the TreeView's image
        // list for indexing purposes.
        internal partial class TreeNodeImageIndexer : ImageList.Indexer
        {
            private readonly TreeNode owner;

            private readonly ImageListType imageListType;

            public TreeNodeImageIndexer(TreeNode node, ImageListType imageListType)
            {
                owner = node;
                this.imageListType = imageListType;
            }

            public override ImageList ImageList
            {
                get
                {
                    if (owner.TreeView is not null)
                    {
                        if (imageListType == ImageListType.State)
                        {
                            return owner.TreeView.StateImageList;
                        }
                        else
                        {
                            return owner.TreeView.ImageList;
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
}
