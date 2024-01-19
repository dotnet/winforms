using System.ComponentModel;

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
    }

    public void Dispose()
    {
        // This line was added due to https://github.com/dotnet/winforms/issues/10692
        using NoAssertContext context = new();
        _control.Dispose();
        _form.Dispose();
    }
}
