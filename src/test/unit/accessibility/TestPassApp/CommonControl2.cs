// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace TestPassApp;

public partial class CommonControl2 : Form
{
    public CommonControl2()
    {
        InitializeComponent();

        string executable = Environment.ProcessPath;
        string executablePath = Path.GetDirectoryName(executable);
        string page = Path.Join(executablePath, "HTMLPage1.html");
        webBrowser1.Url = new Uri($"file://{page}", UriKind.Absolute);
    }
}
