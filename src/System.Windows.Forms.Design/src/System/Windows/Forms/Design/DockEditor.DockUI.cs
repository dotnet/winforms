// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design
{
    public sealed partial class DockEditor
    {
        /// <summary>
        ///  User Interface for the DockEditor.
        /// </summary>
        private class DockUI : Control
        {
            private const int NoneHeight = 24;
            private const int NoneWidth = 90;
            private const int ControlWidth = 94;
            private const int ControlHeight = 116;
            private const int Offset2X = 2;
            private const int Offset2Y = 2;

            private static bool s_isScalingInitialized;
            private static readonly Size s_buttonSizeDefault = new(20, 20);
            private static readonly Size s_containerSizeDefault = new(90, 90);

            private static int s_noneHeight = NoneHeight;
            private static int s_noneWidth = NoneWidth;
            private static Size s_buttonSize = s_buttonSizeDefault;
            private static Size s_containerSize = s_containerSizeDefault;
            private static int s_controlWidth = ControlWidth;
            private static int s_controlHeight = ControlHeight;
            private static int s_offset2X = Offset2X;
            private static int s_offset2Y = Offset2Y;

            private bool _allowExit = true;

            private readonly ContainerPlaceholder _container = new();
            private readonly DockEditor _editor;
            private IWindowsFormsEditorService _edSvc;
            private readonly RadioButton _fill = new DockEditorRadioButton();
            private readonly RadioButton _left = new DockEditorRadioButton();
            private readonly RadioButton[] _leftRightOrder;
            private readonly RadioButton _none = new DockEditorRadioButton();
            private readonly RadioButton _right = new DockEditorRadioButton();
            private readonly RadioButton[] _tabOrder;
            private readonly RadioButton _top = new DockEditorRadioButton();
            private readonly RadioButton _bottom = new DockEditorRadioButton();
            private readonly RadioButton[] _upDownOrder;
            private RadioButton _checkedControl;

            public DockUI(DockEditor editor)
            {
                this._editor = editor;
                _upDownOrder = new[] { _top, _fill, _bottom, _none };
                _leftRightOrder = new[] { _left, _fill, _right };
                _tabOrder = new[] { _top, _left, _fill, _right, _bottom, _none };

                if (!s_isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        s_noneHeight = DpiHelper.LogicalToDeviceUnitsY(NoneHeight);
                        s_noneWidth = DpiHelper.LogicalToDeviceUnitsX(NoneWidth);
                        s_controlHeight = DpiHelper.LogicalToDeviceUnitsY(ControlHeight);
                        s_controlWidth = DpiHelper.LogicalToDeviceUnitsX(ControlWidth);
                        s_offset2Y = DpiHelper.LogicalToDeviceUnitsY(Offset2Y);
                        s_offset2X = DpiHelper.LogicalToDeviceUnitsX(Offset2X);

                        s_buttonSize = DpiHelper.LogicalToDeviceUnits(s_buttonSizeDefault);
                        s_containerSize = DpiHelper.LogicalToDeviceUnits(s_containerSizeDefault);
                    }

                    s_isScalingInitialized = true;
                }

                InitializeComponent();
            }

            private RadioButton CheckedControl
            {
                get => _checkedControl;
                set
                {
                    _checkedControl = value;
                    FocusCheckedControl();
                }
            }

            private DockStyle DockStyle
            {
                get
                {
                    if (ReferenceEquals(CheckedControl, _fill))
                    {
                        return DockStyle.Fill;
                    }

                    if (ReferenceEquals(CheckedControl, _left))
                    {
                        return DockStyle.Left;
                    }

                    if (ReferenceEquals(CheckedControl, _right))
                    {
                        return DockStyle.Right;
                    }

                    if (ReferenceEquals(CheckedControl, _top))
                    {
                        return DockStyle.Top;
                    }

                    if (ReferenceEquals(CheckedControl, _bottom))
                    {
                        return DockStyle.Bottom;
                    }

                    return DockStyle.None;
                }
                set
                {
                    switch (value)
                    {
                        case DockStyle.None:
                            CheckedControl = _none;
                            break;
                        case DockStyle.Fill:
                            CheckedControl = _fill;
                            break;
                        case DockStyle.Left:
                            CheckedControl = _left;
                            break;
                        case DockStyle.Right:
                            CheckedControl = _right;
                            break;
                        case DockStyle.Top:
                            CheckedControl = _top;
                            break;
                        case DockStyle.Bottom:
                            CheckedControl = _bottom;
                            break;
                    }
                }
            }

            public object Value { get; private set; }

            public void End()
            {
                _edSvc = null;
                Value = null;
            }

            private void FocusCheckedControl()
            {
                // To actually move focus to a radio button, we need to call Focus() method.
                // However, that would raise OnClick event, which would close the editor.
                // We set allowExit to false, to block editor exit, on radio button selection change.
                _allowExit = false;
                CheckedControl.Focus();
                _allowExit = true;
            }

            public virtual DockStyle GetDock(RadioButton btn)
            {
                if (_top == btn)
                {
                    return DockStyle.Top;
                }

                if (_left == btn)
                {
                    return DockStyle.Left;
                }

                if (_bottom == btn)
                {
                    return DockStyle.Bottom;
                }

                if (_right == btn)
                {
                    return DockStyle.Right;
                }

                if (_fill == btn)
                {
                    return DockStyle.Fill;
                }

                return DockStyle.None;
            }

            private void InitializeComponent()
            {
                SetBounds(0, 0, s_controlWidth, s_controlHeight);

                BackColor = SystemColors.Control;
                ForeColor = SystemColors.ControlText;
                AccessibleName = SR.DockEditorAccName;

                _none.Dock = DockStyle.Bottom;
                _none.Size = new Size(s_noneWidth, s_noneHeight);
                _none.Text = DockStyle.None.ToString();
                _none.TabIndex = 0;
                _none.TabStop = true;
                _none.Appearance = Appearance.Button;
                _none.Click += OnClick;
                _none.KeyDown += OnKeyDown;
                _none.AccessibleName = SR.DockEditorNoneAccName;

                _container.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                _container.Location = new Point(s_offset2X, s_offset2Y);
                _container.Size = s_containerSize;

                _none.Dock = DockStyle.Bottom;
                _container.Dock = DockStyle.Fill;

                _right.Dock = DockStyle.Right;
                _right.Size = s_buttonSize;
                _right.TabIndex = 4;
                _right.TabStop = true;
                _right.Text = " "; // Needs at least one character so focus rect will show.
                _right.Appearance = Appearance.Button;
                _right.Click += OnClick;
                _right.KeyDown += OnKeyDown;
                _right.AccessibleName = SR.DockEditorRightAccName;

                _left.Dock = DockStyle.Left;
                _left.Size = s_buttonSize;
                _left.TabIndex = 2;
                _left.TabStop = true;
                _left.Text = " ";
                _left.Appearance = Appearance.Button;
                _left.Click += OnClick;
                _left.KeyDown += OnKeyDown;
                _left.AccessibleName = SR.DockEditorLeftAccName;

                _top.Dock = DockStyle.Top;
                _top.Size = s_buttonSize;
                _top.TabIndex = 1;
                _top.TabStop = true;
                _top.Text = " ";
                _top.Appearance = Appearance.Button;
                _top.Click += OnClick;
                _top.KeyDown += OnKeyDown;
                _top.AccessibleName = SR.DockEditorTopAccName;

                _bottom.Dock = DockStyle.Bottom;
                _bottom.Size = s_buttonSize;
                _bottom.TabIndex = 5;
                _bottom.TabStop = true;
                _bottom.Text = " ";
                _bottom.Appearance = Appearance.Button;
                _bottom.Click += OnClick;
                _bottom.KeyDown += OnKeyDown;
                _bottom.AccessibleName = SR.DockEditorBottomAccName;

                _fill.Dock = DockStyle.Fill;
                _fill.Size = s_buttonSize;
                _fill.TabIndex = 3;
                _fill.TabStop = true;
                _fill.Text = " ";
                _fill.Appearance = Appearance.Button;
                _fill.Click += OnClick;
                _fill.KeyDown += OnKeyDown;
                _fill.AccessibleName = SR.DockEditorFillAccName;

                Controls.Clear();
                Controls.Add(_container);

                _container.Controls.Clear();
                _container.Controls.AddRange(new Control[]
                {
                    _fill,
                    _left,
                    _right,
                    _top,
                    _bottom,
                    _none
                });
            }

            private void OnClick(object sender, EventArgs e)
            {
                if (_allowExit)
                {
                    CheckedControl = (RadioButton)sender;
                    Value = DockStyle;
                    Teardown();
                }
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                FocusCheckedControl();
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
                        if (sender == _left || sender == _right)
                        {
                            sender = _fill;
                        }

                        maxI = _upDownOrder.Length - 1;
                        for (int i = 0; i <= maxI; i++)
                        {
                            if (_upDownOrder[i] == sender)
                            {
                                target = key == Keys.Up
                                    ? _upDownOrder[Math.Max(i - 1, 0)]
                                    : _upDownOrder[Math.Min(i + 1, maxI)];

                                break;
                            }
                        }

                        break;
                    case Keys.Left:
                    case Keys.Right:
                        maxI = _leftRightOrder.Length - 1;
                        for (int i = 0; i <= maxI; i++)
                        {
                            if (_leftRightOrder[i] == sender)
                            {
                                target = key == Keys.Left
                                    ? _leftRightOrder[Math.Max(i - 1, 0)]
                                    : _leftRightOrder[Math.Min(i + 1, maxI)];

                                break;
                            }
                        }

                        break;

                    case Keys.Return:
                        InvokeOnClick((RadioButton)sender, EventArgs.Empty); // Will tear down editor
                        return;
                    default:
                        return; // Unhandled keys return here
                }

                e.Handled = true;

                if (target is RadioButton targetButton && target != sender)
                {
                    CheckedControl = targetButton;
                }
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                if ((keyData & Keys.KeyCode) == Keys.Tab && (keyData & (Keys.Alt | Keys.Control)) == 0)
                {
                    for (int i = 0; i < _tabOrder.Length; i++)
                    {
                        if (_tabOrder[i] == CheckedControl)
                        {
                            i += (keyData & Keys.Shift) == 0 ? 1 : -1;
                            i = i < 0 ? i + _tabOrder.Length : i % _tabOrder.Length;
                            CheckedControl = _tabOrder[i];
                            break;
                        }
                    }

                    return true;
                }

                return base.ProcessDialogKey(keyData);
            }

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                _none.Checked = false;
                _top.Checked = false;
                _left.Checked = false;
                _right.Checked = false;
                _bottom.Checked = false;
                _fill.Checked = false;

                this._edSvc = edSvc;
                Value = value;

                DockStyle = value is DockStyle dockStyle ? dockStyle : DockStyle.None;
                CheckedControl.Checked = true;
            }
            
            private void Teardown()
            {
                _edSvc.CloseDropDown();
            }

            private class DockEditorRadioButton : RadioButton
            {
                public DockEditorRadioButton()
                {
                    AutoCheck = false;
                }

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
