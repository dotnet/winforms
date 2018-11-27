// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design.Editors.Resources;

namespace System.Windows.Forms.Design
{
    /// <summary>>
    ///         Implements the design time editor for specifying the
    ///         <see cref='System.Windows.Forms.Control.Dock' /> property.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DockEditor : UITypeEditor
    {
        private DockUI dockUI;

        /// <summary>
        ///     Edits the given object value using the editor style provided by
        ///     GetEditorStyle.  A service provider is provided so that any
        ///     required editing services can be obtained.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        // everything in this assembly is full trust.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    if (dockUI == null) dockUI = new DockUI(this);
                    dockUI.Start(edSvc, value);
                    edSvc.DropDownControl(dockUI);
                    value = dockUI.Value;
                    dockUI.End();
                }
            }

            return value;
        }

        /// <summary>
        ///     Retrieves the editing style of the Edit method.  If the method
        ///     is not supported, this will return None.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        // everything in this assembly is full trust.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        ///     User Interface for the DockEditor.
        /// </summary>

        private class DockUI : Control
        {
            private const int NONE_HEIGHT = 24;
            private const int NONE_WIDTH = 90;
            private const int CONTROL_WIDTH = 94;
            private const int CONTROL_HEIGHT = 116;
            private const int OFFSET2X = 2;
            private const int OFFSET2Y = 2;
            private const int NONE_Y = 94;

            private static bool isScalingInitialized;
            private static readonly Size buttonSizeDefault = new Size(20, 20);
            private static readonly Size containerSizeDefault = new Size(90, 90);

            private static int noneHeight = NONE_HEIGHT;
            private static int noneWidth = NONE_WIDTH;
            private static Size buttonSize = buttonSizeDefault;
            private static Size containerSize = containerSizeDefault;
            private static int controlWidth = CONTROL_WIDTH;
            private static int controlHeight = CONTROL_HEIGHT;
            private static int offset2X = OFFSET2X;
            private static int offset2Y = OFFSET2Y;
            private static int noneY = NONE_Y;

            private readonly CheckBox bottom = new DockEditorCheckBox();

            // Even though the selections are mutually exclusive, I'm using
            // CheckBoxes instead of RadioButtons because RadioButtons fire Click
            // events whenever they get focus, which is bad when the user is trying
            // to tab to a specific control using the keyboard.
            private readonly ContainerPlaceholder container = new ContainerPlaceholder();
            private DockEditor editor;
            private IWindowsFormsEditorService edSvc;
            private readonly CheckBox fill = new DockEditorCheckBox();
            private readonly CheckBox left = new DockEditorCheckBox();
            private readonly CheckBox[] leftRightOrder;
            private readonly CheckBox none = new DockEditorCheckBox();
            private readonly CheckBox right = new DockEditorCheckBox();
            private readonly CheckBox[] tabOrder;
            private readonly CheckBox top = new DockEditorCheckBox();
            private readonly CheckBox[] upDownOrder;

            public DockUI(DockEditor editor)
            {
                this.editor = editor;
                upDownOrder = new[] { top, fill, bottom, none };
                leftRightOrder = new[] { left, fill, right };
                tabOrder = new[] { top, left, fill, right, bottom, none };
                
                if (!isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        noneHeight = DpiHelper.LogicalToDeviceUnitsY(NONE_HEIGHT);
                        noneWidth = DpiHelper.LogicalToDeviceUnitsX(NONE_WIDTH);
                        controlHeight = DpiHelper.LogicalToDeviceUnitsY(CONTROL_HEIGHT);
                        controlWidth = DpiHelper.LogicalToDeviceUnitsX(CONTROL_WIDTH);
                        offset2Y = DpiHelper.LogicalToDeviceUnitsY(OFFSET2Y);
                        offset2X = DpiHelper.LogicalToDeviceUnitsX(OFFSET2X);
                        noneY = DpiHelper.LogicalToDeviceUnitsY(NONE_Y);

                        buttonSize = DpiHelper.LogicalToDeviceUnits(buttonSizeDefault);
                        containerSize = DpiHelper.LogicalToDeviceUnits(containerSizeDefault);
                    }

                    isScalingInitialized = true;
            }

                InitializeComponent();
            }

            public object Value { get; private set; }

            public void End()
            {
                edSvc = null;
                Value = null;
            }

            public virtual DockStyle GetDock(CheckBox btn)
            {
                if (top == btn)
                    return DockStyle.Top;
                if (left == btn)
                    return DockStyle.Left;
                if (bottom == btn)
                    return DockStyle.Bottom;
                if (right == btn)
                    return DockStyle.Right;
                if (fill == btn) return DockStyle.Fill;
                return DockStyle.None;
            }

            [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
            private void InitializeComponent()
            {
                SetBounds(0, 0, controlWidth, controlHeight);

                BackColor = SystemColors.Control;
                ForeColor = SystemColors.ControlText;
                AccessibleName = SR.DockEditorAccName;

                none.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                none.Location = new Point(offset2X, noneY);
                none.Size = new Size(noneWidth, noneHeight);
                none.Text = DockStyle.None.ToString();
                none.TabIndex = 0;
                none.TabStop = true;
                none.Appearance = Appearance.Button;
                none.Click += OnClick;
                none.KeyDown += OnKeyDown;
                none.AccessibleName = SR.DockEditorNoneAccName;

                container.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                container.Location = new Point(offset2X, offset2Y);
                container.Size = containerSize;

                none.Dock = DockStyle.Bottom;
                container.Dock = DockStyle.Fill;

                right.Dock = DockStyle.Right;
                right.Size = buttonSize;
                right.TabIndex = 4;
                right.TabStop = true;
                right.Text = " "; // Needs at least one character so focus rect will show.
                right.Appearance = Appearance.Button;
                right.Click += OnClick;
                right.KeyDown += OnKeyDown;
                right.AccessibleName = SR.DockEditorRightAccName;

                left.Dock = DockStyle.Left;
                left.Size = buttonSize;
                left.TabIndex = 2;
                left.TabStop = true;
                left.Text = " ";
                left.Appearance = Appearance.Button;
                left.Click += OnClick;
                left.KeyDown += OnKeyDown;
                left.AccessibleName = SR.DockEditorLeftAccName;

                top.Dock = DockStyle.Top;
                top.Size = buttonSize;
                top.TabIndex = 1;
                top.TabStop = true;
                top.Text = " ";
                top.Appearance = Appearance.Button;
                top.Click += OnClick;
                top.KeyDown += OnKeyDown;
                top.AccessibleName = SR.DockEditorTopAccName;

                bottom.Dock = DockStyle.Bottom;
                bottom.Size = buttonSize;
                bottom.TabIndex = 5;
                bottom.TabStop = true;
                bottom.Text = " ";
                bottom.Appearance = Appearance.Button;
                bottom.Click += OnClick;
                bottom.KeyDown += OnKeyDown;
                bottom.AccessibleName = SR.DockEditorBottomAccName;

                fill.Dock = DockStyle.Fill;
                fill.Size = buttonSize;
                fill.TabIndex = 3;
                fill.TabStop = true;
                fill.Text = " ";
                fill.Appearance = Appearance.Button;
                fill.Click += OnClick;
                fill.KeyDown += OnKeyDown;
                fill.AccessibleName = SR.DockEditorFillAccName;

                Controls.Clear();
                Controls.AddRange(new Control[]
                {
                    container,
                    none
                });

                container.Controls.Clear();
                container.Controls.AddRange(new Control[]
                {
                    fill,
                    left,
                    right,
                    top,
                    bottom
                });
            }

            private void OnClick(object sender, EventArgs eventargs)
            {
                DockStyle val = GetDock((CheckBox)sender);
                if (val >= 0) Value = val;
                Teardown();
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);

                // Set focus to currently selected Dock style
                for (int i = 0; i < tabOrder.Length; i++)
                    if (tabOrder[i].Checked)
                    {
                        tabOrder[i].Focus();
                        break;
                    }
            }

            private void OnKeyDown(object sender, KeyEventArgs e)
            {
                Keys key = e.KeyCode;
                Control target = null;
                int maxI;

                switch (key)
                {
                    case Keys.Up:
                    case Keys.Down:
                        // If we're going up or down from one of the 'sides', act like we're doing
                        // it from the center
                        if (sender == left || sender == right)
                            sender = fill;

                        maxI = upDownOrder.Length - 1;
                        for (int i = 0; i <= maxI; i++)
                            if (upDownOrder[i] == sender)
                            {
                                if (key == Keys.Up)
                                    target = upDownOrder[Math.Max(i - 1, 0)];
                                else
                                    target = upDownOrder[Math.Min(i + 1, maxI)];
                                break;
                            }

                        break;
                    case Keys.Left:
                    case Keys.Right:
                        maxI = leftRightOrder.Length - 1;
                        for (int i = 0; i <= maxI; i++)
                            if (leftRightOrder[i] == sender)
                            {
                                if (key == Keys.Left)
                                    target = leftRightOrder[Math.Max(i - 1, 0)];
                                else
                                    target = leftRightOrder[Math.Min(i + 1, maxI)];
                                break;
                            }

                        break;
                    case Keys.Tab:
                        for (int i = 0; i < tabOrder.Length; i++)
                            if (tabOrder[i] == sender)
                            {
                                i += (e.Modifiers & Keys.Shift) == 0 ? 1 : -1;
                                i = i < 0 ? i + tabOrder.Length : i % tabOrder.Length;
                                target = tabOrder[i];
                                break;
                            }

                        break;
                    case Keys.Return:
                        InvokeOnClick((CheckBox)sender, EventArgs.Empty); // Will tear down editor
                        return;
                    default:
                        return; // Unhandled keys return here
                }

                e.Handled = true;

                if (target != null && target != sender) target.Focus();
            }

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                this.edSvc = edSvc;
                Value = value;

                if (value is DockStyle)
                {
                    DockStyle dock = (DockStyle)value;

                    none.Checked = false;
                    top.Checked = false;
                    left.Checked = false;
                    right.Checked = false;
                    bottom.Checked = false;
                    fill.Checked = false;

                    switch (dock)
                    {
                        case DockStyle.None:
                            none.Checked = true;
                            break;
                        case DockStyle.Top:
                            top.Checked = true;
                            break;
                        case DockStyle.Left:
                            left.Checked = true;
                            break;
                        case DockStyle.Right:
                            right.Checked = true;
                            break;
                        case DockStyle.Bottom:
                            bottom.Checked = true;
                            break;
                        case DockStyle.Fill:
                            fill.Checked = true;
                            break;
                    }
                }
            }

            private void Teardown()
            {
                edSvc.CloseDropDown();
            }

            private class DockEditorCheckBox : CheckBox
            {
                protected override bool ShowFocusCues => true;

                protected override bool IsInputKey(Keys keyData)
                {
                    switch (keyData)
                    {
                        case Keys.Left:
                        case Keys.Right:
                        case Keys.Up:
                        case Keys.Down:
                        case Keys.Return:
                            return true;
                    }

                    return base.IsInputKey(keyData);
                }
            }

            private class ContainerPlaceholder : Control
            {
                public ContainerPlaceholder()
                {
                    BackColor = SystemColors.Control;
                    TabStop = false;
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                    Rectangle rc = ClientRectangle;
                    ControlPaint.DrawButton(e.Graphics, rc, ButtonState.Pushed);
                }
            }
        }
    }
}
