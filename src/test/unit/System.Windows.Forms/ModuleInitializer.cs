// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Runtime.CompilerServices;

namespace System.Windows.Forms.Tests;

public class ModuleInitializer
{
    [ModuleInitializer]
    public static void InitializeModule()
    {
        // We do not want thread exceptions swallowed during test runs, or worse, popping the exception dialog,
        // which can hang the test run. Setting this switch will force throwing of exceptions that happen during
        // handling of a Windows message.
        AppContext.SetSwitch("System.Windows.Forms.DoNotCatchUnhandledExceptions", isEnabled: true);

        Application.EnableVisualStyles();
    }
}
