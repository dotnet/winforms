// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class TreeView
{
#if NET11_0_OR_GREATER
    /// <inheritdoc/>
    protected override void BeginSuspendPaintingCore()
    {
        BeginSuspendPaintingScope();
        BeginUpdate();
    }

    /// <inheritdoc/>
    protected override void EndSuspendPaintingCore()
    {
        if (EndSuspendPaintingScope())
        {
            EndUpdate(invalidate: !IsRecursiveInvalidateAfterSuspendPaintingRequested);
        }
    }
#endif
}
