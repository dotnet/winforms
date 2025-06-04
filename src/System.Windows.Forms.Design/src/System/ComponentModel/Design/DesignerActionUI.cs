// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    private Adorner _designerActionAdorner; // used to add designeraction-related glyphs
    private IServiceProvider _serviceProvider; // standard service provider
    private ISelectionService _selectionService; // used to determine if comps have selection or not
    private DesignerActionService _designerActionService; // this is how all designeractions will be managed
    private DesignerActionUIService _designerActionUIService; // this is how all designeractions UI elements will be managed
    private BehaviorService _behaviorService; // this is how all of our UI is implemented (glyphs, behaviors, etc...)
    private DesignerActionKeyboardBehavior? _designerActionKeyboardBehavior;   // out keyboard behavior
    private readonly Dictionary<object, DesignerActionGlyph> _componentToGlyph; // used for quick reference between components and our glyphs
    private Control _marshalingControl; // used to invoke events on our main gui thread
    private IComponent? _lastPanelComponent;

    private readonly IWin32Window? _mainParentWindow;
    internal DesignerActionToolStripDropDown? _designerActionHost;

    private readonly MenuCommand? _cmdShowDesignerActions; // used to respond to the Alt+Shift+F10 command
    private bool _inTransaction;
    private IComponent? _relatedComponentTransaction;
    private DesignerActionGlyph? _relatedGlyphTransaction;
    private readonly bool _disposeActionService;
    private readonly bool _disposeActionUIService;
    private bool _cancelClose;

    private delegate void ActionChangedEventHandler(object sender, DesignerActionListsChangedEventArgs e);

    /// <summary>
    ///  Constructor that takes a service provider. This is needed to establish references to the BehaviorService
    ///  and SelectionService, as well as spin-up the DesignerActionService.
    /// </summary>
    public DesignerActionUI(IServiceProvider serviceProvider, Adorner containerAdorner)
    {
        _serviceProvider = serviceProvider;
        _designerActionAdorner = containerAdorner;
        IMenuCommandService? menuCommandService = serviceProvider.GetService<IMenuCommandService>();
        if (!serviceProvider.TryGetService(out BehaviorService? behaviorService) ||
            !serviceProvider.TryGetService(out ISelectionService? selectionService))
        {
            Debug.Fail("Either BehaviorService or ISelectionService is null, cannot continue.");
            return;
        }

        _behaviorService = behaviorService;
        _selectionService = selectionService;

        // query for our DesignerActionService
        if (!serviceProvider.TryGetService(out DesignerActionService? designerActionService))
        {
            // start the service
            designerActionService = new DesignerActionService(serviceProvider);
            _disposeActionService = true;
        }

        if (!serviceProvider.TryGetService(out DesignerActionUIService? designerActionUIService))
        {
            designerActionUIService = new DesignerActionUIService(serviceProvider);
            _disposeActionUIService = true;
        }

        _designerActionService = designerActionService;
        _designerActionUIService = designerActionUIService;

        _designerActionUIService.DesignerActionUIStateChange += OnDesignerActionUIStateChange;
        _designerActionService.DesignerActionListsChanged += OnDesignerActionsChanged;
        _lastPanelComponent = null;

        if (serviceProvider.TryGetService(out IComponentChangeService? cs))
        {
            cs.ComponentChanged += OnComponentChanged;
        }

        if (menuCommandService is not null)
        {
            _cmdShowDesignerActions = new MenuCommand(OnKeyShowDesignerActions, MenuCommands.KeyInvokeSmartTag);
            menuCommandService.AddCommand(_cmdShowDesignerActions);
        }

        if (serviceProvider.TryGetService(out IUIService? uiService))
        {
            _mainParentWindow = uiService.GetDialogOwnerWindow();
        }

        _componentToGlyph = [];
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
            _marshalingControl = null!;
        }

        if (_serviceProvider is not null)
        {
            if (_serviceProvider.TryGetService(out IComponentChangeService? cs))
            {
                cs.ComponentChanged -= OnComponentChanged;
            }

            if (_cmdShowDesignerActions is not null)
            {
                IMenuCommandService? mcs = _serviceProvider.GetService<IMenuCommandService>();
                mcs?.RemoveCommand(_cmdShowDesignerActions);
            }
        }

        _serviceProvider = null!;
        _behaviorService = null!;
        _selectionService = null!;

        if (_designerActionService is not null)
        {
            _designerActionService.DesignerActionListsChanged -= OnDesignerActionsChanged;
            if (_disposeActionService)
            {
                _designerActionService.Dispose();
            }
        }

        _designerActionService = null!;

        if (_designerActionUIService is not null)
        {
            _designerActionUIService.DesignerActionUIStateChange -= OnDesignerActionUIStateChange;
            if (_disposeActionUIService)
            {
                _designerActionUIService.Dispose();
            }
        }

        _designerActionUIService = null!;
        _designerActionAdorner = null!;
    }

    public DesignerActionGlyph? GetDesignerActionGlyph(IComponent comp)
    {
        return GetDesignerActionGlyph(comp, null);
    }

    internal DesignerActionGlyph? GetDesignerActionGlyph(IComponent comp, DesignerActionListCollection? dalColl)
    {
        // check this component origin, this class or is it readonly because inherited...
        InheritanceAttribute? attribute = (InheritanceAttribute?)TypeDescriptor.GetAttributes(comp)[typeof(InheritanceAttribute)];
        if (attribute == InheritanceAttribute.InheritedReadOnly)
        {
            // Only do it if we can change the control.
            return null;
        }

        // we didnt get on, fetch it
        dalColl ??= _designerActionService.GetComponentActions(comp);

        if (dalColl is not null && dalColl.Count > 0)
        {
            if (!_componentToGlyph.TryGetValue(comp, out DesignerActionGlyph? designerActionGlyph))
            {
                DesignerActionBehavior dab = new(_serviceProvider, comp, dalColl, this);

                // if comp is a component then try to find a TrayControl associated with it...
                // this should really be in ComponentTray but there is no behaviorService for the CT
                if (comp is not Control or ToolStripDropDown)
                {
                    // Here, we'll try to get the TrayControl associated with the comp and supply the glyph with an alternative bounds
                    ComponentTray? componentTray = _serviceProvider.GetService<ComponentTray>();
                    if (componentTray is not null)
                    {
                        ComponentTray.TrayControl trayControl = ComponentTray.GetTrayControlFromComponent(comp);
                        if (trayControl is not null)
                        {
                            Rectangle trayBounds = trayControl.Bounds;
                            designerActionGlyph = new DesignerActionGlyph(dab, trayBounds, componentTray);
                        }
                    }
                }

                // either comp is a control or we failed to find a traycontrol
                // (which could be the case for toolstripitem components) - in this case just create a standard glyph.
                // if the related comp is a control, then this shortcut will be off its bounds
                designerActionGlyph ??= new DesignerActionGlyph(dab, _designerActionAdorner);

                // store off this relationship
                _componentToGlyph[comp] = designerActionGlyph;
            }
            else
            {
                if (designerActionGlyph.Behavior is DesignerActionBehavior behavior)
                {
                    behavior.ActionLists = dalColl;
                }

                designerActionGlyph.Invalidate(); // need to invalidate here too, someone could have called refresh too soon, causing the glyph to get created in the wrong place
            }

            return designerActionGlyph;
        }
        else
        {
            // the list is now empty... remove the panel and glyph for this control
            RemoveActionGlyph(comp);
            return null;
        }
    }

    /// <summary>
    ///  We monitor this event so we can update smart tag locations when controls move.
    /// </summary>
    private void OnComponentChanged(object? source, ComponentChangedEventArgs ce)
    {
        // validate event args
        if (ce.Component is null || ce.Member is null || !IsDesignerActionPanelVisible)
        {
            return;
        }

        // If the smart tag is showing, we only move the smart tag if the changing component is the component
        // for the currently showing smart tag.
        if (_lastPanelComponent is not null && !_lastPanelComponent.Equals(ce.Component))
        {
            return;
        }

        // if something changed on a component we have actions associated with then invalidate all (repaint & reposition)
        if (_componentToGlyph.TryGetValue(ce.Component, out DesignerActionGlyph? glyph))
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

    private void RecreatePanel(IComponent? comp)
    {
        if (_inTransaction || comp is null || comp != _selectionService.PrimarySelection)
        {
            // we only ever need to do that when the comp is the primary selection
            return;
        }

        // we check whether or not we're in a transaction, if we are, we only the refresh at the end of the transaction to avoid flicker.
        IDesignerHost? host = _serviceProvider.GetService<IDesignerHost>();
        if (host is { InTransaction: true } and not IDesignerHostTransactionState { IsClosingTransaction: true })
        {
            host.TransactionClosed += DesignerTransactionClosed;
            _inTransaction = true;
            _relatedComponentTransaction = comp;
            return;
        }

        RecreateInternal(comp);
    }

    private void DesignerTransactionClosed(object? sender, DesignerTransactionCloseEventArgs e)
    {
        if (e.LastTransaction && _relatedComponentTransaction is not null)
        {
            // We can get multiple even with e.LastTransaction set to true, even though we unhook here. This is because
            // the list on which we enumerate (the event handler list) is copied before it's enumerated on which means
            // that if the undo engine for example creates and commit a transaction during the OnCancel of another
            // completed transaction we will get this twice. So we have to check also for relatedComponentTransaction
            // is not null.

            _inTransaction = false;
            IDesignerHost host = _serviceProvider.GetRequiredService<IDesignerHost>();
            host.TransactionClosed -= DesignerTransactionClosed;
            RecreateInternal(_relatedComponentTransaction);
            _relatedComponentTransaction = null;
        }
    }

    private void RecreateInternal(IComponent comp)
    {
        DesignerActionGlyph? glyph = GetDesignerActionGlyph(comp);
        if (glyph is not null)
        {
            VerifyGlyphIsInAdorner(glyph);

            // This could happen when a verb change state or suddenly a control gets a new action in the panel and we
            // are the primary selection in that case there would not be a glyph active in the adorner to be shown
            // because we update that on selection change. We have to do that here too.

            RecreatePanel(glyph);
            UpdateDAPLocation(comp, glyph);
        }
    }

    private void RecreatePanel(Glyph glyphWithPanelToRegen)
    {
        // We don't want to do anything if the panel is not visible.
        if (!IsDesignerActionPanelVisible)
        {
            return;
        }

        // recreate a designeraction panel
        if (glyphWithPanelToRegen.Behavior is DesignerActionBehavior behaviorWithPanelToRegen)
        {
            Debug.Assert(behaviorWithPanelToRegen.RelatedComponent is not null, "could not find related component for this refresh");
            DesignerActionPanel? dap = _designerActionHost.CurrentPanel; // WE DO NOT RECREATE THE WHOLE THING / WE UPDATE THE TASKS - should flicker less
            dap?.UpdateTasks(behaviorWithPanelToRegen.ActionLists,
                [],
                string.Format(SR.DesignerActionPanel_DefaultPanelTitle, behaviorWithPanelToRegen.RelatedComponent.GetType().Name),
                subtitle: null);
            _designerActionHost.UpdateContainerSize();
        }
    }

    private void VerifyGlyphIsInAdorner(DesignerActionGlyph glyph)
    {
        if (glyph.IsInComponentTray)
        {
            ComponentTray? compTray = _serviceProvider.GetService<ComponentTray>();
            if (compTray?.SelectionGlyphs is not null && !compTray.SelectionGlyphs.Contains(glyph))
            {
                compTray.SelectionGlyphs.Insert(0, glyph);
            }
        }
        else
        {
            if (_designerActionAdorner?.Glyphs is { } glyphs && !glyphs.Contains(glyph))
            {
                _designerActionAdorner.Glyphs.Insert(0, glyph);
            }
        }

        glyph.InvalidateOwnerLocation();
    }

    /// <summary>
    ///  This event is fired by the DesignerActionService in response to a DesignerActionCollection changing.
    ///  The event args contains information about the related object, the type of change (added or removed)
    ///  and the remaining DesignerActionCollection for the object. Note that when new DesignerActions are added,
    ///  if the related control/ is not yet parented - we add these actions to a "delay" list and they are later
    ///  created when the control is finally parented.
    /// </summary>
    private void OnDesignerActionsChanged(object sender, DesignerActionListsChangedEventArgs e)
    {
        // We need to invoke this async because the designer action service will raise this event from the thread pool.
        if (_marshalingControl is not null && _marshalingControl.IsHandleCreated)
        {
            _marshalingControl.BeginInvoke(new ActionChangedEventHandler(OnInvokedDesignerActionChanged), [sender, e]);
        }
    }

    private void OnDesignerActionUIStateChange(object sender, DesignerActionUIStateChangeEventArgs e)
    {
        if (e.RelatedObject is IComponent component)
        {
            DesignerActionGlyph? relatedGlyph = GetDesignerActionGlyph(component);
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
                    RecreatePanel(component);
                }
            }
        }
        else if (e.ChangeType == DesignerActionUIStateChangeType.Hide)
        {
            HideDesignerActionPanel();
        }
        else
        {
            Debug.Fail("related object is not an IComponent, something is wrong here...");
        }
    }

    /// <summary>
    ///  This is the same as DesignerActionChanged, but it is invoked on our control's thread
    /// </summary>
    private void OnInvokedDesignerActionChanged(object sender, DesignerActionListsChangedEventArgs e)
    {
        DesignerActionGlyph? g = null;
        if (e.ChangeType == DesignerActionListsChangedType.ActionListsAdded)
        {
            if (e.RelatedObject is not IComponent relatedComponent)
            {
                Debug.Fail("How can we add a DesignerAction glyphs when it's related object is not  an IComponent?");
                return;
            }

            IComponent? primSel = _selectionService.PrimarySelection as IComponent;
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

        if (e.ChangeType == DesignerActionListsChangedType.ActionListsRemoved && e.ActionLists!.Count == 0)
        {
            // only remove our glyph if there are no more DesignerActions associated with it.
            RemoveActionGlyph(e.RelatedObject);
        }
        else if (g is not null)
        {
            // we need to recreate the panel here, since it's content has changed...
            RecreatePanel(e.RelatedObject as IComponent);
        }
    }

    /// <summary>
    ///  Called when our KeyShowDesignerActions menu command is fired
    ///  (a.k.a. Alt+Shift+F10) - we will find the primary selection,
    ///  see if it has designer actions, and if so - show the menu.
    /// </summary>
    private void OnKeyShowDesignerActions(object? sender, EventArgs e)
    {
        ShowDesignerActionPanelForPrimarySelection();
    }

    // we cannot attach several menu command to the same command id, we need a single entry point,
    // we put it in designershortcutui. but we need a way to call the show ui on the related behavior
    // hence this internal function to hack it together. we return false if we have nothing to display,
    // we hide it and return true if we're already displaying
    internal bool ShowDesignerActionPanelForPrimarySelection()
    {
        // can't do anything w/o selection service
        if (_selectionService is null)
        {
            return false;
        }

        object? primarySelection = _selectionService.PrimarySelection;
        // verify that we have obtained a valid component with designer actions
        if (primarySelection is null || !_componentToGlyph.TryGetValue(primarySelection, out DesignerActionGlyph? glyph))
        {
            return false;
        }

        if (glyph.Behavior is DesignerActionBehavior behavior)
        // show the menu
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

        return false;
    }

    /// <summary>
    ///  When all the DesignerActions have been removed for a particular object,
    ///  we remove any UI (glyphs) that we may have been managing.
    /// </summary>
    internal void RemoveActionGlyph(object? relatedObject)
    {
        if (relatedObject is null)
        {
            return;
        }

        if (IsDesignerActionPanelVisible && relatedObject == _lastPanelComponent)
        {
            HideDesignerActionPanel();
        }

        if (!_componentToGlyph.TryGetValue(relatedObject, out DesignerActionGlyph? glyph))
        {
            return;
        }

        // Check ComponentTray first
        _serviceProvider.GetService<ComponentTray>()?.SelectionGlyphs?.Remove(glyph);

        _designerActionAdorner.Glyphs.Remove(glyph);
        _componentToGlyph.Remove(relatedObject);

        // we only do this when we're in a transaction, see bug VSWHIDBEY 418709.
        // This is for compatibility reason - infragistic. if we're not in a transaction, too bad, we don't update the screen
        if (_serviceProvider.TryGetService(out IDesignerHost? host) && host.InTransaction)
        {
            host.TransactionClosed += InvalidateGlyphOnLastTransaction;
            _relatedGlyphTransaction = glyph;
        }
    }

    private void InvalidateGlyphOnLastTransaction(object? sender, DesignerTransactionCloseEventArgs e)
    {
        if (e.LastTransaction)
        {
            if (_serviceProvider.TryGetService(out IDesignerHost? host))
            {
                host.TransactionClosed -= InvalidateGlyphOnLastTransaction;
            }

            _relatedGlyphTransaction?.InvalidateOwnerLocation();

            _relatedGlyphTransaction = null;
        }
    }

    internal void HideDesignerActionPanel()
    {
        if (IsDesignerActionPanelVisible)
        {
            _designerActionHost.Close();
        }
    }

    [MemberNotNullWhen(true, nameof(_designerActionHost))]
    internal bool IsDesignerActionPanelVisible
    {
        get => (_designerActionHost is not null && _designerActionHost.Visible);
    }

    internal IComponent? LastPanelComponent
    {
        get => (IsDesignerActionPanelVisible ? _lastPanelComponent : null);
    }

    private void ToolStripDropDown_Closing(object? sender, ToolStripDropDownClosingEventArgs e)
    {
        if (_cancelClose || e.Cancel)
        {
            e.Cancel = true;
            return;
        }

        if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
        {
            e.Cancel = true;
        }

        if (e.CloseReason == ToolStripDropDownCloseReason.Keyboard)
        {
            e.Cancel = false;
        }

        if (!e.Cancel)
        {
            // We WILL disappear
            Debug.Assert(_lastPanelComponent is not null, "last panel component should not be null here... " +
                "(except if you're currently debugging VS where deactivation messages in the middle of the pump can mess up everything...)");

            if (_lastPanelComponent is null)
            {
                return;
            }

            // If we're actually closing get the coordinate of the last message, the one causing us to close, is it within
            // the glyph coordinate. if it is that mean that someone just clicked back from the panel, on VS, but ON THE GLYPH,
            // that means that he actually wants to close it. The activation change is going to do that for us but we should
            // NOT reopen right away because he clicked on the glyph. This code is here to prevent this.
            Point point = DesignerUtils.LastCursorPoint;
            if (_componentToGlyph.TryGetValue(_lastPanelComponent, out DesignerActionGlyph? currentGlyph))
            {
                Point glyphCoord = GetGlyphLocationScreenCoord(_lastPanelComponent, currentGlyph);
                if ((new Rectangle(glyphCoord, new Size(currentGlyph.Bounds.Width, currentGlyph.Bounds.Height))).Contains(point))
                {
                    DesignerActionBehavior behavior = (DesignerActionBehavior)currentGlyph.Behavior!;
                    behavior.IgnoreNextMouseUp = true;
                }

                currentGlyph.InvalidateOwnerLocation();
            }

            _lastPanelComponent = null;
            // panel is going away, pop the behavior that's on the stack...
            Debug.Assert(_designerActionKeyboardBehavior is not null, $"why is {nameof(_designerActionKeyboardBehavior)} null?");
            Behavior? popBehavior = _behaviorService.PopBehavior(_designerActionKeyboardBehavior);
            Debug.Assert(popBehavior is DesignerActionKeyboardBehavior, "behavior returned is of the wrong kind?");
        }
    }

    internal Point UpdateDAPLocation(IComponent? component, DesignerActionGlyph? glyph)
    {
        component ??= _lastPanelComponent;

        if (_designerActionHost is null)
        {
            return Point.Empty;
        }

        if (component is null || glyph is null)
        {
            return _designerActionHost.Location;
        }

        // check that the glyph is still visible in the adorner window
        if (_behaviorService is not null &&
            !_behaviorService.AdornerWindowControl.DisplayRectangle.IntersectsWith(glyph.Bounds))
        {
            HideDesignerActionPanel();
            return _designerActionHost.Location;
        }

        Point glyphLocationScreenCoord = GetGlyphLocationScreenCoord(component, glyph);
        Rectangle rectGlyph = new(glyphLocationScreenCoord, glyph.Bounds.Size);
        Point pt = DesignerActionPanel.ComputePreferredDesktopLocation(rectGlyph, _designerActionHost.Size, out DockStyle edgeToDock);
        glyph.DockEdge = edgeToDock;
        _designerActionHost.Location = pt;
        return pt;
    }

    private Point GetGlyphLocationScreenCoord(IComponent relatedComponent, Glyph glyph)
    {
        Point glyphLocationScreenCoord = new(0, 0);
        if (relatedComponent is Control and not ToolStripDropDown)
        {
            glyphLocationScreenCoord = _behaviorService.AdornerWindowPointToScreen(glyph.Bounds.Location);
        }

        // ISSUE: we can't have this special cased here - we should find a more generic approach to solving this problem
        else if (relatedComponent is ToolStripItem item)
        {
            if (item.Owner is not null)
            {
                glyphLocationScreenCoord = _behaviorService.AdornerWindowPointToScreen(glyph.Bounds.Location);
            }
        }
        else
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
        if (_designerActionHost is null)
        {
            _designerActionHost = new DesignerActionToolStripDropDown(this, _mainParentWindow)
            {
                AutoSize = false,
                Padding = Padding.Empty,
                Renderer = new NoBorderRenderer(),
                Text = "DesignerActionTopLevelForm"
            };
            _designerActionHost.Closing += ToolStripDropDown_Closing;
        }

        // set the accessible name of the panel to the same title as the panel header. do that every time
        _designerActionHost.AccessibleName = string.Format(SR.DesignerActionPanel_DefaultPanelTitle, relatedComponent.GetType().Name);
        panel.AccessibleName = string.Format(SR.DesignerActionPanel_DefaultPanelTitle, relatedComponent.GetType().Name);

        _designerActionHost.SetDesignerActionPanel(panel, glyph);
        Point location = UpdateDAPLocation(relatedComponent, glyph);

        // check that the panel will have at least it's parent glyph visible on the adorner window
        if (_behaviorService is not null &&
            _behaviorService.AdornerWindowControl.DisplayRectangle.IntersectsWith(glyph.Bounds))
        {
            if (_mainParentWindow is not null && _mainParentWindow.Handle != 0)
            {
                PInvokeCore.SetWindowLong(
                    _designerActionHost,
                    WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT,
                    new HandleRef<HWND>(_mainParentWindow, (HWND)_mainParentWindow.Handle));
            }

            _cancelClose = true;
            _designerActionHost.Show(location);
            _designerActionHost.Focus();

            // When a control is drag and dropped and autoshow is set to true the vs designer is going to get activated
            // as soon as the control is dropped we don't want to close the panel then, so we post a message (using the
            // trick to call begin invoke) and once everything is settled re-activate the autoclose logic.
            _designerActionHost.BeginInvoke(OnShowComplete);

            // invalidate the glyph to have it point the other way
            glyph.InvalidateOwnerLocation();
            _lastPanelComponent = relatedComponent;
            // push new behavior for keyboard handling on the behavior stack
            _designerActionKeyboardBehavior = new DesignerActionKeyboardBehavior(_designerActionHost.CurrentPanel, _serviceProvider, _behaviorService);
            _behaviorService.PushBehavior(_designerActionKeyboardBehavior);
        }
    }

    private void OnShowComplete()
    {
        _cancelClose = false;

        // Force the panel to be the active window - for some reason someone else could have forced VS to become active
        // for real while we were ignoring close. This might be bad cause we'd be in a bad state.
        if (_designerActionHost is not null && _designerActionHost.Handle != 0 && _designerActionHost.Visible)
        {
            PInvoke.SetActiveWindow(_designerActionHost);
            _designerActionHost.CheckFocusIsRight();
        }
    }
}
