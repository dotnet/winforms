// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Part of DpiHelper class, with methods specific to WinForms assembly
    ///  Also handles configuration switches that control states of various WinForms features
    /// </summary>
    internal static partial class DpiHelper
    {
        /// <summary>
        ///  Returns a new Padding with the input's
        ///  dimensions converted from logical units to device units.
        /// </summary>
        /// <param name="logicalPadding">Padding in logical units</param>
        /// <returns>Padding in device units</returns>
        public static Padding LogicalToDeviceUnits(Padding logicalPadding, int deviceDpi = 0)
        {
            return new Padding(LogicalToDeviceUnits(logicalPadding.Left, deviceDpi),
                               LogicalToDeviceUnits(logicalPadding.Top, deviceDpi),
                               LogicalToDeviceUnits(logicalPadding.Right, deviceDpi),
                               LogicalToDeviceUnits(logicalPadding.Bottom, deviceDpi));
        }
    }
}
