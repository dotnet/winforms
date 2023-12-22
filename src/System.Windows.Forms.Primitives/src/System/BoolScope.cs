// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

/// <summary>
///  Allows a bool to be temporarily set to its opposite value within a scope.
///  It will reset the bool to its original value once out of scope.
/// </summary>
internal ref struct BoolScope
{
    private ref bool _value;

    public BoolScope(ref bool value)
    {
        _value = ref value;
        _value = !value;
    }

    public void Dispose() => _value = !_value;
}
