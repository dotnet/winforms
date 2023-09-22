// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Com;

/// <summary>
///  Helper to scope lifetime of a <see cref="SAFEARRAY"/> created via <see cref="PInvoke.SafeArrayCreate(VARENUM, uint, in SAFEARRAYBOUND)"/>
///  Destroys the <see cref="SAFEARRAY"/> (if any) when disposed.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement to ensure the <see cref="SAFEARRAY"/> gets disposed.
///   Use <see cref="Value"/> to pass the <see cref="SAFEARRAY"/> to APIs that populate it.
///  </para>
/// </remarks>
internal unsafe ref struct SafeArrayScope
{
    public SAFEARRAY* Value { get; private set; }

    public SafeArrayScope(VARENUM vt, uint cDims, SAFEARRAYBOUND bound)
    {
        Value = PInvoke.SafeArrayCreate(vt, cDims, &bound);
    }

    public readonly bool IsNull => Value is null;

    public void Dispose()
    {
        if (!IsNull)
        {
            PInvoke.SafeArrayDestroy(Value);
            Value = null;
        }
    }
}
