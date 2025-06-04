// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.ApplicationServices.Tests;

public class ApplicationBaseTests
{
    [Fact]
    public void Culture()
    {
        ApplicationBase app = new();
        var culture = app.Culture;
        Assert.Equal(Thread.CurrentThread.CurrentCulture, culture);
        try
        {
            app.ChangeCulture("en-US");
            Assert.Equal(Thread.CurrentThread.CurrentCulture, app.Culture);
            Assert.Equal("en-US", app.Culture.Name);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = culture;
        }
    }

    [Fact]
    public void UICulture()
    {
        ApplicationBase app = new();
        var culture = app.UICulture;
        Assert.Equal(Thread.CurrentThread.CurrentUICulture, culture);
        try
        {
            app.ChangeUICulture("en-US");
            Assert.Equal(Thread.CurrentThread.CurrentUICulture, app.UICulture);
            Assert.Equal("en-US", app.UICulture.Name);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }

    [Fact]
    public void GetEnvironmentVariable()
    {
        ApplicationBase app = new();
        foreach (var (key, value) in GetEnvironmentVariables())
        {
            Assert.Equal(value, app.GetEnvironmentVariable(key));
        }
    }

    [Fact]
    public void GetEnvironmentVariable_ArgumentException()
    {
        ApplicationBase app = new();
        string key = GetEnvironmentVariables().LastOrDefault().Item1 ?? "";
        var ex = Assert.Throws<ArgumentException>(() => app.GetEnvironmentVariable($"{key}z"));
        _ = ex.ToString(); // ensure message can be formatted
    }

    private static (string, string)[] GetEnvironmentVariables()
    {
        List<(string, string)> pairs = [];
        var vars = Environment.GetEnvironmentVariables();
        foreach (string key in vars.Keys)
        {
            pairs.Add((key, (string)vars[key]));
        }

        return [.. pairs.OrderBy(pair => pair.Item1)];
    }

    [Fact]
    [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Different entry assembly")]
    public void Info()
    {
        ApplicationBase app = new();
        var assembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();
        var assemblyName = assembly.GetName();
        Assert.Equal(assemblyName.Name, app.Info.AssemblyName);
        Assert.Equal(assemblyName.Version, app.Info.Version);
    }

    [Fact]
    public void Log()
    {
        ApplicationBase app = new();
        var log = app.Log;
        _ = log.TraceSource;
        _ = log.DefaultFileLogWriter;
    }
}
