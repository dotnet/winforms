// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.Internal;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class wrapps a WindowsGraphics and is provided to be able to manipulate WindowsGraphics objects
    ///  created from a Graphics object in the same way as one created from any other IDeviceContext object,
    ///  which could be a custom one.
    ///  This class was designed to help TextRenderer determine how to create the underlying WindowsGraphics.
    /// </summary>
    internal sealed class WindowsGraphicsWrapper : IDisposable
    {
        private IDeviceContext _deviceContext;
        private WindowsGraphics _windowsGraphics;

        /// <summary>
        ///  Constructor that determines how to create the WindowsGraphics, there are three posible cases
        ///  for the IDeviceContext object type:
        ///  1. It is a Graphics object: In this case we need to check the TextFormatFlags to determine whether
        ///     we need to re-apply some of the Graphics properties to the WindowsGraphics, if so we call
        ///     WindowsGraphics.FromGraphics passing the corresponding flags. If not, we treat it as a custom
        ///     IDeviceContext (see below).
        ///  2. It is a WindowsGraphics object:
        ///     In this case we just need to use the wg directly and be careful not to dispose of it since
        ///     it is owned by the caller.
        ///  3. It is a custom IDeviceContext object:
        ///     In this case we create the WindowsGraphics from the native DC by calling IDeviceContext.GetHdc,
        ///     on dispose we need to call IDeviceContext.ReleaseHdc.
        /// </summary>
        public WindowsGraphicsWrapper(IDeviceContext deviceContext, TextFormatFlags flags)
        {
            if (deviceContext is Graphics)
            {
                ApplyGraphicsProperties properties = ApplyGraphicsProperties.None;

                if ((flags & TextFormatFlags.PreserveGraphicsClipping) != 0)
                {
                    properties |= ApplyGraphicsProperties.Clipping;
                }

                if ((flags & TextFormatFlags.PreserveGraphicsTranslateTransform) != 0)
                {
                    properties |= ApplyGraphicsProperties.TranslateTransform;
                }

                // Create the WindowsGraphics from the Grahpics object only if Graphics properties need
                // to be reapplied to the DC wrapped by the WindowsGraphics.
                if (properties != ApplyGraphicsProperties.None)
                {
                    try
                    {
                        _windowsGraphics = WindowsGraphics.FromGraphics(deviceContext as Graphics, properties);
                    }
                    catch
                    {
                        GC.SuppressFinalize(this);
                        throw;
                    }
                }
            }
            else
            {
                // If passed-in IDeviceContext object is a WindowsGraphics we can use it directly.
                _windowsGraphics = deviceContext as WindowsGraphics;

                if (_windowsGraphics != null)
                {
                    // In this case we cache the idc to compare it against the wg in the Dispose method to avoid
                    // disposing of the wg.
                    _deviceContext = deviceContext;
                }
            }

            if (_windowsGraphics == null)
            {
                // The IDeviceContext object is not a WindowsGraphics, or it is a custom IDeviceContext, or
                // it is a Graphics object but we did not need to re-apply Graphics propertiesto the hdc.
                // So create the WindowsGraphics from the hdc directly.
                // Cache the IDC so the hdc can be released ons dispose.
                try
                {
                    _deviceContext = deviceContext;
                    _windowsGraphics = WindowsGraphics.FromHdc(deviceContext.GetHdc());
                }
                catch
                {
                    SuppressFinalize();
                    deviceContext.ReleaseHdc();
                    throw;
                }
            }

            // Set text padding on the WindowsGraphics (if any).
            if ((flags & TextFormatFlags.LeftAndRightPadding) != 0)
            {
                _windowsGraphics.TextPadding = TextPaddingOptions.LeftAndRightPadding;
            }
            else if ((flags & TextFormatFlags.NoPadding) != 0)
            {
                _windowsGraphics.TextPadding = TextPaddingOptions.NoPadding;
            }
            // else wg.TextPadding = TextPaddingOptions.GlyphOverhangPadding - the default value.
        }

        public WindowsGraphics WindowsGraphics
        {
            get
            {
                Debug.Assert(_windowsGraphics != null, "WindowsGraphics is null.");
                return _windowsGraphics;
            }
        }

#if DEBUG
        // We only need the finalizer to track missed Dispose() calls.

        private readonly string _callStack = new StackTrace().ToString();

        ~WindowsGraphicsWrapper()
        {
            Debug.Fail($"{nameof(WindowsGraphicsWrapper)} was not disposed properly. Originating stack:\n{_callStack}");

            // We can't do anything with the fields when we're on the finalizer as they're all classes. If any of
            // them become structs they'll be a part of this instance and possible to clean up. Ideally we fix
            // the leaks and never come in on the finalizer.
            return;
        }
#endif

        [Conditional("DEBUG")]
        private void SuppressFinalize() => GC.SuppressFinalize(this);

        public void Dispose()
        {
            // Dispose() can throw. As such we'll suppress preemptively so we don't roll right back in and
            // potentially throw again on the finalizer thread.
            SuppressFinalize();

            // We need to dispose of the WindowsGraphics if it is created by this class only, if the IDeviceContext is
            // a WindowsGraphics object we must not dispose of it since it is owned by the caller.
            if (_windowsGraphics != null && _windowsGraphics != _deviceContext)
            {
                try
                {
                    _windowsGraphics.Dispose();
                }
                finally
                {
                    // Always make an attempt to release the HDC.
                    _deviceContext?.ReleaseHdc();
                }
            }

            // Clear our fields that have finalizers
            _deviceContext = null;
            _windowsGraphics = null;
        }
    }
}
