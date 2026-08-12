// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")]
public class SystemVisualSettingsTests
{
    private const int WmDwmColorizationColorChanged = 0x0320;

    [WinFormsFact]
    public void SystemVisualSettings_Snapshot_ValuesAreImmutable()
    {
        SystemVisualSettings settings = CreateSettings(
            accentColor: Color.MediumPurple,
            textScaleFactor: 1.5f,
            highContrastEnabled: true,
            clientAreaAnimationEnabled: false,
            keyboardCuesVisible: true,
            focusBorderMetrics: new Size(3, 4));

        Assert.Equal(Color.MediumPurple, settings.AccentColor);
        Assert.Equal(1.5f, settings.TextScaleFactor);
        Assert.True(settings.HighContrastEnabled);
        Assert.False(settings.ClientAreaAnimationEnabled);
        Assert.True(settings.KeyboardCuesVisible);
        Assert.Equal(new Size(3, 4), settings.FocusBorderMetrics);
    }

    [WinFormsFact]
    public void SystemVisualSettingsTracker_CreateSnapshot_MapsNativeValues()
    {
        SystemVisualSettingsNativeValues values = new(
            Color.CornflowerBlue,
            1.25f,
            true,
            false,
            true,
            new Size(2, 5));

        SystemVisualSettings snapshot = SystemVisualSettingsTracker.CreateSnapshot(values);

        Assert.Equal(values.AccentColor, snapshot.AccentColor);
        Assert.Equal(values.TextScaleFactor, snapshot.TextScaleFactor);
        Assert.Equal(values.HighContrastEnabled, snapshot.HighContrastEnabled);
        Assert.Equal(values.ClientAreaAnimationEnabled, snapshot.ClientAreaAnimationEnabled);
        Assert.Equal(values.KeyboardCuesVisible, snapshot.KeyboardCuesVisible);
        Assert.Equal(values.FocusBorderMetrics, snapshot.FocusBorderMetrics);
    }

    [WinFormsFact]
    public void SystemVisualSettingsTracker_GetChangedCategories_ReturnsAllChangedCategories()
    {
        SystemVisualSettings oldSettings = CreateSettings();
        SystemVisualSettings newSettings = CreateSettings(
            accentColor: Color.Crimson,
            textScaleFactor: 1.5f,
            highContrastEnabled: true,
            clientAreaAnimationEnabled: false,
            keyboardCuesVisible: true,
            focusBorderMetrics: new Size(2, 3));

        SystemVisualSettingsCategories changed = SystemVisualSettingsTracker.GetChangedCategories(oldSettings, newSettings);

        Assert.Equal(
            SystemVisualSettingsCategories.AccentColor
                | SystemVisualSettingsCategories.TextScale
                | SystemVisualSettingsCategories.HighContrast
                | SystemVisualSettingsCategories.ClientAreaAnimations
                | SystemVisualSettingsCategories.KeyboardCues
                | SystemVisualSettingsCategories.FocusMetrics,
            changed);
    }

    [WinFormsFact]
    public void SystemVisualSettingsTestScope_OverridesAnimationsAndRestoresPreviousSnapshot()
    {
        SystemVisualSettings disabled = CreateSettings(clientAreaAnimationEnabled: false);

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(disabled);

            using (new SystemVisualSettingsTestScope(clientAreaAnimationEnabled: true))
            {
                Assert.True(Application.SystemVisualSettings.ClientAreaAnimationEnabled);
            }

            Assert.Same(disabled, Application.SystemVisualSettings);
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void Application_SystemVisualSettingsChanged_RefreshRaisesOncePerTransition()
    {
        SystemVisualSettings initial = CreateSettings();
        SystemVisualSettings next = CreateSettings(textScaleFactor: 1.5f);
        SystemVisualSettingsChangedEventHandler? handler = null;
        int callCount = 0;

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial, () => next);
            handler = (sender, e) =>
            {
                Assert.Null(sender);
                Assert.Same(initial, e.OldSettings);
                Assert.Same(next, e.NewSettings);
                Assert.Equal(SystemVisualSettingsCategories.TextScale, e.Changed);
                callCount++;
            };

            Application.SystemVisualSettingsChanged += handler;

            Assert.Same(initial, Application.SystemVisualSettings);
            Assert.Same(next, SystemVisualSettingsTracker.Refresh());
            Assert.Same(next, SystemVisualSettingsTracker.Refresh());
            Assert.Equal(1, callCount);
            Assert.Equal(1, SystemVisualSettingsTracker.TransitionVersion);
        }
        finally
        {
            if (handler is not null)
            {
                Application.SystemVisualSettingsChanged -= handler;
            }

            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public async Task SystemVisualSettingsTracker_ConcurrentRefresh_DoesNotPublishStaleSnapshot()
    {
        SystemVisualSettings initial = CreateSettings();
        SystemVisualSettings stale = CreateSettings(accentColor: Color.Orange);
        SystemVisualSettings current = CreateSettings(accentColor: Color.Purple);
        using ManualResetEventSlim firstSampleStarted = new(initialState: false);
        using ManualResetEventSlim releaseFirstSample = new(initialState: false);
        int providerCallCount = 0;

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(
                initial,
                () =>
                {
                    if (Interlocked.Increment(ref providerCallCount) == 1)
                    {
                        firstSampleStarted.Set();
                        releaseFirstSample.Wait(TestContext.Current.CancellationToken);
                        return stale;
                    }

                    return current;
                });

            Task<SystemVisualSettings> staleRefresh = Task.Run(
                SystemVisualSettingsTracker.Refresh,
                TestContext.Current.CancellationToken);
            Assert.True(firstSampleStarted.Wait(TimeSpan.FromSeconds(10)));

            Task<SystemVisualSettings> currentRefresh = Task.Run(
                SystemVisualSettingsTracker.Refresh,
                TestContext.Current.CancellationToken);
            Assert.Same(current, await currentRefresh);
            releaseFirstSample.Set();
            Assert.Same(current, await staleRefresh);

            Assert.Same(current, SystemVisualSettingsTracker.CurrentSettings);
        }
        finally
        {
            releaseFirstSample.Set();
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void Control_SystemVisualSettingsChanged_CascadesToChildren()
    {
        SystemVisualSettingsChangedEventArgs eventArgs = new(
            CreateSettings(),
            CreateSettings(textScaleFactor: 1.5f),
            SystemVisualSettingsCategories.TextScale);
        using SubSystemVisualSettingsControl parent = new();
        using Control child = new();
        parent.Controls.Add(child);

        int parentCallCount = 0;
        int childCallCount = 0;
        parent.SystemVisualSettingsChanged += (sender, e) =>
        {
            Assert.Same(parent, sender);
            Assert.Same(eventArgs, e);
            parentCallCount++;
        };
        child.SystemVisualSettingsChanged += (sender, e) =>
        {
            Assert.Same(child, sender);
            Assert.Same(eventArgs, e);
            childCallCount++;
        };

        parent.RaiseSystemVisualSettingsChanged(eventArgs);

        Assert.Equal(1, parentCallCount);
        Assert.Equal(1, childCallCount);
    }

    [WinFormsFact]
    public void AnimatedControlRenderer_SystemVisualSettingsAccentChange_InvalidatesCachedAccentAndRepaints()
    {
        SystemVisualSettings initial = CreateSettings(accentColor: Color.Crimson);
        SystemVisualSettings next = CreateSettings(accentColor: Color.RoyalBlue);

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial);
            using SubSystemVisualSettingsControl control = new();
            control.CreateControl();
            using SettingsAnimationRenderer renderer = new(control);
            bool invalidated = false;
            control.Invalidated += (sender, e) => invalidated = true;

            Assert.Equal(Color.Crimson, renderer.AccentColor);
            Assert.True(renderer.IsAccentColorCached);

            SystemVisualSettingsTracker.ResetForTesting(next);
            control.RaiseSystemVisualSettingsChanged(
                new SystemVisualSettingsChangedEventArgs(
                    initial,
                    next,
                    SystemVisualSettingsCategories.AccentColor));

            Assert.False(renderer.IsAccentColorCached);
            Assert.Equal(Color.RoyalBlue, renderer.AccentColor);
            Assert.True(invalidated);
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void AnimatedControlRenderer_AnimationsDisabled_StartsAndSettlesWithoutRegistering()
    {
        SystemVisualSettings disabled = CreateSettings(clientAreaAnimationEnabled: false);

        try
        {
            AnimationManager.DisposeCurrentForTesting();
            SystemVisualSettingsTracker.ResetForTesting(disabled);
            using Control control = new();
            using SettingsAnimationRenderer renderer = new(control);

            renderer.StartAnimation();

            Assert.False(renderer.IsRunning);
            Assert.Equal(1, renderer.StartCount);
            Assert.Equal(1, renderer.EndCount);
            Assert.Equal(1f, renderer.LastProgress);
        }
        finally
        {
            AnimationManager.DisposeCurrentForTesting();
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void AnimatedControlRenderer_AnimationsDisabledAtRuntime_SnapsAndReenablingOnlyAffectsFutureTransitions()
    {
        SystemVisualSettings enabled = CreateSettings(clientAreaAnimationEnabled: true);
        SystemVisualSettings disabled = CreateSettings(clientAreaAnimationEnabled: false);

        try
        {
            AnimationManager.DisposeCurrentForTesting();
            SystemVisualSettingsTracker.ResetForTesting(enabled);
            using SubSystemVisualSettingsControl control = new();
            using SettingsAnimationRenderer renderer = new(control);
            renderer.StartAnimation();
            AnimationManager manager = AnimationManager.GetCurrentForTesting();

            Assert.True(renderer.IsRunning);

            SystemVisualSettingsTracker.ResetForTesting(disabled);
            control.RaiseSystemVisualSettingsChanged(
                new SystemVisualSettingsChangedEventArgs(
                    enabled,
                    disabled,
                    SystemVisualSettingsCategories.ClientAreaAnimations));

            Assert.False(renderer.IsRunning);
            Assert.Equal(1f, renderer.LastProgress);
            Assert.Equal(1, renderer.EndCount);
            Assert.Equal(0, manager.RendererCountForTesting);

            SystemVisualSettingsTracker.ResetForTesting(enabled);
            control.RaiseSystemVisualSettingsChanged(
                new SystemVisualSettingsChangedEventArgs(
                    disabled,
                    enabled,
                    SystemVisualSettingsCategories.ClientAreaAnimations));

            Assert.False(renderer.IsRunning);
            Assert.Equal(1, renderer.StartCount);
            Assert.Equal(1, renderer.EndCount);

            renderer.StartAnimation();

            Assert.True(renderer.IsRunning);
            Assert.Equal(2, renderer.StartCount);
        }
        finally
        {
            AnimationManager.DisposeCurrentForTesting();
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void Control_SystemVisualSettingsChanged_HighContrastTransitionDispatchesVisualStylesImpactOnce()
    {
        SystemVisualSettings initial = CreateSettings(highContrastEnabled: false);
        SystemVisualSettings next = CreateSettings(highContrastEnabled: true);

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial);
            using VisualStylesImpactForm parent = new() { VisualStylesMode = VisualStylesMode.Net11 };
            using VisualStylesImpactPanel child = new();
            parent.Controls.Add(child);
            parent.CreateControl();
            child.CreateControl();
            parent.ResetVisualStylesTransitionCount();
            child.ResetVisualStylesTransitionCount();
            int parentLayoutCount = 0;
            parent.Layout += (sender, e) => parentLayoutCount++;

            SystemVisualSettingsTracker.ResetForTesting(next);
            parent.RaiseSystemVisualSettingsChanged(
                new SystemVisualSettingsChangedEventArgs(
                    initial,
                    next,
                    SystemVisualSettingsCategories.HighContrast));

            Assert.Equal(1, parent.VisualStylesTransitionCount);
            Assert.Equal(1, child.VisualStylesTransitionCount);
            Assert.Equal(1, parentLayoutCount);
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void Control_SystemVisualSettingsChanged_HighContrastEffectiveEqualityDoesNotDispatchVisualStylesImpact()
    {
        SystemVisualSettings initial = CreateSettings(highContrastEnabled: false);
        SystemVisualSettings next = CreateSettings(highContrastEnabled: true);

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial);
            using VisualStylesImpactForm parent = new() { VisualStylesMode = VisualStylesMode.Classic };
            using VisualStylesImpactPanel child = new();
            parent.Controls.Add(child);
            parent.CreateControl();
            child.CreateControl();
            parent.ResetVisualStylesTransitionCount();
            child.ResetVisualStylesTransitionCount();
            int parentLayoutCount = 0;
            parent.Layout += (sender, e) => parentLayoutCount++;

            SystemVisualSettingsTracker.ResetForTesting(next);
            parent.RaiseSystemVisualSettingsChanged(
                new SystemVisualSettingsChangedEventArgs(
                    initial,
                    next,
                    SystemVisualSettingsCategories.HighContrast));

            Assert.Equal(0, parent.VisualStylesTransitionCount);
            Assert.Equal(0, child.VisualStylesTransitionCount);
            Assert.Equal(0, parentLayoutCount);
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void Control_SystemVisualSettingsChanged_HighContrastReentrantChildChangeDoesNotRecascadeGrandchild()
    {
        SystemVisualSettings initial = CreateSettings(highContrastEnabled: true);
        SystemVisualSettings next = CreateSettings(highContrastEnabled: false);

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial);
            using VisualStylesImpactForm parent = new() { VisualStylesMode = VisualStylesMode.Net11 };
            using VisualStylesImpactPanel child = new();
            using VisualStylesImpactPanel grandchild = new();
            parent.Controls.Add(child);
            child.Controls.Add(grandchild);
            parent.CreateControl();
            child.CreateControl();
            grandchild.CreateControl();
            parent.ResetVisualStylesTransitionCount();
            child.ResetVisualStylesTransitionCount();
            grandchild.ResetVisualStylesTransitionCount();
            bool reentered = false;
            child.VisualStylesModeChanged += (sender, e) =>
            {
                if (!reentered)
                {
                    reentered = true;
                    child.VisualStylesMode = VisualStylesMode.Latest;
                }
            };

            SystemVisualSettingsTracker.ResetForTesting(next);
            parent.RaiseSystemVisualSettingsChanged(
                new SystemVisualSettingsChangedEventArgs(
                    initial,
                    next,
                    SystemVisualSettingsCategories.HighContrast));

            Assert.Equal(1, parent.VisualStylesTransitionCount);
            Assert.Equal(2, child.VisualStylesTransitionCount);
            Assert.Equal(1, grandchild.VisualStylesTransitionCount);
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void SystemVisualSettingsTracker_ProcessTopLevelTrees_DeduplicatesApplicationAndDeliversOnOwningThread()
    {
        SystemVisualSettings initial = CreateSettings();
        SystemVisualSettings next = CreateSettings(
            highContrastEnabled: true,
            clientAreaAnimationEnabled: false);
        SystemVisualSettingsChangedEventHandler? applicationHandler = null;

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial, () => next);
            using Form first = new();
            using Form second = new();
            using Control firstChild = new();
            using Control secondChild = new();
            first.Controls.Add(firstChild);
            second.Controls.Add(secondChild);
            _ = first.Handle;
            _ = second.Handle;

            int applicationCallCount = 0;
            int firstTreeCallCount = 0;
            int secondTreeCallCount = 0;
            int owningThreadId = Environment.CurrentManagedThreadId;

            applicationHandler = (sender, e) =>
            {
                Assert.Null(sender);
                Assert.Equal(
                    SystemVisualSettingsCategories.HighContrast | SystemVisualSettingsCategories.ClientAreaAnimations,
                    e.Changed);
                applicationCallCount++;
            };
            Application.SystemVisualSettingsChanged += applicationHandler;

            firstChild.SystemVisualSettingsChanged += (sender, e) =>
            {
                Assert.Same(firstChild, sender);
                Assert.Equal(owningThreadId, Environment.CurrentManagedThreadId);
                firstTreeCallCount++;
            };
            secondChild.SystemVisualSettingsChanged += (sender, e) =>
            {
                Assert.Same(secondChild, sender);
                Assert.Equal(owningThreadId, Environment.CurrentManagedThreadId);
                secondTreeCallCount++;
            };

            first.ProcessSystemVisualSettingsChange();
            second.ProcessSystemVisualSettingsChange();

            Assert.Equal(1, applicationCallCount);
            Assert.Equal(1, firstTreeCallCount);
            Assert.Equal(1, secondTreeCallCount);
        }
        finally
        {
            if (applicationHandler is not null)
            {
                Application.SystemVisualSettingsChanged -= applicationHandler;
            }

            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void SystemVisualSettingsTracker_RawMessagesAreCoalescedForTopLevel()
    {
        SystemVisualSettings initial = CreateSettings();
        SystemVisualSettings next = CreateSettings(accentColor: Color.OrangeRed);
        SystemVisualSettingsChangedEventHandler? applicationHandler = null;

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial, () => next);
            using SubSystemVisualSettingsForm form = new();
            _ = form.Handle;
            int applicationCallCount = 0;
            int controlCallCount = 0;
            applicationHandler = (sender, e) => applicationCallCount++;
            Application.SystemVisualSettingsChanged += applicationHandler;
            form.SystemVisualSettingsChanged += (sender, e) => controlCallCount++;

            form.Dispatch(PInvokeCore.WM_SETTINGCHANGE);
            form.Dispatch(PInvokeCore.WM_THEMECHANGED);
            form.Dispatch(WmDwmColorizationColorChanged);
            form.Dispatch(PInvokeCore.WM_SYSCOLORCHANGE);
            Application.DoEvents();

            Assert.Equal(1, applicationCallCount);
            Assert.Equal(1, controlCallCount);
        }
        finally
        {
            if (applicationHandler is not null)
            {
                Application.SystemVisualSettingsChanged -= applicationHandler;
            }

            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void SystemVisualSettingsTracker_ProcessTopLevelTree_DeliversOnItsOwningUiThread()
    {
        SystemVisualSettings initial = CreateSettings();
        SystemVisualSettings next = CreateSettings(keyboardCuesVisible: true);
        SystemVisualSettingsChangedEventHandler? applicationHandler = null;

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial, () => next);
            using UiThreadTree tree = new();
            int applicationCallCount = 0;
            applicationHandler = (sender, e) => applicationCallCount++;
            Application.SystemVisualSettingsChanged += applicationHandler;

            tree.ProcessSystemVisualSettingsChange();

            Assert.True(tree.WaitForDelivery(TimeSpan.FromSeconds(10)));
            Assert.Equal(tree.ThreadId, tree.DeliveryThreadId);
            Assert.Equal(1, applicationCallCount);
        }
        finally
        {
            if (applicationHandler is not null)
            {
                Application.SystemVisualSettingsChanged -= applicationHandler;
            }

            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void SystemVisualSettingsTracker_MultipleUiThreads_DeduplicatesApplicationAndDeliversToEveryTree()
    {
        SystemVisualSettings initial = CreateSettings();
        SystemVisualSettings next = CreateSettings(
            accentColor: Color.MediumOrchid,
            keyboardCuesVisible: true);
        SystemVisualSettingsChangedEventHandler? applicationHandler = null;

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial, () => next);
            using UiThreadTree first = new();
            using UiThreadTree second = new();
            int applicationCallCount = 0;
            applicationHandler = (sender, e) => Interlocked.Increment(ref applicationCallCount);
            Application.SystemVisualSettingsChanged += applicationHandler;

            first.ProcessSystemVisualSettingsChange();
            second.ProcessSystemVisualSettingsChange();

            Assert.True(first.WaitForDelivery(TimeSpan.FromSeconds(10)));
            Assert.True(second.WaitForDelivery(TimeSpan.FromSeconds(10)));
            Assert.NotEqual(first.ThreadId, second.ThreadId);
            Assert.Equal(first.ThreadId, first.DeliveryThreadId);
            Assert.Equal(second.ThreadId, second.DeliveryThreadId);
            Assert.Equal(1, first.DeliveryCount);
            Assert.Equal(1, second.DeliveryCount);
            Assert.Equal(1, Volatile.Read(ref applicationCallCount));
        }
        finally
        {
            if (applicationHandler is not null)
            {
                Application.SystemVisualSettingsChanged -= applicationHandler;
            }

            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void Control_SystemVisualSettingsChanged_HandleAndParentResynchronizationDoesNotRaiseSyntheticTransition()
    {
        SystemVisualSettings initial = CreateSettings();
        SystemVisualSettings next = CreateSettings(accentColor: Color.DarkOrange);

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial, () => next);
            SystemVisualSettingsTracker.Refresh();

            using Form parent = new();
            using Control detachedControl = new();
            int callCount = 0;
            detachedControl.SystemVisualSettingsChanged += (sender, e) => callCount++;

            parent.Controls.Add(detachedControl);
            _ = parent.Handle;
            parent.ProcessSystemVisualSettingsChange();

            Assert.Equal(0, callCount);
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting();
        }
    }

    [WinFormsFact]
    public void Control_SystemVisualSettingsChanged_InstanceSubscriptionDoesNotRootControl()
    {
        WeakReference[] weakReferences = Enumerable.Range(0, 10)
            .Select(_ => CreateDisposedFormWithInstanceSubscription())
            .ToArray();

        CollectGarbage();

        Assert.All(weakReferences, weakReference => Assert.False(weakReference.IsAlive));
    }

    [WinFormsFact]
    public void Application_SystemVisualSettingsChanged_StaticSubscriptionRootsSubscriberUntilRemoved()
    {
        (WeakReference subscriberReference, WeakReference<SystemVisualSettingsChangedEventHandler> handlerReference)
            = CreateStaticSubscription();

        CollectGarbage();

        Assert.True(subscriberReference.IsAlive);

        RemoveStaticSubscription(handlerReference);
        CollectGarbage();

        Assert.False(subscriberReference.IsAlive);
    }

    private static SystemVisualSettings CreateSettings(
        Color? accentColor = null,
        float textScaleFactor = 1.0f,
        bool highContrastEnabled = false,
        bool clientAreaAnimationEnabled = true,
        bool keyboardCuesVisible = false,
        Size? focusBorderMetrics = null)
        => new(
            accentColor ?? Color.DodgerBlue,
            textScaleFactor,
            highContrastEnabled,
            clientAreaAnimationEnabled,
            keyboardCuesVisible,
            focusBorderMetrics ?? new Size(1, 1));

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateDisposedFormWithInstanceSubscription()
    {
        SubSystemVisualSettingsForm form = new();
        form.SystemVisualSettingsChanged += form.OnInstanceSystemVisualSettingsChanged;
        _ = form.Handle;
        form.Dispose();

        return new WeakReference(form);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (
        WeakReference SubscriberReference,
        WeakReference<SystemVisualSettingsChangedEventHandler> HandlerReference)
        CreateStaticSubscription()
    {
        StaticSubscriber subscriber = new();
        SystemVisualSettingsChangedEventHandler handler = subscriber.OnSystemVisualSettingsChanged;
        Application.SystemVisualSettingsChanged += handler;

        return (new WeakReference(subscriber), new WeakReference<SystemVisualSettingsChangedEventHandler>(handler));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RemoveStaticSubscription(
        WeakReference<SystemVisualSettingsChangedEventHandler> handlerReference)
    {
        Assert.True(handlerReference.TryGetTarget(out SystemVisualSettingsChangedEventHandler? handler));
        Application.SystemVisualSettingsChanged -= handler;
    }

    private static void CollectGarbage()
    {
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
    }

    private sealed class StaticSubscriber
    {
        public void OnSystemVisualSettingsChanged(object? sender, SystemVisualSettingsChangedEventArgs e)
        {
        }
    }

    private sealed class SubSystemVisualSettingsControl : Control
    {
        public void RaiseSystemVisualSettingsChanged(SystemVisualSettingsChangedEventArgs e)
            => base.OnSystemVisualSettingsChanged(e);

        public void OnInstanceSystemVisualSettingsChanged(object? sender, SystemVisualSettingsChangedEventArgs e)
        {
        }
    }

    private sealed class SettingsAnimationRenderer : AnimatedControlRenderer
    {
        public SettingsAnimationRenderer(Control control) : base(control)
        {
        }

        public Color AccentColor => WindowsAccentColor;

        public int EndCount { get; private set; }

        public float LastProgress { get; private set; }

        public int StartCount { get; private set; }

        public override void AnimationProc(float animationProgress)
        {
            LastProgress = animationProgress;
            base.AnimationProc(animationProgress);
        }

        public override void RenderControl(Graphics graphics)
        {
        }

        protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
        {
            StartCount++;
            return (100, AnimationCycle.Once);
        }

        protected override void OnAnimationEnded() => EndCount++;

        protected override void OnAnimationStopped()
        {
        }
    }

    private sealed class VisualStylesImpactForm : Form
    {
        public int VisualStylesTransitionCount { get; private set; }

        public void RaiseSystemVisualSettingsChanged(SystemVisualSettingsChangedEventArgs e)
            => base.OnSystemVisualSettingsChanged(e);

        public void ResetVisualStylesTransitionCount()
            => VisualStylesTransitionCount = 0;

        protected override VisualStylesModeChangeImpact GetVisualStylesModeChangeImpact(
            VisualStylesMode oldMode,
            VisualStylesMode newMode)
            => VisualStylesModeChangeImpact.Metrics;

        protected override void OnVisualStylesModeChanged(EventArgs e)
        {
            VisualStylesTransitionCount++;
            base.OnVisualStylesModeChanged(e);
        }
    }

    private sealed class VisualStylesImpactPanel : Panel
    {
        public int VisualStylesTransitionCount { get; private set; }

        public void ResetVisualStylesTransitionCount()
            => VisualStylesTransitionCount = 0;

        protected override VisualStylesModeChangeImpact GetVisualStylesModeChangeImpact(
            VisualStylesMode oldMode,
            VisualStylesMode newMode)
            => VisualStylesModeChangeImpact.Metrics;

        protected override void OnVisualStylesModeChanged(EventArgs e)
        {
            VisualStylesTransitionCount++;
            base.OnVisualStylesModeChanged(e);
        }
    }

    private sealed class SubSystemVisualSettingsForm : Form
    {
        public void OnInstanceSystemVisualSettingsChanged(
            object? sender,
            SystemVisualSettingsChangedEventArgs e)
        {
        }

        internal void Dispatch(uint message)
        {
            Message messageToDispatch = Message.Create(Handle, (int)message, IntPtr.Zero, IntPtr.Zero);
            base.WndProc(ref messageToDispatch);
        }

        internal void Dispatch(int message)
        {
            Message messageToDispatch = Message.Create(Handle, message, IntPtr.Zero, IntPtr.Zero);
            base.WndProc(ref messageToDispatch);
        }
    }

    private sealed class UiThreadTree : IDisposable
    {
        private readonly ManualResetEventSlim _ready = new(initialState: false);
        private readonly ManualResetEventSlim _delivered = new(initialState: false);
        private readonly Thread _thread;
        private Exception? _exception;
        private Form? _form;

        internal UiThreadTree()
        {
            _thread = new(ThreadProc)
            {
                IsBackground = true
            };
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();

            if (!_ready.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new TimeoutException("The UI thread did not start.");
            }

            if (_exception is not null)
            {
                throw new InvalidOperationException("The UI thread failed to start.", _exception);
            }
        }

        internal int ThreadId { get; private set; }

        internal int DeliveryThreadId { get; private set; }

        internal int DeliveryCount { get; private set; }

        internal void ProcessSystemVisualSettingsChange()
            => _form!.BeginInvoke((Action)_form.ProcessSystemVisualSettingsChange);

        internal bool WaitForDelivery(TimeSpan timeout)
            => _delivered.Wait(timeout);

        public void Dispose()
        {
            if (_form is { IsDisposed: false } form)
            {
                form.BeginInvoke((Action)form.Close);
            }

            _thread.Join(TimeSpan.FromSeconds(10));
            _ready.Dispose();
            _delivered.Dispose();
        }

        private void ThreadProc()
        {
            try
            {
                using Form form = new();
                using Control child = new();
                _form = form;
                form.Controls.Add(child);
                child.SystemVisualSettingsChanged += OnSystemVisualSettingsChanged;
                _ = form.Handle;
                ThreadId = Environment.CurrentManagedThreadId;
                _ready.Set();
                Application.Run(form);
            }
            catch (Exception exception)
            {
                _exception = exception;
                _ready.Set();
            }
        }

        private void OnSystemVisualSettingsChanged(object? sender, SystemVisualSettingsChangedEventArgs e)
        {
            DeliveryThreadId = Environment.CurrentManagedThreadId;
            DeliveryCount++;
            _delivered.Set();
        }
    }
}
