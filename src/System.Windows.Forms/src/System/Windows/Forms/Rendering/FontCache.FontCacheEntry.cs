// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal sealed partial class FontCache
{
    private sealed class FontCacheEntry : CacheEntry
    {
        public FontCacheEntry(Data data, bool tracked) : base(data, tracked) { }
        public override HFONT Object => Data.HFONT;
    }
}
