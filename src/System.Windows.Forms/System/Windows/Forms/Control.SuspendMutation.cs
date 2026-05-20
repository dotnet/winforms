// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
public unsafe partial class Control :
    ISupportSuspendPainting,
    ISupportSuspendRelocation
#else
public unsafe partial class Control
#endif
{
#if NET11_0_OR_GREATER
    /// <summary>
    ///  Defers child-control location changes until the returned scope is disposed.
    /// </summary>
    /// <returns>A scope that applies deferred location changes when disposed.</returns>
    public DeferLocationChangeScope DeferLocationChange()
        => new(this);

    /// <summary>
    ///  Defers child-control location changes until the returned scope is disposed.
    /// </summary>
    /// <param name="suppressRender">
    ///  <see langword="true"/> to suppress rendering while changes are deferred; otherwise,
    ///  <see langword="false"/>.
    /// </param>
    /// <returns>A scope that applies deferred location changes when disposed.</returns>
    public DeferLocationChangeScope DeferLocationChange(bool suppressRender)
        => new(this, suppressRender);

    /// <summary>
    ///  Defers child-control location changes until the returned scope is disposed.
    /// </summary>
    /// <param name="suppressRender">
    ///  <see langword="true"/> to suppress rendering while changes are deferred; otherwise,
    ///  <see langword="false"/>.
    /// </param>
    /// <param name="suspendLayout">
    ///  <see langword="true"/> to suspend layout while changes are deferred; otherwise,
    ///  <see langword="false"/>.
    /// </param>
    /// <returns>A scope that applies deferred location changes when disposed.</returns>
    public DeferLocationChangeScope DeferLocationChange(bool suppressRender, bool suspendLayout)
        => new(this, suppressRender, suspendLayout);

    /// <summary>
    ///  Begins a painting suspension region for this control.
    /// </summary>
    public virtual void BeginSuspendPainting()
    {
        BeginSuspendPaintingScope();
        BeginUpdateInternal();
    }

    /// <summary>
    ///  Ends a painting suspension region for this control.
    /// </summary>
    public virtual void EndSuspendPainting()
    {
        if (EndSuspendPaintingScope())
        {
            EndUpdateInternal(invalidate: true);
        }
    }

    /// <summary>
    ///  Begins a relocation suspension region for this control.
    /// </summary>
    public virtual void BeginSuspendRelocation() => SuspendLayout();

    /// <summary>
    ///  Ends a relocation suspension region for this control.
    /// </summary>
    public virtual void EndSuspendRelocation() => ResumeLayout();

    internal bool BeginSuspendPaintingScope()
    {
        int suspendPaintingCount = Properties.GetValueOrDefault(s_suspendPaintingCountProperty, 0);
        Properties.AddOrRemoveValue(
            s_suspendPaintingCountProperty,
            suspendPaintingCount + 1,
            defaultValue: 0);

        return true;
    }

    internal bool EndSuspendPaintingScope()
    {
        int suspendPaintingCount = Properties.GetValueOrDefault(s_suspendPaintingCountProperty, 0);
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
#endif
}
