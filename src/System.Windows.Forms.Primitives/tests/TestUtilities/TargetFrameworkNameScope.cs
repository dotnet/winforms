// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;
using System.Windows.Forms.Primitives;

namespace System;

#nullable enable

/// <summary>
///  Scope for setting the see <see cref="Windows.Forms.Primitives.LocalAppContextSwitches.TargetFrameworkName" /> temporarily.
///  Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct TargetFrameworkNameScope
{
    private readonly FrameworkName? _previousTargetFrameworkName;
    private readonly dynamic _testAccessor;

    public TargetFrameworkNameScope(string targetFrameworkName)
    {
        _testAccessor = typeof(LocalAppContextSwitches).TestAccessor().Dynamic;
        ResetLocalSwitches();
        _previousTargetFrameworkName = LocalAppContextSwitches.TargetFrameworkName;
        _testAccessor.s_targetFrameworkName = new FrameworkName(targetFrameworkName);
    }

    public void Dispose()
    {
        _testAccessor.s_targetFrameworkName = _previousTargetFrameworkName;
        ResetLocalSwitches();
    }

    private void ResetLocalSwitches()
    {
        _testAccessor.s_anchorLayoutV2 = 0;
        _testAccessor.s_scaleTopLevelFormMinMaxSizeForDpi = 0;
        _testAccessor.s_trackBarModernRendering = 0;
        _testAccessor.s_servicePointManagerCheckCrl = 0;
    }
}
