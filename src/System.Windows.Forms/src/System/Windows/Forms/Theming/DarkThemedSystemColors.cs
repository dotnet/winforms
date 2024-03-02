// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    internal class DarkThemedSystemColors : ThemedSystemColors
    {
        private static DarkThemedSystemColors? s_instance;

        public static DarkThemedSystemColors DefaultInstance => s_instance ??= new DarkThemedSystemColors();

        public override Color Control => Color.FromArgb(0xFF, 0x20, 0x20, 0x20);
        public override Color ControlText => Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0);
        public override Color ControlDark => Color.FromArgb(0xFF, 0x40, 0x40, 0x40);
        public override Color HighlightText => Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
        public override Color MenuText => Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0);
        public override Color ButtonFace => Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0);
        public override Color ButtonShadow => Color.FromArgb(0xFF, 0x30, 0x30, 0x30);
    }
}
