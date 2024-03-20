// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class MediaPlayer : Form
{
    public MediaPlayer()
    {
        InitializeComponent();
        axWindowsMediaPlayer1.URL = Path.GetFullPath(@".\resources\media.mpg");
    }
}
