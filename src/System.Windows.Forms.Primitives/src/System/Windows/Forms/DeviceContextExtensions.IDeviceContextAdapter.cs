// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal static partial class DeviceContextExtensions
{
    private class IDeviceContextAdapter : IHdcContext
    {
        private readonly IDeviceContext _deviceContext;
        public IDeviceContextAdapter(IDeviceContext deviceContext) => _deviceContext = deviceContext;
        public HDC GetHdc() => (HDC)_deviceContext.GetHdc();
        public void ReleaseHdc() => _deviceContext.ReleaseHdc();
    }
}
