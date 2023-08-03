// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ListViewItem
{
    public partial class ListViewSubItem
    {
        [Serializable] // This type is participating in resx serialization scenarios.
        private class SubItemStyle
        {
            public Color backColor = Color.Empty; // Do NOT rename (binary serialization).
            public Font? font; // Do NOT rename (binary serialization).
            public Color foreColor = Color.Empty; // Do NOT rename (binary serialization).
        }
    }
}
