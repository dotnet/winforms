// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System;

/// <summary>
///  Scope for temporarily setting an <see cref="AppContext"/> switch. Use in a <see langword="using"/> statement.
/// </summary>
/// <remarks>
///  <para>
///   It is recommended to create wrappers for this struct for both simplicity and to allow adding synchronization.
///   See <see cref="BinaryFormatterScope"/> for an example of doing this.
///  </para>
/// </remarks>
public readonly ref struct AppContextSwitchScope
{
    private readonly string _switchName;
    private readonly bool _originalState;

    public AppContextSwitchScope(string? targetFrameworkName, string switchName, bool enable)
    {
        if (!AppContext.TryGetSwitch(AppContextSwitchNames.LocalAppContext_DisableCaching, out bool isEnabled)
            || !isEnabled)
        {
            // It doesn't make sense to try messing with AppContext switches if they are going to be cached.
            throw new InvalidOperationException("LocalAppContext switch caching is not disabled.");
        }
        
        AppContext.TryGetSwitch(switchName, out _originalState);

        AppContext.SetSwitch(switchName, enable);
        if (!AppContext.TryGetSwitch(switchName, out isEnabled) || isEnabled != enable)
        {
            throw new InvalidOperationException($"Could not set {switchName} to {enable}.");
        }

        _switchName = switchName;
    }

    public AppContextSwitchScope(string switchName, bool enable) : this(AppContext.TargetFrameworkName, switchName, enable) { }

        public void Dispose()
    {
        AppContext.SetSwitch(_switchName, _originalState);
        if (!AppContext.TryGetSwitch(_switchName, out bool isEnabled) || isEnabled != _originalState)
        {
            throw new InvalidOperationException($"Could not reset {_switchName} to {_originalState}.");
        }
    }
}
