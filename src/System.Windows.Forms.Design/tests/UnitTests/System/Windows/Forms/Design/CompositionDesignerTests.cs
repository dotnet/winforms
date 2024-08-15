// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design.Tests;

public class CompositionDesignerTests
{
    [Fact]
    public void Designer_ThrowsNotImplementedException()
    {
        ComponentDocumentDesigner designer = new();

        designer.Invoking(d => d.Control).Should().Throw<NotImplementedException>();
        designer.Invoking(d => d.TrayAutoArrange).Should().Throw<NotImplementedException>();
        designer.Invoking(d => d.TrayAutoArrange = true).Should().Throw<NotImplementedException>();
        designer.Invoking(d => d.TrayLargeIcon).Should().Throw<NotImplementedException>();
        designer.Invoking(d => d.TrayLargeIcon = true).Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void IOleDragClient_ThrowsNotImplementedException()
    {
        ComponentDocumentDesigner designer = new();
        Component component = new();
        IOleDragClient oleDragClient = designer;

        designer.Invoking(d => d.Initialize(component)).Should().Throw<NotImplementedException>();
        oleDragClient.Invoking(o => o.CanModifyComponents).Should().Throw<NotImplementedException>();
        oleDragClient.Invoking(o => o.AddComponent(component, "name", true)).Should().Throw<NotImplementedException>();
        oleDragClient.Invoking(o => o.IsDropOk(component)).Should().Throw<NotImplementedException>();
        oleDragClient.Invoking(o => o.GetDesignerControl()).Should().Throw<NotImplementedException>();
        oleDragClient.Invoking(o => o.GetControlForComponent(component)).Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void IRootDesigner_ThrowsNotImplementedException()
    {
        ComponentDocumentDesigner designer = new();
        IRootDesigner rootDesigner = designer;

        rootDesigner.Invoking(r => r.SupportedTechnologies).Should().Throw<NotImplementedException>();
        rootDesigner.Invoking(r => r.GetView(ViewTechnology.Default)).Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void IToolboxUser_ThrowsNotImplementedException()
    {
        ComponentDocumentDesigner designer = new();
        IToolboxUser toolboxUser = designer;
        ToolboxItem tool = new();

        toolboxUser.Invoking(t => t.GetToolSupported(tool)).Should().Throw<NotImplementedException>();
        toolboxUser.Invoking(t => t.ToolPicked(tool)).Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ITypeDescriptorFilterService_ThrowsNotImplementedException()
    {
        ComponentDocumentDesigner designer = new();
        ITypeDescriptorFilterService filterService = designer;
        Component component = new();
        Hashtable attributes = new();
        Hashtable events = new();
        Hashtable properties = new();

        filterService.Invoking(f => f.FilterAttributes(component, attributes)).Should().Throw<NotImplementedException>();
        filterService.Invoking(f => f.FilterEvents(component, events)).Should().Throw<NotImplementedException>();
        filterService.Invoking(f => f.FilterProperties(component, properties)).Should().Throw<NotImplementedException>();
    }
}

