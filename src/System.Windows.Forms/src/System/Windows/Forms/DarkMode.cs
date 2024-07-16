// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    [Experimental("WFO9001")]
    public enum DarkMode
    {
        /// <summary>
        ///  Dark mode in the current context is not supported. This value cannot be set as a default value; it is rather a potential return value
        ///  for the <see cref="Application.EnvironmentDarkMode"/> property.
        /// </summary>
        NotSupported = 0,

        /// <summary>
        ///  The setting for the current dark mode context is inherited from the parent context. Note, that you
        ///  can even pass this value to <see cref="Application.SetDefaultDarkMode(DarkMode)"/>, in which case
        ///  the actual dark mode setting will be inherited from the Windows OS system setting.
        /// </summary>
        Inherits = 1,

        /// <summary>
        ///  Dark mode for the current context is or should be enabled.
        /// </summary>
        Enabled = 2,

        /// <summary>
        ///  Dark mode the current context is or should be disabled.
        /// </summary>
        Disabled = 3
    }
}
