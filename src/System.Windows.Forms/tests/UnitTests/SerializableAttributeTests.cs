// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows;

namespace System.Windows.Forms.Tests.Serialization;

// NB: doesn't require thread affinity
public class SerializableAttributeTests
{
    [Fact]
    public void EnsureSerializableAttribute()
    {
        BinarySerialization.EnsureSerializableAttribute(
            typeof(ListViewItem).Assembly,
            new HashSet<string>
            {
                // This is needed for OLE JSON serialization support
                { typeof(JsonData<>).FullName },
                // This state is serialized to communicate to the native control
                { typeof(AxHost.State).FullName },
                // Following classes are participating in resx serialization scenarios.
                { typeof(ImageListStreamer).FullName },
                { typeof(LinkArea).FullName },
                { typeof(ListViewGroup).FullName },
                { typeof(ListViewItem).FullName },
                { typeof(ListViewItem.ListViewSubItem).FullName },
                { "System.Windows.Forms.ListViewItem+ListViewSubItem+SubItemStyle" },  // Private type.
                { typeof(OwnerDrawPropertyBag).FullName },
                { typeof(Padding).FullName },
                { typeof(TreeNode).FullName },
                { typeof(TableLayoutSettings).FullName },
            });
    }
}
