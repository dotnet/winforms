// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms.Tests;

public class EmbeddedResourceTests
{
    // Get System.Windows.Forms assembly to verify that it contains all the resources that the code uses.
    private readonly Assembly _assembly = Assembly.GetAssembly(typeof(AccessibleObject));

    private const string ExpectedIconNamesString = """
            System.Windows.Forms.ActiveDocumentHost
            System.Windows.Forms.alignment
            System.Windows.Forms.alignToGrid
            System.Windows.Forms.Animation
            System.Windows.Forms.Arrow
            System.Windows.Forms.BindingNavigator
            System.Windows.Forms.BindingNavigator.AddNew
            System.Windows.Forms.BindingNavigator.Delete
            System.Windows.Forms.BindingNavigator.MoveFirst
            System.Windows.Forms.BindingNavigator.MoveLast
            System.Windows.Forms.BindingNavigator.MoveNext
            System.Windows.Forms.BindingNavigator.MovePrevious
            System.Windows.Forms.BindingSource
            System.Windows.Forms.blank
            System.Windows.Forms.bringToFront
            System.Windows.Forms.Button
            System.Windows.Forms.CheckBox
            System.Windows.Forms.Checked
            System.Windows.Forms.CheckedListBox
            System.Windows.Forms.ColorDialog
            System.Windows.Forms.ComboBox
            System.Windows.Forms.ComponentModel.OrderImages
            System.Windows.Forms.ContextMenu
            System.Windows.Forms.ContextMenuStrip
            System.Windows.Forms.copy
            System.Windows.Forms.cut
            System.Windows.Forms.DataConnector
            System.Windows.Forms.DataGrid
            System.Windows.Forms.DataGridCaption.backarrow
            System.Windows.Forms.DataGridCaption.backarrow_bidi
            System.Windows.Forms.DataGridCaption.Details
            System.Windows.Forms.DataGridCaption.down
            System.Windows.Forms.DataGridCaption.right
            System.Windows.Forms.DataGridCaption.up
            System.Windows.Forms.DataGridParentRows.LeftArrow
            System.Windows.Forms.DataGridParentRows.RightArrow
            System.Windows.Forms.DataGridRow.error
            System.Windows.Forms.DataGridRow.left
            System.Windows.Forms.DataGridRow.pencil
            System.Windows.Forms.DataGridRow.right
            System.Windows.Forms.DataGridRow.star
            System.Windows.Forms.DataGridView
            System.Windows.Forms.DataGridViewButtonColumn
            System.Windows.Forms.DataGridViewCheckBoxColumn
            System.Windows.Forms.DataGridViewComboBoxColumn
            System.Windows.Forms.DataGridViewImageColumn
            System.Windows.Forms.DataGridViewLinkColumn
            System.Windows.Forms.DataGridViewRow.error
            System.Windows.Forms.DataGridViewRow.left
            System.Windows.Forms.DataGridViewRow.leftstar
            System.Windows.Forms.DataGridViewRow.pencil_ltr
            System.Windows.Forms.DataGridViewRow.pencil_rtl
            System.Windows.Forms.DataGridViewRow.right
            System.Windows.Forms.DataGridViewRow.rightstar
            System.Windows.Forms.DataGridViewRow.star
            System.Windows.Forms.DataGridViewTextBoxColumn
            System.Windows.Forms.DataNavigator
            System.Windows.Forms.DataNavigator.AddNew
            System.Windows.Forms.DataNavigator.Delete
            System.Windows.Forms.DataNavigator.MoveFirst
            System.Windows.Forms.DataNavigator.MoveLast
            System.Windows.Forms.DataNavigator.MoveNext
            System.Windows.Forms.DataNavigator.MovePrevious
            System.Windows.Forms.DateTimePicker
            System.Windows.Forms.DefaultControl
            System.Windows.Forms.delete
            System.Windows.Forms.Design.ComponentEditorPage
            System.Windows.Forms.Design.EventsTab
            System.Windows.Forms.displaystyle
            System.Windows.Forms.DomainUpDown
            System.Windows.Forms.dotdotdot
            System.Windows.Forms.down
            System.Windows.Forms.Edit
            System.Windows.Forms.editdropdownlist
            System.Windows.Forms.Error
            System.Windows.Forms.ErrorControl
            System.Windows.Forms.ErrorProvider
            System.Windows.Forms.FlowLayoutPanel
            System.Windows.Forms.FolderBrowserDialog
            System.Windows.Forms.FontDialog
            System.Windows.Forms.Form
            System.Windows.Forms.Grid
            System.Windows.Forms.GroupBox
            System.Windows.Forms.help
            System.Windows.Forms.HelpProvider
            System.Windows.Forms.HScrollBar
            System.Windows.Forms.HTMLControl
            System.Windows.Forms.IconInError
            System.Windows.Forms.image
            System.Windows.Forms.ImageInError
            System.Windows.Forms.ImageList
            System.Windows.Forms.IndeterminateChecked
            System.Windows.Forms.Label
            System.Windows.Forms.LinkLabel
            System.Windows.Forms.ListBox
            System.Windows.Forms.ListView
            System.Windows.Forms.lockControls
            System.Windows.Forms.MainMenu
            System.Windows.Forms.MaskedTextBox
            System.Windows.Forms.MenuStrip
            System.Windows.Forms.MonthCalendar
            System.Windows.Forms.MultiplexPanel
            System.Windows.Forms.new
            System.Windows.Forms.NotifyIcon
            System.Windows.Forms.NumericUpDown
            System.Windows.Forms.open
            System.Windows.Forms.OpenFileDialog
            System.Windows.Forms.overflowButton
            System.Windows.Forms.PageSetupDialog
            System.Windows.Forms.Panel
            System.Windows.Forms.paste
            System.Windows.Forms.PBAlpha
            System.Windows.Forms.PBCategory
            System.Windows.Forms.PBEvent
            System.Windows.Forms.PBPPage
            System.Windows.Forms.PBProp
            System.Windows.Forms.PictureBox
            System.Windows.Forms.PictureBox.Loading
            System.Windows.Forms.print
            System.Windows.Forms.PrintDialog
            System.Windows.Forms.printPreview
            System.Windows.Forms.PrintPreviewControl
            System.Windows.Forms.PrintPreviewDialog
            System.Windows.Forms.PrintPreviewStrip
            System.Windows.Forms.ProgressBar
            System.Windows.Forms.properties
            System.Windows.Forms.PropertyGrid
            System.Windows.Forms.PropertyGridInternal.PropertiesTab
            System.Windows.Forms.RadioButton
            System.Windows.Forms.Rebar
            System.Windows.Forms.RichEdit
            System.Windows.Forms.RichTextBox
            System.Windows.Forms.save
            System.Windows.Forms.SaveFileDialog
            System.Windows.Forms.ScrollButtonDown
            System.Windows.Forms.ScrollButtonUp
            System.Windows.Forms.sendToBack
            System.Windows.Forms.Server.Arrow
            System.Windows.Forms.SplitContainer
            System.Windows.Forms.Splitter
            System.Windows.Forms.SplitterPanel
            System.Windows.Forms.StatusStrip
            System.Windows.Forms.StatusStripPanel
            System.Windows.Forms.TabControl
            System.Windows.Forms.TableLayoutPanel
            System.Windows.Forms.TabPage
            System.Windows.Forms.TabStrip
            System.Windows.Forms.TextBox
            System.Windows.Forms.Timer
            System.Windows.Forms.ToolBar
            System.Windows.Forms.ToolBarGrip
            System.Windows.Forms.ToolStrip
            System.Windows.Forms.ToolStripButton
            System.Windows.Forms.ToolStripComboBox
            System.Windows.Forms.ToolStripContainer
            System.Windows.Forms.ToolStripContainer_BottomToolStripPanel
            System.Windows.Forms.ToolStripContainer_LeftToolStripPanel
            System.Windows.Forms.ToolStripContainer_RightToolStripPanel
            System.Windows.Forms.ToolStripContainer_TopToolStripPanel
            System.Windows.Forms.ToolStripContentPanel
            System.Windows.Forms.ToolStripDropDown
            System.Windows.Forms.ToolStripDropDownButton
            System.Windows.Forms.ToolStripDropDownMenu
            System.Windows.Forms.ToolStripLabel
            System.Windows.Forms.ToolStripMenuItem
            System.Windows.Forms.ToolStripPanel_standalone
            System.Windows.Forms.ToolStripProgressBar
            System.Windows.Forms.ToolStripSeparator
            System.Windows.Forms.ToolStripSplitButton
            System.Windows.Forms.ToolStripStatusLabel
            System.Windows.Forms.ToolStripTextBox
            System.Windows.Forms.ToolTip
            System.Windows.Forms.TrackBar
            System.Windows.Forms.TrayIcon
            System.Windows.Forms.TreeView
            System.Windows.Forms.up
            System.Windows.Forms.UserControl
            System.Windows.Forms.viewcode
            System.Windows.Forms.VScrollBar
            System.Windows.Forms.WebBrowser
            System.Windows.Forms.wfc
            """;

    public static TheoryData<string> ExpectedIconNames() => new(ExpectedIconNamesString.Split(Environment.NewLine));

    [Theory]
    [MemberData(nameof(ExpectedIconNames))]
    public void EmbeddedResource_ResourcesExist_Icon(string resourceName)
    {
        using Stream stream = _assembly.GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using Icon icon = new(stream);
        Assert.NotNull(icon);
    }

    private const string ExpectedCursorNamesString = """
            System.Windows.Forms.east.cur
            System.Windows.Forms.hsplit.cur
            System.Windows.Forms.ne.cur
            System.Windows.Forms.nomove2d.cur
            System.Windows.Forms.nomoveh.cur
            System.Windows.Forms.nomovev.cur
            System.Windows.Forms.north.cur
            System.Windows.Forms.nw.cur
            System.Windows.Forms.se.cur
            System.Windows.Forms.south.cur
            System.Windows.Forms.sw.cur
            System.Windows.Forms.vsplit.cur
            System.Windows.Forms.west.cur
            """;

    public static TheoryData<string> ExpectedCursorNames() => new(ExpectedCursorNamesString.Split(Environment.NewLine));

    [Theory]
    [MemberData(nameof(ExpectedCursorNames))]
    public void EmbeddedResource_ResourcesExist_Cursor(string resourceName)
    {
        using Stream stream = _assembly.GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using Cursor cursor = new(stream);
        Assert.NotNull(cursor);
    }

    private const string ExpectedResourceNames = """
            ILLink.Substitutions.xml
            System.SR.resources
            System.Windows.Forms.MdiWindowDialog.resources
            System.Windows.Forms.PrintPreviewDialog.resources
            System.Windows.Forms.XPThemes.manifest
            """;

    [Fact]
    public void EmbeddedResource_VerifyList()
    {
        string[] actual = _assembly.GetManifestResourceNames();
        Array.Sort(actual, StringComparer.Ordinal);

        string allNames = $"{ExpectedIconNamesString}{Environment.NewLine}{ExpectedCursorNamesString}{Environment.NewLine}{ExpectedResourceNames}";
        string[] expected = allNames.Split(Environment.NewLine);
        Array.Sort(expected, StringComparer.Ordinal);

        actual.Should().Equal(expected);
    }
}
