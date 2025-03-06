// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.IO;

namespace AxHosts;

[DesignerCategory("Default")]
public partial class AxHosts : Form
{
    public AxHosts()
    {
        InitializeComponent();
        axWindowsMediaPlayer1.URL = Path.GetFullPath(@"./Resources/media.mpg");
    }
}
