// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

public sealed partial class DockEditor
{
    /// <summary>
    ///  User interface for the DockEditor.
    /// </summary>
    private sealed class DockUI : SelectionPanelBase
    {
        private static int s_initialSystemDpi = -1;

        private readonly ContainerPlaceholder _container;
        private readonly RadioButton _fill;
        private readonly RadioButton _left;
        private readonly RadioButton[] _leftRightOrder;
        private readonly RadioButton _none;
        private readonly RadioButton _right;
        private readonly RadioButton[] _tabOrder;
        private readonly RadioButton _top;
        private readonly RadioButton _bottom;
        private readonly RadioButton[] _upDownOrder;

        public DockUI()
        {
            if (s_initialSystemDpi == -1)
            {
                s_initialSystemDpi = ScaleHelper.InitialSystemDpi;
            }

            _container = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                Dock = DockStyle.Fill
            };

            _none = new SelectionPanelRadioButton()
            {
                Dock = DockStyle.Bottom,
                Name = "_none",
                Text = DockStyle.None.ToString(),
                TabIndex = 0,
                TabStop = true,
                Appearance = Appearance.Button,
                AccessibleName = SR.DockEditorNoneAccName
            };

            _right = new SelectionPanelRadioButton()
            {
                Dock = DockStyle.Right,
                TabIndex = 4,
                TabStop = true,
                Name = "_right",
                // Needs at least one character so focus rect will show.
                Text = " ",
                Appearance = Appearance.Button,
                AccessibleName = SR.DockEditorRightAccName
            };

            _left = new SelectionPanelRadioButton()
            {
                Dock = DockStyle.Left,
                TabIndex = 2,
                TabStop = true,
                Name = "_left",
                Text = " ",
                Appearance = Appearance.Button,
                AccessibleName = SR.DockEditorLeftAccName
            };

            _top = new SelectionPanelRadioButton()
            {
                Dock = DockStyle.Top,
                TabIndex = 1,
                TabStop = true,
                Name = "_top",
                Text = " ",
                Appearance = Appearance.Button,
                AccessibleName = SR.DockEditorTopAccName
            };

            _bottom = new SelectionPanelRadioButton()
            {
                Dock = DockStyle.Bottom,
                TabIndex = 5,
                TabStop = true,
                Name = "_bottom",
                Text = " ",
                Appearance = Appearance.Button,
                AccessibleName = SR.DockEditorBottomAccName
            };

            _fill = new SelectionPanelRadioButton()
            {
                Dock = DockStyle.Fill,
                TabIndex = 3,
                TabStop = true,
                Name = "_fill",
                Text = " ",
                Appearance = Appearance.Button,
                AccessibleName = SR.DockEditorFillAccName
            };

            InitializeComponent(s_initialSystemDpi);

            _upDownOrder = [_top, _fill, _bottom, _none];
            _leftRightOrder = [_left, _fill, _right];
            _tabOrder = [_top, _left, _fill, _right, _bottom, _none];
        }

        private DockStyle DockStyle
        {
            get
            {
                if (CheckedControl == _fill)
                {
                    return DockStyle.Fill;
                }
                else if (CheckedControl == _left)
                {
                    return DockStyle.Left;
                }
                else if (CheckedControl == _right)
                {
                    return DockStyle.Right;
                }
                else if (CheckedControl == _top)
                {
                    return DockStyle.Top;
                }
                else if (CheckedControl == _bottom)
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

        private void InitializeComponent(int dpi)
        {
            const int LogicalNoneHeight = 24;
            const int LogicalNoneWidth = 90;
            const int LogicalControlWidth = 94;
            const int LogicalControlHeight = 116;
            const int LogicalContainerOffset = 2;
            const int LogicalContainerSize = 90;
            const int LogicalButtonSize = 20;

            SetBounds(
                0,
                0,
                ScaleHelper.ScaleToDpi(LogicalControlWidth, dpi),
                ScaleHelper.ScaleToDpi(LogicalControlHeight, dpi));

            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            AccessibleName = SR.DockEditorAccName;

            _none.Size = ScaleHelper.ScaleToDpi(new Size(LogicalNoneWidth, LogicalNoneHeight), dpi);

            int scaledOffset = ScaleHelper.ScaleToDpi(LogicalContainerOffset, dpi);
            _container.Location = new Point(scaledOffset, scaledOffset);
            _container.Size = ScaleHelper.ScaleToDpi(new Size(LogicalContainerSize, LogicalContainerSize), dpi);

            Size scaledButtonSize = ScaleHelper.ScaleToDpi(new Size(LogicalButtonSize, LogicalButtonSize), dpi);

            _right.Size = scaledButtonSize;
            _left.Size = scaledButtonSize;
            _top.Size = scaledButtonSize;
            _bottom.Size = scaledButtonSize;
            _fill.Size = scaledButtonSize;

            Controls.Clear();
            Controls.Add(_container);

            _container.Controls.Clear();
            _container.Controls.AddRange(
            [
                _fill,
                _left,
                _right,
                _top,
                _bottom,
                _none
            ]);

            ConfigureButtons();
        }

        protected override RadioButton ProcessDownKey(RadioButton checkedControl)
            => ProcessUpDown(checkedControl, upDirection: false);

        protected override RadioButton ProcessLeftKey(RadioButton checkedControl)
            => ProcessLeftRight(checkedControl, leftDirection: true);

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
