// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class encapsulates the tab order UI for our form designer.
/// </summary>
[DesignTimeVisible(false)]
[ToolboxItem(false)]
internal class TabOrder : Control, IMouseHandler, IMenuStatusHandler
{
    private IDesignerHost _host;
    private Control? _ctlHover;
    private List<Control>? _tabControls;
    private Rectangle[]? _tabGlyphs;
    private HashSet<Control>? _tabComplete;
    private Dictionary<Control, int>? _tabNext;
    private readonly Font _tabFont;
    private readonly StringBuilder _drawString;
    private readonly Stack<int> _controlIndices;
    private readonly Brush _highlightTextBrush;
    private readonly Pen _highlightPen;
    private readonly int _selSize;
    private readonly Dictionary<Control, PropertyDescriptor> _tabProperties;
    private Region? _region;
    private readonly MenuCommand[] _commands;
    private readonly MenuCommand[] _newCommands;
    private readonly string _decimalSep;

    /// <summary>
    ///  Creates a new tab order control that displays the tab order
    ///  UI for a form.
    /// </summary>
    public TabOrder(IDesignerHost host)
    {
        _host = host;

        // Determine a font for us to use.
        IUIService? uisvc = (IUIService?)host.GetService(typeof(IUIService));
        _tabFont = uisvc is not null && uisvc.Styles["DialogFont"] is Font dialogFont ? dialogFont : DefaultFont;
        _tabFont = new Font(_tabFont, FontStyle.Bold);

        // And compute the proper highlight dimensions.
        _selSize = DesignerUtils.GetAdornmentDimensions(AdornmentType.GrabHandle).Width;

        // Colors and brushes...
        _drawString = new StringBuilder(12);
        _controlIndices = new Stack<int>();
        _highlightTextBrush = new SolidBrush(SystemColors.HighlightText);
        _highlightPen = new Pen(SystemColors.Highlight);

        // The decimal separator
        NumberFormatInfo? formatInfo = (NumberFormatInfo?)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo));
        _decimalSep = formatInfo is not null ? formatInfo.NumberDecimalSeparator : ".";

        _tabProperties = [];

        // Set up a NULL brush so we never try to invalidate the control. This is
        // more efficient for what we're doing
        SetStyle(ControlStyles.Opaque, true);

        // We're an overlay on top of the form
        IOverlayService? os = (IOverlayService?)host.GetService(typeof(IOverlayService));
        Debug.Assert(os is not null, "No overlay service -- tab order UI cannot be shown");
        os?.PushOverlay(this);

        // Push a help keyword so the help system knows we're in place.
        IHelpService? hs = (IHelpService?)host.GetService(typeof(IHelpService));
        hs?.AddContextAttribute("Keyword", "TabOrderView", HelpKeywordType.FilterKeyword);

        _commands =
        [
            new(new EventHandler(OnKeyCancel),
                MenuCommands.KeyCancel),

            new(new EventHandler(OnKeyDefault),
                MenuCommands.KeyDefaultAction),

            new(new EventHandler(OnKeyPrevious),
                MenuCommands.KeyMoveUp),

            new(new EventHandler(OnKeyNext),
                MenuCommands.KeyMoveDown),

            new(new EventHandler(OnKeyPrevious),
                MenuCommands.KeyMoveLeft),

            new(new EventHandler(OnKeyNext),
                MenuCommands.KeyMoveRight),

            new(new EventHandler(OnKeyNext),
                MenuCommands.KeySelectNext),

            new(new EventHandler(OnKeyPrevious),
                MenuCommands.KeySelectPrevious),
        ];

        _newCommands =
        [
            new(new EventHandler(OnKeyDefault),
                MenuCommands.KeyTabOrderSelect),
        ];

        IMenuCommandService? mcs = (IMenuCommandService?)host.GetService(typeof(IMenuCommandService));
        if (mcs is not null)
        {
            foreach (MenuCommand mc in _newCommands)
            {
                mcs.AddCommand(mc);
            }
        }

        // We also override keyboard, menu and mouse handlers. Our override relies on the
        // above array of menu commands, so this must come after we initialize the array.
        IEventHandlerService? ehs = (IEventHandlerService?)host.GetService(typeof(IEventHandlerService));
        ehs?.PushHandler(this);

        // We sync add, remove and change events so we remain in sync with any nastiness that the
        // form may pull on us.
        IComponentChangeService? cs = (IComponentChangeService?)host.GetService(typeof(IComponentChangeService));
        if (cs is not null)
        {
            cs.ComponentAdded += OnComponentAddRemove;
            cs.ComponentRemoved += OnComponentAddRemove;
            cs.ComponentChanged += OnComponentChanged;
        }
    }

    /// <summary>
    ///  Called when it is time for the tab order UI to go away.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_region is not null)
            {
                _region.Dispose();
                _region = null;
            }

            if (_host is not null)
            {
                IOverlayService? os = (IOverlayService?)_host.GetService(typeof(IOverlayService));
                os?.RemoveOverlay(this);

                IEventHandlerService? ehs = (IEventHandlerService?)_host.GetService(typeof(IEventHandlerService));
                ehs?.PopHandler(this);

                IMenuCommandService? mcs = (IMenuCommandService?)_host.GetService(typeof(IMenuCommandService));
                if (mcs is not null)
                {
                    foreach (MenuCommand mc in _newCommands)
                    {
                        mcs.RemoveCommand(mc);
                    }
                }

                // We sync add, remove and change events so we remain in sync with any nastiness that the
                // form may pull on us.
                IComponentChangeService? cs = (IComponentChangeService?)_host.GetService(typeof(IComponentChangeService));
                if (cs is not null)
                {
                    cs.ComponentAdded -= OnComponentAddRemove;
                    cs.ComponentRemoved -= OnComponentAddRemove;
                    cs.ComponentChanged -= OnComponentChanged;
                }

                IHelpService? hs = (IHelpService?)_host.GetService(typeof(IHelpService));
                hs?.RemoveContextAttribute("Keyword", "TabOrderView");

                _host = null!;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  This function does double duty: it draws the tabs if fRegion is false, or it
    ///  computes control region rectangles if fRegion is true (both require that we essentially
    ///  "draw" the tabs)
    /// </summary>
    private void DrawTabs(List<Control> tabs, Graphics graphics, bool fRegion)
    {
        Font font = _tabFont;
        if (fRegion)
        {
            _region = new Region(new Rectangle(0, 0, 0, 0));
        }

        if (_ctlHover is not null)
        {
            Rectangle ctlInner = GetConvertedBounds(_ctlHover);
            Rectangle ctlOuter = ctlInner;
            ctlOuter.Inflate(_selSize, _selSize);

            if (fRegion)
            {
                _region = new Region(ctlOuter);
                _region.Exclude(ctlInner);
            }
            else
            {
                if (_ctlHover.Parent is Control hoverParent)
                {
                    Color backColor = hoverParent.BackColor;
                    Region clip = graphics.Clip;
                    graphics.ExcludeClip(ctlInner);
                    using (SolidBrush brush = new(backColor))
                    {
                        graphics.FillRectangle(brush, ctlOuter);
                    }

                    ControlPaint.DrawSelectionFrame(graphics, active: false, ctlOuter, ctlInner, backColor);
                    graphics.Clip = clip;
                }
            }
        }

        int iCtl = 0;
        foreach (Control control in tabs)
        {
            Rectangle convertedRectangle = GetConvertedBounds(control);

            _drawString.Clear();
            _controlIndices.Clear();

            Control? parent = control;
            Control baseControl = (Control)_host.RootComponent;

            do
            {
                _controlIndices.Push(parent.TabIndex);
                parent = GetSitedParent(parent);
            }
            while (parent != baseControl && parent is not null);

            _drawString.Append(' ').AppendJoin(_decimalSep, _controlIndices).Append(' ');

            if (_tabProperties[control].IsReadOnly)
            {
                _drawString.Append(SR.WindowsFormsTabOrderReadOnly);
                _drawString.Append(' ');
            }

            string str = _drawString.ToString();
            var sz = Size.Ceiling(graphics.MeasureString(str, font));
            convertedRectangle.Width = sz.Width + 2;
            convertedRectangle.Height = sz.Height + 2;

            Debug.Assert(_tabGlyphs is not null, "tabGlyps should not be null here.");
            _tabGlyphs[iCtl++] = convertedRectangle;

            Brush brush;
            Pen pen;
            Color textColor;
            if (fRegion)
            {
                _region?.Union(convertedRectangle);
            }
            else
            {
                if (_tabComplete is not null && !_tabComplete.Contains(control))
                {
                    brush = _highlightTextBrush;
                    pen = _highlightPen;
                    textColor = SystemColors.Highlight;
                }
                else
                {
                    brush = SystemBrushes.Highlight;
                    pen = SystemPens.HighlightText;
                    textColor = SystemColors.HighlightText;
                }

                graphics.FillRectangle(brush, convertedRectangle);
                graphics.DrawRectangle(pen, convertedRectangle.X, convertedRectangle.Y, convertedRectangle.Width - 1, convertedRectangle.Height - 1);

                Brush foreBrush = new SolidBrush(textColor);
                graphics.DrawString(str, font, foreBrush, convertedRectangle.X + 1, convertedRectangle.Y + 1);
                foreBrush.Dispose();
            }
        }

        if (fRegion)
        {
            Control rootControl = (Control)_host.RootComponent;
            _region?.Intersect(GetConvertedBounds(rootControl));
            Region = _region;
        }
    }

    /// <summary>
    ///  returns a control in the given tab vector that is at the given point, in
    ///  screen coords.
    /// </summary>
    private Control? GetControlAtPoint(List<Control> tabs, int x, int y)
    {
        Rectangle screenRectangle;
        Control? ctlFound = null;
        Control? parent;

        foreach (Control control in tabs)
        {
            parent = GetSitedParent(control);
            if (parent is null)
            {
                continue;
            }

            screenRectangle = parent.RectangleToScreen(control.Bounds);

            // We do not break if we find it here. The vector is already setup
            // to have all controls in the current tabbing order, and child controls
            // are always after their parents. If we broke, we wouldn't necessarily
            // find the appropriate child.
            if (screenRectangle.Contains(x, y))
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

        Rectangle convertedBounds = control.Bounds;
        convertedBounds = parent.RectangleToScreen(convertedBounds);
        convertedBounds = RectangleToClient(convertedBounds);
        return convertedBounds;
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
    ///  by the tab order UI. We only want parents that are
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
            if (site is not null && container == _host)
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

        // Now actually count the controls. We add them to the list in reverse
        // order because the Controls collection is in z-order, and our list
        // needs to be in reverse z-order. When done, we want this list to be
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

        if (control.Site is not ISite site || site.Container != _host)
        {
            return false;
        }

        PropertyDescriptor? prop = TypeDescriptor.GetProperties(control)["TabIndex"];

        if (prop is null || !prop.IsBrowsable)
        {
            return false;
        }

        _tabProperties[control] = prop;
        return true;
    }

    /// <summary>
    ///  Called in response to a component add or remove event. Here we re-acquire our
    ///  set of tabs.
    /// </summary>
    private void OnComponentAddRemove(object? sender, ComponentEventArgs ce)
    {
        _ctlHover = null;
        _tabControls = null;
        _tabGlyphs = null;
        _tabComplete?.Clear();
        _tabNext?.Clear();

        if (_region is not null)
        {
            _region.Dispose();
            _region = null;
        }

        Invalidate();
    }

    /// <summary>
    ///  Called in response to a component change event. Here we update our
    ///  tab order and redraw.
    /// </summary>
    private void OnComponentChanged(object? sender, ComponentChangedEventArgs ce)
    {
        _tabControls = null;
        _tabGlyphs = null;
        if (_region is not null)
        {
            _region.Dispose();
            _region = null;
        }

        Invalidate();
    }

    /// <summary>
    ///  Closes the tab order UI.
    /// </summary>
    private void OnKeyCancel(object? sender, EventArgs e)
    {
        IMenuCommandService? mcs = (IMenuCommandService?)_host.GetService(typeof(IMenuCommandService));
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
        if (_ctlHover is not null)
        {
            SetNextTabIndex(_ctlHover);
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
    ///  This is called when the user double clicks on a component. The typical
    ///  behavior is to create an event handler for the component's default event
    ///  and delegate (?) to the handler.
    /// </summary>
    public virtual void OnMouseDoubleClick(IComponent component)
    {
    }

    /// <summary>
    ///  This is called when a mouse button is depressed. This will perform
    ///  the default drag action for the selected components, which is to
    ///  move those components around by the mouse.
    /// </summary>
    public virtual void OnMouseDown(IComponent component, MouseButtons button, int x, int y)
    {
        if (_ctlHover is not null)
        {
            SetNextTabIndex(_ctlHover);
        }
    }

    /// <summary>
    ///  Overrides control.OnMouseDown. Here we set the tab index. We must
    ///  do this as well as the above OnMouseDown to take into account clicks
    ///  in the tab index numbers.
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (_ctlHover is not null)
        {
            SetNextTabIndex(_ctlHover);
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
        if (_tabControls is not null)
        {
            Control? ctl = GetControlAtPoint(_tabControls, x, y);
            SetNewHover(ctl);
        }
    }

    /// <summary>
    ///  Overrides control. We update our cursor here. We must do this
    ///  as well as the OnSetCursor to take into account mouse movements
    ///  over the tab index numbers.
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_tabGlyphs is not null && _tabControls is not null)
        {
            Control? ctl = null;

            for (int i = 0; i < _tabGlyphs.Length; i++)
            {
                if (_tabGlyphs[i].Contains(e.X, e.Y))
                {
                    // Do not break if we find it -- we must
                    // work for nested children too.
                    ctl = _tabControls[i];
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
        => Cursor.Current = _ctlHover is not null ? Cursors.Cross : Cursors.Default;

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

        if (_tabControls is null)
        {
            _tabControls = [];
            GetTabbing((Control)_host.RootComponent, _tabControls);
            _tabGlyphs = new Rectangle[_tabControls.Count];
        }

        _tabComplete ??= [];
        _tabNext ??= [];

        if (_region is null)
        {
            DrawTabs(_tabControls, e.Graphics, true);
        }

        DrawTabs(_tabControls, e.Graphics, false);
    }

    /// <summary>
    ///  CommandSet will check with this handler on each status update
    ///  to see if the handler wants to override the availability of
    ///  this command.
    /// </summary>
    public bool OverrideInvoke(MenuCommand cmd)
    {
        for (int i = 0; i < _commands.Length; i++)
        {
            if (Equals(_commands[i].CommandID, cmd.CommandID))
            {
                _commands[i].Invoke();
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
        for (int i = 0; i < _commands.Length; i++)
        {
            if (Equals(_commands[i].CommandID, cmd.CommandID))
            {
                cmd.Enabled = _commands[i].Enabled;
                return true;
            }
        }

        // Overriding the status of commands is easy. We only
        // get commands that the designer implements, so we don't
        // have to pick and choose which ones to get rid of. We
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
        Control? control = _ctlHover;
        Control form = (Control)_host.RootComponent;
        control ??= form;
        while ((control = form.GetNextControl(control, forward)) is not null)
        {
            if (GetTabbable(control))
                break;
        }

        SetNewHover(control);
    }

    /// <summary>
    ///  Establishes a new hover control.
    /// </summary>
    private void SetNewHover(Control? ctl)
    {
        if (_ctlHover != ctl)
        {
            InvalidateHoverRegion();
            _ctlHover = ctl;
            InvalidateHoverRegion();
        }

        void InvalidateHoverRegion()
        {
            if (_ctlHover is not null)
            {
                if (_region is not null)
                {
                    _region.Dispose();
                    _region = null;
                }

                Rectangle rc = GetConvertedBounds(_ctlHover);
                rc.Inflate(_selSize, _selSize);
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
        if (_tabControls is null || _tabComplete is null || _tabNext is null)
        {
            return;
        }

        int max, index = 0;
        Control? parent = GetSitedParent(ctl);
        _tabComplete.Add(ctl);
        if (parent is not null)
        {
            _tabNext.TryGetValue(parent, out index);
        }

        try
        {
            if (!_tabProperties.TryGetValue(ctl, out PropertyDescriptor? prop))
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
                _tabNext[parent] = newIndex;
            }

            if (_tabComplete.Count == _tabControls.Count)
                _tabComplete.Clear();

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
        catch (Exception ex) when (!ex.IsCriticalException())
        {
        }
    }
}
