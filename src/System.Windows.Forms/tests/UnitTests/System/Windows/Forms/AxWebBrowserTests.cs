// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using AxSHDocVw;

namespace System.Windows.Forms.Tests.System.Windows.Forms;
public class AxMicrosoftWebBrowserTests
{
    private readonly Form _form;
    private readonly AxMicrosoftWebBrowserTests _control;

   public AxMicrosoftWebBrowserTests()
    {
        using Form form = new();
        using AxWebBrowser control = new();
        ((ISupportInitialize)control).BeginInit();
        form.Controls.Add(control);
        ((ISupportInitialize)control).EndInit();
    } 
}
