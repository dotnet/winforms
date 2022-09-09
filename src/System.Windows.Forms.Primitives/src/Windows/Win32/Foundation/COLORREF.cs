// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace Windows.Win32.Foundation
{
    internal partial struct COLORREF
    {
        public static implicit operator COLORREF(Color color) => new((uint)ColorTranslator.ToWin32(color));
        public static implicit operator Color(COLORREF color) => ColorTranslator.FromWin32((int)color.Value);
    }
}
