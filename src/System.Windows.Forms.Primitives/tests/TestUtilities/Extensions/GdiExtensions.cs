// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System
{
    internal static class GdiExtensions
    {
        public static string ToSystemColorString(this COLORREF colorRef)
            => SystemCOLORs.ToSystemColorString(colorRef);
    }
}
