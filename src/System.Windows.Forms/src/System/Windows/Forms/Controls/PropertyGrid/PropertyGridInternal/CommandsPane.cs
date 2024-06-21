// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  The commands pane optionally shown at the bottom of the <see cref="PropertyGrid"/>. This pane is used
///  to host links to <see cref="DesignerVerb"/>s associated with the <see cref="PropertyGrid"/>'s selected
///  object(s).
/// </summary>
/// <remarks>
///  <para>
///   <see cref="DesignerVerb"/> commands are found by looking for services on <see cref="IComponent.Site"/>.
///   Specifically, it first looks for <see cref="IMenuCommandService"/>, then falls back to looking for
///   <see cref="IDesigner.Verbs"/> from any associated <see cref="IDesignerHost"/>. Note that
///   <see cref="PropertyGrid"/> will not set commands when it is in design mode.
///  </para>
///  <para>
///   <see cref="PropertyGrid.CommandsVisible"/> controls the visibility of this control.
///  </para>
/// </remarks>
internal partial class CommandsPane : PropertyGrid.SnappableControl
{
    private object? _component;
    private DesignerVerb[]? _verbs;
    private LinkLabel? _label;
    private bool _allowVisible = true;
    private int _optimalHeight = -1;

    internal CommandsPane(PropertyGrid owner) : base(owner)
    {
        Text = "Command Pane";
    }

    public virtual bool AllowVisible
    {
        get => _allowVisible;
        set
        {
            if (_allowVisible != value)
            {
                _allowVisible = value;
                Visible = value && WouldBeVisible;
            }
        }
    }

    /// <inheritdoc />
    protected override AccessibleObject CreateAccessibilityInstance()
        => new CommandsPaneAccessibleObject(this, OwnerPropertyGrid);

    public override Rectangle DisplayRectangle
    {
        get
        {
            Size size = ClientSize;
            return new Rectangle(4, 4, size.Width - 8, size.Height - 8);
        }
    }

    public LinkLabel Label
    {
        get
        {
            if (_label is null)
            {
                _label = new LinkLabel
                {
                    Dock = DockStyle.Fill,
                    LinkBehavior = LinkBehavior.AlwaysUnderline,

                    // Use default LinkLabel colors for regular, active, and visited.
                    DisabledLinkColor = Application.SystemColors.ControlDark
                };

                _label.LinkClicked += LinkClicked;
                Controls.Add(_label);
            }

            return _label;
        }
    }

    public virtual bool WouldBeVisible => _component is not null;

    public override int GetOptimalHeight(int width)
    {
        if (_optimalHeight == -1)
        {
            int lineHeight = (int)(1.5 * Font.Height);
            int verbCount = 0;
            if (_verbs is not null)
            {
                verbCount = _verbs.Length;
            }

            _optimalHeight = verbCount * lineHeight + 8;
        }

        return _optimalHeight;
    }

    public override int SnapHeightRequest(int request) => request;

    /// <inheritdoc />
    internal override bool SupportsUiaProviders => true;

    private void LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            if (e.Link is null || !e.Link.Enabled)
            {
                return;
            }

            ((DesignerVerb?)e.Link.LinkData)?.Invoke();
        }
        catch (Exception ex)
        {
            RTLAwareMessageBox.Show(
                this,
                ex.Message,
                SR.PBRSErrorTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1,
                0);
        }
    }

    private void OnCommandChanged(object? sender, EventArgs e) => InitializeLabelLinks();

    protected override void OnGotFocus(EventArgs e)
    {
        Label.Focus();
        Label.Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        _optimalHeight = -1;
    }

    internal void SetColors(Color background, Color normalText, Color link, Color activeLink, Color visitedLink, Color disabledLink)
    {
        Label.BackColor = background;
        Label.ForeColor = normalText;
        Label.LinkColor = link;
        Label.ActiveLinkColor = activeLink;
        Label.VisitedLinkColor = visitedLink;
        Label.DisabledLinkColor = disabledLink;
    }

    public void FocusLabel() => Label.Focus();

    public virtual void SetVerbs(object? component, DesignerVerb[]? verbs)
    {
        if (_verbs is not null)
        {
            for (int i = 0; i < _verbs.Length; i++)
            {
                _verbs[i].CommandChanged -= OnCommandChanged;
            }

            _component = null;
            _verbs = null;
        }

        if (component is null || verbs is null || verbs.Length == 0)
        {
            Visible = false;
            Label.Links.Clear();
            Label.Text = null;
        }
        else
        {
            _component = component;
            _verbs = verbs;

            for (int i = 0; i < verbs.Length; i++)
            {
                verbs[i].CommandChanged += OnCommandChanged;
            }

            if (_allowVisible)
            {
                Visible = true;
            }

            InitializeLabelLinks();
        }

        _optimalHeight = -1;
    }

    private void InitializeLabelLinks()
    {
        Label.Links.Clear();
        StringBuilder sb = new();
        Point[] links = new Point[_verbs!.Length];
        int index = 0;
        bool firstVerb = true;

        for (int i = 0; i < _verbs.Length; i++)
        {
            if (_verbs[i].Visible && _verbs[i].Supported)
            {
                if (!firstVerb)
                {
                    sb.Append(Application.CurrentCulture.TextInfo.ListSeparator);
                    sb.Append(' ');
                    index += 2;
                }

                string name = _verbs[i].Text;

                links[i] = new Point(index, name.Length);
                sb.Append(name);
                index += name.Length;
                firstVerb = false;
            }
        }

        Label.Text = sb.ToString();

        for (int i = 0; i < _verbs.Length; i++)
        {
            if (_verbs[i].Visible && _verbs[i].Supported)
            {
                LinkLabel.Link link = Label.Links.Add(links[i].X, links[i].Y, _verbs[i]);
                if (!_verbs[i].Enabled)
                {
                    link.Enabled = false;
                }
            }
        }
    }
}
