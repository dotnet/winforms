// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


/*
 * KeyboardToolTipStateMachine implements keyboard ToolTips for controls with ToolTip set on them. 
 * 
 * A keyboard ToolTip is shown when a user focuses a control using keyboard keys such as Tab, arrow keys etc.
 * This state machine attempts to simulate the mouse ToolTip behavior.
 * The control should be focused with keyboard for an amount of time specified with TTDT_INITIAL flag to make the keyboard ToolTip appear.
 * Once visible, the keyboard ToolTip will be demonstrated for an amount of time specified with TTDT_AUTOPOP flag. The ToolTip will disappear afterwards.
 * If the keyboard ToolTip is visible and the focus moves from one ToolTip-enabled control to another, the keyboard ToolTip will be shown after a time interval specified with TTDT_RESHOW flag has passed.
 * 
 * This behavior is disabled by default and can be enabled by adding the "Switch.System.Windows.Forms.UseLegacyToolTipDisplay=false" line to an application's App.config file:
 
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
    <!-- AppContextSwitchOverrides values are in the form of 'key1=true|false;key2=true|false  -->
    <!-- Enabling newer accessibility features (e.g. UseLegacyAccessibilityFeatures.2=false) requires all older accessibility features to be enabled (e.g. UseLegacyAccessibilityFeatures=false) -->
    <AppContextSwitchOverrides value="Switch.UseLegacyAccessibilityFeatures=false;Switch.UseLegacyAccessibilityFeatures.2=false;Switch.UseLegacyAccessibilityFeatures.3=false;Switch.System.Windows.Forms.UseLegacyToolTipDisplay=false"/>
  </runtime>
</configuration>

 * Please note that disabling Switch.UseLegacyAccessibilityFeatures, Switch.UseLegacyAccessibilityFeatures.2 and Switch.UseLegacyAccessibilityFeatures.3 is required to disable Switch.System.Windows.Forms.UseLegacyToolTipDisplay
 */
namespace System.Windows.Forms
{
    using Runtime.CompilerServices;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    internal sealed class KeyboardToolTipStateMachine
    {

        public static KeyboardToolTipStateMachine Instance
        {
            get
            {
                if (KeyboardToolTipStateMachine.instance == null)
                {
                    KeyboardToolTipStateMachine.instance = new KeyboardToolTipStateMachine();
                }
                return KeyboardToolTipStateMachine.instance;
            }
        }

        [ThreadStatic]
        private static KeyboardToolTipStateMachine instance;

        private readonly Dictionary<SmTransition, Func<IKeyboardToolTip, ToolTip, SmState>> transitions;
        private readonly ToolToTipDictionary toolToTip = new ToolToTipDictionary();

        private SmState currentState = SmState.Hidden;
        private IKeyboardToolTip currentTool;
        private readonly InternalStateMachineTimer timer = new InternalStateMachineTimer();
        private SendOrPostCallback refocusDelayExpirationCallback;

        private readonly WeakReference<IKeyboardToolTip> lastFocusedTool = new WeakReference<IKeyboardToolTip>(null);

        private KeyboardToolTipStateMachine()
        {
            transitions = new Dictionary<SmTransition, Func<IKeyboardToolTip, ToolTip, SmState>>
            {
                [new SmTransition(SmState.Hidden, SmEvent.FocusedTool)] = SetupInitShowTimer,
                [new SmTransition(SmState.Hidden, SmEvent.LeftTool)] = DoNothing, // OK

                [new SmTransition(SmState.ReadyForInitShow, SmEvent.FocusedTool)] = DoNothing, // unlikely: focus without leave
                [new SmTransition(SmState.ReadyForInitShow, SmEvent.LeftTool)] = ResetFsmToHidden,
                [new SmTransition(SmState.ReadyForInitShow, SmEvent.InitialDelayTimerExpired)] = ShowToolTip,

                [new SmTransition(SmState.Shown, SmEvent.FocusedTool)] = DoNothing, // unlikely: focus without leave
                [new SmTransition(SmState.Shown, SmEvent.LeftTool)] = HideAndStartWaitingForRefocus,
                [new SmTransition(SmState.Shown, SmEvent.AutoPopupDelayTimerExpired)] = ResetFsmToHidden,

                [new SmTransition(SmState.WaitForRefocus, SmEvent.FocusedTool)] = SetupReshowTimer,
                [new SmTransition(SmState.WaitForRefocus, SmEvent.LeftTool)] = DoNothing, // OK
                [new SmTransition(SmState.WaitForRefocus, SmEvent.RefocusWaitDelayExpired)] = ResetFsmToHidden,

                [new SmTransition(SmState.ReadyForReshow, SmEvent.FocusedTool)] = DoNothing, // unlikely: focus without leave
                [new SmTransition(SmState.ReadyForReshow, SmEvent.LeftTool)] = StartWaitingForRefocus,
                [new SmTransition(SmState.ReadyForReshow, SmEvent.ReshowDelayTimerExpired)] = ShowToolTip
            };
        }

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
            return toolToTip[sender] != null;
        }

        public void NotifyAboutLostFocus(IKeyboardToolTip sender)
        {
            if (IsToolTracked(sender) && sender.ShowsOwnToolTip())
            {
                Transit(SmEvent.LeftTool, sender);
                if (currentTool == null)
                {
                    lastFocusedTool.SetTarget(null);
                }
            }
        }

        public void NotifyAboutGotFocus(IKeyboardToolTip sender)
        {
            if (IsToolTracked(sender) && sender.ShowsOwnToolTip() && sender.IsBeingTabbedTo())
            {
                Transit(SmEvent.FocusedTool, sender);
                if (currentTool == sender)
                {
                    lastFocusedTool.SetTarget(sender);
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


        internal IKeyboardToolTip LastFocusedTool
        {
            get
            {
                if (lastFocusedTool.TryGetTarget(out IKeyboardToolTip tool))
                {
                    return tool;
                }

                return Control.FromHandle(UnsafeNativeMethods.GetFocus());
            }
        }

        private SmState HideAndStartWaitingForRefocus(IKeyboardToolTip tool, ToolTip toolTip)
        {
            toolTip.HideToolTip(currentTool);
            return StartWaitingForRefocus(tool, toolTip);
        }

        private SmState StartWaitingForRefocus(IKeyboardToolTip tool, ToolTip toolTip)
        {
            ResetTimer();
            currentTool = null;
            SendOrPostCallback expirationCallback = null;
            refocusDelayExpirationCallback = expirationCallback = (object toolObject) =>
            {
                if (currentState == SmState.WaitForRefocus && refocusDelayExpirationCallback == expirationCallback)
                {
                    Transit(SmEvent.RefocusWaitDelayExpired, (IKeyboardToolTip)toolObject);
                }
            };
            WindowsFormsSynchronizationContext.Current.Post(expirationCallback, tool);
            return SmState.WaitForRefocus;
        }

        private SmState SetupReshowTimer(IKeyboardToolTip tool, ToolTip toolTip)
        {
            currentTool = tool;
            ResetTimer();
            StartTimer(toolTip.GetDelayTime(NativeMethods.TTDT_RESHOW), GetOneRunTickHandler((Timer sender) => Transit(SmEvent.ReshowDelayTimerExpired, tool)));
            return SmState.ReadyForReshow;
        }

        private SmState ShowToolTip(IKeyboardToolTip tool, ToolTip toolTip)
        {
            string toolTipText = tool.GetCaptionForTool(toolTip);
            int autoPopDelay = toolTip.GetDelayTime(NativeMethods.TTDT_AUTOPOP);
            if (!currentTool.IsHoveredWithMouse())
            {
                toolTip.ShowKeyboardToolTip(toolTipText, currentTool, autoPopDelay);
            }
            StartTimer(autoPopDelay, GetOneRunTickHandler((Timer sender) => Transit(SmEvent.AutoPopupDelayTimerExpired, currentTool)));
            return SmState.Shown;
        }

        private SmState ResetFsmToHidden(IKeyboardToolTip tool, ToolTip toolTip)
        {
            return FullFsmReset();
        }

        private SmState DoNothing(IKeyboardToolTip tool, ToolTip toolTip)
        {
            return currentState;
        }

        private SmState SetupInitShowTimer(IKeyboardToolTip tool, ToolTip toolTip)
        {
            currentTool = tool;
            ResetTimer();
            StartTimer(toolTip.GetDelayTime(NativeMethods.TTDT_INITIAL), GetOneRunTickHandler((Timer sender) => Transit(SmEvent.InitialDelayTimerExpired, currentTool)));

            return SmState.ReadyForInitShow;
        }

        private void StartTimer(int interval, EventHandler eventHandler)
        {
            timer.Interval = interval;
            timer.Tick += eventHandler;
            timer.Start();
        }

        private EventHandler GetOneRunTickHandler(Action<Timer> handler)
        {
            EventHandler wrapper = null;
            wrapper = (object sender, EventArgs eventArgs) =>
            {
                timer.Stop();
                timer.Tick -= wrapper;
                handler(timer);
            };
            return wrapper;
        }

        private void Transit(SmEvent @event, IKeyboardToolTip source)
        {
            Debug.Assert(transitions.ContainsKey(new SmTransition(currentState, @event)), "Unsupported KeyboardToolTipFsmTransition!");
            bool fullFsmResetRequired = false;
            try
            {
                ToolTip toolTip = toolToTip[source];
                if ((currentTool == null || currentTool.CanShowToolTipsNow()) && toolTip != null)
                {
                    Func<IKeyboardToolTip, ToolTip, SmState> transitionFunction = transitions[new SmTransition(currentState, @event)];
                    currentState = transitionFunction(source, toolTip);
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

        private SmState FullFsmReset()
        {
            if (currentState == SmState.Shown && currentTool != null)
            {
                ToolTip currentToolTip = toolToTip[currentTool];
                if (currentToolTip != null)
                {
                    currentToolTip.HideToolTip(currentTool);
                }
            }
            ResetTimer();
            currentTool = null;
            return currentState = SmState.Hidden;
        }

        private void ResetTimer()
        {
            timer.ClearTimerTickHandlers();
            timer.Stop();
        }

        private void Reset(ToolTip toolTipToReset)
        {
            if (toolTipToReset == null || (currentTool != null && toolToTip[currentTool] == toolTipToReset))
            {
                FullFsmReset();
            }
        }

        private void StartTracking(IKeyboardToolTip tool, ToolTip toolTip)
        {
            toolToTip[tool] = toolTip;
        }

        private void StopTracking(IKeyboardToolTip tool, ToolTip toolTip)
        {
            toolToTip.Remove(tool, toolTip);
        }

        private void OnFormDeactivation(ToolTip sender)
        {
            if (currentTool != null && toolToTip[currentTool] == sender)
            {
                FullFsmReset();
            }
        }

        private enum SmEvent : byte
        {
            FocusedTool,
            LeftTool,
            InitialDelayTimerExpired, // internal
            ReshowDelayTimerExpired, // internal
            AutoPopupDelayTimerExpired, // internal
            RefocusWaitDelayExpired // internal
        }

        private enum SmState : byte
        {
            Hidden,
            ReadyForInitShow,
            Shown,
            ReadyForReshow,
            WaitForRefocus
        }

        private struct SmTransition : IEquatable<SmTransition>
        {
            private readonly SmState currentState;
            private readonly SmEvent @event;

            public SmTransition(SmState currentState, SmEvent @event)
            {
                this.currentState = currentState;
                this.@event = @event;
            }

            public bool Equals(SmTransition other)
            {
                return currentState == other.currentState && @event == other.@event;
            }

            public override bool Equals(object obj)
            {
                return obj is SmTransition && Equals((SmTransition)obj);
            }

            public override int GetHashCode()
            {
                return (byte)currentState << 16 | (byte)@event;
            }
        }

        private sealed class InternalStateMachineTimer : Timer
        {
            public void ClearTimerTickHandlers() => _onTimer = null;
        }

        private sealed class ToolToTipDictionary
        {
            private ConditionalWeakTable<IKeyboardToolTip, WeakReference<ToolTip>> table = new ConditionalWeakTable<IKeyboardToolTip, WeakReference<ToolTip>>();

            public ToolTip this[IKeyboardToolTip tool]
            {
                get
                {
                    ToolTip toolTip = null;
                    if (table.TryGetValue(tool, out WeakReference<ToolTip> toolTipReference))
                    {
                        if (!toolTipReference.TryGetTarget(out toolTip))
                        {
                            // removing dead reference
                            table.Remove(tool);
                        }
                    }
                    return toolTip;
                }

                set
                {
                    if (table.TryGetValue(tool, out WeakReference<ToolTip> toolTipReference))
                    {
                        toolTipReference.SetTarget(value);
                    }
                    else
                    {
                        table.Add(tool, new WeakReference<ToolTip>(value));
                    }
                }
            }

            public void Remove(IKeyboardToolTip tool, ToolTip toolTip)
            {
                if (table.TryGetValue(tool, out WeakReference<ToolTip> toolTipReference))
                {
                    if (toolTipReference.TryGetTarget(out ToolTip existingToolTip))
                    {
                        if (existingToolTip == toolTip)
                        {
                            table.Remove(tool);
                        }
                    }
                    else
                    {
                        // removing dead reference
                        table.Remove(tool);
                    }
                }
            }
        }
    }
}
