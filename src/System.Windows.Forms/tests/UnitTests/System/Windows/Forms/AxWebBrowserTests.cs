// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;
using AxSHDocVw;
using SHDocVw;

namespace System.Windows.Forms.Tests;
public class AxWebBrowserTests : IDisposable
{
    private readonly Form _form;
    private readonly AxWebBrowser _control;
    private object _url = "https://github.com/dotnet/winforms";

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

    [WinFormsFact]
    public void AxWebBrowser_GoHome_ShouldNotThrowException()
    {
        _control.Invoking(c => c.GoHome()).Should().NotThrow();
    }

    [WinFormsFact]
    public void AxWebBrowser_GoSearch_ShouldNotThrowException()
    {
        _control.Invoking(c => c.GoSearch()).Should().NotThrow();
    }

    [WinFormsFact]
    public void AxWebBrowser_Navigate_ShouldNotThrowException()
    {
        _control.Invoking(c => c.Navigate(_url.ToString())).Should().NotThrow();
    }

    [WinFormsFact]
    public void AxWebBrowser_Navigate2_ShouldRaiseBeforeNavigate2()
    {
        bool eventRaised = false;
        _control.BeforeNavigate2 += (sender, e) => eventRaised = true;

        _control.Invoking(c => c.Navigate2(ref _url)).Should().NotThrow();

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void AxWebBrowser_Stop_ShouldNotThrowException()
    {
        _control.Invoking(c => c.Stop()).Should().NotThrow();
    }

    [WinFormsFact]
    public void AxWebBrowser_GetProperty_ShouldNotThrowException()
    {
        _control.Invoking(c => c.GetProperty("url")).Should().NotThrow();
    }

    [WinFormsFact]
    public void AxWebBrowser_PutProperty_ReturnsExpected()
    {
        object prop = _control.GetProperty("url");
        prop.Should().BeNull();

        _control.Invoking(c => c.PutProperty("url", _url)).Should().NotThrow();

        prop = _control.GetProperty("url");
        prop.Should().NotBeNull();
        prop.Should().Be(_url.ToString());
    }

    [WinFormsTheory]
    [InlineData(OLECMDID.OLECMDID_OPEN)]
    [InlineData(OLECMDID.OLECMDID_FOCUSVIEWCONTROLS)]
    [InlineData(OLECMDID.OLECMDID_CLOSE)]
    public void AxWebBrowser_QueryStatusWB_ShouldNotThrowException(OLECMDID cmdId)
    {
        _control.Invoking(c => c.QueryStatusWB(cmdId)).Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(OLECMDID.OLECMDID_COPY, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT)]
    [InlineData(OLECMDID.OLECMDID_COPY, OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER)]
    [InlineData(OLECMDID.OLECMDID_COPY, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER)]
    [InlineData(OLECMDID.OLECMDID_COPY, OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP)]
    [InlineData(OLECMDID.OLECMDID_STOP, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT)]
    [InlineData(OLECMDID.OLECMDID_STOP, OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER)]
    [InlineData(OLECMDID.OLECMDID_STOP, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER)]
    [InlineData(OLECMDID.OLECMDID_STOP, OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP)]
    [InlineData(OLECMDID.OLECMDID_CLOSE, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT)]
    [InlineData(OLECMDID.OLECMDID_CLOSE, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER)]
    [InlineData(OLECMDID.OLECMDID_CLOSE, OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER)]
    [InlineData(OLECMDID.OLECMDID_CLOSE, OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP)]
    public void AxWebBrowser_ExecWB_ShouldNotThrowException(OLECMDID cmdId, OLECMDEXECOPT cmdExecOpt)
    {
        _control.Invoking(c => c.ExecWB(cmdId, cmdExecOpt)).Should().NotThrow();
    }

    public void Dispose()
    {
        _control.Dispose();
        _form.Dispose();
    }
}
