// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

public sealed partial class DockEditor
{
    /// <summary>
    ///  User Interface for the DockEditor.
    /// </summary>
    private sealed class DockUI : SelectionPanelBase
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

        private readonly ContainerPlaceholder _container = new();
        private readonly DockEditor _editor;
        private readonly RadioButton _fill = new SelectionPanelRadioButton();
        private readonly RadioButton _left = new SelectionPanelRadioButton();
        private readonly RadioButton[] _leftRightOrder;
        private readonly RadioButton _none = new SelectionPanelRadioButton();
        private readonly RadioButton _right = new SelectionPanelRadioButton();
        private readonly RadioButton[] _tabOrder;
        private readonly RadioButton _top = new SelectionPanelRadioButton();
        private readonly RadioButton _bottom = new SelectionPanelRadioButton();
        private readonly RadioButton[] _upDownOrder;

        public DockUI(DockEditor editor)
        {
            _editor = editor;
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

        private DockStyle DockStyle
        {
            get
            {
                if (CheckedControl == _fill)
                {
                    return DockStyle.Fill;
                }

                if (CheckedControl == _left)
                {
                    return DockStyle.Left;
                }

                if (CheckedControl == _right)
                {
                    return DockStyle.Right;
                }

                if (CheckedControl == _top)
                {
                    return DockStyle.Top;
                }

                if (CheckedControl == _bottom)
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

        protected override ControlCollection SelectionOptions => _container.Controls;

        private void InitializeComponent()
        {
            SetBounds(0, 0, s_controlWidth, s_controlHeight);

            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            AccessibleName = SR.DockEditorAccName;

            _none.Dock = DockStyle.Bottom;
            _none.Size = new Size(s_noneWidth, s_noneHeight);
            _none.Name = "_none";
            _none.Text = DockStyle.None.ToString();
            _none.TabIndex = 0;
            _none.TabStop = true;
            _none.Appearance = Appearance.Button;
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
            _right.Name = "_right";
            _right.Text = " "; // Needs at least one character so focus rect will show.
            _right.Appearance = Appearance.Button;
            _right.AccessibleName = SR.DockEditorRightAccName;

            _left.Dock = DockStyle.Left;
            _left.Size = s_buttonSize;
            _left.TabIndex = 2;
            _left.TabStop = true;
            _left.Name = "_left";
            _left.Text = " ";
            _left.Appearance = Appearance.Button;
            _left.AccessibleName = SR.DockEditorLeftAccName;

            _top.Dock = DockStyle.Top;
            _top.Size = s_buttonSize;
            _top.TabIndex = 1;
            _top.TabStop = true;
            _top.Name = "_top";
            _top.Text = " ";
            _top.Appearance = Appearance.Button;
            _top.AccessibleName = SR.DockEditorTopAccName;

            _bottom.Dock = DockStyle.Bottom;
            _bottom.Size = s_buttonSize;
            _bottom.TabIndex = 5;
            _bottom.TabStop = true;
            _bottom.Name = "_bottom";
            _bottom.Text = " ";
            _bottom.Appearance = Appearance.Button;
            _bottom.AccessibleName = SR.DockEditorBottomAccName;

            _fill.Dock = DockStyle.Fill;
            _fill.Size = s_buttonSize;
            _fill.TabIndex = 3;
            _fill.TabStop = true;
            _fill.Name = "_fill";
            _fill.Text = " ";
            _fill.Appearance = Appearance.Button;
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

            ConfigureButtons();
        }

        protected override RadioButton ProcessDownKey(RadioButton checkedControl) => ProcessUpDown(checkedControl, false);

        protected override RadioButton ProcessLeftKey(RadioButton checkedControl) => ProcessLeftRight(checkedControl, true);

        private RadioButton ProcessLeftRight(RadioButton checkedControl, bool leftDirection)
        {
            int maxI = _leftRightOrder.Length - 1;
            for (int i = 0; i <= maxI; i++)
            {
                if (_leftRightOrder[i] == checkedControl)
                {
                    return leftDirection
                        ? _leftRightOrder[Math.Max(i - 1, 0)]
                        : _leftRightOrder[Math.Min(i + 1, maxI)];
                }
            }

            return checkedControl;
        }

        protected override RadioButton ProcessRightKey(RadioButton checkedControl) => ProcessLeftRight(checkedControl, false);

        protected override RadioButton ProcessTabKey(Keys keyData)
        {
            for (int i = 0; i < _tabOrder.Length; i++)
            {
                if (_tabOrder[i] == CheckedControl)
                {
                    i += (keyData & Keys.Shift) == 0 ? 1 : -1;
                    i = i < 0 ? i + _tabOrder.Length : i % _tabOrder.Length;
                    return _tabOrder[i];
                }
            }

            return CheckedControl;
        }

        protected override RadioButton ProcessUpKey(RadioButton checkedControl) => ProcessUpDown(checkedControl, true);

        private RadioButton ProcessUpDown(RadioButton checkedControl, bool upDirection)
        {
            // If we're going up or down from one of the 'sides', act like we're doing
            // it from the center
            if (checkedControl == _left || checkedControl == _right)
            {
                checkedControl = _fill;
            }

            int maxI = _upDownOrder.Length - 1;
            for (int i = 0; i <= maxI; i++)
            {
                if (_upDownOrder[i] == checkedControl)
                {
                    return upDirection
                        ? _upDownOrder[Math.Max(i - 1, 0)]
                        : _upDownOrder[Math.Min(i + 1, maxI)];
                }
            }

            return checkedControl;
        }

        protected override void SetInitialCheckedControl() =>
            DockStyle = Value is DockStyle dockStyle ? dockStyle : DockStyle.None;

        protected override void UpdateValue() => Value = DockStyle;

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
