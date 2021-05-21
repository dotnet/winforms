// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal abstract partial class GridEntry
    {
        private class CacheItems
        {
            public string? lastLabel;
            public Font? lastLabelFont;
            public int lastLabelWidth;
            public bool lastShouldSerialize;
            public object? lastValue;
            public Font? lastValueFont;
            public string? lastValueString;
            public int lastValueTextWidth;
            public bool useCompatTextRendering;
            public bool useShouldSerialize;
            public bool useValueString;
        }
    }
}
