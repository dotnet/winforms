// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.PropertyGridInternal;
using Moq;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Behavior tests for the <see cref="PropertyGrid"/> description pane, commands (verbs) pane,
///  toolbar and the reset menu command routing.
/// </summary>
public class PropertyGridPanesAndCommandsTests
{
    private const string NameDescription = "Gets or sets the widget name.";
    private const string SizeDescription = "Gets or sets the widget size.";

    private static HelpPane GetHelpPane(PropertyGrid grid) => grid.Controls.OfType<HelpPane>().Single();

    private static CommandsPane GetCommandsPane(PropertyGrid grid) => grid.Controls.OfType<CommandsPane>().Single();

    // The reset command lookup resolves services through the grid view control's own site, which the
    // PropertyGrid site setter does not assign, so the site is attached to the grid view directly.
    private static void AttachMenuCommandService(PropertyGrid grid, ISite site)
        => grid.Controls.OfType<PropertyGridView>().Single().Site = site;

    private static ToolStrip GetToolStrip(PropertyGrid grid) => grid.Controls.OfType<ToolStrip>().Single();

    private static (string Title, string Description) GetHelpText(PropertyGrid grid)
    {
        Label[] labels = GetHelpPane(grid).Controls.OfType<Label>().ToArray();
        labels.Should().HaveCount(2);
        return (labels[0].Text, labels[1].Text);
    }

    private static GridItem FindProperty(PropertyGrid grid, string name)
    {
        GridItem root = grid.SelectedGridItem!;
        while (root.Parent is not null)
        {
            root = root.Parent;
        }

        Queue<GridItem> pending = new([root]);
        while (pending.Count > 0)
        {
            GridItem item = pending.Dequeue();
            if (item.PropertyDescriptor?.Name == name)
            {
                return item;
            }

            foreach (GridItem child in item.GridItems)
            {
                pending.Enqueue(child);
            }
        }

        throw new InvalidOperationException($"Property {name} not found.");
    }

    private static PropertyGrid CreateGrid(object? selectedObject = null, ISite? site = null)
    {
        PropertyGrid grid = new()
        {
            PropertySort = PropertySort.Alphabetical
        };

        if (site is not null)
        {
            grid.Site = site;
        }

        grid.CreateControl();
        if (selectedObject is not null)
        {
            grid.SelectedObject = selectedObject;
        }

        return grid;
    }

    private static Mock<ISite> CreateSite(Action<Mock<ISite>>? configure = null, bool designMode = false)
    {
        Mock<ISite> site = new();
        site.SetupGet(s => s.DesignMode).Returns(designMode);
        site.SetupGet(s => s.Name).Returns("site1");
        configure?.Invoke(site);
        return site;
    }

    private static SampleComponent CreateSitedComponent(ISite site)
    {
        return new SampleComponent { Site = site };
    }

    private static Mock<IMenuCommandService> CreateMenuCommandService(params DesignerVerb[] verbs)
    {
        Mock<IMenuCommandService> service = new();
        service.SetupGet(s => s.Verbs).Returns(new DesignerVerbCollection(verbs));
        return service;
    }

    private static Mock<ISite> CreateComponentSite(IMenuCommandService? menuCommandService, IDesignerHost? designerHost = null)
        => CreateSite(site =>
        {
            site.Setup(s => s.GetService(typeof(IMenuCommandService))).Returns(menuCommandService);
            site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(designerHost);
        });

    // Scenario: Selecting a property shows its display name as the help pane title and its
    // [Description] text as the help pane body.
    [WinFormsFact]
    public void HelpPane_SelectingProperty_ShowsNameAndDescription()
    {
        using SampleComponent selected = new();
        using PropertyGrid grid = CreateGrid(selected);

        FindProperty(grid, nameof(SampleComponent.WidgetName)).Select().Should().BeTrue();

        GetHelpText(grid).Should().Be((nameof(SampleComponent.WidgetName), NameDescription));
    }

    // Scenario: Moving the selection to another property replaces the help pane title and body
    // so they always describe the currently selected property.
    [WinFormsFact]
    public void HelpPane_ChangingSelection_ReplacesTitleAndDescription()
    {
        using SampleComponent selected = new();
        using PropertyGrid grid = CreateGrid(selected);

        FindProperty(grid, nameof(SampleComponent.WidgetName)).Select().Should().BeTrue();
        FindProperty(grid, nameof(SampleComponent.WidgetSize)).Select().Should().BeTrue();

        GetHelpText(grid).Should().Be((nameof(SampleComponent.WidgetSize), SizeDescription));
    }

    // Scenario: A property without a description still shows its name in the title and leaves
    // the body empty instead of keeping the previous property's text.
    [WinFormsFact]
    public void HelpPane_PropertyWithoutDescription_ShowsNameAndEmptyBody()
    {
        using SampleComponent selected = new();
        using PropertyGrid grid = CreateGrid(selected);

        FindProperty(grid, nameof(SampleComponent.WidgetName)).Select().Should().BeTrue();
        FindProperty(grid, nameof(SampleComponent.Undocumented)).Select().Should().BeTrue();

        (string title, string description) = GetHelpText(grid);
        title.Should().Be(nameof(SampleComponent.Undocumented));
        description.Should().BeNullOrEmpty();
    }

    // Scenario: Clearing the selected object removes all entries, so the help pane no longer
    // shows the text of the previously selected property.
    [WinFormsFact]
    public void HelpPane_ClearingSelectedObject_ClearsText()
    {
        using SampleComponent selected = new();
        using PropertyGrid grid = CreateGrid(selected);

        FindProperty(grid, nameof(SampleComponent.WidgetName)).Select().Should().BeTrue();
        grid.SelectedObject = null;

        (string title, string description) = GetHelpText(grid);
        title.Should().BeNullOrEmpty();
        description.Should().BeNullOrEmpty();
    }

    // Scenario: Selecting a long description keeps the complete text available to accessibility
    // clients even though the visible label may truncate it.
    [WinFormsFact]
    public void HelpPane_Description_IsFullyExposedToAccessibility()
    {
        using SampleComponent selected = new();
        using PropertyGrid grid = CreateGrid(selected);

        FindProperty(grid, nameof(SampleComponent.WidgetSize)).Select().Should().BeTrue();

        Label descriptionLabel = GetHelpPane(grid).Controls.OfType<Label>().Last();
        descriptionLabel.AccessibleName.Should().Be(SizeDescription);
    }

    // Scenario: HelpVisible shows and hides the description pane, and the setting survives
    // selection changes.
    [WinFormsFact]
    public void HelpPane_HelpVisible_TogglesPaneVisibility()
    {
        using SampleComponent selected = new();
        using PropertyGrid grid = CreateGrid(selected);
        HelpPane helpPane = GetHelpPane(grid);

        grid.HelpVisible.Should().BeTrue();
        helpPane.Visible.Should().BeTrue();

        grid.HelpVisible = false;
        helpPane.Visible.Should().BeFalse();
        FindProperty(grid, nameof(SampleComponent.WidgetName)).Select().Should().BeTrue();
        helpPane.Visible.Should().BeFalse();

        grid.HelpVisible = true;
        helpPane.Visible.Should().BeTrue();
        GetHelpText(grid).Title.Should().Be(nameof(SampleComponent.WidgetName));
    }

    // Scenario: HelpBackColor, HelpForeColor and HelpBorderColor are applied to the actual
    // description pane rather than only stored on the grid.
    [WinFormsFact]
    public void HelpPane_HelpColors_AreAppliedToPane()
    {
        using PropertyGrid grid = CreateGrid();
        HelpPane helpPane = GetHelpPane(grid);

        grid.HelpBackColor = Color.Yellow;
        grid.HelpForeColor = Color.Green;
        grid.HelpBorderColor = Color.Purple;

        helpPane.BackColor.Should().Be(Color.Yellow);
        helpPane.ForeColor.Should().Be(Color.Green);
        helpPane.BorderColor.Should().Be(Color.Purple);
    }

    // Scenario: The help pane exposes a UI Automation pane control type and a non-null name.
    [WinFormsFact]
    public void HelpPane_AccessibleObject_ExposesPaneControlType()
    {
        using PropertyGrid grid = CreateGrid();

        ((int)grid.HelpPaneAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId))
            .Should().Be((int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId);
        grid.HelpPaneAccessibleObject.Name.Should().NotBeNull();
    }

    // Scenario: A component whose site provides a menu command service shows its visible and
    // supported verbs as comma-separated links in the commands pane.
    [WinFormsFact]
    public void CommandsPane_MenuCommandServiceVerbs_AreListedAsLinks()
    {
        DesignerVerb first = new("First verb", (_, _) => { });
        DesignerVerb second = new("Second verb", (_, _) => { });
        Mock<ISite> componentSite = CreateComponentSite(CreateMenuCommandService(first, second).Object);
        using SampleComponent selected = CreateSitedComponent(componentSite.Object);
        using PropertyGrid grid = CreateGrid(selected);

        CommandsPane pane = GetCommandsPane(grid);
        grid.CommandsVisible.Should().BeTrue();
        grid.CanShowCommands.Should().BeTrue();
        pane.Visible.Should().BeTrue();
        pane.Label.Text.Should().Be($"First verb{Application.CurrentCulture.TextInfo.ListSeparator} Second verb");
        pane.Label.Links.Cast<LinkLabel.Link>().Select(link => link.LinkData).Should().Equal(first, second);
        pane.Label.Links[0].Start.Should().Be(0);
        pane.Label.Links[0].Length.Should().Be("First verb".Length);
    }

    // Scenario: Verbs that are hidden or unsupported are left out of the commands pane, and a
    // disabled verb is shown as a disabled link.
    [WinFormsFact]
    public void CommandsPane_HiddenUnsupportedAndDisabledVerbs_AreFilteredOrDisabled()
    {
        DesignerVerb visible = new("Visible verb", (_, _) => { });
        DesignerVerb hidden = new("Hidden verb", (_, _) => { }) { Visible = false };
        DesignerVerb unsupported = new("Unsupported verb", (_, _) => { }) { Supported = false };
        DesignerVerb disabled = new("Disabled verb", (_, _) => { }) { Enabled = false };
        Mock<ISite> componentSite = CreateComponentSite(CreateMenuCommandService(visible, hidden, unsupported, disabled).Object);
        using SampleComponent selected = CreateSitedComponent(componentSite.Object);
        using PropertyGrid grid = CreateGrid(selected);

        LinkLabel label = GetCommandsPane(grid).Label;
        label.Text.Should().NotContain("Hidden").And.NotContain("Unsupported");
        label.Links.Count.Should().Be(2);
        label.Links[0].Enabled.Should().BeTrue();
        label.Links[1].LinkData.Should().BeSameAs(disabled);
        label.Links[1].Enabled.Should().BeFalse();
    }

    // Scenario: Activating an enabled verb link runs the verb's handler exactly once, while
    // activating a disabled verb link does nothing.
    [WinFormsFact]
    public void CommandsPane_LinkClick_InvokesOnlyEnabledVerbs()
    {
        int enabledCount = 0;
        int disabledCount = 0;
        DesignerVerb enabled = new("Enabled verb", (_, _) => enabledCount++);
        DesignerVerb disabled = new("Disabled verb", (_, _) => disabledCount++) { Enabled = false };
        Mock<ISite> componentSite = CreateComponentSite(CreateMenuCommandService(enabled, disabled).Object);
        using SampleComponent selected = CreateSitedComponent(componentSite.Object);
        using PropertyGrid grid = CreateGrid(selected);
        CommandsPane pane = GetCommandsPane(grid);

        foreach (LinkLabel.Link link in pane.Label.Links.Cast<LinkLabel.Link>())
        {
            pane.TestAccessor.Dynamic.LinkClicked(pane.Label, new LinkLabelLinkClickedEventArgs(link));
        }

        enabledCount.Should().Be(1);
        disabledCount.Should().Be(0);
    }

    // Scenario: Changing a verb's Enabled or Visible state at run time refreshes the links
    // shown in the commands pane.
    [WinFormsFact]
    public void CommandsPane_VerbStateChange_RefreshesLinks()
    {
        DesignerVerb first = new("First verb", (_, _) => { });
        DesignerVerb second = new("Second verb", (_, _) => { });
        Mock<ISite> componentSite = CreateComponentSite(CreateMenuCommandService(first, second).Object);
        using SampleComponent selected = CreateSitedComponent(componentSite.Object);
        using PropertyGrid grid = CreateGrid(selected);
        LinkLabel label = GetCommandsPane(grid).Label;

        second.Enabled = false;
        label.Links.Count.Should().Be(2);
        label.Links[1].Enabled.Should().BeFalse();

        first.Visible = false;
        label.Text.Should().Be("Second verb");
        label.Links.Count.Should().Be(1);
        label.Links[0].LinkData.Should().BeSameAs(second);

        first.Visible = true;
        label.Links.Count.Should().Be(2);
    }

    // Scenario: Without a menu command service, a single selected component falls back to the
    // verbs of its designer from the designer host.
    [WinFormsFact]
    public void CommandsPane_NoMenuCommandService_UsesDesignerVerbs()
    {
        DesignerVerb verb = new("Designer verb", (_, _) => { });
        using SampleComponent selected = new();
        Mock<IDesigner> designer = new();
        designer.SetupGet(d => d.Verbs).Returns(new DesignerVerbCollection([verb]));
        Mock<IDesignerHost> host = new();
        host.Setup(h => h.GetDesigner(selected)).Returns(designer.Object);
        selected.Site = CreateComponentSite(menuCommandService: null, host.Object).Object;
        using PropertyGrid grid = CreateGrid(selected);

        grid.CommandsVisible.Should().BeTrue();
        GetCommandsPane(grid).Label.Text.Should().Be("Designer verb");
    }

    // Scenario: Designers do not support verbs across a multi-selection, so the designer
    // fallback shows no commands when more than one object is selected.
    [WinFormsFact]
    public void CommandsPane_DesignerFallbackWithMultiSelection_ShowsNoCommands()
    {
        DesignerVerb verb = new("Designer verb", (_, _) => { });
        using SampleComponent first = new();
        using SampleComponent second = new();
        Mock<IDesigner> designer = new();
        designer.SetupGet(d => d.Verbs).Returns(new DesignerVerbCollection([verb]));
        Mock<IDesignerHost> host = new();
        host.Setup(h => h.GetDesigner(It.IsAny<IComponent>())).Returns(designer.Object);
        first.Site = CreateComponentSite(menuCommandService: null, host.Object).Object;
        second.Site = CreateComponentSite(menuCommandService: null, host.Object).Object;
        using PropertyGrid grid = CreateGrid();

        grid.SelectedObjects = [first, second];

        grid.CommandsVisible.Should().BeFalse();
        grid.CanShowCommands.Should().BeFalse();
    }

    // Scenario: Selecting an object that is not a sited component, or clearing the selection,
    // hides the commands pane again.
    [WinFormsFact]
    public void CommandsPane_NonComponentOrClearedSelection_HidesPane()
    {
        DesignerVerb verb = new("Verb", (_, _) => { });
        Mock<ISite> componentSite = CreateComponentSite(CreateMenuCommandService(verb).Object);
        using SampleComponent selected = CreateSitedComponent(componentSite.Object);
        using PropertyGrid grid = CreateGrid(selected);
        grid.CommandsVisible.Should().BeTrue();

        grid.SelectedObject = new SamplePlainObject();
        grid.CommandsVisible.Should().BeFalse();
        GetCommandsPane(grid).Label.Text.Should().BeNullOrEmpty();
        GetCommandsPane(grid).Label.Links.Count.Should().Be(0);

        grid.SelectedObject = selected;
        grid.CommandsVisible.Should().BeTrue();

        grid.SelectedObject = null;
        grid.CommandsVisible.Should().BeFalse();
    }

    // Scenario: CommandsVisibleIfAvailable = false keeps the pane hidden even when verbs exist
    // (CanShowCommands stays true), and turning it back on shows the pane again.
    [WinFormsFact]
    public void CommandsPane_CommandsVisibleIfAvailable_ControlsVisibility()
    {
        DesignerVerb verb = new("Verb", (_, _) => { });
        Mock<ISite> componentSite = CreateComponentSite(CreateMenuCommandService(verb).Object);
        using SampleComponent selected = CreateSitedComponent(componentSite.Object);
        using PropertyGrid grid = CreateGrid(selected);

        grid.CommandsVisibleIfAvailable = false;
        grid.CommandsVisible.Should().BeFalse();
        grid.CanShowCommands.Should().BeTrue();

        grid.CommandsVisibleIfAvailable = true;
        grid.CommandsVisible.Should().BeTrue();
    }

    // Scenario: When the grid itself is in design mode (hosted on a form being designed) it
    // never shows verbs for the selected object.
    [WinFormsFact]
    public void CommandsPane_GridInDesignMode_ShowsNoCommands()
    {
        DesignerVerb verb = new("Verb", (_, _) => { });
        Mock<ISite> componentSite = CreateComponentSite(CreateMenuCommandService(verb).Object);
        using SampleComponent selected = CreateSitedComponent(componentSite.Object);
        Mock<ISite> gridSite = CreateSite(designMode: true);
        using PropertyGrid grid = CreateGrid(site: gridSite.Object);

        grid.SelectedObject = selected;

        grid.CommandsVisible.Should().BeFalse();
    }

    // Scenario: The commands pane requests more height as more verbs are listed so every verb
    // line is visible.
    [WinFormsFact]
    public void CommandsPane_OptimalHeight_GrowsWithVerbCount()
    {
        DesignerVerb one = new("One", (_, _) => { });
        DesignerVerb two = new("Two", (_, _) => { });
        DesignerVerb three = new("Three", (_, _) => { });
        Mock<ISite> componentSite = CreateComponentSite(CreateMenuCommandService(one).Object);
        using SampleComponent selected = CreateSitedComponent(componentSite.Object);
        using PropertyGrid grid = CreateGrid(selected);
        CommandsPane pane = GetCommandsPane(grid);

        int oneVerbHeight = pane.GetOptimalHeight(width: 200);

        pane.SetVerbs(selected, [one, two, three]);
        int threeVerbHeight = pane.GetOptimalHeight(width: 200);

        threeVerbHeight.Should().BeGreaterThan(oneVerbHeight);
    }

    // Scenario: The command colors set on the grid are applied to the pane's link label.
    [WinFormsFact]
    public void CommandsPane_CommandColors_AreAppliedToPaneAndLabel()
    {
        using PropertyGrid grid = CreateGrid();
        CommandsPane pane = GetCommandsPane(grid);

        grid.CommandsBackColor = Color.Yellow;
        grid.CommandsForeColor = Color.Green;
        grid.CommandsLinkColor = Color.Purple;
        grid.CommandsActiveLinkColor = Color.Orange;
        grid.CommandsDisabledLinkColor = Color.Gray;
        grid.CommandsBorderColor = Color.Black;

        pane.BackColor.Should().Be(Color.Yellow);
        pane.Label.BackColor.Should().Be(Color.Yellow);
        pane.ForeColor.Should().Be(Color.Green);
        pane.Label.ForeColor.Should().Be(Color.Green);
        pane.Label.LinkColor.Should().Be(Color.Purple);
        pane.Label.ActiveLinkColor.Should().Be(Color.Orange);
        pane.Label.DisabledLinkColor.Should().Be(Color.Gray);
        pane.BorderColor.Should().Be(Color.Black);
    }

    // Scenario: The default toolbar has an alphabetical and a categorized sort button with
    // tooltips and images, a hidden no-sort button and a disabled property pages button.
    [WinFormsFact]
    public void Toolbar_DefaultButtons_HaveExpectedMetadata()
    {
        using PropertyGrid grid = CreateGrid();
        ToolStripButton[] buttons = GetToolStrip(grid).Items.OfType<ToolStripButton>().ToArray();

        buttons.Should().HaveCount(4);
        ToolStripButton alpha = buttons[1];
        ToolStripButton category = buttons[0];
        ToolStripButton noSort = buttons[2];
        ToolStripButton propertyPages = buttons[3];

        alpha.Text.Should().NotBeNullOrEmpty();
        category.Text.Should().NotBeNullOrEmpty();
        propertyPages.Text.Should().NotBeNullOrEmpty();
        new[] { alpha.Text, category.Text, propertyPages.Text }.Should().OnlyHaveUniqueItems();
        alpha.AutoToolTip.Should().BeTrue();
        alpha.DisplayStyle.Should().Be(ToolStripItemDisplayStyle.Image);
        alpha.ImageIndex.Should().BeGreaterThanOrEqualTo(0);
        category.ImageIndex.Should().BeGreaterThanOrEqualTo(0);
        propertyPages.ImageIndex.Should().BeGreaterThanOrEqualTo(0);
        noSort.Visible.Should().BeFalse();
        propertyPages.Enabled.Should().BeFalse();
    }

    // Scenario: The sort buttons expose a radio-button accessible role, and the property pages
    // button keeps the default push-button behavior.
    [WinFormsFact]
    public void Toolbar_SortButtons_ExposeRadioButtonRole()
    {
        using PropertyGrid grid = CreateGrid();
        ToolStripButton[] buttons = GetToolStrip(grid).Items.OfType<ToolStripButton>().ToArray();

        buttons[0].AccessibilityObject.Role.Should().Be(AccessibleRole.RadioButton);
        buttons[1].AccessibilityObject.Role.Should().Be(AccessibleRole.RadioButton);
        buttons[3].AccessibilityObject.Role.Should().NotBe(AccessibleRole.RadioButton);
    }

    // Scenario: Changing PropertySort checks exactly the matching sort button and the buttons
    // stay in sync, including the hidden no-sort state.
    [WinFormsTheory]
    [InlineData(PropertySort.Alphabetical, 1)]
    [InlineData(PropertySort.Categorized, 0)]
    [InlineData(PropertySort.CategorizedAlphabetical, 0)]
    [InlineData(PropertySort.NoSort, 2)]
    public void Toolbar_PropertySort_ChecksMatchingSortButton(PropertySort sort, int expectedCheckedIndex)
    {
        using PropertyGrid grid = CreateGrid();
        ToolStripButton[] buttons = GetToolStrip(grid).Items.OfType<ToolStripButton>().ToArray();

        // Start from a different sort so the change is observable.
        grid.PropertySort = sort == PropertySort.Alphabetical ? PropertySort.Categorized : PropertySort.Alphabetical;
        grid.PropertySort = sort;

        grid.PropertySort.Should().Be(sort);
        for (int i = 0; i < 3; i++)
        {
            buttons[i].Checked.Should().Be(i == expectedCheckedIndex, $"button {i}");
        }
    }

    // Scenario: Clicking the sort buttons changes the grid's PropertySort and moves the
    // checked state; clicking the already checked button leaves it unchanged.
    [WinFormsFact]
    public void Toolbar_ClickingSortButtons_ChangesPropertySort()
    {
        using PropertyGrid grid = CreateGrid();
        ToolStripButton[] buttons = GetToolStrip(grid).Items.OfType<ToolStripButton>().ToArray();

        buttons[0].PerformClick();
        grid.PropertySort.Should().Be(PropertySort.Alphabetical | PropertySort.Categorized);
        buttons[0].Checked.Should().BeTrue();
        buttons[1].Checked.Should().BeFalse();

        buttons[1].PerformClick();
        grid.PropertySort.Should().Be(PropertySort.Alphabetical);
        buttons[1].Checked.Should().BeTrue();
        buttons[0].Checked.Should().BeFalse();

        buttons[1].PerformClick();
        grid.PropertySort.Should().Be(PropertySort.Alphabetical);
        buttons[1].Checked.Should().BeTrue();
    }

    // Scenario: Switching to categorized sorting shows category rows in the grid and
    // switching back to alphabetical shows only the flat property list.
    [WinFormsFact]
    public void Toolbar_SortSwitch_ChangesGridEntryStructure()
    {
        using SampleComponent selected = new();
        using PropertyGrid grid = CreateGrid(selected);
        ToolStripButton[] buttons = GetToolStrip(grid).Items.OfType<ToolStripButton>().ToArray();

        buttons[0].PerformClick();
        FindProperty(grid, nameof(SampleComponent.WidgetName)).Parent!.GridItemType.Should().Be(GridItemType.Category);

        buttons[1].PerformClick();
        FindProperty(grid, nameof(SampleComponent.WidgetName)).Parent!.GridItemType.Should().Be(GridItemType.Root);
    }

    // Scenario: ToolbarVisible hides and shows the toolbar control without losing its buttons.
    [WinFormsFact]
    public void Toolbar_ToolbarVisible_TogglesToolStrip()
    {
        using PropertyGrid grid = CreateGrid();
        ToolStrip toolStrip = GetToolStrip(grid);
        int buttonCount = toolStrip.Items.Count;

        grid.ToolbarVisible = false;
        toolStrip.Visible.Should().BeFalse();

        grid.ToolbarVisible = true;
        toolStrip.Visible.Should().BeTrue();
        toolStrip.Items.Count.Should().Be(buttonCount);
    }

    // Scenario: LargeButtons switches the toolbar to a larger image list and image size, and
    // turning it off again restores the normal size.
    [WinFormsFact]
    public void Toolbar_LargeButtons_SwitchesImageSize()
    {
        using PropertyGrid grid = CreateGrid();
        ToolStrip toolStrip = GetToolStrip(grid);
        Size normalSize = toolStrip.ImageScalingSize;
        ImageList normalImages = toolStrip.ImageList!;

        grid.LargeButtons = true;
        toolStrip.ImageScalingSize.Width.Should().BeGreaterThan(normalSize.Width);
        toolStrip.ImageScalingSize.Height.Should().BeGreaterThan(normalSize.Height);
        toolStrip.ImageList.Should().NotBeSameAs(normalImages);
        toolStrip.ImageList!.Images.Count.Should().Be(normalImages.Images.Count);

        grid.LargeButtons = false;
        toolStrip.ImageScalingSize.Should().Be(normalSize);
        toolStrip.ImageList.Should().BeSameAs(normalImages);
    }

    // Scenario: A toolbar rebuild while large buttons are enabled keeps the large image size.
    [WinFormsFact]
    public void Toolbar_LargeButtons_SurvivesToolbarRebuild()
    {
        using PropertyGrid grid = CreateGrid();
        ToolStrip toolStrip = GetToolStrip(grid);
        grid.LargeButtons = true;
        Size largeSize = toolStrip.ImageScalingSize;

        grid.TestAccessor.Dynamic.SetupToolbar(true);

        toolStrip.ImageScalingSize.Should().Be(largeSize);
    }

    // Scenario: The reset menu command is enabled only while the selected property has a
    // non-default value that can be reset, and follows the selection.
    [WinFormsFact]
    public void ResetCommand_FollowsSelectedPropertyResetState()
    {
        MenuCommand reset = new((_, _) => { }, PropertyGridCommands.Reset) { Enabled = false };
        Mock<IMenuCommandService> menuCommandService = new();
        menuCommandService.Setup(s => s.FindCommand(PropertyGridCommands.Reset)).Returns(reset);
        Mock<ISite> gridSite = CreateSite(site =>
            site.Setup(s => s.GetService(typeof(IMenuCommandService))).Returns(menuCommandService.Object));
        using SampleComponent selected = new() { WidgetSize = 42 };
        using PropertyGrid grid = CreateGrid(selected, gridSite.Object);
        AttachMenuCommandService(grid, gridSite.Object);

        FindProperty(grid, nameof(SampleComponent.WidgetSize)).Select().Should().BeTrue();
        reset.Enabled.Should().BeTrue();

        FindProperty(grid, nameof(SampleComponent.WidgetName)).Select().Should().BeTrue();
        reset.Enabled.Should().BeFalse();

        FindProperty(grid, nameof(SampleComponent.WidgetSize)).Select().Should().BeTrue();
        reset.Enabled.Should().BeTrue();
    }

    // Scenario: Without a menu command service the selection changes still work and update
    // the help pane, so the reset routing is optional.
    [WinFormsFact]
    public void ResetCommand_NoMenuCommandService_SelectionStillWorks()
    {
        using SampleComponent selected = new() { WidgetSize = 42 };
        using PropertyGrid grid = CreateGrid(selected);

        FindProperty(grid, nameof(SampleComponent.WidgetSize)).Select().Should().BeTrue();

        GetHelpText(grid).Title.Should().Be(nameof(SampleComponent.WidgetSize));
    }

    // Scenario: A menu command service that does not register the reset command is tolerated.
    [WinFormsFact]
    public void ResetCommand_ServiceWithoutResetCommand_IsIgnored()
    {
        Mock<IMenuCommandService> menuCommandService = new();
        menuCommandService.Setup(s => s.FindCommand(It.IsAny<CommandID>())).Returns((MenuCommand?)null);
        Mock<ISite> gridSite = CreateSite(site =>
            site.Setup(s => s.GetService(typeof(IMenuCommandService))).Returns(menuCommandService.Object));
        using SampleComponent selected = new() { WidgetSize = 42 };
        using PropertyGrid grid = CreateGrid(selected, gridSite.Object);
        AttachMenuCommandService(grid, gridSite.Object);

        FindProperty(grid, nameof(SampleComponent.WidgetSize)).Select().Should().BeTrue();

        menuCommandService.Verify(s => s.FindCommand(PropertyGridCommands.Reset), Times.AtLeastOnce);
    }

    private class SampleComponent : Component
    {
        [Description(NameDescription)]
        [DefaultValue("")]
        public string WidgetName { get; set; } = string.Empty;

        [Description(SizeDescription)]
        [DefaultValue(0)]
        public int WidgetSize { get; set; }

        public string Undocumented { get; set; } = string.Empty;
    }

    private class SamplePlainObject
    {
        public int Value { get; set; }
    }
}
