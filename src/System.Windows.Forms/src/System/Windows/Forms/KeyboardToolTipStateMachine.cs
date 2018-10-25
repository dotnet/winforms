//------------------------------------------------------------------------------
// <copyright file="KeyboardToolTipStateMachine.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


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
namespace System.Windows.Forms {
    using Runtime.CompilerServices;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    internal sealed class KeyboardToolTipStateMachine {

        public static KeyboardToolTipStateMachine Instance {
            get {
                if (KeyboardToolTipStateMachine.instance == null) {
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

        private KeyboardToolTipStateMachine() {
            this.transitions = new Dictionary<SmTransition, Func<IKeyboardToolTip, ToolTip, SmState>> {
                [new SmTransition(SmState.Hidden, SmEvent.FocusedTool)] = this.SetupInitShowTimer,
                [new SmTransition(SmState.Hidden, SmEvent.LeftTool)] = this.DoNothing, // OK

                [new SmTransition(SmState.ReadyForInitShow, SmEvent.FocusedTool)] = this.DoNothing, // unlikely: focus without leave
                [new SmTransition(SmState.ReadyForInitShow, SmEvent.LeftTool)] = this.ResetFsmToHidden,
                [new SmTransition(SmState.ReadyForInitShow, SmEvent.InitialDelayTimerExpired)] = this.ShowToolTip,

                [new SmTransition(SmState.Shown, SmEvent.FocusedTool)] = this.DoNothing, // unlikely: focus without leave
                [new SmTransition(SmState.Shown, SmEvent.LeftTool)] = this.HideAndStartWaitingForRefocus,
                [new SmTransition(SmState.Shown, SmEvent.AutoPopupDelayTimerExpired)] = this.ResetFsmToHidden,

                [new SmTransition(SmState.WaitForRefocus, SmEvent.FocusedTool)] = this.SetupReshowTimer,
                [new SmTransition(SmState.WaitForRefocus, SmEvent.LeftTool)] = this.DoNothing, // OK
                [new SmTransition(SmState.WaitForRefocus, SmEvent.RefocusWaitDelayExpired)] = this.ResetFsmToHidden,

                [new SmTransition(SmState.ReadyForReshow, SmEvent.FocusedTool)] = this.DoNothing, // unlikely: focus without leave
                [new SmTransition(SmState.ReadyForReshow, SmEvent.LeftTool)] = this.StartWaitingForRefocus,
                [new SmTransition(SmState.ReadyForReshow, SmEvent.ReshowDelayTimerExpired)] = this.ShowToolTip
            };
        }

        public void ResetStateMachine(ToolTip toolTip) {
            this.Reset(toolTip);
        }

        public void Hook(IKeyboardToolTip tool, ToolTip toolTip) {
            if (tool.AllowsToolTip()) {
                this.StartTracking(tool, toolTip);
                tool.OnHooked(toolTip);
            }
        }

        public void NotifyAboutMouseEnter(IKeyboardToolTip sender) {
            if (this.IsToolTracked(sender) && sender.ShowsOwnToolTip()) {
                this.Reset(null);
            }
        }

        private bool IsToolTracked(IKeyboardToolTip sender) {
            return this.toolToTip[sender] != null;
        }

        public void NotifyAboutLostFocus(IKeyboardToolTip sender) {
            if (this.IsToolTracked(sender) && sender.ShowsOwnToolTip()) {
                this.Transit(SmEvent.LeftTool, sender);
                if (this.currentTool == null) {
                    this.lastFocusedTool.SetTarget(null);
                }
            }
        }

        public void NotifyAboutGotFocus(IKeyboardToolTip sender) {
            if (this.IsToolTracked(sender) && sender.ShowsOwnToolTip() && sender.IsBeingTabbedTo()) {
                this.Transit(SmEvent.FocusedTool, sender);
                if (this.currentTool == sender) {
                    this.lastFocusedTool.SetTarget(sender);
                }
            }
        }

        public void Unhook(IKeyboardToolTip tool, ToolTip toolTip) {
            if (tool.AllowsToolTip()) {
                this.StopTracking(tool, toolTip);
                tool.OnUnhooked(toolTip);
            }
        }

        public void NotifyAboutFormDeactivation(ToolTip sender) {
            this.OnFormDeactivation(sender);
        }


        internal IKeyboardToolTip LastFocusedTool {
            get {
                IKeyboardToolTip tool;
                if(this.lastFocusedTool.TryGetTarget(out tool)) {
                    return tool;
                }

                return Control.FromHandleInternal(UnsafeNativeMethods.GetFocus());
            }
        }

        private SmState HideAndStartWaitingForRefocus(IKeyboardToolTip tool, ToolTip toolTip) {
            toolTip.HideToolTip(this.currentTool);
            return StartWaitingForRefocus(tool, toolTip);
        }

        private SmState StartWaitingForRefocus(IKeyboardToolTip tool, ToolTip toolTip) {
            this.ResetTimer();
            this.currentTool = null;
            SendOrPostCallback expirationCallback = null;
            this.refocusDelayExpirationCallback = expirationCallback = (object toolObject) => {
                if (this.currentState == SmState.WaitForRefocus && this.refocusDelayExpirationCallback == expirationCallback) {
                    this.Transit(SmEvent.RefocusWaitDelayExpired, (IKeyboardToolTip)toolObject);
                }
            };
            WindowsFormsSynchronizationContext.Current.Post(expirationCallback, tool);
            return SmState.WaitForRefocus;
        }

        private SmState SetupReshowTimer(IKeyboardToolTip tool, ToolTip toolTip) {
            this.currentTool = tool;
            this.ResetTimer();
            this.StartTimer(toolTip.GetDelayTime(NativeMethods.TTDT_RESHOW), this.GetOneRunTickHandler((Timer sender) => this.Transit(SmEvent.ReshowDelayTimerExpired, tool)));
            return SmState.ReadyForReshow;
        }

        private SmState ShowToolTip(IKeyboardToolTip tool, ToolTip toolTip) {
            string toolTipText = tool.GetCaptionForTool(toolTip);
            int autoPopDelay = toolTip.GetDelayTime(NativeMethods.TTDT_AUTOPOP);
            if (!this.currentTool.IsHoveredWithMouse()) {
                toolTip.ShowKeyboardToolTip(toolTipText, this.currentTool, autoPopDelay);
            }
            this.StartTimer(autoPopDelay, this.GetOneRunTickHandler((Timer sender) => this.Transit(SmEvent.AutoPopupDelayTimerExpired, this.currentTool)));
            return SmState.Shown;
        }

        private SmState ResetFsmToHidden(IKeyboardToolTip tool, ToolTip toolTip) {
            return this.FullFsmReset();
        }

        private SmState DoNothing(IKeyboardToolTip tool, ToolTip toolTip) {
            return this.currentState;
        }

        private SmState SetupInitShowTimer(IKeyboardToolTip tool, ToolTip toolTip) {
            this.currentTool = tool;
            this.ResetTimer();
            this.StartTimer(toolTip.GetDelayTime(NativeMethods.TTDT_INITIAL), this.GetOneRunTickHandler((Timer sender) => this.Transit(SmEvent.InitialDelayTimerExpired, this.currentTool)));

            return SmState.ReadyForInitShow;
        }

        private void StartTimer(int interval, EventHandler eventHandler) {
            this.timer.Interval = interval;
            this.timer.Tick += eventHandler;
            this.timer.Start();
        }

        private EventHandler GetOneRunTickHandler(Action<Timer> handler) {
            EventHandler wrapper = null;
            wrapper = (object sender, EventArgs eventArgs) => {
                this.timer.Stop();
                this.timer.Tick -= wrapper;
                handler(this.timer);
            };
            return wrapper;
        }

        private void Transit(SmEvent @event, IKeyboardToolTip source) {
            Debug.Assert(transitions.ContainsKey(new SmTransition(this.currentState, @event)), "Unsupported KeyboardToolTipFsmTransition!");
            bool fullFsmResetRequired = false;
            try {
                ToolTip toolTip = this.toolToTip[source];
                if ((this.currentTool == null || this.currentTool.CanShowToolTipsNow()) && toolTip != null) {
                    Func<IKeyboardToolTip, ToolTip, SmState> transitionFunction = transitions[new SmTransition(this.currentState, @event)];
                    this.currentState = transitionFunction(source, toolTip);
                }
                else {
                    fullFsmResetRequired = true;
                }
            }
            catch {
                fullFsmResetRequired = true;
                throw;
            }
            finally {
                if (fullFsmResetRequired) {
                    this.FullFsmReset();
                }
            }
        }

        private SmState FullFsmReset() {
            if (this.currentState == SmState.Shown && this.currentTool != null) {
                ToolTip currentToolTip = this.toolToTip[this.currentTool];
                if (currentToolTip != null) {
                    currentToolTip.HideToolTip(this.currentTool);
                }
            }
            this.ResetTimer();
            this.currentTool = null;
            return this.currentState = SmState.Hidden;
        }

        private void ResetTimer() {
            this.timer.ClearTimerTickHandlers();
            this.timer.Stop();
        }

        private void Reset(ToolTip toolTipToReset) {
            if (toolTipToReset == null || (this.currentTool != null && this.toolToTip[this.currentTool] == toolTipToReset)) {
                this.FullFsmReset();
            }
        }

        private void StartTracking(IKeyboardToolTip tool, ToolTip toolTip) {
            this.toolToTip[tool] = toolTip;
        }

        private void StopTracking(IKeyboardToolTip tool, ToolTip toolTip) {
            this.toolToTip.Remove(tool, toolTip);
        }

        private void OnFormDeactivation(ToolTip sender) {
            if (this.currentTool != null && this.toolToTip[this.currentTool] == sender) {
                this.FullFsmReset();
            }
        }

        private enum SmEvent : byte {
            FocusedTool,
            LeftTool,
            InitialDelayTimerExpired, // internal
            ReshowDelayTimerExpired, // internal
            AutoPopupDelayTimerExpired, // internal
            RefocusWaitDelayExpired // internal
        }

        private enum SmState : byte {
            Hidden,
            ReadyForInitShow,
            Shown,
            ReadyForReshow,
            WaitForRefocus
        }

        private struct SmTransition : IEquatable<SmTransition> {
            private readonly SmState currentState;
            private readonly SmEvent @event;

            public SmTransition(SmState currentState, SmEvent @event) {
                this.currentState = currentState;
                this.@event = @event;
            }

            public bool Equals(SmTransition other) {
                return this.currentState == other.currentState && this.@event == other.@event;
            }

            public override bool Equals(object obj) {
                return obj is SmTransition && this.Equals((SmTransition)obj);
            }

            public override int GetHashCode() {
                return (byte)this.currentState << 16 | (byte)this.@event;
            }
        }

        private sealed class InternalStateMachineTimer : Timer {
            public void ClearTimerTickHandlers() {
                this.onTimer = null;
            }
        }

        private sealed class ToolToTipDictionary {
            private ConditionalWeakTable<IKeyboardToolTip, WeakReference<ToolTip>> table = new ConditionalWeakTable<IKeyboardToolTip, WeakReference<ToolTip>>();

            public ToolTip this[IKeyboardToolTip tool] {
                get {
                    WeakReference<ToolTip> toolTipReference;
                    ToolTip toolTip = null;
                    if (this.table.TryGetValue(tool, out toolTipReference)) {
                        if (!toolTipReference.TryGetTarget(out toolTip)) {
                            // removing dead reference
                            this.table.Remove(tool);
                        }
                    }
                    return toolTip;
                }

                set {
                    WeakReference<ToolTip> toolTipReference;
                    if (this.table.TryGetValue(tool, out toolTipReference)) {
                        toolTipReference.SetTarget(value);
                    }
                    else {
                        this.table.Add(tool, new WeakReference<ToolTip>(value));
                    }
                }
            }

            public void Remove(IKeyboardToolTip tool, ToolTip toolTip) {
                WeakReference<ToolTip> toolTipReference;
                ToolTip existingToolTip;
                if (this.table.TryGetValue(tool, out toolTipReference)) {
                    if (toolTipReference.TryGetTarget(out existingToolTip)) {
                        if (existingToolTip == toolTip) {
                            this.table.Remove(tool);
                        }
                    }
                    else {
                        // removing dead reference
                        this.table.Remove(tool);
                    }
                }
            }
        }
    }
}
