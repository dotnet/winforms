﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinformsControlsTest;

public partial class Password : Form
{
    public Password()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        textBox3.UseSystemPasswordChar = !textBox3.UseSystemPasswordChar;
    }
}
