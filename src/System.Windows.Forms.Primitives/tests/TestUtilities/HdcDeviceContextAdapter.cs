// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System
{
    /// <summary>
    ///  Simple adapter for passing <see cref="Gdi32.HDC"/> as <see cref="IDeviceContext"/>. Does not manage HDC
    ///  lifetime.
    /// </summary>
    internal class HdcDeviceContextAdapter : IDeviceContext
    {
        private readonly Gdi32.HDC _hdc;

        public HdcDeviceContextAdapter(Gdi32.HDC hdc) => _hdc = hdc;

        public IntPtr GetHdc() => (IntPtr)_hdc;
        public void ReleaseHdc() { }
        public void Dispose() { }
    }
}
