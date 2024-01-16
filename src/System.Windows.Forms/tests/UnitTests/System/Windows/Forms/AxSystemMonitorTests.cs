using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public class AxSystemMonitorTests
{
    private readonly Form form;
    private readonly AxSystemMonitor.AxSystemMonitor control;

    public AxSystemMonitorTests()
    {
        form = new Form();
        control = new AxSystemMonitor.AxSystemMonitor();
        ((ISupportInitialize)control).BeginInit();
        form.Controls.Add(control);
        ((ISupportInitialize)control).EndInit();
    }

    [WinFormsFact]
    public void AxSystemMonitor_WhenInitialized_ExpectsProperties()
    {
        var properties = TypeDescriptor.GetProperties(control);
        Assert.NotEmpty(properties);

        var events = TypeDescriptor.GetEvents(control);
        Assert.NotEmpty(events);
    }

    [WinFormsFact]
    public void AxSystemMonitor_WhenInitialized_IsEnabled()
    {
        Assert.True(control.Enabled);
    }

    [WinFormsFact]
    public void AxSystemMonitor_Counter_ReturnsDefaultCounter()
    {
        Assert.Equal(0, control.Counters.Count);
    }
}
