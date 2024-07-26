// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    [Experimental("WFO9001")]
    public enum ColorMode
    {
        /// <summary>
        ///  Dark mode for the current context is or should be disabled.
        /// </summary>
        Classic = 0,

        /// <summary>
        ///  Dark mode for the current context is or should be enabled.
        /// </summary>
        Dark = 1,

        /// <summary>
        ///  The setting for the current color mode context is inherited from the parent context. Note, that you
        ///  can even pass this value to <see cref="Application.SetDefaultColorMode(ColorMode, bool)"/>, in which case
        ///  the actual dark mode setting will be inherited from the Windows OS system setting.
        /// </summary>
        System = 2
    }
}
