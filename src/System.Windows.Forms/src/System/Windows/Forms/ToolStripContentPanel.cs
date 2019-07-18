// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.Windows.Forms.Design.ToolStripContentPanelDesigner, " + AssemblyRef.SystemDesign),
    DefaultEvent(nameof(Load)),
    Docking(DockingBehavior.Never),
    InitializationEvent(nameof(Load)),
    ToolboxItem(false)
    ]
    public class ToolStripContentPanel : Panel
    {
        private ToolStripRendererSwitcher rendererSwitcher = null;
        private BitVector32 state = new BitVector32();
        private static readonly int stateLastDoubleBuffer = BitVector32.CreateMask();

        private static readonly object EventRendererChanged = new object();
        private static readonly object EventLoad = new object();

        public ToolStripContentPanel()
        {
            // Consider: OptimizedDoubleBuffer
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        ///  Allows the control to optionally shrink when AutoSize is true.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        Localizable(false)
        ]
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

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }

        [
       Browsable(false),
       EditorBrowsable(EditorBrowsableState.Never)
       ]
        public override bool AutoScroll
        {
            get { return base.AutoScroll; }
            set { base.AutoScroll = value; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Size AutoScrollMargin
        {
            get { return base.AutoScrollMargin; }
            set { base.AutoScrollMargin = value; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Size AutoScrollMinSize
        {
            get { return base.AutoScrollMinSize; }
            set { base.AutoScrollMinSize = value; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

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

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool CausesValidation
        {
            get { return base.CausesValidation; }
            set { base.CausesValidation = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler CausesValidationChanged
        {
            add => base.CausesValidationChanged += value;
            remove => base.CausesValidationChanged -= value;
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = value;
            }
        }
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new event EventHandler DockChanged
        {
            add => base.DockChanged += value;
            remove => base.DockChanged -= value;
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ToolStripContentPanelOnLoadDescr))]
        public event EventHandler Load
        {
            add => Events.AddHandler(EventLoad, value);
            remove => Events.RemoveHandler(EventLoad, value);
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Point Location
        {
            get { return base.Location; }
            set { base.Location = value; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new event EventHandler LocationChanged
        {
            add => base.LocationChanged += value;
            remove => base.LocationChanged -= value;
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override Size MinimumSize
        {
            get { return base.MinimumSize; }
            set { base.MinimumSize = value; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override Size MaximumSize
        {
            get { return base.MaximumSize; }
            set { base.MaximumSize = value; }
        }

        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public new string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            set
            {
                base.TabIndex = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TabIndexChanged
        {
            add => base.TabIndexChanged += value;
            remove => base.TabIndexChanged -= value;
        }

        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

        private ToolStripRendererSwitcher RendererSwitcher
        {
            get
            {
                if (rendererSwitcher == null)
                {
                    rendererSwitcher = new ToolStripRendererSwitcher(this, ToolStripRenderMode.System);
                    HandleRendererChanged(this, EventArgs.Empty);
                    rendererSwitcher.RendererChanged += new EventHandler(HandleRendererChanged);
                }
                return rendererSwitcher;
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

        [
        SRDescription(nameof(SR.ToolStripRenderModeDescr)),
        SRCategory(nameof(SR.CatAppearance)),
        ]
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

        [SRCategory(nameof(SR.CatAppearance)), SRDescription(nameof(SR.ToolStripRendererChanged))]
        public event EventHandler RendererChanged
        {
            add => Events.AddHandler(EventRendererChanged, value);
            remove => Events.RemoveHandler(EventRendererChanged, value);
        }

        private void HandleRendererChanged(object sender, EventArgs e)
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
            ((EventHandler)Events[EventLoad])?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            ToolStripContentPanelRenderEventArgs rea = new ToolStripContentPanelRenderEventArgs(e.Graphics, this);

            Renderer.DrawToolStripContentPanelBackground(rea);

            if (!rea.Handled)
            {
                base.OnPaintBackground(e);
            }

        }

        protected virtual void OnRendererChanged(EventArgs e)
        {
            // we dont want to be greedy.... if we're using TSProfessionalRenderer go DBuf, else dont.
            if (Renderer is ToolStripProfessionalRenderer)
            {
                state[stateLastDoubleBuffer] = DoubleBuffered;
                //this.DoubleBuffered = true;
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            }
            else
            {
                // restore DBuf
                DoubleBuffered = state[stateLastDoubleBuffer];
            }

            Renderer.InitializeContentPanel(this);

            Invalidate();

            ((EventHandler)Events[EventRendererChanged])?.Invoke(this, e);
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
}

