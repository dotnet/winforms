// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    [DefaultProperty(nameof(Items))]
    public class ToolStripComboBox : ToolStripControlHost
    {
        internal static readonly object s_eventDropDown = new object();
        internal static readonly object s_eventDropDownClosed = new object();
        internal static readonly object s_eventDropDownStyleChanged = new object();
        internal static readonly object s_eventSelectedIndexChanged = new object();
        internal static readonly object s_eventSelectionChangeCommitted = new object();
        internal static readonly object s_eventTextUpdate = new object();

        private static readonly Padding s_dropDownPadding = new Padding(2);
        private static readonly Padding s_padding = new Padding(1, 0, 1, 0);

        private Padding _scaledDropDownPadding = s_dropDownPadding;
        private Padding _scaledPadding = s_padding;

        public ToolStripComboBox() : base(CreateControlInstance())
        {
            ToolStripComboBoxControl combo = Control as ToolStripComboBoxControl;
            combo.Owner = this;

            if (DpiHelper.IsScalingRequirementMet)
            {
                _scaledPadding = DpiHelper.LogicalToDeviceUnits(s_padding);
                _scaledDropDownPadding = DpiHelper.LogicalToDeviceUnits(s_dropDownPadding);
            }
        }

        public ToolStripComboBox(string name) : this()
        {
            Name = name;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ToolStripComboBox(Control c) : base(c)
        {
            throw new NotSupportedException(SR.ToolStripMustSupplyItsOwnComboBox);
        }

        private static Control CreateControlInstance()
        {
            ComboBox comboBox = new ToolStripComboBoxControl
            {
                FlatStyle = FlatStyle.Popup,
                Font = ToolStripManager.DefaultFont
            };
            return comboBox;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ComboBoxAutoCompleteCustomSourceDescr))]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return ComboBox.AutoCompleteCustomSource; }
            set { ComboBox.AutoCompleteCustomSource = value; }
        }

        [DefaultValue(AutoCompleteMode.None)]
        [SRDescription(nameof(SR.ComboBoxAutoCompleteModeDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get { return ComboBox.AutoCompleteMode; }
            set { ComboBox.AutoCompleteMode = value; }
        }

        [DefaultValue(AutoCompleteSource.None)]
        [SRDescription(nameof(SR.ComboBoxAutoCompleteSourceDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteSource AutoCompleteSource
        {
            get { return ComboBox.AutoCompleteSource; }
            set { ComboBox.AutoCompleteSource = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ComboBox ComboBox
        {
            get
            {
                return Control as ComboBox;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 22);
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected internal override Padding DefaultMargin
        {
            get
            {
                if (IsOnDropDown)
                {
                    return _scaledDropDownPadding;
                }
                else
                {
                    return _scaledPadding;
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxOnDropDownDescr))]
        public event EventHandler DropDown
        {
            add => Events.AddHandler(s_eventDropDown, value);
            remove => Events.RemoveHandler(s_eventDropDown, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxOnDropDownClosedDescr))]
        public event EventHandler DropDownClosed
        {
            add => Events.AddHandler(s_eventDropDownClosed, value);
            remove => Events.RemoveHandler(s_eventDropDownClosed, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxDropDownStyleChangedDescr))]
        public event EventHandler DropDownStyleChanged
        {
            add => Events.AddHandler(s_eventDropDownStyleChanged, value);
            remove => Events.RemoveHandler(s_eventDropDownStyleChanged, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxDropDownHeightDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DefaultValue(106)]
        public int DropDownHeight
        {
            get { return ComboBox.DropDownHeight; }
            set { ComboBox.DropDownHeight = value; }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(ComboBoxStyle.DropDown)]
        [SRDescription(nameof(SR.ComboBoxStyleDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public ComboBoxStyle DropDownStyle
        {
            get { return ComboBox.DropDownStyle; }
            set { ComboBox.DropDownStyle = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxDropDownWidthDescr))]
        public int DropDownWidth
        {
            get { return ComboBox.DropDownWidth; }
            set { ComboBox.DropDownWidth = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ComboBoxDroppedDownDescr))]
        public bool DroppedDown
        {
            get { return ComboBox.DroppedDown; }
            set { ComboBox.DroppedDown = value; }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(FlatStyle.Popup)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ComboBoxFlatStyleDescr))]
        public FlatStyle FlatStyle
        {
            get { return ComboBox.FlatStyle; }
            set { ComboBox.FlatStyle = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ComboBoxIntegralHeightDescr))]
        public bool IntegralHeight
        {
            get { return ComboBox.IntegralHeight; }
            set { ComboBox.IntegralHeight = value; }
        }
        /// <summary>
        ///  Collection of the items contained in this ComboBox.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ComboBoxItemsDescr))]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        public ComboBox.ObjectCollection Items
        {
            get
            {
                return ComboBox.Items;
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(8)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ComboBoxMaxDropDownItemsDescr))]
        public int MaxDropDownItems
        {
            get { return ComboBox.MaxDropDownItems; }
            set { ComboBox.MaxDropDownItems = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(0)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ComboBoxMaxLengthDescr))]
        public int MaxLength
        {
            get { return ComboBox.MaxLength; }
            set { ComboBox.MaxLength = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ComboBoxSelectedIndexDescr))]
        public int SelectedIndex
        {
            get { return ComboBox.SelectedIndex; }
            set { ComboBox.SelectedIndex = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
        public event EventHandler SelectedIndexChanged
        {
            add => Events.AddHandler(s_eventSelectedIndexChanged, value);
            remove => Events.RemoveHandler(s_eventSelectedIndexChanged, value);
        }

        [Browsable(false)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ComboBoxSelectedItemDescr))]
        public object SelectedItem
        {
            get { return ComboBox.SelectedItem; }
            set { ComboBox.SelectedItem = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ComboBoxSelectedTextDescr))]
        public string SelectedText
        {
            get { return ComboBox.SelectedText; }
            set { ComboBox.SelectedText = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ComboBoxSelectionLengthDescr))]
        public int SelectionLength
        {
            get { return ComboBox.SelectionLength; }
            set { ComboBox.SelectionLength = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ComboBoxSelectionStartDescr))]
        public int SelectionStart
        {
            get { return ComboBox.SelectionStart; }
            set { ComboBox.SelectionStart = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ComboBoxSortedDescr))]
        public bool Sorted
        {
            get { return ComboBox.Sorted; }
            set { ComboBox.Sorted = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxOnTextUpdateDescr))]
        public event EventHandler TextUpdate
        {
            add => Events.AddHandler(s_eventTextUpdate, value);
            remove => Events.RemoveHandler(s_eventTextUpdate, value);
        }

        #region WrappedMethods

        public void BeginUpdate() { ComboBox.BeginUpdate(); }
        public void EndUpdate() { ComboBox.EndUpdate(); }
        public int FindString(string s) { return ComboBox.FindString(s); }
        public int FindString(string s, int startIndex) { return ComboBox.FindString(s, startIndex); }
        public int FindStringExact(string s) { return ComboBox.FindStringExact(s); }
        public int FindStringExact(string s, int startIndex) { return ComboBox.FindStringExact(s, startIndex); }
        public int GetItemHeight(int index) { return ComboBox.GetItemHeight(index); }
        public void Select(int start, int length) { ComboBox.Select(start, length); }
        public void SelectAll() { ComboBox.SelectAll(); }

        #endregion WrappedMethods

        public override Size GetPreferredSize(Size constrainingSize)
        {
            //
            Size preferredSize = base.GetPreferredSize(constrainingSize);
            preferredSize.Width = Math.Max(preferredSize.Width, 75);

            return preferredSize;
        }
        private void HandleDropDown(object sender, EventArgs e)
        {
            OnDropDown(e);
        }
        private void HandleDropDownClosed(object sender, EventArgs e)
        {
            OnDropDownClosed(e);
        }

        private void HandleDropDownStyleChanged(object sender, EventArgs e)
        {
            OnDropDownStyleChanged(e);
        }
        private void HandleSelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged(e);
        }
        private void HandleSelectionChangeCommitted(object sender, EventArgs e)
        {
            OnSelectionChangeCommitted(e);
        }
        private void HandleTextUpdate(object sender, EventArgs e)
        {
            OnTextUpdate(e);
        }

        protected virtual void OnDropDown(EventArgs e)
        {
            if (ParentInternal != null)
            {
                Application.ThreadContext.FromCurrent().RemoveMessageFilter(ParentInternal.RestoreFocusFilter);
                ToolStripManager.ModalMenuFilter.SuspendMenuMode();
            }
            RaiseEvent(s_eventDropDown, e);
        }
        protected virtual void OnDropDownClosed(EventArgs e)
        {
            if (ParentInternal != null)
            {
                // PERF,

                Application.ThreadContext.FromCurrent().RemoveMessageFilter(ParentInternal.RestoreFocusFilter);
                ToolStripManager.ModalMenuFilter.ResumeMenuMode();
            }
            RaiseEvent(s_eventDropDownClosed, e);
        }
        protected virtual void OnDropDownStyleChanged(EventArgs e)
        {
            RaiseEvent(s_eventDropDownStyleChanged, e);
        }
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            RaiseEvent(s_eventSelectedIndexChanged, e);
        }
        protected virtual void OnSelectionChangeCommitted(EventArgs e)
        {
            RaiseEvent(s_eventSelectionChangeCommitted, e);
        }
        protected virtual void OnTextUpdate(EventArgs e)
        {
            RaiseEvent(s_eventTextUpdate, e);
        }

        protected override void OnSubscribeControlEvents(Control control)
        {
            if (control is ComboBox comboBox)
            {
                // Please keep this alphabetized and in sync with Unsubscribe
                //
                comboBox.DropDown += new EventHandler(HandleDropDown);
                comboBox.DropDownClosed += new EventHandler(HandleDropDownClosed);
                comboBox.DropDownStyleChanged += new EventHandler(HandleDropDownStyleChanged);
                comboBox.SelectedIndexChanged += new EventHandler(HandleSelectedIndexChanged);
                comboBox.SelectionChangeCommitted += new EventHandler(HandleSelectionChangeCommitted);
                comboBox.TextUpdate += new EventHandler(HandleTextUpdate);
            }

            base.OnSubscribeControlEvents(control);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            if (control is ComboBox comboBox)
            {
                // Please keep this alphabetized and in sync with Unsubscribe
                //
                comboBox.DropDown -= new EventHandler(HandleDropDown);
                comboBox.DropDownClosed -= new EventHandler(HandleDropDownClosed);
                comboBox.DropDownStyleChanged -= new EventHandler(HandleDropDownStyleChanged);
                comboBox.SelectedIndexChanged -= new EventHandler(HandleSelectedIndexChanged);
                comboBox.SelectionChangeCommitted -= new EventHandler(HandleSelectionChangeCommitted);
                comboBox.TextUpdate -= new EventHandler(HandleTextUpdate);
            }
            base.OnUnsubscribeControlEvents(control);
        }

        private bool ShouldSerializeDropDownWidth()
        {
            return ComboBox.ShouldSerializeDropDownWidth();
        }

        internal override bool ShouldSerializeFont()
        {
            return !Equals(Font, ToolStripManager.DefaultFont);
        }

        public override string ToString()
        {
            return base.ToString() + ", Items.Count: " + Items.Count.ToString(CultureInfo.CurrentCulture);
        }

        internal class ToolStripComboBoxControl : ComboBox
        {
            public ToolStripComboBoxControl()
            {
                FlatStyle = FlatStyle.Popup;
                SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            }

            public ToolStripComboBox Owner { get; set; }

            private ProfessionalColorTable ColorTable
            {
                get
                {
                    if (Owner != null)
                    {
                        if (Owner.Renderer is ToolStripProfessionalRenderer renderer)
                        {
                            return renderer.ColorTable;
                        }
                    }
                    return ProfessionalColors.ColorTable;
                }
            }

            /// <summary>
            ///  Constructs the new instance of the accessibility object for this ToolStripComboBoxControl.
            /// </summary>
            /// <returns>
            ///  The new instance of the accessibility object for this ToolStripComboBoxControl item
            /// </returns>
            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new ToolStripComboBoxControlAccessibleObject(this);
            }

            internal override FlatComboAdapter CreateFlatComboAdapterInstance()
            {
                return new ToolStripComboBoxFlatComboAdapter(this);
            }

            internal class ToolStripComboBoxFlatComboAdapter : FlatComboAdapter
            {
                public ToolStripComboBoxFlatComboAdapter(ComboBox comboBox) : base(comboBox, smallButton: true)
                {
                }

                private static bool UseBaseAdapter(ComboBox comboBox)
                {
                    ToolStripComboBoxControl toolStripComboBox = comboBox as ToolStripComboBoxControl;
                    if (toolStripComboBox is null || !(toolStripComboBox.Owner.Renderer is ToolStripProfessionalRenderer))
                    {
                        Debug.Assert(toolStripComboBox != null, "Why are we here and not a toolstrip combo?");
                        return true;
                    }
                    return false;
                }

                private static ProfessionalColorTable GetColorTable(ToolStripComboBoxControl toolStripComboBoxControl)
                {
                    if (toolStripComboBoxControl != null)
                    {
                        return toolStripComboBoxControl.ColorTable;
                    }

                    return ProfessionalColors.ColorTable;
                }

                protected override Color GetOuterBorderColor(ComboBox comboBox)
                {
                    if (UseBaseAdapter(comboBox))
                    {
                        return base.GetOuterBorderColor(comboBox);
                    }
                    return (comboBox.Enabled) ? SystemColors.Window : GetColorTable(comboBox as ToolStripComboBoxControl).ComboBoxBorder;
                }

                protected override Color GetPopupOuterBorderColor(ComboBox comboBox, bool focused)
                {
                    if (UseBaseAdapter(comboBox))
                    {
                        return base.GetPopupOuterBorderColor(comboBox, focused);
                    }

                    if (!comboBox.Enabled)
                    {
                        return SystemColors.ControlDark;
                    }

                    return focused
                        ? GetColorTable(comboBox as ToolStripComboBoxControl).ComboBoxBorder
                        : SystemColors.Window;
                }

                protected override void DrawFlatComboDropDown(ComboBox comboBox, Graphics g, Rectangle dropDownRect)
                {
                    if (UseBaseAdapter(comboBox))
                    {
                        base.DrawFlatComboDropDown(comboBox, g, dropDownRect);
                        return;
                    }

                    if (!comboBox.Enabled || !ToolStripManager.VisualStylesEnabled)
                    {
                        g.FillRectangle(SystemBrushes.Control, dropDownRect);
                    }
                    else
                    {
                        ToolStripComboBoxControl toolStripComboBox = comboBox as ToolStripComboBoxControl;
                        ProfessionalColorTable colorTable = GetColorTable(toolStripComboBox);

                        if (!comboBox.DroppedDown)
                        {
                            bool focused = comboBox.ContainsFocus || comboBox.MouseIsOver;
                            if (focused)
                            {
                                using Brush b = new LinearGradientBrush(
                                    dropDownRect,
                                    colorTable.ComboBoxButtonSelectedGradientBegin,
                                    colorTable.ComboBoxButtonSelectedGradientEnd,
                                    LinearGradientMode.Vertical);

                                g.FillRectangle(b, dropDownRect);
                            }
                            else if (toolStripComboBox.Owner.IsOnOverflow)
                            {
                                using var b = colorTable.ComboBoxButtonOnOverflow.GetCachedSolidBrushScope();
                                g.FillRectangle(b, dropDownRect);
                            }
                            else
                            {
                                using Brush b = new LinearGradientBrush(
                                    dropDownRect,
                                    colorTable.ComboBoxButtonGradientBegin,
                                    colorTable.ComboBoxButtonGradientEnd,
                                    LinearGradientMode.Vertical);

                                g.FillRectangle(b, dropDownRect);
                            }
                        }
                        else
                        {
                            using Brush b = new LinearGradientBrush(
                                dropDownRect,
                                colorTable.ComboBoxButtonPressedGradientBegin,
                                colorTable.ComboBoxButtonPressedGradientEnd,
                                LinearGradientMode.Vertical);

                            g.FillRectangle(b, dropDownRect);
                        }
                    }

                    Brush brush;
                    if (comboBox.Enabled)
                    {
                        brush = SystemInformation.HighContrast
                            && (comboBox.ContainsFocus || comboBox.MouseIsOver)
                            && ToolStripManager.VisualStylesEnabled
                                ? SystemBrushes.HighlightText
                                : SystemBrushes.ControlText;
                    }
                    else
                    {
                        brush = SystemBrushes.GrayText;
                    }

                    Point middle = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);

                    // If the width is odd - favor pushing it over one pixel right.
                    middle.X += (dropDownRect.Width % 2);
                    g.FillPolygon(brush, new Point[] {
                        new Point(middle.X - FlatComboAdapter.s_offsetPixels, middle.Y - 1),
                        new Point(middle.X + FlatComboAdapter.s_offsetPixels + 1, middle.Y - 1),
                        new Point(middle.X, middle.Y + FlatComboAdapter.s_offsetPixels)
                    });
                }
            }

            protected override bool IsInputKey(Keys keyData)
            {
                if ((keyData & Keys.Alt) == Keys.Alt)
                {
                    switch (keyData & Keys.KeyCode)
                    {
                        case Keys.Down:
                        case Keys.Up:
                            return true;
                    }
                }

                return base.IsInputKey(keyData);
            }

            protected override void OnDropDownClosed(EventArgs e)
            {
                base.OnDropDownClosed(e);
                Invalidate();
                Update();
            }

            internal override bool SupportsUiaProviders => true;

            internal class ToolStripComboBoxControlAccessibleObject : ComboBoxAccessibleObject
            {
                private readonly ChildAccessibleObject _childAccessibleObject;

                public ToolStripComboBoxControlAccessibleObject(ToolStripComboBoxControl toolStripComboBoxControl)
                    : base(toolStripComboBoxControl)
                {
                    _childAccessibleObject = new ChildAccessibleObject(toolStripComboBoxControl, toolStripComboBoxControl.InternalHandle);
                }

                internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
                {
                    switch (direction)
                    {
                        case UiaCore.NavigateDirection.Parent:
                        case UiaCore.NavigateDirection.PreviousSibling:
                        case UiaCore.NavigateDirection.NextSibling:
                            if (Owner is ToolStripComboBoxControl toolStripComboBoxControl)
                            {
                                return toolStripComboBoxControl.Owner.AccessibilityObject.FragmentNavigate(direction);
                            }
                            break;
                    }

                    return base.FragmentNavigate(direction);
                }

                internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                {
                    get
                    {
                        if (Owner is ToolStripComboBoxControl toolStripComboBoxControl)
                        {
                            return toolStripComboBoxControl.Owner.Owner.AccessibilityObject;
                        }

                        return base.FragmentRoot;
                    }
                }

                internal override object GetPropertyValue(UiaCore.UIA propertyID)
                {
                    switch (propertyID)
                    {
                        case UiaCore.UIA.ControlTypePropertyId:
                            return UiaCore.UIA.ComboBoxControlTypeId;
                        case UiaCore.UIA.IsOffscreenPropertyId:
                            return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    }

                    return base.GetPropertyValue(propertyID);
                }

                internal override bool IsPatternSupported(UiaCore.UIA patternId)
                {
                    if (patternId == UiaCore.UIA.ExpandCollapsePatternId ||
                        patternId == UiaCore.UIA.ValuePatternId)
                    {
                        return true;
                    }

                    return base.IsPatternSupported(patternId);
                }
            }
        }
    }
}
