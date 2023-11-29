// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal;

internal abstract partial class GridEntry
{
    private class CacheItems
    {
        public string? LastLabel;
        public Font? LastLabelFont;
        public int LastLabelWidth;
        public bool LastShouldSerialize;
        public object? LastValue;
        public Font? LastValueFont;
        public string? LastValueString;
        public int LastValueTextWidth;
        public bool UseCompatTextRendering;
        public bool UseShouldSerialize;
        public bool UseValueString;
    }
}
