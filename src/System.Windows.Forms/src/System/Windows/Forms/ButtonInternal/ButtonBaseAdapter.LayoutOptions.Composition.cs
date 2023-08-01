// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ButtonInternal;

internal abstract partial class ButtonBaseAdapter
{
    internal partial class LayoutOptions
    {
        private enum Composition
        {
            NoneCombined = 0x00,
            CheckCombined = 0x01,
            TextImageCombined = 0x02,
            AllCombined = 0x03
        }
    }
}
