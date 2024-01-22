using System.ComponentModel;
using System.Reflection;
using Castle.Core.Internal;

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
        Assert.NotEmpty(properties);

        var events = TypeDescriptor.GetEvents(_control);
        Assert.NotEmpty(events);

        Assert.True(_control.Enabled);
        Assert.Equal(0, _control.Counters.Count);

        // Finds out the class's assembly properties and events
        Type type = typeof(AxSystemMonitor.AxSystemMonitor);
        Assembly assembly = Assembly.GetAssembly(type);
        Type classType = assembly.GetType(type.FullName);
        TypeInfo classTypeInfo = classType.GetTypeInfo();

        IEnumerable<PropertyInfo> assemblyClassProperties = classTypeInfo.DeclaredProperties;
        List<string> assemblyClassPropertyNames = new();
        assemblyClassPropertyNames = assemblyClassProperties.Select(p => p.Name).ToList();

        IEnumerable<EventInfo> assemblyClassEvents = classTypeInfo.DeclaredEvents;
        List<string> assemblyClassEventNames = new();
        assemblyClassEventNames = assemblyClassEvents.Select(p => p.Name).ToList();

        // Finds out testing control properties and events that come from the right assembly
        string assemblyNameFromClass = assembly.GetName().Name;
        List<string> testingControlProps = new();
        for (int i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            string assemblyNameFromProperty = property.ComponentType.Assembly.GetName().Name;
            if (!assemblyNameFromProperty.IsNullOrEmpty() && assemblyNameFromProperty == assemblyNameFromClass)
            {
                testingControlProps.Add(property.Name);
            }
        }

        List<string> testingControlEvents = new();
        for (int i = 0; i < events.Count; i++)
        {
            var singleEvent = events[i];
            string assemblyNameFromProperty = singleEvent.ComponentType.Assembly.GetName().Name;
            if (!assemblyNameFromProperty.IsNullOrEmpty() && assemblyNameFromProperty == assemblyNameFromClass)
            {
                testingControlEvents.Add(singleEvent.Name);
            }
        }

        // Tests if the properties and events from control and assembly properly match
        foreach (string testControlProp in testingControlProps)
        {
            string assemblyProp = assemblyClassPropertyNames.Where(p => p == testControlProp).First();
            Assert.Contains(testControlProp, assemblyProp);
        }

        foreach (string testControlEvent in testingControlEvents)
        {
            string assemblyEvent = assemblyClassEventNames.Where(p => p == testControlEvent).First();
            Assert.Contains(testControlEvent, assemblyEvent);
        }
    }

    public void Dispose()
    {
        // This line was added due to https://github.com/dotnet/winforms/issues/10692
        using NoAssertContext context = new();
        _control.Dispose();
        _form.Dispose();
    }
}
