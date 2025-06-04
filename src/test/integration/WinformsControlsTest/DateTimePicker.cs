// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class DateTimePicker : Form
{
    public DateTimePicker()
    {
        InitializeComponent();
        dateTimePicker5.CustomFormat = "yyyy-MM-dd HH:mm:ss";
    }
}
