// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an editor for setting the ToolStripStatusLabel BorderSides property..
    /// </summary>
    [CLSCompliant(false)]
    public class BorderSidesEditor : UITypeEditor
    {
        private BorderSidesEditorUI _borderSidesEditorUI;

        /// <summary>
        ///  Edits the given object value using the editor style provided by BorderSidesEditor.GetEditStyle.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider is null)
            {
                return value;
            }
            if (!(provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc))
            {
                return value;
            }

            if (_borderSidesEditorUI is null)
            {
                _borderSidesEditorUI = new BorderSidesEditorUI(this);
            }

            _borderSidesEditorUI.Start(edSvc, value);
            edSvc.DropDownControl(_borderSidesEditorUI);

            if (_borderSidesEditorUI.Value != null)
            {
                value = _borderSidesEditorUI.Value;
            }

            _borderSidesEditorUI.End();
            return value;
        }

        /// <summary>
        ///  Gets the editing style of the Edit method.
        ///  If the method is not supported, this will return UITypeEditorEditStyle.None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        ///  Editor UI for the BorderSides editor.
        /// </summary>
        private class BorderSidesEditorUI : UserControl
        {
            private CheckBox allCheckBox;

            private bool allChecked;
            private CheckBox bottomCheckBox;
            private readonly BorderSidesEditor editor;
            private CheckBox leftCheckBox;
            private CheckBox noneCheckBox;
            private bool noneChecked;
            private object originalValue;
            private CheckBox rightCheckBox;
            private Label splitterLabel;

            private TableLayoutPanel tableLayoutPanel1;
            private CheckBox topCheckBox;

            private bool updateCurrentValue;

            public BorderSidesEditorUI(BorderSidesEditor editor)
            {
                this.editor = editor;
                End();
                InitializeComponent();
                Size = PreferredSize;
            }

            /// <summary>
            ///  Allows someone else to close our dropdown.
            /// </summary>
            public IWindowsFormsEditorService EditorService
            {
                get;
                private set;
            }

            /// <summary>
            ///  Returns the current value of BorderSides, if nothing is selected returns BorderSides.None.
            /// </summary>
            public object Value { get; private set; }

            public void End()
            {
                EditorService = null;
                originalValue = null;
                Value = null;
                updateCurrentValue = false;
            }

            /// <summary>
            ///  The first checkBox (allCheckBox) gets the focus by default.
            /// </summary>
            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                noneCheckBox.Focus();
            }

            private void InitializeComponent()
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(BorderSidesEditor));
                tableLayoutPanel1 = new TableLayoutPanel();
                noneCheckBox = new CheckBox();
                allCheckBox = new CheckBox();
                topCheckBox = new CheckBox();
                bottomCheckBox = new CheckBox();
                rightCheckBox = new CheckBox();
                leftCheckBox = new CheckBox();
                splitterLabel = new Label();
                tableLayoutPanel1.SuspendLayout();
                SuspendLayout();
                //
                // tableLayoutPanel1
                //
                resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
                tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                tableLayoutPanel1.BackColor = SystemColors.Window;
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
                tableLayoutPanel1.Controls.Add(noneCheckBox, 0, 0);
                tableLayoutPanel1.Controls.Add(allCheckBox, 0, 2);
                tableLayoutPanel1.Controls.Add(topCheckBox, 0, 3);
                tableLayoutPanel1.Controls.Add(bottomCheckBox, 0, 4);
                tableLayoutPanel1.Controls.Add(rightCheckBox, 0, 6);
                tableLayoutPanel1.Controls.Add(leftCheckBox, 0, 5);
                tableLayoutPanel1.Controls.Add(splitterLabel, 0, 1);
                tableLayoutPanel1.Name = "tableLayoutPanel1";
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.Margin = new Padding(0);
                //
                // noneCheckBox
                //
                resources.ApplyResources(noneCheckBox, "noneCheckBox");
                noneCheckBox.Name = "noneCheckBox";
                noneCheckBox.Margin = new Padding(3, 3, 3, 1);
                //
                // allCheckBox
                //
                resources.ApplyResources(allCheckBox, "allCheckBox");
                allCheckBox.Name = "allCheckBox";
                allCheckBox.Margin = new Padding(3, 3, 3, 1);
                //
                // topCheckBox
                //
                resources.ApplyResources(topCheckBox, "topCheckBox");
                topCheckBox.Margin = new Padding(20, 1, 3, 1);
                topCheckBox.Name = "topCheckBox";
                //
                // bottomCheckBox
                //
                resources.ApplyResources(bottomCheckBox, "bottomCheckBox");
                bottomCheckBox.Margin = new Padding(20, 1, 3, 1);
                bottomCheckBox.Name = "bottomCheckBox";
                //
                // rightCheckBox
                //
                resources.ApplyResources(rightCheckBox, "rightCheckBox");
                rightCheckBox.Margin = new Padding(20, 1, 3, 1);
                rightCheckBox.Name = "rightCheckBox";
                //
                // leftCheckBox
                //
                resources.ApplyResources(leftCheckBox, "leftCheckBox");
                leftCheckBox.Margin = new Padding(20, 1, 3, 1);
                leftCheckBox.Name = "leftCheckBox";
                //
                // splitterLabel
                //
                resources.ApplyResources(splitterLabel, "splitterLabel");
                splitterLabel.BackColor = SystemColors.ControlDark;
                splitterLabel.Name = "splitterLabel";
                //
                // Control
                //
                resources.ApplyResources(this, "$this");
                Controls.Add(tableLayoutPanel1);
                Padding = new Padding(1, 1, 1, 1);
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                AutoScaleMode = AutoScaleMode.Font;
                AutoScaleDimensions = new SizeF(6F, 13F);
                tableLayoutPanel1.ResumeLayout(false);
                tableLayoutPanel1.PerformLayout();
                ResumeLayout(false);
                PerformLayout();

                //Events
                rightCheckBox.CheckedChanged += rightCheckBox_CheckedChanged;
                leftCheckBox.CheckedChanged += leftCheckBox_CheckedChanged;
                bottomCheckBox.CheckedChanged += bottomCheckBox_CheckedChanged;
                topCheckBox.CheckedChanged += topCheckBox_CheckedChanged;
                noneCheckBox.CheckedChanged += noneCheckBox_CheckedChanged;
                allCheckBox.CheckedChanged += allCheckBox_CheckedChanged;

                noneCheckBox.Click += noneCheckBoxClicked;
                allCheckBox.Click += allCheckBoxClicked;
            }

            /// <summary>
            ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
            /// </summary>
            private void rightCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                CheckBox senderCheckBox = sender as CheckBox;
                if (senderCheckBox.Checked)
                {
                    noneCheckBox.Checked = false;
                }
                else // this is turned off....
                {
                    if (allCheckBox.Checked)
                    {
                        allCheckBox.Checked = false;
                    }
                }

                UpdateCurrentValue();
            }

            /// <summary>
            ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
            /// </summary>
            private void leftCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                CheckBox senderCheckBox = sender as CheckBox;
                if (senderCheckBox.Checked)
                {
                    noneCheckBox.Checked = false;
                }
                else // this is turned off....
                {
                    if (allCheckBox.Checked)
                    {
                        allCheckBox.Checked = false;
                    }
                }

                UpdateCurrentValue();
            }

            /// <summary>
            ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
            /// </summary>
            private void bottomCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                CheckBox senderCheckBox = sender as CheckBox;
                if (senderCheckBox.Checked)
                {
                    noneCheckBox.Checked = false;
                }
                else // this is turned off....
                {
                    if (allCheckBox.Checked)
                    {
                        allCheckBox.Checked = false;
                    }
                }

                UpdateCurrentValue();
            }

            /// <summary>
            ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
            /// </summary>
            private void topCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                CheckBox senderCheckBox = sender as CheckBox;
                if (senderCheckBox.Checked)
                {
                    noneCheckBox.Checked = false;
                }
                else // this is turned off....
                {
                    if (allCheckBox.Checked)
                    {
                        allCheckBox.Checked = false;
                    }
                }

                UpdateCurrentValue();
            }

            /// <summary>
            ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
            /// </summary>
            private void noneCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                CheckBox senderCheckBox = sender as CheckBox;
                if (senderCheckBox.Checked)
                {
                    allCheckBox.Checked = false;
                    topCheckBox.Checked = false;
                    bottomCheckBox.Checked = false;
                    leftCheckBox.Checked = false;
                    rightCheckBox.Checked = false;
                }

                UpdateCurrentValue();
            }

            /// <summary>
            ///  CheckBox CheckedChanged event.. allows selecting/Deselecting proper values.
            /// </summary>
            private void allCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                CheckBox senderCheckBox = sender as CheckBox;
                if (senderCheckBox.Checked)
                {
                    noneCheckBox.Checked = false;
                    topCheckBox.Checked = true;
                    bottomCheckBox.Checked = true;
                    leftCheckBox.Checked = true;
                    rightCheckBox.Checked = true;
                }

                UpdateCurrentValue();
            }

            /// <summary>
            ///  Click event.
            /// </summary>
            private void noneCheckBoxClicked(object sender, EventArgs e)
            {
                if (noneChecked)
                {
                    noneCheckBox.Checked = true;
                }
            }

            /// <summary>
            ///  Click event.
            /// </summary>
            private void allCheckBoxClicked(object sender, EventArgs e)
            {
                if (allChecked)
                {
                    allCheckBox.Checked = true;
                }
            }

            /// <summary>
            ///  Allows to reset the state and start afresh.
            /// </summary>
            private void ResetCheckBoxState()
            {
                allCheckBox.Checked = false;
                noneCheckBox.Checked = false;
                topCheckBox.Checked = false;
                bottomCheckBox.Checked = false;
                leftCheckBox.Checked = false;
                rightCheckBox.Checked = false;
            }

            /// <summary>
            ///  Allows to select proper values..
            /// </summary>
            private void SetCheckBoxCheckState(ToolStripStatusLabelBorderSides sides)
            {
                ResetCheckBoxState();
                if ((sides & ToolStripStatusLabelBorderSides.All) == ToolStripStatusLabelBorderSides.All)
                {
                    allCheckBox.Checked = true;
                    topCheckBox.Checked = true;
                    bottomCheckBox.Checked = true;
                    leftCheckBox.Checked = true;
                    rightCheckBox.Checked = true;
                    allCheckBox.Checked = true;
                }
                else
                {
                    noneCheckBox.Checked = (sides & ToolStripStatusLabelBorderSides.None) ==
                                           ToolStripStatusLabelBorderSides.None;
                    topCheckBox.Checked = (sides & ToolStripStatusLabelBorderSides.Top) ==
                                          ToolStripStatusLabelBorderSides.Top;
                    bottomCheckBox.Checked = (sides & ToolStripStatusLabelBorderSides.Bottom) ==
                                             ToolStripStatusLabelBorderSides.Bottom;
                    leftCheckBox.Checked = (sides & ToolStripStatusLabelBorderSides.Left) ==
                                           ToolStripStatusLabelBorderSides.Left;
                    rightCheckBox.Checked = (sides & ToolStripStatusLabelBorderSides.Right) ==
                                            ToolStripStatusLabelBorderSides.Right;
                }
            }

            /// <summary>
            ///  Triggered whenever the user drops down the editor.
            /// </summary>
            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                Debug.Assert(edSvc != null);

                EditorService = edSvc;
                originalValue = Value = value;

                if (value is ToolStripStatusLabelBorderSides currentSides)
                {
                    SetCheckBoxCheckState(currentSides);
                    updateCurrentValue = true;
                }
            }

            /// <summary>
            ///  Update the current value based on the state of the UI controls.
            /// </summary>
            private void UpdateCurrentValue()
            {
                if (!updateCurrentValue)
                {
                    return;
                }

                ToolStripStatusLabelBorderSides valueSide = ToolStripStatusLabelBorderSides.None;
                if (allCheckBox.Checked)
                {
                    valueSide |= ToolStripStatusLabelBorderSides.All;
                    Value = valueSide;
                    allChecked = true;
                    noneChecked = false;
                    return;
                }

                if (noneCheckBox.Checked)
                {
                    valueSide |= ToolStripStatusLabelBorderSides.None;
                }

                if (topCheckBox.Checked)
                {
                    valueSide |= ToolStripStatusLabelBorderSides.Top;
                }

                if (bottomCheckBox.Checked)
                {
                    valueSide |= ToolStripStatusLabelBorderSides.Bottom;
                }

                if (leftCheckBox.Checked)
                {
                    valueSide |= ToolStripStatusLabelBorderSides.Left;
                }

                if (rightCheckBox.Checked)
                {
                    valueSide |= ToolStripStatusLabelBorderSides.Right;
                }

                if (valueSide == ToolStripStatusLabelBorderSides.None)
                {
                    allChecked = false;
                    noneChecked = true;
                    noneCheckBox.Checked = true;
                }

                if (valueSide == (ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right |
                                  ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Bottom))
                {
                    allChecked = true;
                    noneChecked = false;
                    allCheckBox.Checked = true;
                }

                Value = valueSide;
            }
        }
    }
}
