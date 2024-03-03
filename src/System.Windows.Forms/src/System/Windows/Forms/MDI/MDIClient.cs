// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Represents the container for multiple-document interface (MDI) child forms.
///  This class cannot be inherited.
/// </summary>
/// <remarks>
///  <para>
///   Don't create an <see cref="MdiClient"/> control. A form creates and uses the <see cref="MdiClient"/> when you set
///   the <see cref="Form.IsMdiContainer"/> property to <see langword="true"/>.
///  </para>
/// </remarks>
[ToolboxItem(false)]
[DesignTimeVisible(false)]
public sealed partial class MdiClient : Control
{
    // Kept in add order, not ZOrder. Need to return the correct array of items.
    private readonly List<Form> _children = [];

    /// <summary>
    ///  Creates a new MdiClient.
    /// </summary>
    public MdiClient() : base()
    {
        SetStyle(ControlStyles.Selectable, false);
        BackColor = Application.SystemColors.AppWorkspace;
        Dock = DockStyle.Fill;
    }

    /// <summary>
    ///  Gets or sets the background image displayed in the <see cref="MdiClient" /> control.
    /// </summary>
    /// <value>The image to display in the background of the control.</value>
    [Localizable(true)]
    public override Image? BackgroundImage
    {
        get
        {
            Image? result = base.BackgroundImage;
            if (result is null && ParentInternal is not null)
            {
                result = ParentInternal.BackgroundImage;
            }

            return result;
        }
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get
        {
            Image? backgroundImage = BackgroundImage;
            if (backgroundImage is not null && ParentInternal is not null)
            {
                ImageLayout imageLayout = base.BackgroundImageLayout;
                if (imageLayout != ParentInternal.BackgroundImageLayout)
                {
                    // if the Layout is set on the parent use that.
                    return ParentInternal.BackgroundImageLayout;
                }
            }

            return base.BackgroundImageLayout;
        }
        set => base.BackgroundImageLayout = value;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;

            cp.ClassName = "MDICLIENT";

            // Note: Don't set the MDIS_ALLCHILDSTYLES CreatParams.Style bit, it prevents an MDI child form from getting
            // activated when made visible (no WM_MDIACTIVATE sent to it), and forcing activation on it changes the
            // activation event sequence (MdiChildActivate/Enter/Focus/Activate/etc.).
            cp.Style |= (int)(WINDOW_STYLE.WS_VSCROLL | WINDOW_STYLE.WS_HSCROLL);
            cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
            cp.Param = new CLIENTCREATESTRUCT
            {
                idFirstChild = 1
            };

            ISite? site = ParentInternal?.Site;
            if (site is not null && site.DesignMode)
            {
                cp.Style |= (int)WINDOW_STYLE.WS_DISABLED;
                SetState(States.Enabled, false);
            }

            if (RightToLeft == RightToLeft.Yes && ParentInternal is not null && ParentInternal.IsMirrored)
            {
                // We want to turn on mirroring for MdiClient explicitly.
                cp.ExStyle |= (int)(WINDOW_EX_STYLE.WS_EX_LAYOUTRTL | WINDOW_EX_STYLE.WS_EX_NOINHERITLAYOUT);
                // Don't need these styles when mirroring is turned on.
                cp.ExStyle &= ~(int)(WINDOW_EX_STYLE.WS_EX_RTLREADING | WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR);
            }

            return cp;
        }
    }

    /// <summary>
    ///  The list of MDI children contained. This list will be sorted by the order in which the children were
    ///  added to the form, not the current ZOrder.
    /// </summary>
    public Form[] MdiChildren
    {
        get
        {
            return [.. _children];
        }
    }

    protected override Control.ControlCollection CreateControlsInstance()
    {
        return new ControlCollection(this);
    }

    /// <summary>
    ///  Arranges the multiple-document interface (MDI) child forms within the MDI parent form.
    /// </summary>
    /// <param name="value">One of the enumeration values that defines the layout of MDI child forms.</param>
    public void LayoutMdi(MdiLayout value)
    {
        if (Handle == IntPtr.Zero)
        {
            return;
        }

        switch (value)
        {
            case MdiLayout.Cascade:
                PInvoke.SendMessage(this, PInvoke.WM_MDICASCADE);
                break;
            case MdiLayout.TileVertical:
                PInvoke.SendMessage(this, PInvoke.WM_MDITILE, (WPARAM)(uint)TILE_WINDOWS_HOW.MDITILE_VERTICAL);
                break;
            case MdiLayout.TileHorizontal:
                PInvoke.SendMessage(this, PInvoke.WM_MDITILE, (WPARAM)(uint)TILE_WINDOWS_HOW.MDITILE_HORIZONTAL);
                break;
            case MdiLayout.ArrangeIcons:
                PInvoke.SendMessage(this, PInvoke.WM_MDIICONARRANGE);
                break;
        }
    }

    /// <summary>
    ///  Raises the <see cref="Control.Resize" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnResize(EventArgs e)
    {
        ISite? site = ParentInternal?.Site;
        if (site is not null && site.DesignMode && Handle != IntPtr.Zero)
        {
            SetWindowRgn();
        }

        base.OnResize(e);
    }

    /// <summary>
    ///  Scales the entire control and any child controls.
    /// </summary>
    /// <param name="dx">The scaling factor for the x-axis</param>
    /// <param name="dy">The scaling factor for the y-axis.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void ScaleCore(float dx, float dy)
    {
        // Don't scale child forms.

        SuspendLayout();
        try
        {
            Rectangle bounds = Bounds;
            int sx = (int)Math.Round(bounds.X * dx);
            int sy = (int)Math.Round(bounds.Y * dy);
            int sw = (int)Math.Round((bounds.X + bounds.Width) * dx - sx);
            int sh = (int)Math.Round((bounds.Y + bounds.Height) * dy - sy);
            SetBounds(sx, sy, sw, sh, BoundsSpecified.All);
        }
        finally
        {
            ResumeLayout();
        }
    }

    /// <summary>
    ///  Scales this form's location, size, padding, and margin. The <see cref="MdiClient" /> overrides
    ///  <see cref="ScaleControl(SizeF,BoundsSpecified)" /> to enforce a minimum and maximum size.
    /// </summary>
    /// <param name="factor">The factor by which the height and width of the control will be scaled.</param>
    /// <param name="specified">The bounds of the control to use when defining its size and position.</param>
    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        // never scale X and Y of an MDI client form
        specified &= ~BoundsSpecified.Location;
        base.ScaleControl(factor, specified);
    }

    protected override unsafe void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (!IsHandleCreated
            || ParentInternal is Form { MdiChildrenMinimizedAnchorBottom: false }
            || ParentInternal?.Site?.DesignMode == true)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            return;
        }

        Rectangle oldBounds = Bounds;
        base.SetBoundsCore(x, y, width, height, specified);
        Rectangle newBounds = Bounds;

        int yDelta = oldBounds.Height - newBounds.Height;
        if (yDelta == 0)
        {
            return;
        }

        // NOTE: This logic is to keep minimized MDI children anchored to
        // the bottom left of the client area, normally they are anchored
        // to the top left which just looks weird!
        for (int i = 0; i < Controls.Count; i++)
        {
            // Only adjust the window position for visible MDI Child windows to prevent
            // them from being re-displayed.
            if (Controls[i] is Form child && child.CanRecreateHandle() && child.WindowState == FormWindowState.Minimized)
            {
                WINDOWPLACEMENT wp = new()
                {
                    length = (uint)sizeof(WINDOWPLACEMENT)
                };

                bool result = PInvoke.GetWindowPlacement(child.HWND, &wp);
                Debug.Assert(result);
                wp.ptMinPosition.Y -= yDelta;
                if (wp.ptMinPosition.Y == -1)
                {
                    if (yDelta < 0)
                    {
                        wp.ptMinPosition.Y = 0;
                    }
                    else
                    {
                        wp.ptMinPosition.Y = -2;
                    }
                }

                wp.flags = WINDOWPLACEMENT_FLAGS.WPF_SETMINPOSITION;
                PInvoke.SetWindowPlacement(child.HWND, &wp);

                GC.KeepAlive(child);
            }
        }
    }

    /// <summary>
    ///  This code is required to set the correct window region during the resize of the Form at design time.
    ///  There is case when the form contains a MainMenu and also has IsMdiContainer property set, in which, the MdiClient fails to
    ///  resize and hence draw the correct backcolor.
    /// </summary>
    private void SetWindowRgn()
    {
        RECT rect = default;
        CreateParams cp = CreateParams;

        AdjustWindowRectExForControlDpi(ref rect, (WINDOW_STYLE)cp.Style, false, (WINDOW_EX_STYLE)cp.ExStyle);

        Rectangle bounds = Bounds;
        using RegionScope rgn1 = new(0, 0, bounds.Width, bounds.Height);
        using RegionScope rgn2 = new(
            -rect.left,
            -rect.top,
            bounds.Width - rect.right,
            bounds.Height - rect.bottom);

        if (rgn1.IsNull || rgn2.IsNull)
        {
            throw new InvalidOperationException(SR.ErrorSettingWindowRegion);
        }

        if (PInvokeCore.CombineRgn(rgn1, rgn1, rgn2, RGN_COMBINE_MODE.RGN_DIFF) == GDI_REGION_TYPE.RGN_ERROR)
        {
            throw new InvalidOperationException(SR.ErrorSettingWindowRegion);
        }

        if (PInvoke.SetWindowRgn(this, rgn1, true) == 0)
        {
            throw new InvalidOperationException(SR.ErrorSettingWindowRegion);
        }
        else
        {
            // The hwnd now owns the region.
            rgn1.RelinquishOwnership();
        }
    }

    internal override bool ShouldSerializeBackColor()
    {
        return BackColor != Application.SystemColors.AppWorkspace;
    }

    private static bool ShouldSerializeLocation() => false;

    internal override bool ShouldSerializeSize() => false;

    /// <summary>
    ///  Processes Windows messages.
    /// </summary>
    /// <param name="m">The Windows <see cref="Message" /> to process.</param>
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvoke.WM_CREATE:
                if (ParentInternal is not null && ParentInternal.Site is not null && ParentInternal.Site.DesignMode && Handle != IntPtr.Zero)
                {
                    SetWindowRgn();
                }

                break;

            case PInvoke.WM_SETFOCUS:
                InvokeGotFocus(ParentInternal, EventArgs.Empty);
                Form? childForm = null;
                if (ParentInternal is Form parentInternalAsForm)
                {
                    childForm = parentInternalAsForm.ActiveMdiChildInternal;
                }

                if (childForm is null && MdiChildren.Length > 0 && MdiChildren[0].IsMdiChildFocusable)
                {
                    childForm = MdiChildren[0];
                }

                if (childForm is not null && childForm.Visible)
                {
                    childForm.Active = true;
                }

                // Do not use control's implementation of WmSetFocus
                // as it will improperly activate this control.
                WmImeSetFocus();
                DefWndProc(ref m);
                InvokeGotFocus(this, EventArgs.Empty);
                return;
            case PInvoke.WM_KILLFOCUS:
                InvokeLostFocus(ParentInternal, EventArgs.Empty);
                break;
        }

        base.WndProc(ref m);
    }

    internal override void OnInvokedSetScrollPosition(object? sender, EventArgs e)
    {
        Application.Idle += new EventHandler(OnIdle); // do this on idle (it must be mega-delayed).
    }

    private void OnIdle(object? sender, EventArgs e)
    {
        Application.Idle -= new EventHandler(OnIdle);
        base.OnInvokedSetScrollPosition(sender, e);
    }
}
