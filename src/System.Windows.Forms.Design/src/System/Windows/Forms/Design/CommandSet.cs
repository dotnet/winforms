// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design.Behavior;
using System.Drawing;
using System.Drawing.Design;
using System.Collections;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class implements the standard set of menu commands for
///  the form designer. This set of command is shared between
///  the form designer (and other UI-based form packages), and
///  composition designer, which doesn't manipulate controls.
///  Therefore, this set of command should only contain commands
///  that are common to both functions.
/// </summary>
/// <internalonly/>
internal partial class CommandSet : IDisposable
{
    protected ISite site;
    private readonly CommandSetItem[] _commandSet;
    private IMenuCommandService? _menuService;
    private IEventHandlerService _eventService;

    // Selection service fields. We keep some state about the
    // currently selected components so we can determine proper
    // command enabling quickly.
    //
    protected int selCount;                // the current selection count
    protected IComponent? primarySelection;        // the primary selection, or null
    private bool _selectionInherited;      // the selection contains inherited components
    protected bool controlsOnlySelection;   // is the selection containing only controls or are there components in it?

    // Selection sort constants
    //
    private const int SORT_HORIZONTAL = 0;
    private const int SORT_VERTICAL = 1;
    private const int SORT_ZORDER = 2;

    private const string CF_DESIGNER = "CF_DESIGNERCOMPONENTS_V2"; // See VSWhidbey #172531

    // these are used for snapping control via keyboard movement
    protected DragAssistanceManager? dragManager; // point to the snapline engine (only valid between keydown and timer expiration)
    private Timer? _snapLineTimer; // used to track the time from when a snapline is rendered until it should expire
    private BehaviorService? _behaviorService; // demand created pointer to the behaviorservice
    private StatusCommandUI _statusCommandUI; // Used to update the statusBar Information.
    private readonly IUIService? _uiService;

    /// <summary>
    ///  Creates a new CommandSet object. This object implements the set
    ///  of commands that the UI.Win32 form designer offers.
    /// </summary>
    public CommandSet(ISite site)
    {
        this.site = site;

        _eventService = site.GetRequiredService<IEventHandlerService>();
        _eventService.EventHandlerChanged += OnEventHandlerChanged;

        if (site.TryGetService(out IDesignerHost? host))
        {
            host.Activated += UpdateClipboardItems;
        }

        _statusCommandUI = new StatusCommandUI(site);

        _uiService = site.GetService<IUIService>();

        // Establish our set of commands
        _commandSet =
        [
            // Editing commands
            new(
                this,
                OnStatusDelete,
                OnMenuDelete,
                StandardCommands.Delete,
                _uiService),

            new(
                this,
                OnStatusCopy,
                OnMenuCopy,
                StandardCommands.Copy,
                _uiService),

            new(
                this,
                OnStatusCut,
                OnMenuCut,
                StandardCommands.Cut,
                _uiService),

            new ImmediateCommandSetItem(
                this,
                OnStatusPaste,
                OnMenuPaste,
                StandardCommands.Paste,
                _uiService),

            // Miscellaneous commands
            new(
                this,
                OnStatusSelectAll,
                OnMenuSelectAll,
                StandardCommands.SelectAll, true,
                _uiService),

            new(
                this,
                OnStatusAlways,
                OnMenuDesignerProperties,
                MenuCommands.DesignerProperties,
                _uiService),

            // Keyboard commands
            new(
                this,
                OnStatusAlways,
                OnKeyCancel,
                MenuCommands.KeyCancel,
                _uiService),

            new(
                this,
                OnStatusAlways,
                OnKeyCancel,
                MenuCommands.KeyReverseCancel,
                _uiService),

            new(
                this,
                OnStatusPrimarySelection,
                OnKeyDefault,
                MenuCommands.KeyDefaultAction, true,
                _uiService),

            new(
                this,
                OnStatusAnySelection,
                OnKeyMove,
                MenuCommands.KeyMoveUp, true,
                _uiService),

            new(
                this,
                OnStatusAnySelection,
                OnKeyMove,
                MenuCommands.KeyMoveDown, true,
                _uiService),

            new(
                this,
                OnStatusAnySelection,
                OnKeyMove,
                MenuCommands.KeyMoveLeft, true,
                _uiService),

            new(
                this,
                OnStatusAnySelection,
                OnKeyMove,
                MenuCommands.KeyMoveRight, true),

            new(
                this,
                OnStatusAnySelection,
                OnKeyMove,
                MenuCommands.KeyNudgeUp, true,
                _uiService),

            new(
                this,
                OnStatusAnySelection,
                OnKeyMove,
                MenuCommands.KeyNudgeDown, true,
                _uiService),

            new(
                this,
                OnStatusAnySelection,
                OnKeyMove,
                MenuCommands.KeyNudgeLeft, true,
                _uiService),

            new(
                this,
                OnStatusAnySelection,
                OnKeyMove,
                MenuCommands.KeyNudgeRight, true,
                _uiService),
        ];

        SelectionService = site.GetService<ISelectionService>();
        Debug.Assert(SelectionService is not null, "CommandSet relies on the selection service, which is unavailable.");
        if (SelectionService is not null)
        {
            SelectionService.SelectionChanged += OnSelectionChanged;
        }

        IMenuCommandService? menuService = MenuService;
        if (menuService is not null)
        {
            for (int i = 0; i < _commandSet.Length; i++)
            {
                menuService.AddCommand(_commandSet[i]);
            }
        }

        // Now setup the default command GUID for this designer. This GUID is also used in our toolbar
        // definition file to identify toolbars we own. We store the GUID in a command ID here in the
        // dictionary of the root component. Our host may pull this GUID out and use it.
        //
        IDictionaryService? ds = site.GetService<IDictionaryService>();
        Debug.Assert(ds is not null, "No dictionary service");
        ds?.SetValue(typeof(CommandID), new CommandID(new Guid("BA09E2AF-9DF2-4068-B2F0-4C7E5CC19E2F"), 0));
    }

    /// <summary>
    ///  Demand creates a pointer to the BehaviorService
    /// </summary>
    protected BehaviorService? BehaviorService => _behaviorService ??= GetService<BehaviorService>();

    /// <summary>
    ///  Retrieves the menu command service, which the command set
    ///  typically uses quite a bit.
    /// </summary>
    protected IMenuCommandService? MenuService => _menuService ??= GetService<IMenuCommandService>();

    /// <summary>
    ///  Retrieves the selection service, which the command set
    ///  typically uses quite a bit.
    /// </summary>
    protected ISelectionService? SelectionService { get; private set; }

    /// <summary>
    ///  This is the counter of selection changes.
    /// </summary>
    protected int SelectionVersion { get; private set; } = 1;

    /// <summary>
    ///  This property demand creates our snaplinetimer used to
    ///  track how long we'll leave snaplines on the screen before
    ///  erasing them
    /// </summary>
    protected Timer SnapLineTimer
    {
        get
        {
            if (_snapLineTimer is null)
            {
                // instantiate our snapline timer
                _snapLineTimer = new Timer
                {
                    Interval = DesignerUtils.s_snapLineDelay
                };

                _snapLineTimer.Tick += OnSnapLineTimerExpire;
            }

            return _snapLineTimer;
        }
    }

    /// <summary>
    ///  Checks if an object supports ComponentEditors, and optionally launches
    ///  the editor.
    /// </summary>
    private bool CheckComponentEditor([NotNullWhen(true)] object? obj, bool launchEditor)
    {
        if (obj is IComponent)
        {
            try
            {
                if (!launchEditor)
                {
                    return true;
                }

                if (!TypeDescriptorHelper.TryGetEditor(obj, out ComponentEditor? editor))
                {
                    return false;
                }

                if (TryGetService(out IComponentChangeService? changeService))
                {
                    try
                    {
                        changeService.OnComponentChanging(obj, null);
                    }
                    catch (CheckoutException coEx) when (coEx == CheckoutException.Canceled)
                    {
                        return false;
                    }
                    catch
                    {
                        Debug.Fail("non-CLS compliant exception");
                        throw;
                    }
                }

                bool success;

                if (editor is WindowsFormsComponentEditor winEditor)
                {
                    IWin32Window? parent = null;

                    // REVIEW: This smells wrong
                    if (obj is IWin32Window)
                    {
#pragma warning disable 1717 // assignment to self
                        parent = parent;
#pragma warning restore 1717

                    }

                    success = winEditor.EditComponent(obj, parent);
                }
                else
                {
                    success = editor.EditComponent(obj);
                }

                if (success)
                {
                    // Notify the change service that the change was successful.
                    changeService?.OnComponentChanged(obj);
                }

                return true;
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
            }
        }

        return false;
    }

    /// <summary>
    ///  Disposes of this object, removing all commands from the menu service.
    /// </summary>

    // We don't need to Dispose snapLineTimer
    public virtual void Dispose()
    {
        if (_menuService is not null)
        {
            for (int i = 0; i < _commandSet.Length; i++)
            {
                _menuService.RemoveCommand(_commandSet[i]);
                _commandSet[i].Dispose();
            }

            _menuService = null;
        }

        if (SelectionService is not null)
        {
            SelectionService.SelectionChanged -= OnSelectionChanged;
            SelectionService = null;
        }

        if (_eventService is not null)
        {
            _eventService.EventHandlerChanged -= OnEventHandlerChanged;
            _eventService = null!;
        }

        if (site.TryGetService(out IDesignerHost? host))
        {
            host.Activated -= UpdateClipboardItems;
        }

        if (_snapLineTimer is not null)
        {
            _snapLineTimer.Stop();
            _snapLineTimer.Tick -= OnSnapLineTimerExpire;
            _snapLineTimer = null;
        }

        EndDragManager();
        _statusCommandUI = null!;
        site = null!;
    }

    /// <summary>
    ///  Properly cleans up our drag engine.
    /// </summary>
    protected void EndDragManager()
    {
        if (dragManager is not null)
        {
            _snapLineTimer?.Stop();

            dragManager.EraseSnapLines();
            dragManager.OnMouseUp();
            dragManager = null;
        }
    }

    // Returns true if the action is successful, false otherwise
    internal static bool ExecuteSafely(Action action, bool throwOnException)
    {
        try
        {
            action();
            return true;
        }
        catch when (!throwOnException)
        {
            return false;
        }
    }

    // This function will return true if call to func is successful, false otherwise
    // Output of call to func is available in result out parameter
    private static bool ExecuteSafely<T>(Func<T> func, bool throwOnException, [MaybeNullWhen(false)] out T result)
    {
        try
        {
            result = func();
            return true;
        }
        catch when (!throwOnException)
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    ///  Filters the set of selected components. The selection service will retrieve all
    ///  components that are currently selected. This method allows you to filter this
    ///  set down to components that match your criteria. The selectionRules parameter
    ///  must contain one or more flags from the SelectionRules class. These flags
    ///  allow you to constrain the set of selected objects to visible, movable,
    ///  sizeable or all objects.
    /// </summary>
    private IComponent[] FilterSelection(IComponent[]? components, SelectionRules selectionRules)
    {
        if (components is null)
            return [];

        // Mask off any selection object that doesn't adhere to the given ruleset.
        // We can ignore this if the ruleset is zero, as all components would be accepted.
        //
        if (selectionRules != SelectionRules.None)
        {
            if (TryGetService(out IDesignerHost? host))
            {
                List<IComponent> list = new(components.Length);
                foreach (IComponent comp in components)
                {
                    if (host.GetDesigner(comp) is ControlDesigner des && (des.SelectionRules & selectionRules) == selectionRules)
                    {
                        list.Add(comp);
                    }
                }

                return [.. list];
            }
        }

        return [];
    }

    /// <summary>
    ///  Used to retrieve the selection for a copy. The default implementation
    ///  retrieves the current selection.
    /// </summary>
    protected virtual ICollection GetCopySelection()
    {
        ICollection selectedComponents = SelectionService!.GetSelectedComponents();
        bool sort = false;
        IComponent[] comps = new IComponent[selectedComponents.Count];
        selectedComponents.CopyTo(comps, 0);

        foreach (IComponent comp in comps)
        {
            if (comp is Control)
            {
                sort = true;
                break;
            }
        }

        if (sort)
        {
            SortSelection(comps, SORT_ZORDER);
        }

        IDesignerHost? host = site.GetService<IDesignerHost>();
        if (host is not null)
        {
            List<IComponent> copySelection = [];
            foreach (IComponent comp in comps)
            {
                copySelection.Add(comp);
                GetAssociatedComponents(comp, host, copySelection);
            }

            return copySelection;
        }

        return comps;
    }

    private static void GetAssociatedComponents(IComponent component, IDesignerHost host, List<IComponent> list)
    {
        if (host.GetDesigner(component) is not ComponentDesigner designer)
        {
            return;
        }

        foreach (IComponent childComp in designer.AssociatedComponents)
        {
            if (childComp.Site is not null)
            {
                list.Add(childComp);
                GetAssociatedComponents(childComp, host, list);
            }
        }
    }

    /// <summary>
    ///  Used to retrieve the current location of the given component.
    /// </summary>
    private static Point GetLocation(IComponent comp)
    {
        PropertyDescriptor? prop = GetProperty(comp, "Location");

        if (prop is not null)
        {
            try
            {
                return (Point)prop.GetValue(comp)!;
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
                Debug.Fail("Commands may be disabled, the location property was not accessible", e.ToString());
            }
        }

        return Point.Empty;
    }

    /// <summary>
    ///  Retrieves the given property on the given component.
    /// </summary>
    protected static PropertyDescriptor? GetProperty(object comp, string propName)
    {
        return TypeDescriptor.GetProperties(comp)[propName];
    }

    /// <summary>
    ///  Retrieves the requested service.
    /// </summary>
    protected virtual object? GetService(Type serviceType)
    {
        return site?.GetService(serviceType);
    }

    /// <summary>
    ///  Retrieves the requested service.
    /// </summary>
    private protected T? GetService<T>() where T : class
    {
        return GetService(typeof(T)) as T;
    }

    /// <summary>
    ///  Retrieves the requested service.
    /// </summary>
    private protected bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : class
    {
        service = GetService(typeof(T)) as T;
        return service is not null;
    }

    /// <summary>
    ///  Used to retrieve the current size of the given component.
    /// </summary>
    private static Size GetSize(IComponent comp)
    {
        PropertyDescriptor? prop = GetProperty(comp, "Size");
        return prop is not null ? (Size)prop.GetValue(comp)! : Size.Empty;
    }

    /// <summary>
    ///  Retrieves the snap information for the given component.
    /// </summary>
    protected virtual void GetSnapInformation(IDesignerHost host, IComponent component, out Size snapSize, out IComponent snapComponent, out PropertyDescriptor? snapProperty)
    {
        // This implementation is shared by all. It just looks for snap properties on the base component.
        //
        IComponent currentSnapComponent = host.RootComponent;
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(currentSnapComponent);

        PropertyDescriptor? currentSnapProp = props["SnapToGrid"];
        if (currentSnapProp is not null && currentSnapProp.PropertyType != typeof(bool))
        {
            currentSnapProp = null;
        }

        PropertyDescriptor? gridSizeProp = props["GridSize"];
        if (gridSizeProp is not null && gridSizeProp.PropertyType != typeof(Size))
        {
            gridSizeProp = null;
        }

        // Finally, now that we've got the various properties and components, dole out the
        // values.
        //
        snapComponent = currentSnapComponent;
        snapProperty = currentSnapProp;
        snapSize = gridSizeProp is not null ? (Size)gridSizeProp.GetValue(snapComponent)! : Size.Empty;
    }

    /// <summary>
    ///  Called before doing any change to multiple controls
    ///  to check if we have the right to make any change
    ///  otherwise we would get a checkout message for each control we call setvalue on
    /// </summary>
    protected bool CanCheckout(IComponent comp)
    {
        // look if it's ok to change
        if (TryGetService(out IComponentChangeService? changeSvc))
        {
            try
            {
                changeSvc.OnComponentChanging(comp, null);
            }
            catch (CheckoutException chkex) when (chkex == CheckoutException.Canceled)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  Called by the event handler service when the current event handler
    ///  has changed. Here we invalidate all of our menu items so that
    ///  they can pick up the new event handler.
    /// </summary>
    private void OnEventHandlerChanged(object? sender, EventArgs e)
    {
        OnUpdateCommandStatus();
    }

    /// <summary>
    ///  Called for the two cancel commands we support.
    /// </summary>
    private void OnKeyCancel(object? sender, EventArgs e)
    {
        OnKeyCancel(sender);
    }

    /// <summary>
    ///  Called for the two cancel commands we support. Returns true
    ///  If we did anything with the cancel, or false if not.
    /// </summary>
    protected virtual bool OnKeyCancel(object? sender)
    {
        bool handled = false;

        // The base implementation here just checks to see if we are dragging.
        // If we are, then we abort the drag.
        //
        if (BehaviorService is not null && BehaviorService.HasCapture)
        {
            BehaviorService.OnLoseCapture();
            handled = true;
        }
        else
        {
            if (TryGetService(out IToolboxService? tbx) && tbx.GetSelectedToolboxItem(GetService<IDesignerHost>()) is not null)
            {
                tbx.SelectedToolboxItemUsed();

                PInvoke.GetCursorPos(out Point p);
                HWND hwnd = PInvoke.WindowFromPoint(p);
                if (!hwnd.IsNull)
                {
                    PInvokeCore.SendMessage(hwnd, PInvokeCore.WM_SETCURSOR, hwnd, (nint)PInvoke.HTCLIENT);
                }
                else
                {
                    Cursor.Current = Cursors.Default;
                }

                handled = true;
            }
        }

        return handled;
    }

    /// <summary>
    ///  Called for the "default" command, typically the Enter key.
    /// </summary>
    protected void OnKeyDefault(object? sender, EventArgs e)
    {
        // Return key. Handle it like a double-click on the
        // primary selection
        //
        if (SelectionService?.PrimarySelection is IComponent pri)
        {
            if (TryGetService(out IDesignerHost? host))
            {
                IDesigner? designer = host.GetDesigner(pri);

                designer?.DoDefaultAction();
            }
        }
    }

    /// <summary>
    ///  Called for all cursor movement commands.
    /// </summary>
    protected virtual void OnKeyMove(object? sender, EventArgs e)
    {
        // Arrow keys. Begin a drag if the selection isn't locked.
        //

        if (SelectionService?.PrimarySelection is IComponent comp && TryGetService(out IDesignerHost? host) &&
            (!TypeDescriptorHelper.TryGetPropertyValue(comp, "Locked", out bool b) || !b))
        {
            CommandID? cmd = ((MenuCommand)sender!).CommandID;
            bool invertSnap = false;
            int moveOffsetX = 0;
            int moveOffsetY = 0;

            if (Equals(cmd, MenuCommands.KeyMoveUp))
            {
                moveOffsetY = -1;
            }
            else if (Equals(cmd, MenuCommands.KeyMoveDown))
            {
                moveOffsetY = 1;
            }
            else if (Equals(cmd, MenuCommands.KeyMoveLeft))
            {
                moveOffsetX = -1;
            }
            else if (Equals(cmd, MenuCommands.KeyMoveRight))
            {
                moveOffsetX = 1;
            }
            else if (Equals(cmd, MenuCommands.KeyNudgeUp))
            {
                moveOffsetY = -1;
                invertSnap = true;
            }
            else if (Equals(cmd, MenuCommands.KeyNudgeDown))
            {
                moveOffsetY = 1;
                invertSnap = true;
            }
            else if (Equals(cmd, MenuCommands.KeyNudgeLeft))
            {
                moveOffsetX = -1;
                invertSnap = true;
            }
            else if (Equals(cmd, MenuCommands.KeyNudgeRight))
            {
                moveOffsetX = 1;
                invertSnap = true;
            }
            else
            {
                Debug.Fail($"Unknown command mapped to OnKeyMove: {cmd}");
            }

            DesignerTransaction trans = SelectionService.SelectionCount > 1
                ? host.CreateTransaction(string.Format(SR.DragDropMoveComponents, SelectionService.SelectionCount))
                : host.CreateTransaction(string.Format(SR.DragDropMoveComponent, comp.Site?.Name));

            try
            {
                // if we can find the behaviorservice, then we can use it and the SnapLineEngine to help us
                // move these controls...
                if (BehaviorService is not null)
                {
                    Control? primaryControl = comp as Control; // this can be null (when we are moving a component in the ComponentTray)

                    bool useSnapLines = BehaviorService.UseSnapLines;

                    // If we have previous snaplines, we always want to erase them, no matter what. VS Whidbey #397709
                    if (dragManager is not null)
                    {
                        EndDragManager();
                    }

                    // If we CTRL+Arrow and we're using SnapLines - snap to the next location
                    // Don't snap if we are moving a component in the ComponentTray
                    if (invertSnap && useSnapLines && primaryControl is not null && comp.Site is not null)
                    {
                        List<IComponent> selComps = [..SelectionService.GetSelectedComponents().Cast<IComponent>()];

                        // create our snapline engine
                        dragManager = new DragAssistanceManager(comp.Site, selComps);

                        // ask our snapline engine to find the nearest snap position with the given direction
                        Point snappedOffset = dragManager.OffsetToNearestSnapLocation(primaryControl, new Point(moveOffsetX, moveOffsetY));

                        // update the offset according to the snapline engine

                        // This is the offset assuming origin is in the upper-left.
                        moveOffsetX = snappedOffset.X;
                        moveOffsetY = snappedOffset.Y;

                        // If the parent is mirrored then we need to negate moveOffsetX.
                        // This is because moveOffsetX assumes that the origin
                        // is upper left. That is, when moveOffsetX is positive, we
                        // are moving right, negative when moving left.

                        // The parent container's origin depends on its mirroring property.
                        // Thus when we call propLoc.setValue below, we need to make sure
                        // that our moveOffset.X correctly reflects the placement of the
                        // parent container's origin.

                        // We need to do this AFTER we calculate the snappedOffset.
                        // This is because the dragManager calculations are all based
                        // on an origin in the upper-left.
                        if (primaryControl.Parent!.IsMirrored)
                        {
                            moveOffsetX *= -1;
                        }
                    }

                    // if we used a regular arrow key and we're in SnapToGrid mode...

                    else if (!invertSnap && !useSnapLines)
                    {
                        bool snapOn = false;
                        GetSnapInformation(host, comp, out Size snapSize, out IComponent snapComponent, out PropertyDescriptor? snapProperty);

                        if (snapProperty is not null)
                        {
                            snapOn = (bool)snapProperty.GetValue(snapComponent)!;
                        }

                        if (snapOn && !snapSize.IsEmpty)
                        {
                            moveOffsetX *= snapSize.Width;
                            moveOffsetY *= snapSize.Height;

                            if (primaryControl is not null)
                            {
                                // ask the parent to adjust our wanna-be snapped position
                                if (host.GetDesigner(primaryControl.Parent!) is ParentControlDesigner parentDesigner)
                                {
                                    Point loc = primaryControl.Location;

                                    // If the parent is mirrored then we need to negate moveOffsetX.
                                    // This is because moveOffsetX assumes that the origin
                                    // is upper left. That is, when moveOffsetX is positive, we
                                    // are moving right, negative when moving left.

                                    // The parent container's origin depends on its mirroring property.
                                    // Thus when we call propLoc.setValue below, we need to make sure
                                    // that our moveOffset.X correctly reflects the placement of the
                                    // parent container's origin.

                                    // Should do this BEFORE we get the snapped point.
                                    if (primaryControl.Parent!.IsMirrored)
                                    {
                                        moveOffsetX *= -1;
                                    }

                                    loc.Offset(moveOffsetX, moveOffsetY);

                                    loc = parentDesigner.GetSnappedPoint(loc);

                                    // reset our offsets now that we've snapped correctly
                                    if (moveOffsetX != 0)
                                    {
                                        moveOffsetX = loc.X - primaryControl.Location.X;
                                    }

                                    if (moveOffsetY != 0)
                                    {
                                        moveOffsetY = loc.Y - primaryControl.Location.Y;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // In this case we are just going to move 1 pixel, so let's adjust for Mirroring
                            if (primaryControl is not null && primaryControl.Parent!.IsMirrored)
                            {
                                moveOffsetX *= -1;
                            }
                        }
                    }
                    else
                    {
                        if (primaryControl is not null && primaryControl.Parent!.IsMirrored)
                        {
                            moveOffsetX *= -1;
                        }
                    }

                    SelectionRules rules = SelectionRules.Moveable | SelectionRules.Visible;
                    foreach (IComponent component in SelectionService.GetSelectedComponents())
                    {
                        if (host.GetDesigner(component) is ControlDesigner des && ((des.SelectionRules & rules) != rules))
                        {
                            // the control must match the rules, if not, then we don't move it
                            continue;
                        }

                        // Components are always moveable and visible

                        PropertyDescriptor? propLoc = TypeDescriptor.GetProperties(component)["Location"];
                        if (propLoc is not null)
                        {
                            Point loc = (Point)propLoc.GetValue(component)!;
                            loc.Offset(moveOffsetX, moveOffsetY);
                            propLoc.SetValue(component, loc);
                        }

                        // change the Status information ....
                        if (component == SelectionService.PrimarySelection)
                        {
                            _statusCommandUI?.SetStatusInformation(component as Component);
                        }
                    }
                }
            }
            finally
            {
                trans?.Commit();

                if (dragManager is not null)
                {
                    // start our timer for the snaplines
                    SnapLineTimer.Start();

                    // render any lines
                    dragManager.RenderSnapLinesInternal();
                }
            }
        }
    }

    /// <summary>
    ///  Called for all alignment operations that key off of a primary
    ///  selection.
    /// </summary>
    protected void OnMenuAlignByPrimary(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        CommandID id = cmd.CommandID!;

        // Need to get the location for the primary control, we do this here
        // (instead of onselectionchange) because the control could be dragged
        // around once it is selected and might have a new location
        Point primaryLocation = GetLocation(primarySelection!);
        Size primarySize = GetSize(primarySelection!);

        if (SelectionService is null)
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            // Now loop through each of the components.
            ICollection comps = SelectionService.GetSelectedComponents();

            // Inform the designer that we are about to monkey with a ton of properties.
            IDesignerHost? host = GetService<IDesignerHost>();
            DesignerTransaction? trans = null;
            try
            {
                trans = host?.CreateTransaction(string.Format(SR.CommandSetAlignByPrimary, comps.Count));

                bool firstTry = true;
                Point loc = Point.Empty;
                foreach (IComponent comp in comps)
                {
                    if (comp == primarySelection)
                    {
                        continue;
                    }

                    if (host?.GetDesigner(comp) is not ControlDesigner)
                    {
                        continue;
                    }

                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);

                    PropertyDescriptor? locProp = props["Location"];
                    PropertyDescriptor? sizeProp = props["Size"];
                    PropertyDescriptor? lockProp = props["Locked"];

                    // Skip all components that are locked
                    if (lockProp is not null)
                    {
                        if ((bool)lockProp.GetValue(comp)!)
                            continue;
                    }

                    // Skip all components that don't have a location property
                    if (locProp is null || locProp.IsReadOnly)
                    {
                        continue;
                    }

                    // Skip all components that don't have size if we're doing a size operation.
                    if (id.Equals(StandardCommands.AlignBottom)
                        || id.Equals(StandardCommands.AlignHorizontalCenters)
                        || id.Equals(StandardCommands.AlignVerticalCenters)
                        || id.Equals(StandardCommands.AlignRight))
                    {
                        if (sizeProp is null || sizeProp.IsReadOnly)
                        {
                            continue;
                        }

                        Size size = (Size)sizeProp.GetValue(comp)!;

                        // Align bottom
                        //
                        if (id.Equals(StandardCommands.AlignBottom))
                        {
                            loc = (Point)locProp.GetValue(comp)!;
                            loc.Y = primaryLocation.Y + primarySize.Height - size.Height;
                        }

                        // Align horizontal centers
                        //
                        else if (id.Equals(StandardCommands.AlignHorizontalCenters))
                        {
                            loc = (Point)locProp.GetValue(comp)!;
                            loc.Y = primarySize.Height / 2 + primaryLocation.Y - size.Height / 2;
                        }

                        // Align right
                        //
                        else if (id.Equals(StandardCommands.AlignRight))
                        {
                            loc = (Point)locProp.GetValue(comp)!;
                            loc.X = primaryLocation.X + primarySize.Width - size.Width;
                        }

                        // Align vertical centers
                        //
                        else if (id.Equals(StandardCommands.AlignVerticalCenters))
                        {
                            loc = (Point)locProp.GetValue(comp)!;
                            loc.X = primarySize.Width / 2 + primaryLocation.X - size.Width / 2;
                        }
                    }

                    // Align left
                    //
                    else if (id.Equals(StandardCommands.AlignLeft))
                    {
                        loc = (Point)locProp.GetValue(comp)!;
                        loc.X = primaryLocation.X;
                    }

                    // Align top
                    //
                    else if (id.Equals(StandardCommands.AlignTop))
                    {
                        loc = (Point)locProp.GetValue(comp)!;
                        loc.Y = primaryLocation.Y;
                    }
                    else
                    {
                        Debug.Fail($"Unrecognized command: {id}");
                    }

                    if (firstTry && !CanCheckout(comp))
                    {
                        return;
                    }

                    firstTry = false;

                    locProp.SetValue(comp, loc);
                }
            }
            finally
            {
                trans?.Commit();
            }
        }
        finally
        {
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the align->to grid menu item is selected.
    /// </summary>
    protected void OnMenuAlignToGrid(object? sender, EventArgs e)
    {
        Size gridSize = Size.Empty;
        int delta;

        if (SelectionService is null)
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            ICollection selectedComponents = SelectionService.GetSelectedComponents();
            IDesignerHost? host = GetService<IDesignerHost>();
            DesignerTransaction? trans = null;

            try
            {
                if (host is not null)
                {
                    trans = host.CreateTransaction(string.Format(SR.CommandSetAlignToGrid, selectedComponents.Count));

                    if (host.RootComponent is Control baseComponent)
                    {
                        PropertyDescriptor? prop = GetProperty(baseComponent, "GridSize");
                        if (prop is not null)
                        {
                            gridSize = (Size)prop.GetValue(baseComponent)!;
                        }

                        if (prop is null || gridSize.IsEmpty)
                        {
                            // bail silently here
                            return;
                        }
                    }
                }

                bool firstTry = true;

                // For each component, we round to the nearest snap offset for x and y.
                foreach (IComponent comp in selectedComponents)
                {
                    // first check to see if the component is locked, if so - don't move it...
                    PropertyDescriptor? lockedProp = GetProperty(comp, "Locked");
                    if (lockedProp is not null && ((bool)lockedProp.GetValue(comp)!))
                    {
                        continue;
                    }

                    // if the designer for this component isn't a ControlDesigner (maybe
                    // it's something in the component tray) then don't try to align it to grid.
                    //
                    if (host is not null && host.GetDesigner(comp) is not ControlDesigner)
                    {
                        continue;
                    }

                    // get the location property
                    PropertyDescriptor? locProp = GetProperty(comp, "Location");

                    // get the current value
                    if (locProp is null || locProp.IsReadOnly)
                    {
                        continue;
                    }

                    var loc = (Point)locProp.GetValue(comp)!;

                    // round the x to the snap size
                    delta = loc.X % gridSize.Width;
                    if (delta < (gridSize.Width / 2))
                    {
                        loc.X -= delta;
                    }
                    else
                    {
                        loc.X += (gridSize.Width - delta);
                    }

                    // round the y to the gridsize
                    delta = loc.Y % gridSize.Height;
                    if (delta < (gridSize.Height / 2))
                    {
                        loc.Y -= delta;
                    }
                    else
                    {
                        loc.Y += (gridSize.Height - delta);
                    }

                    // look if it's ok to change
                    if (firstTry && !CanCheckout(comp))
                    {
                        return;
                    }

                    firstTry = false;

                    // set the value
                    locProp.SetValue(comp, loc);
                }
            }
            finally
            {
                trans?.Commit();
            }
        }
        finally
        {
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the center horizontally or center vertically menu item is selected.
    /// </summary>
    protected void OnMenuCenterSelection(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        CommandID? cmdID = cmd.CommandID;

        if (SelectionService is null)
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            // NOTE: this only works on Control types
            ICollection selectedComponents = SelectionService.GetSelectedComponents();
            Control? viewParent = null;

            IDesignerHost? host = GetService<IDesignerHost>();
            DesignerTransaction? trans = null;

            try
            {
                if (host is not null)
                {
                    string batchString = cmdID == StandardCommands.CenterHorizontally
                        ? string.Format(SR.WindowsFormsCommandCenterX, selectedComponents.Count)
                        : string.Format(SR.WindowsFormsCommandCenterY, selectedComponents.Count);

                    trans = host.CreateTransaction(batchString);
                }

                int top = int.MaxValue;
                int left = int.MaxValue;
                int right = int.MinValue;
                int bottom = int.MinValue;

                foreach (object obj in selectedComponents)
                {
                    if (obj is Control comp)
                    {
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);

                        PropertyDescriptor? locProp = props["Location"];
                        PropertyDescriptor? sizeProp = props["Size"];

                        // Skip all components that don't have location and size properties
                        if (locProp is null || sizeProp is null || locProp.IsReadOnly || sizeProp.IsReadOnly)
                        {
                            continue;
                        }

                        // Also, skip all locked components.
                        PropertyDescriptor? lockProp = props["Locked"];
                        if (lockProp is not null && (bool)lockProp.GetValue(comp)!)
                        {
                            continue;
                        }

                        Size size = (Size)sizeProp.GetValue(comp)!;
                        Point loc = (Point)locProp.GetValue(comp)!;

                        // cache the first parent we see - if there's a mix of different parents - we'll
                        // just center based on the first one
                        viewParent ??= comp.Parent;

                        if (loc.X < left)
                            left = loc.X;
                        if (loc.Y < top)
                            top = loc.Y;
                        if (loc.X + size.Width > right)
                            right = loc.X + size.Width;
                        if (loc.Y + size.Height > bottom)
                            bottom = loc.Y + size.Height;
                    }
                }

                // if we never found a viewParent (some read-only inherited scenarios
                // then simply bail
                if (viewParent is null)
                {
                    return;
                }

                int centerOfUnionRectX = (left + right) / 2;
                int centerOfUnionRectY = (top + bottom) / 2;

                int centerOfParentX = (viewParent.ClientSize.Width) / 2;
                int centerOfParentY = (viewParent.ClientSize.Height) / 2;

                bool shiftRight = centerOfParentX >= centerOfUnionRectX;
                bool shiftBottom = centerOfParentY >= centerOfUnionRectY;

                int deltaX = shiftRight ? centerOfParentX - centerOfUnionRectX : centerOfUnionRectX - centerOfParentX;
                int deltaY = shiftBottom ? centerOfParentY - centerOfUnionRectY : centerOfUnionRectY - centerOfParentY;

                bool firstTry = true;
                foreach (object obj in selectedComponents)
                {
                    if (obj is Control comp)
                    {
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);

                        PropertyDescriptor locProp = props["Location"]!;
                        if (locProp.IsReadOnly)
                        {
                            continue;
                        }

                        Point loc = (Point)locProp.GetValue(comp)!;

                        if (cmdID == StandardCommands.CenterHorizontally)
                        {
                            if (shiftRight)
                                loc.X += deltaX;
                            else
                                loc.X -= deltaX;
                        }
                        else if (cmdID == StandardCommands.CenterVertically)
                        {
                            if (shiftBottom)
                                loc.Y += deltaY;
                            else
                                loc.Y -= deltaY;
                        }

                        // look if it's ok to change the first time
                        if (firstTry && !CanCheckout(comp))
                        {
                            return;
                        }

                        firstTry = false;
                        // do the change
                        locProp.SetValue(comp, loc);
                    }
                }
            }
            finally
            {
                trans?.Commit();
            }
        }
        finally
        {
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the copy menu item is selected.
    /// </summary>
    protected void OnMenuCopy(object? sender, EventArgs e)
    {
        if (SelectionService is null)
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            ICollection selectedComponents = GetCopySelection();

            selectedComponents = PrependComponentNames(selectedComponents);

            IDesignerSerializationService? ds = GetService<IDesignerSerializationService>();
            Debug.Assert(ds is not null, "No designer serialization service -- we cannot copy to clipboard");
            if (ds is not null)
            {
                object serializationData = ds.Serialize(selectedComponents);
                using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                new BinaryFormatter().Serialize(stream, serializationData);
#pragma warning restore SYSLIB0011
                stream.Seek(0, SeekOrigin.Begin);
                byte[] bytes = stream.GetBuffer();
                IDataObject dataObj = new DataObject(CF_DESIGNER, bytes);
                if (!ExecuteSafely(() => Clipboard.SetDataObject(dataObj), throwOnException: false))
                {
                    _uiService?.ShowError(SR.ClipboardError);
                }
            }

            UpdateClipboardItems(null, null);
        }
        finally
        {
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the cut menu item is selected.
    /// </summary>
    protected void OnMenuCut(object? sender, EventArgs e)
    {
        if (SelectionService is null)
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            ICollection selectedComponents = GetCopySelection();
            int cutCount = selectedComponents.Count;

            selectedComponents = PrependComponentNames(selectedComponents);
            IDesignerSerializationService? ds = GetService<IDesignerSerializationService>();
            Debug.Assert(ds is not null, "No designer serialization service -- we cannot copy to clipboard");
            if (ds is not null)
            {
                object serializationData = ds.Serialize(selectedComponents);
                using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                new BinaryFormatter().Serialize(stream, serializationData);
#pragma warning restore SYSLIB0011
                stream.Seek(0, SeekOrigin.Begin);
                byte[] bytes = stream.GetBuffer();
                IDataObject dataObj = new DataObject(CF_DESIGNER, bytes);

                if (ExecuteSafely(() => Clipboard.SetDataObject(dataObj), throwOnException: false))
                {
                    Control? commonParent = null;

                    if (TryGetService(out IDesignerHost? host))
                    {
                        IComponentChangeService? changeService = GetService<IComponentChangeService>();
                        DesignerTransaction? trans = null;

                        List<ParentControlDesigner> designerList = [];
                        try
                        {
                            trans = host.CreateTransaction(string.Format(SR.CommandSetCutMultiple, cutCount));

                            // clear the selected components so we aren't browsing them
                            //
                            SelectionService.SetSelectedComponents(Array.Empty<object>(), SelectionTypes.Replace);

                            object[] selComps = new object[selectedComponents.Count];
                            selectedComponents.CopyTo(selComps, 0);

                            foreach (object obj in selComps)
                            {
                                // We should never delete the base component.
                                //
                                if (obj == host.RootComponent || obj is not Control c)
                                {
                                    continue;
                                }

                                // Perf: We suspend Component Changing Events on parent for bulk changes
                                // to avoid unnecessary serialization\deserialization for undo
                                // see bug 488115
                                Control? parent = c.Parent;
                                if (parent is not null
                                    && host.GetDesigner(parent) is ParentControlDesigner designer
                                    && !designerList.Contains(designer))
                                {
                                    designer.SuspendChangingEvents();
                                    designerList.Add(designer);
                                    designer.ForceComponentChanging();
                                }
                            }

                            // go backward so we destroy parents before children

                            foreach (object obj in selComps)
                            {
                                // We should never delete the base component.
                                //
                                if (obj == host.RootComponent || obj is not IComponent component)
                                {
                                    continue;
                                }

                                // VSWhidbey # 370813.
                                // Cannot use idx = 1 to check (see diff) due to the call to PrependComponentNames, which
                                // adds non IComponent objects to the beginning of selectedComponents. Thus when we finally get
                                // here idx would be > 1.
                                if (obj is Control selectedControl)
                                {
                                    if (commonParent is null)
                                    {
                                        commonParent = selectedControl.Parent;
                                    }
                                    else if (selectedControl.Parent != commonParent && !commonParent.Contains(selectedControl))
                                    {
                                        // look for internal parenting
                                        commonParent = selectedControl == commonParent || selectedControl.Contains(commonParent) ? selectedControl.Parent : null;
                                    }
                                }

                                if (changeService is not null)
                                {
                                    List<IComponent> al = [];
                                    GetAssociatedComponents(component, host, al);
                                    foreach (IComponent comp in al)
                                    {
                                        changeService.OnComponentChanging(comp, null);
                                    }
                                }

                                host.DestroyComponent(component);
                            }
                        }
                        finally
                        {
                            trans?.Commit();
                            foreach (ParentControlDesigner des in designerList)
                            {
                                des.ResumeChangingEvents();
                            }
                        }

                        if (commonParent is not null)
                        {
                            SelectionService.SetSelectedComponents(new object[] { commonParent }, SelectionTypes.Replace);
                        }
                        else if (SelectionService.PrimarySelection is null)
                        {
                            SelectionService.SetSelectedComponents(new object[] { host.RootComponent }, SelectionTypes.Replace);
                        }
                    }
                }
                else
                {
                    _uiService?.ShowError(SR.ClipboardError);
                }
            }
        }
        finally
        {
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the delete menu item is selected.
    /// </summary>
    protected void OnMenuDelete(object? sender, EventArgs e)
    {
        if (site is null || SelectionService is null || !TryGetService(out IDesignerHost? host))
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            IComponentChangeService? changeService = GetService<IComponentChangeService>();

            ICollection comps = SelectionService.GetSelectedComponents();
            string desc = string.Format(SR.CommandSetDelete, comps.Count);

            DesignerTransaction? trans = null;
            IComponent? commonParent = null;
            bool commonParentSet = false;
            List<ParentControlDesigner> designerList = [];

            try
            {
                trans = host.CreateTransaction(desc);
                SelectionService.SetSelectedComponents(Array.Empty<object>(), SelectionTypes.Replace);
                foreach (object obj in comps)
                {
                    if (obj is not IComponent comp || comp.Site is null)
                    {
                        continue;
                    }

                    // Perf: Suspend ComponentChanging Events on parent for bulk changes to avoid unnecessary
                    // serialization\deserialization for undo.
                    if (obj is Control c)
                    {
                        Control? parent = c.Parent;
                        if (parent is not null)
                        {
                            if (host.GetDesigner(parent) is ParentControlDesigner designer
                                && !designerList.Contains(designer))
                            {
                                designer.SuspendChangingEvents();
                                designerList.Add(designer);
                                designer.ForceComponentChanging();
                            }
                        }
                    }
                }

                foreach (object obj in comps)
                {
                    // If it's not a component, we can't delete it. It also may have already been deleted
                    // as part of a parent operation, so we skip it.
                    if (obj is not IComponent c || c.Site is null)
                    {
                        continue;
                    }

                    // We should never delete the base component.
                    if (obj == host.RootComponent)
                    {
                        continue;
                    }

                    if (!commonParentSet)
                    {
                        if (obj is Control control)
                        {
                            commonParent = control.Parent;
                        }
                        else
                        {
                            // If this is not a Control, see if we can get an ITreeDesigner from it,
                            // and figure out the Component from that.
                            if (host.GetDesigner((IComponent)obj) is ITreeDesigner designer)
                            {
                                IDesigner? parentDesigner = designer.Parent;
                                if (parentDesigner is not null)
                                {
                                    commonParent = parentDesigner.Component;
                                }
                            }
                        }

                        commonParentSet = (commonParent is not null);
                    }
                    else if (commonParent is not null)
                    {
                        if (obj is Control selectedControl && commonParent is Control controlCommonParent)
                        {
                            if (selectedControl.Parent != controlCommonParent && !controlCommonParent.Contains(selectedControl))
                            {
                                // look for internal parenting
                                if (selectedControl == controlCommonParent || selectedControl.Contains(controlCommonParent))
                                {
                                    commonParent = selectedControl.Parent;
                                }
                                else
                                {
                                    Control? parent = controlCommonParent;
                                    // start walking up until we find a common parent
                                    while (parent is not null && !parent.Contains(selectedControl))
                                    {
                                        parent = parent.Parent;
                                    }

                                    commonParent = parent;
                                }
                            }
                        }
                        else
                        {
                            // For these we aren't as thorough as we are with the Control-based ones.
                            // we just walk up the chain until we find that parent or the root component.
                            if (host.GetDesigner(c) is ITreeDesigner designer && host.GetDesigner(commonParent) is ITreeDesigner commonParentDesigner && designer.Parent != commonParentDesigner)
                            {
                                // Walk the chain of designers from the current parent designer
                                // up to the root component, and for the current component designer.
                                static List<ITreeDesigner> GetDesignerChain(ITreeDesigner designer)
                                {
                                    List<ITreeDesigner> designerChain = [];
                                    while (designer.Parent is ITreeDesigner parent)
                                    {
                                        designerChain.Add(parent);
                                        designer = parent;
                                    }

                                    return designerChain;
                                }

                                List<ITreeDesigner> designerChain = GetDesignerChain(designer);
                                List<ITreeDesigner> parentDesignerChain = GetDesignerChain(commonParentDesigner);

                                // Now that we've got the trees built up, start comparing them from the ends to see where
                                // they diverge.
                                List<ITreeDesigner> shorterList = designerChain.Count < parentDesignerChain.Count ? designerChain : parentDesignerChain;
                                List<ITreeDesigner> longerList = (shorterList == designerChain ? parentDesignerChain : designerChain);
                                ITreeDesigner? commonDesigner = null;

                                if (shorterList.Count > 0 && longerList.Count > 0)
                                {
                                    int shortIndex = Math.Max(0, shorterList.Count - 1);
                                    int longIndex = Math.Max(0, longerList.Count - 1);
                                    while (shortIndex >= 0 && longIndex >= 0)
                                    {
                                        if (shorterList[shortIndex] != longerList[longIndex])
                                        {
                                            break;
                                        }

                                        commonDesigner = shorterList[shortIndex];
                                        shortIndex--;
                                        longIndex--;
                                    }
                                }

                                // alright, what have we got?
                                commonParent = commonDesigner?.Component;
                            }
                        }
                    }

                    if (changeService is not null)
                    {
                        List<IComponent> al = [];
                        GetAssociatedComponents(c, host, al);
                        foreach (IComponent comp in al)
                        {
                            changeService.OnComponentChanging(comp, null);
                        }
                    }

                    host.DestroyComponent((IComponent)obj);
                }
            }
            finally
            {
                trans?.Commit();

                foreach (ParentControlDesigner des in designerList)
                {
                    des.ResumeChangingEvents();
                }
            }

            if (commonParent is not null && SelectionService.PrimarySelection is null)
            {
                if (host.GetDesigner(commonParent) is ITreeDesigner { Children: not null } commonParentDesigner)
                {
                    // Choose the first child of the common parent if it has any.
                    foreach (IDesigner designer in commonParentDesigner.Children)
                    {
                        IComponent component = designer.Component;
                        if (component.Site is not null)
                        {
                            commonParent = component;
                            break;
                        }
                    }
                }
                else if (commonParent is Control controlCommonParent)
                {
                    // If we have a common parent, select it's first child.
                    if (controlCommonParent.Controls.Count > 0)
                    {
                        Control? parent = controlCommonParent.Controls[0];

                        // 126240 -- make sure we've got a sited thing.
                        //
                        while (parent is not null && parent.Site is null)
                        {
                            parent = parent.Parent;
                        }

                        commonParent = parent;
                    }
                }

                if (commonParent is not null)
                {
                    SelectionService.SetSelectedComponents(new object[] { commonParent }, SelectionTypes.Replace);
                }
                else
                {
                    SelectionService.SetSelectedComponents(new object[] { host.RootComponent }, SelectionTypes.Replace);
                }
            }
            else
            {
                if (SelectionService.PrimarySelection is null)
                {
                    SelectionService.SetSelectedComponents(new object[] { host.RootComponent }, SelectionTypes.Replace);
                }
            }
        }
        finally
        {
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the paste menu item is selected.
    /// </summary>

    [SuppressMessage("Microsoft.Security", "CA2301:DoNotCallBinaryFormatterDeserializeWithoutFirstSettingBinaryFormatterBinder", Justification = "data is trusted")]
    protected void OnMenuPaste(object? sender, EventArgs e)
    {
        Cursor? oldCursor = Cursor.Current;
        List<ParentControlDesigner> designerList = [];
        try
        {
            Cursor.Current = Cursors.WaitCursor;
            // If a control fails to get pasted; then we should remember its associatedComponents
            // so that they are not pasted.
            // Refer VsWhidbey : 477583
            ICollection? associatedCompsOfFailedControl = null;

            if (!TryGetService(out IDesignerHost? host))
            {
                return;
            }

            bool clipboardOperationSuccessful = ExecuteSafely(Clipboard.GetDataObject, false, out IDataObject? dataObj);

            if (clipboardOperationSuccessful)
            {
                ICollection? components = null;
                bool createdItems = false;

                // Get the current number of controls in the Component Tray in the target
                ComponentTray? tray = GetService<ComponentTray>();
                int numberOfOriginalTrayControls = tray is not null ? tray.Controls.Count : 0;

                // We understand two things:  CF_DESIGNER, and toolbox items.
                object? data = dataObj?.GetData(CF_DESIGNER);

                using DesignerTransaction trans = host.CreateTransaction(SR.CommandSetPaste);
                if (data is byte[] bytes)
                {
                    MemoryStream s = new(bytes);

                    // CF_DESIGNER was put on the clipboard by us using the designer serialization service.
                    if (TryGetService(out IDesignerSerializationService? ds))
                    {
                        s.Seek(0, SeekOrigin.Begin);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
                        object serializationData = new BinaryFormatter().Deserialize(s); // CodeQL[SM03722, SM04191] : The operation is essential for the design experience when users are running their own designers they have created. This cannot be achieved without BinaryFormatter
#pragma warning restore CA2300
#pragma warning restore SYSLIB0011
                        using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
                        {
                            components = ds.Deserialize(serializationData);
                        }
                    }
                }
                else if (TryGetService(out IToolboxService? ts) && ts.IsSupported(dataObj, host))
                {
                    // Now check for a toolbox item.
                    ToolboxItem? ti = ts.DeserializeToolboxItem(dataObj, host);
                    if (ti is not null)
                    {
                        using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
                        {
                            components = ti.CreateComponents(host);
                        }

                        createdItems = true;
                    }
                }

                // Now, if we got some components, hook 'em up!
                //
                if (components is not null && components.Count > 0)
                {
                    // Make copy of Items in Array..
                    object[] allComponents = new object[components.Count];
                    components.CopyTo(allComponents, 0);

                    List<IComponent> selectComps = [];
                    List<Control> controls = [];
                    string[]? componentNames = null;
                    int idx = 0;

                    // if the selected item is a frame designer, add to that, otherwise
                    // add to the form
                    IOleDragClient? designer = null;
                    bool dragClient = false;

                    IComponent baseComponent = host.RootComponent;
                    IComponent? selectedComponent = (IComponent?)SelectionService?.PrimarySelection;

                    selectedComponent ??= baseComponent;

                    ITreeDesigner? tree = host.GetDesigner(selectedComponent) as ITreeDesigner;

                    while (tree is not null)
                    {
                        if (tree is IOleDragClient oleDragClient)
                        {
                            designer = oleDragClient;
                            break;
                        }

                        if (tree == tree.Parent)
                        {
                            break;
                        }

                        tree = tree.Parent as ITreeDesigner;
                    }

                    foreach (object obj in components)
                    {
                        string? name = null;

                        // see if we can fish out the original name. When we
                        // serialized, we serialized an array of names at the
                        // head of the list. This array matches the components
                        // that were created.
                        if (obj is IComponent curComp)
                        {
                            if (componentNames is not null && idx < componentNames.Length)
                            {
                                name = componentNames[idx++];
                            }
                        }
                        else
                        {
                            if (componentNames is null && obj is string[] sa)
                            {
                                componentNames = sa;
                                idx = 0;
                            }

                            continue;
                        }

                        if (TryGetService(out IEventBindingService? evs))
                        {
                            PropertyDescriptorCollection eventProps = evs.GetEventProperties(TypeDescriptor.GetEvents(curComp));
                            foreach (PropertyDescriptor pd in eventProps)
                            {
                                // If we couldn't find a property for this event, or of the property is read only, then
                                // abort.
                                //
                                if (pd is null || pd.IsReadOnly)
                                {
                                    continue;
                                }

                                if (pd.GetValue(curComp) is string)
                                {
                                    pd.SetValue(curComp, null);
                                }
                            }
                        }

                        if (dragClient)
                        {
                            // If we have failed to add a control in this Paste operation ...
                            if (associatedCompsOfFailedControl is not null)
                            {
                                bool foundAssociatedControl = false;

                                // then don't add its children controls.
                                foreach (Component comp in associatedCompsOfFailedControl)
                                {
                                    if (comp == obj as Component)
                                    {
                                        foundAssociatedControl = true;
                                        break;
                                    }
                                }

                                if (foundAssociatedControl)
                                {
                                    continue; // continue from here so that we don't add the associated component of a control that failed paste operation.
                                }
                            }

                            // VSWhidbey 390442 - DGV has columns which are sited IComponents that don't
                            // have designers. in this case, ignore them.

                            if (host.GetDesigner(curComp) is not ComponentDesigner cDesigner)
                            {
                                continue;
                            }

                            // store associatedComponents.
                            ICollection? designerComps = cDesigner.AssociatedComponents;

                            ComponentDesigner? parentCompDesigner = ((ITreeDesigner)cDesigner).Parent as ComponentDesigner;
                            Component? parentComp = parentCompDesigner?.Component as Component;

                            List<IComponent> associatedComps = [];

                            if (parentComp is not null)
                            {
                                foreach (IComponent childComp in parentCompDesigner!.AssociatedComponents)
                                {
                                    associatedComps.Add(childComp);
                                }
                            }

                            if (parentComp is null || !(associatedComps.Contains(curComp)))
                            {
                                if (parentComp is not null)
                                {
                                    if (host.GetDesigner(parentComp) is ParentControlDesigner parentDesigner && !designerList.Contains(parentDesigner))
                                    {
                                        parentDesigner.SuspendChangingEvents();
                                        designerList.Add(parentDesigner);
                                        parentDesigner.ForceComponentChanging();
                                    }
                                }

                                if (!designer!.AddComponent(curComp, name!, createdItems))
                                {
                                    // cache the associatedComponents only for FAILED control.
                                    associatedCompsOfFailedControl = designerComps;
                                    // now we will jump out of the using block and call trans.Dispose()
                                    // which in turn calls trans.Cancel for an uncommitted transaction,
                                    // We want to cancel the transaction because otherwise we'll have
                                    // un-parented controls
                                    return;
                                }

                                Control designerControl = designer.GetControlForComponent(curComp);
                                if (designerControl is not null)
                                {
                                    controls.Add(designerControl);
                                }

                                // Select the newly Added top level component
                                if ((TypeDescriptor.GetAttributes(curComp).Contains(DesignTimeVisibleAttribute.Yes)) || curComp is ToolStripItem)
                                {
                                    selectComps.Add(curComp);
                                }
                            }

                            // if Parent is not selected... select the curcomp.
                            else if (associatedComps.Contains(curComp) && Array.IndexOf(allComponents, parentComp) == -1)
                            {
                                selectComps.Add(curComp);
                            }

                            bool changeName = false;

                            if (curComp is Control c)
                            {
                                // if the text is the same as the name, remember it.
                                // After we add the control, we'll update the text with
                                // the new name.
                                //
                                if (name is not null && name.Equals(c.Text))
                                {
                                    changeName = true;
                                }
                            }

                            if (changeName)
                            {
                                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(curComp);
                                PropertyDescriptor? nameProp = props["Name"];
                                if (nameProp is not null && nameProp.TryGetValue(curComp, out string? newName))
                                {
                                    if (newName != name)
                                    {
                                        PropertyDescriptor? textProp = props["Text"];
                                        if (textProp is not null && textProp.PropertyType == typeof(string))
                                        {
                                            textProp.SetValue(curComp, newName);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Find those controls that have ControlDesigners and center them on the designer surface
                    List<Control> compsWithControlDesigners = [];
                    foreach (Control c in controls)
                    {
                        IDesigner? des = host.GetDesigner(c);
                        if (des is ControlDesigner)
                        {
                            compsWithControlDesigners.Add(c);
                        }
                    }

                    if (compsWithControlDesigners.Count > 0)
                    {
                        // Update the control positions. We want to keep the entire block
                        // of controls relative to each other, but relocate them within
                        // the container.
                        //
                        UpdatePastePositions(compsWithControlDesigners);
                    }

                    // Figure out if we added components to the component tray, and have the
                    // tray adjust their position.
                    // MartinTh - removed the old check, since ToolStrips breaks the scenario.
                    // ToolStrips have a ControlDesigner, but also add a component to the tray.
                    // The old code wouldn't detect that, so the tray location wouldn't get adjusted.
                    // Rather than fixing this up in ToolStripKeyboardHandlingService.OnCommandPaste,
                    // we do it here, since doing it in the service, wouldn't handle cross-form paste.

                    // the paste target did not have a tray already, so let's go get it - if there is one
                    tray ??= GetService<ComponentTray>();

                    if (tray is not null)
                    {
                        int numberOfTrayControlsAdded = tray.Controls.Count - numberOfOriginalTrayControls;

                        if (numberOfTrayControlsAdded > 0)
                        {
                            List<Control> listOfTrayControls = [];
                            for (int i = 0; i < numberOfTrayControlsAdded; i++)
                            {
                                listOfTrayControls.Add(tray.Controls[numberOfOriginalTrayControls + i]);
                            }

                            tray.UpdatePastePositions(listOfTrayControls);
                        }
                    }

                    // Update the tab indices of all the components. We must first sort the
                    // components by their existing tab indices or else we will not preserve their
                    // original intent.
                    //
                    controls.Sort(TabIndexComparer.Instance);
                    foreach (Control c in controls)
                    {
                        UpdatePasteTabIndex(c, c.Parent);
                    }

                    // finally select all the components we added
                    SelectionService?.SetSelectedComponents(selectComps.ToArray(), SelectionTypes.Replace);

                    // and bring them to the front - but only if we can mess with the Z-order. VSWhidbey 515990
                    if (designer is ParentControlDesigner parentControlDesigner
                        && parentControlDesigner.AllowSetChildIndexOnDrop)
                    {
                        MenuCommand? btf = MenuService?.FindCommand(StandardCommands.BringToFront);
                        btf?.Invoke();
                    }

                    trans.Commit();
                }
            }
            else
            {
                _uiService?.ShowError(SR.ClipboardError);
            }
        }
        finally
        {
            Cursor.Current = oldCursor;
            foreach (ParentControlDesigner des in designerList)
            {
                des.ResumeChangingEvents();
            }
        }
    }

    /// <summary>
    ///  Called when the select all menu item is selected.
    /// </summary>
    protected void OnMenuSelectAll(object? sender, EventArgs e)
    {
        if (site is null || SelectionService is null || !TryGetService(out IDesignerHost? host))
        {
            Debug.Assert(SelectionService is not null, "We need the SelectionService, but we can't find it!");
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            ComponentCollection components = host.Container.Components;
            IComponent[] selComps;

            if (components is null || components.Count == 0)
            {
                selComps = [];
            }
            else
            {
                selComps = new IComponent[components.Count - 1];
                IComponent baseComp = host.RootComponent;

                int j = 0;
                foreach (IComponent comp in components)
                {
                    if (baseComp != comp)
                    {
                        selComps[j++] = comp;
                    }
                }
            }

            SelectionService.SetSelectedComponents(selComps, SelectionTypes.Replace);
        }
        finally
        {
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the show grid menu item is selected.
    /// </summary>
    protected void OnMenuShowGrid(object? sender, EventArgs e)
    {
        if (site is null || !TryGetService(out IDesignerHost? host))
        {
            return;
        }

        DesignerTransaction? trans = null;

        try
        {
            trans = host.CreateTransaction();

            IComponent baseComponent = host.RootComponent;
            if (baseComponent is Control)
            {
                PropertyDescriptor? prop = GetProperty(baseComponent, "DrawGrid");
                if (prop is not null)
                {
                    bool drawGrid = (bool)prop.GetValue(baseComponent)!;
                    prop.SetValue(baseComponent, !drawGrid);
                    ((MenuCommand)sender!).Checked = !drawGrid;
                }
            }
        }
        finally
        {
            trans?.Commit();
        }
    }

    /// <summary>
    ///  Handles the various size to commands.
    /// </summary>
    protected void OnMenuSizingCommand(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        CommandID? cmdID = cmd.CommandID;

        if (SelectionService is null)
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            ICollection sel = SelectionService.GetSelectedComponents();
            IComponent[] selectedObjects = new IComponent[sel.Count];
            sel.CopyTo(selectedObjects, 0);

            selectedObjects = FilterSelection(selectedObjects, SelectionRules.Visible);

            object? selPrimary = SelectionService.PrimarySelection;

            Size primarySize = Size.Empty;
            if (selPrimary is IComponent component)
            {
                PropertyDescriptor? sizeProp = GetProperty(component, "Size");
                if (sizeProp is null)
                {
                    // if we couldn't get a valid size for our primary selection, we'll fail silently
                    return;
                }

                primarySize = (Size)sizeProp.GetValue(component)!;
            }
            else if (selPrimary is null)
            {
                return;
            }

            Debug.Assert(selectedObjects is not null, "queryStatus should have disabled this");

            IDesignerHost? host = GetService<IDesignerHost>();
            DesignerTransaction? trans = null;

            try
            {
                trans = host?.CreateTransaction(string.Format(SR.CommandSetSize, selectedObjects.Length));

                foreach (IComponent obj in selectedObjects)
                {
                    if (obj.Equals(selPrimary))
                        continue;

                    // if the component is locked, no sizing is allowed...
                    PropertyDescriptor? lockedDesc = GetProperty(obj, "Locked");
                    if (lockedDesc is not null && (bool)lockedDesc.GetValue(obj)!)
                    {
                        continue;
                    }

                    PropertyDescriptor? sizeProp = GetProperty(obj, "Size");

                    // Skip all components that don't have a size property
                    //
                    if (sizeProp is null || sizeProp.IsReadOnly)
                    {
                        continue;
                    }

                    Size itemSize = (Size)sizeProp.GetValue(obj)!;

                    if (cmdID == StandardCommands.SizeToControlHeight ||
                        cmdID == StandardCommands.SizeToControl)
                    {
                        itemSize.Height = primarySize.Height;
                    }

                    if (cmdID == StandardCommands.SizeToControlWidth ||
                        cmdID == StandardCommands.SizeToControl)
                    {
                        itemSize.Width = primarySize.Width;
                    }

                    sizeProp.SetValue(obj, itemSize);
                }
            }
            finally
            {
                trans?.Commit();
            }
        }
        finally
        {
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the size->to grid menu item is selected.
    /// </summary>
    protected void OnMenuSizeToGrid(object? sender, EventArgs e)
    {
        if (SelectionService is null)
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        IDesignerHost? host = GetService<IDesignerHost>();
        DesignerTransaction? trans = null;

        try
        {
            Cursor.Current = Cursors.WaitCursor;

            ICollection sel = SelectionService.GetSelectedComponents();
            IComponent[] selectedObjects = new IComponent[sel.Count];
            sel.CopyTo(selectedObjects, 0);
            selectedObjects = FilterSelection(selectedObjects, SelectionRules.Visible);

            Debug.Assert(selectedObjects is not null, "queryStatus should have disabled this");
            Size grid = Size.Empty;

            if (host is not null)
            {
                trans = host.CreateTransaction(string.Format(SR.CommandSetSizeToGrid, selectedObjects.Length));

                IComponent baseComponent = host.RootComponent;
                if (baseComponent is not null and Control)
                {
                    PropertyDescriptor? prop = GetProperty(baseComponent, "CurrentGridSize");
                    if (prop is not null)
                    {
                        grid = (Size)prop.GetValue(baseComponent)!;
                    }
                }
            }

            if (!grid.IsEmpty)
            {
                foreach (IComponent comp in selectedObjects)
                {
                    PropertyDescriptor? sizeProp = GetProperty(comp, "Size");
                    PropertyDescriptor? locProp = GetProperty(comp, "Location");

                    Debug.Assert(sizeProp is not null, "No size property on component");
                    Debug.Assert(locProp is not null, "No location property on component");

                    if (sizeProp is null || locProp is null || sizeProp.IsReadOnly || locProp.IsReadOnly)
                    {
                        continue;
                    }

                    Size size = (Size)sizeProp.GetValue(comp)!;
                    Point loc = (Point)locProp.GetValue(comp)!;

                    size.Width = ((size.Width + (grid.Width / 2)) / grid.Width) * grid.Width;
                    size.Height = ((size.Height + (grid.Height / 2)) / grid.Height) * grid.Height;
                    loc.X = (loc.X / grid.Width) * grid.Width;
                    loc.Y = (loc.Y / grid.Height) * grid.Height;

                    sizeProp.SetValue(comp, size);
                    locProp.SetValue(comp, loc);
                }
            }
        }
        finally
        {
            trans?.Commit();

            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the properties menu item is selected on the Context menu
    /// </summary>
    protected void OnMenuDesignerProperties(object? sender, EventArgs e)
    {
        // first, look if the currently selected object has a component editor...
        object? obj = SelectionService!.PrimarySelection;

        if (CheckComponentEditor(obj, true))
        {
            return;
        }

        if (TryGetService(out IMenuCommandService? menuSvc))
        {
            if (menuSvc.GlobalInvoke(StandardCommands.PropertiesWindow))
            {
                return;
            }
        }

        Debug.Assert(false, "Invoking pbrs command failed");
    }

    /// <summary>
    ///  Called when the snap to grid menu item is selected.
    /// </summary>
    protected void OnMenuSnapToGrid(object? sender, EventArgs e)
    {
        if (site.TryGetService(out IDesignerHost? host))
        {
            DesignerTransaction? trans = null;

            try
            {
                trans = host.CreateTransaction(string.Format(SR.CommandSetPaste, 0));

                IComponent baseComponent = host.RootComponent;
                if (baseComponent is Control)
                {
                    PropertyDescriptor? prop = GetProperty(baseComponent, "SnapToGrid");
                    if (prop is not null)
                    {
                        bool snapToGrid = (bool)prop.GetValue(baseComponent)!;
                        prop.SetValue(baseComponent, !snapToGrid);
                        ((MenuCommand)sender!).Checked = !snapToGrid;
                    }
                }
            }
            finally
            {
                trans?.Commit();
            }
        }
    }

    /// <summary>
    ///  Called when a spacing command is selected
    ///
    /// </summary>
    protected void OnMenuSpacingCommand(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        CommandID? cmdID = cmd.CommandID;
        DesignerTransaction? trans = null;

        if (SelectionService is null)
        {
            return;
        }

        Cursor? oldCursor = Cursor.Current;
        IDesignerHost? host = GetService<IDesignerHost>();

        try
        {
            Cursor.Current = Cursors.WaitCursor;

            // Inform the designer that we are about to monkey with a ton
            // of properties.
            //
            Size grid = Size.Empty;
            ICollection sel = SelectionService.GetSelectedComponents();
            IComponent[] selectedObjects = new IComponent[sel.Count];
            sel.CopyTo(selectedObjects, 0);

            if (host is not null)
            {
                trans = host.CreateTransaction(string.Format(SR.CommandSetFormatSpacing, selectedObjects.Length));

                IComponent baseComponent = host.RootComponent;
                if (baseComponent is Control)
                {
                    PropertyDescriptor? prop = GetProperty(baseComponent, "CurrentGridSize");
                    if (prop is not null)
                    {
                        grid = (Size)prop.GetValue(baseComponent)!;
                    }
                }
            }

            selectedObjects = FilterSelection(selectedObjects, SelectionRules.Visible);

            int nEqualDelta = 0;

            Debug.Assert(selectedObjects is not null, "queryStatus should have disabled this");

            PropertyDescriptor? curSizeDesc = null, lastSizeDesc = null;
            PropertyDescriptor? curLocDesc = null, lastLocDesc = null;
            Size curSize = Size.Empty, lastSize = Size.Empty;
            Point curLoc = Point.Empty, lastLoc = Point.Empty;
            Point primaryLoc = Point.Empty;
            IComponent? lastComp = null;
            int sort = -1;

            // Must sort differently if we're horizontal or vertical...
            //
            if (cmdID == StandardCommands.HorizSpaceConcatenate ||
                cmdID == StandardCommands.HorizSpaceDecrease ||
                cmdID == StandardCommands.HorizSpaceIncrease ||
                cmdID == StandardCommands.HorizSpaceMakeEqual)
            {
                sort = SORT_HORIZONTAL;
            }
            else
            {
                sort = cmdID == StandardCommands.VertSpaceConcatenate ||
                                         cmdID == StandardCommands.VertSpaceDecrease ||
                                         cmdID == StandardCommands.VertSpaceIncrease ||
                                         cmdID == StandardCommands.VertSpaceMakeEqual
                    ? SORT_VERTICAL
                    : throw new ArgumentException(SR.CommandSetUnknownSpacingCommand);
            }

            SortSelection(selectedObjects, sort);

            // now that we're sorted, lets get our primary selection and it's index
            //
            object? primary = SelectionService.PrimarySelection;
            int primaryIndex = 0;
            if (primary is not null)
                primaryIndex = Array.IndexOf(selectedObjects, primary);

            // And compute delta values for Make Equal
            if (cmdID == StandardCommands.HorizSpaceMakeEqual ||
                cmdID == StandardCommands.VertSpaceMakeEqual)
            {
                int total = 0;
                for (int n = 0; n < selectedObjects.Length; n++)
                {
                    curSize = Size.Empty;

                    IComponent component = selectedObjects[n];

                    if (component is not null)
                    {
                        curSizeDesc = GetProperty(component, "Size");
                        if (curSizeDesc is not null)
                        {
                            curSize = (Size)curSizeDesc.GetValue(component)!;
                        }
                    }

                    if (sort == SORT_HORIZONTAL)
                    {
                        total += curSize.Width;
                    }
                    else
                    {
                        total += curSize.Height;
                    }
                }

                lastComp = null;
                curSize = Size.Empty;
                curLoc = Point.Empty;

                foreach (IComponent curComp in selectedObjects)
                {
                    if (curComp is not null)
                    {
                        // only get the descriptors if we've changed component types
                        if (lastComp is null || curComp.GetType() != lastComp.GetType())
                        {
                            curSizeDesc = GetProperty(curComp, "Size");
                            curLocDesc = GetProperty(curComp, "Location");
                        }

                        lastComp = curComp;

                        if (curLocDesc is not null)
                        {
                            curLoc = (Point)curLocDesc.GetValue(curComp)!;
                        }
                        else
                        {
                            continue;
                        }

                        if (curSizeDesc is not null)
                        {
                            curSize = (Size)curSizeDesc.GetValue(curComp)!;
                        }
                        else
                        {
                            continue;
                        }

                        if (!curSize.IsEmpty && !curLoc.IsEmpty)
                        {
                            break;
                        }
                    }
                }

                for (int n = selectedObjects.Length - 1; n >= 0; n--)
                {
                    IComponent curComp = selectedObjects[n];
                    if (curComp is not null)
                    {
                        // only get the descriptors if we've changed component types
                        if (lastComp is null || curComp.GetType() != lastComp.GetType())
                        {
                            curSizeDesc = GetProperty(curComp, "Size");
                            curLocDesc = GetProperty(curComp, "Location");
                        }

                        lastComp = curComp;

                        if (curLocDesc is not null)
                        {
                            lastLoc = (Point)curLocDesc.GetValue(curComp)!;
                        }
                        else
                        {
                            continue;
                        }

                        if (curSizeDesc is not null)
                        {
                            lastSize = (Size)curSizeDesc.GetValue(curComp)!;
                        }
                        else
                        {
                            continue;
                        }

                        break;
                    }
                }

                if (curSizeDesc is not null && curLocDesc is not null)
                {
                    nEqualDelta = sort == SORT_HORIZONTAL
                        ? (lastSize.Width + lastLoc.X - curLoc.X - total) / (selectedObjects.Length - 1)
                        : (lastSize.Height + lastLoc.Y - curLoc.Y - total) / (selectedObjects.Length - 1);

                    if (nEqualDelta < 0)
                        nEqualDelta = 0;
                }
            }

            lastComp = null;

            if (primary is not null)
            {
                PropertyDescriptor? primaryLocDesc = GetProperty(primary, "Location");
                if (primaryLocDesc is not null)
                {
                    primaryLoc = (Point)primaryLocDesc.GetValue(primary)!;
                }
            }

            // Finally move the components
            //
            for (int n = 0; n < selectedObjects.Length; n++)
            {
                IComponent curComp = selectedObjects[n];

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(curComp);

                // Check to see if the component we are about to move is locked...
                //
                PropertyDescriptor? lockedDesc = props["Locked"];
                if (lockedDesc is not null && (bool)lockedDesc.GetValue(curComp)!)
                {
                    continue; // locked property of our component is true, so don't move it
                }

                if (lastComp is null || lastComp.GetType() != curComp.GetType())
                {
                    curSizeDesc = props["Size"];
                    curLocDesc = props["Location"];
                }
                else
                {
                    curSizeDesc = lastSizeDesc;
                    curLocDesc = lastLocDesc;
                }

                if (curLocDesc is not null)
                {
                    curLoc = (Point)curLocDesc.GetValue(curComp)!;
                }
                else
                {
                    continue;
                }

                if (curSizeDesc is not null)
                {
                    curSize = (Size)curSizeDesc.GetValue(curComp)!;
                }
                else
                {
                    continue;
                }

                int lastIndex = Math.Max(0, n - 1);
                lastComp = selectedObjects[lastIndex];
                if (lastComp.GetType() != curComp.GetType())
                {
                    lastSizeDesc = GetProperty(lastComp, "Size");
                    lastLocDesc = GetProperty(lastComp, "Location");
                }
                else
                {
                    lastSizeDesc = curSizeDesc;
                    lastLocDesc = curLocDesc;
                }

                if (lastLocDesc is not null)
                {
                    lastLoc = (Point)lastLocDesc.GetValue(lastComp)!;
                }
                else
                {
                    continue;
                }

                if (lastSizeDesc is not null)
                {
                    lastSize = (Size)lastSizeDesc.GetValue(lastComp)!;
                }
                else
                {
                    continue;
                }

                if (cmdID == StandardCommands.HorizSpaceConcatenate && n > 0)
                {
                    curLoc.X = lastLoc.X + lastSize.Width;
                }
                else if (cmdID == StandardCommands.HorizSpaceDecrease)
                {
                    if (primaryIndex < n)
                    {
                        curLoc.X -= grid.Width * (n - primaryIndex);
                        if (curLoc.X < primaryLoc.X)
                            curLoc.X = primaryLoc.X;
                    }
                    else if (primaryIndex > n)
                    {
                        curLoc.X += grid.Width * (primaryIndex - n);
                        if (curLoc.X > primaryLoc.X)
                            curLoc.X = primaryLoc.X;
                    }
                }
                else if (cmdID == StandardCommands.HorizSpaceIncrease)
                {
                    if (primaryIndex < n)
                    {
                        curLoc.X += grid.Width * (n - primaryIndex);
                    }
                    else if (primaryIndex > n)
                    {
                        curLoc.X -= grid.Width * (primaryIndex - n);
                    }
                }
                else if (cmdID == StandardCommands.HorizSpaceMakeEqual && n > 0)
                {
                    curLoc.X = lastLoc.X + lastSize.Width + nEqualDelta;
                }
                else if (cmdID == StandardCommands.VertSpaceConcatenate && n > 0)
                {
                    curLoc.Y = lastLoc.Y + lastSize.Height;
                }
                else if (cmdID == StandardCommands.VertSpaceDecrease)
                {
                    if (primaryIndex < n)
                    {
                        curLoc.Y -= grid.Height * (n - primaryIndex);
                        if (curLoc.Y < primaryLoc.Y)
                            curLoc.Y = primaryLoc.Y;
                    }
                    else if (primaryIndex > n)
                    {
                        curLoc.Y += grid.Height * (primaryIndex - n);
                        if (curLoc.Y > primaryLoc.Y)
                            curLoc.Y = primaryLoc.Y;
                    }
                }
                else if (cmdID == StandardCommands.VertSpaceIncrease)
                {
                    if (primaryIndex < n)
                    {
                        curLoc.Y += grid.Height * (n - primaryIndex);
                    }
                    else if (primaryIndex > n)
                    {
                        curLoc.Y -= grid.Height * (primaryIndex - n);
                    }
                }
                else if (cmdID == StandardCommands.VertSpaceMakeEqual && n > 0)
                {
                    curLoc.Y = lastLoc.Y + lastSize.Height + nEqualDelta;
                }

                if (!curLocDesc.IsReadOnly)
                {
                    curLocDesc.SetValue(curComp, curLoc);
                }

                lastComp = curComp;
            }
        }
        finally
        {
            trans?.Commit();

            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Called when the current selection changes. Here we determine what
    ///  commands can and can't be enabled.
    /// </summary>
    protected void OnSelectionChanged(object? sender, EventArgs e)
    {
        if (SelectionService is null)
        {
            return;
        }

        SelectionVersion++;

        // Update our cached selection counts.
        //
        selCount = SelectionService.SelectionCount;

        IDesignerHost? designerHost = GetService<IDesignerHost>();
        Debug.Assert(designerHost is not null, "Failed to get designer host");

        // if the base component is selected, we'll say that nothing's selected
        // so we don't get wierd behavior
        if (selCount > 0 && designerHost is not null)
        {
            object baseComponent = designerHost.RootComponent;
            if (baseComponent is not null && SelectionService.GetComponentSelected(baseComponent))
            {
                selCount = 0;
            }
        }

        primarySelection = SelectionService.PrimarySelection as IComponent;
        _selectionInherited = false;
        controlsOnlySelection = true;

        if (selCount > 0)
        {
            ICollection selection = SelectionService.GetSelectedComponents();
            foreach (object obj in selection)
            {
                if (obj is not Control)
                {
                    controlsOnlySelection = false;
                }

                if (!Equals(TypeDescriptor.GetAttributes(obj)[typeof(InheritanceAttribute)], InheritanceAttribute.NotInherited))
                {
                    _selectionInherited = true;
                    break;
                }
            }
        }

        OnUpdateCommandStatus();
    }

    /// <summary>
    ///  When this timer expires, this tells us that we need to
    ///  erase any snaplines we have drawn. First, we need
    ///  to marshal this back to the correct thread.
    /// </summary>
    private void OnSnapLineTimerExpire(object? sender, EventArgs e)
    {
        Control? marshalControl = BehaviorService?.AdornerWindowControl;

        if (marshalControl is not null && marshalControl.IsHandleCreated)
        {
            marshalControl.BeginInvoke(OnSnapLineTimerExpireMarshalled, [sender, e]);
        }
    }

    /// <summary>
    ///  Called when our snapline timer expires - this method has been call
    ///  has been properly marshalled back to the correct thread.
    /// </summary>
    private void OnSnapLineTimerExpireMarshalled(object? sender, EventArgs e)
    {
        _snapLineTimer!.Stop();
        EndDragManager();
    }

    /// <summary>
    ///  Determines the status of a menu command. Commands with this event
    ///  handler are always enabled.
    /// </summary>
    protected void OnStatusAlways(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        cmd.Enabled = true;
    }

    /// <summary>
    ///  Determines the status of a menu command. Commands with this event
    ///  handler are enabled when one or more objects are selected.
    /// </summary>
    protected void OnStatusAnySelection(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        cmd.Enabled = selCount > 0;
    }

    /// <summary>
    ///  Status for the copy command. This is enabled when
    ///  there is something juicy selected.
    /// </summary>
    protected void OnStatusCopy(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        bool enable = false;

        if (!_selectionInherited
            && TryGetService(out IDesignerHost? host)
            && !host.Loading
            && TryGetService(out ISelectionService? selSvc))
        {
            // There must also be a component in the mix, and not the base component
            ICollection selectedComponents = selSvc.GetSelectedComponents();
            object baseComp = host.RootComponent;

            if (!selSvc.GetComponentSelected(baseComp))
            {
                foreach (object obj in selectedComponents)
                {
                    // if the object is not sited to the same thing as the host container
                    // then don't allow copy. VSWhidbey# 275790
                    if (obj is IComponent { Site: { } objSite } && objSite.Container == host.Container)
                    {
                        enable = true;
                        break;
                    }
                }
            }
        }

        cmd.Enabled = enable;
    }

    /// <summary>
    ///  Status for the cut command. This is enabled when
    ///  there is something juicy selected and that something
    ///  does not contain any inherited components.
    /// </summary>
    protected void OnStatusCut(object? sender, EventArgs e)
    {
        OnStatusDelete(sender, e);
        if (((MenuCommand)sender!).Enabled)
        {
            OnStatusCopy(sender, e);
        }
    }

    /// <summary>
    ///  Status for the delete command. This is enabled when there
    ///  is something selected and that something does not contain
    ///  inherited components.
    /// </summary>
    protected void OnStatusDelete(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        if (_selectionInherited)
        {
            cmd.Enabled = false;
        }
        else
        {
            if (TryGetService(out IDesignerHost? host))
            {
                if (TryGetService(out ISelectionService? selSvc))
                {
                    ICollection selectedComponents = selSvc.GetSelectedComponents();
                    foreach (object obj in selectedComponents)
                    {
                        // if the object is not sited to the same thing as the host container
                        // then don't allow delete. VSWhidbey# 275790
                        if (obj is IComponent comp && (comp.Site is null || comp.Site.Container != host.Container))
                        {
                            cmd.Enabled = false;
                            return;
                        }
                    }
                }
            }

            OnStatusAnySelection(sender, e);
        }
    }

    /// <summary>
    ///  Determines the status of a menu command. Commands with this event are
    ///  enabled when there is something useful on the clipboard.
    /// </summary>
    protected void OnStatusPaste(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;

        // Before we even look at the data format, check to see if the thing we're going to paste
        // into is privately inherited. If it is, then we definitely cannot paste.
        if (TryGetService(out IDesignerHost? host)
            && primarySelection is not null
            && host.GetDesigner(primarySelection) is ParentControlDesigner)
        {
            // This component is a target for our paste operation. We must ensure
            // that it is not privately inherited.
            InheritanceAttribute? attr = (InheritanceAttribute?)TypeDescriptor.GetAttributes(primarySelection)[typeof(InheritanceAttribute)];
            Debug.Assert(attr is not null, "Type descriptor gave us a null attribute -- problem in type descriptor");
            if (attr.InheritanceLevel == InheritanceLevel.InheritedReadOnly)
            {
                cmd.Enabled = false;
                return;
            }
        }

        // Not being inherited. Now look at the contents of the data
        bool clipboardOperationSuccessful = ExecuteSafely(Clipboard.GetDataObject, false, out IDataObject? dataObj);

        bool enable = false;

        if (clipboardOperationSuccessful && dataObj is not null)
        {
            if (dataObj.GetDataPresent(CF_DESIGNER))
            {
                enable = true;
            }
            else
            {
                // Not ours, check to see if the toolbox service understands this
                if (TryGetService(out IToolboxService? ts))
                {
                    enable = host is not null ? ts.IsSupported(dataObj, host) : ts.IsToolboxItem(dataObj);
                }
            }
        }

        cmd.Enabled = enable;
    }

    private void OnStatusPrimarySelection(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;
        cmd.Enabled = primarySelection is not null;
    }

    protected virtual void OnStatusSelectAll(object? sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender!;

        IDesignerHost host = GetService<IDesignerHost>()!;

        cmd.Enabled = host.Container.Components.Count > 1;
    }

    /// <summary>
    ///  This is called when the selection has changed. Anyone using CommandSetItems
    ///  that need to update their status based on selection changes should override
    ///  this and update their own commands at this time. The base implementation
    ///  runs through all base commands and calls UpdateStatus on them.
    /// </summary>
    protected virtual void OnUpdateCommandStatus()
    {
        // Now whip through all of the commands and ask them to update.
        //
        for (int i = 0; i < _commandSet.Length; i++)
        {
            _commandSet[i].UpdateStatus();
        }
    }

    /// <summary>
    ///  This method grows the objects collection by one. It prepends the
    ///  collection with a string[] which contains the component names in order
    ///  for each component in the list.
    /// </summary>
    private static object[] PrependComponentNames(ICollection objects)
    {
        object[] newObjects = new object[objects.Count + 1];
        int idx = 1;
        List<string?> names = new(objects.Count);

        foreach (object o in objects)
        {
            if (o is IComponent comp)
            {
                string? name = comp.Site?.Name;
                names.Add(name);
            }

            newObjects[idx++] = o;
        }

        newObjects[0] = names.ToArray();
        return newObjects;
    }

    /// <summary>
    ///  called by the formatting commands when we need a given selection array sorted.
    ///  Sorting the array sorts by x from left to right, and by Y from top to bottom.
    /// </summary>
    private static void SortSelection(IComponent[] selectedObjects, int nSortBy)
    {
        IComparer<IComponent> comp;
        switch (nSortBy)
        {
            case SORT_HORIZONTAL:
                comp = new ComponentLeftCompare();
                break;
            case SORT_VERTICAL:
                comp = new ComponentTopCompare();
                break;
            case SORT_ZORDER:
                comp = new ControlZOrderCompare();
                break;
            default:
                return;
        }

        Array.Sort(selectedObjects, comp);
    }

#if UNUSED
    private void TestCommandCut(string[] args) {
        this.OnMenuCut(null, EventArgs.Empty);
    }

    private void TestCommandCopy(string[] args) {
        this.OnMenuCopy(null, EventArgs.Empty);
    }

    private void TestCommandPaste(string[] args) {
        this.OnMenuPaste(null, EventArgs.Empty);
    }
#endif

    /// <summary>
    ///  Common function that updates the status of clipboard menu items only
    /// </summary>
    private void UpdateClipboardItems(object? s, EventArgs? e)
    {
        int itemCount = 0;
        for (int i = 0; itemCount < 3 && i < _commandSet.Length; i++)
        {
            CommandSetItem curItem = _commandSet[i];
            if (curItem.CommandID == StandardCommands.Paste ||
                curItem.CommandID == StandardCommands.Copy ||
                curItem.CommandID == StandardCommands.Cut)
            {
                itemCount++;
                curItem.UpdateStatus();
            }
        }
    }

    private void UpdatePastePositions(List<Control> controls)
    {
        if (controls.Count == 0)
        {
            return;
        }

        // Find the offset to apply to these controls. The offset
        // is the location needed to center the controls in the parent.
        // If there is no parent, we relocate to 0, 0.
        //
        Control? parentControl = controls[0].Parent;
        Point min = controls[0].Location;
        Point max = min;
        foreach (Control c in controls)
        {
            Point loc = c.Location;
            Size size = c.Size;
            if (min.X > loc.X)
            {
                min.X = loc.X;
            }

            if (min.Y > loc.Y)
            {
                min.Y = loc.Y;
            }

            if (max.X < loc.X + size.Width)
            {
                max.X = loc.X + size.Width;
            }

            if (max.Y < loc.Y + size.Height)
            {
                max.Y = loc.Y + size.Height;
            }
        }

        // We have the bounding rect for the controls. Next,
        // offset this rect so that we center it in the parent.
        // If we have no parent, the offset will position the
        // control at 0, 0, to whatever parent we eventually
        // get.
        //
        Point offset = new(-min.X, -min.Y);

        // Look to ensure that we're not going to paste this control over
        // the top of another control. We only do this for the first
        // control because preserving the relationship between controls
        // is more important than obscuring a control.
        //
        if (parentControl is not null)
        {
            bool bumpIt;
            bool wrapped = false;
            Size parentSize = parentControl.ClientSize;
            Size gridSize = Size.Empty;
            Point parentOffset = new(parentSize.Width / 2, parentSize.Height / 2);
            parentOffset.X -= (max.X - min.X) / 2;
            parentOffset.Y -= (max.Y - min.Y) / 2;

            do
            {
                bumpIt = false;

                // Cycle through the controls on the parent. We're
                // interested in controls that (a) are not in our
                // set of controls and (b) have a location ==
                // to our current bumpOffset OR (c) are the same
                // size as our parent. If we find such a
                // control, we increment the bump offset by one
                // grid size.
                //
                foreach (Control child in parentControl.Controls)
                {
                    Rectangle childBounds = child.Bounds;

                    if (controls.Contains(child))
                    {
                        // We still want to bump if the child is the same size as the parent.
                        // Otherwise the child would overlay exactly on top of the parent.
                        //
                        if (!child.Size.Equals(parentSize))
                        {
                            continue;
                        }

                        // We're dealing with our own pasted control, so
                        // offset its bounds. We don't use parent offset here
                        // because, well, we're comparing against the parent!
                        //
                        childBounds.Offset(offset);
                    }

                    // We need only compare against one of our pasted controls, so
                    // pick the first one.
                    //
                    Control pasteControl = controls[0];
                    Rectangle pasteControlBounds = pasteControl.Bounds;
                    pasteControlBounds.Offset(offset);
                    pasteControlBounds.Offset(parentOffset);

                    if (pasteControlBounds.Equals(childBounds))
                    {
                        bumpIt = true;

                        if (gridSize.IsEmpty)
                        {
                            IDesignerHost? host = GetService<IDesignerHost>();
                            IComponent? baseComponent = host?.RootComponent;
                            if (baseComponent is Control)
                            {
                                PropertyDescriptor? gs = GetProperty(baseComponent, "GridSize");
                                if (gs is not null)
                                {
                                    gridSize = (Size)gs.GetValue(baseComponent)!;
                                }
                            }

                            if (gridSize.IsEmpty)
                            {
                                gridSize.Width = 8;
                                gridSize.Height = 8;
                            }
                        }

                        parentOffset += gridSize;

                        // Extra check:  If the end of our control group is > the
                        // parent size, bump back to zero. We still allow further
                        // bumps after this so we can continue to offset, but if
                        // we cycle again then we quit so we won't loop indefinitely.
                        // We only do this if we're a group. If we're a single control
                        // we use the beginning of the control + a grid size.
                        //
                        int groupEndX;
                        int groupEndY;

                        if (controls.Count > 1)
                        {
                            groupEndX = parentOffset.X + max.X - min.X;
                            groupEndY = parentOffset.Y + max.Y - min.Y;
                        }
                        else
                        {
                            groupEndX = parentOffset.X + gridSize.Width;
                            groupEndY = parentOffset.Y + gridSize.Height;
                        }

                        if (groupEndX > parentSize.Width || groupEndY > parentSize.Height)
                        {
                            parentOffset.X = 0;
                            parentOffset.Y = 0;

                            if (wrapped)
                            {
                                bumpIt = false;
                            }
                            else
                            {
                                wrapped = true;
                            }
                        }

                        break;
                    }
                }
            }
            while (bumpIt);

            offset.Offset(parentOffset.X, parentOffset.Y);
        }

        // Now, for each control, update the offset.
        //

        parentControl?.SuspendLayout();

        try
        {
            foreach (Control c in controls)
            {
                Point newLoc = c.Location;
                newLoc.Offset(offset.X, offset.Y);
                c.Location = newLoc;
            }
        }
        finally
        {
            parentControl?.ResumeLayout();
        }
    }

    private static void UpdatePasteTabIndex(Control componentControl, Control? parentControl)
    {
        if (parentControl is null || componentControl is null)
        {
            return;
        }

        bool tabIndexCollision = false;
        int tabIndexOriginal = componentControl.TabIndex;

        // Find the next highest tab index
        //
        int nextTabIndex = 0;
        foreach (Control c in parentControl.Controls)
        {
            int t = c.TabIndex;
            if (nextTabIndex <= t)
            {
                nextTabIndex = t + 1;
            }

            if (t == tabIndexOriginal)
            {
                tabIndexCollision = true;
            }
        }

        if (tabIndexCollision)
        {
            componentControl.TabIndex = nextTabIndex;
        }
    }

    /// <summary>
    ///  Component comparer that compares the left property of a component.
    /// </summary>
    private class ComponentLeftCompare : IComparer<IComponent>
    {
        public int Compare(IComponent? p, IComponent? q)
        {
            PropertyDescriptor pProp = TypeDescriptor.GetProperties(p!)["Location"]!;
            PropertyDescriptor qProp = TypeDescriptor.GetProperties(q!)["Location"]!;

            Point pLoc = (Point)pProp.GetValue(p)!;
            Point qLoc = (Point)qProp.GetValue(q)!;

            // if our lefts are equal, then compare tops
            return pLoc.X == qLoc.X ? pLoc.Y - qLoc.Y : pLoc.X - qLoc.X;
        }
    }

    /// <summary>
    ///  Component comparer that compares the top property of a component.
    /// </summary>
    private class ComponentTopCompare : IComparer<IComponent>
    {
        public int Compare(IComponent? p, IComponent? q)
        {
            PropertyDescriptor pProp = TypeDescriptor.GetProperties(p!)["Location"]!;
            PropertyDescriptor qProp = TypeDescriptor.GetProperties(q!)["Location"]!;

            Point pLoc = (Point)pProp.GetValue(p)!;
            Point qLoc = (Point)qProp.GetValue(q)!;

            // if our tops are equal, then compare lefts
            return pLoc.Y == qLoc.Y ? pLoc.X - qLoc.X : pLoc.Y - qLoc.Y;
        }
    }

    private class ControlZOrderCompare : IComparer<IComponent>
    {
        public int Compare(IComponent? p, IComponent? q)
        {
            if (p is null)
            {
                return -1;
            }
            else if (q is null)
            {
                return 1;
            }
            else if (p == q)
            {
                return 0;
            }

            if (p is not Control c1 || q is not Control c2)
            {
                return 1;
            }

            return c1.Parent == c2.Parent && c1.Parent is not null ? c1.Parent.Controls.GetChildIndex(c1) - c1.Parent.Controls.GetChildIndex(c2) : 1;
        }
    }

    private class TabIndexComparer : IComparer<Control>
    {
        public static TabIndexComparer Instance { get; } = new();

        private TabIndexComparer() { }

        public int Compare(Control? c1, Control? c2)
        {
            if (c1 == c2)
            {
                return 0;
            }

            return c1 is null ? -1 : c2 is null ? 1 : c1.TabIndex - c2.TabIndex;
        }
    }
}
