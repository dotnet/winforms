﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.ComponentModel.Design;

/// <summary>
///  The DesignerActionUI is the designer/UI-specific implementation of the DesignerActions feature.
///  This class instantiates the DesignerActionService and hooks to its DesignerActionsChanged event.
///  Responding to this single event will enable the DesignerActionUI to perform all necessary UI-related operations.
///  Note that the DesignerActionUI uses the BehaviorService to manage all UI interaction.
///  For every component containing a DesignerAction (determined by the DesignerActionsChanged event)
///  there will be an associated DesignerActionGlyph and DesignerActionBehavior.
///  Finally, the DesignerActionUI is also responsible for showing and managing the Action's context menus.
///  Note that every DesignerAction context menu has an item that will bring up the DesignerActions
///  option pane in the options dialog.
/// </summary>
internal partial class DesignerActionUI : IDisposable
{
    private static readonly TraceSwitch s_designerActionPanelTraceSwitch = new("DesignerActionPanelTrace", "DesignerActionPanel tracing");

    private Adorner _designerActionAdorner; //used to add designeraction-related glyphs
    private IServiceProvider _serviceProvider; //standard service provider
    private ISelectionService _selSvc; //used to determine if comps have selection or not
    private DesignerActionService _designerActionService; //this is how all designeractions will be managed
    private DesignerActionUIService _designerActionUIService; //this is how all designeractions UI elements will be managed
    private BehaviorService _behaviorService; //this is how all of our UI is implemented (glyphs, behaviors, etc...)
    private readonly IMenuCommandService _menuCommandService;
    private DesignerActionKeyboardBehavior _dapkb;   //out keyboard behavior
    private readonly Dictionary<object, DesignerActionGlyph> _componentToGlyph; //used for quick reference between components and our glyphs
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
    private bool _cancelClose;

    private delegate void ActionChangedEventHandler(object sender, DesignerActionListsChangedEventArgs e);
#if DEBUG
    internal static readonly TraceSwitch DropDownVisibilityDebug = new("DropDownVisibilityDebug", "Debug ToolStrip Selection code");
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

        _componentToGlyph = new();
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
                mcs?.RemoveCommand(_cmdShowDesignerActions);
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
        dalColl ??= _designerActionService.GetComponentActions(comp);

        if (dalColl is not null && dalColl.Count > 0)
        {
            DesignerActionGlyph dag = null;
            if (!_componentToGlyph.ContainsKey(comp))
            {
                DesignerActionBehavior dab = new DesignerActionBehavior(_serviceProvider, comp, dalColl, this);

                //if comp is a component then try to find a traycontrol associated with it... this should really be in ComponentTray but there is no behaviorService for the CT
                if (comp is not Control or ToolStripDropDown)
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
                //if the related comp is a control, then this shortcut will be off its bounds
                dag ??= new DesignerActionGlyph(dab, _designerActionAdorner);

                if (dag is not null)
                {
                    //store off this relationship
                    _componentToGlyph[comp] = dag;
                }
            }
            else
            {
                if (_componentToGlyph.TryGetValue(comp, out dag))
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
        if (_componentToGlyph.TryGetValue(ce.Component, out DesignerActionGlyph glyph))
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
            if (e.RelatedObject is not IComponent relatedComponent)
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
        if (primarySelection is null || !_componentToGlyph.TryGetValue(primarySelection, out DesignerActionGlyph glyph))
        {
            return false;
        }

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

        if (!_componentToGlyph.TryGetValue(relatedObject, out DesignerActionGlyph glyph))
        {
            return;
        }

        // Check ComponentTray first
        if (_serviceProvider.GetService(typeof(ComponentTray)) is ComponentTray compTray && compTray.SelectionGlyphs is not null)
        {
            compTray.SelectionGlyphs.Remove(glyph);
        }

        _designerActionAdorner.Glyphs.Remove(glyph);
        _componentToGlyph.Remove(relatedObject);

        // we only do this when we're in a transaction, see bug VSWHIDBEY 418709. This is for compat reason - infragistic. if we're not in a transaction, too bad, we don't update the screen
        if (_serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host && host.InTransaction)
        {
            host.TransactionClosed += new DesignerTransactionCloseEventHandler(InvalidateGlyphOnLastTransaction);
            _relatedGlyphTransaction = glyph;
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

            _relatedGlyphTransaction?.InvalidateOwnerLocation();

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
            Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, $"[DesignerActionUI.toolStripDropDown_Closing] ItemClicked: e.Cancel set to: {e.Cancel}");
        }

        if (e.CloseReason == ToolStripDropDownCloseReason.Keyboard)
        {
            e.Cancel = false;
            Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, $"[DesignerActionUI.toolStripDropDown_Closing] Keyboard: e.Cancel set to: {e.Cancel}");
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
            if (_componentToGlyph.TryGetValue(_lastPanelComponent, out DesignerActionGlyph currentGlyph))
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
        component ??= _lastPanelComponent;

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
        if (relatedComponent is Control and not ToolStripDropDown)
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
                PInvoke.SetWindowLong(
                    designerActionHost,
                    WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT,
                    new HandleRef<HWND>(_mainParentWindow, (HWND)_mainParentWindow.Handle));
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
        if (designerActionHost is not null && designerActionHost.Handle != 0 && designerActionHost.Visible)
        {
            PInvoke.SetActiveWindow(designerActionHost);
            designerActionHost.CheckFocusIsRight();
        }
    }
}
