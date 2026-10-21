// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.UITests;

public class ComboBoxTests : ControlTestBase
{
    public ComboBoxTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task ComboBoxTest_ChangeAutoCompleteSource_DoesNotThrowAsync()
    {
        await RunSingleControlTestAsync<ComboBox>((form, comboBox) =>
        {
            // Test case captured from here.
            // https://github.com/dotnet/winforms/issues/6953
            comboBox.AutoCompleteCustomSource.AddRange(
            [
                "_sss",
                "_sss"
            ]);
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;

            return Task.CompletedTask;
        });
    }
}

/// <summary>
///  Exercises native ComboBox layout without input-simulation infrastructure.
/// </summary>
public class ComboBoxNativeLayoutTests
{
    [WinFormsFact]
    public void ComboBox_ModernVisualStyles_TableLayoutRoundTrips()
    {
        using Form form = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            VisualStylesMode = VisualStylesMode.Classic
        };
        using TableLayoutPanel table = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 1
        };
        table.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));
        table.RowStyles.Add(
            new RowStyle(SizeType.AutoSize));
        using ComboBox comboBox = new()
        {
            Anchor = AnchorStyles.Left
                | AnchorStyles.Top
                | AnchorStyles.Bottom,
            FlatStyle = FlatStyle.Standard,
            Size = new Size(180, 40)
        };
        table.Controls.Add(comboBox, 0, 0);
        form.Controls.Add(table);
        form.CreateControl();
        table.CreateControl();
        comboBox.CreateControl();
        _ = comboBox.Handle;

        form.VisualStylesMode = VisualStylesMode.Net11;
        form.PerformLayout();
        form.VisualStylesMode = VisualStylesMode.Classic;
        form.PerformLayout();
        var classicState = GetNativeComboState(comboBox);
        int handleCreatedCallCount = 0;
        comboBox.HandleCreated += (sender, e) =>
            handleCreatedCallCount++;

        for (int i = 0; i < 10; i++)
        {
            form.VisualStylesMode = VisualStylesMode.Net11;
            form.PerformLayout();
            Rectangle modernButtonBounds = GetButtonBounds(comboBox);
            Assert.True(modernButtonBounds.Width > 0);
            Assert.True(modernButtonBounds.Height > 0);
            using Bitmap bitmap = new(
                comboBox.Width,
                comboBox.Height);
            comboBox.DrawToBitmap(
                bitmap,
                new Rectangle(Point.Empty, comboBox.Size));

            form.VisualStylesMode = VisualStylesMode.Classic;
            form.PerformLayout();
            Assert.Equal(classicState, GetNativeComboState(comboBox));
            Assert.Equal((i + 1) * 2, handleCreatedCallCount);
        }
    }

    [WinFormsFact]
    public void ComboBox_ModernVisualStyles_DesignSurfacePropertyOrderConverges()
    {
        ServiceContainer services = new();
        using DesignSurface designSurface = new(services);
        bool loaded = false;
        designSurface.Loaded += (sender, e) =>
            loaded = true;
        designSurface.BeginLoad(
            new DesignBehaviorsTests.SampleDesignerLoader());
        Assert.True(loaded);

        var designerHost = (IDesignerHost)designSurface.GetService(
            typeof(IDesignerHost))!;
        var designedForm = (Form)designerHost.RootComponent;
        designedForm.Size = new Size(400, 220);
        designedForm.VisualStylesMode = VisualStylesMode.Classic;
        var comboBox = (ComboBox)designerHost.CreateComponent(
            typeof(ComboBox));
        comboBox.Size = new Size(180, 40);
        designedForm.Controls.Add(comboBox);

        var rootView = (Control)designSurface.View;
        rootView.CreateControl();
        comboBox.CreateControl();

        using Font font = new(
            Control.DefaultFont.FontFamily,
            11f);
        PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(comboBox);
        properties[nameof(ComboBox.FlatStyle)]!.SetValue(
            comboBox,
            FlatStyle.Popup);
        properties[nameof(ComboBox.Font)]!.SetValue(
            comboBox,
            font);
        properties[nameof(ComboBox.Padding)]!.SetValue(
            comboBox,
            new Padding(2, 3, 4, 5));
        properties[nameof(ComboBox.VisualStylesMode)]!.SetValue(
            comboBox,
            VisualStylesMode.Net11);
        var expectedModernState = GetNativeComboState(comboBox);

        properties[nameof(ComboBox.VisualStylesMode)]!.SetValue(
            comboBox,
            VisualStylesMode.Classic);
        properties[nameof(ComboBox.Padding)]!.SetValue(
            comboBox,
            Padding.Empty);
        properties[nameof(ComboBox.VisualStylesMode)]!.SetValue(
            comboBox,
            VisualStylesMode.Net11);
        properties[nameof(ComboBox.Padding)]!.SetValue(
            comboBox,
            new Padding(2, 3, 4, 5));
        properties[nameof(ComboBox.Font)]!.SetValue(
            comboBox,
            font);
        properties[nameof(ComboBox.FlatStyle)]!.SetValue(
            comboBox,
            FlatStyle.Popup);

        Assert.Equal(
            expectedModernState,
            GetNativeComboState(comboBox));
    }

    private static (
        Size size,
        int selectionHeight,
        nint margins,
        Rectangle editBounds,
        Rectangle itemBounds,
        Rectangle buttonBounds) GetNativeComboState(
            ComboBox comboBox)
    {
        COMBOBOXINFO comboBoxInfo = GetComboBoxInfo(comboBox);
        PInvokeCore.GetWindowRect(
            comboBoxInfo.hwndItem,
            out RECT editBounds);
        Point editTopLeft = comboBox.PointToClient(
            new Point(editBounds.left, editBounds.top));

        return (
            comboBox.Size,
            (int)PInvokeCore.SendMessage(
                comboBox,
                PInvoke.CB_GETITEMHEIGHT,
                (WPARAM)(-1)),
            PInvokeCore.SendMessage(
                comboBoxInfo.hwndItem,
                PInvokeCore.EM_GETMARGINS),
            new Rectangle(
                editTopLeft,
                new Size(editBounds.Width, editBounds.Height)),
            new Rectangle(
                comboBoxInfo.rcItem.left,
                comboBoxInfo.rcItem.top,
                comboBoxInfo.rcItem.Width,
                comboBoxInfo.rcItem.Height),
            GetButtonBounds(comboBoxInfo));
    }

    private static Rectangle GetButtonBounds(
        ComboBox comboBox)
        => GetButtonBounds(GetComboBoxInfo(comboBox));

    private static Rectangle GetButtonBounds(
        COMBOBOXINFO comboBoxInfo)
        => new(
            comboBoxInfo.rcButton.left,
            comboBoxInfo.rcButton.top,
            comboBoxInfo.rcButton.Width,
            comboBoxInfo.rcButton.Height);

    private static unsafe COMBOBOXINFO GetComboBoxInfo(
        ComboBox comboBox)
    {
        COMBOBOXINFO comboBoxInfo = default;
        comboBoxInfo.cbSize = (uint)sizeof(COMBOBOXINFO);
        Assert.True(PInvoke.GetComboBoxInfo(
            comboBox.HWND,
            ref comboBoxInfo));

        return comboBoxInfo;
    }
}
