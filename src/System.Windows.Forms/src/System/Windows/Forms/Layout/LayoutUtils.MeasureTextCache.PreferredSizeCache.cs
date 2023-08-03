// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Layout;

internal partial class LayoutUtils
{
    private struct PreferredSizeCache
    {
        public Size ConstrainingSize;

        public Size PreferredSize;

        public PreferredSizeCache(Size constrainingSize, Size preferredSize)
        {
            ConstrainingSize = constrainingSize;
            PreferredSize = preferredSize;
        }
    }
}
