﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Docking(DockingBehavior.Never)]
    [Designer("System.Windows.Forms.Design.SplitterPanelDesigner, " + AssemblyRef.SystemDesign)]
    [ToolboxItem(false)]
    public sealed class SplitterPanel : Panel
    {
        public SplitterPanel(SplitContainer owner) : base()
        {
            Owner = owner;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        internal bool Collapsed { get; set; }

        /// <summary>
        ///  Gets or sets a value that indicates whether the <see cref="SplitterPanel" /> is automatically
        ///  resized to display its entire contents. This property is not relevant to this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        /// <summary>
        ///  Occurs when the value of the <see cref="AutoSize" /> property has changed.
        ///  This event is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Allows the control to optionally shrink when AutoSize is true.
        /// </summary>
        /// <value>One of the <see cref="AutoSizeMode" /> values. </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Localizable(false)]
        public override AutoSizeMode AutoSizeMode
        {
            get => AutoSizeMode.GrowOnly;
            set { }
        }

        /// <summary>
        ///  Override Anchor to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new AnchorStyles Anchor
        {
            get => base.Anchor;
            set => base.Anchor = value;
        }

        /// <summary>
        ///  Indicates what type of border the Splitter control has.  This value
        ///  comes from the System.Windows.Forms.BorderStyle enumeration.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new BorderStyle BorderStyle
        {
            get => base.BorderStyle;
            set => base.BorderStyle = value;
        }

        /// <summary>
        ///  Gets or sets which edge of the <see cref="SplitContainer" /> that the
        ///  <see cref="SplitterPanel" /> is docked to.
        ///  This property is not relevant to this class.
        /// </summary>
        /// <value>
        ///  One of the enumeration values that specifies which edge of the <see cref="SplitContainer" />
        ///  that the <see cref="SplitterPanel" /> is docked to.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new DockStyle Dock
        {
            get => base.Dock;
            set => base.Dock = value;
        }

        /// <summary>
        ///  Override DockPadding to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new DockPaddingEdges DockPadding
        {
            get => base.DockPadding;
        }

        /// <summary>
        ///  The height of this SplitterPanel
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ControlHeightDescr))]
        public new int Height
        {
            get => Collapsed ? 0 : base.Height;
            set => throw new NotSupportedException(SR.SplitContainerPanelHeight);
        }

        internal int HeightInternal
        {
            get => ((Panel)this).Height;
            set => ((Panel)this).Height = value;
        }

        /// <summary>
        ///  Override Location to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Point Location
        {
            get => base.Location;
            set => base.Location = value;
        }

        /// <summary>
        ///  Gets the default space, in pixels, that separates the controls.
        /// </summary>
        /// <value>
        ///  A new instance of the <see cref="Padding" /> struct with the padding set to 0 for all edges.
        /// </value>
        protected override Padding DefaultMargin => new Padding(0, 0, 0, 0);

        /// <summary>
        ///  Override AutoSize to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Size MinimumSize
        {
            get => base.MinimumSize;
            set => base.MinimumSize = value;
        }

        /// <summary>
        ///  Override AutoSize to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Size MaximumSize
        {
            get => base.MaximumSize;
            set => base.MaximumSize = value;
        }

        /// <summary>
        ///  The name of this <see cref="SplitterPanel" />.
        ///  This property is not relevant to this class.
        /// </summary>
        /// <value>The name of this <see cref="SplitterPanel" />.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new string Name
        {
            get => base.Name;
            set => base.Name = value;
        }

        /// <summary>
        ///  The parent of this control.
        /// </summary>
        internal SplitContainer Owner { get; }

        /// <summary>
        ///  Gets or sets the <see cref="SplitContainer" /> that contains this <see cref="SplitterPanel" />.
        ///  This property is not relevant to this class.
        /// </summary>
        /// <value>
        ///  A control representing the <see cref="SplitContainer" /> that contains this <see cref="SplitterPanel" />.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Control Parent
        {
            get => base.Parent;
            set => base.Parent = value;
        }

        /// <summary>
        ///  Override Size to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Size Size
        {
            get => Collapsed ? Size.Empty : base.Size;
            set => base.Size = value;
        }

        /// <summary>
        ///  Override TabIndex to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new int TabIndex
        {
            get => base.TabIndex;
            set => base.TabIndex = value;
        }

        /// <summary>
        ///  Gets or sets a value that indicates whether the user can give the focus to this
        ///  <see cref="SplitterPanel" /> using the TAB key.
        ///  This property is not relevant to this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        /// <summary>
        ///  Gets or sets a value that indicates whether the <see cref="SplitterPanel" /> is displayed.
        ///  This property is not relevant to this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new bool Visible
        {
            get => base.Visible;
            set => base.Visible = value;
        }

        /// <summary>
        ///  The width of this control.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ControlWidthDescr))]
        public new int Width
        {
            get => Collapsed ? 0 : base.Width;
            set => throw new NotSupportedException(SR.SplitContainerPanelWidth);
        }

        internal int WidthInternal
        {
            get => base.Width;
            set => base.Width = value;
        }

        /// <summary>
        ///  Override VisibleChanged to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new event EventHandler VisibleChanged
        {
            add => base.VisibleChanged += value;
            remove => base.VisibleChanged -= value;
        }

        /// <summary>
        ///  Override DockChanged to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new event EventHandler DockChanged
        {
            add => base.DockChanged += value;
            remove => base.DockChanged -= value;
        }

        /// <summary>
        ///  Override LocationChanged to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new event EventHandler LocationChanged
        {
            add => base.LocationChanged += value;
            remove => base.LocationChanged -= value;
        }

        /// <summary>
        ///  Override TabIndexChanged to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new event EventHandler TabIndexChanged
        {
            add => base.TabIndexChanged += value;
            remove => base.TabIndexChanged -= value;
        }

        /// <summary>
        ///  Override TabStopChanged to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }
    }
}
