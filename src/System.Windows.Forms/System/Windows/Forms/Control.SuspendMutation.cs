// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
public unsafe partial class Control :
    ISupportSuspendPainting
#else
public unsafe partial class Control
#endif
{
#if NET11_0_OR_GREATER
    /// <summary>
    ///  Begins a painting suspension region for this control.
    /// </summary>
    void ISupportSuspendPainting.BeginSuspendPainting() => BeginSuspendPaintingCore();

    /// <summary>
    ///  Ends a painting suspension region for this control.
    /// </summary>
    void ISupportSuspendPainting.EndSuspendPainting()
    {
        EndSuspendPaintingCore();
        CompleteRecursiveInvalidateAfterSuspendPainting();
    }

    /// <summary>
    ///  When overridden in a derived class, begins a painting suspension region for this control.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The default implementation suppresses native painting through <see cref="BeginUpdateInternal"/>.
    ///   Controls that already have a public update mechanism (such as <see cref="ListView.BeginUpdate"/>)
    ///   should override this method to route through that existing mechanism instead of introducing a
    ///   second, competing suspension path.
    ///  </para>
    /// </remarks>
    protected virtual void BeginSuspendPaintingCore()
    {
        BeginSuspendPaintingScope();
        BeginUpdateInternal();
    }

    /// <summary>
    ///  When overridden in a derived class, ends a painting suspension region for this control.
    /// </summary>
    protected virtual void EndSuspendPaintingCore()
    {
        if (EndSuspendPaintingScope())
        {
            EndUpdateInternal(invalidate: !IsRecursiveInvalidateAfterSuspendPaintingRequested);
        }
    }

    internal bool IsRecursiveInvalidateAfterSuspendPaintingRequested
        => Properties.ContainsKey(s_recursiveInvalidateAfterSuspendPaintingProperty);

    internal int SuspendPaintingCount
        => Properties.GetValueOrDefault(s_suspendPaintingCountProperty, 0);

    internal void RequestRecursiveInvalidateAfterSuspendPainting()
        => Properties.AddValue(s_recursiveInvalidateAfterSuspendPaintingProperty, true);

    internal bool BeginSuspendPaintingScope()
    {
        int suspendPaintingCount = Properties.GetValueOrDefault(
            key: s_suspendPaintingCountProperty,
            defaultValue: 0);

        Properties.AddOrRemoveValue(
            key: s_suspendPaintingCountProperty,
            value: suspendPaintingCount + 1,
            defaultValue: 0);

        return true;
    }

    internal bool EndSuspendPaintingScope()
    {
        int suspendPaintingCount = Properties.GetValueOrDefault(
            key: s_suspendPaintingCountProperty,
            defaultValue: 0);

        if (suspendPaintingCount == 0)
        {
            return false;
        }

        Properties.AddOrRemoveValue(
            s_suspendPaintingCountProperty,
            suspendPaintingCount - 1,
            defaultValue: 0);

        return true;
    }

    internal void ResumeLayoutAfterSuspendPainting()
    {
        byte layoutSuspendCount = LayoutSuspendCount;

        try
        {
            ResumeLayout();
        }
        catch
        {
            if (LayoutSuspendCount == layoutSuspendCount && LayoutSuspendCount > 0)
            {
                LayoutSuspendCount--;
            }

            throw;
        }
    }

    private void CompleteRecursiveInvalidateAfterSuspendPainting()
    {
        if (Properties.GetValueOrDefault(s_suspendPaintingCountProperty, 0) != 0
            || !Properties.ContainsKey(s_recursiveInvalidateAfterSuspendPaintingProperty))
        {
            return;
        }

        Properties.RemoveValue(s_recursiveInvalidateAfterSuspendPaintingProperty);
        Invalidate(invalidateChildren: true);
    }
#endif
}
