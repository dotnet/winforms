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

        // Finds out desired assembly properties
        const string ClassName = "AxSystemMonitor.AxSystemMonitor";
        Assembly assembly = Assembly.GetAssembly(typeof(AxSystemMonitor.AxSystemMonitor));
        Type classType = assembly.GetType(ClassName);
        TypeInfo classTypeInfo = classType.GetTypeInfo();
        var assemblyClassProperties = classTypeInfo.DeclaredProperties;
        List<string> assemblyClassPropertyNames = new();
        assemblyClassPropertyNames = assemblyClassProperties.Select(p => p.Name).ToList();

        // Finds out testing control properties that come from the right assembly
        List<string> testingControlProps = new();
        for (int i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            string assemblyNameFromControl = property.ComponentType.AssemblyQualifiedName.Split(", ")[0];
            if (!assemblyNameFromControl.IsNullOrEmpty() && assemblyNameFromControl == assembly.GetName().Name)
            {
                testingControlProps.Add(property.Name);
            }
        }

        // Tests if the properties from control and assembly properly match
        foreach (string testControlProp in testingControlProps)
        {
            string assemblyProp = assemblyClassPropertyNames.Where(p => p == testControlProp).First();
            Assert.Contains(testControlProp, assemblyProp);
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
