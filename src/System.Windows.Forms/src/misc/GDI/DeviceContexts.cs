// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WGCM_TEST_SUITE // Enable tracking when built for the test suites.
#define TRACK_HDC
#define GDI_FONT_CACHE_TRACK
#endif

#if WINFORMS_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Internal;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading;
    using Microsoft.Win32;

    /// <devdoc>
    ///     Keeps a cache of some graphics primitives.
    ///     Created to improve performance of TextRenderer.MeasureText methods that don't receive a WindowsGraphics.
    ///     This class mantains a cache of MRU WindowsFont objects in the process.
    /// </devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    static class DeviceContexts
    {
        [ThreadStatic] 
        private static ClientUtils.WeakRefCollection activeDeviceContexts;

        /// <devdoc>
        /// WindowsGraphicsCacheManager needs to track DeviceContext
        /// objects so it can ask them if a font is in use before they 
        /// it's deleted.  
        internal static void AddDeviceContext(DeviceContext dc) {
            if (activeDeviceContexts == null) {
                activeDeviceContexts = new ClientUtils.WeakRefCollection();
                activeDeviceContexts.RefCheckThreshold = 20;
            }

            if (!activeDeviceContexts.Contains(dc)) {
                dc.Disposing += new EventHandler(OnDcDisposing);
                activeDeviceContexts.Add(dc);
            }
        }

        private static void OnDcDisposing(object sender, EventArgs e)
        {
            DeviceContext dc = sender as DeviceContext;

            if (dc != null)
            {
                dc.Disposing -= new EventHandler(OnDcDisposing);
                RemoveDeviceContext(dc);
            }
        }
        
        internal static void RemoveDeviceContext(DeviceContext dc) {
            if (activeDeviceContexts == null) {
                return;
            }
            activeDeviceContexts.RemoveByHashCode(dc);
        }

 #if !DRAWING_NAMESPACE
        internal static bool IsFontInUse(WindowsFont wf) {
            if (wf == null) {
                return false;
            }
            
            for (int i = 0; i < activeDeviceContexts.Count; i++) {
                DeviceContext dc = activeDeviceContexts[i] as DeviceContext;
                if (dc != null && (dc.ActiveFont == wf || dc.IsFontOnContextStack(wf))) {
                    return true;
                }
            }

            return false;
        }
 #endif
 
    }
}
