// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class FontCacheTests
    {
        [Fact(Skip = "Run manually, takes a few minutes and is very resource intensive.")]
        public void StressTest()
        {
            Font[] fonts = new Font[10];
            for (int i = 0; i < 10; i++)
            {
                fonts[i] = new Font("Arial", i + 6);
            }

            FontCache cache = new FontCache(softLimit: 4, hardLimit: 8);
            Random random = new Random();
            try
            {
                for (int i = 0; i < 10_000; i++)
                {
                    Thread.Sleep(random.Next(5));
                    Task.Run(() =>
                    {
                        using var hfont = cache.GetEntry(
                            fonts[random.Next(10)],
                            (Gdi32.QUALITY)random.Next(7));

                        Assert.False(hfont.Object.IsNull);
                        Thread.Sleep(random.Next(10));
                    });
                }
            }
            finally
            {
                cache.Dispose();
                for (int i = 0; i < 10; i++)
                {
                    fonts[i]?.Dispose();
                }
            }
        }
    }
}
