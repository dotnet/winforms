// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualStudio.Shell;

[Flags]
internal enum CTLBLDTYPE : uint
{
    /// <summary>
    ///  Invoke a standard system builder (not supported in Visual Studio).
    /// </summary>
    CTLBLDTYPE_FSTDPROPBUILDER = 0x00000001,

    /// <summary>
    ///  Invoke a custom builder.
    /// </summary>
    CTLBLDTYPE_FINTERNALBUILDER = 0x00000002,

    /// <summary>
    ///  Builder modifies the object. This is common behavior.
    /// </summary>
    CTLBLDTYPE_FEDITSOBJIDRECTLY = 0x00000004,
}
