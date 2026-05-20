// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ListView
{
#if NET11_0_OR_GREATER
    /// <inheritdoc/>
    public override void BeginSuspendPainting()
    {
        BeginSuspendPaintingScope();
        BeginUpdate();
    }

    /// <inheritdoc/>
    public override void EndSuspendPainting()
    {
        if (EndSuspendPaintingScope())
        {
            EndUpdate();
        }
    }
#endif
}
