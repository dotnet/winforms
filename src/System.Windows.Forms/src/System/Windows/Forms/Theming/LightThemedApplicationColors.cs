// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    internal class LightThemedApplicationColors : ApplicationColors
    {
        private static LightThemedApplicationColors? s_instance;

        public static LightThemedApplicationColors DefaultInstance => s_instance ??= new LightThemedApplicationColors();
    }
}
