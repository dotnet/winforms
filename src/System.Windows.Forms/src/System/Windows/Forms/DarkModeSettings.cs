// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    public enum DarkMode
    {
        /// <summary>
        ///  Dark mode in the current context is not supported.
        /// </summary>
        NotSupported = 0,

        /// <summary>
        ///  The setting for the current dark mode context is inherited from the parent context.
        /// </summary>
        Inherits = 1,

        /// <summary>
        ///  Dark mode for the current context is enabled.
        /// </summary>
        Enabled = 2,

        /// <summary>
        ///  Dark mode the current context is disabled.
        /// </summary>
        Disabled = 3
    }
}
