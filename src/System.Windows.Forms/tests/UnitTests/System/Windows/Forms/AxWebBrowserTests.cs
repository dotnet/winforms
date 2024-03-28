// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;
using AxSHDocVw;

namespace System.Windows.Forms.Tests;
public class AxWebBrowserTests : IDisposable
{
    private readonly Form _form;
    private readonly AxWebBrowser _control;

    public AxWebBrowserTests()
    {
        _form = new Form();
        _control = new AxWebBrowser();
        ((ISupportInitialize)_control).BeginInit();
        _form.Controls.Add(_control);
        ((ISupportInitialize)_control).EndInit();
    }

    [WinFormsFact]
    public void AxWebBrowser_WhenInitialized_ExpectsProperties()
    {
        var properties = TypeDescriptor.GetProperties(_control);
        var events = TypeDescriptor.GetEvents(_control);

        properties.Count.Should().BeGreaterThan(0);
        events.Count.Should().BeGreaterThan(0);

        // Filters testing control properties and events so only those related to the AxWebBrowser assembly remain.
        Type assemblyType = typeof(AxWebBrowser);
        Assembly assembly = Assembly.GetAssembly(assemblyType);
        string assemblyNameFromType = assembly.GetName().Name;

        var testingControlProps = properties
            .Cast<PropertyDescriptor>()
            .Where(prop => prop.ComponentType.Assembly.GetName().Name == assemblyNameFromType)
            .Select(prop => prop.Name)
            .ToList();

        var testingControlEvents = events
            .Cast<EventDescriptor>()
            .Where(ev => ev.ComponentType.Assembly.GetName().Name == assemblyNameFromType)
            .Select(ev => ev.Name)
            .ToList();

        // Compares testing control's properties and events to those of the assembly
        TypeInfo assemblyTypeInfo = assembly.GetType(assemblyType.FullName).GetTypeInfo();
        testingControlProps.All(p => assemblyTypeInfo.DeclaredProperties.Any(ap => ap.Name == p)).Should().BeTrue();
        testingControlEvents.All(e => assemblyTypeInfo.DeclaredEvents.Any(ae => ae.Name == e)).Should().BeTrue();
    }

    public void Dispose()
    {
        // This line was added due to https://github.com/dotnet/winforms/issues/10692
        using NoAssertContext context = new NoAssertContext();
        _control.Dispose();
        _form.Dispose();
    }
}
