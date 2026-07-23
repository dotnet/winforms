// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Moq;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ComboBox;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ComboBoxTests
{
    [WinFormsFact]
    public void ComboBox_Ctor_Default()
    {
        using SubComboBox control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.True(control.AllowSelection);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Empty(control.AutoCompleteCustomSource);
        Assert.Same(control.AutoCompleteCustomSource, control.AutoCompleteCustomSource);
        Assert.Equal(AutoCompleteMode.None, control.AutoCompleteMode);
        Assert.Equal(AutoCompleteSource.None, control.AutoCompleteSource);
        Assert.False(control.AutoSize);
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(control.PreferredHeight, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 121, control.PreferredHeight), control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(117, control.PreferredHeight - 4), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 117, control.PreferredHeight - 4), control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Null(control.DataManager);
        Assert.Null(control.DataSource);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(121, control.PreferredHeight), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Empty(control.DisplayMember);
        Assert.Equal(new Rectangle(0, 0, 117, control.PreferredHeight - 4), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.Equal(DrawMode.Normal, control.DrawMode);
        Assert.Equal(106, control.DropDownHeight);
        Assert.Equal(ComboBoxStyle.DropDown, control.DropDownStyle);
        Assert.Equal(121, control.DropDownWidth);
        Assert.False(control.DroppedDown);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.Equal(FlatStyle.Standard, control.FlatStyle);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Null(control.FormatInfo);
        Assert.Empty(control.FormatString);
        Assert.False(control.FormattingEnabled);
        Assert.False(control.HasChildren);
        Assert.Equal(control.PreferredHeight, control.Height);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.True(control.IntegralHeight);
        Assert.Equal(Control.DefaultFont.Height + 2, control.ItemHeight);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.Empty(control.Items);
        Assert.Same(control.Items, control.Items);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(8, control.MaxDropDownItems);
        Assert.Equal(0, control.MaxLength);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.True(control.PreferredHeight > 0);
        Assert.Equal(new Size(121, control.PreferredHeight), control.PreferredSize);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(121, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Null(control.SelectedValue);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedItem);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(121, control.PreferredHeight), control.Size);
        Assert.False(control.Sorted);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.Empty(control.ValueMember);
        Assert.True(control.Visible);
        Assert.Equal(121, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ComboBox_ModernVisualStyles_AdapterSelection(
        FlatStyle flatStyle)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            FlatStyle = flatStyle,
            VisualStylesMode = VisualStylesMode.Net11
        };

        FlatComboAdapter adapter = control.CreateAdapter();

        if (flatStyle == FlatStyle.System)
        {
            Assert.IsType<FlatComboAdapter>(adapter);
        }
        else
        {
            Assert.IsType<ComboBox.ModernComboAdapter>(adapter);
        }

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<FlatStyle>]
    public void ComboBox_HighContrast_DoesNotSelectModernAdapter(
        FlatStyle flatStyle)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: true);
        using ComboBox control = new()
        {
            FlatStyle = flatStyle,
            VisualStylesMode = VisualStylesMode.Net11
        };

        FlatComboAdapter adapter = control.CreateFlatComboAdapterInstance();

        Assert.IsNotType<ComboBox.ModernComboAdapter>(adapter);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Standard)]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    public void ComboBox_ModernVisualStyles_PreferredHeightMatchesTextBox(
        FlatStyle flatStyle)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using Font font = new(
            Control.DefaultFont.FontFamily,
            11f);
        using TextBox textBox = new()
        {
            Font = font,
            Padding = new Padding(
                ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.ComboBoxStyleInset,
                    ScaleHelper.InitialSystemDpi)),
            VisualStylesMode = VisualStylesMode.Net11
        };
        using ComboBox comboBox = new()
        {
            FlatStyle = flatStyle,
            Font = font,
            VisualStylesMode = VisualStylesMode.Net11
        };

        Assert.Equal(textBox.PreferredHeight, comboBox.PreferredHeight);
        Assert.Equal(textBox.Height, comboBox.Height);

        textBox.CreateControl();
        comboBox.CreateControl();

        Assert.Equal(textBox.PreferredHeight, comboBox.PreferredHeight);
        Assert.Equal(textBox.Height, comboBox.Height);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown, FlatStyle.Standard)]
    [InlineData(ComboBoxStyle.DropDown, FlatStyle.Flat)]
    [InlineData(ComboBoxStyle.DropDown, FlatStyle.Popup)]
    [InlineData(ComboBoxStyle.DropDownList, FlatStyle.Standard)]
    [InlineData(ComboBoxStyle.DropDownList, FlatStyle.Flat)]
    [InlineData(ComboBoxStyle.DropDownList, FlatStyle.Popup)]
    [InlineData(ComboBoxStyle.Simple, FlatStyle.Standard)]
    [InlineData(ComboBoxStyle.Simple, FlatStyle.Flat)]
    [InlineData(ComboBoxStyle.Simple, FlatStyle.Popup)]
    public void ComboBox_ModernVisualStyles_DropDownStyleMatrixPaints(
        ComboBoxStyle dropDownStyle,
        FlatStyle flatStyle)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using Panel parent = new() { Size = new Size(180, 140) };
        using ComboBox control = new()
        {
            DropDownStyle = dropDownStyle,
            FlatStyle = flatStyle,
            Size = dropDownStyle == ComboBoxStyle.Simple
                ? new Size(160, 100)
                : new Size(160, 40),
            Text = "Modern combo",
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.Items.AddRange(["First", "Second"]);
        parent.Controls.Add(control);
        parent.CreateControl();
        control.CreateControl();
        using Bitmap bitmap = new(
            control.Width,
            control.Height);

        control.DrawToBitmap(
            bitmap,
            new Rectangle(Point.Empty, control.Size));

        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_ModernVisualStyles_SystemModeChangeDoesNotRequestLayout()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using Panel parent = new();
        using VisualStylesComboBox control = new()
        {
            FlatStyle = FlatStyle.System,
            VisualStylesMode = VisualStylesMode.Classic
        };
        parent.Controls.Add(control);
        int layoutCallCount = 0;
        parent.Layout += (sender, e) => layoutCallCount++;

        control.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Equal(0, layoutCallCount);
        Assert.IsType<FlatComboAdapter>(control.CreateAdapter());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_ModernVisualStyles_ModeChangeRemeasuresAutoSizeRow()
    {
        SystemVisualSettings previous =
            SystemVisualSettingsTracker.CurrentSettings;
        SystemVisualSettings modernMetrics = new(
            previous.AccentColor,
            1.5f,
            highContrastEnabled: false,
            clientAreaAnimationEnabled: false,
            previous.KeyboardCuesVisible,
            focusBorderMetrics: new Size(3, 3));

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(
                modernMetrics);
            using Form form = new()
            {
                VisualStylesMode = VisualStylesMode.Classic
            };
            using TableLayoutPanel tableLayoutPanel = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 1
            };
            tableLayoutPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));
            using Font font = new(
                Control.DefaultFont.FontFamily,
                11f);
            using VisualStylesComboBox control = new()
            {
                Anchor = AnchorStyles.Left
                    | AnchorStyles.Top
                    | AnchorStyles.Bottom,
                AutoSize = true,
                FlatStyle = FlatStyle.Standard,
                Font = font
            };
            tableLayoutPanel.Controls.Add(control, 0, 0);
            form.Controls.Add(tableLayoutPanel);
            form.CreateControl();
            _ = control.Handle;
            int handleCreatedCallCount = 0;
            control.HandleCreated += (sender, e) =>
                handleCreatedCallCount++;
            int classicControlHeight = control.Height;
            int classicHeight = tableLayoutPanel.GetRowHeights()[0];
            Size classicTableSize = tableLayoutPanel.Size;

            form.VisualStylesMode = VisualStylesMode.Net11;

            Assert.Equal(control.PreferredHeight, control.Height);
            Assert.NotEqual(classicControlHeight, control.Height);
            Assert.NotEqual(
                classicHeight,
                tableLayoutPanel.GetRowHeights()[0]);
            Assert.NotEqual(classicTableSize, tableLayoutPanel.Size);
            Assert.Equal(1, handleCreatedCallCount);

            form.VisualStylesMode = VisualStylesMode.Classic;

            Assert.Equal(classicControlHeight, control.Height);
            Assert.Equal(
                classicHeight,
                tableLayoutPanel.GetRowHeights()[0]);
            Assert.Equal(classicTableSize, tableLayoutPanel.Size);
            Assert.Equal(2, handleCreatedCallCount);
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting(previous);
        }
    }

    [WinFormsFact]
    public void ComboBox_ModernVisualStyles_RtlMirrorsDropDownButton()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox leftToRight = new()
        {
            FlatStyle = FlatStyle.Standard,
            RightToLeft = RightToLeft.No,
            Size = new Size(120, 32),
            VisualStylesMode = VisualStylesMode.Net11
        };
        using VisualStylesComboBox rightToLeft = new()
        {
            FlatStyle = FlatStyle.Standard,
            RightToLeft = RightToLeft.Yes,
            Size = new Size(120, 32),
            VisualStylesMode = VisualStylesMode.Net11
        };
        var leftToRightAdapter =
            (ComboBox.ModernComboAdapter)leftToRight.CreateAdapter();
        var rightToLeftAdapter =
            (ComboBox.ModernComboAdapter)rightToLeft.CreateAdapter();

        Rectangle leftToRightButton =
            leftToRightAdapter.TestAccessor.Dynamic._buttonBounds;
        Rectangle rightToLeftButton =
            rightToLeftAdapter.TestAccessor.Dynamic._buttonBounds;

        Assert.Equal(
            leftToRight.ClientRectangle.Right,
            leftToRightButton.Right);
        Assert.Equal(
            rightToLeft.ClientRectangle.Left,
            rightToLeftButton.Left);
    }

    [WinFormsFact]
    public void ComboBox_ModernVisualStyles_RoundedCornersPaintParentBackground()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using Bitmap backgroundImage = new(2, 1);
        backgroundImage.SetPixel(0, 0, Color.Red);
        backgroundImage.SetPixel(1, 0, Color.Blue);
        using Panel parent = new()
        {
            BackgroundImage = backgroundImage,
            BackgroundImageLayout = ImageLayout.Tile,
            Size = new Size(80, 40)
        };
        using VisualStylesComboBox control = new()
        {
            BackColor = Color.White,
            FlatStyle = FlatStyle.Standard,
            Size = new Size(60, 30),
            VisualStylesMode = VisualStylesMode.Net11
        };
        parent.Controls.Add(control);
        parent.CreateControl();
        control.CreateControl();
        using Bitmap actual = new(
            control.ClientSize.Width,
            control.ClientSize.Height);
        using Graphics graphics = Graphics.FromImage(actual);
        var adapter =
            (ComboBox.ModernComboAdapter)control.CreateAdapter();

        adapter.DrawFlatCombo(control, graphics);

        Assert.Equal(
            Color.Red.ToArgb(),
            actual.GetPixel(0, 0).ToArgb());
        Assert.True(
            ColorsAreClose(
                actual.GetPixel(1, 0),
                Color.Blue,
                channelTolerance: 16));
        Assert.Equal(
            backgroundImage.GetPixel(
                (actual.Width - 1) % backgroundImage.Width,
                0).ToArgb(),
            actual.GetPixel(actual.Width - 1, 0).ToArgb());
        Assert.Equal(
            Color.Red.ToArgb(),
            actual.GetPixel(0, actual.Height - 1).ToArgb());
        Assert.Equal(
            backgroundImage.GetPixel(
                (actual.Width - 1) % backgroundImage.Width,
                0).ToArgb(),
            actual.GetPixel(
                actual.Width - 1,
                actual.Height - 1).ToArgb());
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Standard, false)]
    [InlineData(FlatStyle.Flat, false)]
    [InlineData(FlatStyle.Popup, true)]
    public void ComboBox_ModernVisualStyles_FramesUseExpectedGeometryAndColor(
        FlatStyle flatStyle,
        bool usesAccent)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false,
            focusBorderMetrics: new Size(3, 3));
        using Panel parent = new()
        {
            BackColor = Color.Red,
            Size = new Size(100, 50)
        };
        using VisualStylesComboBox control = new()
        {
            BackColor = Color.White,
            FlatStyle = flatStyle,
            ForeColor = Color.Blue,
            Size = new Size(80, 32),
            VisualStylesMode = VisualStylesMode.Net11
        };
        parent.Controls.Add(control);
        parent.CreateControl();
        control.CreateControl();
        using Bitmap actual = new(
            control.ClientSize.Width,
            control.ClientSize.Height);
        using Graphics graphics = Graphics.FromImage(actual);
        var adapter =
            (ComboBox.ModernComboAdapter)control.CreateAdapter();

        adapter.DrawFlatCombo(control, graphics);

        Color expectedBorder = usesAccent
            ? Application.SystemVisualSettings.AccentColor
            : control.ForeColor;
        Assert.True(
            CountPixels(
                actual,
                expectedBorder,
                channelTolerance: 16) > 0);
        Assert.Equal(
            flatStyle == FlatStyle.Flat
                ? expectedBorder.ToArgb()
                : parent.BackColor.ToArgb(),
            actual.GetPixel(0, 0).ToArgb());
    }

    [WinFormsFact]
    public void ComboBox_Padding_HasDesignerSerializationConfiguration()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(
            typeof(ComboBox))[nameof(ComboBox.Padding)];
        EditorBrowsableAttribute editorBrowsable = Assert.IsType<EditorBrowsableAttribute>(
            property.Attributes[typeof(EditorBrowsableAttribute)]);
        DesignerSerializationVisibilityAttribute serializationVisibility =
            Assert.IsType<DesignerSerializationVisibilityAttribute>(
                property.Attributes[typeof(DesignerSerializationVisibilityAttribute)]);

        Assert.True(property.IsBrowsable);
        Assert.Equal(
            EditorBrowsableState.Always,
            editorBrowsable.State);
        Assert.Equal(
            DesignerSerializationVisibility.Visible,
            serializationVisibility.Visibility);
    }

    [WinFormsFact]
    public void ComboBox_Padding_ResetValue_RestoresEmpty()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(
            typeof(ComboBox))[nameof(ComboBox.Padding)];
        using ComboBox control = new();

        Assert.False(property.ShouldSerializeValue(control));
        control.Padding = new Padding(3, 4, 5, 6);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);

        Assert.Equal(Padding.Empty, control.Padding);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBox_ModernPadding_UpdatesHeightAndEditBounds(
        ComboBoxStyle dropDownStyle)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            DropDownStyle = dropDownStyle,
            FlatStyle = FlatStyle.Standard,
            Padding = new Padding(2, 3, 4, 5),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.CreateControl();
        IntPtr handle = control.Handle;

        Rectangle editBounds = control.GetEditBounds();
        Assert.True(
            editBounds.Left
                >= control.ModernChromeInsets.Left
                    + control.Padding.Left);
        if (dropDownStyle != ComboBoxStyle.Simple)
        {
            Assert.Equal(control.PreferredHeight, control.Height);
        }

        control.Padding = new Padding(5);

        Assert.Equal(handle, control.Handle);
        if (dropDownStyle != ComboBoxStyle.Simple)
        {
            Assert.Equal(control.PreferredHeight, control.Height);
        }

        Rectangle updatedEditBounds = control.GetEditBounds();
        Assert.True(updatedEditBounds.Left > editBounds.Left);
        Assert.True(updatedEditBounds.Width < editBounds.Width);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBox_ModernPadding_PositionsEditUsingTopAndBottom(
        ComboBoxStyle dropDownStyle)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            DropDownStyle = dropDownStyle,
            FlatStyle = FlatStyle.Standard,
            Padding = new Padding(2, 1, 2, 5),
            Size = dropDownStyle == ComboBoxStyle.Simple
                ? new Size(140, 100)
                : new Size(140, 40),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.CreateControl();
        Rectangle topWeightedBounds = control.GetEditBounds();
        Rectangle baseBounds = control.ModernEditBaseBounds;

        control.Padding = new Padding(2, 5, 2, 1);

        Rectangle bottomWeightedBounds = control.GetEditBounds();
        Assert.Equal(baseBounds, control.ModernEditBaseBounds);
        Assert.True(
            bottomWeightedBounds.Top
                > topWeightedBounds.Top);
        Assert.Equal(
            topWeightedBounds.Height,
            bottomWeightedBounds.Height);
        if (dropDownStyle == ComboBoxStyle.Simple)
        {
            Rectangle listBounds = control.GetListBounds();
            Assert.Equal(
                bottomWeightedBounds.Bottom
                    + control.ModernChromeInsets.Bottom
                    + control.Padding.Bottom,
                listBounds.Top);
        }
    }

    [WinFormsFact]
    public void ComboBox_ModernDropDownList_PreservesOwnerDrawnSelection()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            DrawMode = DrawMode.OwnerDrawFixed,
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Standard,
            Size = new Size(140, 36),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.Items.Add("Owner drawn");
        control.SelectedIndex = 0;
        control.CreateControl();
        var adapter =
            (ComboBox.ModernComboAdapter)control.CreateAdapter();
        using Bitmap actual = new(
            control.ClientSize.Width,
            control.ClientSize.Height);
        using Graphics graphics = Graphics.FromImage(actual);
        graphics.Clear(Color.Fuchsia);

        adapter.DrawFlatCombo(control, graphics);

        Assert.Equal(
            Color.Fuchsia.ToArgb(),
            actual.GetPixel(
                control.ClientSize.Width / 3,
                control.ClientSize.Height / 2).ToArgb());
    }

    [WinFormsFact]
    public void ComboBox_ModernDropDownList_FocusedSelectionUsesHighlight()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using Form form = new();
        using VisualStylesComboBox control = new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Standard,
            Size = new Size(140, 36),
            VisualStylesMode = VisualStylesMode.Net11
        };
        form.Controls.Add(control);
        form.CreateControl();
        control.CreateControl();
        PInvoke.SetFocus(control);
        Assert.True(control.ContainsFocus);
        var adapter =
            (ComboBox.ModernComboAdapter)control.CreateAdapter();
        using Bitmap actual = new(
            control.ClientSize.Width,
            control.ClientSize.Height);
        using Graphics graphics = Graphics.FromImage(actual);

        adapter.DrawFlatCombo(control, graphics);

        Rectangle textBounds = adapter.TestAccessor.Dynamic
            .GetDropDownListTextBounds(
                control,
                control.ClientRectangle);
        Assert.Equal(
            SystemColors.Highlight.ToArgb(),
            actual.GetPixel(
                textBounds.Left + (textBounds.Width / 2),
                textBounds.Top + (textBounds.Height / 2)).ToArgb());
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No)]
    [InlineData(RightToLeft.Yes)]
    public void ComboBox_ModernPadding_OffsetsDropDownListText(
        RightToLeft rightToLeft)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Standard,
            Padding = new Padding(3, 2, 5, 4),
            RightToLeft = rightToLeft,
            Size = new Size(140, 36),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.Items.Add("Padded item");
        control.SelectedIndex = 0;
        control.CreateControl();
        var adapter =
            (ComboBox.ModernComboAdapter)control.CreateAdapter();

        Rectangle textBounds = adapter.TestAccessor.Dynamic
            .GetDropDownListTextBounds(
                control,
                control.ClientRectangle);
        Padding effectivePadding = control.ModernFieldPadding;
        Rectangle buttonBounds =
            adapter.TestAccessor.Dynamic._buttonBounds;

        Assert.Equal(
            control.ClientRectangle.Top + effectivePadding.Top,
            textBounds.Top);
        Assert.Equal(
            control.ClientRectangle.Height
                - effectivePadding.Vertical,
            textBounds.Height);
        if (rightToLeft == RightToLeft.Yes)
        {
            Assert.Equal(
                buttonBounds.Right + effectivePadding.Left,
                textBounds.Left);
            Assert.Equal(
                control.ClientRectangle.Right
                    - effectivePadding.Right,
                textBounds.Right);
        }
        else
        {
            Assert.Equal(
                control.ClientRectangle.Left
                    + effectivePadding.Left,
                textBounds.Left);
            Assert.Equal(
                buttonBounds.Left - effectivePadding.Right,
                textBounds.Right);
        }
    }

    [WinFormsFact]
    public void ComboBox_ModernRoundedFrame_RepaintsChangedParentBackground()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using Panel parent = new()
        {
            BackColor = Color.Red,
            Size = new Size(100, 50)
        };
        using VisualStylesComboBox control = new()
        {
            BackColor = Color.White,
            FlatStyle = FlatStyle.Standard,
            Size = new Size(80, 32),
            VisualStylesMode = VisualStylesMode.Net11
        };
        parent.Controls.Add(control);
        parent.CreateControl();
        control.CreateControl();
        var adapter =
            (ComboBox.ModernComboAdapter)control.CreateAdapter();
        using Bitmap actual = new(
            control.ClientSize.Width,
            control.ClientSize.Height);
        using Graphics graphics = Graphics.FromImage(actual);

        adapter.DrawFlatCombo(control, graphics);
        Assert.Equal(
            Color.Red.ToArgb(),
            actual.GetPixel(0, 0).ToArgb());

        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
            invalidatedCallCount++;
        parent.BackColor = Color.Blue;
        graphics.Clear(Color.Transparent);
        adapter.DrawFlatCombo(control, graphics);

        Assert.True(invalidatedCallCount > 0);
        Assert.Equal(
            Color.Blue.ToArgb(),
            actual.GetPixel(0, 0).ToArgb());
    }

    [WinFormsFact]
    public void ComboBox_ModernSimple_ResizeRefreshesListBounds()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            DropDownStyle = ComboBoxStyle.Simple,
            FlatStyle = FlatStyle.Standard,
            Padding = new Padding(2),
            Size = new Size(140, 100),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.CreateControl();
        Rectangle initialListBounds = control.GetListBounds();

        control.Height += 40;

        Rectangle resizedListBounds = control.GetListBounds();
        Assert.True(
            resizedListBounds.Bottom
                > initialListBounds.Bottom);
        Assert.Equal(
            control.GetEditBounds().Bottom
                + control.ModernChromeInsets.Bottom
                + control.Padding.Bottom,
            resizedListBounds.Top);
    }

    [WinFormsFact]
    public void ComboBox_ModernSimple_ModeChangeRestoresNativeChildBounds()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            DropDownStyle = ComboBoxStyle.Simple,
            FlatStyle = FlatStyle.Standard,
            Padding = new Padding(3),
            Size = new Size(140, 100),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.CreateControl();
        using VisualStylesComboBox classic = new()
        {
            DropDownStyle = ComboBoxStyle.Simple,
            FlatStyle = FlatStyle.Standard,
            Padding = control.Padding,
            Size = control.Size,
            VisualStylesMode = VisualStylesMode.Classic
        };
        classic.CreateControl();

        control.VisualStylesMode = VisualStylesMode.Classic;

        Assert.Equal(
            classic.GetEditBounds(),
            control.GetEditBounds());
    }

    [WinFormsFact]
    public void ComboBox_ModernDropDown_ModeChangeResetsNativeChildGeometry()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            DropDownStyle = ComboBoxStyle.DropDown,
            FlatStyle = FlatStyle.Standard,
            Size = new Size(140, 40),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.CreateControl();
        _ = control.Handle;
        int handleCreatedCallCount = 0;
        control.HandleCreated += (sender, e) =>
            handleCreatedCallCount++;
        Rectangle nativeEditBounds = control.ModernEditBaseBounds;
        int nativeSelectionHeight = control.NativeSelectionHeight;

        control.VisualStylesMode = VisualStylesMode.Classic;

        Assert.Equal(1, handleCreatedCallCount);
        Assert.Equal(control.PreferredHeight, control.Height);
        Assert.Equal(nativeSelectionHeight, control.GetSelectionHeight());
        Assert.Equal(nativeEditBounds, control.GetEditBounds());
        Assert.IsNotType<ComboBox.ModernComboAdapter>(control.CreateAdapter());

        control.VisualStylesMode = VisualStylesMode.Net11;
        Assert.Equal(2, handleCreatedCallCount);
        Assert.NotEqual(nativeEditBounds, control.GetEditBounds());
        Assert.IsType<ComboBox.ModernComboAdapter>(control.CreateAdapter());

        control.VisualStylesMode = VisualStylesMode.Classic;

        Assert.Equal(3, handleCreatedCallCount);
        Assert.Equal(control.PreferredHeight, control.Height);
        Assert.Equal(nativeSelectionHeight, control.GetSelectionHeight());
        Assert.Equal(nativeEditBounds, control.GetEditBounds());
        Assert.IsNotType<ComboBox.ModernComboAdapter>(control.CreateAdapter());
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No)]
    [InlineData(RightToLeft.Yes)]
    public void ComboBox_ModernDropDown_RepeatedModeRoundTripsRestoreNativeState(
        RightToLeft rightToLeft)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            FlatStyle = FlatStyle.Standard,
            RightToLeft = rightToLeft,
            Size = new Size(160, 40),
            VisualStylesMode = VisualStylesMode.Classic
        };
        control.CreateControl();
        control.InitializeNativeComboBaseline();
        int handleCreatedCallCount = 0;
        control.HandleCreated += (sender, e) =>
            handleCreatedCallCount++;
        var classicState = GetNativeComboState(control);

        for (int i = 0; i < 10; i++)
        {
            control.VisualStylesMode = VisualStylesMode.Net11;
            Rectangle modernButtonBounds = control.GetButtonBounds();
            Assert.True(modernButtonBounds.Width > 0);
            Assert.True(modernButtonBounds.Height > 0);
            Assert.NotEqual(classicState, GetNativeComboState(control));

            control.VisualStylesMode = VisualStylesMode.Classic;
            Assert.Equal(classicState, GetNativeComboState(control));
        }

        Assert.Equal(20, handleCreatedCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ComboBox_ModernDropDown_PropertyOrderConverges(
        int order)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using Font font = new(
            Control.DefaultFont.FontFamily,
            11f);
        using VisualStylesComboBox expected = new()
        {
            FlatStyle = FlatStyle.Popup,
            Font = font,
            Padding = new Padding(2, 3, 4, 5),
            Size = new Size(180, 50),
            VisualStylesMode = VisualStylesMode.Net11
        };
        expected.CreateControl();
        var expectedState = GetNativeComboState(expected);

        using VisualStylesComboBox actual = new()
        {
            Size = expected.Size,
            VisualStylesMode = VisualStylesMode.Classic
        };
        switch (order)
        {
            case 0:
                actual.FlatStyle = FlatStyle.Popup;
                actual.Font = font;
                actual.Padding = expected.Padding;
                actual.VisualStylesMode = VisualStylesMode.Net11;
                actual.CreateControl();
                break;
            case 1:
                actual.VisualStylesMode = VisualStylesMode.Net11;
                actual.Padding = expected.Padding;
                actual.Font = font;
                actual.FlatStyle = FlatStyle.Popup;
                actual.CreateControl();
                break;
            case 2:
                actual.CreateControl();
                actual.FlatStyle = FlatStyle.Popup;
                actual.Font = font;
                actual.Padding = expected.Padding;
                actual.VisualStylesMode = VisualStylesMode.Net11;
                break;
            case 3:
                actual.CreateControl();
                actual.VisualStylesMode = VisualStylesMode.Net11;
                actual.Padding = expected.Padding;
                actual.Font = font;
                actual.FlatStyle = FlatStyle.Popup;
                break;
            default:
                throw new InvalidOperationException();
        }

        Assert.Equal(expectedState, GetNativeComboState(actual));
    }

    [WinFormsTheory]
    [InlineData(DrawMode.OwnerDrawFixed)]
    [InlineData(DrawMode.OwnerDrawVariable)]
    public void ComboBox_ModernOwnerDraw_PreservesSelectionFieldAuthority(
        DrawMode drawMode)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            DrawMode = drawMode,
            FlatStyle = FlatStyle.Standard,
            ItemHeight = 31,
            Size = new Size(160, 40),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.Items.AddRange(["First", "Second"]);
        control.CreateControl();
        int selectionHeight = control.GetSelectionHeight();

        control.ApplyModernComboLayout();
        control.VisualStylesMode = VisualStylesMode.Classic;
        control.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Equal(selectionHeight, control.GetSelectionHeight());
        Assert.Equal(control.PreferredHeight, control.Height);
    }

    [WinFormsFact]
    public void ComboBox_ModernSimple_FontChangeRecomputesNativeSplit()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using Font font = new(
            Control.DefaultFont.FontFamily,
            16f);
        using VisualStylesComboBox control = new()
        {
            DropDownStyle = ComboBoxStyle.Simple,
            FlatStyle = FlatStyle.Standard,
            Size = new Size(180, 120),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.CreateControl();
        Rectangle initialEditBounds = control.GetEditBounds();

        control.Font = font;

        Rectangle updatedEditBounds = control.GetEditBounds();
        Assert.NotEqual(initialEditBounds.Height, updatedEditBounds.Height);
        Assert.Equal(
            updatedEditBounds.Bottom
                + control.ModernChromeInsets.Bottom
                + control.Padding.Bottom,
            control.GetListBounds().Top);
        int writeCount = control.ModernComboLayoutWriteCount;
        var state = GetNativeComboState(control);

        control.ApplyModernComboLayout();

        Assert.Equal(state, GetNativeComboState(control));
        Assert.Equal(writeCount, control.ModernComboLayoutWriteCount);
    }

    [WinFormsFact]
    public void ComboBox_ModernVisualStyles_HandleRecreationImmediatelyReappliesLayout()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            FlatStyle = FlatStyle.Standard,
            Padding = new Padding(2),
            Size = new Size(180, 100),
            VisualStylesMode = VisualStylesMode.Net11
        };
        _ = control.Handle;
        Assert.True(
            control.GetEditBounds().Left
                >= control.ModernChromeInsets.Left
                    + control.Padding.Left);
        int handleCreatedCallCount = 0;
        control.HandleCreated += (sender, e) =>
            handleCreatedCallCount++;

        control.DropDownStyle = ComboBoxStyle.Simple;

        Assert.Equal(1, handleCreatedCallCount);
        Assert.True(
            control.GetEditBounds().Top
                >= control.ModernChromeInsets.Top
                    + control.Padding.Top);
        int writeCount = control.ModernComboLayoutWriteCount;

        control.ApplyModernComboLayout();

        Assert.Equal(writeCount, control.ModernComboLayoutWriteCount);
    }

    [WinFormsFact]
    public void ComboBox_ModernDropDown_RepeatedApplyPerformsNoNativeWrites()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            FlatStyle = FlatStyle.Standard,
            Padding = new Padding(3),
            Size = new Size(160, 40),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.CreateControl();
        int initialWriteCount = control.ModernComboLayoutWriteCount;
        var initialState = GetNativeComboState(control);

        for (int i = 0; i < 5; i++)
        {
            control.ApplyModernComboLayout();
        }

        Assert.Equal(initialWriteCount, control.ModernComboLayoutWriteCount);
        Assert.Equal(initialState, GetNativeComboState(control));
    }

    [WinFormsFact]
    public void ComboBox_ModernDropDown_EmptyPaddingKeepsEditInsideChrome()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using VisualStylesComboBox control = new()
        {
            FlatStyle = FlatStyle.Standard,
            Padding = Padding.Empty,
            Size = new Size(200, 60),
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.SetTestDeviceDpi(
            ScaleHelper.OneHundredPercentLogicalDpi);
        control.CreateControl();

        Rectangle editBounds = control.GetEditBounds();
        Padding chromeInsets = control.ModernChromeInsets;
        Assert.True(
            editBounds.Top >= chromeInsets.Top,
            $"Edit {editBounds} must start inside {chromeInsets}.");
        Assert.True(
            editBounds.Bottom
                <= control.ClientRectangle.Bottom - chromeInsets.Bottom,
            $"Edit {editBounds} must end inside {chromeInsets} within {control.ClientRectangle}.");
    }

    [WinFormsTheory]
    [InlineData(96)]
    [InlineData(144)]
    [InlineData(192)]
    public void ComboBox_ModernChromeInsets_ScaleWithDpi(
        int deviceDpi)
    {
        using VisualStylesComboBox control = new();
        control.SetTestDeviceDpi(deviceDpi);

        Padding chromeInsets = control.ModernChromeInsets;

        Assert.Equal(
            ScaleHelper.ScaleToDpi(
                ModernControlVisualStyles.BorderThickness
                    + ModernControlVisualStyles.ComboBoxStyleInset
                    + ModernControlVisualStyles.ComboBoxFieldArcClearance,
                deviceDpi),
            chromeInsets.Left);
        Assert.Equal(
            ScaleHelper.ScaleToDpi(
                ModernControlVisualStyles.BorderThickness
                    + ModernControlVisualStyles.ComboBoxStyleInset
                    + ModernControlVisualStyles.ComboBoxFieldArcClearance,
                deviceDpi),
            chromeInsets.Top);
    }

    [WinFormsFact]
    public void ComboBox_ModernSimple_RepeatedMetricChangeDoesNotAccumulatePadding()
    {
        SystemVisualSettings previous =
            SystemVisualSettingsTracker.CurrentSettings;
        SystemVisualSettings initial = new(
            previous.AccentColor,
            previous.TextScaleFactor,
            highContrastEnabled: false,
            previous.ClientAreaAnimationEnabled,
            previous.KeyboardCuesVisible,
            focusBorderMetrics: new Size(1, 1));
        SystemVisualSettings changed = new(
            previous.AccentColor,
            previous.TextScaleFactor,
            highContrastEnabled: false,
            previous.ClientAreaAnimationEnabled,
            previous.KeyboardCuesVisible,
            focusBorderMetrics: new Size(2, 2));

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial);
            using VisualStylesComboBox control = new()
            {
                DropDownStyle = ComboBoxStyle.Simple,
                FlatStyle = FlatStyle.Standard,
                Padding = new Padding(3),
                Size = new Size(140, 100),
                VisualStylesMode = VisualStylesMode.Net11
            };
            control.CreateControl();
            SystemVisualSettingsChangedEventArgs eventArgs = new(
                initial,
                changed,
                SystemVisualSettingsCategories.FocusMetrics);

            SystemVisualSettingsTracker.ResetForTesting(changed);
            control.RaiseSystemVisualSettingsChanged(eventArgs);
            Rectangle firstEditBounds = control.GetEditBounds();
            Rectangle firstListBounds = control.GetListBounds();

            control.RaiseSystemVisualSettingsChanged(eventArgs);

            Assert.Equal(firstEditBounds, control.GetEditBounds());
            Assert.Equal(firstListBounds, control.GetListBounds());
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting(previous);
        }
    }

    [Theory]
    [InlineData(10, 0, 21999, false)]
    [InlineData(10, 0, 22000, true)]
    [InlineData(10, 1, 0, true)]
    public void ComboBox_SupportsModernDropDownCorners_ReturnsExpected(
        int major,
        int minor,
        int build,
        bool expected)
    {
        Assert.Equal(
            expected,
            ComboBox.SupportsModernDropDownCorners(
                new Version(major, minor, build)));
    }

    private static (
        Size size,
        int selectionHeight,
        (int left, int right) margins,
        Rectangle editBounds,
        Rectangle itemBounds,
        Rectangle buttonBounds) GetNativeComboState(
            VisualStylesComboBox control)
        => (
            control.Size,
            control.GetSelectionHeight(),
            control.GetEditMargins(),
            control.GetEditBounds(),
            control.GetItemBounds(),
            control.GetButtonBounds());

    [WinFormsFact]
    public void ComboBox_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubComboBox control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("ComboBox", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(control.PreferredHeight, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56210242, createParams.Style);
        Assert.Equal(121, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<AutoCompleteMode>]
    public void ComboBox_AutoCompleteMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(AutoCompleteMode value)
    {
        using ComboBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoCompleteMode = value);
    }

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void ComboBox_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using ComboBox control = new()
        {
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window, 0 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithHandle_TestData))]
    public void ComboBox_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using ComboBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(-1, 0)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void ComboBox_MaxLength_Set_GetReturnsExpected(int value, int expected)
    {
        using ComboBox control = new();
        control.MaxLength = value;
        control.MaxLength.Should().Be(expected);
        control.IsHandleCreated.Should().BeFalse();

        if (control.Handle != IntPtr.Zero)
        {
            control.MaxLength = value;
            control.MaxLength.Should().Be(expected);
            control.IsHandleCreated.Should().BeTrue();
        }
    }

    [WinFormsFact]
    public void ComboBox_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using ComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackColorChanged += handler;

        // Set different.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(1, callCount);

        // Set different.
        control.BackColor = Color.Empty;
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ComboBox_BackColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ComboBox))[nameof(ComboBox.BackColor)];
        using ComboBox control = new();
        Assert.False(property.CanResetValue(control));

        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void ComboBox_BackColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ComboBox))[nameof(ComboBox.BackColor)];
        using ComboBox control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsFact]
    public void ComboBox_MeasureItem_AddHandler()
    {
        using ComboBox control = new();
        int initialItemHeight = control.ItemHeight;

        MeasureItemEventHandler handler = (sender, e) => { };
        control.MeasureItem += handler;

        control.ItemHeight.Should().NotBe(initialItemHeight);
    }

    private class TestComboBox : ComboBox
    {
        public void TriggerMeasureItem(MeasureItemEventArgs e) =>
            base.OnMeasureItem(e);
    }

    [WinFormsFact]
    public void ComboBox_MeasureItem_RemoveHandler()
    {
        using TestComboBox control = new();
        int handlerCallCount = 0;
        MeasureItemEventHandler handler = (sender, e) => { handlerCallCount++; };

        control.MeasureItem += handler;

        // Simulate the MeasureItem event
        control.TriggerMeasureItem(new MeasureItemEventArgs(Graphics.FromHwnd(IntPtr.Zero), 0, 0));

        control.MeasureItem -= handler;

        // Simulate the MeasureItem event again to ensure the handler was removed
        control.TriggerMeasureItem(new MeasureItemEventArgs(Graphics.FromHwnd(IntPtr.Zero), 0, 0));

        handlerCallCount.Should().Be(1, "The MeasureItem event handler was not removed as expected.");
    }

    private class TestableComboBox : ComboBox
    {
        public void InvokeOnPaint(PaintEventArgs e) => base.OnPaint(e);
    }

    [WinFormsFact]
    public void ComboBox_Paint_AddHandler_ShouldSubscribeEvent()
    {
        using TestableComboBox comboBox = new();
        int callCount = 0;

        PaintEventHandler handler = (sender, e) => callCount++;

        comboBox.Paint += handler;
        comboBox.TestAccessor.Dynamic.OnPaint(new PaintEventArgs(Graphics.FromHwnd(comboBox.Handle), default));

        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ComboBox_Paint_RemoveHandler_ShouldUnsubscribeEvent()
    {
        using TestableComboBox comboBox = new();
        int callCount = 0;

        PaintEventHandler handler = (sender, e) => callCount++;
        comboBox.Paint += handler;
        comboBox.InvokeOnPaint(new PaintEventArgs(Graphics.FromHwnd(comboBox.Handle), default));
        callCount.Should().Be(1);

        comboBox.Paint -= handler;
        comboBox.InvokeOnPaint(new PaintEventArgs(Graphics.FromHwnd(comboBox.Handle), default));
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ComboBox_Paint_EventHandlerCalledOnPaint()
    {
        using TestableComboBox comboBox = new();
        using Bitmap bitmap = new(100, 100);
        Graphics graphics = Graphics.FromImage(bitmap);
        bool handlerCalled = false;

        comboBox.Paint += (sender, e) => handlerCalled = true;
        comboBox.InvokeOnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 100, 100)));

        handlerCalled.Should().BeTrue();
    }

    [WinFormsFact]
    public void VerifyAutoCompleteEntries()
    {
        static void AssertAutoCompleteCustomSource(string[] items, bool isHandleCreated)
        {
            using ComboBox control = new();

            if (items is not null)
            {
                AutoCompleteStringCollection autoCompleteCustomSource = [.. items];
                control.AutoCompleteCustomSource = autoCompleteCustomSource;
                control.AutoCompleteCustomSource.Should().BeEquivalentTo(autoCompleteCustomSource);
            }
            else
            {
                control.AutoCompleteCustomSource = null;
                control.AutoCompleteCustomSource.Should().NotBeNull();
                control.AutoCompleteCustomSource.Count.Should().Be(0);
            }

            control.IsHandleCreated.Should().Be(isHandleCreated);
        }

        AssertAutoCompleteCustomSource(["item1", "item2"], false);
        AssertAutoCompleteCustomSource(null, false);
        AssertAutoCompleteCustomSource(["item3", "item4"], false);
    }

    [WinFormsFact]
    public void ComboBox_BeginEndUpdate()
    {
        using ComboBox control1 = new();
        control1.BeginUpdate();
        control1.EndUpdate();

        using ComboBox control2 = new() { AutoCompleteSource = AutoCompleteSource.ListItems };
        control2.BeginUpdate();
        control2.EndUpdate();
        control2.AutoCompleteMode.Should().Be(AutoCompleteMode.None);

        using ComboBox control3 = new();
        control3.BeginUpdate();
        control3.CreateControl();
        control3.EndUpdate();
        control3.IsHandleCreated.Should().BeTrue();

        using ComboBox control4 = new();
        Exception exception = Record.Exception(control4.EndUpdate);
        exception.Should().BeNull();
    }

    [WinFormsFact]
    public void ComboBox_SelectedTextTests()
    {
        using ComboBox control = new();

        control.IsHandleCreated.Should().BeFalse();
        {
            control.SelectedText.Should().BeEmpty();
        }

        control.CreateControl();
        control.IsHandleCreated.Should().BeTrue();
        {
            control.SelectedText.Should().BeEmpty();
        }

        control.DropDownStyle = ComboBoxStyle.DropDownList;
        control.CreateControl();
        if (control.DropDownStyle == ComboBoxStyle.DropDownList)
        {
            control.SelectedText.Should().BeEmpty();
        }

        // Test SetWithoutHandle
        control.CreateControl();
        {
            control.SelectedText = "Test";
            control.SelectedText.Should().BeEmpty();
        }

        // Test SetWithHandle
        control.DropDownStyle = ComboBoxStyle.DropDown;
        control.Text = "Initial";
        control.SelectionStart = 0;
        control.SelectionLength = 7;
        control.CreateControl();
        {
            control.SelectedText = "Test";
            control.Text.Should().Be("Test");
        }

        // Test SetWithDropDownListStyle
        control.DropDownStyle = ComboBoxStyle.DropDownList;
        control.CreateControl();
        if (control.DropDownStyle == ComboBoxStyle.DropDownList)
        {
            control.SelectedText = "Test";
            control.SelectedText.Should().BeEmpty();
        }
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void ComboBox_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using ComboBox control = new()
        {
            BackgroundImage = value
        };
        Assert.Same(value, control.BackgroundImage);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using ComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackgroundImageChanged += handler;

        // Set different.
        using Bitmap image1 = new(10, 10);
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set different.
        using Bitmap image2 = new(10, 10);
        control.BackgroundImage = image2;
        Assert.Same(image2, control.BackgroundImage);
        Assert.Equal(2, callCount);

        // Set null.
        control.BackgroundImage = null;
        Assert.Null(control.BackgroundImage);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.BackgroundImageChanged -= handler;
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void ComboBox_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using ComboBox control = new()
        {
            BackgroundImageLayout = value
        };
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using ComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackgroundImageLayoutChanged += handler;

        // Set different.
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(1, callCount);

        // Set different.
        control.BackgroundImageLayout = ImageLayout.Stretch;
        Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackgroundImageLayoutChanged -= handler;
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImageLayout>]
    public void ComboBox_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using ComboBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
    }

    public static IEnumerable<object[]> DataSource_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new List<int>() };
        yield return new object[] { Array.Empty<int>() };

        Mock<IListSource> mockSource = new(MockBehavior.Strict);
        mockSource
            .Setup(s => s.GetList())
            .Returns(new int[] { 1 });
        yield return new object[] { mockSource.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ComboBox_DataSource_Set_GetReturnsExpected(object value)
    {
        using SubComboBox control = new()
        {
            DataSource = value
        };
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_DataSource_SetWithHandler_CallsDataSourceChanged()
    {
        using ComboBox control = new();
        int dataSourceCallCount = 0;
        int displayMemberCallCount = 0;
        EventHandler dataSourceHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            dataSourceCallCount++;
        };
        EventHandler displayMemberHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            displayMemberCallCount++;
        };
        control.DataSourceChanged += dataSourceHandler;
        control.DisplayMemberChanged += displayMemberHandler;

        // Set different.
        List<int> dataSource1 = [];
        control.DataSource = dataSource1;
        Assert.Same(dataSource1, control.DataSource);
        Assert.Equal(0, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);

        // Set same.
        control.DataSource = dataSource1;
        Assert.Same(dataSource1, control.DataSource);
        Assert.Equal(0, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);

        // Set different.
        List<int> dataSource2 = [];
        control.DataSource = dataSource2;
        Assert.Same(dataSource2, control.DataSource);
        Assert.Equal(0, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);

        // Set null.
        control.DataSource = null;
        Assert.Null(control.DataSource);
        Assert.Equal(0, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);

        // Remove handler.
        control.DataSourceChanged -= dataSourceHandler;
        control.DisplayMemberChanged -= displayMemberHandler;
        control.DataSource = dataSource1;
        Assert.Same(dataSource1, control.DataSource);
        Assert.Equal(0, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);
    }

    public class CustomComboBox : ComboBox
    {
        // Make methods private to improve encapsulation
        private void RaiseOnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);

        private void TriggerDoubleClick() => base.OnDoubleClick(EventArgs.Empty);

        // Public methods to trigger the private methods for testing purposes
        public void TestRaiseOnDrawItem(DrawItemEventArgs e) => RaiseOnDrawItem(e);

        public void TestTriggerDoubleClick() => TriggerDoubleClick();
    }

    [WinFormsFact]
    public void ComboBox_DrawItem_AddHandler_ShouldCallHandler()
    {
        using CustomComboBox control = new();
        using Bitmap bitmap = new(1, 1);
        Graphics graphics = Graphics.FromImage(bitmap);
        int callCount = 0;

        DrawItemEventHandler handler = (sender, e) => callCount++;

        control.DrawItem += handler;
        control.TestRaiseOnDrawItem(new DrawItemEventArgs(graphics, control.Font, default, 0, DrawItemState.Default));

        callCount.Should().Be(1);
        control.DrawItem -= handler;
    }

    [WinFormsFact]
    public void ComboBox_DrawItem_RemoveHandler_ShouldNotCallHandler()
    {
        using CustomComboBox control = new();
        int callCount = 0;

        DrawItemEventHandler handler = (sender, e) => callCount++;

        control.DrawItem += handler;
        control.DrawItem -= handler;
        using Bitmap bitmap = new(1, 1);
        control.TestRaiseOnDrawItem(new DrawItemEventArgs(Graphics.FromImage(bitmap), control.Font, default, 0, DrawItemState.Default));

        callCount.Should().Be(0);
    }

    [WinFormsFact]
    public void ComboBox_DrawItem_AddNullHandler_ShouldNotThrow()
    {
        using CustomComboBox control = new();

        // No exception means the test passed.
        control.DrawItem += null;
        control.DrawItem -= null;
    }

    [WinFormsFact]
    public void ComboBox_DrawItem_AddRemoveMultipleHandlers_ShouldCallHandlers()
    {
        using CustomComboBox control = new();
        using Bitmap bitmap = new(1, 1);
        Graphics graphics = Graphics.FromImage(bitmap);
        int callCount1 = 0;
        int callCount2 = 0;

        DrawItemEventHandler handler1 = (sender, e) => callCount1++;
        DrawItemEventHandler handler2 = (sender, e) => callCount2++;

        control.DrawItem += handler1;
        control.DrawItem += handler2;
        control.TestRaiseOnDrawItem(new DrawItemEventArgs(graphics, control.Font, default, 0, DrawItemState.Default));

        callCount1.Should().Be(1);
        callCount2.Should().Be(1);

        control.DrawItem -= handler1;
        control.TestRaiseOnDrawItem(new DrawItemEventArgs(graphics, control.Font, default, 0, DrawItemState.Default));

        callCount1.Should().Be(1); // Should not increase
        callCount2.Should().Be(2); // Should increase
    }

    [WinFormsFact]
    public void ComboBox_DrawItem_RemoveNonExistentHandler_ShouldNotThrow()
    {
        using CustomComboBox control = new();
        DrawItemEventHandler handler = (sender, e) => { };

        // No exception means the test passed.
        control.DrawItem -= handler;
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(0)]
    public void ComboBox_DoubleClick_AddRemoveEvent_Success(int expectedCallCount)
    {
        using CustomComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;

        if (expectedCallCount == 1)
        {
            control.DoubleClick += handler;
        }

        control.TestTriggerDoubleClick();
        callCount.Should().Be(expectedCallCount);

        control.DoubleClick -= handler;
        control.TestTriggerDoubleClick();
        callCount.Should().Be(expectedCallCount);
    }

    [WinFormsTheory]
    [EnumData<ComboBoxStyle>]
    public void ComboBox_DropDownStyle_Set_GetReturnsExpected(ComboBoxStyle value)
    {
        using ComboBox control = new()
        {
            DropDownStyle = value
        };
        Assert.Equal(value, control.DropDownStyle);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DropDownStyle = value;
        Assert.Equal(value, control.DropDownStyle);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> DropDownStyle_Set_TestData()
    {
        foreach (AutoCompleteSource source in Enum.GetValues(typeof(AutoCompleteSource)))
        {
            foreach (AutoCompleteMode mode in Enum.GetValues(typeof(AutoCompleteMode)))
            {
                yield return new object[] { source, mode, ComboBoxStyle.Simple, mode };
                yield return new object[] { source, mode, ComboBoxStyle.DropDown, mode };
                yield return new object[] { source, mode, ComboBoxStyle.DropDownList, source != AutoCompleteSource.ListItems ? AutoCompleteMode.None : mode };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DropDownStyle_Set_TestData))]
    public void ComboBox_DropDownStyle_SetWithSourceAndMode_GetReturnsExpected(AutoCompleteSource source, AutoCompleteMode mode, ComboBoxStyle value, AutoCompleteMode expectedMode)
    {
        using ComboBox control = new()
        {
            AutoCompleteSource = source,
            AutoCompleteMode = mode,
            DropDownStyle = value
        };
        Assert.Equal(value, control.DropDownStyle);
        Assert.Equal(source, control.AutoCompleteSource);
        Assert.Equal(expectedMode, control.AutoCompleteMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DropDownStyle = value;
        Assert.Equal(value, control.DropDownStyle);
        Assert.Equal(source, control.AutoCompleteSource);
        Assert.Equal(expectedMode, control.AutoCompleteMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<AutoCompleteSource>]
    public void ComboBox_AutoCompleteSource_Set_GetReturnsExpected(AutoCompleteSource value)
    {
        using ComboBox control = new();
        control.AutoCompleteSource = value;
        Assert.Equal(value, control.AutoCompleteSource);
    }

    [WinFormsTheory]
    [InvalidEnumData<AutoCompleteSource>]
    public void ComboBox_AutoCompleteSource_InvalidAutoCompleteSource_ThrowsInvalidEnumArgumentException(AutoCompleteSource source)
    {
        using ComboBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoCompleteSource = source);
    }

    [WinFormsFact]
    public void ComboBox_DropDownStyle_SetWithPreferredHeight_ResetsPreferredHeight()
    {
        using ComboBox control = new()
        {
            FormattingEnabled = true
        };
        int height1 = control.PreferredHeight;

        control.DropDownStyle = ComboBoxStyle.DropDownList;
        Assert.Equal(height1, control.PreferredHeight);

        control.DropDownStyle = ComboBoxStyle.Simple;
        int height2 = control.PreferredHeight;
        Assert.True(height2 > height1);

        control.DropDownStyle = ComboBoxStyle.DropDownList;
        Assert.Equal(height1, control.PreferredHeight);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.Simple, 1)]
    [InlineData(ComboBoxStyle.DropDown, 0)]
    [InlineData(ComboBoxStyle.DropDownList, 1)]
    public void ComboBox_DropDownStyle_SetWithHandle_GetReturnsExpected(ComboBoxStyle value, int expectedCreatedCallCount)
    {
        using ComboBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.DropDownStyle = value;
        Assert.Equal(value, control.DropDownStyle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.DropDownStyle = value;
        Assert.Equal(value, control.DropDownStyle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsFact]
    public void ComboBox_DropDownStyle_SetWithHandler_CallsDropDownStyleChanged()
    {
        using ComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.DropDownStyleChanged += handler;

        // Set different.
        control.DropDownStyle = ComboBoxStyle.DropDownList;
        Assert.Equal(ComboBoxStyle.DropDownList, control.DropDownStyle);
        Assert.Equal(1, callCount);

        // Set same.
        control.DropDownStyle = ComboBoxStyle.DropDownList;
        Assert.Equal(ComboBoxStyle.DropDownList, control.DropDownStyle);
        Assert.Equal(1, callCount);

        // Set different.
        control.DropDownStyle = ComboBoxStyle.Simple;
        Assert.Equal(ComboBoxStyle.Simple, control.DropDownStyle);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.DropDownStyleChanged -= handler;
        control.DropDownStyle = ComboBoxStyle.DropDownList;
        Assert.Equal(ComboBoxStyle.DropDownList, control.DropDownStyle);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ComboBoxStyle>]
    public void ComboBox_DropDownStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(ComboBoxStyle value)
    {
        using ComboBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.DropDownStyle = value);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ComboBox_Font_Set_GetReturnsExpected(Font value)
    {
        using SubComboBox control = new()
        {
            Font = value
        };
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_Font_SetWithHandler_CallsFontChanged()
    {
        using ComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FontChanged += handler;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Equal(2, callCount);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(3, callCount);
    }

    public static IEnumerable<object[]> ForeColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.WindowText };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
        yield return new object[] { Color.White, Color.White };
        yield return new object[] { Color.Black, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void ComboBox_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using ComboBox control = new()
        {
            ForeColor = value
        };
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.WindowText, 0 };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
        yield return new object[] { Color.White, Color.White, 1 };
        yield return new object[] { Color.Black, Color.Black, 1 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
    public void ComboBox_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using ComboBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ComboBox_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using ComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ForeColorChanged += handler;

        // Set different.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(1, callCount);

        // Set same.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(1, callCount);

        // Set different.
        control.ForeColor = Color.Empty;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ComboBox_ForeColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ComboBox))[nameof(ComboBox.ForeColor)];
        using ComboBox control = new();
        Assert.False(property.CanResetValue(control));

        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void ComboBox_ForeColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ComboBox))[nameof(ComboBox.ForeColor)];
        using ComboBox control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void ComboBox_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using ComboBox control = new()
        {
            Padding = value
        };
        Assert.Equal(expected, control.Padding);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void ComboBox_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using ComboBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ComboBox_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using ComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set same.
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set different.
        Padding padding2 = new(2);
        control.Padding = padding2;
        Assert.Equal(padding2, control.Padding);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetRightToLeftTheoryData))]
    public void ComboBox_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
    {
        using ComboBox control = new()
        {
            RightToLeft = value
        };
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
    {
        using ComboBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RightToLeftChanged += handler;

        // Set different.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(1, callCount);

        // Set same.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(1, callCount);

        // Set different.
        control.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.RightToLeftChanged -= handler;
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<RightToLeft>]
    public void ComboBox_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
    {
        using ComboBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
    }

    [WinFormsTheory]
    [InlineData(-1, "")]
    [InlineData(0, "System.Windows.Forms.Tests.ComboBoxTests+DataClass")]
    [InlineData(1, "System.Windows.Forms.Tests.ComboBoxTests+DataClass")]
    public void ComboBox_SelectedIndex_SetWithoutDisplayMember_GetReturnsExpected(int value, string expectedText)
    {
        using ComboBox control = new();
        control.Items.Add(new DataClass { Value = "Value1" });
        control.Items.Add(new DataClass { Value = "Value2" });

        control.SelectedIndex = value;
        Assert.Equal(value, control.SelectedIndex);
        Assert.Equal(value == -1 ? null : control.Items[control.SelectedIndex], control.SelectedItem);
        Assert.Equal(expectedText, control.Text);

        // Set same.
        control.SelectedIndex = value;
        Assert.Equal(value, control.SelectedIndex);
        Assert.Equal(value == -1 ? null : control.Items[control.SelectedIndex], control.SelectedItem);
        Assert.Equal(expectedText, control.Text);
    }

    [WinFormsTheory]
    [InlineData(-1, "")]
    [InlineData(0, "Value1")]
    [InlineData(1, "Value2")]
    public void ComboBox_SelectedIndex_SetWithDisplayMember_GetReturnsExpected(int value, string expectedText)
    {
        using ComboBox control = new()
        {
            DisplayMember = "Value"
        };
        control.Items.Add(new DataClass { Value = "Value1" });
        control.Items.Add(new DataClass { Value = "Value2" });

        control.SelectedIndex = value;
        Assert.Equal(value, control.SelectedIndex);
        Assert.Equal(value == -1 ? null : control.Items[control.SelectedIndex], control.SelectedItem);
        Assert.Equal(expectedText, control.Text);

        // Set same.
        control.SelectedIndex = value;
        Assert.Equal(value, control.SelectedIndex);
        Assert.Equal(value == -1 ? null : control.Items[control.SelectedIndex], control.SelectedItem);
        Assert.Equal(expectedText, control.Text);
    }

    [WinFormsFact]
    public void ComboBox_SelectedText_GetWithoutHandle_ReturnsExpected()
    {
        using ComboBox control = new();
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Get again.
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_SelectedText_GetWithHandle_ReturnsExpected()
    {
        using ComboBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Get again.
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_SelectionLength_GetWithoutHandle_ReturnsExpected()
    {
        using ComboBox control = new();
        Assert.Equal(0, control.SelectionLength);
        Assert.True(control.IsHandleCreated);

        // Get again.
        Assert.Equal(0, control.SelectionLength);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_SelectionLength_GetWithHandle_ReturnsExpected()
    {
        using ComboBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(0, control.SelectionLength);
        Assert.True(control.IsHandleCreated);

        // Get again.
        Assert.Equal(0, control.SelectionLength);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_SelectionStart_GetWithoutHandle_ReturnsExpected()
    {
        using ComboBox control = new();
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated); // SelectionStart forces Handle creating

        // Get again.
        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_SelectionStart_GetWithHandle_ReturnsExpected()
    {
        using ComboBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);

        // Get again.
        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubComboBox control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, false)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, false)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, false)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void ComboBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubComboBox control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ComboBox_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubComboBox control = new();
        Assert.False(control.GetTopLevel());
    }

    public static IEnumerable<object[]> FindString_TestData()
    {
        foreach (int startIndex in new int[] { -2, -1, 0, 1 })
        {
            yield return new object[] { new ComboBox(), null, startIndex, -1 };
            yield return new object[] { new ComboBox(), string.Empty, startIndex, -1 };
            yield return new object[] { new ComboBox(), "s", startIndex, -1 };

            using ComboBox controlWithNoItems = new();
            Assert.Empty(controlWithNoItems.Items);
            yield return new object[] { new ComboBox(), null, startIndex, -1 };
            yield return new object[] { new ComboBox(), string.Empty, startIndex, -1 };
            yield return new object[] { new ComboBox(), "s", startIndex, -1 };
        }

        using ComboBox controlWithItems = new()
        {
            DisplayMember = "Value"
        };
        controlWithItems.Items.Add(new DataClass { Value = "abc" });
        controlWithItems.Items.Add(new DataClass { Value = "abc" });
        controlWithItems.Items.Add(new DataClass { Value = "ABC" });
        controlWithItems.Items.Add(new DataClass { Value = "def" });
        controlWithItems.Items.Add(new DataClass { Value = "" });
        controlWithItems.Items.Add(new DataClass { Value = null });

        yield return new object[] { controlWithItems, "abc", -1, 0 };
        yield return new object[] { controlWithItems, "abc", 0, 1 };
        yield return new object[] { controlWithItems, "abc", 1, 2 };
        yield return new object[] { controlWithItems, "abc", 2, 0 };
        yield return new object[] { controlWithItems, "abc", 5, 0 };

        yield return new object[] { controlWithItems, "ABC", -1, 0 };
        yield return new object[] { controlWithItems, "ABC", 0, 1 };
        yield return new object[] { controlWithItems, "ABC", 1, 2 };
        yield return new object[] { controlWithItems, "ABC", 2, 0 };
        yield return new object[] { controlWithItems, "ABC", 5, 0 };

        yield return new object[] { controlWithItems, "a", -1, 0 };
        yield return new object[] { controlWithItems, "a", 0, 1 };
        yield return new object[] { controlWithItems, "a", 1, 2 };
        yield return new object[] { controlWithItems, "a", 2, 0 };
        yield return new object[] { controlWithItems, "a", 5, 0 };

        yield return new object[] { controlWithItems, "A", -1, 0 };
        yield return new object[] { controlWithItems, "A", 0, 1 };
        yield return new object[] { controlWithItems, "A", 1, 2 };
        yield return new object[] { controlWithItems, "A", 2, 0 };
        yield return new object[] { controlWithItems, "A", 5, 0 };

        yield return new object[] { controlWithItems, "abcd", -1, -1 };
        yield return new object[] { controlWithItems, "abcd", 0, -1 };
        yield return new object[] { controlWithItems, "abcd", 1, -1 };
        yield return new object[] { controlWithItems, "abcd", 2, -1 };
        yield return new object[] { controlWithItems, "abcd", 5, -1 };

        yield return new object[] { controlWithItems, "def", -1, 3 };
        yield return new object[] { controlWithItems, "def", 0, 3 };
        yield return new object[] { controlWithItems, "def", 1, 3 };
        yield return new object[] { controlWithItems, "def", 2, 3 };
        yield return new object[] { controlWithItems, "def", 5, 3 };

        yield return new object[] { controlWithItems, null, -1, -1 };
        yield return new object[] { controlWithItems, null, 0, -1 };
        yield return new object[] { controlWithItems, null, 1, -1 };
        yield return new object[] { controlWithItems, null, 2, -1 };
        yield return new object[] { controlWithItems, null, 5, -1 };

        yield return new object[] { controlWithItems, string.Empty, -1, 0 };
        yield return new object[] { controlWithItems, string.Empty, 0, 1 };
        yield return new object[] { controlWithItems, string.Empty, 1, 2 };
        yield return new object[] { controlWithItems, string.Empty, 2, 3 };
        yield return new object[] { controlWithItems, string.Empty, 5, 0 };

        yield return new object[] { controlWithItems, "NoSuchItem", -1, -1 };
        yield return new object[] { controlWithItems, "NoSuchItem", 0, -1 };
        yield return new object[] { controlWithItems, "NoSuchItem", 1, -1 };
        yield return new object[] { controlWithItems, "NoSuchItem", 2, -1 };
        yield return new object[] { controlWithItems, "NoSuchItem", 5, -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(FindString_TestData))]
    public void ComboBox_FindString_Invoke_ReturnsExpected(ComboBox control, string s, int startIndex, int expected)
    {
        if (startIndex == -1)
        {
            Assert.Equal(expected, control.FindString(s));
        }

        Assert.Equal(expected, control.FindString(s, startIndex));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(1)]
    [InlineData(2)]
    public void ComboBox_FindString_InvalidStartIndex_ThrowsArgumentOutOfRangeException(int startIndex)
    {
        using ComboBox control = new();
        control.Items.Add("item");
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindString("s", startIndex));
    }

    public static IEnumerable<object[]> FindStringExact_TestData()
    {
        foreach (int startIndex in new int[] { -2, -1, 0, 1 })
        {
            foreach (bool ignoreCase in new bool[] { true, false })
            {
                yield return new object[] { new ComboBox(), null, startIndex, ignoreCase, -1 };
                yield return new object[] { new ComboBox(), string.Empty, startIndex, ignoreCase, -1 };
                yield return new object[] { new ComboBox(), "s", startIndex, ignoreCase, -1 };

                using ComboBox controlWithNoItems = new();
                Assert.Empty(controlWithNoItems.Items);
                yield return new object[] { new ComboBox(), null, startIndex, ignoreCase, -1 };
                yield return new object[] { new ComboBox(), string.Empty, startIndex, ignoreCase, -1 };
                yield return new object[] { new ComboBox(), "s", startIndex, ignoreCase, -1 };
            }
        }

        using ComboBox controlWithItems = new()
        {
            DisplayMember = "Value"
        };
        controlWithItems.Items.Add(new DataClass { Value = "abc" });
        controlWithItems.Items.Add(new DataClass { Value = "abc" });
        controlWithItems.Items.Add(new DataClass { Value = "ABC" });
        controlWithItems.Items.Add(new DataClass { Value = "def" });
        controlWithItems.Items.Add(new DataClass { Value = "" });
        controlWithItems.Items.Add(new DataClass { Value = null });

        foreach (bool ignoreCase in new bool[] { true, false })
        {
            yield return new object[] { controlWithItems, "abc", -1, ignoreCase, 0 };
            yield return new object[] { controlWithItems, "abc", 0, ignoreCase, 1 };
            yield return new object[] { controlWithItems, "abc", 1, ignoreCase, ignoreCase ? 2 : 0 };
            yield return new object[] { controlWithItems, "abc", 2, ignoreCase, 0 };
            yield return new object[] { controlWithItems, "abc", 5, ignoreCase, 0 };
        }

        yield return new object[] { controlWithItems, "ABC", -1, false, 2 };
        yield return new object[] { controlWithItems, "ABC", 0, false, 2 };
        yield return new object[] { controlWithItems, "ABC", 1, false, 2 };
        yield return new object[] { controlWithItems, "ABC", 2, false, 2 };
        yield return new object[] { controlWithItems, "ABC", 5, false, 2 };

        yield return new object[] { controlWithItems, "ABC", -1, true, 0 };
        yield return new object[] { controlWithItems, "ABC", 0, true, 1 };
        yield return new object[] { controlWithItems, "ABC", 1, true, 2 };
        yield return new object[] { controlWithItems, "ABC", 2, true, 0 };
        yield return new object[] { controlWithItems, "ABC", 5, true, 0 };

        foreach (bool ignoreCase in new bool[] { true, false })
        {
            yield return new object[] { controlWithItems, "a", -1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "a", 0, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "a", 1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "a", 2, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "a", 5, ignoreCase, -1 };

            yield return new object[] { controlWithItems, "A", -1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "A", 0, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "A", 1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "A", 2, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "A", 5, ignoreCase, -1 };

            yield return new object[] { controlWithItems, "abcd", -1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "abcd", 0, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "abcd", 1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "abcd", 2, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "abcd", 5, ignoreCase, -1 };

            yield return new object[] { controlWithItems, "def", -1, ignoreCase, 3 };
            yield return new object[] { controlWithItems, "def", 0, ignoreCase, 3 };
            yield return new object[] { controlWithItems, "def", 1, ignoreCase, 3 };
            yield return new object[] { controlWithItems, "def", 2, ignoreCase, 3 };
            yield return new object[] { controlWithItems, "def", 5, ignoreCase, 3 };

            yield return new object[] { controlWithItems, null, -1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, null, 0, ignoreCase, -1 };
            yield return new object[] { controlWithItems, null, 1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, null, 2, ignoreCase, -1 };
            yield return new object[] { controlWithItems, null, 5, ignoreCase, -1 };

            yield return new object[] { controlWithItems, string.Empty, -1, ignoreCase, 4 };
            yield return new object[] { controlWithItems, string.Empty, 0, ignoreCase, 4 };
            yield return new object[] { controlWithItems, string.Empty, 1, ignoreCase, 4 };
            yield return new object[] { controlWithItems, string.Empty, 2, ignoreCase, 4 };
            yield return new object[] { controlWithItems, string.Empty, 5, ignoreCase, 4 };

            yield return new object[] { controlWithItems, "NoSuchItem", -1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 0, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 1, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 2, ignoreCase, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 5, ignoreCase, -1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(FindStringExact_TestData))]
    public void ComboBox_FindStringExact_Invoke_ReturnsExpected(ComboBox control, string s, int startIndex, bool ignoreCase, int expected)
    {
        if (ignoreCase)
        {
            if (startIndex == -1)
            {
                Assert.Equal(expected, control.FindStringExact(s));
            }

            Assert.Equal(expected, control.FindStringExact(s, startIndex));
        }

        Assert.Equal(expected, control.FindStringExact(s, startIndex, ignoreCase));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(1)]
    [InlineData(2)]
    public void ComboBox_FindStringExact_InvalidStartIndex_ThrowsArgumentOutOfRangeException(int startIndex)
    {
        using ComboBox control = new();
        control.Items.Add("item");
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindStringExact("s", startIndex));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindStringExact("s", startIndex, ignoreCase: true));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindStringExact("s", startIndex, ignoreCase: false));
    }

    private void SendCtrlBackspace(SubComboBox tb)
    {
        Message message = default;
        tb.ProcessCmdKey(ref message, Keys.Control | Keys.Back);
    }

    [WinFormsFact]
    public void CtrlBackspaceTextRemainsEmpty()
    {
        using SubComboBox control = new();
        control.ConfigureForCtrlBackspace();
        SendCtrlBackspace(control);
        Assert.Equal("", control.Text);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetCtrlBackspaceData))]
    public void CtrlBackspaceTextChanged(string value, string expected, int cursorRelativeToEnd)
    {
        using SubComboBox control = new(value);
        control.ConfigureForCtrlBackspace(cursorRelativeToEnd);
        SendCtrlBackspace(control);
        Assert.Equal(expected, control.Text);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetCtrlBackspaceRepeatedData))]
    public void CtrlBackspaceRepeatedTextChanged(string value, string expected, int repeats)
    {
        using SubComboBox control = new(value);
        control.ConfigureForCtrlBackspace();
        for (int i = 0; i < repeats; i++)
        {
            SendCtrlBackspace(control);
        }

        Assert.Equal(expected, control.Text);
    }

    [WinFormsFact]
    public void CtrlBackspaceDeletesSelection()
    {
        using SubComboBox control = new("123-5-7-9");
        control.ConfigureForCtrlBackspace();
        control.SelectionStart = 2;
        control.SelectionLength = 5;
        SendCtrlBackspace(control);
        Assert.Equal("12-9", control.Text);
    }

    public static IEnumerable<object[]> WndProc_PaintWithoutWParam_TestData()
    {
        foreach (bool allPaintingInWmPaint in new bool[] { true, false })
        {
            yield return new object[] { FlatStyle.Flat, false, true, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Popup, false, true, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Standard, false, true, allPaintingInWmPaint, false, 0 };
            yield return new object[] { FlatStyle.System, false, true, allPaintingInWmPaint, false, 0 };
            yield return new object[] { FlatStyle.Flat, false, false, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Popup, false, false, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Standard, false, false, allPaintingInWmPaint, false, 0 };
            yield return new object[] { FlatStyle.System, false, false, allPaintingInWmPaint, false, 0 };

            yield return new object[] { FlatStyle.Flat, true, true, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Popup, true, true, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Standard, true, true, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.System, true, true, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Flat, true, false, allPaintingInWmPaint, true, 1 };
            yield return new object[] { FlatStyle.Popup, true, false, allPaintingInWmPaint, true, 1 };
            yield return new object[] { FlatStyle.Standard, true, false, allPaintingInWmPaint, true, 1 };
            yield return new object[] { FlatStyle.System, true, false, allPaintingInWmPaint, true, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PaintWithoutWParam_TestData))]
    public void ComboBox_WndProc_InvokePaintWithoutWParam_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, bool expectedIsHandleCreated, int expectedPaintCallCount)
    {
        using (new NoAssertContext())
        {
            using SubComboBox control = new()
            {
                FlatStyle = flatStyle
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_PAINT
            };
            control.WndProc(ref m);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PaintWithoutWParam_TestData))]
    public void ComboBox_WndProc_InvokePaintWithoutWParamWithBounds_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, bool expectedIsHandleCreated, int expectedPaintCallCount)
    {
        using (new NoAssertContext())
        {
            using SubComboBox control = new()
            {
                FlatStyle = flatStyle,
                Bounds = new Rectangle(1, 2, 30, 40)
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_PAINT
            };
            control.WndProc(ref m);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
        }
    }

    public static IEnumerable<object[]> WndProc_PaintWithoutWParamWithHandle_TestData()
    {
        foreach (bool allPaintingInWmPaint in new bool[] { true, false })
        {
            yield return new object[] { FlatStyle.Flat, false, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Popup, false, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Standard, false, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.System, false, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Flat, false, false, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Popup, false, false, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Standard, false, false, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.System, false, false, allPaintingInWmPaint, 0 };

            yield return new object[] { FlatStyle.Flat, true, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Popup, true, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Standard, true, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.System, true, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Flat, true, false, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.Popup, true, false, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.Standard, true, false, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.System, true, false, allPaintingInWmPaint, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PaintWithoutWParamWithHandle_TestData))]
    public void ComboBox_WndProc_InvokePaintWithoutWParamWithHandle_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, int expectedPaintCallCount)
    {
        using SubComboBox control = new()
        {
            FlatStyle = flatStyle
        };
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
        control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int paintCallCount = 0;
        control.Paint += (sender, e) => paintCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_PAINT
        };
        control.WndProc(ref m);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(expectedPaintCallCount, paintCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PaintWithoutWParamWithHandle_TestData))]
    public void ComboBox_WndProc_InvokePaintWithoutWParamWithBoundsWithHandle_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, int expectedPaintCallCount)
    {
        using SubComboBox control = new()
        {
            FlatStyle = flatStyle,
            Bounds = new Rectangle(1, 2, 30, 40)
        };
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
        control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int paintCallCount = 0;
        control.Paint += (sender, e) => paintCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_PAINT
        };
        control.WndProc(ref m);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(expectedPaintCallCount, paintCallCount);
    }

    public static IEnumerable<object[]> WndProc_PaintWithWParam_TestData()
    {
        foreach (bool allPaintingInWmPaint in new bool[] { true, false })
        {
            yield return new object[] { FlatStyle.Flat, false, true, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Popup, false, true, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Standard, false, true, allPaintingInWmPaint, false, 0 };
            yield return new object[] { FlatStyle.System, false, true, allPaintingInWmPaint, false, 0 };
            yield return new object[] { FlatStyle.Flat, false, false, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Popup, false, false, allPaintingInWmPaint, true, 0 };
            yield return new object[] { FlatStyle.Standard, false, false, allPaintingInWmPaint, false, 0 };
            yield return new object[] { FlatStyle.System, false, false, allPaintingInWmPaint, false, 0 };

            yield return new object[] { FlatStyle.Flat, true, true, allPaintingInWmPaint, false, 1 };
            yield return new object[] { FlatStyle.Popup, true, true, allPaintingInWmPaint, false, 1 };
            yield return new object[] { FlatStyle.Standard, true, true, allPaintingInWmPaint, false, 1 };
            yield return new object[] { FlatStyle.System, true, true, allPaintingInWmPaint, false, 1 };
            yield return new object[] { FlatStyle.Flat, true, false, allPaintingInWmPaint, false, 1 };
            yield return new object[] { FlatStyle.Popup, true, false, allPaintingInWmPaint, false, 1 };
            yield return new object[] { FlatStyle.Standard, true, false, allPaintingInWmPaint, false, 1 };
            yield return new object[] { FlatStyle.System, true, false, allPaintingInWmPaint, false, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PaintWithWParam_TestData))]
    public void ComboBox_WndProc_InvokePaintWithWParam_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, bool expectedIsHandleCreated, int expectedPaintCallCount)
    {
        using (new NoAssertContext())
        {
            using Bitmap image = new(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            IntPtr hdc = graphics.GetHdc();
            try
            {
                using SubComboBox control = new()
                {
                    FlatStyle = flatStyle
                };
                control.SetStyle(ControlStyles.UserPaint, userPaint);
                control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                int paintCallCount = 0;
                control.Paint += (sender, e) => paintCallCount++;

                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_PAINT,
                    WParam = hdc
                };
                control.WndProc(ref m);
                Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
                Assert.Equal(expectedPaintCallCount, paintCallCount);
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PaintWithWParam_TestData))]
    public void ComboBox_WndProc_InvokePaintWithWParamWithBounds_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, bool expectedIsHandleCreated, int expectedPaintCallCount)
    {
        using (new NoAssertContext())
        {
            using Bitmap image = new(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            IntPtr hdc = graphics.GetHdc();
            try
            {
                using SubComboBox control = new()
                {
                    FlatStyle = flatStyle,
                    Bounds = new Rectangle(1, 2, 30, 40)
                };
                control.SetStyle(ControlStyles.UserPaint, userPaint);
                control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                int paintCallCount = 0;
                control.Paint += (sender, e) => paintCallCount++;

                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_PAINT,
                    WParam = hdc
                };
                control.WndProc(ref m);
                Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
                Assert.Equal(expectedPaintCallCount, paintCallCount);
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }
    }

    public static IEnumerable<object[]> WndProc_PaintWithWParamWithHandle_TestData()
    {
        foreach (bool allPaintingInWmPaint in new bool[] { true, false })
        {
            yield return new object[] { FlatStyle.Flat, false, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Popup, false, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Standard, false, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.System, false, true, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Flat, false, false, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Popup, false, false, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.Standard, false, false, allPaintingInWmPaint, 0 };
            yield return new object[] { FlatStyle.System, false, false, allPaintingInWmPaint, 0 };

            yield return new object[] { FlatStyle.Flat, true, true, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.Popup, true, true, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.Standard, true, true, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.System, true, true, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.Flat, true, false, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.Popup, true, false, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.Standard, true, false, allPaintingInWmPaint, 1 };
            yield return new object[] { FlatStyle.System, true, false, allPaintingInWmPaint, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PaintWithWParamWithHandle_TestData))]
    public void ComboBox_WndProc_InvokePaintWithWParamWithHandle_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, int expectedPaintCallCount)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            using SubComboBox control = new()
            {
                FlatStyle = flatStyle
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_PAINT,
                WParam = hdc
            };
            control.WndProc(ref m);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PaintWithWParamWithHandle_TestData))]
    public void ComboBox_WndProc_InvokePaintWithWParamWithBoundsWithHandle_Success(FlatStyle flatStyle, bool userPaint, bool doubleBuffered, bool allPaintingInWmPaint, int expectedPaintCallCount)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            using SubComboBox control = new()
            {
                FlatStyle = flatStyle,
                Bounds = new Rectangle(1, 2, 30, 40)
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_PAINT,
                WParam = hdc
            };
            control.WndProc(ref m);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.DropDownList)]
    [InlineData(ComboBoxStyle.Simple)]
    public void Combobox_SetCustomSize_DoesNotCreateHandle(ComboBoxStyle dropDownStyle)
    {
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(100, 50) };
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_CustomAccessibleObject_DoesNotCrashControl_WhenAddingItems()
    {
        using AutomationEventCountingComboBox control = new();
        control.Items.Add("item1");
        control.Items.Add("item2");

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_OnKeyUp_DoesNotInvoke_RaiseAutomationEvent_AccessibilityObjectIsNotCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();
        comboBox.CreateControl();
        AutomationEventCountingComboBoxChildEditUiaProvider comboBoxChildEditUiaProvider = new(comboBox, comboBox.TestAccessor.Dynamic._childEdit.HWND);
        comboBox.TestAccessor.Dynamic._childEditAccessibleObject = comboBoxChildEditUiaProvider;

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");

        comboBox.OnKeyUp();

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);

        // The "RaiseAutomationCallCount" method is called from "OnTextChanged" method
        Assert.Equal(0, comboBoxChildEditUiaProvider._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_OnKeyUp_Invoke_RaiseAutomationEvent_AccessibilityObjectIsCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();
        comboBox.CreateControl();
        AutomationEventCountingComboBoxChildEditUiaProvider comboBoxChildEditUiaProvider = new(comboBox, comboBox.TestAccessor.Dynamic._childEdit.HWND);
        comboBox.TestAccessor.Dynamic._childEditAccessibleObject = comboBoxChildEditUiaProvider;

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");

        Assert.IsType<AutomationEventCountingComboBoxAccessibleObject>(comboBox.AccessibilityObject);

        comboBox.OnKeyUp();

        Assert.True(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);

        // The "RaiseAutomationCallCount" method is called from "OnTextChanged" method
        Assert.Equal(1, comboBoxChildEditUiaProvider._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_OnMouseDown_DoesNotInvoke_RaiseAutomationEvent_AccessibilityObjectIsNotCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();
        comboBox.CreateControl();
        AutomationEventCountingComboBoxChildEditUiaProvider comboBoxChildEditUiaProvider = new(comboBox, comboBox.TestAccessor.Dynamic._childEdit.HWND);
        comboBox.TestAccessor.Dynamic._childEditAccessibleObject = comboBoxChildEditUiaProvider;

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");

        comboBox.OnMouseDown(comboBox.PointToClient(comboBoxChildEditUiaProvider.Bounds.Location));

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);

        // The "RaiseAutomationCallCount" method is called from "OnTextChanged" method
        Assert.Equal(0, comboBoxChildEditUiaProvider._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_OnMouseDown_Invoke_RaiseAutomationEvent_AccessibilityObjectIsCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();
        comboBox.CreateControl();
        AutomationEventCountingComboBoxChildEditUiaProvider comboBoxChildEditUiaProvider = new(comboBox, comboBox.TestAccessor.Dynamic._childEdit.HWND);
        comboBox.TestAccessor.Dynamic._childEditAccessibleObject = comboBoxChildEditUiaProvider;

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");

        Assert.IsType<AutomationEventCountingComboBoxAccessibleObject>(comboBox.AccessibilityObject);

        comboBox.OnMouseDown(comboBox.PointToClient(comboBoxChildEditUiaProvider.Bounds.Location));

        Assert.True(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);

        // The "RaiseAutomationCallCount" method is called from "OnTextChanged" method
        Assert.Equal(1, comboBoxChildEditUiaProvider._raiseAutomationCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Combox_SelectedItem_HandlesItemRemoval(int selectedIndex)
    {
        using ComboBox comboBox = new();
        for (int i = 0; i < 3; i++)
        {
            comboBox.Items.Add(i.ToString());
        }

        comboBox.SelectedItem = comboBox.Items[selectedIndex];
        Assert.Equal(selectedIndex, comboBox.SelectedIndex);
        Assert.Equal(selectedIndex.ToString(), comboBox.SelectedItem);
        Assert.Equal(selectedIndex.ToString(), comboBox.Text);

        comboBox.Items.RemoveAt(selectedIndex);
        Assert.Equal(-1, comboBox.SelectedIndex);
        Assert.Null(comboBox.SelectedItem);
        Assert.Empty(comboBox.Text);
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_SelectedItem_Set_DoesNotInvoke_RaiseAutomationEvent_AccessibilityObjectIsCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();
        comboBox.CreateControl();
        AutomationEventCountingComboBoxChildEditUiaProvider comboBoxChildEditUiaProvider = new(comboBox, comboBox.TestAccessor.Dynamic._childEdit.HWND);
        comboBox.TestAccessor.Dynamic._childEditAccessibleObject = comboBoxChildEditUiaProvider;

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.SelectedItem = comboBox.Items[0];

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);
        Assert.Equal(0, comboBoxChildEditUiaProvider._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_SelectedItem_Set_Invoke_RaiseAutomationEvent_AccessibilityObjectIsCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();
        comboBox.CreateControl();
        AutomationEventCountingComboBoxChildEditUiaProvider comboBoxChildEditUiaProvider = new(comboBox, comboBox.TestAccessor.Dynamic._childEdit.HWND);
        comboBox.TestAccessor.Dynamic._childEditAccessibleObject = comboBoxChildEditUiaProvider;

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");

        Assert.IsType<AutomationEventCountingComboBoxAccessibleObject>(comboBox.AccessibilityObject);

        comboBox.SelectedItem = comboBox.Items[0];

        Assert.True(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);

        // The "RaiseAutomationEvent" method is called from "OnTextChanged" and "OnSelectedIndexChanged" methods
        Assert.Equal(2, comboBoxChildEditUiaProvider._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_Text_Set_DoesNotInvoke_RaiseAutomationEvent_AccessibilityObjectIsCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();
        comboBox.CreateControl();

        AutomationEventCountingComboBoxChildEditUiaProvider comboBoxChildEditUiaProvider = new(comboBox, comboBox.TestAccessor.Dynamic._childEdit.HWND);
        comboBox.TestAccessor.Dynamic._childEditAccessibleObject = comboBoxChildEditUiaProvider;

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.Text = "Test";

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);
        Assert.Equal(0, comboBoxChildEditUiaProvider._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_Text_Set_Invoke_RaiseAutomationEvent_AccessibilityObjectIsCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();
        comboBox.CreateControl();
        AutomationEventCountingComboBoxChildEditUiaProvider comboBoxChildEditUiaProvider = new(comboBox, comboBox.TestAccessor.Dynamic._childEdit.HWND);
        comboBox.TestAccessor.Dynamic._childEditAccessibleObject = comboBoxChildEditUiaProvider;

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");

        Assert.IsType<AutomationEventCountingComboBoxAccessibleObject>(comboBox.AccessibilityObject);

        comboBox.Text = "Test";

        Assert.True(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);

        // The "RaiseAutomationEvent" method is called from "OnTextChanged" method
        Assert.Equal(1, comboBoxChildEditUiaProvider._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_DroppedDown_Set_True_DoesNotInvoke_RaiseAutomationPropertyChangedEvent_AccessibilityObjectIsNotCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.DroppedDown = true;

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);
        Assert.Equal(0, ((AutomationEventCountingComboBoxAccessibleObject)comboBox.AccessibilityObject)._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_DroppedDown_Set_False_DoesNotInvoke_RaiseAutomationPropertyChangedEvent_AccessibilityObjectIsNotCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.DroppedDown = true;
        comboBox.DroppedDown = false;

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);
        Assert.Equal(0, ((AutomationEventCountingComboBoxAccessibleObject)comboBox.AccessibilityObject)._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_DroppedDownProperty_Set_True_Invoke_RaiseAutomationPropertyChangedEvent_AccessibilityObjectIsCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();

        Assert.IsType<AutomationEventCountingComboBoxAccessibleObject>(comboBox.AccessibilityObject);

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.DroppedDown = true;

        Assert.True(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);

        // The "RaiseAutomationPropertyChangedEvent" method is called from "OnDropDown" method
        Assert.Equal(1, ((AutomationEventCountingComboBoxAccessibleObject)comboBox.AccessibilityObject)._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_DroppedDownProperty_Set_False_Invoke_RaiseAutomationPropertyChangedEvent_AccessibilityObjectIsCreated()
    {
        using AutomationEventCountingComboBox comboBox = new();

        Assert.IsType<AutomationEventCountingComboBoxAccessibleObject>(comboBox.AccessibilityObject);

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.DroppedDown = true;
        comboBox.DroppedDown = false;

        Assert.True(comboBox.IsAccessibilityObjectCreated);
        Assert.True(comboBox.IsHandleCreated);

        // The "RaiseAutomationPropertyChangedEvent" method is called from "OnDropDown" and "OnDropDownClosed" method
        Assert.Equal(2, ((AutomationEventCountingComboBoxAccessibleObject)comboBox.AccessibilityObject)._raiseAutomationCallCount);
    }

    [WinFormsFact]
    public void ComboBox_Add_Items_By_Index()
    {
        using ComboBox comboBox = new();
        Random random = new(DateTime.Now.Millisecond);
        InitializeItems(comboBox, 10);

        // Add 5 items by index
        for (int i = 0; i < 5; i++)
        {
            int count = comboBox.Items.Count;
            int index = random.Next(0, count - 1);
            string item = $"new item{i}";
            comboBox.Items.Insert(index, item);

            Assert.True(item.Equals(comboBox.Items[index]));
            Assert.Equal(comboBox.Items.Count, ++count);
        }
    }

    [WinFormsTheory]
    [InlineData(10, 0, 5)]
    [InlineData(10, 2, 4)]
    public void ComboBox_Remove_Items_By_Index(int numberOfItems, int index, int numberOfItemsToRemove)
    {
        using ComboBox comboBox = new();
        InitializeItems(comboBox, numberOfItems);

        int count = comboBox.Items.Count;
        for (int i = 0; i < numberOfItemsToRemove; i++)
        {
            comboBox.Items.RemoveAt(index);
        }

        Assert.Equal(count - numberOfItemsToRemove, comboBox.Items.Count);
    }

    [WinFormsTheory]
    [InlineData(10, 0, 5)]
    [InlineData(10, 2, 4)]
    public void ComboBox_Remove_Items_By_Object(int numberOfItems, int index, int number)
    {
        using ComboBox comboBox = new();
        InitializeItems(comboBox, numberOfItems);

        int count = comboBox.Items.Count;
        for (int i = 0; i < number; i++)
        {
            comboBox.Items.Remove(comboBox.Items[index]);
        }

        Assert.Equal(count - number, comboBox.Items.Count);
    }

    [WinFormsFact]
    public void ComboBox_Sorted_Sorts_By_Ascending()
    {
        using ComboBox comboBox = new();
        const int numItems = 200;
        Random random = new();

        string[] items = new string[numItems];

        // All the items are between a - z
        for (int j = 0; j < numItems; j++)
        {
            string item = (random.Next() % 26 + 'a').ToString();
            items[j] = item;
            comboBox.Items.Add(item);
        }

        comboBox.Sorted = true;
        Array.Sort(items);

        for (int i = 0; i < numItems; i++)
        {
            Assert.Equal(items[i], comboBox.Items[i]);
        }
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(5)]
    public void ComboBox_SelectedIndex_DoesNotChange_SelectedItem_AndDoesNotFire_OnSelectedIndexChanged(int index)
    {
        using ComboBoxWithSelectCounter comboBox = new();
        InitializeItems(comboBox, 10);

        comboBox.SelectedIndex = index;
        comboBox.Sorted = false;
        int selectedIndex = comboBox.SelectedIndex;
        comboBox.SelectedIndex = index;

        Assert.Equal(index, comboBox.SelectedIndex);
        Assert.True(comboBox.Items[comboBox.SelectedIndex].Equals(comboBox.SelectedItem));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    public void ComboBox_SelectedIndexChangesSelectedItemAndFiresOnSelectedIndexChanged(int index)
    {
        using ComboBoxWithSelectCounter comboBox = new();
        InitializeItems(comboBox, 10);

        comboBox.Sorted = false;
        int eventsCountExpected = 0;
        comboBox.ResetEventsCount();
        int selectedIndex = comboBox.SelectedIndex;
        comboBox.SelectedIndex = index;
        eventsCountExpected++;

        Assert.Equal(index, comboBox.SelectedIndex);
        Assert.True(comboBox.Items[comboBox.SelectedIndex].Equals(comboBox.SelectedItem));
        Assert.Equal(eventsCountExpected, comboBox.EventsCount);
    }

    [WinFormsTheory]
    [InlineData("item0", 0, 0, false)]
    [InlineData("item1", 0, 1, false)]
    [InlineData("item1", 1, 1, false)]
    [InlineData("item1", 2, 1, false)]
    [InlineData("item7", 2, 7, false)]
    [InlineData("item7", 9, 7, false)]
    [InlineData("item9", 0, 9, false)]
    [InlineData("item9", 9, 9, false)]
    [InlineData("Item9", 9, -1, false)]
    [InlineData("item1984", 9, -1, false)]
    [InlineData("item0", 0, 0, true)]
    [InlineData("ITEM1", 0, 1, true)]
    [InlineData("itEm1", 1, 1, true)]
    [InlineData("item1", 2, 1, true)]
    [InlineData("item7", 2, 7, true)]
    [InlineData("item7", 9, 7, true)]
    [InlineData("item9", 0, 9, true)]
    [InlineData("item9", 9, 9, true)]
    [InlineData("Item9", 9, 9, true)]
    [InlineData("item1984", 9, -1, true)]
    public void ComboBox_FindString(string value, int index, int expected, bool isExact)
    {
        using ComboBox comboBox = new();
        InitializeItems(comboBox, 10);

        int actual = comboBox.FindStringExact(value, index, isExact);

        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void OwnerDrawComboBox_Verify_OnMeasureItem_Receives_Correct_Arguments()
    {
        using OwnerDrawComboBox ownerDrawComboBox = new();
        ownerDrawComboBox.CreateControl();
        ownerDrawComboBox.Items.AddRange(
        [
            "One",
            "Two",
            "Three"
        ]);
        ownerDrawComboBox.Location = new Point(0, 50);
        Assert.Equal(3, ownerDrawComboBox.MeasureItemEventArgs.Count);

        for (int i = 0; i < 3; i++)
        {
            MeasureItemEventArgs e = ownerDrawComboBox.MeasureItemEventArgs[i];
            Assert.NotNull(e.Graphics);
            Assert.Equal(e.Index, i);
            Assert.Equal(18, e.ItemHeight);
            Assert.Equal(0, e.ItemWidth);
        }
    }

    [WinFormsTheory]
    [InlineData(Keys.Up, 9, 9)]
    [InlineData(Keys.Down, 9, 0)]
    public void ComboBox_Select_Item_By_Key(Keys key, int expectedKeyPressesCount, int selectedIndex)
    {
        using ComboBoxWithSelectCounter comboBox = new();
        comboBox.AddItems(expectedKeyPressesCount + 1);
        comboBox.CreateControl();
        comboBox.SelectedIndex = selectedIndex;
        comboBox.ResetEventsCount();

        for (int i = 0; i < expectedKeyPressesCount; i++)
        {
            KeyboardSimulator.KeyPress(comboBox, key);
        }

        Assert.Equal(expectedKeyPressesCount, comboBox.EventsCount);
    }

    [WinFormsTheory]
    [InlineData(DrawMode.Normal)]
    [InlineData(DrawMode.OwnerDrawFixed)]
    [InlineData(DrawMode.OwnerDrawVariable)]
    public void ComboBox_GetItemHeight_Invoke_ReturnsExpected(DrawMode drawMode)
    {
        int index = 0;
        int expected = 15;
        using ComboBox control = CreateComboBox(drawMode, expected);
        control.GetItemHeight(index).Should().Be(expected);
    }

    private ComboBox CreateComboBox(DrawMode drawMode, int itemHeight = 15)
    {
        ComboBox control = new()
        {
            DrawMode = drawMode,
            ItemHeight = itemHeight
        };

        control.Items.Add("Item1");
        control.Items.Add("Item2");
        control.Items.Add("Item3");
        control.CreateControl();

        return control;
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ComboBox_GetItemHeight_Index_ThrowsArgumentOutOfRangeException(int index)
    {
        using ComboBox control = CreateComboBox(DrawMode.OwnerDrawVariable);
        control.CreateControl(); // Ensure the handle is created

        if (index < 0 || index >= control.Items.Count)
        {
            control.Invoking(y => y.GetItemHeight(index))
                   .Should().Throw<ArgumentOutOfRangeException>()
                   .WithMessage("*index*")
                   .Where(ex => ex.ParamName == "index");
        }
        else
        {
            // Test valid index
            int itemHeight = control.GetItemHeight(index);
            itemHeight.Should().BeGreaterThan(0, "Item height should be greater than 0.");
        }
    }

    [WinFormsFact]
    public void ComboBox_GetItemHeight_NoItems_ThrowsArgumentOutOfRangeException()
    {
        using ComboBox control = new();
        control.DrawMode = DrawMode.OwnerDrawVariable;
        control.CreateControl();
        control.Invoking(c => c.GetItemHeight(0))
               .Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsTheory]
    [BoolData]
    public void ComboBox_ResetText_Invoke_Success(bool withHandle)
    {
        using ComboBox control = new();
        if (withHandle)
        {
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        control.Text = "Some text";
        control.ResetText();
        Assert.Equal(string.Empty, control.Text);
    }

    [WinFormsFact]
    public void ComboBox_SelectAll_InvokeWithoutHandle_Success()
    {
        using ComboBox control = new();
        control.Items.AddRange(new string[] { "Item1", "Item2", "Item3" });
        control.Handle.Should().NotBe(IntPtr.Zero);
        control.SelectAll();

        control.SelectionStart.Should().Be(0);
        control.SelectionLength.Should().Be(control.Text.Length);
    }

    [WinFormsFact]
    public void ComboBox_SelectAll_InvokeWithoutItems_Success()
    {
        using ComboBox control = new();
        control.Handle.Should().NotBe(IntPtr.Zero);
        control.SelectAll();

        control.SelectionStart.Should().Be(0);
        control.SelectionLength.Should().Be(control.Text.Length);
    }

    // Unit test for https://github.com/microsoft/winforms-designer/issues/2707
    [WinFormsFact]
    public void ComboBox_CorrectHeightAfterSetDropDownStyleSimple()
    {
        using ComboBox comboBox = new();

        int handleCreatedInvoked = 0;
        comboBox.HandleCreated += (s, e) =>
        {
            handleCreatedInvoked++;
        };

        comboBox.Height.Should().Be(23);

        comboBox.CreateControl();

        comboBox.Height.Should().Be(23);
        comboBox.DropDownStyle.Should().Be(ComboBoxStyle.DropDown);

        comboBox.DropDownStyle = ComboBoxStyle.Simple;

        // DefaultSimpleStyleHeight is 150 in ComboBox class
        comboBox.Height.Should().Be(150);
        comboBox.DropDownStyle.Should().Be(ComboBoxStyle.Simple);
        handleCreatedInvoked.Should().Be(2);
    }

    private void InitializeItems(ComboBox comboBox, int numItems)
    {
        for (int i = 0; i < numItems; i++)
        {
            comboBox.Items.Add($"item{i}");
        }

        Assert.Equal(numItems, comboBox.Items.Count);
    }

    private class OwnerDrawComboBox : ComboBox
    {
        public OwnerDrawComboBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
            FormattingEnabled = true;
        }

        public List<MeasureItemEventArgs> MeasureItemEventArgs { get; } = [];

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            MeasureItemEventArgs.Add(e);
        }
    }

    private class AutomationEventCountingComboBox : ComboBox
    {
        protected override AccessibleObject CreateAccessibilityInstance()
            => new AutomationEventCountingComboBoxAccessibleObject(this);

        public void OnKeyUp() => base.OnKeyUp(new KeyEventArgs(Keys.Left));

        public void OnMouseDown(Point p) => base.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0));
    }

    private class AutomationEventCountingComboBoxAccessibleObject : Control.ControlAccessibleObject
    {
        public AutomationEventCountingComboBoxAccessibleObject(ComboBox owner) : base(owner)
        { }

        internal int _raiseAutomationCallCount;

        internal override bool RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID propertyId, VARIANT oldValue, VARIANT newValue)
        {
            _raiseAutomationCallCount++;
            return base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);
        }
    }

    private class AutomationEventCountingComboBoxChildEditUiaProvider : ComboBoxChildEditUiaProvider
    {
        public AutomationEventCountingComboBoxChildEditUiaProvider(ComboBox owner, HWND childEditControlhandle) : base(owner, childEditControlhandle)
        {
            _raiseAutomationCallCount = 0;
        }

        internal int _raiseAutomationCallCount;

        internal override bool RaiseAutomationEvent(UIA_EVENT_ID eventId)
        {
            _raiseAutomationCallCount++;
            return base.RaiseAutomationEvent(eventId);
        }
    }

    /// <summary>
    ///  Exposes modern ComboBox seams for deterministic renderer tests.
    /// </summary>
    private sealed class VisualStylesComboBox : SubComboBox
    {
        public FlatComboAdapter CreateAdapter()
            => base.CreateFlatComboAdapterInstance();

        public (int left, int right) GetEditMargins()
        {
            COMBOBOXINFO comboBoxInfo = GetComboBoxInfo();
            nint margins = PInvokeCore.SendMessage(
                comboBoxInfo.hwndItem,
                PInvokeCore.EM_GETMARGINS);

            return (PARAM.LOWORD(margins), PARAM.HIWORD(margins));
        }

        public int GetSelectionHeight()
            => (int)PInvokeCore.SendMessage(
                this,
                PInvoke.CB_GETITEMHEIGHT,
                (WPARAM)(-1));

        public Rectangle GetEditBounds()
        {
            COMBOBOXINFO comboBoxInfo = GetComboBoxInfo();
            PInvokeCore.GetWindowRect(
                comboBoxInfo.hwndItem,
                out RECT bounds);
            Point topLeft = PointToClient(
                new Point(bounds.left, bounds.top));

            return new Rectangle(
                topLeft,
                new Size(bounds.Width, bounds.Height));
        }

        public Rectangle GetListBounds()
        {
            COMBOBOXINFO comboBoxInfo = GetComboBoxInfo();
            PInvokeCore.GetWindowRect(
                comboBoxInfo.hwndList,
                out RECT bounds);
            Point topLeft = PointToClient(
                new Point(bounds.left, bounds.top));

            return new Rectangle(
                topLeft,
                new Size(bounds.Width, bounds.Height));
        }

        public Rectangle GetItemBounds()
        {
            COMBOBOXINFO comboBoxInfo = GetComboBoxInfo();

            return new Rectangle(
                comboBoxInfo.rcItem.left,
                comboBoxInfo.rcItem.top,
                comboBoxInfo.rcItem.Width,
                comboBoxInfo.rcItem.Height);
        }

        public Rectangle GetButtonBounds()
        {
            COMBOBOXINFO comboBoxInfo = GetComboBoxInfo();

            return new Rectangle(
                comboBoxInfo.rcButton.left,
                comboBoxInfo.rcButton.top,
                comboBoxInfo.rcButton.Width,
                comboBoxInfo.rcButton.Height);
        }

        private unsafe COMBOBOXINFO GetComboBoxInfo()
        {
            COMBOBOXINFO comboBoxInfo = default;
            comboBoxInfo.cbSize = (uint)sizeof(COMBOBOXINFO);
            Assert.True(PInvoke.GetComboBoxInfo(
                HWND,
                ref comboBoxInfo));

            return comboBoxInfo;
        }

        public Padding ModernFieldPadding
            => (Padding)this.TestAccessor.Dynamic.GetModernFieldPadding();

        public Padding ModernChromeInsets
            => (Padding)this.TestAccessor.Dynamic.GetModernChromeInsets();

        public Rectangle ModernEditBaseBounds
            => (Rectangle)this.TestAccessor.Dynamic
                .GetNativeComboBaselineEditBounds();

        public Rectangle ModernSimpleListBaseBounds
            => (Rectangle)this.TestAccessor.Dynamic
                .GetNativeComboBaselineSimpleListBounds();

        public int ModernComboLayoutWriteCount
            => (int)this.TestAccessor.Dynamic
                .GetModernComboLayoutWriteCount();

        public int NativeSelectionHeight
            => (int)this.TestAccessor.Dynamic
                .GetNativeComboBaselineSelectionFieldItemHeight();

        public void ApplyModernComboLayout()
            => this.TestAccessor.Dynamic.ApplyModernComboLayout();

        public void InitializeNativeComboBaseline()
            => this.TestAccessor.Dynamic.InitializeNativeComboBaseline();

        public void SetTestDeviceDpi(int deviceDpi)
            => DeviceDpiInternal = deviceDpi;

        public void RaiseSystemVisualSettingsChanged(
            SystemVisualSettingsChangedEventArgs e)
            => base.OnSystemVisualSettingsChanged(e);

        internal override bool IsHighContrast => false;
    }

    private static int CountPixels(
        Bitmap bitmap,
        Color color,
        int channelTolerance = 0)
    {
        int count = 0;
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (ColorsAreClose(
                    bitmap.GetPixel(x, y),
                    color,
                    channelTolerance))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool ColorsAreClose(
        Color actual,
        Color expected,
        int channelTolerance)
        => Math.Abs(actual.A - expected.A) <= channelTolerance
            && Math.Abs(actual.R - expected.R) <= channelTolerance
            && Math.Abs(actual.G - expected.G) <= channelTolerance
            && Math.Abs(actual.B - expected.B) <= channelTolerance;

    private class SubComboBox : ComboBox
    {
        public SubComboBox()
        { }

        public SubComboBox(string text)
        {
            Text = text;
        }

        public new bool AllowSelection => base.AllowSelection;

        public new bool CanEnableIme => base.CanEnableIme;

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new CreateParams CreateParams => base.CreateParams;

        public new CurrencyManager DataManager => base.DataManager;

        public new Cursor DefaultCursor => base.DefaultCursor;

        public new ImeMode DefaultImeMode => base.DefaultImeMode;

        public new Padding DefaultMargin => base.DefaultMargin;

        public new Size DefaultMaximumSize => base.DefaultMaximumSize;

        public new Size DefaultMinimumSize => base.DefaultMinimumSize;

        public new Padding DefaultPadding => base.DefaultPadding;

        public new Size DefaultSize => base.DefaultSize;

        public new bool DesignMode => base.DesignMode;

        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        public new EventHandlerList Events => base.Events;

        public new int FontHeight
        {
            get => base.FontHeight;
            set => base.FontHeight = value;
        }

        public new ImeMode ImeModeBase
        {
            get => base.ImeModeBase;
            set => base.ImeModeBase = value;
        }

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

#pragma warning disable 0618
        public new void AddItemsCore(object[] value) => base.AddItemsCore(value);
#pragma warning restore 0618

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new void CreateHandle() => base.CreateHandle();

        public void ConfigureForCtrlBackspace(int cursorRelativeToEnd = 0)
        {
            Focus();
            SelectionStart = Text.Length + cursorRelativeToEnd;
            SelectionLength = 0;
        }

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

        public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);

        public new void OnDropDown(EventArgs e) => base.OnDropDown(e);

        public new void OnDropDownStyleChanged(EventArgs e) => base.OnDropDownStyleChanged(e);

        public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

        public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

        public new void OnMeasureItem(MeasureItemEventArgs e) => base.OnMeasureItem(e);

        public new void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

        public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

        public new void OnParentBackColorChanged(EventArgs e) => base.OnParentBackColorChanged(e);

        public new void OnSelectedIndexChanged(EventArgs e) => base.OnSelectedIndexChanged(e);

        public new void OnSelectedItemChanged(EventArgs e) => base.OnSelectedItemChanged(e);

        public new void OnSelectedValueChanged(EventArgs e) => base.OnSelectedValueChanged(e);

        public new void OnSelectionChangeCommitted(EventArgs e) => base.OnSelectionChangeCommitted(e);

        public new bool ProcessCmdKey(ref Message msg, Keys keyData) => base.ProcessCmdKey(ref msg, keyData);

        public new void ScaleControl(SizeF factor, BoundsSpecified specified) => base.ScaleControl(factor, specified);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }

    private class ComboBoxWithSelectCounter : ComboBox
    {
        public int EventsCount { get; private set; }

        public void AddItems(int numToAdd)
        {
            for (int i = 0; i < numToAdd; i++)
            {
                Items.Add($"Item {i}");
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            EventsCount++;
        }

        public void ResetEventsCount()
        {
            EventsCount = 0;
        }
    }

    private class DataClass
    {
        public string Value { get; set; }
    }
}
