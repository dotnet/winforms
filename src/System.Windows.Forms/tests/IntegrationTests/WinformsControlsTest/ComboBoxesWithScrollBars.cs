// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class ComboBoxesWithScrollBars : Form
{
    public ComboBoxesWithScrollBars()
    {
        InitializeComponent();

        for (int i = 0; i <= 40; i++)
        {
            comboBox1.Items.Add(i);
            comboBox2.Items.Add(i);
            comboBox3.Items.Add(i);
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        // Set current NumericUpDowns values to heights
        changeDDH_UpDown1.Value = comboBox1.DropDownHeight;
        changeCBHeight_UpDown2.Value = comboBox2.Size.Height;
        changeDDH_UpDown3.Value = comboBox3.DropDownHeight;

        // Set current NumericUpDowns values to MaxDropDownItems values
        maxDropDownItemsUpDown1.Value = comboBox1.MaxDropDownItems;
        maxDropDownItemsUpDown2.Value = comboBox2.MaxDropDownItems;
        maxDropDownItemsUpDown3.Value = comboBox3.MaxDropDownItems;

        // Set current CheckBoxes values to IntegralHeight values
        integralHeightCheckBox1.Checked = comboBox1.IntegralHeight;
        integralHeightCheckBox2.Checked = comboBox2.IntegralHeight;
        integralHeightCheckBox3.Checked = comboBox3.IntegralHeight;

        // Set current CheckBoxes values to DrawMode values
        useDifferentHeightsCheckBox1.Checked = comboBox1.DrawMode == DrawMode.OwnerDrawVariable;
        useDifferentHeightsCheckBox2.Checked = comboBox2.DrawMode == DrawMode.OwnerDrawVariable;
        useDifferentHeightsCheckBox3.Checked = comboBox3.DrawMode == DrawMode.OwnerDrawVariable;

        // Add event handlers
        // These handlers are not added to the designer and put here to avoid redundant invocations
        // when the values of the upDowns and checkBoxes are set above.
        // These handlers should be invoked when a user changes the upDowns and checkBoxes values only.
        changeDDH_UpDown1.ValueChanged += (s, e) =>
        {
            comboBox1.DropDownHeight = (int)changeDDH_UpDown1.Value;
            integralHeightCheckBox1.Checked = false;
        };
        changeCBHeight_UpDown2.ValueChanged += (s, e) => comboBox2.Size = new Size(comboBox2.Size.Width, (int)changeCBHeight_UpDown2.Value);
        changeDDH_UpDown3.ValueChanged += (s, e) =>
        {
            comboBox3.DropDownHeight = (int)changeDDH_UpDown3.Value;
            integralHeightCheckBox3.Checked = false;
        };
        maxDropDownItemsUpDown1.ValueChanged += (s, e) => comboBox1.MaxDropDownItems = (int)maxDropDownItemsUpDown1.Value;
        maxDropDownItemsUpDown2.ValueChanged += (s, e) => comboBox2.MaxDropDownItems = (int)maxDropDownItemsUpDown2.Value;
        maxDropDownItemsUpDown3.ValueChanged += (s, e) => comboBox3.MaxDropDownItems = (int)maxDropDownItemsUpDown3.Value;
        integralHeightCheckBox1.CheckedChanged += (s, e) => comboBox1.IntegralHeight = integralHeightCheckBox1.Checked;
        integralHeightCheckBox2.CheckedChanged += (s, e) => comboBox2.IntegralHeight = integralHeightCheckBox2.Checked;
        integralHeightCheckBox3.CheckedChanged += (s, e) => comboBox3.IntegralHeight = integralHeightCheckBox3.Checked;
        useDifferentHeightsCheckBox1.CheckedChanged += useDifferentHeightsCheckBox1_CheckedChanged;
        useDifferentHeightsCheckBox2.CheckedChanged += useDifferentHeightsCheckBox2_CheckedChanged;
        useDifferentHeightsCheckBox3.CheckedChanged += useDifferentHeightsCheckBox3_CheckedChanged;

        base.OnLoad(e);
    }

    private void useDifferentHeightsCheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        bool isChecked = useDifferentHeightsCheckBox1.Checked;

        if (isChecked)
        {
            comboBox1.DrawMode = DrawMode.OwnerDrawVariable;
            comboBox1.MeasureItem += comboBox_MeasureItem;
        }
        else
        {
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.MeasureItem -= comboBox_MeasureItem;
        }
    }

    private void useDifferentHeightsCheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        bool isChecked = useDifferentHeightsCheckBox2.Checked;

        if (isChecked)
        {
            comboBox2.DrawMode = DrawMode.OwnerDrawVariable;
            comboBox2.MeasureItem += comboBox_MeasureItem;
        }
        else
        {
            comboBox2.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox2.MeasureItem -= comboBox_MeasureItem;
        }
    }

    private void useDifferentHeightsCheckBox3_CheckedChanged(object sender, EventArgs e)
    {
        bool isChecked = useDifferentHeightsCheckBox3.Checked;

        if (isChecked)
        {
            comboBox3.SuspendLayout();
            comboBox3.DrawMode = DrawMode.OwnerDrawVariable;
            comboBox3.MeasureItem += comboBox_MeasureItem;
            comboBox3.ResumeLayout(false);
        }
        else
        {
            comboBox3.SuspendLayout();
            comboBox3.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox3.MeasureItem -= comboBox_MeasureItem;
            comboBox3.ResumeLayout(false);
        }
    }

    private void comboBox_MeasureItem(object sender, MeasureItemEventArgs e) => e.ItemHeight = 15 + (e.Index % 5) * 5;

    private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        ComboBox control = (ComboBox)sender;

        if (e.Index < 0 || e.Index >= control.Items.Count)
        {
            return;
        }

        e.DrawBackground();
        using SolidBrush brush = new(e.ForeColor);
        e.Graphics.DrawString(
            control.Items[e.Index].ToString(),
            e.Font,
            brush,
            e.Bounds);
        e.DrawFocusRectangle();
    }
}
