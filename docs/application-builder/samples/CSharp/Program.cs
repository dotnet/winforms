// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WinForms;

namespace ApplicationBuilderSample.CSharp;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);
        hostBuilder.Services.AddHostedService<HeartbeatService>();
        IHost host = hostBuilder.Build();

        WinFormsApplicationBuilder applicationBuilder = WinFormsApplication.CreateBuilder()
            .UseHost(host);

        if (args.Contains("--custom-context", StringComparer.OrdinalIgnoreCase))
        {
            applicationBuilder.UseApplicationContext(new MainApplicationContext());
        }
        else
        {
            applicationBuilder.UseStartupForm<MainForm>();
        }

        using WinFormsApplication application = applicationBuilder.Build();
        application.Run();
    }
}

internal sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "WinForms Application Builder";
        ClientSize = new Size(520, 170);

        Label description = new()
        {
            AutoSize = true,
            Location = new Point(16, 20),
            Text = "A hosted background service writes a heartbeat each second."
        };

        Label shutdownDescription = new()
        {
            AutoSize = true,
            Location = new Point(16, 50),
            Text = "Close this window to cancel the service and stop the host gracefully."
        };

        Button closeButton = new()
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Location = new Point(410, 115),
            Text = "Close"
        };
        closeButton.Click += (_, _) => Close();

        Controls.Add(description);
        Controls.Add(shutdownDescription);
        Controls.Add(closeButton);
    }
}

internal sealed class MainApplicationContext : ApplicationContext
{
    public MainApplicationContext()
        : base(new MainForm())
    {
    }
}

internal sealed class HeartbeatService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
            {
                Debug.WriteLine($"Background service heartbeat at {DateTimeOffset.Now}.");
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            Debug.WriteLine("Background service observed host shutdown.");
        }
    }
}
