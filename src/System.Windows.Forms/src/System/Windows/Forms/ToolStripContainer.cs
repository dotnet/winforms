// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms
{
    [Designer("System.Windows.Forms.Design.ToolStripContainerDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.ToolStripContainerDesc))]
    public class ToolStripContainer : ContainerControl
    {
        private readonly ToolStripPanel topPanel;
        private readonly ToolStripPanel bottomPanel;
        private readonly ToolStripPanel leftPanel;
        private readonly ToolStripPanel rightPanel;
        private readonly ToolStripContentPanel contentPanel;

        public ToolStripContainer()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            SuspendLayout();
            try
            {
                // undone - smart demand creation
                topPanel = new ToolStripPanel(this);
                bottomPanel = new ToolStripPanel(this);
                leftPanel = new ToolStripPanel(this);
                rightPanel = new ToolStripPanel(this);
                contentPanel = new ToolStripContentPanel
                {
                    Dock = DockStyle.Fill
                };
                topPanel.Dock = DockStyle.Top;
                bottomPanel.Dock = DockStyle.Bottom;
                rightPanel.Dock = DockStyle.Right;
                leftPanel.Dock = DockStyle.Left;

                if (Controls is ToolStripContainerTypedControlCollection controlCollection)
                {
                    controlCollection.AddInternal(contentPanel);
                    controlCollection.AddInternal(leftPanel);
                    controlCollection.AddInternal(rightPanel);
                    controlCollection.AddInternal(topPanel);
                    controlCollection.AddInternal(bottomPanel);
                }
                // else consider throw new exception
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        public new Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler BackColorChanged
        {
            add => base.BackColorChanged += value;
            remove => base.BackColorChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged += value;
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerBottomToolStripPanelDescr))]
        [Localizable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripPanel BottomToolStripPanel
        {
            get
            {
                return bottomPanel;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerBottomToolStripPanelVisibleDescr))]
        [DefaultValue(true)]
        public bool BottomToolStripPanelVisible
        {
            get
            {
                return BottomToolStripPanel.Visible;
            }
            set
            {
                BottomToolStripPanel.Visible = value;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerContentPanelDescr))]
        [Localizable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripContentPanel ContentPanel
        {
            get
            {
                return contentPanel;
            }
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
        public new event EventHandler CausesValidationChanged
        {
            add => base.CausesValidationChanged += value;
            remove => base.CausesValidationChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ContextMenuStrip ContextMenuStrip
        {
            get => base.ContextMenuStrip;
            set => base.ContextMenuStrip = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ContextMenuStripChanged
        {
            add => base.ContextMenuStripChanged += value;
            remove => base.ContextMenuStripChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Cursor Cursor
        {
            get => base.Cursor;
            set => base.Cursor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler CursorChanged
        {
            add => base.CursorChanged += value;
            remove => base.CursorChanged -= value;
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(150, 175);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler ForeColorChanged
        {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerLeftToolStripPanelDescr))]
        [Localizable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripPanel LeftToolStripPanel
        {
            get
            {
                return leftPanel;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerLeftToolStripPanelVisibleDescr))]
        [DefaultValue(true)]
        public bool LeftToolStripPanelVisible
        {
            get
            {
                return LeftToolStripPanel.Visible;
            }
            set
            {
                LeftToolStripPanel.Visible = value;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerRightToolStripPanelDescr))]
        [Localizable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripPanel RightToolStripPanel
        {
            get
            {
                return rightPanel;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerRightToolStripPanelVisibleDescr))]
        [DefaultValue(true)]
        public bool RightToolStripPanelVisible
        {
            get
            {
                return RightToolStripPanel.Visible;
            }
            set
            {
                RightToolStripPanel.Visible = value;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerTopToolStripPanelDescr))]
        [Localizable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripPanel TopToolStripPanel
        {
            get
            {
                return topPanel;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripContainerTopToolStripPanelVisibleDescr))]
        [DefaultValue(true)]
        public bool TopToolStripPanelVisible
        {
            get
            {
                return TopToolStripPanel.Visible;
            }
            set
            {
                TopToolStripPanel.Visible = value;
            }
        }

        /// <summary>
        ///  Controls Collection...
        ///  This is overriden so that the Controls.Add ( ) is not Code Gened...
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ControlCollection Controls
        {
            get => base.Controls;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override ControlCollection CreateControlsInstance()
        {
            return new ToolStripContainerTypedControlCollection(this, /*isReadOnly*/true);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            RightToLeft rightToLeft = RightToLeft;

            // no need to suspend layout - we're already in a layout transaction.
            if (rightToLeft == RightToLeft.Yes)
            {
                RightToolStripPanel.Dock = DockStyle.Left;
                LeftToolStripPanel.Dock = DockStyle.Right;
            }
            else
            {
                RightToolStripPanel.Dock = DockStyle.Right;
                LeftToolStripPanel.Dock = DockStyle.Left;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            foreach (Control c in Controls)
            {
                c.SuspendLayout();
            }
            base.OnSizeChanged(e);
            foreach (Control c in Controls)
            {
                c.ResumeLayout();
            }
        }

        internal override void RecreateHandleCore()
        {
            //If ToolStripContainer's Handle is getting created demand create the childControl handle's
            if (IsHandleCreated)
            {
                foreach (Control c in Controls)
                {
                    c.CreateControl(true);
                }
            }
            base.RecreateHandleCore();
        }

        internal override bool AllowsKeyboardToolTip()
        {
            return false;
        }

        internal class ToolStripContainerTypedControlCollection : ReadOnlyControlCollection
        {
            readonly ToolStripContainer owner;
            readonly Type contentPanelType = typeof(ToolStripContentPanel);
            readonly Type panelType = typeof(ToolStripPanel);

            public ToolStripContainerTypedControlCollection(Control c, bool isReadOnly)
                : base(c, isReadOnly)
            {
                owner = c as ToolStripContainer;
            }

            public override void Add(Control value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (IsReadOnly)
                {
                    throw new NotSupportedException(SR.ToolStripContainerUseContentPanel);
                }

                Type controlType = value.GetType();
                if (!contentPanelType.IsAssignableFrom(controlType) && !panelType.IsAssignableFrom(controlType))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, string.Format(SR.TypedControlCollectionShouldBeOfTypes, contentPanelType.Name, panelType.Name)), value.GetType().Name);
                }
                base.Add(value);
            }
            public override void Remove(Control value)
            {
                if (value is ToolStripPanel || value is ToolStripContentPanel)
                {
                    if (!owner.DesignMode)
                    {
                        if (IsReadOnly)
                        {
                            throw new NotSupportedException(SR.ReadonlyControlsCollection);
                        }
                    }
                }
                base.Remove(value);
            }

            internal override void SetChildIndexInternal(Control child, int newIndex)
            {
                if (child is ToolStripPanel || child is ToolStripContentPanel)
                {
                    if (!owner.DesignMode)
                    {
                        if (IsReadOnly)
                        {
                            throw new NotSupportedException(SR.ReadonlyControlsCollection);
                        }
                    }
                    else
                    {
                        // just no-op it at DT.
                        return;
                    }
                }
                base.SetChildIndexInternal(child, newIndex);
            }
        }
    }
}
