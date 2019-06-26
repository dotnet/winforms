// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        // We need a special way to defer to the ListView's image
        // list for indexing purposes.
        internal class ListViewItemImageIndexer : ImageList.Indexer
        {
            private readonly ListViewItem _owner;


            public ListViewItemImageIndexer(ListViewItem item)
            {
                _owner = item;
            }


            public override ImageList ImageList
            {
                get => _owner?.ImageList;
                set => Debug.Fail("We should never set the image list");
            }
        }
    }
}
