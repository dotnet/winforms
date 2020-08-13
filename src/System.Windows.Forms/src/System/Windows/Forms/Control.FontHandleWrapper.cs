// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        // Fonts can be a pain to track, so we wrap Hfonts in this class to get a Finalize method.
        internal sealed class FontHandleWrapper : IDisposable
        {
            private Gdi32.HFONT _handle;

            internal FontHandleWrapper(Font font)
            {
                _handle = (Gdi32.HFONT)font.ToHfont();
            }

            internal Gdi32.HFONT Handle
            {
                get
                {
                    Debug.Assert(!_handle.IsNull, "FontHandleWrapper disposed, but still being accessed");
                    return _handle;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!_handle.IsNull)
                {
                    Gdi32.DeleteObject(_handle);
                    _handle = default;
                }
            }

            ~FontHandleWrapper()
            {
                Dispose(false);
            }
        }
    }
}
