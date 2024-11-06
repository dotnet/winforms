// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class ToolStripTextBoxTests : IDisposable
{
    private readonly ToolStripTextBox _toolStripTextBox;

    public ToolStripTextBoxTests()
    {
        _toolStripTextBox = new();
    }

    public void Dispose()
    {
        _toolStripTextBox.Dispose();
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("name")]
    public void ToolStripTextBox_ConstructorWithName_Success(string name)
    {
        _toolStripTextBox.Name = name;
        _toolStripTextBox.Name.Should().Be(name);
    }

    [WinFormsFact]
    public void ToolStripTextBox_ConstructorWithControl_ThrowsNotSupportedException()
    {
        Control control = new();
        try
        {
            new Action(() => new ToolStripTextBox(control)).Should().Throw<NotSupportedException>();
        }
        finally
        {
            control?.Dispose();
        }
    }

    public static TheoryData<int, int> ImageData => new()
    {
        { 10, 10 },
        { 20, 20 }
    };

    [WinFormsTheory]
    [MemberData(nameof(ImageData))]
    public void ToolStripTextBox_BackgroundImage_GetSet_ReturnsExpected(int width, int height)
    {
        using Image image = new Bitmap(width, height);
        _toolStripTextBox.BackgroundImage = image;
        _toolStripTextBox.BackgroundImage.Should().Be(image);

        _toolStripTextBox.BackgroundImage = null;
        _toolStripTextBox.BackgroundImage.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(ImageLayout.Center)]
    [InlineData(ImageLayout.Stretch)]
    [InlineData(ImageLayout.Zoom)]
    public void ToolStripTextBox_BackgroundImageLayout_GetSet_ReturnsExpected(ImageLayout imageLayout)
    {
        _toolStripTextBox.BackgroundImageLayout = imageLayout;
        _toolStripTextBox.BackgroundImageLayout.Should().Be(imageLayout);
    }

    [WinFormsFact]
    public void ToolStripTextBox_AcceptsTab_GetSet_ReturnsExpected()
    {
        bool eventRaised = false;
        _toolStripTextBox.AcceptsTabChanged += (sender, e) => eventRaised = true;

        _toolStripTextBox.AcceptsTab = true;
        _toolStripTextBox.AcceptsTab.Should().BeTrue();
        eventRaised.Should().BeTrue();

        eventRaised = false;

        _toolStripTextBox.AcceptsTab = true;
        _toolStripTextBox.AcceptsTab.Should().BeTrue();
        eventRaised.Should().BeFalse();

        _toolStripTextBox.AcceptsTab = false;
        _toolStripTextBox.AcceptsTab.Should().BeFalse();
        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_HideSelectionChanged_RaisedSuccessfully()
    {
        bool eventRaised = false;

        _toolStripTextBox.HideSelectionChanged += (sender, args) => eventRaised = true;
        _toolStripTextBox.HideSelection = !_toolStripTextBox.HideSelection;

        eventRaised.Should().BeTrue("because HideSelectionChanged event should be raised");
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripTextBox_Modified_GetSet_ReturnsExpected(bool modified)
    {
        _toolStripTextBox.Modified = modified;
        _toolStripTextBox.Modified.Should().Be(modified);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripTextBox_Multiline_GetSet_ReturnsExpected(bool multiline)
    {
        _toolStripTextBox.Multiline = multiline;
        _toolStripTextBox.Multiline.Should().Be(multiline);
    }

    [WinFormsFact]
    public void ToolStripTextBox_HandleTextBoxTextAlignChanged_RaisesEvent()
    {
        bool eventRaised = false;
        _toolStripTextBox.TextBoxTextAlignChanged += (sender, e) => eventRaised = true;
        _toolStripTextBox.TestAccessor().Dynamic.HandleTextBoxTextAlignChanged(null, EventArgs.Empty);

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_OnAcceptsTabChanged_RaisesEvent()
    {
        bool eventRaised = false;
        _toolStripTextBox.AcceptsTabChanged += (sender, e) => eventRaised = true;
        _toolStripTextBox.TestAccessor().Dynamic.OnAcceptsTabChanged(EventArgs.Empty);

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_OnBorderStyleChanged_RaisesEvent()
    {
        bool eventRaised = false;
        _toolStripTextBox.BorderStyleChanged += (sender, e) => eventRaised = true;
        _toolStripTextBox.TestAccessor().Dynamic.OnBorderStyleChanged(EventArgs.Empty);

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_OnHideSelectionChanged_RaisesEvent()
    {
        bool eventRaised = false;
        _toolStripTextBox.HideSelectionChanged += (sender, e) => eventRaised = true;
        _toolStripTextBox.TestAccessor().Dynamic.OnHideSelectionChanged(EventArgs.Empty);

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_OnModifiedChanged_RaisesEvent()
    {
        bool eventRaised = false;
        _toolStripTextBox.ModifiedChanged += (sender, e) => eventRaised = true;
        _toolStripTextBox.TestAccessor().Dynamic.OnModifiedChanged(EventArgs.Empty);

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_OnMultilineChanged_RaisesEvent()
    {
        bool eventRaised = false;
        _toolStripTextBox.MultilineChanged += (sender, e) => eventRaised = true;
        _toolStripTextBox.TestAccessor().Dynamic.OnMultilineChanged(EventArgs.Empty);

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_ShouldSerializeFont_ReturnsExpected()
    {
        _toolStripTextBox.Font = ToolStripManager.DefaultFont;
        bool result = _toolStripTextBox.TestAccessor().Dynamic.ShouldSerializeFont();
        result.Should().BeFalse();

        _toolStripTextBox.Font = new Font("Arial", 8.25f);
        result = _toolStripTextBox.TestAccessor().Dynamic.ShouldSerializeFont();
        result.Should().BeTrue();
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripTextBox_AcceptsReturn_GetSet_ReturnsExpected(bool acceptsReturn)
    {
        _toolStripTextBox.AcceptsReturn = acceptsReturn;
        _toolStripTextBox.AcceptsReturn.Should().Be(acceptsReturn);
    }

    [WinFormsFact]
    public void ToolStripTextBox_AutoCompleteCustomSource_GetSet_ReturnsExpected()
    {
        AutoCompleteStringCollection collection = [];
        _toolStripTextBox.AutoCompleteCustomSource = collection;
        _toolStripTextBox.AutoCompleteCustomSource.Should().BeEquivalentTo(collection);

        AutoCompleteStringCollection newCollection = [];
        _toolStripTextBox.AutoCompleteCustomSource = newCollection;
        _toolStripTextBox.AutoCompleteCustomSource.Should().BeEquivalentTo(newCollection);
    }

    [WinFormsTheory]
    [InlineData(AutoCompleteSource.AllSystemSources)]
    [InlineData(AutoCompleteSource.None)]
    public void ToolStripTextBox_AutoCompleteSource_GetSet_ReturnsExpected(AutoCompleteSource autoCompleteSource)
    {
        _toolStripTextBox.AutoCompleteSource = autoCompleteSource;
        _toolStripTextBox.AutoCompleteSource.Should().Be(autoCompleteSource);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.FixedSingle)]
    [InlineData(BorderStyle.Fixed3D)]
    public void ToolStripTextBox_BorderStyle_GetSet_ReturnsExpected(BorderStyle borderStyle)
    {
        _toolStripTextBox.BorderStyle = borderStyle;
        _toolStripTextBox.BorderStyle.Should().Be(borderStyle);
    }

    [WinFormsTheory]
    [InlineData(CharacterCasing.Upper)]
    [InlineData(CharacterCasing.Normal)]
    public void ToolStripTextBox_CharacterCasing_GetSet_ReturnsExpected(CharacterCasing characterCasing)
    {
        _toolStripTextBox.CharacterCasing = characterCasing;
        _toolStripTextBox.CharacterCasing.Should().Be(characterCasing);
    }

    [WinFormsTheory]
    [InlineData(new string[] { "Line1", "Line2", "Line3" }, false)]
    [InlineData(new string[] { "Line1", "Line2", "Line3" }, true)]
    [InlineData(new string[] { }, false)]
    [InlineData(null, false)]
    public void ToolStripTextBox_Lines_GetSet_ReturnsExpected(string[] inputLines, bool multiline)
    {
        _toolStripTextBox.Multiline = multiline;
        _toolStripTextBox.Lines = inputLines;
        _toolStripTextBox.Lines.Should().Equal(inputLines ?? Array.Empty<string>());
    }

    [WinFormsTheory]
    [InlineData(5000)]
    [InlineData(32767)]
    public void ToolStripTextBox_MaxLength_GetSet_ReturnsExpected(int maxLength)
    {
        _toolStripTextBox.MaxLength = maxLength;
        _toolStripTextBox.MaxLength.Should().Be(maxLength);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripTextBox_ReadOnly_GetSet_ReturnsExpected(bool readOnly)
    {
        _toolStripTextBox.ReadOnly = readOnly;
        _toolStripTextBox.ReadOnly.Should().Be(readOnly);
    }

    [WinFormsTheory]
    [InlineData("This is some longer text for testing", 0, 4, "This")]
    [InlineData("This is some longer text for testing", 5, 2, "is")]
    [InlineData("This is some longer text for testing", 8, 4, "some")]
    public void ToolStripTextBox_SelectedText_GetSet_ReturnsExpected(string initialText, int selectionStart, int selectionLength, string expectedSelectedText)
    {
        _toolStripTextBox.Text = initialText;
        _toolStripTextBox.SelectionStart = selectionStart;
        _toolStripTextBox.SelectionLength = selectionLength;
        _toolStripTextBox.SelectedText.Should().Be(expectedSelectedText);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(null)]
    public void ToolStripTextBox_SelectionLength_GetSet_ReturnsExpected(int? textLength)
    {
        if (textLength.HasValue)
        {
            _toolStripTextBox.Text = new string('a', textLength.Value);
            _toolStripTextBox.SelectionLength = textLength.Value;
            _toolStripTextBox.SelectionLength.Should().Be(textLength.Value);
        }
        else
        {
            _toolStripTextBox.Invoking(t => t.Text = null).Should().NotThrow();
            _toolStripTextBox.Text.Should().BeNullOrEmpty();
        }
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public void ToolStripTextBox_SelectionStart_GetSet_ReturnsExpected(int value)
    {
        _toolStripTextBox.Text = "0123456789";
        _toolStripTextBox.SelectionStart = value;
        _toolStripTextBox.SelectionStart.Should().Be(value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripTextBox_ShortcutsEnabled_GetSet_ReturnsExpected(bool shortcutsEnabled)
    {
        _toolStripTextBox.ShortcutsEnabled = shortcutsEnabled;
        _toolStripTextBox.ShortcutsEnabled.Should().Be(shortcutsEnabled);
    }

    [WinFormsTheory]
    [InlineData("Test", 4)]
    [InlineData("&Test", 5)]
    [InlineData("T&est", 5)]
    [InlineData("Line1\nLine2\nLine3", 17)]
    public void ToolStripTextBox_TextLength_Get_ReturnsExpected(string text, int expectedLength)
    {
        _toolStripTextBox.Text = text;
        _toolStripTextBox.TextLength.Should().Be(expectedLength);
    }

    [WinFormsTheory]
    [InlineData(HorizontalAlignment.Left)]
    [InlineData(HorizontalAlignment.Center)]
    [InlineData(HorizontalAlignment.Right)]
    public void ToolStripTextBox_TextBoxTextAlign_GetSet_ReturnsExpected(HorizontalAlignment textAlign)
    {
        _toolStripTextBox.TextBoxTextAlign = textAlign;
        _toolStripTextBox.TextBoxTextAlign.Should().Be(textAlign);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripTextBox_WordWrap_GetSet_ReturnsExpected(bool wordWrap)
    {
        _toolStripTextBox.WordWrap = wordWrap;
        _toolStripTextBox.WordWrap.Should().Be(wordWrap);
    }

    [WinFormsFact]
    public void ToolStripTextBox_AcceptsTabChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_toolStripTextBox);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _toolStripTextBox.AcceptsTabChanged += handler;
        _toolStripTextBox.AcceptsTab = !_toolStripTextBox.AcceptsTab;
        callCount.Should().Be(1);

        _toolStripTextBox.AcceptsTabChanged -= handler;
        _toolStripTextBox.AcceptsTab = !_toolStripTextBox.AcceptsTab;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripTextBox_AcceptsTabChanged_AddRemoveMultipleHandlers_Success()
    {
        int callCount1 = 0, callCount2 = 0;
        EventHandler handler1 = (sender, e) => callCount1++;
        EventHandler handler2 = (sender, e) => callCount2++;

        _toolStripTextBox.AcceptsTabChanged += handler1;
        _toolStripTextBox.AcceptsTabChanged += handler2;
        _toolStripTextBox.AcceptsTab = !_toolStripTextBox.AcceptsTab;
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);

        _toolStripTextBox.AcceptsTabChanged -= handler1;
        _toolStripTextBox.AcceptsTab = !_toolStripTextBox.AcceptsTab;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);

        _toolStripTextBox.AcceptsTabChanged -= handler2;
        _toolStripTextBox.AcceptsTab = !_toolStripTextBox.AcceptsTab;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
    }

    [WinFormsFact]
    public void ToolStripTextBox_BorderStyleChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(_toolStripTextBox);
            e.Should().BeSameAs(EventArgs.Empty);
            callCount++;
        };

        _toolStripTextBox.BorderStyleChanged += handler;
        _toolStripTextBox.BorderStyle = BorderStyle.None;
        callCount.Should().Be(1);
        _toolStripTextBox.BorderStyle.Should().Be(BorderStyle.None);

        _toolStripTextBox.BorderStyleChanged -= handler;
        _toolStripTextBox.BorderStyle = BorderStyle.Fixed3D;
        callCount.Should().Be(1);
        _toolStripTextBox.BorderStyle.Should().Be(BorderStyle.Fixed3D);
    }

    [WinFormsFact]
    public void ToolStripTextBox_BorderStyleChanged_AddRemoveMultipleHandlers_Success()
    {
        int callCount1 = 0, callCount2 = 0;
        EventHandler handler1 = (sender, e) => callCount1++;
        EventHandler handler2 = (sender, e) => callCount2++;

        _toolStripTextBox.BorderStyleChanged += handler1;
        _toolStripTextBox.BorderStyleChanged += handler2;
        _toolStripTextBox.BorderStyle = BorderStyle.None;
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
        _toolStripTextBox.BorderStyle.Should().Be(BorderStyle.None);

        _toolStripTextBox.BorderStyleChanged -= handler1;
        _toolStripTextBox.BorderStyle = BorderStyle.Fixed3D;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.BorderStyle.Should().Be(BorderStyle.Fixed3D);

        _toolStripTextBox.BorderStyleChanged -= handler2;
        _toolStripTextBox.BorderStyle = BorderStyle.None;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.BorderStyle.Should().Be(BorderStyle.None);
    }

    [WinFormsFact]
    public void ToolStripTextBox_HideSelectionChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_toolStripTextBox);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _toolStripTextBox.HideSelectionChanged += handler;
        _toolStripTextBox.HideSelection = false;
        callCount.Should().Be(1);
        _toolStripTextBox.HideSelection.Should().BeFalse();

        _toolStripTextBox.HideSelectionChanged -= handler;
        _toolStripTextBox.HideSelection = true;
        callCount.Should().Be(1);
        _toolStripTextBox.HideSelection.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_HideSelectionChanged_AddRemoveMultipleHandlers_Success()
    {
        int callCount1 = 0, callCount2 = 0;
        EventHandler handler1 = (sender, e) => callCount1++;
        EventHandler handler2 = (sender, e) => callCount2++;

        _toolStripTextBox.HideSelectionChanged += handler1;
        _toolStripTextBox.HideSelectionChanged += handler2;
        _toolStripTextBox.HideSelection = false;
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
        _toolStripTextBox.HideSelection.Should().BeFalse();

        _toolStripTextBox.HideSelectionChanged -= handler1;
        _toolStripTextBox.HideSelection = true;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.HideSelection.Should().BeTrue();

        _toolStripTextBox.HideSelectionChanged -= handler2;
        _toolStripTextBox.HideSelection = false;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.HideSelection.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripTextBox_ModifiedChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_toolStripTextBox);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _toolStripTextBox.ModifiedChanged += handler;
        _toolStripTextBox.Modified = true;
        callCount.Should().Be(1);
        _toolStripTextBox.Modified.Should().BeTrue();

        _toolStripTextBox.ModifiedChanged -= handler;
        _toolStripTextBox.Modified = false;
        callCount.Should().Be(1);
        _toolStripTextBox.Modified.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripTextBox_ModifiedChanged_AddRemoveMultipleHandlers_Success()
    {
        int callCount1 = 0, callCount2 = 0;
        EventHandler handler1 = (sender, e) => callCount1++;
        EventHandler handler2 = (sender, e) => callCount2++;

        _toolStripTextBox.ModifiedChanged += handler1;
        _toolStripTextBox.ModifiedChanged += handler2;
        _toolStripTextBox.Modified = true;
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
        _toolStripTextBox.Modified.Should().BeTrue();

        _toolStripTextBox.ModifiedChanged -= handler1;
        _toolStripTextBox.Modified = false;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.Modified.Should().BeFalse();

        _toolStripTextBox.ModifiedChanged -= handler2;
        _toolStripTextBox.Modified = true;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.Modified.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_MultilineChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_toolStripTextBox);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _toolStripTextBox.MultilineChanged += handler;
        _toolStripTextBox.Multiline = true;
        callCount.Should().Be(1);
        _toolStripTextBox.Multiline.Should().BeTrue();

        _toolStripTextBox.MultilineChanged -= handler;
        _toolStripTextBox.Multiline = false;
        callCount.Should().Be(1);
        _toolStripTextBox.Multiline.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripTextBox_MultilineChanged_AddRemoveMultipleHandlers_Success()
    {
        int callCount1 = 0, callCount2 = 0;
        EventHandler handler1 = (sender, e) => callCount1++;
        EventHandler handler2 = (sender, e) => callCount2++;

        _toolStripTextBox.MultilineChanged += handler1;
        _toolStripTextBox.MultilineChanged += handler2;
        _toolStripTextBox.Multiline = true;
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
        _toolStripTextBox.Multiline.Should().BeTrue();

        _toolStripTextBox.MultilineChanged -= handler1;
        _toolStripTextBox.Multiline = false;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.Multiline.Should().BeFalse();

        _toolStripTextBox.MultilineChanged -= handler2;
        _toolStripTextBox.Multiline = true;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.Multiline.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_ReadOnlyChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_toolStripTextBox);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _toolStripTextBox.ReadOnlyChanged += handler;
        _toolStripTextBox.ReadOnly = true;
        callCount.Should().Be(1);
        _toolStripTextBox.ReadOnly.Should().BeTrue();

        _toolStripTextBox.ReadOnlyChanged -= handler;
        _toolStripTextBox.ReadOnly = false;
        callCount.Should().Be(1);
        _toolStripTextBox.ReadOnly.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripTextBox_ReadOnlyChanged_AddRemoveMultipleHandlers_Success()
    {
        int callCount1 = 0, callCount2 = 0;
        EventHandler handler1 = (sender, e) => callCount1++;
        EventHandler handler2 = (sender, e) => callCount2++;

        _toolStripTextBox.ReadOnlyChanged += handler1;
        _toolStripTextBox.ReadOnlyChanged += handler2;
        _toolStripTextBox.ReadOnly = true;
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
        _toolStripTextBox.ReadOnly.Should().BeTrue();

        _toolStripTextBox.ReadOnlyChanged -= handler1;
        _toolStripTextBox.ReadOnly = false;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.ReadOnly.Should().BeFalse();

        _toolStripTextBox.ReadOnlyChanged -= handler2;
        _toolStripTextBox.ReadOnly = true;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.ReadOnly.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripTextBox_TextBoxTextAlignChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_toolStripTextBox);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _toolStripTextBox.TextBoxTextAlignChanged += handler;
        _toolStripTextBox.TextBoxTextAlign = HorizontalAlignment.Center;
        callCount.Should().Be(1);
        _toolStripTextBox.TextBoxTextAlign.Should().Be(HorizontalAlignment.Center);

        _toolStripTextBox.TextBoxTextAlignChanged -= handler;
        _toolStripTextBox.TextBoxTextAlign = HorizontalAlignment.Left;
        callCount.Should().Be(1);
        _toolStripTextBox.TextBoxTextAlign.Should().Be(HorizontalAlignment.Left);
    }

    [WinFormsFact]
    public void ToolStripTextBox_TextBoxTextAlignChanged_AddRemoveMultipleHandlers_Success()
    {
        int callCount1 = 0, callCount2 = 0;
        EventHandler handler1 = (sender, e) => callCount1++;
        EventHandler handler2 = (sender, e) => callCount2++;

        _toolStripTextBox.TextBoxTextAlignChanged += handler1;
        _toolStripTextBox.TextBoxTextAlignChanged += handler2;
        _toolStripTextBox.TextBoxTextAlign = HorizontalAlignment.Center;
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
        _toolStripTextBox.TextBoxTextAlign.Should().Be(HorizontalAlignment.Center);

        _toolStripTextBox.TextBoxTextAlignChanged -= handler1;
        _toolStripTextBox.TextBoxTextAlign = HorizontalAlignment.Left;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.TextBoxTextAlign.Should().Be(HorizontalAlignment.Left);

        _toolStripTextBox.TextBoxTextAlignChanged -= handler2;
        _toolStripTextBox.TextBoxTextAlign = HorizontalAlignment.Center;
        callCount1.Should().Be(1);
        callCount2.Should().Be(2);
        _toolStripTextBox.TextBoxTextAlign.Should().Be(HorizontalAlignment.Center);
    }

    [WinFormsFact]
    public void ToolStripTextBox_AppendText_Success()
    {
        _toolStripTextBox.AppendText("Hello");
        _toolStripTextBox.Text.Should().Be("Hello");
    }

    [WinFormsFact]
    public void ToolStripTextBox_Clear_Success()
    {
        _toolStripTextBox.Text = "Hello";
        _toolStripTextBox.Clear();
        _toolStripTextBox.Text.Should().BeEmpty();
    }

    [WinFormsFact]
    public void ToolStripTextBox_ClearUndo_Success()
    {
        _toolStripTextBox.Text = "Hello";
        _toolStripTextBox.ClearUndo();
        _toolStripTextBox.Undo();
        _toolStripTextBox.Text.Should().Be("Hello");
    }

    [WinFormsFact]
    public void ToolStripTextBox_DeselectAll_Success()
    {
        _toolStripTextBox.Text = "Hello";
        _toolStripTextBox.SelectAll();
        _toolStripTextBox.DeselectAll();
        _toolStripTextBox.SelectionLength.Should().Be(0);
    }

    [WinFormsTheory]
    [InlineData("Hello", 1, 1)]
    [InlineData("This is a test with &ampersand", 10, 1)]
    [InlineData("Line1\nLine2\nLine3", 5, 10)]
    [InlineData("Special characters: !@#$%^&*()", 20, 1)]
    [InlineData("Mixed: Line1\nLine2 & special!", 15, 5)]
    public void ToolStripTextBox_GetCharFromPosition_Success(string text, int x, int y)
    {
        _toolStripTextBox.Text = text;
        char result = _toolStripTextBox.GetCharFromPosition(new Point(x, y));
        result.Should().NotBe('\0');
    }

    [WinFormsFact]
    public void ToolStripTextBox_GetCharIndexFromPosition_Success()
    {
        _toolStripTextBox.Text = "Hello";
        int result = _toolStripTextBox.GetCharIndexFromPosition(new Point(1, 1));
        result.Should().BeGreaterOrEqualTo(0);
    }

    [WinFormsFact]
    public void ToolStripTextBox_GetFirstCharIndexFromLine_ReturnsExpected()
    {
        _toolStripTextBox.Multiline = true;
        _toolStripTextBox.Text = "Hello\r\nWorld\r\nTest";

        _toolStripTextBox.GetFirstCharIndexFromLine(0).Should().Be(0);
        _toolStripTextBox.GetFirstCharIndexFromLine(1).Should().Be(7);
        _toolStripTextBox.GetFirstCharIndexFromLine(2).Should().Be(14);
    }

    [WinFormsFact]
    public void ToolStripTextBox_GetFirstCharIndexOfCurrentLine_Success()
    {
        _toolStripTextBox.Multiline = true;
        _toolStripTextBox.Text = "Hello\r\nWorld\r\nTest";
        _toolStripTextBox.SelectionStart = 7;

        _toolStripTextBox.GetFirstCharIndexOfCurrentLine().Should().Be(7);
    }

    [WinFormsFact]
    public void ToolStripTextBox_GetLineFromCharIndex_ReturnsExpected()
    {
        _toolStripTextBox.Multiline = true;
        _toolStripTextBox.Text = "Hello\r\nWorld\r\nTest";

        _toolStripTextBox.GetLineFromCharIndex(0).Should().Be(0);
        _toolStripTextBox.GetLineFromCharIndex(7).Should().Be(1);
        _toolStripTextBox.GetLineFromCharIndex(14).Should().Be(2);
    }

    [WinFormsFact]
    public void ToolStripTextBox_GetPositionFromCharIndex_ReturnsExpected()
    {
        _toolStripTextBox.Text = "Hello World";

        _toolStripTextBox.GetPositionFromCharIndex(0).Should().Be(new Point(1, 0));
        _toolStripTextBox.GetPositionFromCharIndex(5).Should().Be(new Point(29, 0));
        _toolStripTextBox.GetPositionFromCharIndex(10).Should().Be(new Point(57, 0));
    }

    [WinFormsFact]
    public void ToolStripTextBox_ScrollToCaret_Success()
    {
        _toolStripTextBox.Text = "Hello\nWorld";
        _toolStripTextBox.SelectionStart = _toolStripTextBox.Text.Length;
        _toolStripTextBox.ScrollToCaret();
        Point position = _toolStripTextBox.GetPositionFromCharIndex(_toolStripTextBox.SelectionStart);
        position.Y.Should().BeLessOrEqualTo(_toolStripTextBox.Height);
    }

    [WinFormsFact]
    public void ToolStripTextBox_Select_Success()
    {
        _toolStripTextBox.Text = "Hello World";
        _toolStripTextBox.Select(6, 5);
        _toolStripTextBox.SelectedText.Should().Be("World");
    }

    [WinFormsFact]
    public void ToolStripTextBox_SelectAll_Success()
    {
        _toolStripTextBox.Text = "Hello World";
        _toolStripTextBox.SelectAll();
        _toolStripTextBox.SelectedText.Should().Be("Hello World");
    }
}
