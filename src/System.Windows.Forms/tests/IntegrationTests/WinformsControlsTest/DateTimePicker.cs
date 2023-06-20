// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest;

public partial class DateTimePicker : Form
{
    public DateTimePicker()
    {
        InitializeComponent();
        this.dateTimePicker5.CustomFormat = "yyyy-MM-dd HH:mm:ss";
    }
}
