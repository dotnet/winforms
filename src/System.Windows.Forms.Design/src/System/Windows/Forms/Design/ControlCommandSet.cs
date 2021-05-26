// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This class implements menu commands that are specific to designers that
    ///  manipulate controls.
    /// </summary>
    internal class ControlCommandSet : CommandSet
    {
        private readonly CommandSetItem[] commandSet;
        private TabOrder tabOrder;
        private readonly Control baseControl;
        private StatusCommandUI statusCommandUI; //used to update the StatusBarInfo.

        /// <summary>
        ///  Creates a new CommandSet object.  This object implements the set
        ///  of commands that the UI.Win32 form designer offers.
        /// </summary>

        // Since we don't override GetService it is okay to suppress this.
        public ControlCommandSet(ISite site) : base(site)
        {
            statusCommandUI = new StatusCommandUI(site);

            // Establish our set of commands
            //
            commandSet = new CommandSetItem[]
            {
                // Alignment commands
                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuAlignByPrimary),
                                  StandardCommands.AlignLeft, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuAlignByPrimary),
                                  StandardCommands.AlignTop, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusControlsOnlySelectionAndGrid),
                                  new EventHandler(OnMenuAlignToGrid),
                                  StandardCommands.AlignToGrid, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuAlignByPrimary),
                                  StandardCommands.AlignBottom, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuAlignByPrimary),
                                  StandardCommands.AlignHorizontalCenters, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuAlignByPrimary),
                                  StandardCommands.AlignRight, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuAlignByPrimary),
                                  StandardCommands.AlignVerticalCenters, true),

                // Centering commands
                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusControlsOnlySelection),
                                  new EventHandler(OnMenuCenterSelection),
                                  StandardCommands.CenterHorizontally, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusControlsOnlySelection),
                                  new EventHandler(OnMenuCenterSelection),
                                  StandardCommands.CenterVertically, true),

                // Spacing commands
                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectNonContained),
                                  new EventHandler(OnMenuSpacingCommand),
                                  StandardCommands.HorizSpaceConcatenate, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectNonContained),
                                  new EventHandler(OnMenuSpacingCommand),
                                  StandardCommands.HorizSpaceDecrease, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectNonContained),
                                  new EventHandler(OnMenuSpacingCommand),
                                  StandardCommands.HorizSpaceIncrease, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectNonContained),
                                  new EventHandler(OnMenuSpacingCommand),
                                  StandardCommands.HorizSpaceMakeEqual, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectNonContained),
                                  new EventHandler(OnMenuSpacingCommand),
                                  StandardCommands.VertSpaceConcatenate, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectNonContained),
                                  new EventHandler(OnMenuSpacingCommand),
                                  StandardCommands.VertSpaceDecrease, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectNonContained),
                                  new EventHandler(OnMenuSpacingCommand),
                                  StandardCommands.VertSpaceIncrease, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectNonContained),
                                  new EventHandler(OnMenuSpacingCommand),
                                  StandardCommands.VertSpaceMakeEqual, true),

                // Sizing commands
                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuSizingCommand),
                                  StandardCommands.SizeToControl, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuSizingCommand),
                                  StandardCommands.SizeToControlWidth, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusMultiSelectPrimary),
                                  new EventHandler(OnMenuSizingCommand),
                                  StandardCommands.SizeToControlHeight, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusControlsOnlySelectionAndGrid),
                                  new EventHandler(OnMenuSizeToGrid),
                                  StandardCommands.SizeToGrid, true),

                // Z-Order commands
                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusZOrder),
                                  new EventHandler(OnMenuZOrderSelection),
                                  StandardCommands.BringToFront, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusZOrder),
                                  new EventHandler(OnMenuZOrderSelection),
                                  StandardCommands.SendToBack, true),

                // Miscellaneous commands
                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusShowGrid),
                                  new EventHandler(OnMenuShowGrid),
                                  StandardCommands.ShowGrid, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusSnapToGrid),
                                  new EventHandler(OnMenuSnapToGrid),
                                  StandardCommands.SnapToGrid, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAnyControls),
                                  new EventHandler(OnMenuTabOrder),
                                  StandardCommands.TabOrder, true),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusLockControls),
                                  new EventHandler(OnMenuLockControls),
                                  StandardCommands.LockControls, true),

                // Keyboard commands
                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySize),
                                  MenuCommands.KeySizeWidthIncrease),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySize),
                                  MenuCommands.KeySizeHeightIncrease),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySize),
                                  MenuCommands.KeySizeWidthDecrease),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySize),
                                  MenuCommands.KeySizeHeightDecrease),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySize),
                                  MenuCommands.KeyNudgeWidthIncrease),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySize),
                                  MenuCommands.KeyNudgeHeightIncrease),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySize),
                                  MenuCommands.KeyNudgeWidthDecrease),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySize),
                                  MenuCommands.KeyNudgeHeightDecrease),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySelect),
                                  MenuCommands.KeySelectNext),

                new CommandSetItem(
                                  this,
                                  new EventHandler(OnStatusAlways),
                                  new EventHandler(OnKeySelect),
                                  MenuCommands.KeySelectPrevious),
            };

            if (MenuService != null)
            {
                for (int i = 0; i < commandSet.Length; i++)
                {
                    MenuService.AddCommand(commandSet[i]);
                }
            }

            // Get the base control object.
            //
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null)
            {
                Control comp = host.RootComponent as Control;
                if (comp != null)
                {
                    baseControl = comp;
                }
            }
        }

        /// <summary>
        ///  Ensures there are no items in the selection that are children of another item in the selection
        ///
        /// </summary>
        private bool CheckSelectionParenting()
        {
            ICollection sel = SelectionService.GetSelectedComponents();

            Hashtable itemHash = new Hashtable(sel.Count);
            foreach (object obj in sel)
            {
                Control c = obj as Control;

                if (c == null || c.Site == null)
                {
                    return false;
                }

                itemHash.Add(obj, obj);
            }

            Control okParent = null;

            // just walk up the chain for each selected item...if any other items
            // are in that chain, fail.
            //
            foreach (object component in sel)
            {
                Control c = component as Control;

                if (c == null || c.Site == null)
                {
                    return false;
                }

                // walk up the parent chain, checking each component to see if it's
                // in the selection list.  If it is, we've got a bad selection.
                //
                for (Control parent = c.Parent; parent != null; parent = parent.Parent)
                {
                    // if this parent has already been okayed, skip it.
                    //
                    if (parent == okParent)
                    {
                        continue;
                    }

                    object hashItem = itemHash[parent];
                    if (hashItem != null && hashItem != component)
                    {
                        return false;
                    }
                }

                // mark that this compoent checked out okay, so any siblings (or children of siblings) of this control
                // are ok.
                //
                okParent = c.Parent;
            }

            return true;
        }

        /// <summary>
        ///  Disposes of this object, removing all commands from the menu service.
        /// </summary>

        // We don't need to Dispose baseControl
        public override void Dispose()
        {
            if (MenuService != null)
            {
                for (int i = 0; i < commandSet.Length; i++)
                {
                    MenuService.RemoveCommand(commandSet[i]);
                    commandSet[i].Dispose();
                }
            }

            if (tabOrder != null)
            {
                tabOrder.Dispose();
                tabOrder = null;
            }

            statusCommandUI = null;
            base.Dispose();
        }

        /// <summary>
        ///  Retrieves the snap information for the given component.
        /// </summary>
        protected override void GetSnapInformation(IDesignerHost host, IComponent component, out Size snapSize, out IComponent snapComponent, out PropertyDescriptor snapProperty)
        {
            IComponent currentSnapComponent = null;
            IContainer container = component.Site.Container;
            PropertyDescriptor gridSizeProp = null;
            PropertyDescriptor currentSnapProp = null;
            PropertyDescriptorCollection props;

            // This implementation is specific to controls.  It looks in the parent hierarchy for an object with a
            // snap property.  If it fails to find one, it just gets the base component.
            //
            Control ctrl = component as Control;
            if (ctrl != null)
            {
                Control c = ctrl.Parent;
                while (c != null && currentSnapComponent == null)
                {
                    props = TypeDescriptor.GetProperties(c);
                    currentSnapProp = props["SnapToGrid"];
                    if (currentSnapProp != null)
                    {
                        if (currentSnapProp.PropertyType == typeof(bool) && c.Site != null && c.Site.Container == container)
                        {
                            currentSnapComponent = c;
                        }
                        else
                        {
                            currentSnapProp = null;
                        }
                    }

                    c = c.Parent;
                }
            }

            if (currentSnapComponent == null)
            {
                currentSnapComponent = host.RootComponent;
            }

            props = TypeDescriptor.GetProperties(currentSnapComponent);

            if (currentSnapProp == null)
            {
                currentSnapProp = props["SnapToGrid"];
                if (currentSnapProp != null && currentSnapProp.PropertyType != typeof(bool))
                {
                    currentSnapProp = null;
                }
            }

            if (gridSizeProp == null)
            {
                gridSizeProp = props["GridSize"];
                if (gridSizeProp != null && gridSizeProp.PropertyType != typeof(Size))
                {
                    gridSizeProp = null;
                }
            }

            // Finally, now that we've got the various properties and components, dole out the
            // values.
            //
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
        ///  Called for the two cancel commands we support.
        /// </summary>
        protected override bool OnKeyCancel(object sender)
        {
            // The base implementation here just checks to see if we are dragging.
            // If we are, then we abort the drag.
            //
            if (!base.OnKeyCancel(sender))
            {
                MenuCommand cmd = (MenuCommand)sender;
                bool reverse = (cmd.CommandID.Equals(MenuCommands.KeyReverseCancel));
                RotateParentSelection(reverse);
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Builds up an array of snaplines used during resize to adjust/snap
        ///  the controls bounds.
        /// </summary>
        private ArrayList GenerateSnapLines(SelectionRules rules, Control primaryControl, int directionOffsetX, int directionOffsetY)
        {
            ArrayList lines = new ArrayList(2);

            Point pt = BehaviorService.ControlToAdornerWindow(primaryControl);
            bool fRTL = (primaryControl.Parent != null && primaryControl.Parent.IsMirrored);

            //remember that snaplines must be in adornerwindow coordinates

            if (directionOffsetX != 0)
            {
                Debug.Assert(directionOffsetY == 0, "Can only resize in one direction at a time using the keyboard");

                //we are resizing in the x-dir, so add the vertical snaplines for the right edge
                if (!fRTL)
                {
                    if ((rules & SelectionRules.RightSizeable) != 0)
                    {
                        lines.Add(new SnapLine(SnapLineType.Right, pt.X + primaryControl.Width - 1));
                        lines.Add(new SnapLine(SnapLineType.Vertical, pt.X + primaryControl.Width + primaryControl.Margin.Right, SnapLine.MarginRight, SnapLinePriority.Always));
                    }
                }
                else
                {
                    if ((rules & SelectionRules.LeftSizeable) != 0)
                    {
                        lines.Add(new SnapLine(SnapLineType.Left, pt.X));
                        lines.Add(new SnapLine(SnapLineType.Vertical, pt.X - primaryControl.Margin.Left, SnapLine.MarginLeft, SnapLinePriority.Always));
                    }
                }
            }

            if (directionOffsetY != 0)
            {
                Debug.Assert(directionOffsetX == 0, "Can only resize in one direction at a time using the keyboard");

                //we are resizing in the y-dir, so add the horizonal snaplines for the bottom edge
                if ((rules & SelectionRules.BottomSizeable) != 0)
                {
                    lines.Add(new SnapLine(SnapLineType.Bottom, pt.Y + primaryControl.Height - 1));
                    lines.Add(new SnapLine(SnapLineType.Horizontal, pt.Y + primaryControl.Height + primaryControl.Margin.Bottom, SnapLine.MarginBottom, SnapLinePriority.Always));
                }
            }

            return lines;
        }

        /// <summary>
        ///  Called for the various sizing commands we support.
        /// </summary>
        protected void OnKeySize(object sender, EventArgs e)
        {
            // Arrow keys.  Begin a drag if the selection isn't locked.
            //
            ISelectionService selSvc = SelectionService;
            if (selSvc != null)
            {
                IComponent comp = selSvc.PrimarySelection as IComponent;
                if (comp != null)
                {
                    IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    if (host != null)
                    {
                        //This will exluded components in the ComponentTray, but that's okay, they are not resizable to begin with.
                        ControlDesigner des = host.GetDesigner(comp) as ControlDesigner;
                        if (des != null && ((des.SelectionRules & SelectionRules.Locked) == 0))
                        {
                            //Possibly flip our size adjustments depending on the dock prop of the control.
                            //EX: If the control is docked right, then shift+left arrow will cause
                            //the control's width to decrease when it should increase
                            bool flipOffset = false;
                            PropertyDescriptor dockProp = TypeDescriptor.GetProperties(comp)["Dock"];
                            if (dockProp != null)
                            {
                                DockStyle docked = (DockStyle)dockProp.GetValue(comp);
                                flipOffset = (docked == DockStyle.Bottom) || (docked == DockStyle.Right);
                            }

                            SelectionRules rules = SelectionRules.Visible;
                            CommandID cmd = ((MenuCommand)sender).CommandID;
                            bool invertSnap = false;
                            int moveOffsetX = 0;
                            int moveOffsetY = 0;

                            bool checkForIntegralHeight = false;

                            if (cmd.Equals(MenuCommands.KeySizeHeightDecrease))
                            {
                                moveOffsetY = flipOffset ? 1 : -1;
                                rules |= SelectionRules.BottomSizeable;
                            }
                            else if (cmd.Equals(MenuCommands.KeySizeHeightIncrease))
                            {
                                moveOffsetY = flipOffset ? -1 : 1;
                                rules |= SelectionRules.BottomSizeable;
                            }
                            else if (cmd.Equals(MenuCommands.KeySizeWidthDecrease))
                            {
                                moveOffsetX = flipOffset ? 1 : -1;
                                rules |= SelectionRules.RightSizeable;
                            }
                            else if (cmd.Equals(MenuCommands.KeySizeWidthIncrease))
                            {
                                moveOffsetX = flipOffset ? -1 : 1;
                                rules |= SelectionRules.RightSizeable;
                            }
                            else if (cmd.Equals(MenuCommands.KeyNudgeHeightDecrease))
                            {
                                moveOffsetY = -1;
                                invertSnap = true;
                                rules |= SelectionRules.BottomSizeable;
                            }
                            else if (cmd.Equals(MenuCommands.KeyNudgeHeightIncrease))
                            {
                                moveOffsetY = 1;
                                invertSnap = true;
                                rules |= SelectionRules.BottomSizeable;
                            }
                            else if (cmd.Equals(MenuCommands.KeyNudgeWidthDecrease))
                            {
                                moveOffsetX = -1;
                                invertSnap = true;
                                rules |= SelectionRules.RightSizeable;
                            }
                            else if (cmd.Equals(MenuCommands.KeyNudgeWidthIncrease))
                            {
                                moveOffsetX = 1;
                                invertSnap = true;
                                rules |= SelectionRules.RightSizeable;
                            }
                            else
                            {
                                Debug.Fail("Unknown command mapped to OnKeySize: " + cmd.ToString());
                            }

                            DesignerTransaction trans;
                            if (selSvc.SelectionCount > 1)
                            {
                                trans = host.CreateTransaction(string.Format(SR.DragDropSizeComponents, selSvc.SelectionCount));
                            }
                            else
                            {
                                trans = host.CreateTransaction(string.Format(SR.DragDropSizeComponent, comp.Site.Name));
                            }

                            try
                            {
                                //if we can find the behaviorservice, then we can use it and the SnapLineEngine to help us
                                //move these controls...
                                if (BehaviorService != null)
                                {
                                    Control primaryControl = comp as Control;

                                    bool useSnapLines = BehaviorService.UseSnapLines;

                                    // If we have previous snaplines, we always want to erase them, no matter what. VS Whidbey #397709
                                    if (dragManager != null)
                                    {
                                        EndDragManager();
                                    }

                                    //If we CTRL+Arrow and we're using SnapLines - snap to the next location
                                    if (invertSnap && useSnapLines)
                                    {
                                        ArrayList selComps = new ArrayList(selSvc.GetSelectedComponents());

                                        //create our snapline engine
                                        dragManager = new DragAssistanceManager(des.Component.Site, selComps);

                                        ArrayList targetSnaplines = GenerateSnapLines(des.SelectionRules, primaryControl, moveOffsetX, moveOffsetY);

                                        //ask our snapline engine to find the nearest snap position with the given direction
                                        Point snappedOffset = dragManager.OffsetToNearestSnapLocation(primaryControl, targetSnaplines, new Point(moveOffsetX, moveOffsetY));

                                        //update the offset according to the snapline engine - but only if the new size is not smaller than the minimum control size
                                        //E.g. Say button 1 is above button 2 (in the y direction). Button 2 is selected.
                                        //If the user does a ctrl-shift-up arrow, then OffsetToNearestSnapLocation would return a match to the bottom snapline for button 1
                                        //resulting in button2's size to be negative

                                        Size primaryControlsize = primaryControl.Size;
                                        primaryControlsize += new Size(snappedOffset.X, snappedOffset.Y);
                                        if ((primaryControlsize.Width <= 0) || (primaryControlsize.Height <= 0))
                                        {
                                            // simulate that there is nothing to snap to
                                            moveOffsetX = 0;
                                            moveOffsetY = 0;
                                            EndDragManager();
                                        }
                                        else
                                        {
                                            // This is the offset assuming origin is in the upper-left.
                                            moveOffsetX = snappedOffset.X;
                                            moveOffsetY = snappedOffset.Y;
                                        }

                                        // If the parent is mirrored then we need to negate moveOffsetX.
                                        // This is because moveOffsetX assumes that the origin
                                        // is upper left. That is, when moveOffsetX is positive, we
                                        // are moving right, negative when moving left.

                                        // The parent container's origin depends on its mirroring property.
                                        // Thus when we call propSize.setValue below, we need to make sure
                                        // that our moveOffset.X correctly reflects the placement of the
                                        // parent container's origin.

                                        // We need to do this AFTER we calculate the snappedOffset.
                                        // This is because the dragManager calculations are all based
                                        // on an origin in the upper-left.
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
                                        IComponent snapComponent = null;
                                        PropertyDescriptor snapProperty = null;

                                        GetSnapInformation(host, comp, out snapSize, out snapComponent, out snapProperty);

                                        if (snapProperty != null)
                                        {
                                            snapOn = (bool)snapProperty.GetValue(snapComponent);
                                        }

                                        if (snapOn && !snapSize.IsEmpty)
                                        {
                                            ParentControlDesigner parentDesigner = host.GetDesigner(primaryControl.Parent) as ParentControlDesigner;
                                            if (parentDesigner != null)
                                            {
                                                //ask the parent to adjust our wanna-be snapped position
                                                moveOffsetX *= snapSize.Width;
                                                moveOffsetY *= snapSize.Height;

                                                // If the parent is mirrored then we need to negate moveOffsetX.
                                                // This is because moveOffsetX assumes that the origin
                                                // is upper left. That is, when moveOffsetX is positive, we
                                                // are moving right, negative when moving left.

                                                // The parent container's origin depends on its mirroring property.
                                                // Thus when we call propLoc.setValue below, we need to make sure
                                                // that our moveOffset.X correctly reflects the placement of the
                                                // parent container's origin.

                                                // Should do this BEFORE we get the snapped point.
                                                if (primaryControl.Parent.IsMirrored)
                                                {
                                                    moveOffsetX *= -1;
                                                }

                                                Rectangle dragRect = new Rectangle(primaryControl.Location.X, primaryControl.Location.Y,
                                                                                    primaryControl.Width + moveOffsetX, primaryControl.Height + moveOffsetY);

                                                Rectangle newRect = parentDesigner.GetSnappedRect(primaryControl.Bounds, dragRect, true);

                                                //reset our offsets now that we've snapped correctly
                                                if (moveOffsetX != 0)
                                                {
                                                    moveOffsetX = newRect.Width - primaryControl.Width;
                                                }

                                                if (moveOffsetY != 0)
                                                {
                                                    moveOffsetY = newRect.Height - primaryControl.Height;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //In this case we are just moving 1 pixel
                                            checkForIntegralHeight = true;
                                            if (primaryControl.Parent.IsMirrored)
                                            {
                                                moveOffsetX *= -1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        checkForIntegralHeight = true;
                                        if (primaryControl.Parent.IsMirrored)
                                        {
                                            moveOffsetX *= -1;
                                        }
                                    }

                                    foreach (IComponent component in selSvc.GetSelectedComponents())
                                    {
                                        des = host.GetDesigner(component) as ControlDesigner;
                                        if (des != null && ((des.SelectionRules & rules) != rules))
                                        {
                                            //the control must match the rules, if not, then we don't resize it
                                            continue;
                                        }

                                        Control control = component as Control;

                                        if (control != null)
                                        {
                                            int offsetY = moveOffsetY; //we don't want to change moveOFfsetY for all subsequent controls, so cache it off

                                            if (checkForIntegralHeight)
                                            {
                                                PropertyDescriptor propIntegralHeight = TypeDescriptor.GetProperties(component)["IntegralHeight"];
                                                if (propIntegralHeight != null)
                                                {
                                                    object value = propIntegralHeight.GetValue(component);
                                                    if (value is bool && (bool)value == true)
                                                    {
                                                        PropertyDescriptor propItemHeight = TypeDescriptor.GetProperties(component)["ItemHeight"];
                                                        if (propItemHeight != null)
                                                        {
                                                            offsetY *= (int)propItemHeight.GetValue(component); //adjust for integralheight
                                                        }
                                                    }
                                                }
                                            }

                                            PropertyDescriptor propSize = TypeDescriptor.GetProperties(component)["Size"];
                                            if (propSize != null)
                                            {
                                                Size size = (Size)propSize.GetValue(component);
                                                size += new Size(moveOffsetX, offsetY);
                                                propSize.SetValue(component, size);
                                            }
                                        }

                                        //change the Status information ....
                                        if (control == selSvc.PrimarySelection && statusCommandUI != null)
                                        {
                                            statusCommandUI.SetStatusInformation(control);
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
        ///  Called for selection via the tab key.
        /// </summary>
        protected void OnKeySelect(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            bool reverse = (cmd.CommandID.Equals(MenuCommands.KeySelectPrevious));
            RotateTabSelection(reverse);
        }

        /// <summary>
        ///  Called for selection via the tab key.
        /// </summary>
        protected override void OnKeyMove(object sender, EventArgs e)
        {
            base.OnKeyMove(sender, e);
        }

        /// <summary>
        ///  Called when the lock controls menu item is selected.
        /// </summary>
        protected void OnMenuLockControls(object sender, EventArgs e)
        {
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));

                if (host != null)
                {
                    ComponentCollection components = host.Container.Components;

                    if (components != null && components.Count > 0)
                    {
                        DesignerTransaction trans = null;

                        try
                        {
                            trans = host.CreateTransaction(string.Format(SR.CommandSetLockControls, components.Count));
                            MenuCommand cmd = (MenuCommand)sender;
                            bool targetValue = !cmd.Checked;

                            // do the change
                            bool firstTry = true;
                            foreach (IComponent comp in components)
                            {
                                PropertyDescriptor prop = GetProperty(comp, "Locked");
                                //check to see the prop is not null & not readonly
                                if (prop == null)
                                {
                                    continue;
                                }

                                if (prop.IsReadOnly)
                                {
                                    continue;
                                }

                                // look if it's ok to change
                                if (firstTry && !CanCheckout(comp))
                                {
                                    return;
                                }

                                firstTry = false;

                                // do the change
                                prop.SetValue(comp, targetValue);
                            }

                            cmd.Checked = targetValue;
                        }
                        finally
                        {
                            if (trans != null)
                                trans.Commit();
                        }
                    }
                }
            }
            finally
            {
                Cursor.Current = oldCursor;
            }
        }

#if UNUSED
        /// <summary>
        ///  This should never be called.  It is a placeholder for
        ///  menu items that we temporarially want to disable.
        /// </summary>
        private void OnMenuNever(object sender, EventArgs e) {
            Debug.Fail("This menu item should never be invoked.");
        }
#endif

        /// <summary>
        ///  Called to display or destroy the tab order UI.
        /// </summary>
        private void OnMenuTabOrder(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            if (cmd.Checked)
            {
                Debug.Assert(tabOrder != null, "Tab order and menu enabling are out of sync");
                if (tabOrder != null)
                {
                    tabOrder.Dispose();
                    tabOrder = null;
                }

                cmd.Checked = false;
            }
            else
            {
                //if we're creating a tab order view, set the focus to the base comp,
                //this prevents things such as the menu editor service getting broken.
                //
                ISelectionService selSvc = SelectionService;
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host != null && selSvc != null)
                {
                    object baseComp = host.RootComponent;
                    if (baseComp != null)
                    {
                        selSvc.SetSelectedComponents(new object[] { baseComp }, SelectionTypes.Replace);
                    }
                }

                using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
                {
                    tabOrder = new TabOrder((IDesignerHost)GetService(typeof(IDesignerHost)));
                }

                cmd.Checked = true;
            }
        }

        /// <summary>
        ///  Called when the zorder->send to back menu item is selected.
        /// </summary>
        private void OnMenuZOrderSelection(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            CommandID cmdID = cmd.CommandID;

            Debug.Assert(SelectionService != null, "Need SelectionService for sizing command");

            if (SelectionService == null)
            {
                return;
            }

            ArrayList layoutParentList = new ArrayList();
            ArrayList parentList = new ArrayList();
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                IComponentChangeService ccs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
                DesignerTransaction trans = null;

                try
                {
                    string batchString;

                    // NOTE: this only works on Control types
                    ICollection sel = SelectionService.GetSelectedComponents();
                    object[] selectedComponents = new object[sel.Count];
                    sel.CopyTo(selectedComponents, 0);

                    if (cmdID == StandardCommands.BringToFront)
                    {
                        batchString = string.Format(SR.CommandSetBringToFront, selectedComponents.Length);
                    }
                    else
                    {
                        batchString = string.Format(SR.CommandSetSendToBack, selectedComponents.Length);
                    }

                    // sort the components by their current zOrder
                    Array.Sort(selectedComponents, new ControlComparer());

                    trans = designerHost.CreateTransaction(batchString);

                    if (selectedComponents.Length > 0)
                    {
                        int len = selectedComponents.Length;
                        for (int i = len - 1; i >= 0; i--)
                        {
                            Control control = selectedComponents[i] as Control;
                            // Check for NestedComponents like SplitterPanels.
                            // If SplitterPanel is selected and you choose the SendToBack (or BringToFront) option then it should
                            // perform the operation on the Owner (namely SplitContainer)
                            IComponent selComp = selectedComponents[i] as IComponent;
                            if (selComp != null)
                            {
                                INestedSite nestedSite = selComp.Site as INestedSite;
                                if (nestedSite != null)
                                {
                                    INestedContainer nestedContainer = nestedSite.Container as INestedContainer;
                                    if (nestedContainer != null)
                                    {
                                        control = nestedContainer.Owner as Control;
                                        selectedComponents[i] = control; // set this so that we dont have to re-do this logic in the BrintToFront case down.
                                    }
                                }
                            }

                            if (control != null)
                            {
                                Control parent = control.Parent;
                                PropertyDescriptor controlsProp = null;
                                if (parent != null)
                                {
                                    if (ccs != null)
                                    {
                                        try
                                        {
                                            if (!parentList.Contains(parent))
                                            {
                                                controlsProp = TypeDescriptor.GetProperties(parent)["Controls"];
                                                if (controlsProp != null)
                                                {
                                                    // For a perf improvement, we will
                                                    // call OnComponentChanging only once per parent to make sure we do not do unnecessaru serialization for Undo
                                                    //this makes bulk operations way faster (see bug 532657)

                                                    parentList.Add(parent);
                                                    ccs.OnComponentChanging(parent, controlsProp);
                                                }
                                            }
                                        }
                                        catch (CheckoutException ex)
                                        {
                                            if (ex == CheckoutException.Canceled)
                                            {
                                                // If the user canceled the check out then cancel the transaction
                                                if (trans != null)
                                                    trans.Cancel();
                                                return;
                                            }

                                            throw;
                                        }
                                    }

                                    if (!layoutParentList.Contains(parent))
                                    {
                                        // For a perf improvement, we will
                                        // suspendlayout on parentControls.
                                        // Calling BringToFront() forces a layout on the parent each time
                                        // so for many controls this will happen a lot.

                                        layoutParentList.Add(parent);
                                        parent.SuspendLayout();
                                    }
                                }
                            }
                        }

                        for (int i = len - 1; i >= 0; i--)
                        {
                            if (cmdID == StandardCommands.BringToFront)
                            {
                                // we do this backwards to maintain zorder
                                Control otherControl = selectedComponents[len - i - 1] as Control;

                                if (otherControl != null)
                                {
                                    otherControl.BringToFront();
                                }
                            }
                            else if (cmdID == StandardCommands.SendToBack)
                            {
                                Control control = selectedComponents[i] as Control;
                                if (control != null)
                                {
                                    control.SendToBack();
                                }
                            }
                        }
                    }
                }
                finally
                {
                    // Do not fire changed events if the transaction was canceled
                    if ((null != trans) && !trans.Canceled)
                    {
                        foreach (Control parent in parentList)
                        {
                            PropertyDescriptor controlsProp = TypeDescriptor.GetProperties(parent)["Controls"];
                            Debug.Assert(ccs != null && controlsProp != null, "Wrong parent in parent list");
                            if (ccs != null && controlsProp != null)
                            {
                                ccs.OnComponentChanged(parent, controlsProp, null, null);
                            }
                        }

                        foreach (Control parent in layoutParentList)
                        {
                            parent.ResumeLayout();
                        }

                        // now we need to regenerate so the ordering is right.
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
        ///  Determines the status of a menu command.  Commands with this event
        ///  handler are enabled when there is one or more controls on the design
        ///  surface.
        /// </summary>
        protected void OnStatusAnyControls(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            bool enabled = false;
            if (baseControl != null && baseControl.Controls.Count > 0)
            {
                enabled = true;
            }

            cmd.Enabled = enabled;
        }

        /// <summary>
        ///  Determines the status of a menu command.  Commands with this event
        ///  handler are enabled when one or more objects are selected.
        /// </summary>
        protected void OnStatusControlsOnlySelection(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            cmd.Enabled = (selCount > 0) && controlsOnlySelection;
        }

        /// <summary>
        ///  Determines the status of a menu command.  Commands with this event
        ///  handler are enabled when one or more objects are selected and
        ///  SnapToGrid is selected.
        /// </summary>
        protected void OnStatusControlsOnlySelectionAndGrid(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;

            cmd.Enabled = (selCount > 0) && controlsOnlySelection && (!BehaviorService.UseSnapLines);
        }

        protected void OnStatusLockControls(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;

            if (baseControl == null)
            {
                cmd.Enabled = false;
                return;
            }

            cmd.Enabled = controlsOnlySelection;
            cmd.Checked = false;

            //Get the locked property of the base control first...
            //
            PropertyDescriptor lockedProp = TypeDescriptor.GetProperties(baseControl)["Locked"];
            if (lockedProp != null && ((bool)lockedProp.GetValue(baseControl)) == true)
            {
                cmd.Checked = true;
                return;
            }

            IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));

            if (host == null)
            {
                return;
            }

            ComponentDesigner baseDesigner = host.GetDesigner(baseControl) as ComponentDesigner;

            foreach (object component in baseDesigner.AssociatedComponents)
            {
                lockedProp = TypeDescriptor.GetProperties(component)["Locked"];
                if (lockedProp != null && ((bool)lockedProp.GetValue(component)) == true)
                {
                    cmd.Checked = true;
                    return;
                }
            }
        }

        /// <summary>
        ///  Determines the status of a menu command.  Commands with this event
        ///  handler are enabled when more than one object is selected.
        /// </summary>
        protected void OnStatusMultiSelect(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            cmd.Enabled = controlsOnlySelection && selCount > 1;
        }

        /// <summary>
        ///  Determines the status of a menu command.  Commands with this event
        ///  handler are enabled when more than one object is selected and one
        ///  of them is marked as the primary selection.
        /// </summary>
        protected void OnStatusMultiSelectPrimary(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            cmd.Enabled = controlsOnlySelection && selCount > 1 && primarySelection != null;
        }

        /// <summary>
        ///  Determines the status of a menu command.  Ensures that all the selected controls have the same parent.
        /// </summary>
        private void OnStatusMultiSelectNonContained(object sender, EventArgs e)
        {
            OnStatusMultiSelect(sender, e);
            MenuCommand cmd = (MenuCommand)sender;
            if (cmd.Enabled)
            {
                cmd.Enabled = CheckSelectionParenting();
            }
        }

        /// <summary>
        ///  Determines the status of a menu command.  This event handler is
        ///  dedicated to the ShowGrid item.
        /// </summary>
        protected void OnStatusShowGrid(object sender, EventArgs e)
        {
            if (site != null)
            {
                IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");

                if (host != null)
                {
                    IComponent baseComponent = host.RootComponent;
                    if (baseComponent != null && baseComponent is Control)
                    {
                        PropertyDescriptor prop = GetProperty(baseComponent, "DrawGrid");
                        if (prop != null)
                        {
                            bool drawGrid = (bool)prop.GetValue(baseComponent);
                            MenuCommand cmd = (MenuCommand)sender;
                            cmd.Enabled = true;
                            cmd.Checked = drawGrid;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Determines the status of a menu command.  This event handler is
        ///  dedicated to the SnapToGrid item.
        /// </summary>
        protected void OnStatusSnapToGrid(object sender, EventArgs e)
        {
            if (site != null)
            {
                IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || host != null, "IDesignerHost not found");

                if (host != null)
                {
                    IComponent baseComponent = host.RootComponent;
                    if (baseComponent != null && baseComponent is Control)
                    {
                        PropertyDescriptor prop = GetProperty(baseComponent, "SnapToGrid");
                        if (prop != null)
                        {
                            bool snapToGrid = (bool)prop.GetValue(baseComponent);
                            MenuCommand cmd = (MenuCommand)sender;
                            cmd.Enabled = controlsOnlySelection;
                            cmd.Checked = snapToGrid;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Determines the status of a menu command.  Commands with this event
        ///  handler are enabled for zordering.  The rules are:
        ///
        ///  1) More than one component on the form
        ///  2) At least one Control-derived component must be selected
        ///  3) The form must not be selected
        /// </summary>
        private void OnStatusZOrder(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null)
            {
                ComponentCollection comps = host.Container.Components;
                object baseComp = host.RootComponent;

                // The form by itself is one component, so this means
                // we need more than two.
                bool enable = (comps != null && comps.Count > 2 && controlsOnlySelection);

                if (enable)
                {
                    Debug.Assert(SelectionService != null, "Need SelectionService for sizing command");

                    if (SelectionService == null)
                    {
                        return;
                    }

                    // There must also be a control in the mix, and not the base component, and
                    // it cannot be privately inherited.
                    //
                    ICollection selectedComponents = SelectionService.GetSelectedComponents();

                    enable = false;
                    foreach (object obj in selectedComponents)
                    {
                        if (obj is Control &&
                            !TypeDescriptor.GetAttributes(obj)[typeof(InheritanceAttribute)].Equals(InheritanceAttribute.InheritedReadOnly))
                        {
                            enable = true;
                        }

                        // if the form is in there we're always false.
                        if (obj == baseComp)
                        {
                            enable = false;
                            break;
                        }
                    }
                }

                cmd.Enabled = enable;
                return;
            }

            cmd.Enabled = false;
        }

        /// <summary>
        ///  This is called when the selection has changed.  Anyone using CommandSetItems
        ///  that need to update their status based on selection changes should override
        ///  this and update their own commands at this time.  The base implementaion
        ///  runs through all base commands and calls UpdateStatus on them.
        /// </summary>
        protected override void OnUpdateCommandStatus()
        {
            // Now whip through all of the commands and ask them to update.
            //
            for (int i = 0; i < commandSet.Length; i++)
            {
                commandSet[i].UpdateStatus();
            }

            base.OnUpdateCommandStatus();
        }

        /// <summary>
        ///  Rotates the selection to the next parent element.  If backwards is
        ///  set, this will rotate to the first child element.
        /// </summary>
        private void RotateParentSelection(bool backwards)
        {
            object next = null;

            ISelectionService selSvc = SelectionService;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));

            if (selSvc == null || host == null || !(host.RootComponent is Control))
            {
                return;
            }

            IContainer container = host.Container;

            Control component = selSvc.PrimarySelection as Control;
            Control current;
            if (component != null)
            {
                current = component;
            }
            else
            {
                current = (Control)host.RootComponent;
            }

            if (backwards)
            {
                if (current != null)
                {
                    if (current.Controls.Count > 0)
                    {
                        next = current.Controls[0];
                    }
                    else
                    {
                        next = current;
                    }
                }
            }
            else
            {
                if (current != null)
                {
                    next = current.Parent;
                    Control nextControl = next as Control;
                    IContainer controlSiteContainer = null;
                    if (nextControl != null && nextControl.Site != null)
                    {
                        controlSiteContainer = DesignerUtils.CheckForNestedContainer(nextControl.Site.Container); // ...necessary to support SplitterPanel components
                    }

                    if (nextControl == null || nextControl.Site == null || controlSiteContainer != container)
                    {
                        next = current;
                    }
                }
            }

            selSvc.SetSelectedComponents(new object[] { next }, SelectionTypes.Replace);
        }

        /// <summary>
        ///  Rotates the selection to the element next in the tab index.  If backwards
        ///  is set, this will rotate to the previous tab index.
        /// </summary>
        private void RotateTabSelection(bool backwards)
        {
            Control ctl;
            Control baseCtl;
            object targetSelection = null;
            object currentSelection;

            ISelectionService selSvc = SelectionService;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (selSvc == null || host == null || !(host.RootComponent is Control))
            {
                return;
            }

            baseCtl = (Control)host.RootComponent;

            // We must handle two cases of logic here.  We are responsible for handling
            // selection within ourself, and also for components on the tray.  For our
            // own tabbing around, we want to go by tab-order.  When we get to the end
            // of the form, however, we go by selection order into the tray.  And,
            // when we're at the end of the tray we start back at the form.  We must
            // reverse this logic to go backwards.

            currentSelection = selSvc.PrimarySelection;
            ctl = currentSelection as Control;

            if (targetSelection == null && ctl != null && (baseCtl.Contains(ctl) || baseCtl == currentSelection))
            {
                // Our current selection is a control.  Select the next control in
                // the z-order.
                //
                while (null != (ctl = GetNextControlInTab(baseCtl, ctl, !backwards)))
                {
                    if (ctl.Site != null && ctl.Site.Container == ctl.Container)
                    {
                        break;
                    }
                }

                targetSelection = ctl;
            }

            if (targetSelection == null)
            {
                ComponentTray tray = (ComponentTray)GetService(typeof(ComponentTray));
                if (tray != null)
                {
                    targetSelection = tray.GetNextComponent((IComponent)currentSelection, !backwards);
                    if (targetSelection != null)
                    {
                        IComponent selection = targetSelection as IComponent;
                        ControlDesigner controlDesigner = host.GetDesigner(selection) as ControlDesigner;
                        // In Whidbey controls like ToolStrips have componentTray presence, So dont select them again
                        // through compoenent tray since here we select only Components. Hence only
                        // components that have ComponentDesigners should be selected via the ComponentTray.
                        while (controlDesigner != null)
                        {
                            // if the targetSelection from the Tray is a control .. try the next one.
                            selection = tray.GetNextComponent(selection, !backwards);
                            if (selection != null)
                            {
                                controlDesigner = host.GetDesigner(selection) as ControlDesigner;
                            }
                            else
                            {
                                controlDesigner = null;
                            }
                        }
                    }
                }

                if (targetSelection == null)
                {
                    targetSelection = baseCtl;
                }
            }

            selSvc.SetSelectedComponents(new object[] { targetSelection }, SelectionTypes.Replace);
        }

        private Control GetNextControlInTab(Control basectl, Control ctl, bool forward)
        {
            if (forward)
            {
                Control.ControlCollection ctlControls = ctl.Controls;

                if (ctlControls != null && ctlControls.Count > 0)
                {
                    Control found = null;

                    // Cycle through the controls in z-order looking for the lowest tab index.
                    //
                    for (int c = 0; c < ctlControls.Count; c++)
                    {
                        if (found == null || found.TabIndex > ctlControls[c].TabIndex)
                        {
                            found = ctlControls[c];
                        }
                    }

                    return found;
                }

                while (ctl != basectl)
                {
                    int targetIndex = ctl.TabIndex;
                    bool hitCtl = false;
                    Control found = null;
                    Control p = ctl.Parent;

                    // Cycle through the controls in z-order looking for the one with the next highest
                    // tab index.  Because there can be dups, we have to start with the existing tab index and
                    // remember to exclude the current control.
                    //
                    int parentControlCount = 0;

                    Control.ControlCollection parentControls = p.Controls;

                    if (parentControls != null)
                    {
                        parentControlCount = parentControls.Count;
                    }

                    for (int c = 0; c < parentControlCount; c++)
                    {
                        // The logic for this is a bit lengthy, so I have broken it into separate
                        // caluses:

                        // We are not interested in ourself.
                        //
                        if (parentControls[c] != ctl)
                        {
                            // We are interested in controls with >= tab indexes to ctl.  We must include those
                            // controls with equal indexes to account for duplicate indexes.
                            //
                            if (parentControls[c].TabIndex >= targetIndex)
                            {
                                // Check to see if this control replaces the "best match" we've already
                                // found.
                                //
                                if (found == null || found.TabIndex > parentControls[c].TabIndex)
                                {
                                    // Finally, check to make sure that if this tab index is the same as ctl,
                                    // that we've already encountered ctl in the z-order.  If it isn't the same,
                                    // than we're more than happy with it.
                                    //
                                    if (parentControls[c].TabIndex != targetIndex || hitCtl)
                                    {
                                        found = parentControls[c];
                                    }
                                }
                            }
                        }
                        else
                        {
                            // We track when we have encountered "ctl".  We never want to select ctl again, but
                            // we want to know when we've seen it in case we find another control with the same tab index.
                            //
                            hitCtl = true;
                        }
                    }

                    if (found != null)
                    {
                        return found;
                    }

                    ctl = ctl.Parent;
                }
            }
            else
            {
                if (ctl != basectl)
                {
                    int targetIndex = ctl.TabIndex;
                    bool hitCtl = false;
                    Control found = null;
                    Control p = ctl.Parent;

                    // Cycle through the controls in reverse z-order looking for the next lowest tab index.  We must
                    // start with the same tab index as ctl, because there can be dups.
                    //
                    int parentControlCount = 0;

                    Control.ControlCollection parentControls = p.Controls;

                    if (parentControls != null)
                    {
                        parentControlCount = parentControls.Count;
                    }

                    for (int c = parentControlCount - 1; c >= 0; c--)
                    {
                        // The logic for this is a bit lengthy, so I have broken it into separate
                        // caluses:

                        // We are not interested in ourself.
                        //
                        if (parentControls[c] != ctl)
                        {
                            // We are interested in controls with <= tab indexes to ctl.  We must include those
                            // controls with equal indexes to account for duplicate indexes.
                            //
                            if (parentControls[c].TabIndex <= targetIndex)
                            {
                                // Check to see if this control replaces the "best match" we've already
                                // found.
                                //
                                if (found == null || found.TabIndex < parentControls[c].TabIndex)
                                {
                                    // Finally, check to make sure that if this tab index is the same as ctl,
                                    // that we've already encountered ctl in the z-order.  If it isn't the same,
                                    // than we're more than happy with it.
                                    //
                                    if (parentControls[c].TabIndex != targetIndex || hitCtl)
                                    {
                                        found = parentControls[c];
                                    }
                                }
                            }
                        }
                        else
                        {
                            // We track when we have encountered "ctl".  We never want to select ctl again, but
                            // we want to know when we've seen it in case we find another control with the same tab index.
                            //
                            hitCtl = true;
                        }
                    }

                    // If we were unable to find a control we should return the control's parent.  However, if that parent is us, return
                    // NULL.
                    //
                    if (found != null)
                    {
                        ctl = found;
                    }
                    else
                    {
                        if (p == basectl)
                        {
                            return null;
                        }
                        else
                        {
                            return p;
                        }
                    }
                }

                // We found a control.  Walk into this control to find the proper sub control within it to select.
                //
                Control.ControlCollection ctlControls = ctl.Controls;

                while (ctlControls != null && ctlControls.Count > 0)
                {
                    Control found = null;

                    // Cycle through the controls in reverse z-order looking for the one with the highest
                    // tab index.
                    //
                    for (int c = ctlControls.Count - 1; c >= 0; c--)
                    {
                        if (found == null || found.TabIndex < ctlControls[c].TabIndex)
                        {
                            found = ctlControls[c];
                        }
                    }

                    ctl = found;

                    ctlControls = ctl.Controls;
                }
            }

            return ctl == basectl ? null : ctl;
        }

        /// <summary>
        ///  <para>Compares two controls for equality.</para>
        /// </summary>
        private class ControlComparer : IComparer
        {
            /// <summary>
            ///  <para>Compares two controls for equality.</para>
            /// </summary>
            public int Compare(object x, object y)
            {
                // we want to sort items here based on their z-order
                //

                // if they have the same parent,
                // return the comparison based on z-order
                //
                // otherwise based on parent handles so parent groupings
                // will be together
                //
                // otherwise just put non-controls ahead of controls.
                if (x == y)
                {
                    return 0;
                }

                Control cX = x as Control;
                Control cY = y as Control;
                if (cX != null && cY != null)
                {
                    if (cX.Parent == cY.Parent)
                    {
                        Control parent = cX.Parent;
                        if (parent == null)
                        {
                            return 0;
                        }
                        else if (parent.Controls.GetChildIndex(cX) > parent.Controls.GetChildIndex(cY))
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else if (cX.Parent == null || cX.Contains(cY))
                    {
                        return 1;
                    }
                    else if (cY.Parent == null || cY.Contains(cX))
                    {
                        return -1;
                    }
                    else
                    {
                        //REVIEW This doesn't look 64-bit safe
                        return (int)cX.Parent.Handle - (int)cY.Parent.Handle;
                    }
                }
                else if (cY != null)
                {
                    return -1;
                }
                else if (cX != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}

