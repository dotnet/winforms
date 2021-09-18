// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing
{
    internal static class ColorExtensions
    {
        public static int ToWin32(this Color color) => ColorTranslator.ToWin32(color);
    }
}
