﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;

namespace System.Windows.Forms;

[Experimental(DiagnosticIDs.ExperimentalDarkMode, UrlFormat = DiagnosticIDs.UrlFormat)]
public enum SystemColorMode
{
    /// <summary>
    ///  Dark mode for the current context is or should be disabled.
    /// </summary>
    Classic = 0,

    /// <summary>
    ///  The setting for the current system color mode is inherited from the Windows OS.
    /// </summary>
    System = 1,

    /// <summary>
    ///  Dark mode for the current context is enabled.
    /// </summary>
    Dark = 2
}
