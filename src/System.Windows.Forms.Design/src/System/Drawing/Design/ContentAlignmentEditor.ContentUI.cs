// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Drawing.Design
{
    public partial class ContentAlignmentEditor
    {
        /// <summary>
        /// Control we use to provide the content alignment UI.
        /// </summary>
        private class ContentUI : Control
        {
            private IWindowsFormsEditorService _edSvc;
            private double _pixelFactor;
            private bool _allowExit = true;

            private readonly RadioButton _topLeft = new RadioButton();
            private readonly RadioButton _topCenter = new RadioButton();
            private readonly RadioButton _topRight = new RadioButton();
            private readonly RadioButton _middleLeft = new RadioButton();
            private readonly RadioButton _middleCenter = new RadioButton();
            private readonly RadioButton _middleRight = new RadioButton();
            private readonly RadioButton _bottomLeft = new RadioButton();
            private readonly RadioButton _bottomCenter = new RadioButton();
            private readonly RadioButton _bottomRight = new RadioButton();

            public ContentUI()
            {
                _pixelFactor = DpiHelper.LogicalToDeviceUnits(1);
                InitComponent();
            }

            private ContentAlignment Align
            {
                get
                {
                    if (_topLeft.Checked)
                    {
                        return ContentAlignment.TopLeft;
                    }
                    else if (_topCenter.Checked)
                    {
                        return ContentAlignment.TopCenter;
                    }
                    else if (_topRight.Checked)
                    {
                        return ContentAlignment.TopRight;
                    }
                    else if (_middleLeft.Checked)
                    {
                        return ContentAlignment.MiddleLeft;
                    }
                    else if (_middleCenter.Checked)
                    {
                        return ContentAlignment.MiddleCenter;
                    }
                    else if (_middleRight.Checked)
                    {
                        return ContentAlignment.MiddleRight;
                    }
                    else if (_bottomLeft.Checked)
                    {
                        return ContentAlignment.BottomLeft;
                    }
                    else if (_bottomCenter.Checked)
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

            protected override bool ShowFocusCues
            {
                get => true;
            }

            public object Value { get; private set; }

            public void End()
            {
                _edSvc = null;
                Value = null;
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

            private void SetDimensions()
            {
                SuspendLayout();
                try
                {
                    // This is to invoke parent changed message that help rescaling the controls based on parent font (when it changed)
                    Controls.Clear();

                    //local cache.
                    var pixel_24 = DpiHelper.ConvertToGivenDpiPixel(24, _pixelFactor);
                    var pixel_25 = DpiHelper.ConvertToGivenDpiPixel(25, _pixelFactor);
                    var pixel_32 = DpiHelper.ConvertToGivenDpiPixel(32, _pixelFactor);
                    var pixel_59 = DpiHelper.ConvertToGivenDpiPixel(59, _pixelFactor);
                    var pixel_64 = DpiHelper.ConvertToGivenDpiPixel(64, _pixelFactor);
                    var pixel_89 = DpiHelper.ConvertToGivenDpiPixel(89, _pixelFactor);
                    var pixel_99 = DpiHelper.ConvertToGivenDpiPixel(99, _pixelFactor);
                    var pixel_125 = DpiHelper.ConvertToGivenDpiPixel(125, _pixelFactor);

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
                    Controls.AddRange(new Control[]
                    {
                        _bottomRight,
                        _bottomCenter,
                        _bottomLeft,
                        _middleRight,
                        _middleCenter,
                        _middleLeft,
                        _topRight,
                        _topCenter,
                        _topLeft
                    });
                }
                finally
                {
                    ResumeLayout();
                }
            }

            private void InitComponent()
            {
                BackColor = SystemColors.Control;
                ForeColor = SystemColors.ControlText;
                AccessibleName = SR.ContentAlignmentEditorAccName;

                _topLeft.TabIndex = 8;
                _topLeft.Text = string.Empty;
                _topLeft.Appearance = Appearance.Button;
                _topLeft.Click += new EventHandler(OptionClick);
                _topLeft.AccessibleName = SR.ContentAlignmentEditorTopLeftAccName;

                _topCenter.TabIndex = 0;
                _topCenter.Text = string.Empty;
                _topCenter.Appearance = Appearance.Button;
                _topCenter.Click += new EventHandler(OptionClick);
                _topCenter.AccessibleName = SR.ContentAlignmentEditorTopCenterAccName;

                _topRight.TabIndex = 1;
                _topRight.Text = string.Empty;
                _topRight.Appearance = Appearance.Button;
                _topRight.Click += new EventHandler(OptionClick);
                _topRight.AccessibleName = SR.ContentAlignmentEditorTopRightAccName;

                _middleLeft.TabIndex = 2;
                _middleLeft.Text = string.Empty;
                _middleLeft.Appearance = Appearance.Button;
                _middleLeft.Click += new EventHandler(OptionClick);
                _middleLeft.AccessibleName = SR.ContentAlignmentEditorMiddleLeftAccName;

                _middleCenter.TabIndex = 3;
                _middleCenter.Text = string.Empty;
                _middleCenter.Appearance = Appearance.Button;
                _middleCenter.Click += new EventHandler(OptionClick);
                _middleCenter.AccessibleName = SR.ContentAlignmentEditorMiddleCenterAccName;

                _middleRight.TabIndex = 4;
                _middleRight.Text = string.Empty;
                _middleRight.Appearance = Appearance.Button;
                _middleRight.Click += new EventHandler(OptionClick);
                _middleRight.AccessibleName = SR.ContentAlignmentEditorMiddleRightAccName;

                _bottomLeft.TabIndex = 5;
                _bottomLeft.Text = string.Empty;
                _bottomLeft.Appearance = Appearance.Button;
                _bottomLeft.Click += new EventHandler(OptionClick);
                _bottomLeft.AccessibleName = SR.ContentAlignmentEditorBottomLeftAccName;

                _bottomCenter.TabIndex = 6;
                _bottomCenter.Text = string.Empty;
                _bottomCenter.Appearance = Appearance.Button;
                _bottomCenter.Click += new EventHandler(OptionClick);
                _bottomCenter.AccessibleName = SR.ContentAlignmentEditorBottomCenterAccName;

                _bottomRight.TabIndex = 7;
                _bottomRight.Text = string.Empty;
                _bottomRight.Appearance = Appearance.Button;
                _bottomRight.Click += new EventHandler(OptionClick);
                _bottomRight.AccessibleName = SR.ContentAlignmentEditorBottomRightAccName;
                SetDimensions();
            }

            protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
            {
                var factor = (double)deviceDpiNew / deviceDpiOld;
                _pixelFactor *= factor;
                ResetAnchorStyle(toNone: true);
                SetDimensions();
            }

            protected override bool IsInputKey(Keys keyData)
                => keyData switch
                {
                    // here, we will return false, because we want the arrow keys
                    // to get picked up by the process key method below
                    Keys.Left => false,
                    Keys.Right => false,
                    Keys.Up => false,
                    Keys.Down => false,
                    _ => base.IsInputKey(keyData),
                };

            private void OptionClick(object sender, EventArgs e)
            {
                // We allow dialog exit if allowExit is set to true
                // and don't want the unintended Click event to close the dialog
                if (_allowExit)
                {
                    Value = Align;
                    _edSvc.CloseDropDown();
                }
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);

                // Refresh current selection - this moves the focus to selected control, on editor launch
                Align = Align;
            }

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                _edSvc = edSvc;
                Value = value;

                Align = (value is null) ? ContentAlignment.MiddleLeft : (ContentAlignment)value;
            }

            /// <summary>
            /// Here, we handle the return, tab, and escape keys appropriately
            /// </summary>
            protected override bool ProcessDialogKey(Keys keyData)
            {
                var checkedControl = CheckedControl;

                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Left:
                        ProcessLeftKey(checkedControl);
                        return true;

                    case Keys.Right:
                        ProcessRightKey(checkedControl);
                        return true;

                    case Keys.Up:
                        ProcessUpKey(checkedControl);
                        return true;

                    case Keys.Down:
                        ProcessDownKey(checkedControl);
                        return true;

                    case Keys.Space:
                        OptionClick(this, EventArgs.Empty);
                        return true;

                    case Keys.Return:
                        if ((keyData & (Keys.Alt | Keys.Control)) == 0)
                        {
                            OptionClick(this, EventArgs.Empty);
                            return true;
                        }
                        goto default;

                    case Keys.Escape:
                        if ((keyData & (Keys.Alt | Keys.Control)) == 0)
                        {
                            _edSvc.CloseDropDown();
                            return true;
                        }
                        goto default;

                    case Keys.Tab:
                        if ((keyData & (Keys.Alt | Keys.Control)) == 0)
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
                                if (Controls[i] is RadioButton && Controls[i].TabIndex == nextTabIndex)
                                {
                                    CheckedControl = (RadioButton)Controls[i];
                                    return true;
                                }
                            }

                            return true;
                        }
                        goto default;

                    default:
                        return base.ProcessDialogKey(keyData);
                }
            }

            /// <summary>
            /// Imagine the grid to choose alignment:
            /// [TL] [TC] [TR]
            /// [ML] [MC] [MR]
            /// [BL] [BC] [BR]
            /// Pressing Down on any of these will lead to the same column but
            /// a lower row; and pressing Down on the bottom row is meaningless
            /// </summary>
            /// <param name="checkedControl"></param>
            private void ProcessDownKey(RadioButton checkedControl)
            {
                if (checkedControl == _topRight)
                {
                    CheckedControl = _middleRight;
                }
                else if (checkedControl == _middleRight)
                {
                    CheckedControl = _bottomRight;
                }
                else if (checkedControl == _topCenter)
                {
                    CheckedControl = _middleCenter;
                }
                else if (checkedControl == _middleCenter)
                {
                    CheckedControl = _bottomCenter;
                }
                else if (checkedControl == _topLeft)
                {
                    CheckedControl = _middleLeft;
                }
                else if (checkedControl == _middleLeft)
                {
                    CheckedControl = _bottomLeft;
                }
            }

            /// <summary>
            /// Imagine the grid to choose alignment:
            /// [TL] [TC] [TR]
            /// [ML] [MC] [MR]
            /// [BL] [BC] [BR]
            /// Pressing Up on any of these will lead to the same column but
            /// a higher row; and pressing Up on the top row is meaningless
            /// </summary>
            /// <param name="checkedControl"></param>
            private void ProcessUpKey(RadioButton checkedControl)
            {
                if (checkedControl == _bottomRight)
                {
                    CheckedControl = _middleRight;
                }
                else if (checkedControl == _middleRight)
                {
                    CheckedControl = _topRight;
                }
                else if (checkedControl == _bottomCenter)
                {
                    CheckedControl = _middleCenter;
                }
                else if (checkedControl == _middleCenter)
                {
                    CheckedControl = _topCenter;
                }
                else if (checkedControl == _bottomLeft)
                {
                    CheckedControl = _middleLeft;
                }
                else if (checkedControl == _middleLeft)
                {
                    CheckedControl = _topLeft;
                }
            }

            /// <summary>
            /// Imagine the grid to choose alignment:
            /// [TL] [TC] [TR]
            /// [ML] [MC] [MR]
            /// [BL] [BC] [BR]
            /// Pressing Right on any of these will lead to the same row but
            /// a farther Right column; and pressing right on the right-most column is meaningless
            /// </summary>
            /// <param name="checkedControl"></param>
            private void ProcessRightKey(RadioButton checkedControl)
            {
                if (checkedControl == _bottomLeft)
                {
                    CheckedControl = _bottomCenter;
                }
                else if (checkedControl == _middleLeft)
                {
                    CheckedControl = _middleCenter;
                }
                else if (checkedControl == _topLeft)
                {
                    CheckedControl = _topCenter;
                }
                else if (checkedControl == _bottomCenter)
                {
                    CheckedControl = _bottomRight;
                }
                else if (checkedControl == _middleCenter)
                {
                    CheckedControl = _middleRight;
                }
                else if (checkedControl == _topCenter)
                {
                    CheckedControl = _topRight;
                }
            }

            /// <summary>
            /// Imagine the grid to choose alignment:
            /// [TL] [TC] [TR]
            /// [ML] [MC] [MR]
            /// [BL] [BC] [BR]
            /// Pressing Left on any of these will lead to the same row but
            /// a farther left column; and pressing Left on the left-most column is meaningless
            /// </summary>
            /// <param name="checkedControl"></param>
            private void ProcessLeftKey(RadioButton checkedControl)
            {
                if (checkedControl == _bottomRight)
                {
                    CheckedControl = _bottomCenter;
                }
                else if (checkedControl == _middleRight)
                {
                    CheckedControl = _middleCenter;
                }
                else if (checkedControl == _topRight)
                {
                    CheckedControl = _topCenter;
                }
                else if (checkedControl == _bottomCenter)
                {
                    CheckedControl = _bottomLeft;
                }
                else if (checkedControl == _middleCenter)
                {
                    CheckedControl = _middleLeft;
                }
                else if (checkedControl == _topCenter)
                {
                    CheckedControl = _topLeft;
                }
            }

            /// <summary>
            /// Gets/Sets the checked control value of our editor
            /// </summary>
            private RadioButton CheckedControl
            {
                get
                {
                    foreach (var control in Controls)
                    {
                        if (control is RadioButton radioButton && radioButton.Checked)
                        {
                            return radioButton;
                        }
                    }
                    return _middleLeft;
                }
                set
                {
                    CheckedControl.Checked = false;
                    value.Checked = true;

                    // To actually move focus to a radio button, we need to call Focus() method.
                    // However, that would raise OnClick event, which would close the editor.
                    // We set allowExit to false, to block editor exit, on radio button selection change.
                    _allowExit = false;
                    value.Focus();
                    _allowExit = true;

                    // RadioButton::Checked will tell Accessibility that State and Name changed.
                    // Tell Accessibility that focus changed as well.
                    if (value.IsHandleCreated)
                    {
                        User32.NotifyWinEvent((int)AccessibleEvents.Focus, new HandleRef(value, value.Handle), User32.OBJID.CLIENT, 0);
                    }
                }
            }
        }
    }
}
