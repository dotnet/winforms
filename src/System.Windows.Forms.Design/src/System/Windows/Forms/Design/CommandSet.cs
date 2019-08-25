// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This class implements the standard set of menu commands for the form designer.  This set of command is shared between the form designer (and other UI-based form packages), and composition designer, which doesn't manipulate controls. Therefore, this set of command should only contain commands that are common to both functions.
    /// </summary>
    internal class CommandSet : IDisposable
    {
        protected ISite site;
        private readonly CommandSetItem[] _commandSet;
        private IMenuCommandService _menuService;
        private IEventHandlerService _eventService;
        // Selection service fields. We keep some state about the currently selected components so we can determine proper command enabling quickly.
        private ISelectionService _selectionService;
        protected int selCount; // the current selection count
        protected IComponent primarySelection; // the primary selection, or null
        private bool _selectionInherited; // the selection contains inherited components
        protected bool controlsOnlySelection; // is the selection containing only controls or are there components in it?
        private int _selectionVersion = 1; // the counter of selection changes.
        // Selection sort constants
        private const int SORT_HORIZONTAL = 0;
        private const int SORT_VERTICAL = 1;
        private const int SORT_ZORDER = 2;
        private const string CF_DESIGNER = "CF_DESIGNERCOMPONENTS_V2";
        //these are used for snapping control via keyboard movement
        protected DragAssistanceManager dragManager = null;//point to the snapline engine (only valid between keydown and timer expiration)
        private Timer _snapLineTimer;//used to track the time from when a snapline is rendered until it should expire
        private BehaviorService _behaviorService;//demand created pointer to the behaviorservice
        private StatusCommandUI _statusCommandUI; //Used to update the statusBar Information.

        /// <summary>
        ///  Creates a new CommandSet object. This object implements the set of commands that the UI.Win32 form designer offers.
        /// </summary>
        public CommandSet(ISite site)
        {
            this.site = site;
            _eventService = (IEventHandlerService)site.GetService(typeof(IEventHandlerService));
            Debug.Assert(_eventService != null, "Command set must have the event service.  Is command set being initialized too early?");
            _eventService.EventHandlerChanged += new EventHandler(OnEventHandlerChanged);
            IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
            Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
            if (host != null)
            {
                host.Activated += new EventHandler(UpdateClipboardItems);
            }
            _statusCommandUI = new StatusCommandUI(site);
            IUIService uiService = site.GetService(typeof(IUIService)) as IUIService;
            // Establish our set of commands
            _commandSet = new CommandSetItem[]
            {
                // Editing commands
                new CommandSetItem( this, new EventHandler(OnStatusDelete), new EventHandler(OnMenuDelete), MenuCommands.Delete, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusCopy), new EventHandler(OnMenuCopy), MenuCommands.Copy,  uiService),
                new CommandSetItem( this, new EventHandler(OnStatusCut), new EventHandler(OnMenuCut), MenuCommands.Cut, uiService),
                new ImmediateCommandSetItem( this, new EventHandler(OnStatusPaste), new EventHandler(OnMenuPaste), MenuCommands.Paste, uiService),
                // Miscellaneous commands
                new CommandSetItem( this, new EventHandler(OnStatusSelectAll), new EventHandler(OnMenuSelectAll), MenuCommands.SelectAll, true, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAlways), new EventHandler(OnMenuDesignerProperties), MenuCommands.DesignerProperties, uiService),
                // Keyboard commands
                new CommandSetItem( this, new EventHandler(OnStatusAlways), new EventHandler(OnKeyCancel), MenuCommands.KeyCancel, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAlways), new EventHandler(OnKeyCancel), MenuCommands.KeyReverseCancel, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusPrimarySelection), new EventHandler(OnKeyDefault), MenuCommands.KeyDefaultAction, true, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAnySelection), new EventHandler(OnKeyMove), MenuCommands.KeyMoveUp, true, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAnySelection), new EventHandler(OnKeyMove), MenuCommands.KeyMoveDown, true, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAnySelection), new EventHandler(OnKeyMove), MenuCommands.KeyMoveLeft, true, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAnySelection), new EventHandler(OnKeyMove), MenuCommands.KeyMoveRight, true),
                new CommandSetItem(this, new EventHandler(OnStatusAnySelection), new EventHandler(OnKeyMove), MenuCommands.KeyNudgeUp, true, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAnySelection), new EventHandler(OnKeyMove), MenuCommands.KeyNudgeDown, true, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAnySelection), new EventHandler(OnKeyMove), MenuCommands.KeyNudgeLeft, true, uiService),
                new CommandSetItem( this, new EventHandler(OnStatusAnySelection), new EventHandler(OnKeyMove), MenuCommands.KeyNudgeRight, true, uiService),
            };

            _selectionService = (ISelectionService)site.GetService(typeof(ISelectionService));
            Debug.Assert(_selectionService != null, "CommandSet relies on the selection service, which is unavailable.");
            if (_selectionService != null)
            {
                _selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
            }

            _menuService = (IMenuCommandService)site.GetService(typeof(IMenuCommandService));
            if (_menuService != null)
            {
                for (int i = 0; i < _commandSet.Length; i++)
                {
                    _menuService.AddCommand(_commandSet[i]);
                }
            }

            // Now setup the default command GUID for this designer.  This GUID is also used in our toolbar definition file to identify toolbars we own.  We store the GUID in a command ID here in the dictionary of the root component.  Our host may pull this GUID out and use it.
            IDictionaryService ds = site.GetService(typeof(IDictionaryService)) as IDictionaryService;
            Debug.Assert(ds != null, "No dictionary service");
            if (ds != null)
            {
                ds.SetValue(typeof(CommandID), new CommandID(new Guid("BA09E2AF-9DF2-4068-B2F0-4C7E5CC19E2F"), 0));
            }
        }

        /// <summary>
        ///  Demand creates a pointer to the BehaviorService
        /// </summary>
        protected BehaviorService BehaviorService
        {
            get
            {
                if (_behaviorService == null)
                {
                    _behaviorService = GetService(typeof(BehaviorService)) as BehaviorService;
                }
                return _behaviorService;
            }
        }

        /// <summary>
        ///  Retrieves the menu command service, which the command set typically uses quite a bit.
        /// </summary>
        protected IMenuCommandService MenuService
        {
            get
            {
                if (_menuService == null)
                {
                    _menuService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
                }
                return _menuService;
            }
        }

        /// <summary>
        ///  Retrieves the selection service, which the command set typically uses quite a bit.
        /// </summary>
        protected ISelectionService SelectionService
        {
            get => _selectionService;
        }

        protected int SelectionVersion
        {
            get => _selectionVersion;
        }

        /// <summary>
        ///  This property demand creates our snaplinetimer used to track how long we'll leave snaplines on the screen before erasing them
        /// </summary>
        protected Timer SnapLineTimer
        {
            get
            {
                if (_snapLineTimer == null)
                {
                    //instantiate our snapline timer
                    _snapLineTimer = new Timer
                    {
                        Interval = DesignerUtils.SNAPELINEDELAY
                    };
                    _snapLineTimer.Tick += new EventHandler(OnSnapLineTimerExpire);
                }
                return _snapLineTimer;
            }
        }

        /// <summary>
        ///  Checks if an object supports ComponentEditors, and optionally launches the editor.
        /// </summary>
        private bool CheckComponentEditor(object obj, bool launchEditor)
        {
            if (obj is IComponent)
            {
                try
                {
                    if (!launchEditor)
                    {
                        return true;
                    }
                    ComponentEditor editor = (ComponentEditor)TypeDescriptor.GetEditor(obj, typeof(ComponentEditor));
                    if (editor == null)
                    {
                        return false;
                    }

                    bool success = false;
                    IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                    if (changeService != null)
                    {
                        try
                        {
                            changeService.OnComponentChanging(obj, null);
                        }
                        catch (CheckoutException coEx)
                        {
                            if (coEx == CheckoutException.Canceled)
                            {
                                return false;
                            }
                            throw coEx;
                        }
                        catch
                        {
                            Debug.Fail("non-CLS compliant exception");
                            throw;
                        }
                    }

                    if (editor is WindowsFormsComponentEditor winEditor)
                    {
                        success = winEditor.EditComponent(obj, obj as IWin32Window);
                    }
                    else
                    {
                        success = editor.EditComponent(obj);
                    }

                    if (success && changeService != null)
                    {
                        // Now notify the change service that the change was successful.
                        changeService.OnComponentChanged(obj, null, null, null);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///  Disposes of this object, removing all commands from the menu service.
        /// </summary>
        public virtual void Dispose()
        {
            if (_menuService != null)
            {
                for (int i = 0; i < _commandSet.Length; i++)
                {
                    _menuService.RemoveCommand(_commandSet[i]);
                    _commandSet[i].Dispose();
                }
                _menuService = null;
            }

            if (_selectionService != null)
            {
                _selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
                _selectionService = null;
            }

            if (_eventService != null)
            {
                _eventService.EventHandlerChanged -= new EventHandler(OnEventHandlerChanged);
                _eventService = null;
            }

            IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
            Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
            if (host != null)
            {
                host.Activated -= new EventHandler(UpdateClipboardItems);
            }

            if (_snapLineTimer != null)
            {
                _snapLineTimer.Stop();
                _snapLineTimer.Tick -= new EventHandler(OnSnapLineTimerExpire);
                _snapLineTimer = null;
            }

            EndDragManager();
            _statusCommandUI = null;
            site = null;
        }

        /// <summary>
        ///  Properly cleans up our drag engine.
        /// </summary>
        protected void EndDragManager()
        {
            if (dragManager != null)
            {
                if (_snapLineTimer != null)
                {
                    _snapLineTimer.Stop();
                }
                dragManager.EraseSnapLines();
                dragManager.OnMouseUp();
                dragManager = null;
            }
        }

        /// <summary>
        ///  Filters the set of selected components.  The selection service will retrieve all components that are currently selected.  This method allows you to filter this set down to components that match your criteria.  The selectionRules parameter must contain one or more flags from the SelectionRules class.  These flags allow you to constrain the set of selected objects to visible, movable, sizeable or all objects.
        /// </summary>
        private object[] FilterSelection(object[] components, SelectionRules selectionRules)
        {
            object[] selection = null;
            if (components == null)
            {
                return Array.Empty<object>();
            }
            // Mask off any selection object that doesn't adhere to the given ruleset. We can ignore this if the ruleset is zero, as all components would be accepted.
            if (selectionRules != SelectionRules.None)
            {
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    ArrayList list = new ArrayList();
                    foreach (IComponent comp in components)
                    {
                        if (host.GetDesigner(comp) is ControlDesigner des && (des.SelectionRules & selectionRules) == selectionRules)
                        {
                            list.Add(comp);
                        }
                    }
                    selection = list.ToArray();
                }
            }
            return selection ?? (Array.Empty<object>());
        }

        /// <summary>
        ///  Used to retrieve the selection for a copy.  The default implementation retrieves the current selection.
        /// </summary>
        protected virtual ICollection GetCopySelection()
        {
            ICollection selectedComponents = SelectionService.GetSelectedComponents();
            bool sort = false;
            object[] comps = new object[selectedComponents.Count];
            selectedComponents.CopyTo(comps, 0);
            foreach (object comp in comps)
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
            selectedComponents = comps;
            IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                ArrayList copySelection = new ArrayList();
                foreach (IComponent comp in selectedComponents)
                {
                    copySelection.Add(comp);
                    GetAssociatedComponents(comp, host, copySelection);
                }
                selectedComponents = copySelection;
            }
            return selectedComponents;
        }

        private void GetAssociatedComponents(IComponent component, IDesignerHost host, ArrayList list)
        {
            if (!(host.GetDesigner(component) is ComponentDesigner designer))
            {
                return;
            }
            foreach (IComponent childComp in designer.AssociatedComponents)
            {
                if (childComp.Site != null)
                {
                    list.Add(childComp);
                    GetAssociatedComponents(childComp, host, list);
                }
            }
        }

        /// <summary>
        ///  Used to retrieve the current location of the given component.
        /// </summary>
        private Point GetLocation(IComponent comp)
        {
            PropertyDescriptor prop = GetProperty(comp, "Location");
            if (prop != null)
            {
                try
                {
                    return (Point)prop.GetValue(comp);
                }
                catch (Exception e)
                {
                    Debug.Fail("Commands may be disabled, the location property was not accessible", e.ToString());
                    if (ClientUtils.IsCriticalException(e))
                    {
                        throw;
                    }
                }
            }
            return Point.Empty;
        }

        /// <summary>
        ///  Retrieves the given property on the given component.
        /// </summary>
        protected PropertyDescriptor GetProperty(object comp, string propName)
        {
            return TypeDescriptor.GetProperties(comp)[propName];
        }

        /// <summary>
        ///  Retrieves the requested service.
        /// </summary>
        protected virtual object GetService(Type serviceType)
        {
            if (site != null)
            {
                return site.GetService(serviceType);
            }
            return null;
        }

        /// <summary>
        ///  Used to retrieve the current size of the given component.
        /// </summary>
        private Size GetSize(IComponent comp)
        {
            PropertyDescriptor prop = GetProperty(comp, "Size");
            if (prop != null)
            {
                return (Size)prop.GetValue(comp);
            }
            return Size.Empty;
        }

        /// <summary>
        ///  Retrieves the snap information for the given component.
        /// </summary>
        protected virtual void GetSnapInformation(IDesignerHost host, IComponent component, out Size snapSize, out IComponent snapComponent, out PropertyDescriptor snapProperty)
        {
            // This implementation is shared by all.  It just looks for snap properties on the base component.
            IComponent currentSnapComponent;
            PropertyDescriptor gridSizeProp;
            PropertyDescriptor currentSnapProp;
            PropertyDescriptorCollection props;

            currentSnapComponent = host.RootComponent;
            props = TypeDescriptor.GetProperties(currentSnapComponent);
            currentSnapProp = props["SnapToGrid"];
            if (currentSnapProp != null && currentSnapProp.PropertyType != typeof(bool))
            {
                currentSnapProp = null;
            }
            gridSizeProp = props["GridSize"];
            if (gridSizeProp != null && gridSizeProp.PropertyType != typeof(Size))
            {
                gridSizeProp = null;
            }
            // Finally, now that we've got the various properties and components, dole out the values.
            snapComponent = currentSnapComponent;
            snapProperty = currentSnapProp;
            if (gridSizeProp != null)
            {
                snapSize = (Size)gridSizeProp.GetValue(snapComponent);
            }
            else
            {
                snapSize = Size.Empty;
            }
        }

        /// <summary>
        ///  Called before doing any change to multiple controls to check if we have the right to make any change otherwise we would get a checkout message for each control we call setvalue on
        /// </summary>
        protected bool CanCheckout(IComponent comp)
        {
            // look if it's ok to change
            IComponentChangeService changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            // is it ok to change?
            if (changeSvc != null)
            {
                try
                {
                    changeSvc.OnComponentChanging(comp, null);
                }
                catch (CheckoutException chkex)
                {
                    if (chkex == CheckoutException.Canceled)
                    {
                        return false;
                    }

                    throw chkex;
                }
            }
            return true;
        }

        /// <summary>
        ///  Called by the event handler service when the current event handler has changed.  Here we invalidate all of our menu items so that they can pick up the new event handler.
        /// </summary>
        private void OnEventHandlerChanged(object sender, EventArgs e)
        {
            OnUpdateCommandStatus();
        }

        /// <summary>
        ///  Called for the two cancel commands we support.
        /// </summary>
        private void OnKeyCancel(object sender, EventArgs e)
        {
            OnKeyCancel(sender);
        }

        /// <summary>
        ///  Called for the two cancel commands we support.  Returns true If we did anything with the cancel, or false if not.
        /// </summary>
        protected virtual bool OnKeyCancel(object sender)
        {
            bool handled = false;
            // The base implementation here just checks to see if we are dragging. If we are, then we abort the drag.
            if (BehaviorService != null && BehaviorService.HasCapture)
            {
                BehaviorService.OnLoseCapture();
                handled = true;
            }
            else
            {
                IToolboxService tbx = (IToolboxService)GetService(typeof(IToolboxService));
                if (tbx != null && tbx.GetSelectedToolboxItem((IDesignerHost)GetService(typeof(IDesignerHost))) != null)
                {
                    tbx.SelectedToolboxItemUsed();
                    NativeMethods.GetCursorPos(out Point p);
                    IntPtr hwnd = NativeMethods.WindowFromPoint(p.X, p.Y);
                    if (hwnd != IntPtr.Zero)
                    {
                        NativeMethods.SendMessage(hwnd, WindowMessages.WM_SETCURSOR, hwnd, (IntPtr)NativeMethods.HTCLIENT);
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
        protected void OnKeyDefault(object sender, EventArgs e)
        {
            // Return key.  Handle it like a double-click on the primary selection
            ISelectionService selSvc = SelectionService;
            if (selSvc != null)
            {
                if (selSvc.PrimarySelection is IComponent pri)
                {
                    IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    if (host != null)
                    {
                        IDesigner designer = host.GetDesigner(pri);

                        if (designer != null)
                        {
                            designer.DoDefaultAction();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Called for all cursor movement commands.
        /// </summary>
        protected virtual void OnKeyMove(object sender, EventArgs e)
        {
            // Arrow keys.  Begin a drag if the selection isn't locked.
            ISelectionService selSvc = SelectionService;
            if (selSvc != null)
            {
                if (selSvc.PrimarySelection is IComponent comp)
                {
                    IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    if (host != null)
                    {
                        PropertyDescriptor lockedProp = TypeDescriptor.GetProperties(comp)["Locked"];
                        if (lockedProp == null || (lockedProp.PropertyType == typeof(bool) && ((bool)lockedProp.GetValue(comp))) == false)
                        {
                            CommandID cmd = ((MenuCommand)sender).CommandID;
                            bool invertSnap = false;
                            int moveOffsetX = 0;
                            int moveOffsetY = 0;

                            if (cmd.Equals(MenuCommands.KeyMoveUp))
                            {
                                moveOffsetY = -1;
                            }
                            else if (cmd.Equals(MenuCommands.KeyMoveDown))
                            {
                                moveOffsetY = 1;
                            }
                            else if (cmd.Equals(MenuCommands.KeyMoveLeft))
                            {
                                moveOffsetX = -1;
                            }
                            else if (cmd.Equals(MenuCommands.KeyMoveRight))
                            {
                                moveOffsetX = 1;
                            }
                            else if (cmd.Equals(MenuCommands.KeyNudgeUp))
                            {
                                moveOffsetY = -1;
                                invertSnap = true;
                            }
                            else if (cmd.Equals(MenuCommands.KeyNudgeDown))
                            {
                                moveOffsetY = 1;
                                invertSnap = true;
                            }
                            else if (cmd.Equals(MenuCommands.KeyNudgeLeft))
                            {
                                moveOffsetX = -1;
                                invertSnap = true;
                            }
                            else if (cmd.Equals(MenuCommands.KeyNudgeRight))
                            {
                                moveOffsetX = 1;
                                invertSnap = true;
                            }
                            else
                            {
                                Debug.Fail("Unknown command mapped to OnKeyMove: " + cmd.ToString());
                            }

                            DesignerTransaction trans;
                            if (selSvc.SelectionCount > 1)
                            {
                                trans = host.CreateTransaction(string.Format(SR.DragDropMoveComponents, selSvc.SelectionCount));
                            }
                            else
                            {
                                trans = host.CreateTransaction(string.Format(SR.DragDropMoveComponent, comp.Site.Name));
                            }
                            try
                            {
                                //if we can find the behaviorservice, then we can use it and the SnapLineEngine to help us move these controls...
                                if (BehaviorService != null)
                                {
                                    Control primaryControl = comp as Control; //this can be null (when we are moving a component in the ComponenTray)
                                    bool useSnapLines = BehaviorService.UseSnapLines;
                                    // If we have previous snaplines, we always want to erase them, no matter what. VS Whidbey #397709
                                    if (dragManager != null)
                                    {
                                        EndDragManager();
                                    }

                                    //If we CTRL+Arrow and we're using SnapLines - snap to the next location. Don't snap if we are moving a component in the ComponentTray
                                    if (invertSnap && useSnapLines && primaryControl != null)
                                    {
                                        ArrayList selComps = new ArrayList(selSvc.GetSelectedComponents());
                                        //create our snapline engine
                                        dragManager = new DragAssistanceManager(comp.Site, selComps);
                                        //ask our snapline engine to find the nearest snap position with the given direction
                                        Point snappedOffset = dragManager.OffsetToNearestSnapLocation(primaryControl, new Point(moveOffsetX, moveOffsetY));
                                        //update the offset according to the snapline engine
                                        // This is the offset assuming origin is in the upper-left.
                                        moveOffsetX = snappedOffset.X;
                                        moveOffsetY = snappedOffset.Y;
                                        // If the parent is mirrored then we need to negate moveOffsetX. This is because moveOffsetX assumes that the origin is upper left. That is, when moveOffsetX is positive, we are moving right, negative when moving left. The parent container's origin depends on its mirroring property. Thus when we call propLoc.setValue below, we need to make sure that our moveOffset.X correctly reflects the placement of the parent container's origin. We need to do this AFTER we calculate the snappedOffset. This is because the dragManager calculations are all based on an origin in the upper-left.
                                        if (primaryControl.Parent.IsMirrored)
                                        {
                                            moveOffsetX *= -1;
                                        }
                                    }
                                    //if we used a regular arrow key and we're in SnapToGrid mode...
                                    else if (!invertSnap && !useSnapLines)
                                    {
                                        bool snapOn = false;
                                        Size snapSize = Size.Empty;
                                        GetSnapInformation(host, comp, out snapSize, out IComponent snapComponent, out PropertyDescriptor snapProperty);
                                        if (snapProperty != null)
                                        {
                                            snapOn = (bool)snapProperty.GetValue(snapComponent);
                                        }
                                        if (snapOn && !snapSize.IsEmpty)
                                        {
                                            moveOffsetX *= snapSize.Width;
                                            moveOffsetY *= snapSize.Height;
                                            if (primaryControl != null)
                                            {
                                                //ask the parent to adjust our wanna-be snapped position
                                                if (host.GetDesigner(primaryControl.Parent) is ParentControlDesigner parentDesigner)
                                                {
                                                    Point loc = primaryControl.Location;
                                                    // If the parent is mirrored then we need to negate moveOffsetX. This is because moveOffsetX assumes that the origin is upper left. That is, when moveOffsetX is positive, we are moving right, negative when moving left. The parent container's origin depends on its mirroring property. Thus when we call propLoc.setValue below, we need to make sure that our moveOffset.X correctly reflects the placement of the parent container's origin. Should do this BEFORE we get the snapped point.
                                                    if (primaryControl.Parent.IsMirrored)
                                                    {
                                                        moveOffsetX *= -1;
                                                    }
                                                    loc.Offset(moveOffsetX, moveOffsetY);
                                                    loc = parentDesigner.GetSnappedPoint(loc);
                                                    //reset our offsets now that we've snapped correctly
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
                                            if (primaryControl != null && primaryControl.Parent.IsMirrored)
                                            {
                                                moveOffsetX *= -1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (primaryControl != null && primaryControl.Parent.IsMirrored)
                                        {
                                            moveOffsetX *= -1;
                                        }
                                    }

                                    SelectionRules rules = SelectionRules.Moveable | SelectionRules.Visible;
                                    foreach (IComponent component in selSvc.GetSelectedComponents())
                                    {
                                        if (host.GetDesigner(component) is ControlDesigner des && ((des.SelectionRules & rules) != rules))
                                        {
                                            //the control must match the rules, if not, then we don't move it
                                            continue;
                                        }
                                        // Components are always moveable and visible
                                        PropertyDescriptor propLoc = TypeDescriptor.GetProperties(component)["Location"];
                                        if (propLoc != null)
                                        {
                                            Point loc = (Point)propLoc.GetValue(component);
                                            loc.Offset(moveOffsetX, moveOffsetY);
                                            propLoc.SetValue(component, loc);
                                        }
                                        //change the Status information ....
                                        if (component == selSvc.PrimarySelection && _statusCommandUI != null)
                                        {
                                            _statusCommandUI.SetStatusInformation(component as Component);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                if (trans != null)
                                {
                                    trans.Commit();
                                }

                                if (dragManager != null)
                                {
                                    //start our timer for the snaplines
                                    SnapLineTimer.Start();

                                    //render any lines
                                    dragManager.RenderSnapLinesInternal();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Called for all alignment operations that key off of a primary selection.
        /// </summary>
        protected void OnMenuAlignByPrimary(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            CommandID id = cmd.CommandID;

            //Need to get the location for the primary control, we do this here (instead of onselectionchange) because the control could be dragged around once it is selected and might have a new location
            Point primaryLocation = GetLocation(primarySelection);
            Size primarySize = GetSize(primarySelection);
            if (SelectionService == null)
            {
                return;
            }

            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                // Now loop through each of the components.
                ICollection comps = SelectionService.GetSelectedComponents();
                // Inform the designer that we are about to monkey with a ton of properties.
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                DesignerTransaction trans = null;
                try
                {
                    if (host != null)
                    {
                        trans = host.CreateTransaction(string.Format(SR.CommandSetAlignByPrimary, comps.Count));
                    }

                    bool firstTry = true;
                    Point loc = Point.Empty;
                    foreach (object obj in comps)
                    {
                        if (obj == primarySelection)
                        {
                            continue;
                        }
                        IComponent comp = obj as IComponent;
                        if (comp != null && host != null)
                        {
                            if (!(host.GetDesigner(comp) is ControlDesigner des))
                            {
                                continue;
                            }
                        }
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
                        PropertyDescriptor locProp = props["Location"];
                        PropertyDescriptor sizeProp = props["Size"];
                        PropertyDescriptor lockProp = props["Locked"];
                        // Skip all components that are locked
                        //
                        if (lockProp != null)
                        {
                            if ((bool)lockProp.GetValue(comp))
                            {
                                continue;
                            }
                        }

                        // Skip all components that don't have a location property
                        //
                        if (locProp == null || locProp.IsReadOnly)
                        {
                            continue;
                        }

                        // Skip all components that don't have size if we're doing a size operation.
                        if (id.Equals(MenuCommands.AlignBottom) ||
                            id.Equals(MenuCommands.AlignHorizontalCenters) ||
                            id.Equals(MenuCommands.AlignVerticalCenters) ||
                            id.Equals(MenuCommands.AlignRight))
                        {
                            if (sizeProp == null || sizeProp.IsReadOnly)
                            {
                                continue;
                            }
                        }

                        // Align bottom
                        if (id.Equals(MenuCommands.AlignBottom))
                        {
                            loc = (Point)locProp.GetValue(comp);
                            Size size = (Size)sizeProp.GetValue(comp);
                            loc.Y = primaryLocation.Y + primarySize.Height - size.Height;
                        }
                        // Align horizontal centers
                        else if (id.Equals(MenuCommands.AlignHorizontalCenters))
                        {
                            loc = (Point)locProp.GetValue(comp);
                            Size size = (Size)sizeProp.GetValue(comp);
                            loc.Y = primarySize.Height / 2 + primaryLocation.Y - size.Height / 2;
                        }
                        // Align left
                        else if (id.Equals(MenuCommands.AlignLeft))
                        {
                            loc = (Point)locProp.GetValue(comp);
                            loc.X = primaryLocation.X;
                        }
                        // Align right
                        else if (id.Equals(MenuCommands.AlignRight))
                        {
                            loc = (Point)locProp.GetValue(comp);
                            Size size = (Size)sizeProp.GetValue(comp);
                            loc.X = primaryLocation.X + primarySize.Width - size.Width;
                        }
                        // Align top
                        else if (id.Equals(MenuCommands.AlignTop))
                        {
                            loc = (Point)locProp.GetValue(comp);
                            loc.Y = primaryLocation.Y;
                        }
                        // Align vertical centers
                        else if (id.Equals(MenuCommands.AlignVerticalCenters))
                        {
                            loc = (Point)locProp.GetValue(comp);
                            Size size = (Size)sizeProp.GetValue(comp);
                            loc.X = primarySize.Width / 2 + primaryLocation.X - size.Width / 2;
                        }
                        else
                        {
                            Debug.Fail("Unrecognized command: " + id.ToString());
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
                    if (trans != null)
                    {
                        trans.Commit();
                    }
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
        protected void OnMenuAlignToGrid(object sender, EventArgs e)
        {
            Size gridSize = Size.Empty;
            if (SelectionService == null)
            {
                return;
            }

            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ICollection selectedComponents = SelectionService.GetSelectedComponents();
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                DesignerTransaction trans = null;
                try
                {
                    if (host != null)
                    {
                        trans = host.CreateTransaction(string.Format(SR.CommandSetAlignToGrid, selectedComponents.Count));
                        if (host.RootComponent is Control baseComponent)
                        {
                            PropertyDescriptor prop = GetProperty(baseComponent, "GridSize");
                            if (prop != null)
                            {
                                gridSize = (Size)prop.GetValue(baseComponent);
                            }
                            if (prop == null || gridSize.IsEmpty)
                            {
                                //bail silently here
                                return;
                            }
                        }

                    }
                    bool firstTry = true;
                    // for each component, we round to the nearest snap offset for x and y
                    foreach (object comp in selectedComponents)
                    {
                        // first check to see if the component is locked, if so - don't move it...
                        PropertyDescriptor lockedProp = GetProperty(comp, "Locked");
                        if (lockedProp != null && ((bool)lockedProp.GetValue(comp)) == true)
                        {
                            continue;
                        }
                        // if the designer for this component isn't a ControlDesigner (maybe it's something in the component tray) then don't try to align it to grid.
                        IComponent component = comp as IComponent;
                        if (component != null && host != null)
                        {
                            if (!(host.GetDesigner(component) is ControlDesigner des))
                            {
                                continue;
                            }
                        }

                        // get the location property
                        PropertyDescriptor locProp = GetProperty(comp, "Location");

                        // get the current value
                        if (locProp == null || locProp.IsReadOnly)
                        {
                            continue;
                        }
                        Point loc = (Point)locProp.GetValue(comp);

                        // round the x to the snap size
                        int delta = loc.X % gridSize.Width;
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
                        if (firstTry && !CanCheckout(component))
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
                    if (trans != null)
                    {
                        trans.Commit();
                    }
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
        protected void OnMenuCenterSelection(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            CommandID cmdID = cmd.CommandID;
            if (SelectionService == null)
            {
                return;
            }

            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                // NOTE: this only works on Control types
                ICollection selectedComponents = SelectionService.GetSelectedComponents();
                Control viewParent = null;
                Size size = Size.Empty;
                Point loc = Point.Empty;
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                DesignerTransaction trans = null;
                try
                {
                    if (host != null)
                    {
                        string batchString;

                        if (cmdID == MenuCommands.CenterHorizontally)
                        {
                            batchString = string.Format(SR.WindowsFormsCommandCenterX, selectedComponents.Count);
                        }
                        else
                        {
                            batchString = string.Format(SR.WindowsFormsCommandCenterY, selectedComponents.Count);
                        }
                        trans = host.CreateTransaction(batchString);
                    }
                    //subhag calculate the union REctangle
                    int top = int.MaxValue;
                    int left = int.MaxValue;
                    int right = int.MinValue;
                    int bottom = int.MinValue;

                    foreach (object obj in selectedComponents)
                    {
                        if (obj is Control)
                        {
                            IComponent comp = (IComponent)obj;
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
                            PropertyDescriptor locProp = props["Location"];
                            PropertyDescriptor sizeProp = props["Size"];
                            // Skip all components that don't have location and size properties
                            if (locProp == null || sizeProp == null || locProp.IsReadOnly || sizeProp.IsReadOnly)
                            {
                                continue;
                            }

                            // Also, skip all locked componenents...
                            PropertyDescriptor lockProp = props["Locked"];
                            if (lockProp != null && (bool)lockProp.GetValue(comp) == true)
                            {
                                continue;
                            }

                            size = (Size)sizeProp.GetValue(comp);
                            loc = (Point)locProp.GetValue(comp);

                            //cache the first parent we see - if there's a mix of different parents - we'll just center based on the first one
                            if (viewParent == null)
                            {
                                viewParent = ((Control)comp).Parent;
                            }

                            if (loc.X < left)
                            {
                                left = loc.X;
                            }

                            if (loc.Y < top)
                            {
                                top = loc.Y;
                            }

                            if (loc.X + size.Width > right)
                            {
                                right = loc.X + size.Width;
                            }

                            if (loc.Y + size.Height > bottom)
                            {
                                bottom = loc.Y + size.Height;
                            }
                        }
                    }

                    //if we never found a viewParent (some read-only inherited scenarios then simply bail
                    if (viewParent == null)
                    {
                        return;
                    }

                    int centerOfUnionRectX = (left + right) / 2;
                    int centerOfUnionRectY = (top + bottom) / 2;
                    int centerOfParentX = (viewParent.ClientSize.Width) / 2;
                    int centerOfParentY = (viewParent.ClientSize.Height) / 2;
                    int deltaX = 0;
                    int deltaY = 0;
                    bool shiftRight = false;
                    bool shiftBottom = false;
                    if (centerOfParentX >= centerOfUnionRectX)
                    {
                        deltaX = centerOfParentX - centerOfUnionRectX;
                        shiftRight = true;
                    }
                    else
                    {
                        deltaX = centerOfUnionRectX - centerOfParentX;
                    }

                    if (centerOfParentY >= centerOfUnionRectY)
                    {
                        deltaY = centerOfParentY - centerOfUnionRectY;
                        shiftBottom = true;
                    }
                    else
                    {
                        deltaY = centerOfUnionRectY - centerOfParentY;
                    }

                    bool firstTry = true;
                    foreach (object obj in selectedComponents)
                    {
                        if (obj is Control)
                        {
                            IComponent comp = (IComponent)obj;
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
                            PropertyDescriptor locProp = props["Location"];
                            if (locProp.IsReadOnly)
                            {
                                continue;
                            }

                            loc = (Point)locProp.GetValue(comp);

                            if (cmdID == MenuCommands.CenterHorizontally)
                            {
                                if (shiftRight)
                                {
                                    loc.X += deltaX;
                                }
                                else
                                {
                                    loc.X -= deltaX;
                                }
                            }
                            else if (cmdID == MenuCommands.CenterVertically)
                            {
                                if (shiftBottom)
                                {
                                    loc.Y += deltaY;
                                }
                                else
                                {
                                    loc.Y -= deltaY;
                                }
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
                    if (trans != null)
                    {
                        trans.Commit();
                    }
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
        protected void OnMenuCopy(object sender, EventArgs e)
        {
            if (SelectionService == null)
            {
                return;
            }

            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ICollection selectedComponents = GetCopySelection();
                selectedComponents = PrependComponentNames(selectedComponents);
                IDesignerSerializationService ds = (IDesignerSerializationService)GetService(typeof(IDesignerSerializationService));
                Debug.Assert(ds != null, "No designer serialization service -- we cannot copy to clipboard");
                if (ds != null)
                {
                    object serializationData = ds.Serialize(selectedComponents);
                    MemoryStream stream = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, serializationData);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] bytes = stream.GetBuffer();
                    IDataObject dataObj = new DataObject(CF_DESIGNER, bytes);
                    Clipboard.SetDataObject(dataObj);
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
        protected void OnMenuCut(object sender, EventArgs e)
        {
            if (SelectionService == null)
            {
                return;
            }
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ICollection selectedComponents = GetCopySelection();
                int cutCount = selectedComponents.Count;
                selectedComponents = PrependComponentNames(selectedComponents);
                IDesignerSerializationService ds = (IDesignerSerializationService)GetService(typeof(IDesignerSerializationService));
                Debug.Assert(ds != null, "No designer serialization service -- we cannot copy to clipboard");
                if (ds != null)
                {
                    object serializationData = ds.Serialize(selectedComponents);
                    MemoryStream stream = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, serializationData);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] bytes = stream.GetBuffer();
                    IDataObject dataObj = new DataObject(CF_DESIGNER, bytes);
                    Clipboard.SetDataObject(dataObj);
                    IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    Control commonParent = null;
                    if (host != null)
                    {
                        IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                        DesignerTransaction trans = null;

                        ArrayList designerList = new ArrayList();
                        try
                        {
                            trans = host.CreateTransaction(string.Format(SR.CommandSetCutMultiple, cutCount));
                            // clear the selected components so we aren't browsing them
                            SelectionService.SetSelectedComponents(Array.Empty<object>(), SelectionTypes.Replace);
                            object[] selComps = new object[selectedComponents.Count];
                            selectedComponents.CopyTo(selComps, 0);
                            for (int i = 0; i < selComps.Length; i++)
                            {
                                object obj = selComps[i];
                                // We should never delete the base component.
                                //
                                if (obj == host.RootComponent || !(obj is IComponent component))
                                {
                                    continue;
                                }
                                //Perf: We suspend Component Changing Events on parent for bulk changes to avoid unnecessary serialization\deserialization for undo
                                if (obj is Control c)
                                {
                                    Control parent = c.Parent;
                                    if (parent != null)
                                    {
                                        if (host.GetDesigner(parent) is ParentControlDesigner designer && !designerList.Contains(designer))
                                        {
                                            designer.SuspendChangingEvents();
                                            designerList.Add(designer);
                                            designer.ForceComponentChanging();
                                        }
                                    }
                                }
                            }
                            // go backward so we destroy parents before children

                            for (int i = 0; i < selComps.Length; i++)
                            {
                                object obj = selComps[i];
                                // We should never delete the base component.
                                //
                                if (obj == host.RootComponent || !(obj is IComponent component))
                                {
                                    continue;
                                }

                                Control c = obj as Control;
                                //Cannot use idx = 1 to check (see diff) due to the call to PrependComponentNames, which adds non IComponent objects to the beginning of selectedComponents. Thus when we finally get here idx would be > 1.
                                if (commonParent == null && c != null)
                                {
                                    commonParent = c.Parent;
                                }
                                else if (commonParent != null && c != null)
                                {
                                    Control selectedControl = c;

                                    if (selectedControl.Parent != commonParent && !commonParent.Contains(selectedControl))
                                    {
                                        // look for internal parenting
                                        if (selectedControl == commonParent || selectedControl.Contains(commonParent))
                                        {
                                            commonParent = selectedControl.Parent;
                                        }
                                        else
                                        {
                                            commonParent = null;
                                        }
                                    }
                                }
                                if (component != null)
                                {
                                    ArrayList al = new ArrayList();
                                    GetAssociatedComponents(component, host, al);
                                    foreach (IComponent comp in al)
                                    {
                                        changeService.OnComponentChanging(comp, null);
                                    }

                                    host.DestroyComponent(component);
                                }
                            }
                        }
                        finally
                        {
                            if (trans != null)
                            {
                                trans.Commit();
                            }

                            foreach (ParentControlDesigner des in designerList)
                            {
                                if (des != null)
                                {
                                    des.ResumeChangingEvents();
                                }
                            }
                        }
                        if (commonParent != null)
                        {
                            SelectionService.SetSelectedComponents(new object[] { commonParent }, SelectionTypes.Replace);
                        }
                        else if (SelectionService.PrimarySelection == null)
                        {
                            SelectionService.SetSelectedComponents(new object[] { host.RootComponent }, SelectionTypes.Replace);
                        }
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
        protected void OnMenuDelete(object sender, EventArgs e)
        {
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (site != null)
                {
                    IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                    if (SelectionService == null)
                    {
                        return;
                    }

                    if (host != null)
                    {
                        IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                        ICollection comps = SelectionService.GetSelectedComponents();
                        string desc = string.Format(SR.CommandSetDelete, comps.Count);
                        DesignerTransaction trans = null;
                        IComponent commonParent = null;
                        bool commonParentSet = false;
                        ArrayList designerList = new ArrayList();
                        try
                        {
                            trans = host.CreateTransaction(desc);
                            SelectionService.SetSelectedComponents(Array.Empty<object>(), SelectionTypes.Replace);
                            foreach (object obj in comps)
                            {
                                if (!(obj is IComponent comp) || comp.Site == null)
                                {
                                    continue;
                                }
                                //Perf: We suspend Component Changing Events on parent for bulk changes to avoid unnecessary serialization\deserialization for undo
                                if (obj is Control c)
                                {
                                    Control parent = c.Parent;
                                    if (parent != null)
                                    {
                                        if (host.GetDesigner(parent) is ParentControlDesigner designer && !designerList.Contains(designer))
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
                                // If it's not a component, we can't delete it.  It also may have already been deleted as part of a parent operation, so we skip it.
                                if (!(obj is IComponent c) || c.Site == null)
                                {
                                    continue;
                                }
                                // We should never delete the base component.
                                if (obj == host.RootComponent)
                                {
                                    continue;
                                }
                                Control control = obj as Control;
                                if (!commonParentSet)
                                {
                                    if (control != null)
                                    {
                                        commonParent = control.Parent;
                                    }
                                    else
                                    {
                                        // if this is not a Control, see if we can get an ITreeDesigner from it, and figure out the Component from that.
                                        if (host.GetDesigner((IComponent)obj) is ITreeDesigner designer)
                                        {
                                            IDesigner parentDesigner = designer.Parent;
                                            if (parentDesigner != null)
                                            {
                                                commonParent = parentDesigner.Component;
                                            }
                                        }
                                    }
                                    commonParentSet = (commonParent != null);
                                }
                                else if (commonParent != null)
                                {
                                    if (control != null && commonParent is Control)
                                    {
                                        Control selectedControl = control;
                                        Control controlCommonParent = (Control)commonParent;
                                        if (selectedControl.Parent != controlCommonParent && !controlCommonParent.Contains(selectedControl))
                                        {
                                            // look for internal parenting
                                            if (selectedControl == controlCommonParent || selectedControl.Contains(controlCommonParent))
                                            {
                                                commonParent = selectedControl.Parent;
                                            }
                                            else
                                            {
                                                // start walking up until we find a common parent
                                                while (controlCommonParent != null && !controlCommonParent.Contains(selectedControl))
                                                {
                                                    controlCommonParent = controlCommonParent.Parent;
                                                }
                                                commonParent = controlCommonParent;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // for these we aren't as thorough as we are with the Control-based ones. we just walk up the chain until we find that parent or the root component.
                                        if (host.GetDesigner((IComponent)obj) is ITreeDesigner designer && host.GetDesigner(commonParent) is ITreeDesigner commonParentDesigner && designer.Parent != commonParentDesigner)
                                        {
                                            ArrayList designerChain = new ArrayList();
                                            ArrayList parentDesignerChain = new ArrayList();
                                            // walk the chain of designers from the current parent designer up to the root component, and for the current component designer.
                                            for (designer = designer.Parent as ITreeDesigner;
                                                 designer != null;
                                                 designer = designer.Parent as ITreeDesigner)
                                            {
                                                designerChain.Add(designer);
                                            }

                                            for (commonParentDesigner = commonParentDesigner.Parent as ITreeDesigner; commonParentDesigner != null; commonParentDesigner = commonParentDesigner.Parent as ITreeDesigner)
                                            {
                                                parentDesignerChain.Add(commonParentDesigner);
                                            }

                                            // now that we've got the trees built up, start comparing them from the ends to see where they diverge.
                                            ArrayList shorterList = designerChain.Count < parentDesignerChain.Count ? designerChain : parentDesignerChain;
                                            ArrayList longerList = (shorterList == designerChain ? parentDesignerChain : designerChain);
                                            commonParentDesigner = null;
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
                                                    commonParentDesigner = (ITreeDesigner)shorterList[shortIndex];
                                                    shortIndex--;
                                                    longIndex--;
                                                }
                                            }
                                            // alright, what have we got?
                                            if (commonParentDesigner != null)
                                            {
                                                commonParent = commonParentDesigner.Component;
                                            }
                                            else
                                            {
                                                commonParent = null;
                                            }
                                        }
                                    }
                                }
                                ArrayList al = new ArrayList();
                                GetAssociatedComponents((IComponent)obj, host, al);
                                foreach (IComponent comp in al)
                                {
                                    changeService.OnComponentChanging(comp, null);
                                }
                                host.DestroyComponent((IComponent)obj);
                            }
                        }
                        finally
                        {
                            if (trans != null)
                            {
                                trans.Commit();
                            }

                            foreach (ParentControlDesigner des in designerList)
                            {
                                if (des != null)
                                {
                                    des.ResumeChangingEvents();
                                }
                            }
                        }

                        if (commonParent != null && SelectionService.PrimarySelection == null)
                        {
                            if (host.GetDesigner(commonParent) is ITreeDesigner commonParentDesigner && commonParentDesigner.Children != null)
                            {
                                // choose the first child of the common parent if it has any.
                                foreach (IDesigner designer in commonParentDesigner.Children)
                                {
                                    IComponent component = designer.Component;
                                    if (component.Site != null)
                                    {
                                        commonParent = component;
                                        break;
                                    }
                                }
                            }
                            else if (commonParent is Control controlCommonParent)
                            {
                                // if we have a common parent, select it's first child
                                if (controlCommonParent.Controls.Count > 0)
                                {
                                    controlCommonParent = controlCommonParent.Controls[0];
                                    while (controlCommonParent != null && controlCommonParent.Site == null)
                                    {
                                        controlCommonParent = controlCommonParent.Parent;
                                    }
                                    commonParent = controlCommonParent;
                                }
                            }
                            if (commonParent != null)
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
                            if (SelectionService.PrimarySelection == null)
                            {
                                SelectionService.SetSelectedComponents(new object[] { host.RootComponent }, SelectionTypes.Replace);
                            }
                        }
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
        protected void OnMenuPaste(object sender, EventArgs e)
        {
            Cursor oldCursor = Cursor.Current;
            ArrayList designerList = new ArrayList();
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                // If a control fails to get pasted; then we should remember its associatedComponents  so that they are not pasted.
                ICollection associatedCompsOfFailedContol = null;
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                if (host == null)
                {
                    return; // nothing we can do here!
                }

                IDataObject dataObj = Clipboard.GetDataObject();
                ICollection components = null;
                bool createdItems = false;
                ComponentTray tray = null;
                int numberOfOriginalTrayControls = 0;
                // Get the current number of controls in the Component Tray in the target
                tray = GetService(typeof(ComponentTray)) as ComponentTray;
                numberOfOriginalTrayControls = tray != null ? tray.Controls.Count : 0;
                // We understand two things:  CF_DESIGNER, and toolbox items.
                object data = dataObj.GetData(CF_DESIGNER);
                using (DesignerTransaction trans = host.CreateTransaction(SR.CommandSetPaste))
                {
                    if (data is byte[] bytes)
                    {
                        MemoryStream s = new MemoryStream(bytes);
                        if (s != null)
                        {
                            // CF_DESIGNER was put on the clipboard by us using the designer serialization service.
                            IDesignerSerializationService ds = (IDesignerSerializationService)GetService(typeof(IDesignerSerializationService));
                            if (ds != null)
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                s.Seek(0, SeekOrigin.Begin);
                                object serializationData = formatter.Deserialize(s);
                                components = ds.Deserialize(serializationData);
                            }
                        }
                    }
                    else
                    {
                        // Now check for a toolbox item.
                        IToolboxService ts = (IToolboxService)GetService(typeof(IToolboxService));
                        if (ts != null && ts.IsSupported(dataObj, host))
                        {
                            ToolboxItem ti = ts.DeserializeToolboxItem(dataObj, host);
                            if (ti != null)
                            {
                                components = ti.CreateComponents(host);
                                createdItems = true;
                            }
                        }
                    }

                    // Now, if we got some components, hook 'em up!
                    if (components != null && components.Count > 0)
                    {
                        IComponent curComp;
                        string name;
                        //Make copy of Items in Array..
                        object[] allComponents = new object[components.Count];
                        components.CopyTo(allComponents, 0);
                        ArrayList selectComps = new ArrayList();
                        ArrayList controls = new ArrayList();
                        string[] componentNames = null;
                        int idx = 0;
                        // if the selected item is a frame designer, add to that, otherwise add to the form
                        IComponent selectedComponent = null;
                        IDesigner designer = null;
                        bool dragClient = false;

                        IComponent baseComponent = host.RootComponent;
                        selectedComponent = (IComponent)SelectionService.PrimarySelection;
                        if (selectedComponent == null)
                        {
                            selectedComponent = baseComponent;
                        }

                        dragClient = false;
                        ITreeDesigner tree = host.GetDesigner(selectedComponent) as ITreeDesigner;
                        while (!dragClient && tree != null)
                        {
                            if (tree is IOleDragClient)
                            {
                                designer = tree;
                                dragClient = true;
                            }
                            else
                            {
                                if (tree == tree.Parent)
                                {
                                    break;
                                }

                                tree = tree.Parent as ITreeDesigner;
                            }
                        }

                        foreach (object obj in components)
                        {

                            name = null;
                            curComp = obj as IComponent;
                            // see if we can fish out the original name. When we serialized, we serialized an array of names at the head of the list.  This array matches the components that were created.
                            if (obj is IComponent)
                            {
                                if (componentNames != null && idx < componentNames.Length)
                                {
                                    name = componentNames[idx++];
                                }
                            }
                            else
                            {
                                if (componentNames == null && obj is string[] sa)
                                {
                                    componentNames = sa;
                                    idx = 0;
                                    continue;
                                }
                            }

                            if (GetService(typeof(IEventBindingService)) is IEventBindingService evs)
                            {
                                PropertyDescriptorCollection eventProps = evs.GetEventProperties(TypeDescriptor.GetEvents(curComp));
                                foreach (PropertyDescriptor pd in eventProps)
                                {
                                    // If we couldn't find a property for this event, or of the property is read only, then abort.
                                    if (pd == null || pd.IsReadOnly)
                                    {
                                        continue;
                                    }

                                    if (pd.GetValue(curComp) is string handler)
                                    {
                                        pd.SetValue(curComp, null);
                                    }
                                }
                            }

                            if (dragClient)
                            {
                                bool foundAssociatedControl = false;
                                // If we have failed to add a control in this Paste operation ...
                                if (associatedCompsOfFailedContol != null)
                                {
                                    // then dont add its children controls.
                                    foreach (Component comp in associatedCompsOfFailedContol)
                                    {
                                        if (comp == obj as Component)
                                        {
                                            foundAssociatedControl = true;
                                            break;
                                        }
                                    }
                                }

                                if (foundAssociatedControl)
                                {
                                    continue; //continue from here so that we dont add the associted compoenet of a control that failed paste operation.
                                }

                                ICollection designerComps = null;
                                // DGV has columns which are sited IComponents that don't have designers.  in this case, ignore them.
                                if (!(host.GetDesigner(curComp) is ComponentDesigner cDesigner))
                                {
                                    continue;
                                }
                                //store associatedComponents.
                                designerComps = cDesigner.AssociatedComponents;
                                ComponentDesigner parentCompDesigner = ((ITreeDesigner)cDesigner).Parent as ComponentDesigner;
                                Component parentComp = null;
                                if (parentCompDesigner != null)
                                {
                                    parentComp = parentCompDesigner.Component as Component;
                                }

                                ArrayList associatedComps = new ArrayList();
                                if (parentComp != null)
                                {
                                    if (parentCompDesigner != null)
                                    {
                                        foreach (IComponent childComp in parentCompDesigner.AssociatedComponents)
                                        {
                                            associatedComps.Add(childComp as Component);
                                        }
                                    }
                                }

                                if (parentComp == null || !(associatedComps.Contains(curComp)))
                                {
                                    if (parentComp != null)
                                    {
                                        if (host.GetDesigner(parentComp) is ParentControlDesigner parentDesigner && !designerList.Contains(parentDesigner))
                                        {
                                            parentDesigner.SuspendChangingEvents();
                                            designerList.Add(parentDesigner);
                                            parentDesigner.ForceComponentChanging();
                                        }
                                    }

                                    if (!((IOleDragClient)designer).AddComponent(curComp, name, createdItems))
                                    {
                                        //cache the associatedComponents only for FAILED control.
                                        associatedCompsOfFailedContol = designerComps;
                                        // now we will jump out of the using block and call trans.Dispose() which in turn calls trans.Cancel for an uncommited transaction, We want to cancel the transaction because otherwise we'll have  un-parented controls
                                        return;
                                    }

                                    Control designerControl = ((IOleDragClient)designer).GetControlForComponent(curComp);
                                    if (designerControl != null)
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
                                    // if the text is the same as the name, remember it. After we add the control, we'll update the text with the new name.
                                    if (name != null && name.Equals(c.Text))
                                    {
                                        changeName = true;
                                    }
                                }
                                if (changeName)
                                {
                                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(curComp);
                                    PropertyDescriptor nameProp = props["Name"];
                                    if (nameProp != null && nameProp.PropertyType == typeof(string))
                                    {
                                        string newName = (string)nameProp.GetValue(curComp);
                                        if (!newName.Equals(name))
                                        {
                                            PropertyDescriptor textProp = props["Text"];
                                            if (textProp != null && textProp.PropertyType == nameProp.PropertyType)
                                            {
                                                textProp.SetValue(curComp, nameProp.GetValue(curComp));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // Find those controls that have ControlDesigners and center them on the designer surface
                        ArrayList compsWithControlDesigners = new ArrayList();
                        foreach (Control c in controls)
                        {
                            IDesigner des = host.GetDesigner((IComponent)c);
                            if (des is ControlDesigner)
                            {
                                compsWithControlDesigners.Add(c);
                            }
                        }

                        if (compsWithControlDesigners.Count > 0)
                        {
                            // Update the control positions.  We want to keep the entire block of controls relative to each other, but relocate them within the container.
                            UpdatePastePositions(compsWithControlDesigners);
                        }

                        // Figure out if we added components to the component tray, and have the tray adjust their position. MartinTh - removed the old check, since ToolStrips breaks the scenario. ToolStrips have a ControlDesigner, but also add a component to the tray. The old code wouldn't detect that, so the tray location wouldn't get adjusted. Rather than fixing this up in ToolStripKeyboardHandlingService.OnCommandPaste, we do it here, since doing it in the service, wouldn't handle cross-form paste.
                        if (tray == null)
                        {
                            // the paste target did not have a tray already, so let's go get it - if there is one
                            tray = GetService(typeof(ComponentTray)) as ComponentTray;
                        }
                        if (tray != null)
                        {
                            int numberOfTrayControlsAdded = tray.Controls.Count - numberOfOriginalTrayControls;
                            if (numberOfTrayControlsAdded > 0)
                            {
                                ArrayList listOfTrayControls = new ArrayList();
                                for (int i = 0; i < numberOfTrayControlsAdded; i++)
                                {
                                    listOfTrayControls.Add(tray.Controls[numberOfOriginalTrayControls + i]);
                                }
                                tray.UpdatePastePositions(listOfTrayControls);
                            }
                        }

                        // Update the tab indices of all the components.  We must first sort the components by their existing tab indices or else we will not preserve their original intent.
                        controls.Sort(new TabIndexCompare());
                        foreach (Control c in controls)
                        {
                            UpdatePasteTabIndex(c, c.Parent);
                        }

                        // finally select all the components we added
                        SelectionService.SetSelectedComponents((object[])selectComps.ToArray(), SelectionTypes.Replace);
                        // and bring them to the front - but only if we can mess with the Z-order.
                        if (designer is ParentControlDesigner parentControlDesigner && parentControlDesigner.AllowSetChildIndexOnDrop)
                        {
                            MenuCommand btf = MenuService.FindCommand(MenuCommands.BringToFront);
                            if (btf != null)
                            {
                                btf.Invoke();
                            }
                        }
                        trans.Commit();
                    }
                }
            }
            finally
            {
                Cursor.Current = oldCursor;
                foreach (ParentControlDesigner des in designerList)
                {
                    if (des != null)
                    {
                        des.ResumeChangingEvents();
                    }
                }
            }
        }

        /// <summary>
        ///  Called when the select all menu item is selected.
        /// </summary>
        protected void OnMenuSelectAll(object sender, EventArgs e)
        {
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (site != null)
                {
                    Debug.Assert(SelectionService != null, "We need the SelectionService, but we can't find it!");
                    if (SelectionService == null)
                    {
                        return;
                    }

                    IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                    if (host != null)
                    {
                        ComponentCollection components = host.Container.Components;
                        object[] selComps;
                        if (components == null || components.Count == 0)
                        {
                            selComps = Array.Empty<IComponent>();
                        }
                        else
                        {
                            selComps = new object[components.Count - 1];
                            object baseComp = host.RootComponent;

                            int j = 0;
                            foreach (IComponent comp in components)
                            {
                                if (baseComp == comp)
                                {
                                    continue;
                                }

                                selComps[j++] = comp;
                            }
                        }
                        SelectionService.SetSelectedComponents(selComps, SelectionTypes.Replace);
                    }
                }
            }
            finally
            {
                Cursor.Current = oldCursor;
            }
        }

        /// <summary>
        ///  Called when the show grid menu item is selected.
        /// </summary>
        protected void OnMenuShowGrid(object sender, EventArgs e)
        {
            if (site != null)
            {
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                if (host != null)
                {
                    DesignerTransaction trans = null;

                    try
                    {
                        trans = host.CreateTransaction();
                        IComponent baseComponent = host.RootComponent;
                        if (baseComponent != null && baseComponent is Control)
                        {
                            PropertyDescriptor prop = GetProperty(baseComponent, "DrawGrid");
                            if (prop != null)
                            {
                                bool drawGrid = (bool)prop.GetValue(baseComponent);
                                prop.SetValue(baseComponent, !drawGrid);
                                ((MenuCommand)sender).Checked = !drawGrid;
                            }
                        }
                    }
                    finally
                    {
                        if (trans != null)
                        {
                            trans.Commit();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Handles the various size to commands.
        /// </summary>
        protected void OnMenuSizingCommand(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            CommandID cmdID = cmd.CommandID;
            if (SelectionService == null)
            {
                return;
            }

            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ICollection sel = SelectionService.GetSelectedComponents();
                object[] selectedObjects = new object[sel.Count];
                sel.CopyTo(selectedObjects, 0);
                selectedObjects = FilterSelection(selectedObjects, SelectionRules.Visible);
                object selPrimary = SelectionService.PrimarySelection;
                Size primarySize = Size.Empty;
                Size itemSize = Size.Empty;
                PropertyDescriptor sizeProp;
                if (selPrimary is IComponent component)
                {
                    sizeProp = GetProperty(component, "Size");
                    if (sizeProp == null)
                    {
                        //if we couldn't get a valid size for our primary selection, we'll fail silently
                        return;
                    }
                    primarySize = (Size)sizeProp.GetValue(component);
                }
                if (selPrimary == null)
                {
                    return;
                }

                Debug.Assert(null != selectedObjects, "queryStatus should have disabled this");
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                DesignerTransaction trans = null;
                try
                {
                    if (host != null)
                    {
                        trans = host.CreateTransaction(string.Format(SR.CommandSetSize, selectedObjects.Length));
                    }

                    foreach (object obj in selectedObjects)
                    {
                        if (obj.Equals(selPrimary))
                        {
                            continue;
                        }

                        if (!(obj is IComponent comp))
                        {
                            continue;
                        }
                        //if the component is locked, no sizing is allowed...
                        PropertyDescriptor lockedDesc = GetProperty(obj, "Locked");
                        if (lockedDesc != null && (bool)lockedDesc.GetValue(obj))
                        {
                            continue;
                        }
                        sizeProp = GetProperty(comp, "Size");
                        // Skip all components that don't have a size property
                        if (sizeProp == null || sizeProp.IsReadOnly)
                        {
                            continue;
                        }
                        itemSize = (Size)sizeProp.GetValue(comp);
                        if (cmdID == MenuCommands.SizeToControlHeight || cmdID == MenuCommands.SizeToControl)
                        {
                            itemSize.Height = primarySize.Height;
                        }

                        if (cmdID == MenuCommands.SizeToControlWidth || cmdID == MenuCommands.SizeToControl)
                        {
                            itemSize.Width = primarySize.Width;
                        }
                        sizeProp.SetValue(comp, itemSize);
                    }
                }
                finally
                {
                    if (trans != null)
                    {
                        trans.Commit();
                    }
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
        protected void OnMenuSizeToGrid(object sender, EventArgs e)
        {
            if (SelectionService == null)
            {
                return;
            }

            Cursor oldCursor = Cursor.Current;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
            DesignerTransaction trans = null;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ICollection sel = SelectionService.GetSelectedComponents();
                object[] selectedObjects = new object[sel.Count];
                sel.CopyTo(selectedObjects, 0);
                selectedObjects = FilterSelection(selectedObjects, SelectionRules.Visible);
                Size size = Size.Empty;
                Point loc = Point.Empty;
                Debug.Assert(null != selectedObjects, "queryStatus should have disabled this");
                Size grid = Size.Empty;
                PropertyDescriptor sizeProp = null;
                PropertyDescriptor locProp = null;
                if (host != null)
                {
                    trans = host.CreateTransaction(string.Format(SR.CommandSetSizeToGrid, selectedObjects.Length));
                    IComponent baseComponent = host.RootComponent;
                    if (baseComponent != null && baseComponent is Control)
                    {
                        PropertyDescriptor prop = GetProperty(baseComponent, "CurrentGridSize");
                        if (prop != null)
                        {
                            grid = (Size)prop.GetValue(baseComponent);
                        }
                    }
                }

                if (!grid.IsEmpty)
                {
                    foreach (object obj in selectedObjects)
                    {
                        IComponent comp = obj as IComponent;
                        if (obj == null)
                        {
                            continue;
                        }

                        sizeProp = GetProperty(comp, "Size");
                        locProp = GetProperty(comp, "Location");
                        Debug.Assert(sizeProp != null, "No size property on component");
                        Debug.Assert(locProp != null, "No location property on component");
                        if (sizeProp == null || locProp == null || sizeProp.IsReadOnly || locProp.IsReadOnly)
                        {
                            continue;
                        }

                        size = (Size)sizeProp.GetValue(comp);
                        loc = (Point)locProp.GetValue(comp);
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
                if (trans != null)
                {
                    trans.Commit();
                }
                Cursor.Current = oldCursor;
            }
        }

        /// <summary>
        ///  Called when the properties menu item is selected on the Context menu
        /// </summary>
        protected void OnMenuDesignerProperties(object sender, EventArgs e)
        {
            // first, look if the currently selected object has a component editor...
            object obj = SelectionService.PrimarySelection;
            if (CheckComponentEditor(obj, true))
            {
                return;
            }

            IMenuCommandService menuSvc = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            if (menuSvc != null)
            {
                if (menuSvc.GlobalInvoke(MenuCommands.PropertiesWindow))
                {
                    return;
                }
            }
            Debug.Assert(false, "Invoking pbrs command failed");
        }

        /// <summary>
        ///  Called when the snap to grid menu item is selected.
        /// </summary>
        protected void OnMenuSnapToGrid(object sender, EventArgs e)
        {
            if (site != null)
            {
                IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    DesignerTransaction trans = null;
                    try
                    {
                        trans = host.CreateTransaction(string.Format(SR.CommandSetPaste, 0));
                        IComponent baseComponent = host.RootComponent;
                        if (baseComponent != null && baseComponent is Control)
                        {
                            PropertyDescriptor prop = GetProperty(baseComponent, "SnapToGrid");
                            if (prop != null)
                            {
                                bool snapToGrid = (bool)prop.GetValue(baseComponent);
                                prop.SetValue(baseComponent, !snapToGrid);
                                ((MenuCommand)sender).Checked = !snapToGrid;
                            }
                        }
                    }
                    finally
                    {
                        if (trans != null)
                        {
                            trans.Commit();
                        }
                    }
                }
            }

        }

        /// <summary>
        ///  Called when a spacing command is selected
        /// </summary>
        protected void OnMenuSpacingCommand(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            CommandID cmdID = cmd.CommandID;
            DesignerTransaction trans = null;
            if (SelectionService == null)
            {
                return;
            }

            Cursor oldCursor = Cursor.Current;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                // Inform the designer that we are about to monkey with a ton of properties.
                Size grid = Size.Empty;
                ICollection sel = SelectionService.GetSelectedComponents();
                object[] selectedObjects = new object[sel.Count];
                sel.CopyTo(selectedObjects, 0);
                if (host != null)
                {
                    trans = host.CreateTransaction(string.Format(SR.CommandSetFormatSpacing, selectedObjects.Length));
                    IComponent baseComponent = host.RootComponent;
                    if (baseComponent != null && baseComponent is Control)
                    {
                        PropertyDescriptor prop = GetProperty(baseComponent, "CurrentGridSize");
                        if (prop != null)
                        {
                            grid = (Size)prop.GetValue(baseComponent);
                        }
                    }
                }

                selectedObjects = FilterSelection(selectedObjects, SelectionRules.Visible);
                int nEqualDelta = 0;
                Debug.Assert(null != selectedObjects, "queryStatus should have disabled this");
                PropertyDescriptor curSizeDesc = null, lastSizeDesc = null;
                PropertyDescriptor curLocDesc = null, lastLocDesc = null;
                Size curSize = Size.Empty, lastSize = Size.Empty;
                Point curLoc = Point.Empty, lastLoc = Point.Empty;
                Point primaryLoc = Point.Empty;
                IComponent curComp = null, lastComp = null;
                int sort = -1;
                // Must sort differently if we're horizontal or vertical...
                if (cmdID == MenuCommands.HorizSpaceConcatenate || cmdID == MenuCommands.HorizSpaceDecrease || cmdID == MenuCommands.HorizSpaceIncrease || cmdID == MenuCommands.HorizSpaceMakeEqual)
                {
                    sort = SORT_HORIZONTAL;
                }
                else if (cmdID == MenuCommands.VertSpaceConcatenate || cmdID == MenuCommands.VertSpaceDecrease || cmdID == MenuCommands.VertSpaceIncrease || cmdID == MenuCommands.VertSpaceMakeEqual)
                {
                    sort = SORT_VERTICAL;
                }
                else
                {
                    throw new ArgumentException(SR.CommandSetUnknownSpacingCommand);
                }

                SortSelection(selectedObjects, sort);
                //now that we're sorted, lets get our primary selection and it's index
                object primary = SelectionService.PrimarySelection;
                int primaryIndex = 0;
                if (primary != null)
                {
                    primaryIndex = Array.IndexOf(selectedObjects, primary);
                }

                // And compute delta values for Make Equal
                if (cmdID == MenuCommands.HorizSpaceMakeEqual ||
                    cmdID == MenuCommands.VertSpaceMakeEqual)
                {
                    int total, n;
                    total = 0;
                    for (n = 0; n < selectedObjects.Length; n++)
                    {
                        curSize = Size.Empty;
                        if (selectedObjects[n] is IComponent component)
                        {
                            curComp = component;
                            curSizeDesc = GetProperty(curComp, "Size");
                            if (curSizeDesc != null)
                            {
                                curSize = (Size)curSizeDesc.GetValue(curComp);
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

                    lastComp = curComp = null;
                    curSize = Size.Empty;
                    curLoc = Point.Empty;
                    for (n = 0; n < selectedObjects.Length; n++)
                    {
                        curComp = selectedObjects[n] as IComponent;
                        if (curComp != null)
                        {
                            // only get the descriptors if we've changed component types
                            if (lastComp == null || curComp.GetType() != lastComp.GetType())
                            {
                                curSizeDesc = GetProperty(curComp, "Size");
                                curLocDesc = GetProperty(curComp, "Location");
                            }
                            lastComp = curComp;

                            if (curLocDesc != null)
                            {
                                curLoc = (Point)curLocDesc.GetValue(curComp);
                            }
                            else
                            {
                                continue;
                            }

                            if (curSizeDesc != null)
                            {
                                curSize = (Size)curSizeDesc.GetValue(curComp);
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

                    for (n = selectedObjects.Length - 1; n >= 0; n--)
                    {
                        curComp = selectedObjects[n] as IComponent;
                        if (curComp != null)
                        {
                            // only get the descriptors if we've changed component types
                            if (lastComp == null || curComp.GetType() != lastComp.GetType())
                            {
                                curSizeDesc = GetProperty(curComp, "Size");
                                curLocDesc = GetProperty(curComp, "Location");
                            }
                            lastComp = curComp;

                            if (curLocDesc != null)
                            {
                                lastLoc = (Point)curLocDesc.GetValue(curComp);
                            }
                            else
                            {
                                continue;
                            }

                            if (curSizeDesc != null)
                            {
                                lastSize = (Size)curSizeDesc.GetValue(curComp);
                            }
                            else
                            {
                                continue;
                            }

                            if (curSizeDesc != null && curLocDesc != null)
                            {
                                break;
                            }
                        }
                    }

                    if (curSizeDesc != null && curLocDesc != null)
                    {
                        if (sort == SORT_HORIZONTAL)
                        {
                            nEqualDelta = (lastSize.Width + lastLoc.X - curLoc.X - total) / (selectedObjects.Length - 1);
                        }
                        else
                        {
                            nEqualDelta = (lastSize.Height + lastLoc.Y - curLoc.Y - total) / (selectedObjects.Length - 1);
                        }
                        if (nEqualDelta < 0)
                        {
                            nEqualDelta = 0;
                        }
                    }
                }
                curComp = lastComp = null;
                if (primary != null)
                {
                    PropertyDescriptor primaryLocDesc = GetProperty(primary, "Location");
                    if (primaryLocDesc != null)
                    {
                        primaryLoc = (Point)primaryLocDesc.GetValue(primary);
                    }
                }

                // Finally move the components
                for (int n = 0; n < selectedObjects.Length; n++)
                {
                    curComp = (IComponent)selectedObjects[n];
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(curComp);
                    //Check to see if the component we are about to move is locked...
                    PropertyDescriptor lockedDesc = props["Locked"];
                    if (lockedDesc != null && (bool)lockedDesc.GetValue(curComp))
                    {
                        continue; // locked property of our component is true, so don't move it
                    }

                    if (lastComp == null || lastComp.GetType() != curComp.GetType())
                    {
                        curSizeDesc = props["Size"];
                        curLocDesc = props["Location"];
                    }
                    else
                    {
                        curSizeDesc = lastSizeDesc;
                        curLocDesc = lastLocDesc;
                    }

                    if (curLocDesc != null)
                    {
                        curLoc = (Point)curLocDesc.GetValue(curComp);
                    }
                    else
                    {
                        continue;
                    }

                    if (curSizeDesc != null)
                    {
                        curSize = (Size)curSizeDesc.GetValue(curComp);
                    }
                    else
                    {
                        continue;
                    }

                    int lastIndex = Math.Max(0, n - 1);
                    lastComp = (IComponent)selectedObjects[lastIndex];
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

                    if (lastLocDesc != null)
                    {
                        lastLoc = (Point)lastLocDesc.GetValue(lastComp);
                    }
                    else
                    {
                        continue;
                    }

                    if (lastSizeDesc != null)
                    {
                        lastSize = (Size)lastSizeDesc.GetValue(lastComp);
                    }
                    else
                    {
                        continue;
                    }

                    if (cmdID == MenuCommands.HorizSpaceConcatenate && n > 0)
                    {
                        curLoc.X = lastLoc.X + lastSize.Width;
                    }
                    else if (cmdID == MenuCommands.HorizSpaceDecrease)
                    {
                        if (primaryIndex < n)
                        {
                            curLoc.X -= grid.Width * (n - primaryIndex);
                            if (curLoc.X < primaryLoc.X)
                            {
                                curLoc.X = primaryLoc.X;
                            }
                        }
                        else if (primaryIndex > n)
                        {
                            curLoc.X += grid.Width * (primaryIndex - n);
                            if (curLoc.X > primaryLoc.X)
                            {
                                curLoc.X = primaryLoc.X;
                            }
                        }
                    }
                    else if (cmdID == MenuCommands.HorizSpaceIncrease)
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
                    else if (cmdID == MenuCommands.HorizSpaceMakeEqual && n > 0)
                    {
                        curLoc.X = lastLoc.X + lastSize.Width + nEqualDelta;
                    }
                    else if (cmdID == MenuCommands.VertSpaceConcatenate && n > 0)
                    {
                        curLoc.Y = lastLoc.Y + lastSize.Height;
                    }
                    else if (cmdID == MenuCommands.VertSpaceDecrease)
                    {
                        if (primaryIndex < n)
                        {
                            curLoc.Y -= grid.Height * (n - primaryIndex);
                            if (curLoc.Y < primaryLoc.Y)
                            {
                                curLoc.Y = primaryLoc.Y;
                            }
                        }
                        else if (primaryIndex > n)
                        {
                            curLoc.Y += grid.Height * (primaryIndex - n);
                            if (curLoc.Y > primaryLoc.Y)
                            {
                                curLoc.Y = primaryLoc.Y;
                            }
                        }
                    }
                    else if (cmdID == MenuCommands.VertSpaceIncrease)
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
                    else if (cmdID == MenuCommands.VertSpaceMakeEqual && n > 0)
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
                if (trans != null)
                {
                    trans.Commit();
                }
                Cursor.Current = oldCursor;
            }
        }

        /// <summary>
        ///  Called when the current selection changes.  Here we determine what commands can and can't be enabled.
        /// </summary>
        protected void OnSelectionChanged(object sender, EventArgs e)
        {
            if (SelectionService == null)
            {
                return;
            }
            _selectionVersion++;
            // Update our cached selection counts.
            selCount = SelectionService.SelectionCount;
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            Debug.Assert(designerHost != null, "Failed to get designer host");
            // if the base component is selected, we'll say that nothing's selected so we don't get wierd behavior
            if (selCount > 0 && designerHost != null)
            {
                object baseComponent = designerHost.RootComponent;
                if (baseComponent != null && SelectionService.GetComponentSelected(baseComponent))
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
                    if (!(obj is Control))
                    {
                        controlsOnlySelection = false;
                    }

                    if (!TypeDescriptor.GetAttributes(obj)[typeof(InheritanceAttribute)].Equals(InheritanceAttribute.NotInherited))
                    {
                        _selectionInherited = true;
                        break;
                    }
                }
            }
            OnUpdateCommandStatus();
        }

        /// <summary>
        ///  When this timer expires, this tells us that we need to erase any snaplines we have drawn.  First, we need to marshal this back to the correct thread.
        /// </summary>
        private void OnSnapLineTimerExpire(object sender, EventArgs e)
        {
            Control marshalControl = BehaviorService.AdornerWindowControl;
            if (marshalControl != null && marshalControl.IsHandleCreated)
            {
                marshalControl.BeginInvoke(new EventHandler(OnSnapLineTimerExpireMarshalled), new object[] { sender, e });
            }
        }

        /// <summary>
        ///  Called when our snapline timer expires - this method has been call has been properly marshalled back to the correct thread.
        /// </summary>
        private void OnSnapLineTimerExpireMarshalled(object sender, EventArgs e)
        {
            _snapLineTimer.Stop();
            EndDragManager();
        }

        /// <summary>
        ///  Determines the status of a menu command.  Commands with this event handler are always enabled.
        /// </summary>
        protected void OnStatusAlways(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            cmd.Enabled = true;
        }

        /// <summary>
        ///  Determines the status of a menu command.  Commands with this event handler are enabled when one or more objects are selected.
        /// </summary>
        protected void OnStatusAnySelection(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            cmd.Enabled = selCount > 0;
        }

        /// <summary>
        ///  Status for the copy command.  This is enabled when there is something juicy selected.
        /// </summary>
        protected void OnStatusCopy(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            bool enable = false;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (!_selectionInherited && host != null && !host.Loading)
            {
                ISelectionService selSvc = (ISelectionService)GetService(typeof(ISelectionService));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || selSvc != null, "ISelectionService not found");
                if (selSvc != null)
                {
                    // There must also be a component in the mix, and not the base component
                    ICollection selectedComponents = selSvc.GetSelectedComponents();
                    object baseComp = host.RootComponent;
                    if (!selSvc.GetComponentSelected(baseComp))
                    {
                        foreach (object obj in selectedComponents)
                        {
                            // if the object is not sited to the same thing as the host container then don't allow copy.
                            if (obj is IComponent comp && comp.Site != null && comp.Site.Container == host.Container)
                            {
                                enable = true;
                                break;
                            }
                        }
                    }
                }
            }
            cmd.Enabled = enable;
        }

        /// <summary>
        ///  Status for the cut command.  This is enabled when there is something juicy selected and that something does not contain any inherited components.
        /// </summary>
        protected void OnStatusCut(object sender, EventArgs e)
        {
            OnStatusDelete(sender, e);
            if (((MenuCommand)sender).Enabled)
            {
                OnStatusCopy(sender, e);
            }
        }

        /// <summary>
        ///  Status for the delete command. This is enabled when there is something selected and that something does not contain inherited components.
        /// </summary>
        protected void OnStatusDelete(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            if (_selectionInherited)
            {
                cmd.Enabled = false;
            }
            else
            {
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    ISelectionService selSvc = (ISelectionService)GetService(typeof(ISelectionService));
                    if (selSvc != null)
                    {
                        ICollection selectedComponents = selSvc.GetSelectedComponents();
                        foreach (object obj in selectedComponents)
                        {
                            // if the object is not sited to the same thing as the host container then don't allow delete. VSWhidbey# 275790
                            if (obj is IComponent comp && (comp.Site == null || (comp.Site != null && comp.Site.Container != host.Container)))
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
        ///  Determines the status of a menu command.  Commands with this event are enabled when there is something yummy on the clipboard.
        /// </summary>
        protected void OnStatusPaste(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            // Before we even look at the data format, check to see if the thing we're going to paste into is privately inherited.  If it is, then we definitely cannot paste.
            if (primarySelection != null)
            {
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");
                if (host != null && host.GetDesigner(primarySelection) is ParentControlDesigner)
                {
                    // This component is a target for our paste operation.  We must ensure that it is not privately inherited.
                    InheritanceAttribute attr = (InheritanceAttribute)TypeDescriptor.GetAttributes(primarySelection)[typeof(InheritanceAttribute)];
                    Debug.Assert(attr != null, "Type descriptor gave us a null attribute -- problem in type descriptor");
                    if (attr.InheritanceLevel == InheritanceLevel.InheritedReadOnly)
                    {
                        cmd.Enabled = false;
                        return;
                    }
                }
            }

            // Not being inherited.  Now look at the contents of the data
            IDataObject dataObj = Clipboard.GetDataObject();
            bool enable = false;
            if (dataObj != null)
            {
                if (dataObj.GetDataPresent(CF_DESIGNER))
                {
                    enable = true;
                }
                else
                {
                    // Not ours, check to see if the toolbox service understands this
                    IToolboxService ts = (IToolboxService)GetService(typeof(IToolboxService));
                    if (ts != null)
                    {
                        enable = (host != null ? ts.IsSupported(dataObj, host) : ts.IsToolboxItem(dataObj));
                    }
                }
            }
            cmd.Enabled = enable;
        }

        private void OnStatusPrimarySelection(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            cmd.Enabled = primarySelection != null;
        }

        protected virtual void OnStatusSelectAll(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            cmd.Enabled = host.Container.Components.Count > 1;
        }

        /// <summary>
        ///  This is called when the selection has changed.  Anyone using CommandSetItems that need to update their status based on selection changes should override this and update their own commands at this time.  The base implementaion runs through all base commands and calls UpdateStatus on them.
        /// </summary>
        protected virtual void OnUpdateCommandStatus()
        {
            // Now whip through all of the commands and ask them to update.
            for (int i = 0; i < _commandSet.Length; i++)
            {
                _commandSet[i].UpdateStatus();
            }
        }

        /// <summary>
        ///  This method grows the objects collection by one.  It prepends the collection with a string[] which contains the component names in order for each component in the list.
        /// </summary>
        private ICollection PrependComponentNames(ICollection objects)
        {
            object[] newObjects = new object[objects.Count + 1];
            int idx = 1;
            ArrayList names = new ArrayList(objects.Count);
            foreach (object o in objects)
            {
                if (o is IComponent comp)
                {
                    string name = null;
                    if (comp.Site != null)
                    {
                        name = comp.Site.Name;
                    }
                    names.Add(name);
                }
                newObjects[idx++] = o;
            }
            string[] nameArray = new string[names.Count];
            names.CopyTo(nameArray, 0);
            newObjects[0] = nameArray;
            return newObjects;
        }

        /// <summary>
        ///  called by the formatting commands when we need a given selection array sorted. Sorting the array sorts by x from left to right, and by Y from top to bottom.
        /// </summary>
        private void SortSelection(object[] selectedObjects, int nSortBy)
        {
            IComparer comp;
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

        /// <summary>
        ///  Common function that updates the status of clipboard menu items only
        /// </summary>
        private void UpdateClipboardItems(object s, EventArgs e)
        {
            int itemCount = 0;
            CommandSetItem curItem;
            for (int i = 0; itemCount < 3 && i < _commandSet.Length; i++)
            {
                curItem = _commandSet[i];
                if (curItem.CommandID == MenuCommands.Paste ||
                    curItem.CommandID == MenuCommands.Copy ||
                    curItem.CommandID == MenuCommands.Cut)
                {
                    itemCount++;
                    curItem.UpdateStatus();
                }
            }
        }

        private void UpdatePastePositions(ArrayList controls)
        {
            if (controls.Count == 0)
            {
                return;
            }

            // Find the offset to apply to these controls.  The offset is the location needed to center the controls in the parent. If there is no parent, we relocate to 0, 0.
            Control parentControl = ((Control)controls[0]).Parent;
            Point min = ((Control)controls[0]).Location;
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
            // We have the bounding rect for the controls.  Next, offset this rect so that we center it in the parent. If we have no parent, the offset will position the control at 0, 0, to whatever parent we eventually get.
            Point offset = new Point(-min.X, -min.Y);
            // Look to ensure that we're not going to paste this control over the top of another control.  We only do this for the first control because preserving the relationship between controls is more important than obscuring a control.
            if (parentControl != null)
            {
                bool bumpIt;
                bool wrapped = false;
                Size parentSize = parentControl.ClientSize;
                Size gridSize = Size.Empty;
                Point parentOffset = new Point(parentSize.Width / 2, parentSize.Height / 2);
                parentOffset.X -= (max.X - min.X) / 2;
                parentOffset.Y -= (max.Y - min.Y) / 2;
                do
                {
                    bumpIt = false;
                    // Cycle through the controls on the parent.  We're interested in controls that (a) are not in our set of controls and (b) have a location == to our current bumpOffset OR (c) are the same size as our parent.  If we find such a control, we increment the bump offset by one grid size.
                    foreach (Control child in parentControl.Controls)
                    {
                        Rectangle childBounds = child.Bounds;
                        if (controls.Contains(child))
                        {
                            // We still want to bump if the child is the same size as the parent. Otherwise the child would overlay exactly on top of the parent.
                            if (!child.Size.Equals(parentSize))
                            {
                                continue;
                            }

                            // We're dealing with our own pasted control, so offset its bounds. We don't use parent offset here because, well, we're comparing against the parent!
                            childBounds.Offset(offset);
                        }

                        // We need only compare against one of our pasted controls, so pick the first one.
                        Control pasteControl = (Control)controls[0];
                        Rectangle pasteControlBounds = pasteControl.Bounds;
                        pasteControlBounds.Offset(offset);
                        pasteControlBounds.Offset(parentOffset);
                        if (pasteControlBounds.Equals(childBounds))
                        {
                            bumpIt = true;
                            if (gridSize.IsEmpty)
                            {
                                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                                IComponent baseComponent = host.RootComponent;
                                if (baseComponent != null && baseComponent is Control)
                                {
                                    PropertyDescriptor gs = GetProperty(baseComponent, "GridSize");
                                    if (gs != null)
                                    {
                                        gridSize = (Size)gs.GetValue(baseComponent);
                                    }
                                }
                                if (gridSize.IsEmpty)
                                {
                                    gridSize.Width = 8;
                                    gridSize.Height = 8;
                                }
                            }

                            parentOffset += gridSize;
                            // Extra check:  If the end of our control group is > the parent size, bump back to zero.  We still allow further bumps after this so we can continue to offset, but if we cycle again then we quit so we won't loop indefinitely. We only do this if we're a group.  If we're a single control we use the beginning of the control + a grid size.
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
                } while (bumpIt);
                offset.Offset(parentOffset.X, parentOffset.Y);
            }

            // Now, for each control, update the offset.
            if (parentControl != null)
            {
                parentControl.SuspendLayout();
            }
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
                if (parentControl != null)
                {
                    parentControl.ResumeLayout();
                }
            }
        }

        private void UpdatePasteTabIndex(Control componentControl, object parentComponent)
        {
            if (!(parentComponent is Control parentControl) || componentControl == null)
            {
                return;
            }
            bool tabIndexCollision = false;
            int tabIndexOriginal = componentControl.TabIndex;
            // Find the next highest tab index
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
        ///  We extend MenuCommand for our command set items.  A command set item is a menu command with an added delegate that is used to determine the flags for the menu item.  We have different classes of delegates here. For example, many  menu items may be enabled when there is at least one object selected, while others are only enabled if there is more than one object or if there is a primary selection.
        /// </summary>
        protected class CommandSetItem : MenuCommand
        {
            private readonly EventHandler _statusHandler;
            private readonly IEventHandlerService _eventService;
            private readonly IUIService _uiService;
            private readonly CommandSet _commandSet;
            private static Hashtable s_commandStatusHash; // list of the command statuses we are tracking.
            private bool _updatingCommand = false; // flag we set when we're updating the command so we don't call back on the status handler.

            public CommandSetItem(CommandSet commandSet, EventHandler statusHandler, EventHandler invokeHandler, CommandID id, IUIService uiService) : this(commandSet, statusHandler, invokeHandler, id, false, uiService)
            {
            }

            public CommandSetItem(CommandSet commandSet, EventHandler statusHandler, EventHandler invokeHandler, CommandID id) : this(commandSet, statusHandler, invokeHandler, id, false, null)
            {
            }

            public CommandSetItem(CommandSet commandSet, EventHandler statusHandler, EventHandler invokeHandler, CommandID id, bool optimizeStatus) : this(commandSet, statusHandler, invokeHandler, id, optimizeStatus, null)
            {
            }

            /// <summary>
            ///  Creates a new CommandSetItem.
            /// </summary>
            public CommandSetItem(CommandSet commandSet, EventHandler statusHandler, EventHandler invokeHandler, CommandID id, bool optimizeStatus, IUIService uiService)
            : base(invokeHandler, id)
            {
                _uiService = uiService;
                _eventService = commandSet._eventService;
                _statusHandler = statusHandler;
                // when we optimize, it's because status is fully based on selection. so what we do is only call the status handler once per selection change to prevent doing the same work over and over again.  we do this by hashing up the command statuses and then filling in the results we get, so we can easily retrieve them when the selection hasn't changed.
                if (optimizeStatus && statusHandler != null)
                {
                    // we use this as our sentinel of when we're doing this.
                    _commandSet = commandSet;
                    // create the hash if needed.
                    lock (typeof(CommandSetItem))
                    {
                        if (s_commandStatusHash == null)
                        {
                            s_commandStatusHash = new Hashtable();
                        }
                    }

                    // UNDONE:CommandSetItem is put in a static hashtable, and CommandSetItem  references CommandSet, CommandSet reference FormDesigner. If we don't  remove the CommandSetItem from the static hashtable, FormDesigner is  leaked. This demonstrates a bad design. We should not keep a static  hashtable for all the items, instead, we should keep a hashtable per  Designer. When designer is disposed, all command items got disposed  automatically. However, at this time, we would pick a simple way with  low risks to fix this.
                    // if this handler isn't already in there, add it.
                    if (!(s_commandStatusHash[statusHandler] is StatusState state))
                    {
                        state = new StatusState();
                        s_commandStatusHash.Add(statusHandler, state);
                    }
                    state.refCount++;
                }
            }

            /// <summary>
            ///  Checks if the status for this command is valid, meaning we don't need to call the status handler.
            /// </summary>
            private bool CommandStatusValid
            {
                get
                {
                    // check to see if this is a command we have hashed up and if it's version stamp is the same as our current selection version.
                    if (_commandSet != null && s_commandStatusHash.Contains(_statusHandler))
                    {
                        if (s_commandStatusHash[_statusHandler] is StatusState state && state.SelectionVersion == _commandSet.SelectionVersion)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            /// <summary>
            ///  Applys the cached status to this item.
            /// </devdco>
            private void ApplyCachedStatus()
            {
                if (_commandSet != null && s_commandStatusHash.Contains(_statusHandler))
                {
                    try
                    {
                        // set our our updating flag so it doesn't call the status handler again.
                        _updatingCommand = true;
                        // and push the state into this command.
                        StatusState state = s_commandStatusHash[_statusHandler] as StatusState;
                        state.ApplyState(this);
                    }
                    finally
                    {
                        _updatingCommand = false;
                    }
                }
            }

            /// <summary>
            ///  This may be called to invoke the menu item.
            /// </summary>
            public override void Invoke()
            {
                // We allow outside parties to override the availability of particular menu commands.
                try
                {
                    if (_eventService != null)
                    {
                        IMenuStatusHandler msh = (IMenuStatusHandler)_eventService.GetHandler(typeof(IMenuStatusHandler));
                        if (msh != null && msh.OverrideInvoke(this))
                        {
                            return;
                        }
                    }
                    base.Invoke();
                }
                catch (Exception e)
                {
                    if (_uiService != null)
                    {
                        _uiService.ShowError(e, string.Format(SR.CommandSetError, e.Message));
                    }
                    if (ClientUtils.IsCriticalException(e))
                    {
                        throw;
                    }
                }
            }

            ///<summary>
            ///  Only pass this down to the base when we're not doing the cached update.
            ///</summary>
            protected override void OnCommandChanged(EventArgs e)
            {
                if (!_updatingCommand)
                {
                    base.OnCommandChanged(e);
                }
            }

            ///<summary>
            ///  Saves the status for this command to the statusstate that's stored in the hashtable based on our status handler delegate.
            ///</summary>
            private void SaveCommandStatus()
            {
                if (_commandSet != null)
                {
                    StatusState state;
                    // see if we need to create one of these StatusState dudes.
                    if (s_commandStatusHash.Contains(_statusHandler))
                    {
                        state = s_commandStatusHash[_statusHandler] as StatusState;
                    }
                    else
                    {
                        state = new StatusState();
                    }
                    // and save the enabled, visible, checked, and supported state.
                    state.SaveState(this, _commandSet.SelectionVersion);
                }
            }

            /// <summary>
            ///  Called when the status of this command should be re-queried.
            /// </summary>
            public void UpdateStatus()
            {
                // We allow outside parties to override the availability of particular menu commands.
                if (_eventService != null)
                {
                    IMenuStatusHandler msh = (IMenuStatusHandler)_eventService.GetHandler(typeof(IMenuStatusHandler));
                    if (msh != null && msh.OverrideStatus(this))
                    {
                        return;
                    }
                }
                if (_statusHandler != null)
                {
                    // if we need to update our status, call the status handler.  otherwise, get the cached status and push it into this command.
                    if (!CommandStatusValid)
                    {
                        try
                        {
                            _statusHandler(this, EventArgs.Empty);
                            SaveCommandStatus();
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        ApplyCachedStatus();
                    }
                }
            }

            /// <summary>
            ///  Remove this command item from the static hashtable to avoid leaking this object.
            /// </summary>
            public virtual void Dispose()
            {
                if (s_commandStatusHash[_statusHandler] is StatusState state)
                {
                    state.refCount--;
                    if (state.refCount == 0)
                    {
                        s_commandStatusHash.Remove(_statusHandler);
                    }
                }
            }

            /// <summary>
            ///  This class saves the state for a given command.  It keeps track of the results of the last status handler invocation and what "selection version" that happened on.
            /// </summary>
            private class StatusState
            {
                // these are the command's possible values.
                private const int Enabled = 0x01;
                private const int Visible = 0x02;
                private const int Checked = 0x04;
                private const int Supported = 0x08;
                private const int NeedsUpdate = 0x10;
                private int _selectionVersion = 0; // the version of the selection that this was initialized with.
                private int _statusFlags = NeedsUpdate; // our flags.
                // Multiple CommandSetItem instances can share a same status handler within a designer host. We use a simple ref count to make sure the CommandSetItem can be properly removed.
                internal int refCount = 0;

                /// <summary>
                ///  Just what it says...
                /// </summary>
                public int SelectionVersion
                {
                    get => _selectionVersion;
                }

                /// <summary>
                ///  Pushes the state stored in this object into the given command item.
                /// </summary>
                internal void ApplyState(CommandSetItem item)
                {
                    Debug.Assert((_statusFlags & NeedsUpdate) != NeedsUpdate, "Updating item when StatusState is not valid.");
                    item.Enabled = ((_statusFlags & Enabled) == Enabled);
                    item.Visible = ((_statusFlags & Visible) == Visible);
                    item.Checked = ((_statusFlags & Checked) == Checked);
                    item.Supported = ((_statusFlags & Supported) == Supported);
                }

                /// <summary>
                ///  Updates this status object  with the state from the given item, and saves teh seletion version.
                /// </summary>
                internal void SaveState(CommandSetItem item, int version)
                {
                    _selectionVersion = version;
                    _statusFlags = 0;
                    if (item.Enabled)
                    {
                        _statusFlags |= Enabled;
                    }
                    if (item.Visible)
                    {
                        _statusFlags |= Visible;
                    }
                    if (item.Checked)
                    {
                        _statusFlags |= Checked;
                    }
                    if (item.Supported)
                    {
                        _statusFlags |= Supported;
                    }
                }
            }
        }

        /// <summary>
        ///  The immediate command set item is used for commands that cannot be cached.  Commands such as Paste that get outside stimulus cannot be cached by our menu system, so they get an ImmediateCommandSetItem instead of a CommandSetItem.
        /// </summary>
        protected class ImmediateCommandSetItem : CommandSetItem
        {
            /// <summary>
            ///  Creates a new ImmediateCommandSetItem.
            /// </summary>
            public ImmediateCommandSetItem(CommandSet commandSet, EventHandler statusHandler, EventHandler invokeHandler, CommandID id, IUIService uiService) : base(commandSet, statusHandler, invokeHandler, id, uiService)
            {
            }

            /// <summary>
            ///  Overrides OleStatus in MenuCommand to invoke our status handler first.
            /// </summary>
            public override int OleStatus
            {
                get
                {
                    UpdateStatus();
                    return base.OleStatus;
                }
            }
        }

        /// <summary>
        ///  Component comparer that compares the left property of a component.
        /// </summary>
        private class ComponentLeftCompare : IComparer
        {
            public int Compare(object p, object q)
            {
                PropertyDescriptor pProp = TypeDescriptor.GetProperties(p)["Location"];
                PropertyDescriptor qProp = TypeDescriptor.GetProperties(q)["Location"];
                Point pLoc = (Point)pProp.GetValue(p);
                Point qLoc = (Point)qProp.GetValue(q);
                //if our lefts are equal, then compare tops
                if (pLoc.X == qLoc.X)
                {
                    return pLoc.Y - qLoc.Y;
                }
                return pLoc.X - qLoc.X;
            }
        }

        /// <summary>
        ///  Component comparer that compares the top property of a component.
        /// </summary>
        private class ComponentTopCompare : IComparer
        {
            public int Compare(object p, object q)
            {
                PropertyDescriptor pProp = TypeDescriptor.GetProperties(p)["Location"];
                PropertyDescriptor qProp = TypeDescriptor.GetProperties(q)["Location"];
                Point pLoc = (Point)pProp.GetValue(p);
                Point qLoc = (Point)qProp.GetValue(q);
                //if our tops are equal, then compare lefts
                if (pLoc.Y == qLoc.Y)
                {
                    return pLoc.X - qLoc.X;
                }
                return pLoc.Y - qLoc.Y;
            }
        }

        private class ControlZOrderCompare : IComparer
        {
            public int Compare(object p, object q)
            {
                if (p == null)
                {
                    return -1;
                }
                else if (q == null)
                {
                    return 1;
                }
                else if (p == q)
                {
                    return 0;
                }
                if (!(p is Control c1) || !(q is Control c2))
                {
                    return 1;
                }

                if (c1.Parent == c2.Parent && c1.Parent != null)
                {
                    return c1.Parent.Controls.GetChildIndex(c1) - c1.Parent.Controls.GetChildIndex(c2);
                }
                return 1;
            }
        }

        private class TabIndexCompare : IComparer
        {
            public int Compare(object p, object q)
            {
                Control c1 = p as Control;
                Control c2 = q as Control;
                if (c1 == c2)
                {
                    return 0;
                }

                if (c1 == null)
                {
                    return -1;
                }

                if (c2 == null)
                {
                    return 1;
                }
                return c1.TabIndex - c2.TabIndex;
            }
        }
    }
}
