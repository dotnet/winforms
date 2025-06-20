// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ToolStripContainer_ToolStripContainerTypedControlCollectionTests
{
    private static ToolStripContainer.ToolStripContainerTypedControlCollection CreateCollection(bool isReadOnly) =>
        new(new ToolStripContainer(), isReadOnly);

    [WinFormsFact]
    public void Add_NullValue_DoesNothing()
    {
        ToolStripContainer.ToolStripContainerTypedControlCollection collection = CreateCollection(isReadOnly: false);

        collection.Add(null);

        collection.Cast<Control>().Should().BeEmpty();
    }

    [WinFormsFact]
    public void Add_WhenReadOnly_ThrowsNotSupportedException()
    {
        ToolStripContainer.ToolStripContainerTypedControlCollection collection = CreateCollection(isReadOnly: true);
        using ToolStripPanel panel = new();

        Action act = () => collection.Add(panel);

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void Add_InvalidType_ThrowsArgumentException()
    {
        ToolStripContainer.ToolStripContainerTypedControlCollection collection = CreateCollection(isReadOnly: false);
        using Button button = new();

        Action act = () => collection.Add(button);

        act.Should().Throw<ArgumentException>();
    }

    [WinFormsTheory]
    [InlineData(typeof(ToolStripPanel))]
    [InlineData(typeof(ToolStripContentPanel))]
    public void Add_ValidType_AddsSuccessfully(Type type)
    {
        using Form form = new();
        using ToolStripContainer container = new();
        form.Controls.Add(container);

        ToolStripContainer.ToolStripContainerTypedControlCollection collection = new(container, isReadOnly: false);

        Control control = (Control)Activator.CreateInstance(type)!;

        collection.Add(control);

        collection.Cast<Control>().Should().Contain(control);

        collection.Remove(control);
        control.Dispose();
    }

    [WinFormsTheory]
    [InlineData(typeof(ToolStripPanel))]
    [InlineData(typeof(ToolStripContentPanel))]
    public void Remove_WhenReadOnly_ThrowsNotSupportedException(Type type)
    {
        using ToolStripContainer container = new();
        ToolStripContainer.ToolStripContainerTypedControlCollection collection = new(container, isReadOnly: true);
        using Control control = (Control)Activator.CreateInstance(type)!;
        container.ContentPanel.Controls.Add(control);

        Action act = () => collection.Remove(control);

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void Remove_Panel_WhenNotReadOnly_RemovesPanel()
    {
        ToolStripContainer.ToolStripContainerTypedControlCollection collection = CreateCollection(isReadOnly: false);
        using ToolStripPanel panel = new();
        collection.Add(panel);

        collection.Remove(panel);

        collection.Cast<Control>().Should().NotContain(panel);
    }

    [WinFormsFact]
    public void Remove_NonPanelOrContentPanel_DoesNotThrow()
    {
        using ToolStripContainer container = new();
        ToolStripContainer.ToolStripContainerTypedControlCollection collection = new(container, isReadOnly: false);
        using Button button = new();
        container.ContentPanel.Controls.Add(button);

        Action act = () => collection.Remove(button);

        act.Should().NotThrow();
        collection.Cast<Control>().Should().NotContain(button);
    }

    [WinFormsTheory]
    [InlineData(typeof(ToolStripPanel))]
    [InlineData(typeof(ToolStripContentPanel))]
    public void SetChildIndexInternal_WhenReadOnly_ThrowsNotSupportedException(Type type)
    {
        using ToolStripContainer container = new();
        ToolStripContainer.ToolStripContainerTypedControlCollection collection = new(container, isReadOnly: true);
        using Control control = (Control)Activator.CreateInstance(type)!;
        container.ContentPanel.Controls.Add(control);

        Action act = () => collection.SetChildIndexInternal(control, 0);

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void SetChildIndexInternal_Panel_WhenNotReadOnly_CallsBase()
    {
        using Form form = new();
        using ToolStripContainer container = new();
        form.Controls.Add(container);

        ToolStripContainer.ToolStripContainerTypedControlCollection collection = new(container, isReadOnly: false);

        ToolStripPanel panel = new();
        collection.Add(panel);

        collection.SetChildIndexInternal(panel, 0);

        collection.GetChildIndex(panel).Should().Be(0);

        collection.Remove(panel);
        panel.Dispose();
    }
}
