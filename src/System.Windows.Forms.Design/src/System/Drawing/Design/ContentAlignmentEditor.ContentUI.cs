// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System.Drawing.Design;

public partial class ContentAlignmentEditor
{
    /// <summary>
    ///  Control we use to provide the content alignment UI.
    /// </summary>
    private sealed class ContentUI : SelectionPanelBase
    {
        private readonly SelectionPanelRadioButton _topLeft = new();
        private readonly SelectionPanelRadioButton _topCenter = new();
        private readonly SelectionPanelRadioButton _topRight = new();
        private readonly SelectionPanelRadioButton _middleLeft = new();
        private readonly SelectionPanelRadioButton _middleCenter = new();
        private readonly SelectionPanelRadioButton _middleRight = new();
        private readonly SelectionPanelRadioButton _bottomLeft = new();
        private readonly SelectionPanelRadioButton _bottomCenter = new();
        private readonly SelectionPanelRadioButton _bottomRight = new();

        public ContentUI()
        {
            InitComponent();
        }

        private ContentAlignment Align
        {
            get
            {
                if (CheckedControl == _topLeft)
                {
                    return ContentAlignment.TopLeft;
                }
                else if (CheckedControl == _topCenter)
                {
                    return ContentAlignment.TopCenter;
                }
                else if (CheckedControl == _topRight)
                {
                    return ContentAlignment.TopRight;
                }
                else if (CheckedControl == _middleLeft)
                {
                    return ContentAlignment.MiddleLeft;
                }
                else if (CheckedControl == _middleCenter)
                {
                    return ContentAlignment.MiddleCenter;
                }
                else if (CheckedControl == _middleRight)
                {
                    return ContentAlignment.MiddleRight;
                }
                else if (CheckedControl == _bottomLeft)
                {
                    return ContentAlignment.BottomLeft;
                }
                else if (CheckedControl == _bottomCenter)
                {
                    return ContentAlignment.BottomCenter;
                }
                else
                {
                    return ContentAlignment.BottomRight;
                }
            }
            set
            {
                switch (value)
                {
                    case ContentAlignment.TopLeft:
                        CheckedControl = _topLeft;
                        break;
                    case ContentAlignment.TopCenter:
                        CheckedControl = _topCenter;
                        break;
                    case ContentAlignment.TopRight:
                        CheckedControl = _topRight;
                        break;
                    case ContentAlignment.MiddleLeft:
                        CheckedControl = _middleLeft;
                        break;
                    case ContentAlignment.MiddleCenter:
                        CheckedControl = _middleCenter;
                        break;
                    case ContentAlignment.MiddleRight:
                        CheckedControl = _middleRight;
                        break;
                    case ContentAlignment.BottomLeft:
                        CheckedControl = _bottomLeft;
                        break;
                    case ContentAlignment.BottomCenter:
                        CheckedControl = _bottomCenter;
                        break;
                    case ContentAlignment.BottomRight:
                        CheckedControl = _bottomRight;
                        break;
                }
            }
        }

        protected override ControlCollection SelectionOptions => Controls;

        private void InitComponent()
        {
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            AccessibleName = SR.ContentAlignmentEditorAccName;

            _topLeft.TabIndex = 8;
            _topLeft.Text = string.Empty;
            _topLeft.Name = "_topLeft";
            _topLeft.Appearance = Appearance.Button;
            _topLeft.AccessibleName = SR.ContentAlignmentEditorTopLeftAccName;

            _topCenter.TabIndex = 0;
            _topCenter.Text = string.Empty;
            _topCenter.Name = "_topCenter";
            _topCenter.Appearance = Appearance.Button;
            _topCenter.AccessibleName = SR.ContentAlignmentEditorTopCenterAccName;

            _topRight.TabIndex = 1;
            _topRight.Text = string.Empty;
            _topRight.Name = "_topRight";
            _topRight.Appearance = Appearance.Button;
            _topRight.AccessibleName = SR.ContentAlignmentEditorTopRightAccName;

            _middleLeft.TabIndex = 2;
            _middleLeft.Text = string.Empty;
            _middleLeft.Name = "_middleLeft";
            _middleLeft.Appearance = Appearance.Button;
            _middleLeft.AccessibleName = SR.ContentAlignmentEditorMiddleLeftAccName;

            _middleCenter.TabIndex = 3;
            _middleCenter.Text = string.Empty;
            _middleCenter.Name = "_middleCenter";
            _middleCenter.Appearance = Appearance.Button;
            _middleCenter.AccessibleName = SR.ContentAlignmentEditorMiddleCenterAccName;

            _middleRight.TabIndex = 4;
            _middleRight.Text = string.Empty;
            _middleRight.Name = "_middleRight";
            _middleRight.Appearance = Appearance.Button;
            _middleRight.AccessibleName = SR.ContentAlignmentEditorMiddleRightAccName;

            _bottomLeft.TabIndex = 5;
            _bottomLeft.Text = string.Empty;
            _bottomLeft.Name = "_bottomLeft";
            _bottomLeft.Appearance = Appearance.Button;
            _bottomLeft.AccessibleName = SR.ContentAlignmentEditorBottomLeftAccName;

            _bottomCenter.TabIndex = 6;
            _bottomCenter.Text = string.Empty;
            _bottomCenter.Name = "_bottomCenter";
            _bottomCenter.Appearance = Appearance.Button;
            _bottomCenter.AccessibleName = SR.ContentAlignmentEditorBottomCenterAccName;

            _bottomRight.TabIndex = 7;
            _bottomRight.Text = string.Empty;
            _bottomRight.Name = "_bottomRight";
            _bottomRight.Appearance = Appearance.Button;
            _bottomRight.AccessibleName = SR.ContentAlignmentEditorBottomRightAccName;

            SetDimensions(ScaleHelper.InitialSystemDpi);
            ConfigureButtons();
        }

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            ResetAnchorStyle(toNone: true);
            SetDimensions(deviceDpiNew);
        }

        private void ResetAnchorStyle(bool toNone = false)
        {
            const AnchorStyles DefaultCenterAnchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            const AnchorStyles DefaultRightAnchor = AnchorStyles.Top | AnchorStyles.Right;

            if (toNone)
            {
                _topCenter.Anchor = AnchorStyles.None;
                _topRight.Anchor = AnchorStyles.None;
                _middleCenter.Anchor = AnchorStyles.None;
                _middleRight.Anchor = AnchorStyles.None;
                _bottomCenter.Anchor = AnchorStyles.None;
                _bottomRight.Anchor = AnchorStyles.None;
            }
            else
            {
                _topCenter.Anchor = DefaultCenterAnchor;
                _topRight.Anchor = DefaultRightAnchor;
                _middleCenter.Anchor = DefaultCenterAnchor;
                _middleRight.Anchor = DefaultRightAnchor;
                _bottomCenter.Anchor = DefaultCenterAnchor;
                _bottomRight.Anchor = DefaultRightAnchor;
            }
        }

        private void SetDimensions(int dpi)
        {
            SuspendLayout();
            try
            {
                // This is to invoke parent changed message that help rescaling the controls based on parent font (when it changed)
                Controls.Clear();

                // Local cache.
                int pixel_24 = ScaleHelper.ScaleToDpi(24, dpi);
                int pixel_25 = ScaleHelper.ScaleToDpi(25, dpi);
                int pixel_32 = ScaleHelper.ScaleToDpi(32, dpi);
                int pixel_59 = ScaleHelper.ScaleToDpi(59, dpi);
                int pixel_64 = ScaleHelper.ScaleToDpi(64, dpi);
                int pixel_89 = ScaleHelper.ScaleToDpi(89, dpi);
                int pixel_99 = ScaleHelper.ScaleToDpi(99, dpi);
                int pixel_125 = ScaleHelper.ScaleToDpi(125, dpi);

                Size = new Size(pixel_125, pixel_89);

                _topLeft.Size = new Size(pixel_24, pixel_25);

                _topCenter.Location = new Point(pixel_32, 0);
                _topCenter.Size = new Size(pixel_59, pixel_25);

                _topRight.Location = new Point(pixel_99, 0);
                _topRight.Size = new Size(pixel_24, pixel_25);

                _middleLeft.Location = new Point(0, pixel_32);
                _middleLeft.Size = new Size(pixel_24, pixel_25);

                _middleCenter.Location = new Point(pixel_32, pixel_32);
                _middleCenter.Size = new Size(pixel_59, pixel_25);

                _middleRight.Location = new Point(pixel_99, pixel_32);
                _middleRight.Size = new Size(pixel_24, pixel_25);

                _bottomLeft.Location = new Point(0, pixel_64);
                _bottomLeft.Size = new Size(pixel_24, pixel_25);

                _bottomCenter.Location = new Point(pixel_32, pixel_64);
                _bottomCenter.Size = new Size(pixel_59, pixel_25);

                _bottomRight.Location = new Point(pixel_99, pixel_64);
                _bottomRight.Size = new Size(pixel_24, pixel_25);

                ResetAnchorStyle();
                Controls.AddRange(
                [
                    _bottomRight,
                    _bottomCenter,
                    _bottomLeft,
                    _middleRight,
                    _middleCenter,
                    _middleLeft,
                    _topRight,
                    _topCenter,
                    _topLeft
                ]);
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        ///  Imagine a grid to choose alignment:
        ///
        ///   [TL] [TC] [TR]
        ///   [ML] [MC] [MR]
        ///   [BL] [BC] [BR]
        ///
        ///  Pressing Down on any of these will lead to the same column but
        ///  a lower row; and pressing Down on the bottom row is meaningless
        /// </summary>
        protected override RadioButton ProcessDownKey(RadioButton checkedControl)
        {
            if (checkedControl == _topRight)
            {
                return _middleRight;
            }
            else if (checkedControl == _middleRight)
            {
                return _bottomRight;
            }
            else if (checkedControl == _topCenter)
            {
                return _middleCenter;
            }
            else if (checkedControl == _middleCenter)
            {
                return _bottomCenter;
            }
            else if (checkedControl == _topLeft)
            {
                return _middleLeft;
            }
            else if (checkedControl == _middleLeft)
            {
                return _bottomLeft;
            }

            return checkedControl;
        }

        /// <summary>
        ///  Imagine a grid to choose alignment:
        ///
        ///   [TL] [TC] [TR]
        ///   [ML] [MC] [MR]
        ///   [BL] [BC] [BR]
        ///
        ///  Pressing Up on any of these will lead to the same column but
        ///  a higher row; and pressing Up on the top row is meaningless
        /// </summary>
        protected override RadioButton ProcessUpKey(RadioButton checkedControl)
        {
            if (checkedControl == _bottomRight)
            {
                return _middleRight;
            }
            else if (checkedControl == _middleRight)
            {
                return _topRight;
            }
            else if (checkedControl == _bottomCenter)
            {
                return _middleCenter;
            }
            else if (checkedControl == _middleCenter)
            {
                return _topCenter;
            }
            else if (checkedControl == _bottomLeft)
            {
                return _middleLeft;
            }
            else if (checkedControl == _middleLeft)
            {
                return _topLeft;
            }

            return checkedControl;
        }

        /// <summary>
        ///  Imagine a grid to choose alignment:
        ///
        ///   [TL] [TC] [TR]
        ///   [ML] [MC] [MR]
        ///   [BL] [BC] [BR]
        ///
        ///  Pressing Right on any of these will lead to the same row but a farther Right column;
        ///  and pressing right on the right-most column is meaningless.
        /// </summary>
        protected override RadioButton ProcessRightKey(RadioButton checkedControl)
        {
            if (checkedControl == _bottomLeft)
            {
                return _bottomCenter;
            }
            else if (checkedControl == _middleLeft)
            {
                return _middleCenter;
            }
            else if (checkedControl == _topLeft)
            {
                return _topCenter;
            }
            else if (checkedControl == _bottomCenter)
            {
                return _bottomRight;
            }
            else if (checkedControl == _middleCenter)
            {
                return _middleRight;
            }
            else if (checkedControl == _topCenter)
            {
                return _topRight;
            }

            return checkedControl;
        }

        /// <summary>
        ///  Imagine a grid to choose alignment:
        ///
        ///   [TL] [TC] [TR]
        ///   [ML] [MC] [MR]
        ///   [BL] [BC] [BR]
        ///
        ///  Pressing Left on any of these will lead to the same row but a farther left column; and pressing Left
        ///  on the left-most column is meaningless
        /// </summary>
        protected override RadioButton ProcessLeftKey(RadioButton checkedControl)
        {
            if (checkedControl == _bottomRight)
            {
                return _bottomCenter;
            }
            else if (checkedControl == _middleRight)
            {
                return _middleCenter;
            }
            else if (checkedControl == _topRight)
            {
                return _topCenter;
            }
            else if (checkedControl == _bottomCenter)
            {
                return _bottomLeft;
            }
            else if (checkedControl == _middleCenter)
            {
                return _middleLeft;
            }
            else if (checkedControl == _topCenter)
            {
                return _topLeft;
            }

            return checkedControl;
        }

        protected override RadioButton ProcessTabKey(Keys keyData)
        {
            int nextTabIndex = CheckedControl.TabIndex + ((keyData & Keys.Shift) == 0 ? 1 : -1);
            if (nextTabIndex < 0)
            {
                nextTabIndex = Controls.Count - 1;
            }
            else if (nextTabIndex >= Controls.Count)
            {
                nextTabIndex = 0;
            }

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is RadioButton button && Controls[i].TabIndex == nextTabIndex)
                {
                    return button;
                }
            }

            return CheckedControl;
        }

        protected override void SetInitialCheckedControl()
            => Align = Value is ContentAlignment contentAligment ? contentAligment : ContentAlignment.MiddleLeft;

        protected override void UpdateValue() => Value = Align;
    }
}
