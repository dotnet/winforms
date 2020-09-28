// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.ButtonInternal
{
    internal abstract partial class ButtonBaseAdapter
    {
        internal class LayoutData
        {
            internal Rectangle client;
            internal Rectangle face;
            internal Rectangle checkArea;
            internal Rectangle checkBounds;
            internal Rectangle textBounds;
            internal Rectangle field;
            internal Rectangle focus;
            internal Rectangle imageBounds;
            internal Point imageStart; // FOR EVERETT COMPATIBILITY - DO NOT CHANGE
            internal LayoutOptions options;

            internal LayoutData(LayoutOptions options)
            {
                Debug.Assert(options != null, "must have options");
                this.options = options;
            }
        }
    }
}
