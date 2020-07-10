// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Used to provide a way to give <see cref="DeviceContextHdcScope"/> direct internal access to HDC's.
    /// </summary>
    internal interface IGraphicsHdcProvider
    {
        Gdi32.HDC GetHDC();

        Graphics? GetGraphics(bool create);

        bool IsStateClean { get; }
    }
}
