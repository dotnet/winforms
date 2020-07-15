// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms
{
    internal sealed partial class FontCache
    {
        internal readonly ref struct FontScope
        {
            // We create default font scopes when we want to draw text using the default font, so this can end up null.
            private readonly Data _data;

            internal FontScope(Data data)
            {
                Debug.Assert(data != null);
                _data = data;
                _data.AddRef();
            }

            public Gdi32.HFONT HFONT => _data?.HFONT ?? default;
            public int FontHeight => _data?.Height ?? default;

            public static implicit operator Gdi32.HFONT(in FontScope scope) => scope.HFONT;
            public static implicit operator Gdi32.HGDIOBJ(in FontScope scope) => scope.HFONT;

            public void Dispose()
            {
                _data?.RemoveRef();
            }
        }
    }
}
