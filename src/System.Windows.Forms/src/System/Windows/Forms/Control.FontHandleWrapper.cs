// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        // Fonts can be a pain to track, so we wrap Hfonts in this class to get a Finalize method.
        internal sealed class FontHandleWrapper : MarshalByRefObject, IDisposable
        {
#if DEBUG
            private readonly string _stackOnCreate = null;
            private string _stackOnDispose = null;
            private bool _finalizing = false;
#endif
            private IntPtr _handle;

            internal FontHandleWrapper(Font font)
            {
#if DEBUG
                if (CompModSwitches.LifetimeTracing.Enabled)
                {
                    _stackOnCreate = new StackTrace().ToString();
                }
#endif
                _handle = font.ToHfont();
            }

            internal IntPtr Handle
            {
                get
                {
                    Debug.Assert(_handle != IntPtr.Zero, "FontHandleWrapper disposed, but still being accessed");
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
#if DEBUG
                Debug.Assert(_finalizing || this != s_defaultFontHandleWrapper, "Don't dispose the defaultFontHandleWrapper");
#endif

                if (_handle != IntPtr.Zero)
                {
#if DEBUG
                    if (CompModSwitches.LifetimeTracing.Enabled)
                    {
                        _stackOnDispose = new StackTrace().ToString();
                    }
#endif
                    Gdi32.DeleteObject(_handle);
                    _handle = IntPtr.Zero;
                }
            }

            ~FontHandleWrapper()
            {
#if DEBUG
                _finalizing = true;
#endif
                Dispose(false);
            }
        }
    }
}
