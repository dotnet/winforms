// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

#pragma warning disable SYSLIB5002 // Type is for evaluation purposes only and is subject to change or removal in future updates.

namespace System.Windows.Forms.Tests;

/// <summary>
///  Temporarily overrides the application color mode without broadcasting system color changes.
/// </summary>
internal sealed class ApplicationColorModeTestScope : IDisposable
{
    private readonly dynamic _applicationAccessor = typeof(Application).TestAccessor.Dynamic;
    private readonly SystemColorMode? _previousColorMode;

    public ApplicationColorModeTestScope(bool darkModeEnabled)
    {
        _previousColorMode = _applicationAccessor.s_colorMode;
        _applicationAccessor.s_colorMode = darkModeEnabled
            ? SystemColorMode.Dark
            : SystemColorMode.Classic;
    }

    public void Dispose()
        => _applicationAccessor.s_colorMode = _previousColorMode;
}

#pragma warning restore SYSLIB5002
