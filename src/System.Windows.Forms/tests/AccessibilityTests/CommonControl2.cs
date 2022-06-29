﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace Accessibility_Core_App
{
    public partial class CommonControl2 : Form
    {
        public CommonControl2()
        {
            InitializeComponent();
            
            string executable = Environment.ProcessPath;
            string executablePath = Path.GetDirectoryName(executable);
            var page = Path.Combine(executablePath, "HTMLPage1.html");
            this.webBrowser1.Url = new System.Uri($"file://{page}", System.UriKind.Absolute);
        }
    }
}
