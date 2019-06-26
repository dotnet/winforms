// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        // TODO: why serializable? 

        [Serializable]
        private class SubItemStyle
        {
            // types cannot be changed to conform to naming standards due to Serialization
#pragma warning disable IDE1006

            public Color backColor = Color.Empty;
            public Color foreColor = Color.Empty;
            public Font font;

#pragma warning restore IDE1006
        }       
    }
}
