// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using WinFormsControlsTest;

// Set STAThread
Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
ApplicationConfiguration.Initialize();

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
Application.SetColorMode(SystemColorMode.Classic);
#pragma warning restore WFO5001

Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

try
{
    MainForm form = new()
    {
        Icon = SystemIcons.GetStockIcon(StockIconId.Shield, StockIconOptions.SmallIcon)
    };

    Application.Run(form);
}
catch (Exception)
{
    Environment.Exit(-1);
}

Environment.Exit(0);
