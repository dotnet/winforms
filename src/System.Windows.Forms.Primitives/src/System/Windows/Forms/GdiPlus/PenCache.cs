// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    internal sealed class PenCache : RefCountedCache<Pen, Color, Color>
    {
        public PenCache(int softLimit = 40, int hardLimit = 60) : base(softLimit, hardLimit) { }

        protected override CacheEntry CreateEntry(Color key, bool cached) => new PenCacheEntry(key, cached);
        protected override bool IsMatch(Color key, CacheEntry entry) => key == entry.Data;

        private sealed class PenCacheEntry : CacheEntry
        {
            private readonly Pen _pen;
            public PenCacheEntry(Color color, bool cached) : base(color, cached) => _pen = new Pen(color);
            public override Pen Object => _pen;
        }
    }
}
