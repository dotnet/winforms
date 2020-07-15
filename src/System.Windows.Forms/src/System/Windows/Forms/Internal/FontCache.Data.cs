// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Threading;
using static Interop;

namespace System.Windows.Forms
{
    internal sealed partial class FontCache
    {
        internal class Data
        {
            public WeakReference<Font> Font { get; }
            public Gdi32.HFONT HFONT { get; }
            public Gdi32.QUALITY Quality { get; }

            private int _refCount;
            private int? _tmHeight;

            public Data(Font font, Gdi32.QUALITY quality)
            {
                Font = new WeakReference<Font>(font);
                Quality = quality;
                HFONT = FromFont(font, quality);
            }

            public int RefCount => _refCount;

            public int Height
            {
                get
                {
                    if (!_tmHeight.HasValue)
                    {
                        using var screenDC = GdiCache.GetScreenHdc();
                        Gdi32.HDC hdc = screenDC.HDC;
                        using var fontSelection = new Gdi32.SelectObjectScope(hdc, HFONT);
                        Debug.Assert(Gdi32.GetMapMode(hdc) == Gdi32.MM.TEXT);

                        Gdi32.TEXTMETRICW tm = default;
                        Gdi32.GetTextMetricsW(hdc, ref tm);
                        _tmHeight = tm.tmHeight;
                    }

                    return _tmHeight.Value;
                }
            }

            public void AddRef() => Interlocked.Increment(ref _refCount);
            public void RemoveRef() => Interlocked.Decrement(ref _refCount);
        }
    }
}
