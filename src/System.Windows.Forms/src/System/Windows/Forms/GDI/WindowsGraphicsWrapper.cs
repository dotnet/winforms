// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        IDeviceContext idc;
        WindowsGraphics wg;

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
        public WindowsGraphicsWrapper(IDeviceContext idc, TextFormatFlags flags)
        {
            if (idc is Graphics)
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
                    wg = WindowsGraphics.FromGraphics(idc as Graphics, properties);
                }
            }
            else
            {
                // If passed-in IDeviceContext object is a WindowsGraphics we can use it directly.
                wg = idc as WindowsGraphics;

                if (wg != null)
                {
                    // In this case we cache the idc to compare it against the wg in the Dispose method to avoid
                    // disposing of the wg.
                    this.idc = idc;
                }
            }

            if (wg == null)
            {
                // The IDeviceContext object is not a WindowsGraphics, or it is a custom IDeviceContext, or
                // it is a Graphics object but we did not need to re-apply Graphics propertiesto the hdc.
                // So create the WindowsGraphics from the hdc directly.
                // Cache the IDC so the hdc can be released on dispose.
                this.idc = idc;
                wg = WindowsGraphics.FromHdc(idc.GetHdc());
            }

            // Set text padding on the WindowsGraphics (if any).
            if ((flags & TextFormatFlags.LeftAndRightPadding) != 0)
            {
                wg.TextPadding = TextPaddingOptions.LeftAndRightPadding;
            }
            else if ((flags & TextFormatFlags.NoPadding) != 0)
            {
                wg.TextPadding = TextPaddingOptions.NoPadding;
            }
            // else wg.TextPadding = TextPaddingOptions.GlyphOverhangPadding - the default value.
        }

        public WindowsGraphics WindowsGraphics
        {
            get
            {
                Debug.Assert(wg != null, "WindowsGraphics is null.");
                return wg;
            }
        }

        ~WindowsGraphicsWrapper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            Debug.Assert(disposing, "We should always dispose of this guy and not let GC do it for us!");

            if (wg != null)
            {
                // We need to dispose of the WindowsGraphics if it is created by this class only, if the IDeviceContext is
                // a WindowsGraphics object we must not dispose of it since it is owned by the caller.
                if (wg != idc)
                {
                    // resets the hdc and disposes of the internal Graphics (if inititialized from one) which releases the hdc.
                    wg.Dispose();

                    if (idc != null) // not initialized from a Graphics idc.
                    {
                        idc.ReleaseHdc();
                    }
                }

                idc = null;
                wg = null;
            }
        }
    }
}
