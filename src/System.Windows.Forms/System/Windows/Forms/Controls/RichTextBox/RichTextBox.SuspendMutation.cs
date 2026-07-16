// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class RichTextBox
{
#if NET11_0_OR_GREATER
    /// <inheritdoc/>
    protected override void BeginSuspendPaintingCore()
    {
        BeginSuspendPaintingScope();
        BeginUpdateInternal();
    }

    /// <inheritdoc/>
    protected override void EndSuspendPaintingCore()
    {
        if (EndSuspendPaintingScope())
        {
            EndUpdateInternal(invalidate: !IsRecursiveInvalidateAfterSuspendPaintingRequested);
        }
    }
#endif
}
