// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class Control
{
    // Fonts can be a pain to track, so we wrap Hfonts in this class to get a Finalize method.
    internal sealed class FontHandleWrapper : IDisposable
    {
        private HFONT _handle;

        internal FontHandleWrapper(Font font)
        {
            _handle = (HFONT)font.ToHfont();
        }

        internal HFONT Handle
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
                PInvoke.DeleteObject(_handle);
                _handle = default;
            }
        }

        ~FontHandleWrapper()
        {
            Dispose(false);
        }
    }
}
