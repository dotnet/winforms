// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
public partial class ToolStripButton : ToolStripItem
{
    private CheckState _checkState = CheckState.Unchecked;
    private CheckState _prevCheckState = CheckState.Unchecked;
    private const int LogicalStandardButtonWidth = 23;
    private int _standardButtonWidth = LogicalStandardButtonWidth;

    private static readonly object s_checkedChangedEvent = new();
    private static readonly object s_checkStateChangedEvent = new();

    public ToolStripButton()
    {
        Initialize();
    }

    public ToolStripButton(string? text)
        : base(text, image: null, onClick: null)
    {
        Initialize();
    }

    public ToolStripButton(Image? image)
        : base(text: null, image, onClick: null)
    {
        Initialize();
    }

    public ToolStripButton(string? text, Image? image)
        : base(text, image, onClick: null)
    {
        Initialize();
    }

    public ToolStripButton(string? text, Image? image, EventHandler? onClick)
        : base(text, image, onClick)
    {
        Initialize();
    }

    public ToolStripButton(string? text, Image? image, EventHandler? onClick, string? name)
        : base(text, image, onClick, name)
    {
        Initialize();
    }

    [DefaultValue(true)]
    public new bool AutoToolTip
    {
        get => base.AutoToolTip;
        set => base.AutoToolTip = value;
    }

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolStripButtonCheckOnClickDescr))]
    public bool CheckOnClick { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the item is checked.
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripButtonCheckedDescr))]
    public bool Checked
    {
        get => _checkState != CheckState.Unchecked;
        set
        {
            if (value != Checked)
            {
                CheckState = value ? CheckState.Checked : CheckState.Unchecked;
                InvokePaint();
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the check box is checked.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(CheckState.Unchecked)]
    [SRDescription(nameof(SR.CheckBoxCheckStateDescr))]
    public CheckState CheckState
    {
        get => _checkState;
        set
        {
            if (value is < CheckState.Unchecked or > CheckState.Indeterminate)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(CheckState));
            }

            if (value != _checkState)
            {
                _prevCheckState = _checkState;
                _checkState = value;
                Invalidate();
                OnCheckedChanged(EventArgs.Empty);
                OnCheckStateChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Occurs when the value of the <see cref="CheckBox.Checked"/> property changes.
    /// </summary>
    [SRDescription(nameof(SR.CheckBoxOnCheckedChangedDescr))]
    public event EventHandler? CheckedChanged
    {
        add => Events.AddHandler(s_checkedChangedEvent, value);
        remove => Events.RemoveHandler(s_checkedChangedEvent, value);
    }

    /// <summary>
    ///  Occurs when the value of the <see cref="CheckBox.CheckState"/> property changes.
    /// </summary>
    [SRDescription(nameof(SR.CheckBoxOnCheckStateChangedDescr))]
    public event EventHandler? CheckStateChanged
    {
        add => Events.AddHandler(s_checkStateChangedEvent, value);
        remove => Events.RemoveHandler(s_checkStateChangedEvent, value);
    }

    protected override bool DefaultAutoToolTip => true;

    /// <remarks>
    ///  <para>
    ///   This gets called via ToolStripItem.RescaleConstantsForDpi.
    ///   It's practically calling Initialize on DpiChanging with the new Dpi value.
    ///  </para>
    /// </remarks>
    internal override int DeviceDpi
    {
        get => base.DeviceDpi;
        set
        {
            if (base.DeviceDpi != value)
            {
                base.DeviceDpi = value;
                _standardButtonWidth = ScaleHelper.ScaleToDpi(LogicalStandardButtonWidth, DeviceDpi);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override AccessibleObject CreateAccessibilityInstance()
        => new ToolStripButtonAccessibleObject(this);

    public override Size GetPreferredSize(Size constrainingSize)
    {
        Size prefSize = base.GetPreferredSize(constrainingSize);
        prefSize.Width = Math.Max(prefSize.Width, _standardButtonWidth);
        return prefSize;
    }

    /// <summary>
    ///  Called by all constructors of ToolStripButton.
    /// </summary>
    private void Initialize()
    {
        SupportsSpaceKey = true;
        _standardButtonWidth = ScaleHelper.ScaleToInitialSystemDpi(LogicalStandardButtonWidth);
    }

    /// <summary>
    ///  Raises the <see cref="ToolStripMenuItem.CheckedChanged"/> event.
    /// </summary>
    protected virtual void OnCheckedChanged(EventArgs e)
        => ((EventHandler?)Events[s_checkedChangedEvent])?.Invoke(this, e);

    /// <summary>
    ///  Raises the <see cref="ToolStripMenuItem.CheckStateChanged"/> event.
    /// </summary>
    protected virtual void OnCheckStateChanged(EventArgs e)
    {
        AccessibilityNotifyClients(AccessibleEvents.StateChange);

        if (IsAccessibilityObjectCreated &&
            AccessibilityObject is ToolStripButtonAccessibleObject accessibilityObject)
        {
            accessibilityObject.OnCheckStateChanged(_prevCheckState, _checkState);
        }

        ((EventHandler?)Events[s_checkStateChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (Owner is null)
        {
            return;
        }

        ToolStripRenderer renderer = Renderer!;
        renderer.DrawButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

        if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
        {
            ToolStripItemImageRenderEventArgs rea = new(e.Graphics, this, InternalLayout.ImageRectangle)
            {
                ShiftOnPress = true
            };
            renderer.DrawItemImage(rea);
        }

        if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
        {
            renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, InternalLayout.TextRectangle, ForeColor, Font, InternalLayout.TextFormat));
        }
    }

    protected override void OnClick(EventArgs e)
    {
        if (CheckOnClick)
        {
            Checked = !Checked;
        }

        base.OnClick(e);
    }

    protected internal override bool ProcessDialogKey(Keys keyData)
    {
        // If this button is top-level checkable button and Space key is pressed,
        // then handle it as a click, but do not unselect the button or switch focus
        // as base implementation of this method does,
        // to allow user to toggle this button again (imitate checkbox behavior)
        if (keyData == Keys.Space && SupportsSpaceKey && CheckOnClick && Enabled && !IsOnDropDown)
        {
            FireEvent(ToolStripItemEventType.Click);
            return true;
        }

        return base.ProcessDialogKey(keyData);
    }
}
