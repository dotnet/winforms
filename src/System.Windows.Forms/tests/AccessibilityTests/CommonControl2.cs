// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class CommonControl2 : Form
{
    public CommonControl2()
    {
        InitializeComponent();

        string executable = Environment.ProcessPath;
        string executablePath = Path.GetDirectoryName(executable);
        string page = Path.Combine(executablePath, "HTMLPage1.html");
        webBrowser1.Url = new Uri($"file://{page}", UriKind.Absolute);
    }
}
