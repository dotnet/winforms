// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

using System.ComponentModel;
using System.Runtime.ExceptionServices;

#if NET11_0_OR_GREATER
/// <summary>
///  Suspends painting (and optionally layout) for a target until the scope is disposed.
/// </summary>
/// <remarks>
///  <para>
///   This is a sealed class rather than a <c>ref struct</c> so the scope can span an
///   <see langword="await"/> in an asynchronous UI event handler (for example, suspending painting for
///   the duration of an async data reload). <see cref="Dispose"/> is idempotent: disposing the scope more
///   than once only resumes painting once.
///  </para>
///  <para>
///   The type is internal; callers obtain it as an <see cref="IDisposable"/> from
///   <see cref="ControlMutationExtensions"/>, which is why the consumer-facing guidance above is also
///   documented on those extension methods.
///  </para>
/// </remarks>
internal sealed class SuspendPaintingScope : IDisposable
{
    private ISupportSuspendPainting? _target;
    private Control[]? _layoutControls;

    /// <summary>
    ///  Initializes a new instance of the <see cref="SuspendPaintingScope"/> class.
    /// </summary>
    /// <param name="target">The target whose painting should be suspended.</param>
    internal SuspendPaintingScope(ISupportSuspendPainting? target)
    {
        _target = target;

        if (target is null)
        {
            return;
        }

        int initialPaintingCount = target is Control control
            ? control.SuspendPaintingCount
            : 0;

        try
        {
            target.BeginSuspendPainting();
        }
        catch (Exception exception)
        {
            _target = null;
            List<Exception> exceptions = [exception];
            UnwindFailedPaintingAcquisition(target, initialPaintingCount, exceptions);
            ThrowExceptions(exceptions);
        }
    }

    internal SuspendPaintingScope(
        ISupportSuspendPainting target,
        LayoutSuspendTraversal layoutSuspendTraversal)
        : this(target, GetLayoutControls(target, layoutSuspendTraversal))
    {
    }

    internal SuspendPaintingScope(
        ISupportSuspendPainting target,
        Func<Control, bool> suspendLayoutContainerFilter)
        : this(target, GetLayoutControls(target, suspendLayoutContainerFilter))
    {
    }

    private SuspendPaintingScope(
        ISupportSuspendPainting target,
        Control[] layoutControls)
    {
        _target = target;
        _layoutControls = layoutControls;
        int initialPaintingCount = ((Control)target).SuspendPaintingCount;
        int suspendedLayoutCount = 0;

        try
        {
            target.BeginSuspendPainting();

            foreach (Control control in layoutControls)
            {
                control.SuspendLayout();
                suspendedLayoutCount++;
            }
        }
        catch (Exception exception)
        {
            List<Exception> exceptions = [exception];
            ResumeLayouts(layoutControls, suspendedLayoutCount, exceptions);
            UnwindFailedPaintingAcquisition(target, initialPaintingCount, exceptions);

            _target = null;
            _layoutControls = null;
            ThrowExceptions(exceptions);
        }
    }

    /// <summary>
    ///  Resumes layout and painting for the target associated with this scope.
    /// </summary>
    public void Dispose()
    {
        ISupportSuspendPainting? target = _target;
        Control[] layoutControls = _layoutControls ?? [];
        _target = null;
        _layoutControls = null;

        if (target is null)
        {
            return;
        }

        List<Exception> exceptions = [];
        ResumeLayouts(layoutControls, layoutControls.Length, exceptions);
        EndPainting(target, exceptions, requestRecursiveInvalidate: target is Control);

        ThrowExceptions(exceptions);
    }

    private static Control[] GetLayoutControls(
        ISupportSuspendPainting target,
        LayoutSuspendTraversal layoutSuspendTraversal)
    {
        Control control = GetControl(target);

        return layoutSuspendTraversal switch
        {
            LayoutSuspendTraversal.None => [],
            LayoutSuspendTraversal.TargetOnly => [control],
            LayoutSuspendTraversal.TargetAndDescendants => GetLayoutControls(control, static _ => true),
            _ => throw new InvalidEnumArgumentException(
                nameof(layoutSuspendTraversal),
                (int)layoutSuspendTraversal,
                typeof(LayoutSuspendTraversal))
        };
    }

    private static Control[] GetLayoutControls(
        ISupportSuspendPainting target,
        Func<Control, bool> suspendLayoutContainerFilter)
    {
        ArgumentNullException.ThrowIfNull(suspendLayoutContainerFilter);

        return GetLayoutControls(GetControl(target), suspendLayoutContainerFilter);
    }

    private static Control GetControl(ISupportSuspendPainting target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return target as Control
            ?? throw new InvalidOperationException(SR.ControlMutationExtensionsLayoutSuspensionRequiresControl);
    }

    private static Control[] GetLayoutControls(
        Control target,
        Func<Control, bool> suspendLayoutContainerFilter)
    {
        List<Control> layoutControls = [];
        Stack<Control> controlsToVisit = new();
        controlsToVisit.Push(target);

        while (controlsToVisit.TryPop(out Control? control))
        {
            if (suspendLayoutContainerFilter(control))
            {
                layoutControls.Add(control);
            }

            if (control.ChildControls is not { } children)
            {
                continue;
            }

            for (int i = children.Count - 1; i >= 0; i--)
            {
                controlsToVisit.Push(children[i]);
            }
        }

        return [.. layoutControls];
    }

    private static void ResumeLayouts(
        Control[] layoutControls,
        int suspendedLayoutCount,
        List<Exception> exceptions)
    {
        for (int i = suspendedLayoutCount - 1; i >= 0; i--)
        {
            try
            {
                layoutControls[i].ResumeLayoutAfterSuspendPainting();
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }
        }
    }

    private static void EndPainting(
        ISupportSuspendPainting target,
        List<Exception> exceptions,
        bool requestRecursiveInvalidate = false)
    {
        try
        {
            if (requestRecursiveInvalidate)
            {
                ((Control)target).RequestRecursiveInvalidateAfterSuspendPainting();
            }

            target.EndSuspendPainting();
        }
        catch (Exception exception)
        {
            exceptions.Add(exception);
        }
    }

    private static void UnwindFailedPaintingAcquisition(
        ISupportSuspendPainting target,
        int initialPaintingCount,
        List<Exception> exceptions)
    {
        if (target is not Control control)
        {
            return;
        }

        while (control.SuspendPaintingCount > initialPaintingCount)
        {
            int paintingCount = control.SuspendPaintingCount;

            try
            {
                target.EndSuspendPainting();
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
                return;
            }

            if (control.SuspendPaintingCount >= paintingCount)
            {
                return;
            }
        }
    }

    private static void ThrowExceptions(List<Exception> exceptions)
    {
        if (exceptions.Count == 1)
        {
            ExceptionDispatchInfo.Throw(exceptions[0]);
        }

        if (exceptions.Count > 1)
        {
            throw new AggregateException(exceptions);
        }
    }
}
#endif
