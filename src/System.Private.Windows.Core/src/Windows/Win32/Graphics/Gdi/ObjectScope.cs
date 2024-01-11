// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

/// <summary>
///  Helper to scope lifetime of a GDI object. Deletes the given object (if any) when disposed.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass
///   by <see langword="ref" /> to avoid duplicating the handle and risking a double deletion.
///  </para>
/// </remarks>
#if DEBUG
internal class ObjectScope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct ObjectScope
#endif
{
    public HGDIOBJ HGDIOBJ { get; }

    /// <param name="object">The object to be deleted when the scope closes.</param>
    public ObjectScope(HGDIOBJ @object) => HGDIOBJ = @object;

    public static implicit operator HGDIOBJ(in ObjectScope objectScope) => objectScope.HGDIOBJ;

    public void Dispose()
    {
        if (!HGDIOBJ.IsNull)
        {
            PInvokeCore.DeleteObject(HGDIOBJ);
        }

        DisposalTracking.SuppressFinalize(this!);
    }
}
