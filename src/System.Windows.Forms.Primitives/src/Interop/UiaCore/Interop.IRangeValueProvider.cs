// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("36dc7aef-33e6-4691-afe1-2be7274b3d33")]
        public interface IRangeValueProvider
        {
            void SetValue(double value);

            double Value { get; }

            BOOL IsReadOnly { get; }

            double Maximum { get; }

            double Minimum { get; }

            double LargeChange { get; }

            double SmallChange { get; }
        }
    }
}
