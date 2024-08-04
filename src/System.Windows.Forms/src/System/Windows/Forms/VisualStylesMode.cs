// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;

namespace System.Windows.Forms;

/// <summary>
///  Represents the version of the visual renderer.
/// </summary>
[Experimental(DiagnosticIDs.ExperimentalVisualStyles, UrlFormat = "https://aka.ms/WfoExperimental/{0}")]
public enum VisualStylesMode
{
    /// <summary>
    ///  The classic version of the visual renderer (.NET 8 and earlier), using the Version 6 of ComCtl.
    /// </summary>
    Classic = 0,

    /// <summary>
    ///  Visual renderers are not in use - see <see cref="Application.UseVisualStyles"/>;
    ///  Controls are based on Version 5 of ComCtl.
    /// </summary>
    Disabled = 1,

    /// <summary>
    ///  The .NET 10 version of the visual renderer. Controls are rendered using the latest version
    ///  of ComCtl and the adorners rendering or the layout of specific controls has been improved/updated
    ///  based on latest accessibility requirements.
    /// </summary>
    Net10 = 2,

    /// <summary>
    ///  The latest version of the visual renderer.
    /// </summary>
    Latest = 65535
}
