// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Implements keyboard ToolTips for controls with a <see cref="ToolTip"/> set on them.
///
///  A keyboard ToolTip is shown when a user focuses a control using keyboard keys such as Tab, arrow keys etc.
///  This state machine attempts to simulate the mouse ToolTip behavior.
/// </summary>
/// <remarks>
///  <para>
///   The control should be focused with keyboard for an amount of time specified with TTDT_INITIAL flag to make
///   the keyboard ToolTip appear.
///   <see href="https://docs.microsoft.com/windows/win32/controls/ttm-getdelaytime">
///    TTM_GETDELAYTIME message (Microsoft Docs)
///   </see>
///  </para>
///  <para>
///   Once visible, the keyboard ToolTip will be demonstrated for an amount of time specified with TTDT_AUTOPOP
///   flag. The ToolTip will disappear afterwards. If the keyboard ToolTip is visible and the focus moves from
///   one ToolTip-enabled control to another, the keyboard ToolTip will be shown after a time interval specified
///   with TTDT_RESHOW flag has passed.
///  </para>
/// </remarks>
internal sealed partial class KeyboardToolTipStateMachine
{
    public static KeyboardToolTipStateMachine Instance
    {
        get
        {
            s_instance ??= new KeyboardToolTipStateMachine();

            return s_instance;
        }
    }

    [ThreadStatic]
    private static KeyboardToolTipStateMachine? s_instance;

    private readonly ToolToTipDictionary _toolToTip = new();

    private SmState _currentState = SmState.Hidden;
    private IKeyboardToolTip? _currentTool;
    private readonly InternalStateMachineTimer _timer = new();
    private SendOrPostCallback? _refocusDelayExpirationCallback;

    private readonly WeakReference<IKeyboardToolTip?> _lastFocusedTool = new(null);

    private KeyboardToolTipStateMachine()
    {
    }

    private SmState Transition(IKeyboardToolTip tool, ToolTip tooltip, SmEvent @event)
        => (_currentState, @event) switch
        {
            (SmState.Hidden, SmEvent.FocusedTool) => SetupInitShowTimer(tool, tooltip),
            (SmState.Hidden, SmEvent.LeftTool) => _currentState, // OK
            (SmState.ReadyForInitShow, SmEvent.FocusedTool) => _currentState, // unlikely: focus without leave
            (SmState.ReadyForInitShow, SmEvent.LeftTool) => FullFsmReset(),
            (SmState.ReadyForInitShow, SmEvent.InitialDelayTimerExpired) => ShowToolTip(tool, tooltip),

            (SmState.Shown, SmEvent.FocusedTool) => _currentState, // unlikely: focus without leave
            (SmState.Shown, SmEvent.LeftTool) => HideAndStartWaitingForRefocus(tool, tooltip),
            (SmState.Shown, SmEvent.DismissTooltips) => FullFsmReset(),

            (SmState.WaitForRefocus, SmEvent.FocusedTool) => SetupReshowTimer(tool, tooltip),
            (SmState.WaitForRefocus, SmEvent.LeftTool) => _currentState, // OK
            (SmState.WaitForRefocus, SmEvent.RefocusWaitDelayExpired) => FullFsmReset(),

            (SmState.ReadyForReshow, SmEvent.FocusedTool) => _currentState, // unlikely: focus without leave
            (SmState.ReadyForReshow, SmEvent.LeftTool) => StartWaitingForRefocus(tool),
            (SmState.ReadyForReshow, SmEvent.ReshowDelayTimerExpired) => ShowToolTip(tool, tooltip),

            // This is what we would have thrown historically
            (_, _) => throw new KeyNotFoundException()
        };

    public void ResetStateMachine(ToolTip toolTip)
    {
        Reset(toolTip);
    }

    public void Hook(IKeyboardToolTip tool, ToolTip toolTip)
    {
        if (tool.AllowsToolTip())
        {
            StartTracking(tool, toolTip);
            tool.OnHooked(toolTip);
        }
    }

    public void NotifyAboutMouseEnter(IKeyboardToolTip sender)
    {
        if (IsToolTracked(sender) && sender.ShowsOwnToolTip())
        {
            Reset(null);
        }
    }

    private bool IsToolTracked(IKeyboardToolTip sender)
    {
        return _toolToTip[sender] is not null;
    }

    public void NotifyAboutLostFocus(IKeyboardToolTip sender)
    {
        if (IsToolTracked(sender) && sender.ShowsOwnToolTip())
        {
            Transit(SmEvent.LeftTool, sender);
            if (_currentTool is null)
            {
                _lastFocusedTool.SetTarget(null);
            }
        }
    }

    public void NotifyAboutGotFocus(IKeyboardToolTip sender)
    {
        if (IsToolTracked(sender) && sender.ShowsOwnToolTip() && sender.IsBeingTabbedTo())
        {
            Transit(SmEvent.FocusedTool, sender);
            if (_currentTool == sender)
            {
                _lastFocusedTool.SetTarget(sender);
            }
        }
    }

    public void Unhook(IKeyboardToolTip tool, ToolTip toolTip)
    {
        if (tool.AllowsToolTip())
        {
            StopTracking(tool, toolTip);
            tool.OnUnhooked(toolTip);
        }
    }

    public void NotifyAboutFormDeactivation(ToolTip sender)
    {
        OnFormDeactivation(sender);
    }

    internal IKeyboardToolTip? LastFocusedTool
    {
        get
        {
            if (_lastFocusedTool.TryGetTarget(out IKeyboardToolTip? tool))
            {
                return tool;
            }

            return Control.FromHandle(PInvoke.GetFocus());
        }
    }

    private SmState HideAndStartWaitingForRefocus(IKeyboardToolTip tool, ToolTip toolTip)
    {
        if (_currentTool is not null)
        {
            toolTip.HideToolTip(_currentTool);
        }

        return StartWaitingForRefocus(tool);
    }

    private SmState StartWaitingForRefocus(IKeyboardToolTip tool)
    {
        ResetTimer();
        _currentTool = null;
        SendOrPostCallback? expirationCallback = null;
        _refocusDelayExpirationCallback = expirationCallback = (object? toolObject) =>
        {
            if (toolObject is not null && _currentState == SmState.WaitForRefocus && _refocusDelayExpirationCallback == expirationCallback)
            {
                Transit(SmEvent.RefocusWaitDelayExpired, (IKeyboardToolTip)toolObject);
            }
        };
        SynchronizationContext.Current?.Post(expirationCallback, tool);
        return SmState.WaitForRefocus;
    }

    private SmState SetupReshowTimer(IKeyboardToolTip tool, ToolTip toolTip)
    {
        _currentTool = tool;
        ResetTimer();
        StartTimer(toolTip.GetDelayTime(PInvoke.TTDT_RESHOW),
            GetOneRunTickHandler((Timer sender) => Transit(SmEvent.ReshowDelayTimerExpired, tool)));
        return SmState.ReadyForReshow;
    }

    private SmState ShowToolTip(IKeyboardToolTip tool, ToolTip toolTip)
    {
        string? toolTipText = tool.GetCaptionForTool(toolTip);

        int autoPopDelay = toolTip.IsPersistent ?
            0 :
            toolTip.GetDelayTime(PInvoke.TTDT_AUTOPOP);

        if (_currentTool is null)
        {
            return SmState.Shown;
        }

        if (!_currentTool.IsHoveredWithMouse())
        {
            toolTip.ShowKeyboardToolTip(toolTipText, _currentTool, autoPopDelay);
        }

        if (!toolTip.IsPersistent)
        {
            StartTimer(
                autoPopDelay,
                GetOneRunTickHandler((Timer sender) => Transit(SmEvent.DismissTooltips, _currentTool)));
        }

        return SmState.Shown;
    }

    private SmState SetupInitShowTimer(IKeyboardToolTip tool, ToolTip toolTip)
    {
        _currentTool = tool;
        ResetTimer();
        StartTimer(toolTip.GetDelayTime(PInvoke.TTDT_INITIAL),
            GetOneRunTickHandler((Timer sender) => Transit(SmEvent.InitialDelayTimerExpired, _currentTool)));

        return SmState.ReadyForInitShow;
    }

    private void StartTimer(int interval, EventHandler eventHandler)
    {
        _timer.Interval = interval;
        _timer.Tick += eventHandler;
        _timer.Start();
    }

    private EventHandler GetOneRunTickHandler(Action<Timer> handler)
    {
        void wrapper(object? sender, EventArgs eventArgs)
        {
            _timer.Stop();
            _timer.Tick -= wrapper;
            handler(_timer);
        }

        return wrapper;
    }

    private void Transit(SmEvent @event, IKeyboardToolTip source)
    {
        bool fullFsmResetRequired = false;
        try
        {
            ToolTip? toolTip = _toolToTip[source];
            if ((_currentTool is null || _currentTool.CanShowToolTipsNow()) && toolTip is not null)
            {
                _currentState = Transition(source, toolTip, @event);
            }
            else
            {
                fullFsmResetRequired = true;
            }
        }
        catch
        {
            fullFsmResetRequired = true;
            throw;
        }
        finally
        {
            if (fullFsmResetRequired)
            {
                FullFsmReset();
            }
        }
    }

    internal static void HidePersistentTooltip() => s_instance?.HidePersistent();

    private void HidePersistent()
    {
        if (_currentState != SmState.Shown || _currentTool is null)
        {
            return;
        }

        ToolTip? currentToolTip = _toolToTip[_currentTool];

        // This test is required because typing is not dismissing non-persistent tooltips.
        if (currentToolTip?.IsPersistent == true)
        {
            currentToolTip.HideToolTip(_currentTool);
            _currentTool = null;
            _currentState = SmState.Hidden;
        }
    }

    private SmState FullFsmReset()
    {
        if (_currentState == SmState.Shown && _currentTool is not null)
        {
            ToolTip? currentToolTip = _toolToTip[_currentTool];
            currentToolTip?.HideToolTip(_currentTool);
        }

        ResetTimer();
        _currentTool = null;
        return _currentState = SmState.Hidden;
    }

    private void ResetTimer()
    {
        _timer.ClearTimerTickHandlers();
        _timer.Stop();
    }

    private void Reset(ToolTip? toolTipToReset)
    {
        if (toolTipToReset is null || (_currentTool is not null && _toolToTip[_currentTool] == toolTipToReset))
        {
            FullFsmReset();
        }
    }

    internal static void Reset() => s_instance?.FullFsmReset();

    private void StartTracking(IKeyboardToolTip tool, ToolTip toolTip)
    {
        _toolToTip[tool] = toolTip;
    }

    private void StopTracking(IKeyboardToolTip tool, ToolTip toolTip)
    {
        _toolToTip.Remove(tool, toolTip);
    }

    private void OnFormDeactivation(ToolTip sender)
    {
        if (_currentTool is not null && _toolToTip[_currentTool] == sender)
        {
            FullFsmReset();
        }
    }
}
