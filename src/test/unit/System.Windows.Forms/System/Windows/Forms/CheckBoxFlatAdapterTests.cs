// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.Tests;

public class CheckBoxFlatAdapterTests : IDisposable
{
    private TestCheckBox? _checkBox;

    private (TestCheckBoxFlatAdapter, TestCheckBox) CreateAdapter(Appearance appearance, bool enabled)
    {
        _checkBox = new TestCheckBox
        {
            Appearance = appearance,
            Enabled = enabled
        };

        TestCheckBoxFlatAdapter checkBoxFlatAdapter = new(_checkBox);

        return (checkBoxFlatAdapter, _checkBox);
    }

    public void Dispose() => _checkBox?.Dispose();

    private class TestCheckBox : CheckBox
    {
        public new bool Enabled
        {
            get => base.Enabled;
            set => base.Enabled = value;
        }

        public new Appearance Appearance
        {
            get => base.Appearance;
            set => base.Appearance = value;
        }

        public new CheckState CheckState
        {
            get => base.CheckState;
            set => base.CheckState = value;
        }
    }

    private class TestCheckBoxFlatAdapter : CheckBoxFlatAdapter
    {
        public TestCheckBoxFlatAdapter(ButtonBase control) : base(control) { }

        public void CallPaintDown(PaintEventArgs e, CheckState state) => PaintDown(e, state);

        public bool PaintFlatWorkerCalled { get; private set; }

        protected void PaintFlatWorker() => PaintFlatWorkerCalled = true;
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, true)]
    [InlineData(Appearance.Button, false)]
    [InlineData(Appearance.Normal, true)]
    [InlineData(Appearance.Normal, false)]
    public void PaintDown_DoesNotThrow(Appearance appearance, bool enabled)
    {
        (TestCheckBoxFlatAdapter checkBoxFlatAdapter, TestCheckBox checkBox) = CreateAdapter(appearance, enabled);
        checkBox.CheckState = CheckState.Checked;

        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 20, 20));

        Exception? exception = Record.Exception(() =>
            checkBoxFlatAdapter.CallPaintDown(e, checkBox.CheckState));

        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, true)]
    [InlineData(Appearance.Button, false)]
    [InlineData(Appearance.Normal, true)]
    [InlineData(Appearance.Normal, false)]
    public void PaintOver_DoesNotThrow(Appearance appearance, bool enabled)
    {
        (TestCheckBoxFlatAdapter checkBoxFlatAdapter, TestCheckBox checkBox) = CreateAdapter(appearance, enabled);
        checkBox.CheckState = CheckState.Indeterminate;

        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 20, 20));

        Exception? exception = Record.Exception(() =>
            checkBoxFlatAdapter.PaintOver(e, checkBox.CheckState));

        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, true)]
    [InlineData(Appearance.Button, false)]
    [InlineData(Appearance.Normal, true)]
    [InlineData(Appearance.Normal, false)]
    public void PaintUp_DoesNotThrow(Appearance appearance, bool enabled)
    {
        (TestCheckBoxFlatAdapter checkBoxFlatAdapter, TestCheckBox checkBox) = CreateAdapter(appearance, enabled);
        checkBox.CheckState = CheckState.Unchecked;

        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 20, 20));

        Exception? exception = Record.Exception(() =>
            checkBoxFlatAdapter.PaintUp(e, checkBox.CheckState));

        exception.Should().BeNull();
    }

    [WinFormsFact]
    public void CreateButtonAdapter_ReturnsButtonFlatAdapter()
    {
        (TestCheckBoxFlatAdapter checkBoxFlatAdapter, _) = CreateAdapter(Appearance.Normal, true);

        ButtonBaseAdapter result = checkBoxFlatAdapter.TestAccessor().Dynamic.CreateButtonAdapter();

        result.Should().NotBeNull();
        result.Should().BeOfType<ButtonFlatAdapter>();
    }
}
