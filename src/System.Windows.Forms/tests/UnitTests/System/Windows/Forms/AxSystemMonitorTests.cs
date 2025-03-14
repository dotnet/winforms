// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.Tests;

public class AxSystemMonitorTests : IDisposable
{
    private readonly Form _form;
    private readonly AxSystemMonitor.AxSystemMonitor _control;

    public AxSystemMonitorTests()
    {
        _form = new Form();
        _control = new AxSystemMonitor.AxSystemMonitor();
        ((ISupportInitialize)_control).BeginInit();
        _form.Controls.Add(_control);
        ((ISupportInitialize)_control).EndInit();
    }

    [WinFormsFact]
    public void AxSystemMonitor_WhenInitialized_ExpectsProperties()
    {
        var properties = TypeDescriptor.GetProperties(_control);
        var events = TypeDescriptor.GetEvents(_control);

        Assert.NotEmpty(properties);
        Assert.NotEmpty(events);
        Assert.True(_control.Enabled);
        Assert.Equal(0, _control.Counters.Count);

        // Filters testing control properties and events so only those related to the SystemMonitor assembly remain.
        Type assemblyType = typeof(AxSystemMonitor.AxSystemMonitor);
        Assembly assembly = Assembly.GetAssembly(assemblyType);
        string assemblyNameFromType = assembly.GetName().Name;

        List<string> testingControlProps = [];
        foreach (PropertyDescriptor prop in properties)
        {
            string assemblyFromTestingControl = prop.ComponentType.Assembly.GetName().Name;
            if (!string.IsNullOrEmpty(assemblyFromTestingControl)
                && assemblyFromTestingControl == assemblyNameFromType)
            {
                testingControlProps.Add(prop.Name);
            }
        }

        List<string> testingControlEvents = [];
        foreach (EventDescriptor singleEvent in events)
        {
            string assemblyFromTestingControl = singleEvent.ComponentType.Assembly.GetName().Name;
            if (!string.IsNullOrEmpty(assemblyFromTestingControl)
                && assemblyFromTestingControl == assemblyNameFromType)
            {
                testingControlEvents.Add(singleEvent.Name);
            }
        }

        // Compares testing control's properties and events to those of the assembly
        TypeInfo assemblyTypeInfo = assembly.GetType(assemblyType.FullName).GetTypeInfo();
        Assert.True(testingControlProps.All(p => assemblyTypeInfo.DeclaredProperties.Any(ap => ap.Name == p)));
        Assert.True(testingControlEvents.All(e => assemblyTypeInfo.DeclaredEvents.Any(ae => ae.Name == e)));
    }

    public void Dispose()
    {
        // This line was added due to https://github.com/dotnet/winforms/issues/10692
        using NoAssertContext context = new();
        _control.Dispose();
        _form.Dispose();
    }
}
