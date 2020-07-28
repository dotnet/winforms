// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Thread safe cache of <see cref="Gdi32.HFONT"/> objects created from <see cref="Font"/> objects.
    /// </summary>
    /// <remarks>
    ///  This adds a slight managed memory overhead to creating the <see cref="Gdi32.HFONT"/>, but saves 56 bytes and
    ///  98%+ of the time on each cache request that hits (taking less than 1 us as opposed to 50-100+ us).
    ///
    ///  This cache is optimized for retrieval speed and limiting the number of unused GDI handles we're caching while
    ///  hopefully handling the majority of application use cases. There is a limit of 65K GDI handles system wide and
    ///  10K (default) per process.
    /// </remarks>
    internal sealed partial class FontCache : RefCountedCache<Gdi32.HFONT, FontCache.Data, (Font Font, Gdi32.QUALITY Quality)>
    {
        private readonly object _lock = new object();

        /// <summary>
        ///  Create a <see cref="FontCache"/> with the specified collection limits.
        /// </summary>
        public FontCache(int softLimit = 20, int hardLimit = 40) : base(softLimit, hardLimit) { }

        /// <summary>
        ///  Gets a ref-counting scope containing the <see cref="Gdi32.HFONT"/> that matches the specified
        ///  <paramref name="font"/> and <paramref name="quality"/>. The scope MUST be disposed to release the ref
        ///  count accurately. Use the result in a using statement to avoid leaking fonts.
        /// </summary>
        public Scope GetEntry(Font font, Gdi32.QUALITY quality = Gdi32.QUALITY.DEFAULT) => GetEntry((font, quality));

        public override Scope GetEntry((Font Font, Gdi32.QUALITY Quality) key)
        {
            lock (_lock)
            {
                return base.GetEntry(key);
            }
        }

        protected override CacheEntry CreateEntry((Font Font, Gdi32.QUALITY Quality) key, bool cached)
            => new FontCacheEntry(new Data(key.Font, key.Quality), cached);

        protected override bool IsMatch((Font Font, Gdi32.QUALITY Quality) key, CacheEntry entry)
            => entry.Data.Font.TryGetTarget(out Font? currentFont)
                && key.Font == currentFont
                && key.Quality == entry.Data.Quality;

        private sealed class FontCacheEntry : CacheEntry
        {
            public FontCacheEntry(Data data, bool tracked) : base(data, tracked) { }
            public override Gdi32.HFONT Object => Data.HFONT;
        }
    }
}
