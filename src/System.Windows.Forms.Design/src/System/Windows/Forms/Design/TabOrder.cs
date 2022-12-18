// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This class encapsulates the tab order UI for our form designer.
    /// </summary>
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    internal class TabOrder : Control, IMouseHandler, IMenuStatusHandler
    {
        private IDesignerHost? host;
        private Control? ctlHover;
        private List<Control>? tabControls;
        private Rectangle[]? tabGlyphs;
        private HashSet<Control>? tabComplete;
        private Dictionary<Control, int>? tabNext;
        private readonly Font tabFont;
        private readonly StringBuilder drawString;
        private readonly Brush highlightTextBrush;
        private readonly Pen highlightPen;
        private readonly int selSize;
        private readonly Dictionary<Control, PropertyDescriptor> tabProperties;
        private Region? region;
        private readonly MenuCommand[] commands;
        private readonly MenuCommand[] newCommands;
        private readonly string decimalSep;

        /// <summary>
        ///  Creates a new tab order control that displays the tab order
        ///  UI for a form.
        /// </summary>
        public TabOrder(IDesignerHost host)
        {
            this.host = host;

            // Determine a font for us to use.
            IUIService? uisvc = (IUIService?)host.GetService(typeof(IUIService));
            tabFont = uisvc is not null && uisvc.Styles["DialogFont"] is Font dialogFont ? dialogFont : DefaultFont;
            tabFont = new Font(tabFont, FontStyle.Bold);

            // And compute the proper highlight dimensions.
            selSize = DesignerUtils.GetAdornmentDimensions(AdornmentType.GrabHandle).Width;

            // Colors and brushes...
            drawString = new StringBuilder(12);
            highlightTextBrush = new SolidBrush(SystemColors.HighlightText);
            highlightPen = new Pen(SystemColors.Highlight);

            // The decimal separator
            NumberFormatInfo? formatInfo = (NumberFormatInfo?)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo));
            decimalSep = formatInfo is not null ? formatInfo.NumberDecimalSeparator : ".";

            tabProperties = new();

            // Set up a NULL brush so we never try to invalidate the control.  This is
            // more efficient for what we're doing
            SetStyle(ControlStyles.Opaque, true);

            // We're an overlay on top of the form
            IOverlayService? os = (IOverlayService?)host.GetService(typeof(IOverlayService));
            Debug.Assert(os is not null, "No overlay service -- tab order UI cannot be shown");
            os?.PushOverlay(this);

            // Push a help keyword so the help system knows we're in place.
            IHelpService? hs = (IHelpService?)host.GetService(typeof(IHelpService));
            hs?.AddContextAttribute("Keyword", "TabOrderView", HelpKeywordType.FilterKeyword);

            commands = new MenuCommand[]
            {
                new MenuCommand(new EventHandler(OnKeyCancel),
                                MenuCommands.KeyCancel),

                new MenuCommand(new EventHandler(OnKeyDefault),
                                MenuCommands.KeyDefaultAction),

                new MenuCommand(new EventHandler(OnKeyPrevious),
                                MenuCommands.KeyMoveUp),

                new MenuCommand(new EventHandler(OnKeyNext),
                                MenuCommands.KeyMoveDown),

                new MenuCommand(new EventHandler(OnKeyPrevious),
                                MenuCommands.KeyMoveLeft),

                new MenuCommand(new EventHandler(OnKeyNext),
                                MenuCommands.KeyMoveRight),

                new MenuCommand(new EventHandler(OnKeyNext),
                                MenuCommands.KeySelectNext),

                new MenuCommand(new EventHandler(OnKeyPrevious),
                                MenuCommands.KeySelectPrevious),
            };

            newCommands = new MenuCommand[]
            {
                new MenuCommand(new EventHandler(OnKeyDefault),
                                MenuCommands.KeyTabOrderSelect),
            };

            IMenuCommandService? mcs = (IMenuCommandService?)host.GetService(typeof(IMenuCommandService));
            if (mcs is not null)
            {
                foreach (MenuCommand mc in newCommands)
                {
                    mcs.AddCommand(mc);
                }
            }

            // We also override keyboard, menu and mouse handlers.  Our override relies on the
            // above array of menu commands, so this must come after we initialize the array.
            IEventHandlerService? ehs = (IEventHandlerService?)host.GetService(typeof(IEventHandlerService));
            ehs?.PushHandler(this);

            // We sync add, remove and change events so we remain in sync with any nastiness that the
            // form may pull on us.
            IComponentChangeService? cs = (IComponentChangeService?)host.GetService(typeof(IComponentChangeService));
            if (cs is not null)
            {
                cs.ComponentAdded += new ComponentEventHandler(OnComponentAddRemove);
                cs.ComponentRemoved += new ComponentEventHandler(OnComponentAddRemove);
                cs.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
            }
        }

        /// <summary>
        ///  Called when it is time for the tab order UI to go away.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (region is not null)
                {
                    region.Dispose();
                    region = null;
                }

                if (host is not null)
                {
                    IOverlayService? os = (IOverlayService?)host.GetService(typeof(IOverlayService));
                    os?.RemoveOverlay(this);

                    IEventHandlerService? ehs = (IEventHandlerService?)host.GetService(typeof(IEventHandlerService));
                    ehs?.PopHandler(this);

                    IMenuCommandService? mcs = (IMenuCommandService?)host.GetService(typeof(IMenuCommandService));
                    if (mcs is not null)
                    {
                        foreach (MenuCommand mc in newCommands)
                        {
                            mcs.RemoveCommand(mc);
                        }
                    }

                    // We sync add, remove and change events so we remain in sync with any nastiness that the
                    // form may pull on us.
                    IComponentChangeService? cs = (IComponentChangeService?)host.GetService(typeof(IComponentChangeService));
                    if (cs is not null)
                    {
                        cs.ComponentAdded -= new ComponentEventHandler(OnComponentAddRemove);
                        cs.ComponentRemoved -= new ComponentEventHandler(OnComponentAddRemove);
                        cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                    }

                    IHelpService? hs = (IHelpService?)host.GetService(typeof(IHelpService));
                    hs?.RemoveContextAttribute("Keyword", "TabOrderView");

                    host = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  this function does double duty:  it draws the tabs if fRegion is false, or it
        ///  computes control region rects if fRegion is true (both require that we essentially
        ///  "draw" the tabs)
        /// </summary>
        private void DrawTabs(List<Control> tabs, Graphics gr, bool fRegion)
        {
            Font font = tabFont;
            if (fRegion)
            {
                region = new Region(new Rectangle(0, 0, 0, 0));
            }

            if (ctlHover is not null)
            {
                Rectangle ctlInner = GetConvertedBounds(ctlHover);
                Rectangle ctlOuter = ctlInner;
                ctlOuter.Inflate(selSize, selSize);

                if (fRegion)
                {
                    region = new Region(ctlOuter);
                    region.Exclude(ctlInner);
                }
                else
                {
                    if (ctlHover.Parent is Control hoverParent)
                    {
                        Color backColor;
                        backColor = hoverParent.BackColor;

                        Region clip = gr.Clip;
                        gr.ExcludeClip(ctlInner);
                        using (SolidBrush brush = new SolidBrush(backColor))
                        {
                            gr.FillRectangle(brush, ctlOuter);
                        }

                        ControlPaint.DrawSelectionFrame(gr, false, ctlOuter, ctlInner, backColor);
                        gr.Clip = clip;
                    }
                }
            }

            int iCtl = 0;
            string str;
            Rectangle rc;
            Control? parent;
            foreach (Control ctl in tabs)
            {
                rc = GetConvertedBounds(ctl);

                drawString.Length = 0;

                parent = GetSitedParent(ctl);
                Debug.Assert(host is not null, "host is set in the contructor should not be null");
                Control baseControl = (Control)host.RootComponent;

                while (parent != baseControl && parent is not null)
                {
                    drawString.Insert(0, decimalSep);
                    drawString.Insert(0, parent.TabIndex.ToString(CultureInfo.CurrentCulture));
                    parent = GetSitedParent(parent);
                }

                drawString.Insert(0, ' ');
                drawString.Append(ctl.TabIndex.ToString(CultureInfo.CurrentCulture));
                drawString.Append(' ');

                if (tabProperties[ctl].IsReadOnly)
                {
                    drawString.Append(SR.WindowsFormsTabOrderReadOnly);
                    drawString.Append(' ');
                }

                str = drawString.ToString();
                var sz = Size.Ceiling(gr.MeasureString(str, font));
                rc.Width = sz.Width + 2;
                rc.Height = sz.Height + 2;

                Debug.Assert(tabGlyphs is not null, "tabGlyps should not be null here.");
                tabGlyphs[iCtl++] = rc;

                Brush brush;
                Pen pen;
                Color textColor;
                if (fRegion)
                {
                    region?.Union(rc);
                }
                else
                {
                    if (tabComplete is not null && !tabComplete.Contains(ctl))
                    {
                        brush = highlightTextBrush;
                        pen = highlightPen;
                        textColor = SystemColors.Highlight;
                    }
                    else
                    {
                        brush = SystemBrushes.Highlight;
                        pen = SystemPens.HighlightText;
                        textColor = SystemColors.HighlightText;
                    }

                    gr.FillRectangle(brush, rc);
                    gr.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);

                    Brush foreBrush = new SolidBrush(textColor);
                    gr.DrawString(str, font, foreBrush, rc.X + 1, rc.Y + 1);
                    foreBrush.Dispose();
                }
            }

            if (fRegion)
            {
                Debug.Assert(host is not null, "host is set in the contructor should not be null");
                Control rootControl = (Control)host.RootComponent;
                rc = GetConvertedBounds(rootControl);
                region?.Intersect(rc);
                Region = region;
            }
        }

        /// <summary>
        ///  returns a control in the given tab vector that is at the given point, in
        ///  screen coords.
        /// </summary>
        private Control? GetControlAtPoint(List<Control> tabs, int x, int y)
        {
            IEnumerator e = tabs.GetEnumerator();
            Rectangle rc;
            Control? ctlFound = null;
            Control? parent;

            foreach (Control control in tabs)
            {
                parent = GetSitedParent(control);
                if (parent is null)
                {
                    continue;
                }

                rc = parent.RectangleToScreen(control.Bounds);

                // We do not break if we find it here. The vector is already setup
                // to have all controls in the current tabbing order, and child controls
                // are always after their parents. If we broke, we wouldn't necessarily
                // find the appropriate child.
                if (rc.Contains(x, y))
                {
                    ctlFound = control;
                }
            }

            return ctlFound;
        }

        /// <summary>
        ///  returns a rectangle in our own client space that represents the bounds
        ///  of the given control.
        /// </summary>
        private Rectangle GetConvertedBounds(Control? control)
        {
            if (control is null || control.Parent is not Control parent)
            {
                return Rectangle.Empty;
            }

            Rectangle rc = control.Bounds;
            rc = parent.RectangleToScreen(rc);
            rc = RectangleToClient(rc);
            return rc;
        }

        /// <summary>
        ///  returns the maximum valid control count for the given control. This
        ///  may be less than Control.getControlCount() because of invisible controls
        ///  and our own control
        /// </summary>
        private int GetMaxControlCount(Control ctl)
        {
            int count = 0;

            for (int n = 0; n < ctl.Controls.Count; n++)
            {
                if (GetTabbable(ctl.Controls[n]))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        ///  Retrieves the next parent control that would be usable
        ///  by the tab order UI.  We only want parents that are
        ///  sited by the designer host.
        /// </summary>
        private Control? GetSitedParent(Control child)
        {
            Control? parent = child.Parent;

            while (parent is not null)
            {
                ISite? site = parent.Site;
                IContainer? container = null;
                if (site is not null)
                {
                    container = site.Container;
                }

                // Necessary to support SplitterPanel components.
                container = DesignerUtils.CheckForNestedContainer(container);
                if (site is not null && container == host)
                {
                    break;
                }

                parent = parent.Parent;
            }

            return parent;
        }

        /// <summary>
        ///  recursively fills the given tab vector with a control list
        /// </summary>
        private void GetTabbing(Control control, IList tabs)
        {
            if (control is null)
            {
                return;
            }

            Control ctlTab;

            // Now actually count the controls.  We add them to the list in reverse
            // order because the Controls collection is in z-order, and our list
            // needs to be in reverse z-order.  When done, we want this list to be
            // in z-order from back-most to top-most, and from parent-most to
            // child-most.
            int count = control.Controls.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                ctlTab = control.Controls[i];

                if (GetSitedParent(ctlTab) is not null && GetTabbable(ctlTab))
                {
                    tabs.Add(ctlTab);
                }

                if (ctlTab.Controls.Count > 0)
                {
                    GetTabbing(ctlTab, tabs);
                }
            }
        }

        /// <summary>
        ///  returns true if this component should show up in our tab list
        /// </summary>
        private bool GetTabbable(Control control)
        {
            for (Control? c = control; c is not null; c = c.Parent)
            {
                if (!c.Visible)
                    return false;
            }

            if (control.Site is not ISite site || site.Container != host)
            {
                return false;
            }

            PropertyDescriptor? prop = TypeDescriptor.GetProperties(control)["TabIndex"];

            if (prop is null || !prop.IsBrowsable)
            {
                return false;
            }

            tabProperties[control] = prop;
            return true;
        }

        /// <summary>
        ///  Called in response to a component add or remove event.  Here we re-acquire our
        ///  set of tabs.
        /// </summary>
        private void OnComponentAddRemove(object? sender, ComponentEventArgs ce)
        {
            ctlHover = null;
            tabControls = null;
            tabGlyphs = null;
            tabComplete?.Clear();
            tabNext?.Clear();

            if (region is not null)
            {
                region.Dispose();
                region = null;
            }

            Invalidate();
        }

        /// <summary>
        ///  Called in response to a component change event.  Here we update our
        ///  tab order and redraw.
        /// </summary>
        private void OnComponentChanged(object? sender, ComponentChangedEventArgs ce)
        {
            tabControls = null;
            tabGlyphs = null;
            if (region is not null)
            {
                region.Dispose();
                region = null;
            }

            Invalidate();
        }

        /// <summary>
        ///  Closes the tab order UI.
        /// </summary>
        private void OnKeyCancel(object? sender, EventArgs e)
        {
            IMenuCommandService? mcs = (IMenuCommandService?)host?.GetService(typeof(IMenuCommandService));
            Debug.Assert(mcs is not null, "No menu command service, can't get out of tab order UI");
            if (mcs is not null)
            {
                MenuCommand? mc = mcs.FindCommand(StandardCommands.TabOrder);
                Debug.Assert(mc is not null, "No tab order menu command, can't get out of tab order UI");
                mc?.Invoke();
            }
        }

        /// <summary>
        ///  Sets the current tab order selection.
        /// </summary>
        private void OnKeyDefault(object? sender, EventArgs e)
        {
            if (ctlHover is not null)
            {
                SetNextTabIndex(ctlHover);
                RotateControls(true);
            }
        }

        /// <summary>
        ///  Selects the next component in the tab order.
        /// </summary>
        private void OnKeyNext(object? sender, EventArgs e)
        {
            RotateControls(true);
        }

        /// <summary>
        ///  Selects the previous component in the tab order.
        /// </summary>
        private void OnKeyPrevious(object? sender, EventArgs e)
        {
            RotateControls(false);
        }

        /// <summary>
        ///  This is called when the user double clicks on a component.  The typical
        ///  behavior is to create an event handler for the component's default event
        ///  and delegate (?) to the handler.
        /// </summary>
        public virtual void OnMouseDoubleClick(IComponent component)
        {
        }

        /// <summary>
        ///  This is called when a mouse button is depressed.  This will perform
        ///  the default drag action for the selected components,  which is to
        ///  move those components around by the mouse.
        /// </summary>
        public virtual void OnMouseDown(IComponent component, MouseButtons button, int x, int y)
        {
            if (ctlHover is not null)
            {
                SetNextTabIndex(ctlHover);
            }
        }

        /// <summary>
        ///  Overrides control.OnMouseDown.  Here we set the tab index.  We must
        ///  do this as well as the above OnMouseDown to take into account clicks
        ///  in the tab index numbers.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (ctlHover is not null)
            {
                SetNextTabIndex(ctlHover);
            }
        }

        /// <summary>
        ///  This is called when the mouse momentarily hovers over the
        ///  view for the given component.
        /// </summary>
        public virtual void OnMouseHover(IComponent component)
        {
        }

        /// <summary>
        ///  This is called for each movement of the mouse.
        /// </summary>
        public virtual void OnMouseMove(IComponent component, int x, int y)
        {
            if (tabControls is not null)
            {
                Control? ctl = GetControlAtPoint(tabControls, x, y);
                SetNewHover(ctl);
            }
        }

        /// <summary>
        ///  Overrides control.  We update our cursor here.  We must do this
        ///  as well as the OnSetCursor to take into account mouse movements
        ///  over the tab index numbers.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (tabGlyphs is not null && tabControls is not null)
            {
                Control? ctl = null;

                for (int i = 0; i < tabGlyphs.Length; i++)
                {
                    if (tabGlyphs[i].Contains(e.X, e.Y))
                    {
                        // Do not break if we find it -- we must
                        // work for nested children too.
                        ctl = tabControls[i];
                    }
                }

                SetNewHover(ctl);
            }

            SetAppropriateCursor();
        }

        /// <summary>
        ///  This is called when the user releases the mouse from a component.
        ///  This will update the UI to reflect the release of the mouse.
        /// </summary>
        public virtual void OnMouseUp(IComponent component, MouseButtons button)
        {
        }

        private void SetAppropriateCursor()
            => Cursor.Current = ctlHover is not null ? Cursors.Cross : Cursors.Default;

        /// <summary>
        ///  This is called when the cursor for the given component should be updated.
        ///  The mouse is always over the given component's view when this is called.
        /// </summary>
        public virtual void OnSetCursor(IComponent component)
            => SetAppropriateCursor();

        /// <summary>
        ///  Paints the tab control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (tabControls is null)
            {
                tabControls = new();
                Debug.Assert(host is not null, "host is set in the contructor should not be null");
                GetTabbing((Control)host.RootComponent, tabControls);
                tabGlyphs = new Rectangle[tabControls.Count];
            }

            tabComplete ??= new();
            tabNext ??= new();

            if (region is null)
            {
                DrawTabs(tabControls, e.Graphics, true);
            }

            DrawTabs(tabControls, e.Graphics, false);
        }

        /// <summary>
        ///  CommandSet will check with this handler on each status update
        ///  to see if the handler wants to override the availability of
        ///  this command.
        /// </summary>
        public bool OverrideInvoke(MenuCommand cmd)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (Equals(commands[i].CommandID, cmd.CommandID))
                {
                    commands[i].Invoke();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  CommandSet will check with this handler on each status update
        ///  to see if the handler wants to override the availability of
        ///  this command.
        /// </summary>
        public bool OverrideStatus(MenuCommand cmd)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (Equals(commands[i].CommandID, cmd.CommandID))
                {
                    cmd.Enabled = commands[i].Enabled;
                    return true;
                }
            }

            // Overriding the status of commands is easy.  We only
            // get commands that the designer implements, so we don't
            // have to pick and choose which ones to get rid of.  We
            // keep a select view and disable the rest.
            if (!Equals(cmd.CommandID, StandardCommands.TabOrder))
            {
                cmd.Enabled = false;
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Called when the keyboard has been pressed to rotate us
        ///  through the control list.
        /// </summary>
        private void RotateControls(bool forward)
        {
            Control? ctl = ctlHover;
            Debug.Assert(host is not null, "host is set in the contructor should not be null");
            Control form = (Control)host.RootComponent;

            ctl ??= form;

            while ((ctl = form.GetNextControl(ctl, forward)) is not null)
            {
                if (GetTabbable(ctl))
                    break;
            }

            SetNewHover(ctl);
        }

        /// <summary>
        ///  Establishes a new hover control.
        /// </summary>
        private void SetNewHover(Control? ctl)
        {
            if (ctlHover != ctl)
            {
                if (ctlHover is not null)
                {
                    if (region is not null)
                    {
                        region.Dispose();
                        region = null;
                    }

                    Rectangle rc = GetConvertedBounds(ctlHover);
                    rc.Inflate(selSize, selSize);
                    Invalidate(rc);
                }

                ctlHover = ctl;

                if (ctlHover is not null)
                {
                    if (region is not null)
                    {
                        region.Dispose();
                        region = null;
                    }

                    Rectangle rc = GetConvertedBounds(ctlHover);
                    rc.Inflate(selSize, selSize);
                    Invalidate(rc);
                }
            }
        }

        /// <summary>
        ///  sets up the next tab index for the given control
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        private void SetNextTabIndex(Control ctl)
        {
            if (tabControls is null || tabComplete is null || tabNext is null)
            {
                return;
            }

            int max, index = 0;
            Control? parent = GetSitedParent(ctl);
            tabComplete.Add(ctl);
            if (parent is not null)
            {
                tabNext.TryGetValue(parent, out index);
            }

            try
            {
                if (!tabProperties.TryGetValue(ctl, out PropertyDescriptor? prop))
                {
                    return;
                }

                int newIndex = index + 1;

                if (prop.IsReadOnly && prop.GetValue(ctl) is int propValue)
                {
                    newIndex = propValue + 1;
                }

                max = parent is not null ? GetMaxControlCount(parent) : 0;

                if (newIndex >= max)
                {
                    newIndex = 0;
                }

                if (parent is not null)
                {
                    tabNext[parent] = newIndex;
                }

                if (tabComplete.Count == tabControls.Count)
                    tabComplete.Clear();

                // Now set the property
                if (!prop.IsReadOnly)
                {
                    try
                    {
                        prop.SetValue(ctl, index);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    // If the property is read only, we still count it
                    // so that other properties can "flow" around it.
                    // Therefore, we need a paint.
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }
            }
        }
    }
}
