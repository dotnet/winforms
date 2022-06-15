// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    internal partial class PropertyStore
    {
        private sealed class ColorWrapper
        {
            public Color Color;

            public ColorWrapper(Color color)
            {
                Color = color;
            }
        }
    }
}
