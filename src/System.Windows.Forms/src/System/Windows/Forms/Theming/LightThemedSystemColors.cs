// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal class LightThemedSystemColors : ThemedSystemColors
    {
        private static LightThemedSystemColors? s_instance;

        public static LightThemedSystemColors DefaultInstance => s_instance ??= new LightThemedSystemColors();
    }
}
