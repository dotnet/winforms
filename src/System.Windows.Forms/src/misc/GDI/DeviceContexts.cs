// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Keeps a cache of some graphics primitives.
    ///  Created to improve performance of TextRenderer.MeasureText methods that don't receive a WindowsGraphics.
    ///  This class mantains a cache of MRU WindowsFont objects in the process.
    /// </summary>
    internal static class DeviceContexts
    {
        [ThreadStatic]
        private static ClientUtils.WeakRefCollection activeDeviceContexts;

        /// <summary>
        ///  WindowsGraphicsCacheManager needs to track DeviceContext
        ///  objects so it can ask them if a font is in use before they
        ///  it's deleted.
        internal static void AddDeviceContext(DeviceContext dc)
        {
            if (activeDeviceContexts == null)
            {
                activeDeviceContexts = new ClientUtils.WeakRefCollection
                {
                    RefCheckThreshold = 20
                };
            }

            if (!activeDeviceContexts.Contains(dc))
            {
                dc.Disposing += new EventHandler(OnDcDisposing);
                activeDeviceContexts.Add(dc);
            }
        }

        private static void OnDcDisposing(object sender, EventArgs e)
        {
            if (sender is DeviceContext dc)
            {
                dc.Disposing -= new EventHandler(OnDcDisposing);
                RemoveDeviceContext(dc);
            }
        }

        internal static void RemoveDeviceContext(DeviceContext dc)
        {
            if (activeDeviceContexts == null)
            {
                return;
            }
            activeDeviceContexts.RemoveByHashCode(dc);
        }

        internal static bool IsFontInUse(WindowsFont wf)
        {
            if (wf == null)
            {
                return false;
            }

            for (int i = 0; i < activeDeviceContexts.Count; i++)
            {
                if (activeDeviceContexts[i] is DeviceContext dc && (dc.ActiveFont == wf || dc.IsFontOnContextStack(wf)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
