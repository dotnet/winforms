// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design;

public partial class BorderSidesEditor
{
    /// <summary>
    ///  Editor UI for the BorderSides editor.
    /// </summary>
    private class BorderSidesEditorUI : UserControl
    {
        private CheckBox _allCheckBox;

        private bool _allChecked;
        private CheckBox _bottomCheckBox;
        private CheckBox _leftCheckBox;
        private CheckBox _noneCheckBox;
        private bool _noneChecked;
        private CheckBox _rightCheckBox;
        private Label _splitterLabel;

        private TableLayoutPanel _tableLayoutPanel;
        private CheckBox _topCheckBox;

        private bool _updateCurrentValue;

        public BorderSidesEditorUI()
        {
            End();
            InitializeComponent();
            Size = PreferredSize;
        }

        /// <summary>
        ///  Allows someone else to close our dropdown.
        /// </summary>
        public IWindowsFormsEditorService? EditorService
        {
            get;
            private set;
        }

        /// <summary>
        ///  Returns the current value of BorderSides, if nothing is selected returns BorderSides.None.
        /// </summary>
        public object? Value { get; private set; }

        public void End()
        {
            EditorService = null;
            Value = null;
            _updateCurrentValue = false;
        }

        /// <summary>
        ///  The first checkBox (allCheckBox) gets the focus by default.
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _noneCheckBox.Focus();
        }

        [MemberNotNull(nameof(_allCheckBox))]
        [MemberNotNull(nameof(_bottomCheckBox))]
        [MemberNotNull(nameof(_leftCheckBox))]
        [MemberNotNull(nameof(_noneCheckBox))]
        [MemberNotNull(nameof(_rightCheckBox))]
        [MemberNotNull(nameof(_splitterLabel))]
        [MemberNotNull(nameof(_tableLayoutPanel))]
        [MemberNotNull(nameof(_topCheckBox))]
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(BorderSidesEditor));
            _tableLayoutPanel = new TableLayoutPanel();
            _noneCheckBox = new CheckBox();
            _allCheckBox = new CheckBox();
            _topCheckBox = new CheckBox();
            _bottomCheckBox = new CheckBox();
            _rightCheckBox = new CheckBox();
            _leftCheckBox = new CheckBox();
            _splitterLabel = new Label();
            _tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            //
            // tableLayoutPanel1
            //
            resources.ApplyResources(_tableLayoutPanel, "tableLayoutPanel1");
            _tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tableLayoutPanel.BackColor = SystemColors.Window;
            _tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            _tableLayoutPanel.Controls.Add(_noneCheckBox, 0, 0);
            _tableLayoutPanel.Controls.Add(_allCheckBox, 0, 2);
            _tableLayoutPanel.Controls.Add(_topCheckBox, 0, 3);
            _tableLayoutPanel.Controls.Add(_bottomCheckBox, 0, 4);
            _tableLayoutPanel.Controls.Add(_rightCheckBox, 0, 6);
            _tableLayoutPanel.Controls.Add(_leftCheckBox, 0, 5);
            _tableLayoutPanel.Controls.Add(_splitterLabel, 0, 1);
            _tableLayoutPanel.Name = "tableLayoutPanel1";
            _tableLayoutPanel.RowStyles.Add(new RowStyle());
            _tableLayoutPanel.RowStyles.Add(new RowStyle());
            _tableLayoutPanel.RowStyles.Add(new RowStyle());
            _tableLayoutPanel.RowStyles.Add(new RowStyle());
            _tableLayoutPanel.RowStyles.Add(new RowStyle());
            _tableLayoutPanel.RowStyles.Add(new RowStyle());
            _tableLayoutPanel.RowStyles.Add(new RowStyle());
            _tableLayoutPanel.Margin = new Padding(0);
            //
            // noneCheckBox
            //
            resources.ApplyResources(_noneCheckBox, "noneCheckBox");
            _noneCheckBox.Name = "noneCheckBox";
            _noneCheckBox.Margin = new Padding(3, 3, 3, 1);
            //
            // allCheckBox
            //
            resources.ApplyResources(_allCheckBox, "allCheckBox");
            _allCheckBox.Name = "allCheckBox";
            _allCheckBox.Margin = new Padding(3, 3, 3, 1);
            //
            // topCheckBox
            //
            resources.ApplyResources(_topCheckBox, "topCheckBox");
            _topCheckBox.Margin = new Padding(20, 1, 3, 1);
            _topCheckBox.Name = "topCheckBox";
            //
            // bottomCheckBox
            //
            resources.ApplyResources(_bottomCheckBox, "bottomCheckBox");
            _bottomCheckBox.Margin = new Padding(20, 1, 3, 1);
            _bottomCheckBox.Name = "bottomCheckBox";
            //
            // rightCheckBox
            //
            resources.ApplyResources(_rightCheckBox, "rightCheckBox");
            _rightCheckBox.Margin = new Padding(20, 1, 3, 1);
            _rightCheckBox.Name = "rightCheckBox";
            //
            // leftCheckBox
            //
            resources.ApplyResources(_leftCheckBox, "leftCheckBox");
            _leftCheckBox.Margin = new Padding(20, 1, 3, 1);
            _leftCheckBox.Name = "leftCheckBox";
            //
            // splitterLabel
            //
            resources.ApplyResources(_splitterLabel, "splitterLabel");
            _splitterLabel.BackColor = SystemColors.ControlDark;
            _splitterLabel.Name = "splitterLabel";
            //
            // Control
            //
            resources.ApplyResources(this, "$this");
            Controls.Add(_tableLayoutPanel);
            Padding = new Padding(1, 1, 1, 1);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoScaleMode = AutoScaleMode.Font;
            AutoScaleDimensions = new SizeF(6F, 13F);
            _tableLayoutPanel.ResumeLayout(false);
            _tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

            // Events
            _rightCheckBox.CheckedChanged += rightCheckBox_CheckedChanged;
            _leftCheckBox.CheckedChanged += leftCheckBox_CheckedChanged;
            _bottomCheckBox.CheckedChanged += bottomCheckBox_CheckedChanged;
            _topCheckBox.CheckedChanged += topCheckBox_CheckedChanged;
            _noneCheckBox.CheckedChanged += noneCheckBox_CheckedChanged;
            _allCheckBox.CheckedChanged += allCheckBox_CheckedChanged;

            _noneCheckBox.Click += noneCheckBoxClicked;
            _allCheckBox.Click += allCheckBoxClicked;
        }

        /// <summary>
        ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
        /// </summary>
        private void rightCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            CheckBox senderCheckBox = (CheckBox)sender!;
            if (senderCheckBox.Checked)
            {
                _noneCheckBox.Checked = false;
            }
            else // this is turned off....
            {
                if (_allCheckBox.Checked)
                {
                    _allCheckBox.Checked = false;
                }
            }

            UpdateCurrentValue();
        }

        /// <summary>
        ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
        /// </summary>
        private void leftCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            CheckBox senderCheckBox = (CheckBox)sender!;
            if (senderCheckBox.Checked)
            {
                _noneCheckBox.Checked = false;
            }
            else // this is turned off....
            {
                if (_allCheckBox.Checked)
                {
                    _allCheckBox.Checked = false;
                }
            }

            UpdateCurrentValue();
        }

        /// <summary>
        ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
        /// </summary>
        private void bottomCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            CheckBox senderCheckBox = (CheckBox)sender!;
            if (senderCheckBox.Checked)
            {
                _noneCheckBox.Checked = false;
            }
            else // this is turned off....
            {
                if (_allCheckBox.Checked)
                {
                    _allCheckBox.Checked = false;
                }
            }

            UpdateCurrentValue();
        }

        /// <summary>
        ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
        /// </summary>
        private void topCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            CheckBox senderCheckBox = (CheckBox)sender!;
            if (senderCheckBox.Checked)
            {
                _noneCheckBox.Checked = false;
            }
            else // this is turned off....
            {
                if (_allCheckBox.Checked)
                {
                    _allCheckBox.Checked = false;
                }
            }

            UpdateCurrentValue();
        }

        /// <summary>
        ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
        /// </summary>
        private void noneCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            CheckBox senderCheckBox = (CheckBox)sender!;
            if (senderCheckBox.Checked)
            {
                _allCheckBox.Checked = false;
                _topCheckBox.Checked = false;
                _bottomCheckBox.Checked = false;
                _leftCheckBox.Checked = false;
                _rightCheckBox.Checked = false;
            }

            UpdateCurrentValue();
        }

        /// <summary>
        ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
        /// </summary>
        private void allCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            CheckBox senderCheckBox = (CheckBox)sender!;
            if (senderCheckBox.Checked)
            {
                _noneCheckBox.Checked = false;
                _topCheckBox.Checked = true;
                _bottomCheckBox.Checked = true;
                _leftCheckBox.Checked = true;
                _rightCheckBox.Checked = true;
            }

            UpdateCurrentValue();
        }

        /// <summary>
        ///  Click event.
        /// </summary>
        private void noneCheckBoxClicked(object? sender, EventArgs e)
        {
            if (_noneChecked)
            {
                _noneCheckBox.Checked = true;
            }
        }

        /// <summary>
        ///  Click event.
        /// </summary>
        private void allCheckBoxClicked(object? sender, EventArgs e)
        {
            if (_allChecked)
            {
                _allCheckBox.Checked = true;
            }
        }

        /// <summary>
        ///  Allows to reset the state and start afresh.
        /// </summary>
        private void ResetCheckBoxState()
        {
            _allCheckBox.Checked = false;
            _noneCheckBox.Checked = false;
            _topCheckBox.Checked = false;
            _bottomCheckBox.Checked = false;
            _leftCheckBox.Checked = false;
            _rightCheckBox.Checked = false;
        }

        /// <summary>
        ///  Allows to select proper values.
        /// </summary>
        private void SetCheckBoxCheckState(ToolStripStatusLabelBorderSides sides)
        {
            ResetCheckBoxState();
            if (sides.HasFlag(ToolStripStatusLabelBorderSides.All))
            {
                _allCheckBox.Checked = true;
                _topCheckBox.Checked = true;
                _bottomCheckBox.Checked = true;
                _leftCheckBox.Checked = true;
                _rightCheckBox.Checked = true;
                _allCheckBox.Checked = true;
            }
            else
            {
                _noneCheckBox.Checked = sides.HasFlag(ToolStripStatusLabelBorderSides.None);
                _topCheckBox.Checked = sides.HasFlag(ToolStripStatusLabelBorderSides.Top);
                _bottomCheckBox.Checked = sides.HasFlag(ToolStripStatusLabelBorderSides.Bottom);
                _leftCheckBox.Checked = sides.HasFlag(ToolStripStatusLabelBorderSides.Left);
                _rightCheckBox.Checked = sides.HasFlag(ToolStripStatusLabelBorderSides.Right);
            }
        }

        /// <summary>
        ///  Triggered whenever the user drops down the editor.
        /// </summary>
        public void Start(IWindowsFormsEditorService edSvc, object? value)
        {
            Debug.Assert(edSvc is not null);

            EditorService = edSvc;
            Value = value;

            if (value is ToolStripStatusLabelBorderSides currentSides)
            {
                SetCheckBoxCheckState(currentSides);
                _updateCurrentValue = true;
            }
        }

        /// <summary>
        ///  Update the current value based on the state of the UI controls.
        /// </summary>
        private void UpdateCurrentValue()
        {
            if (!_updateCurrentValue)
            {
                return;
            }

            ToolStripStatusLabelBorderSides valueSide = ToolStripStatusLabelBorderSides.None;
            if (_allCheckBox.Checked)
            {
                valueSide |= ToolStripStatusLabelBorderSides.All;
                Value = valueSide;
                _allChecked = true;
                _noneChecked = false;
                return;
            }

            if (_noneCheckBox.Checked)
            {
                valueSide |= ToolStripStatusLabelBorderSides.None;
            }

            if (_topCheckBox.Checked)
            {
                valueSide |= ToolStripStatusLabelBorderSides.Top;
            }

            if (_bottomCheckBox.Checked)
            {
                valueSide |= ToolStripStatusLabelBorderSides.Bottom;
            }

            if (_leftCheckBox.Checked)
            {
                valueSide |= ToolStripStatusLabelBorderSides.Left;
            }

            if (_rightCheckBox.Checked)
            {
                valueSide |= ToolStripStatusLabelBorderSides.Right;
            }

            if (valueSide == ToolStripStatusLabelBorderSides.None)
            {
                _allChecked = false;
                _noneChecked = true;
                _noneCheckBox.Checked = true;
            }

            if (valueSide == (ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right |
                              ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Bottom))
            {
                _allChecked = true;
                _noneChecked = false;
                _allCheckBox.Checked = true;
            }

            Value = valueSide;
        }
    }
}
