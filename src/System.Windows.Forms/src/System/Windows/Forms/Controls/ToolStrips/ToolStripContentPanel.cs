// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[Designer($"System.Windows.Forms.Design.ToolStripContentPanelDesigner, {AssemblyRef.SystemDesign}")]
[DefaultEvent(nameof(Load))]
[Docking(DockingBehavior.Never)]
[InitializationEvent(nameof(Load))]
[ToolboxItem(false)]
public class ToolStripContentPanel : Panel
{
    private ToolStripRendererSwitcher? _rendererSwitcher;
    private BitVector32 _state;
    private static readonly int s_stateLastDoubleBuffer = BitVector32.CreateMask();

    private static readonly object s_rendererChangedEvent = new();
    private static readonly object s_loadEvent = new();

    public ToolStripContentPanel()
    {
        // Consider: OptimizedDoubleBuffer
        SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
    }

    /// <summary>
    ///  Allows the control to optionally shrink when AutoSize is true.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    [Localizable(false)]
    public override AutoSizeMode AutoSizeMode
    {
        get
        {
            return AutoSizeMode.GrowOnly;
        }
        set
        {
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override AnchorStyles Anchor
    {
        get => base.Anchor;
        set => base.Anchor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool AutoScroll
    {
        get => base.AutoScroll;
        set => base.AutoScroll = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMargin
    {
        get => base.AutoScrollMargin;
        set => base.AutoScrollMargin = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMinSize
    {
        get => base.AutoScrollMinSize;
        set => base.AutoScrollMinSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    public override Color BackColor
    {
        get => base.BackColor;

        set
        {
            // To support transparency on ToolStripContainer, we need this check
            // to ensure that background color of the container reflects the
            // ContentPanel
            if (ParentInternal is ToolStripContainer && value == Color.Transparent)
            {
                ParentInternal.BackColor = Color.Transparent;
            }

            base.BackColor = value;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool CausesValidation
    {
        get => base.CausesValidation;
        set => base.CausesValidation = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? CausesValidationChanged
    {
        add => base.CausesValidationChanged += value;
        remove => base.CausesValidationChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override DockStyle Dock
    {
        get => base.Dock;
        set => base.Dock = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? DockChanged
    {
        add => base.DockChanged += value;
        remove => base.DockChanged -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolStripContentPanelOnLoadDescr))]
    public event EventHandler? Load
    {
        add => Events.AddHandler(s_loadEvent, value);
        remove => Events.RemoveHandler(s_loadEvent, value);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Point Location
    {
        get => base.Location;
        set => base.Location = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? LocationChanged
    {
        add => base.LocationChanged += value;
        remove => base.LocationChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Size MinimumSize
    {
        get => base.MinimumSize;
        set => base.MinimumSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Size MaximumSize
    {
        get => base.MaximumSize;
        set => base.MaximumSize = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    [AllowNull]
    public new string Name
    {
        get => base.Name;
        set => base.Name = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new int TabIndex
    {
        get => base.TabIndex;
        set => base.TabIndex = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabIndexChanged
    {
        add => base.TabIndexChanged += value;
        remove => base.TabIndexChanged -= value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    private ToolStripRendererSwitcher RendererSwitcher
    {
        get
        {
            if (_rendererSwitcher is null)
            {
                _rendererSwitcher = new ToolStripRendererSwitcher(this, ToolStripRenderMode.System);
                HandleRendererChanged(this, EventArgs.Empty);
                _rendererSwitcher.RendererChanged += HandleRendererChanged;
            }

            return _rendererSwitcher;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ToolStripRenderer Renderer
    {
        get
        {
            return RendererSwitcher.Renderer;
        }
        set
        {
            RendererSwitcher.Renderer = value;
        }
    }

    [SRDescription(nameof(SR.ToolStripRenderModeDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public ToolStripRenderMode RenderMode
    {
        get
        {
            return RendererSwitcher.RenderMode;
        }
        set
        {
            RendererSwitcher.RenderMode = value;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripRendererChanged))]
    public event EventHandler? RendererChanged
    {
        add => Events.AddHandler(s_rendererChangedEvent, value);
        remove => Events.RemoveHandler(s_rendererChangedEvent, value);
    }

    private void HandleRendererChanged(object? sender, EventArgs e)
    {
        OnRendererChanged(e);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!RecreatingHandle)
        {
            OnLoad(EventArgs.Empty);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnLoad(EventArgs e)
    {
        // There is no good way to explain this event except to say
        // that it's just another name for OnControlCreated.
        ((EventHandler?)Events[s_loadEvent])?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        ToolStripContentPanelRenderEventArgs rea = new(e.Graphics, this);

        Renderer.DrawToolStripContentPanelBackground(rea);

        if (!rea.Handled)
        {
            base.OnPaintBackground(e);
        }
    }

    protected virtual void OnRendererChanged(EventArgs e)
    {
        // we don't want to be greedy.... if we're using TSProfessionalRenderer go DBuf, else don't.
        if (Renderer is ToolStripProfessionalRenderer)
        {
            _state[s_stateLastDoubleBuffer] = DoubleBuffered;
            // this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
        else
        {
            // restore DBuf
            DoubleBuffered = _state[s_stateLastDoubleBuffer];
        }

        Renderer.InitializeContentPanel(this);

        Invalidate();

        ((EventHandler?)Events[s_rendererChangedEvent])?.Invoke(this, e);
    }

    private void ResetRenderMode()
    {
        RendererSwitcher.ResetRenderMode();
    }

    private bool ShouldSerializeRenderMode()
    {
        return RendererSwitcher.ShouldSerializeRenderMode();
    }
}
