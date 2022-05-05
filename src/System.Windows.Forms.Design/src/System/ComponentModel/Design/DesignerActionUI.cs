﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  The DesignerActionUI is the designer/UI-specific implementation of the DesignerActions feature.  This class instantiates the DesignerActionService and hooks to its DesignerActionsChanged event.  Responding to this single event will enable the DesignerActionUI to perform all necessary UI-related operations. Note that the DesignerActionUI uses the BehaviorService to manage all UI interaction.  For every component containing a DesignerAction (determined by the DesignerActionsChanged event) there will be an associated  DesignerActionGlyph and DesignerActionBehavior. Finally, the DesignerActionUI is also responsible for showing and managing the Action's context menus.  Note that every DesignerAction context menu has an item that will bring up the DesignerActions option pane in the options dialog.
    /// </summary>
    internal class DesignerActionUI : IDisposable
    {
        private static readonly TraceSwitch s_designerActionPanelTraceSwitch = new TraceSwitch("DesignerActionPanelTrace", "DesignerActionPanel tracing");

        private Adorner _designerActionAdorner; //used to add designeraction-related glyphs
        private IServiceProvider _serviceProvider; //standard service provider
        private ISelectionService _selSvc; //used to determine if comps have selection or not
        private DesignerActionService _designerActionService; //this is how all designeractions will be managed
        private DesignerActionUIService _designerActionUIService; //this is how all designeractions UI elements will be managed
        private BehaviorService _behaviorService; //this is how all of our UI is implemented (glyphs, behaviors, etc...)
        private readonly IMenuCommandService _menuCommandService;
        private DesignerActionKeyboardBehavior _dapkb;   //out keyboard behavior
        private readonly Hashtable _componentToGlyph; //used for quick reference between components and our glyphs
        private Control _marshalingControl; //used to invoke events on our main gui thread
        private IComponent _lastPanelComponent;

        private readonly IUIService _uiService;
        private readonly IWin32Window _mainParentWindow;
        internal DesignerActionToolStripDropDown designerActionHost;

        private readonly MenuCommand _cmdShowDesignerActions; //used to respond to the Alt+Shift+F10 command
        private bool _inTransaction;
        private IComponent _relatedComponentTransaction;
        private DesignerActionGlyph _relatedGlyphTransaction;
        private readonly bool _disposeActionService;
        private readonly bool _disposeActionUIService;

        private delegate void ActionChangedEventHandler(object sender, DesignerActionListsChangedEventArgs e);
#if DEBUG
        internal static readonly TraceSwitch DropDownVisibilityDebug = new TraceSwitch("DropDownVisibilityDebug", "Debug ToolStrip Selection code");
#else
        internal static readonly TraceSwitch DropDownVisibilityDebug;
#endif
        /// <summary>
        ///  Constructor that takes a service provider.  This is needed to establish references to the BehaviorService and SelectionService, as well as spin-up the DesignerActionService.
        /// </summary>
        public DesignerActionUI(IServiceProvider serviceProvider, Adorner containerAdorner)
        {
            _serviceProvider = serviceProvider;
            _designerActionAdorner = containerAdorner;
            _behaviorService = (BehaviorService)serviceProvider.GetService(typeof(BehaviorService));
            _menuCommandService = (IMenuCommandService)serviceProvider.GetService(typeof(IMenuCommandService));
            _selSvc = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));
            if (_behaviorService is null || _selSvc is null)
            {
                Debug.Fail("Either BehaviorService or ISelectionService is null, cannot continue.");
                return;
            }

            //query for our DesignerActionService
            _designerActionService = (DesignerActionService)serviceProvider.GetService(typeof(DesignerActionService));
            if (_designerActionService is null)
            {
                //start the service
                _designerActionService = new DesignerActionService(serviceProvider);
                _disposeActionService = true;
            }

            _designerActionUIService = (DesignerActionUIService)serviceProvider.GetService(typeof(DesignerActionUIService));
            if (_designerActionUIService is null)
            {
                _designerActionUIService = new DesignerActionUIService(serviceProvider);
                _disposeActionUIService = true;
            }

            _designerActionUIService.DesignerActionUIStateChange += new DesignerActionUIStateChangeEventHandler(OnDesignerActionUIStateChange);
            _designerActionService.DesignerActionListsChanged += new DesignerActionListsChangedEventHandler(OnDesignerActionsChanged);
            _lastPanelComponent = null;

            IComponentChangeService cs = (IComponentChangeService)serviceProvider.GetService(typeof(IComponentChangeService));
            if (cs is not null)
            {
                cs.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
            }

            if (_menuCommandService is not null)
            {
                _cmdShowDesignerActions = new MenuCommand(new EventHandler(OnKeyShowDesignerActions), MenuCommands.KeyInvokeSmartTag);
                _menuCommandService.AddCommand(_cmdShowDesignerActions);
            }

            _uiService = (IUIService)serviceProvider.GetService(typeof(IUIService));
            if (_uiService is not null)
            {
                _mainParentWindow = _uiService.GetDialogOwnerWindow();
            }

            _componentToGlyph = new Hashtable();
            _marshalingControl = new Control();
            _marshalingControl.CreateControl();
        }

        /// <summary>
        ///  Disposes all UI-related objects and unhooks services.
        /// </summary>
        // Don't need to dispose of designerActionUIService.
        public void Dispose()
        {
            if (_marshalingControl is not null)
            {
                _marshalingControl.Dispose();
                _marshalingControl = null;
            }

            if (_serviceProvider is not null)
            {
                IComponentChangeService cs = (IComponentChangeService)_serviceProvider.GetService(typeof(IComponentChangeService));
                if (cs is not null)
                {
                    cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                }

                if (_cmdShowDesignerActions is not null)
                {
                    IMenuCommandService mcs = (IMenuCommandService)_serviceProvider.GetService(typeof(IMenuCommandService));
                    if (mcs is not null)
                    {
                        mcs.RemoveCommand(_cmdShowDesignerActions);
                    }
                }
            }

            _serviceProvider = null;
            _behaviorService = null;
            _selSvc = null;
            if (_designerActionService is not null)
            {
                _designerActionService.DesignerActionListsChanged -= new DesignerActionListsChangedEventHandler(OnDesignerActionsChanged);
                if (_disposeActionService)
                {
                    _designerActionService.Dispose();
                }
            }

            _designerActionService = null;

            if (_designerActionUIService is not null)
            {
                _designerActionUIService.DesignerActionUIStateChange -= new DesignerActionUIStateChangeEventHandler(OnDesignerActionUIStateChange);
                if (_disposeActionUIService)
                {
                    _designerActionUIService.Dispose();
                }
            }

            _designerActionUIService = null;
            _designerActionAdorner = null;
        }

        public DesignerActionGlyph GetDesignerActionGlyph(IComponent comp)
        {
            return GetDesignerActionGlyph(comp, null);
        }

        internal DesignerActionGlyph GetDesignerActionGlyph(IComponent comp, DesignerActionListCollection dalColl)
        {
            // check this component origin, this class or is it readonly because inherited...
            InheritanceAttribute attribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(comp)[typeof(InheritanceAttribute)];
            if (attribute == InheritanceAttribute.InheritedReadOnly)
            { // only do it if we can change the control...
                return null;
            }

            // we didnt get on, fetch it
            if (dalColl is null)
            {
                dalColl = _designerActionService.GetComponentActions(comp);
            }

            if (dalColl is not null && dalColl.Count > 0)
            {
                DesignerActionGlyph dag = null;
                if (_componentToGlyph[comp] is null)
                {
                    DesignerActionBehavior dab = new DesignerActionBehavior(_serviceProvider, comp, dalColl, this);

                    //if comp is a component then try to find a traycontrol associated with it... this should really be in ComponentTray but there is no behaviorService for the CT
                    if (!(comp is Control) || comp is ToolStripDropDown)
                    {
                        //Here, we'll try to get the traycontrol associated with the comp and supply the glyph with an alternative bounds
                        if (_serviceProvider.GetService(typeof(ComponentTray)) is ComponentTray compTray)
                        {
                            ComponentTray.TrayControl trayControl = ComponentTray.GetTrayControlFromComponent(comp);
                            if (trayControl is not null)
                            {
                                Rectangle trayBounds = trayControl.Bounds;
                                dag = new DesignerActionGlyph(dab, trayBounds, compTray);
                            }
                        }
                    }

                    //either comp is a control or we failed to find a traycontrol (which could be the case for toolstripitem components) - in this case just create a standard glyph.
                    if (dag is null)
                    {
                        //if the related comp is a control, then this shortcut will be off its bounds
                        dag = new DesignerActionGlyph(dab, _designerActionAdorner);
                    }

                    if (dag is not null)
                    {
                        //store off this relationship
                        _componentToGlyph.Add(comp, dag);
                    }
                }
                else
                {
                    dag = _componentToGlyph[comp] as DesignerActionGlyph;
                    if (dag is not null)
                    {
                        if (dag.Behavior is DesignerActionBehavior behavior)
                        {
                            behavior.ActionLists = dalColl;
                        }

                        dag.Invalidate(); // need to invalidate here too, someone could have called refresh too soon, causing the glyph to get created in the wrong place
                    }
                }

                return dag;
            }
            else
            {
                // the list is now empty... remove the panel and glyph for this control
                RemoveActionGlyph(comp);
                return null;
            }
        }

        /// <summary>
        ///  We monitor this event so we can update smart tag locations when  controls move.
        /// </summary>
        private void OnComponentChanged(object source, ComponentChangedEventArgs ce)
        {
            //validate event args
            if (ce.Component is null || ce.Member is null || !IsDesignerActionPanelVisible)
            {
                return;
            }

            // If the smart tag is showing, we only move the smart tag if the changing  component is the component for the currently showing smart tag.
            if (_lastPanelComponent is not null && !_lastPanelComponent.Equals(ce.Component))
            {
                return;
            }

            //if something changed on a component we have actions associated with then invalidate all (repaint & reposition)
            if (_componentToGlyph[ce.Component] is DesignerActionGlyph glyph)
            {
                glyph.Invalidate();

                if (ce.Member.Name.Equals("Dock"))
                { // this is the only case were we don't require an explicit refresh
                    RecreatePanel(ce.Component as IComponent); // because 99% of the time the action is name "dock in parent container" and get replaced by "undock"
                }

                if (ce.Member.Name.Equals("Location") ||
                     ce.Member.Name.Equals("Width") ||
                     ce.Member.Name.Equals("Height"))
                {
                    // we don't need to regen, we just need to update location calculate the position of the form hosting the panel
                    UpdateDAPLocation(ce.Component as IComponent, glyph);
                }
            }
        }

        private void RecreatePanel(IComponent comp)
        {
            if (_inTransaction || comp != _selSvc.PrimarySelection)
            { // we only ever need to do that when the comp is the primary selection
                return;
            }

            // we check whether or not we're in a transaction, if we are, we only the refresh at the end of the transaction to avoid flicker.
            if (_serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host)
            {
                bool hostIsClosingTransaction = false;
                if (host is IDesignerHostTransactionState hostTransactionState)
                {
                    hostIsClosingTransaction = hostTransactionState.IsClosingTransaction;
                }

                if (host.InTransaction && !hostIsClosingTransaction)
                {
                    host.TransactionClosed += new DesignerTransactionCloseEventHandler(DesignerTransactionClosed);
                    _inTransaction = true;
                    _relatedComponentTransaction = comp;
                    return;
                }
            }

            RecreateInternal(comp);
        }

        private void DesignerTransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
        {
            if (e.LastTransaction && _relatedComponentTransaction is not null)
            {
                // surprise surprise we can get multiple even with e.LastTransaction set to true, even though we unhook here this is because the list on which we enumerate (the event handler list) is copied before it's enumerated on which means that if the undo engine for example creates and commit a transaction during the OnCancel of another  completed transaction we will get this twice. So we have to check also for relatedComponentTransaction is not null
                _inTransaction = false;
                IDesignerHost host = _serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
                host.TransactionClosed -= new DesignerTransactionCloseEventHandler(DesignerTransactionClosed);
                RecreateInternal(_relatedComponentTransaction);
                _relatedComponentTransaction = null;
            }
        }

        private void RecreateInternal(IComponent comp)
        {
            DesignerActionGlyph glyph = GetDesignerActionGlyph(comp);
            if (glyph is not null)
            {
                VerifyGlyphIsInAdorner(glyph);
                // this could happen when a verb change state or suddenly a control gets a new action in the panel and we are the primary selection in that case there would not be a glyph active in the adorner to be shown because we update that on selection change. We have to do that here too. Sad really...
                RecreatePanel(glyph); // recreate the DAP itself
                UpdateDAPLocation(comp, glyph); // reposition the thing
            }
        }

        private void RecreatePanel(Glyph glyphWithPanelToRegen)
        {
            // we don't want to do anything if the panel is not visible
            if (!IsDesignerActionPanelVisible)
            {
                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI.RecreatePanel] panel is not visible, bail");
                return;
            }

            //recreate a designeraction panel
            if (glyphWithPanelToRegen is not null)
            {
                if (glyphWithPanelToRegen.Behavior is DesignerActionBehavior behaviorWithPanelToRegen)
                {
                    Debug.Assert(behaviorWithPanelToRegen.RelatedComponent is not null, "could not find related component for this refresh");
                    DesignerActionPanel dap = designerActionHost.CurrentPanel; // WE DO NOT RECREATE THE WHOLE THING / WE UPDATE THE TASKS - should flicker less
                    dap.UpdateTasks(behaviorWithPanelToRegen.ActionLists, new DesignerActionListCollection(), string.Format(SR.DesignerActionPanel_DefaultPanelTitle,
                        behaviorWithPanelToRegen.RelatedComponent.GetType().Name), null);
                    designerActionHost.UpdateContainerSize();
                }
            }
        }

        private void VerifyGlyphIsInAdorner(DesignerActionGlyph glyph)
        {
            if (glyph.IsInComponentTray)
            {
                ComponentTray compTray = _serviceProvider.GetService(typeof(ComponentTray)) as ComponentTray;
                if (compTray.SelectionGlyphs is not null && !compTray.SelectionGlyphs.Contains(glyph))
                {
                    compTray.SelectionGlyphs.Insert(0, glyph);
                }
            }
            else
            {
                if (_designerActionAdorner is not null && _designerActionAdorner.Glyphs is not null && !_designerActionAdorner.Glyphs.Contains(glyph))
                {
                    _designerActionAdorner.Glyphs.Insert(0, glyph);
                }
            }

            glyph.InvalidateOwnerLocation();
        }

        /// <summary>
        ///  This event is fired by the DesignerActionService in response to a DesignerActionCollection changing.  The event args contains information about the related object, the type of change (added or removed) and the remaining DesignerActionCollection for the object. Note that when new DesignerActions are added, if the related control/ is not yet parented - we add these actions to a "delay" list and they are later created when the control is finally parented.
        /// </summary>
        private void OnDesignerActionsChanged(object sender, DesignerActionListsChangedEventArgs e)
        {
            // We need to invoke this async because the designer action service will  raise this event from the thread pool.
            if (_marshalingControl is not null && _marshalingControl.IsHandleCreated)
            {
                _marshalingControl.BeginInvoke(new ActionChangedEventHandler(OnInvokedDesignerActionChanged), new object[] { sender, e });
            }
        }

        private void OnDesignerActionUIStateChange(object sender, DesignerActionUIStateChangeEventArgs e)
        {
            IComponent comp = e.RelatedObject as IComponent;
            Debug.Assert(comp is not null || e.ChangeType == DesignerActionUIStateChangeType.Hide, "related object is not an IComponent, something is wrong here...");
            if (comp is not null)
            {
                DesignerActionGlyph relatedGlyph = GetDesignerActionGlyph(comp);
                if (relatedGlyph is not null)
                {
                    if (e.ChangeType == DesignerActionUIStateChangeType.Show)
                    {
                        if (relatedGlyph.Behavior is DesignerActionBehavior behavior)
                        {
                            behavior.ShowUI(relatedGlyph);
                        }
                    }
                    else if (e.ChangeType == DesignerActionUIStateChangeType.Hide)
                    {
                        if (relatedGlyph.Behavior is DesignerActionBehavior behavior)
                        {
                            behavior.HideUI();
                        }
                    }
                    else if (e.ChangeType == DesignerActionUIStateChangeType.Refresh)
                    {
                        relatedGlyph.Invalidate();
                        RecreatePanel((IComponent)e.RelatedObject);
                    }
                }
            }
            else
            {
                if (e.ChangeType == DesignerActionUIStateChangeType.Hide)
                {
                    HideDesignerActionPanel();
                }
            }
        }

        /// <summary>
        ///  This is the same as DesignerActionChanged, but it is invoked on our control's thread
        /// </summary>
        private void OnInvokedDesignerActionChanged(object sender, DesignerActionListsChangedEventArgs e)
        {
            DesignerActionGlyph g = null;
            if (e.ChangeType == DesignerActionListsChangedType.ActionListsAdded)
            {
                if (!(e.RelatedObject is IComponent relatedComponent))
                {
                    Debug.Fail("How can we add a DesignerAction glyphs when it's related object is not  an IComponent?");
                    return;
                }

                IComponent primSel = _selSvc.PrimarySelection as IComponent;
                if (primSel == e.RelatedObject)
                {
                    g = GetDesignerActionGlyph(relatedComponent, e.ActionLists);
                    if (g is not null)
                    {
                        VerifyGlyphIsInAdorner(g);
                    }
                    else
                    {
                        RemoveActionGlyph(e.RelatedObject);
                    }
                }
            }

            if (e.ChangeType == DesignerActionListsChangedType.ActionListsRemoved && e.ActionLists.Count == 0)
            {
                //only remove our glyph if there are no more DesignerActions associated with it.
                RemoveActionGlyph(e.RelatedObject);
            }
            else if (g is not null)
            {
                // we need to recreate the panel here, since it's content has changed...
                RecreatePanel(e.RelatedObject as IComponent);
            }
        }

        /// <summary>
        ///  Called when our KeyShowDesignerActions menu command is fired  (a.k.a. Alt+Shift+F10) - we will find the primary selection, see if it has designer actions, and if so - show the menu.
        /// </summary>
        private void OnKeyShowDesignerActions(object sender, EventArgs e)
        {
            ShowDesignerActionPanelForPrimarySelection();
        }

        // we cannot attach several menu command to the same command id, we need a single entry point, we put it in designershortcutui. but we need a way to call the show ui on the related behavior hence this internal function to hack it together. we return false if we have nothing to display, we hide it and return true if we're already displaying
        internal bool ShowDesignerActionPanelForPrimarySelection()
        {
            //can't do anything w/o selection service
            if (_selSvc is null)
            {
                return false;
            }

            object primarySelection = _selSvc.PrimarySelection;
            //verify that we have obtained a valid component with designer actions
            if (primarySelection is null || !_componentToGlyph.Contains(primarySelection))
            {
                return false;
            }

            DesignerActionGlyph glyph = (DesignerActionGlyph)_componentToGlyph[primarySelection];
            if (glyph is not null && glyph.Behavior is DesignerActionBehavior)
            {
                // show the menu
                if (glyph.Behavior is DesignerActionBehavior behavior)
                {
                    if (!IsDesignerActionPanelVisible)
                    {
                        behavior.ShowUI(glyph);
                        return true;
                    }
                    else
                    {
                        behavior.HideUI();
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  When all the DesignerActions have been removed for a particular object, we remove any UI (glyphs) that we may have been managing.
        /// </summary>
        internal void RemoveActionGlyph(object relatedObject)
        {
            if (relatedObject is null)
            {
                return;
            }

            if (IsDesignerActionPanelVisible && relatedObject == _lastPanelComponent)
            {
                HideDesignerActionPanel();
            }

            DesignerActionGlyph glyph = (DesignerActionGlyph)_componentToGlyph[relatedObject];
            if (glyph is not null)
            {
                // Check ComponentTray first
                if (_serviceProvider.GetService(typeof(ComponentTray)) is ComponentTray compTray && compTray.SelectionGlyphs is not null)
                {
                    if (compTray is not null && compTray.SelectionGlyphs.Contains(glyph))
                    {
                        compTray.SelectionGlyphs.Remove(glyph);
                    }
                }

                if (_designerActionAdorner.Glyphs.Contains(glyph))
                {
                    _designerActionAdorner.Glyphs.Remove(glyph);
                }

                _componentToGlyph.Remove(relatedObject);

                // we only do this when we're in a transaction, see bug VSWHIDBEY 418709. This is for compat reason - infragistic. if we're not in a transaction, too bad, we don't update the screen
                if (_serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host && host.InTransaction)
                {
                    host.TransactionClosed += new DesignerTransactionCloseEventHandler(InvalidateGlyphOnLastTransaction);
                    _relatedGlyphTransaction = glyph;
                }
            }
        }

        private void InvalidateGlyphOnLastTransaction(object sender, DesignerTransactionCloseEventArgs e)
        {
            if (e.LastTransaction)
            {
                IDesignerHost host = (_serviceProvider is not null) ? _serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost : null;
                if (host is not null)
                {
                    host.TransactionClosed -= new DesignerTransactionCloseEventHandler(InvalidateGlyphOnLastTransaction);
                }

                if (_relatedGlyphTransaction is not null)
                {
                    _relatedGlyphTransaction.InvalidateOwnerLocation();
                }

                _relatedGlyphTransaction = null;
            }
        }

        internal void HideDesignerActionPanel()
        {
            if (IsDesignerActionPanelVisible)
            {
                designerActionHost.Close();
            }
        }

        internal bool IsDesignerActionPanelVisible
        {
            get => (designerActionHost is not null && designerActionHost.Visible);
        }

        internal IComponent LastPanelComponent
        {
            get => (IsDesignerActionPanelVisible ? _lastPanelComponent : null);
        }

        private void ToolStripDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (_cancelClose || e.Cancel)
            {
                e.Cancel = true;
                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI.toolStripDropDown_Closing] cancelClose true, bail");
                return;
            }

            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI.toolStripDropDown_Closing] ItemClicked: e.Cancel set to: " + e.Cancel.ToString());
            }

            if (e.CloseReason == ToolStripDropDownCloseReason.Keyboard)
            {
                e.Cancel = false;
                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI.toolStripDropDown_Closing] Keyboard: e.Cancel set to: " + e.Cancel.ToString());
            }

            if (e.Cancel == false)
            { // we WILL disappear
                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI.toolStripDropDown_Closing] Closing...");
                Debug.Assert(_lastPanelComponent is not null, "last panel component should not be null here... " +
                    "(except if you're currently debugging VS where deactivation messages in the middle of the pump can mess up everything...)");
                if (_lastPanelComponent is null)
                {
                    return;
                }

                // if we're actually closing get the coordinate of the last message, the one causing us to close, is it within the glyph coordinate. if it is that mean that someone just clicked back from the panel, on VS, but ON THE GLYPH, that means that he actually wants to close it. The activation change is going to do that for us but we should NOT reopen right away because he clicked on the glyph... this code is here to prevent this...
                Point point = DesignerUtils.LastCursorPoint;
                if (_componentToGlyph[_lastPanelComponent] is DesignerActionGlyph currentGlyph)
                {
                    Point glyphCoord = GetGlyphLocationScreenCoord(_lastPanelComponent, currentGlyph);
                    if ((new Rectangle(glyphCoord, new Size(currentGlyph.Bounds.Width, currentGlyph.Bounds.Height))).Contains(point))
                    {
                        DesignerActionBehavior behavior = currentGlyph.Behavior as DesignerActionBehavior;
                        behavior.IgnoreNextMouseUp = true;
                    }

                    currentGlyph.InvalidateOwnerLocation();
                }

                _lastPanelComponent = null;
                // panel is going away, pop the behavior that's on the stack...
                Debug.Assert(_dapkb is not null, "why is dapkb null?");
                Behavior popBehavior = _behaviorService.PopBehavior(_dapkb);
                Debug.Assert(popBehavior is DesignerActionKeyboardBehavior, "behavior returned is of the wrong kind?");
            }
        }

        internal Point UpdateDAPLocation(IComponent component, DesignerActionGlyph glyph)
        {
            if (component is null)
            { // in case of a resize...
                component = _lastPanelComponent;
            }

            if (designerActionHost is null)
            {
                return Point.Empty;
            }

            if (component is null || glyph is null)
            {
                return designerActionHost.Location;
            }

            // check that the glyph is still visible in the adorner window
            if (_behaviorService is not null &&
                !_behaviorService.AdornerWindowControl.DisplayRectangle.IntersectsWith(glyph.Bounds))
            {
                HideDesignerActionPanel();
                return designerActionHost.Location;
            }

            Point glyphLocationScreenCoord = GetGlyphLocationScreenCoord(component, glyph);
            Rectangle rectGlyph = new Rectangle(glyphLocationScreenCoord, glyph.Bounds.Size);
            Point pt = DesignerActionPanel.ComputePreferredDesktopLocation(rectGlyph, designerActionHost.Size, out DockStyle edgeToDock);
            glyph.DockEdge = edgeToDock;
            designerActionHost.Location = pt;
            return pt;
        }

        private Point GetGlyphLocationScreenCoord(IComponent relatedComponent, Glyph glyph)
        {
            Point glyphLocationScreenCoord = new Point(0, 0);
            if (relatedComponent is Control && !(relatedComponent is ToolStripDropDown))
            {
                glyphLocationScreenCoord = _behaviorService.AdornerWindowPointToScreen(glyph.Bounds.Location);
            }

            //ISSUE: we can't have this special cased here - we should find a more generic approach to solving this problem
            else if (relatedComponent is ToolStripItem)
            {
                if (relatedComponent is ToolStripItem item && item.Owner is not null)
                {
                    glyphLocationScreenCoord = _behaviorService.AdornerWindowPointToScreen(glyph.Bounds.Location);
                }
            }
            else if (relatedComponent is IComponent)
            {
                if (_serviceProvider.GetService(typeof(ComponentTray)) is ComponentTray compTray)
                {
                    glyphLocationScreenCoord = compTray.PointToScreen(glyph.Bounds.Location);
                }
            }

            return glyphLocationScreenCoord;
        }

        bool _cancelClose;
        /// <summary>
        ///  This shows the actual chrome panel that is created by the DesignerActionBehavior object.
        /// </summary>
        internal void ShowDesignerActionPanel(IComponent relatedComponent, DesignerActionPanel panel, DesignerActionGlyph glyph)
        {
            if (designerActionHost is null)
            {
                designerActionHost = new DesignerActionToolStripDropDown(this, _mainParentWindow)
                {
                    AutoSize = false,
                    Padding = Padding.Empty,
                    Renderer = new NoBorderRenderer(),
                    Text = "DesignerActionTopLevelForm"
                };
                designerActionHost.Closing += new ToolStripDropDownClosingEventHandler(ToolStripDropDown_Closing);
            }

            // set the accessible name of the panel to the same title as the panel header. do that every time
            designerActionHost.AccessibleName = string.Format(SR.DesignerActionPanel_DefaultPanelTitle, relatedComponent.GetType().Name);
            panel.AccessibleName = string.Format(SR.DesignerActionPanel_DefaultPanelTitle, relatedComponent.GetType().Name);

            designerActionHost.SetDesignerActionPanel(panel, glyph);
            Point location = UpdateDAPLocation(relatedComponent, glyph);

            // check that the panel will have at least it's parent glyph visible on the adorner window
            if (_behaviorService is not null &&
                _behaviorService.AdornerWindowControl.DisplayRectangle.IntersectsWith(glyph.Bounds))
            {
                if (_mainParentWindow is not null && _mainParentWindow.Handle != IntPtr.Zero)
                {
                    Debug.WriteLineIf(s_designerActionPanelTraceSwitch.TraceVerbose, "Assigning owner to mainParentWindow");
                    Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "Assigning owner to mainParentWindow");
                    User32.SetWindowLong(designerActionHost, User32.GWL.HWNDPARENT, new HandleRef(_mainParentWindow, _mainParentWindow.Handle));
                }

                _cancelClose = true;
                designerActionHost.Show(location);
                designerActionHost.Focus();
                // when a control is drag and dropped and autoshow is set to true the vs designer is going to get activated as soon as the control is dropped we don't want to close the panel then, so we post a message (using the trick to call begin invoke) and once everything is settled re-activate the autoclose logic
                designerActionHost.BeginInvoke(new EventHandler(OnShowComplete));
                // invalidate the glyph to have it point the other way
                glyph.InvalidateOwnerLocation();
                _lastPanelComponent = relatedComponent;
                // push new behavior for keyboard handling on the behavior stack
                _dapkb = new DesignerActionKeyboardBehavior(designerActionHost.CurrentPanel, _serviceProvider, _behaviorService);
                _behaviorService.PushBehavior(_dapkb);
            }
        }

        private void OnShowComplete(object sender, EventArgs e)
        {
            _cancelClose = false;
            // force the panel to be the active window - for some reason someone else could have forced VS to become active for real while we were ignoring close. This might be bad cause we'd be in a bad state.
            if (designerActionHost is not null && designerActionHost.Handle != IntPtr.Zero && designerActionHost.Visible)
            {
                User32.SetActiveWindow(new HandleRef(this, designerActionHost.Handle));
                designerActionHost.CheckFocusIsRight();
            }
        }
    }

    internal class DesignerActionToolStripDropDown : ToolStripDropDown
    {
        private readonly IWin32Window _mainParentWindow;
        private ToolStripControlHost _panel;
        private readonly DesignerActionUI _designerActionUI;
        private bool _cancelClose;
        private Glyph _relatedGlyph;

        public DesignerActionToolStripDropDown(DesignerActionUI designerActionUI, IWin32Window mainParentWindow)
        {
            _mainParentWindow = mainParentWindow;
            _designerActionUI = designerActionUI;
        }

        public DesignerActionPanel CurrentPanel
        {
            get
            {
                if (_panel is not null)
                {
                    return _panel.Control as DesignerActionPanel;
                }
                else
                {
                    return null;
                }
            }
        }

        // we're not topmost because we can show modal editors above us.
        protected override bool TopMost
        {
            get => false;
        }

        public void UpdateContainerSize()
        {
            if (CurrentPanel is not null)
            {
                Size panelSize = CurrentPanel.GetPreferredSize(new Size(150, int.MaxValue));
                if (CurrentPanel.Size == panelSize)
                {
                    // If the panel size didn't actually change, we still have to force a call to PerformLayout to make sure that controls get repositioned properly within the panel. The issue arises because we did a measure-only Layout that determined some sizes, and then we end up painting with those values even though there wasn't an actual Layout performed.
                    CurrentPanel.PerformLayout();
                }
                else
                {
                    CurrentPanel.Size = panelSize;
                }

                ClientSize = panelSize;
            }
        }

        public void CheckFocusIsRight()
        { // fix to get the focus to NOT stay on ContainerControl
            Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "Checking focus...");
            IntPtr focusedControl = User32.GetFocus();
            if (focusedControl == Handle)
            {
                Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "    putting focus on the panel...");
                _panel.Focus();
            }

            focusedControl = User32.GetFocus();
            if (CurrentPanel is not null && CurrentPanel.Handle == focusedControl)
            {
                Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "    selecting next available control on the panel...");
                CurrentPanel.SelectNextControl(null, true, true, true, true);
            }

            User32.GetFocus();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            UpdateContainerSize();
        }

        protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
        {
            Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "_____________________________Begin OnClose " + e.CloseReason.ToString());
            Debug.Indent();
            if (e.CloseReason == ToolStripDropDownCloseReason.AppFocusChange && _cancelClose)
            {
                _cancelClose = false;
                e.Cancel = true;
                Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "cancel close prepopulated");
            }

            // when we get closing event as a result of an activation change, pre-populate e.Cancel based on why we're exiting.
            // - if it's a modal window that's owned by VS don't exit
            // - if it's a window that's owned by the toolstrip dropdown don't exit
            else if (e.CloseReason == ToolStripDropDownCloseReason.AppFocusChange || e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
            {
                IntPtr hwndActivating = User32.GetActiveWindow();
                if (Handle == hwndActivating && e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
                {
                    e.Cancel = false;
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] activation hasnt changed, but we've certainly clicked somewhere else.");
                }
                else if (WindowOwnsWindow(Handle, hwndActivating))
                {
                    // we're being de-activated for someone owned by the panel
                    e.Cancel = true;
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] Cancel close - the window activating is owned by this window");
                }
                else if (_mainParentWindow is not null && !WindowOwnsWindow(_mainParentWindow.Handle, hwndActivating))
                {
                    if (IsWindowEnabled(_mainParentWindow.Handle))
                    {
                        // the activated windows is not a child/owned windows of the main top level windows let toolstripdropdown handle this
                        e.Cancel = false;
                        Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] Call close: the activated windows is not a child/owned windows of the main top level windows ");
                    }
                    else
                    {
                        e.Cancel = true;
                        Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] we're being deactivated by a foreign window, but the main window is not enabled - we should stay up");
                    }

                    base.OnClosing(e);
                    Debug.Unindent();
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "_____________________________End OnClose e.Cancel: " + e.Cancel.ToString());
                    return;
                }
                else
                {
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] since the designer action panel dropdown doesnt own the activating window " + hwndActivating.ToString("x") + ", calling close. ");
                }

                // what's the owner of the windows being activated?
                IntPtr parent = User32.GetWindowLong(new HandleRef(this, hwndActivating), User32.GWL.HWNDPARENT);
                // is it currently disabled (ie, the activating windows is in modal mode)
                if (!IsWindowEnabled(parent))
                {
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] modal window activated - cancelling close");
                    // we are in a modal case
                    e.Cancel = true;
                }
            }

            Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] calling base.OnClosing with e.Cancel: " + e.Cancel.ToString());
            base.OnClosing(e);
            Debug.Unindent();
            Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "_____________________________End OnClose e.Cancel: " + e.Cancel.ToString());
        }

        public void SetDesignerActionPanel(DesignerActionPanel panel, Glyph relatedGlyph)
        {
            if (_panel is not null && panel == (DesignerActionPanel)_panel.Control)
            {
                return;
            }

            Debug.Assert(relatedGlyph is not null, "related glyph cannot be null");
            _relatedGlyph = relatedGlyph;
            panel.SizeChanged += new EventHandler(PanelResized);
            // hook up the event
            if (_panel is not null)
            {
                Items.Remove(_panel);
                _panel.Dispose();
                _panel = null;
            }

            _panel = new ToolStripControlHost(panel)
            {
                // we don't want no margin
                Margin = Padding.Empty,
                Size = panel.Size
            };

            SuspendLayout();
            Size = panel.Size;
            Items.Add(_panel);
            ResumeLayout();
            if (Visible)
            {
                CheckFocusIsRight();
            }
        }

        private void PanelResized(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (Size.Width != ctrl.Size.Width || Size.Height != ctrl.Size.Height)
            {
                SuspendLayout();
                Size = ctrl.Size;
                if (_panel is not null)
                {
                    _panel.Size = ctrl.Size;
                }

                _designerActionUI.UpdateDAPLocation(null, _relatedGlyph as DesignerActionGlyph);
                ResumeLayout();
            }
        }

        protected override void SetVisibleCore(bool visible)
        {
            Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.SetVisibleCore] setting dropdown visible=" + visible.ToString());
            base.SetVisibleCore(visible);
            if (visible)
            {
                CheckFocusIsRight();
            }
        }

        /// <summary>
        ///  General purpose method, based on Control.Contains()...
        ///  Determines whether a given window (specified using native window handle) is a descendant of this control. This catches both contained descendants and 'owned' windows such as modal dialogs. Using window handles rather than Control objects allows it to catch un-managed windows as well.
        /// </summary>
        private static bool WindowOwnsWindow(IntPtr hWndOwner, IntPtr hWndDescendant)
        {
            Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[WindowOwnsWindow] Testing if " + hWndOwner.ToString("x") + " is a owned by " + hWndDescendant.ToString("x") + "... ");
#if DEBUG
            if (DesignerActionUI.DropDownVisibilityDebug.TraceVerbose)
            {
                Debug.WriteLine("\t\tOWNER: " + GetControlInformation(hWndOwner));
                Debug.WriteLine("\t\tOWNEE: " + GetControlInformation(hWndDescendant));
                IntPtr claimedOwnerHwnd = User32.GetWindowLong(hWndDescendant, User32.GWL.HWNDPARENT);
                Debug.WriteLine("OWNEE's CLAIMED OWNER: " + GetControlInformation(claimedOwnerHwnd));
            }
#endif
            if (hWndDescendant == hWndOwner)
            {
                Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "they match, YES.");
                return true;
            }

            while (hWndDescendant != IntPtr.Zero)
            {
                hWndDescendant = User32.GetWindowLong(hWndDescendant, User32.GWL.HWNDPARENT);
                if (hWndDescendant == IntPtr.Zero)
                {
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "NOPE.");
                    return false;
                }

                if (hWndDescendant == hWndOwner)
                {
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "YES.");
                    return true;
                }
            }

            Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "NO.");
            return false;
        }

        // helper function for generating infomation about a particular control use AssertControlInformation if sticking in an assert - then the work to figure out the control info will only be done when the assertion is false.
        internal static string GetControlInformation(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                return "Handle is IntPtr.Zero";
            }
#if DEBUG
            if (!DesignerActionUI.DropDownVisibilityDebug.TraceVerbose)
            {
                return string.Empty;
            }

            string windowText = User32.GetWindowText(hwnd);
            string typeOfControl = "Unknown";
            string nameOfControl = string.Empty;
            Control c = FromHandle(hwnd);
            if (c is not null)
            {
                typeOfControl = c.GetType().Name;
                if (!string.IsNullOrEmpty(c.Name))
                {
                    nameOfControl += c.Name;
                }
                else
                {
                    nameOfControl += "Unknown";
                    // some extra debug info for toolstripdropdowns...
                    if (c is ToolStripDropDown dd)
                    {
                        if (dd.OwnerItem is not null)
                        {
                            nameOfControl += "OwnerItem: [" + dd.OwnerItem.ToString() + "]";
                        }
                    }
                }
            }

            return windowText + "\r\n\t\t\tType: [" + typeOfControl + "] Name: [" + nameOfControl + "]";
#else
            return string.Empty;
#endif

        }

        private bool IsWindowEnabled(IntPtr handle)
        {
            int style = (int)User32.GetWindowLong(new HandleRef(this, handle), User32.GWL.STYLE);
            return (style & (int)User32.WS.DISABLED) == 0;
        }

        private void WmActivate(ref Message m)
        {
            if ((User32.WA)m.WParamInternal == User32.WA.INACTIVE)
            {
                IntPtr hwndActivating = m.LParamInternal;
                if (WindowOwnsWindow(Handle, hwndActivating))
                {
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI WmActivate] setting cancel close true because WindowsOwnWindow");
                    Debug.WriteLineIf(DesignerActionUI.DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI WmActivate] checking the focus... " + GetControlInformation(User32.GetFocus()));
                    _cancelClose = true;
                }
                else
                {
                    _cancelClose = false;
                }
            }
            else
            {
                _cancelClose = false;
            }

            base.WndProc(ref m);
        }

        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.ACTIVATE:
                    WmActivate(ref m);
                    return;
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // since we're not hosted in a form we need to do the same logic as Form.cs. If we get an enter key we need to find the current focused control. if it's a button, we click it and return that we handled the message
            if (keyData == Keys.Enter)
            {
                IntPtr focusedControlPtr = User32.GetFocus();
                Control focusedControl = FromChildHandle(focusedControlPtr);
                if (focusedControl is IButtonControl button && button is Control)
                {
                    button.PerformClick();
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }
    }
}
