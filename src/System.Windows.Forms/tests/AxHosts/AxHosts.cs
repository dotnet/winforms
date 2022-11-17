// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.IO;
using AxWMPLib;

namespace AxHosts
{
    public partial class AxHosts : Form
    {
        private AxWindowsMediaPlayer _mediaPlayer;

        public AxHosts()
        {
            InitializeComponent();
            _mediaPlayer = new()
            {
                Dock = DockStyle.Fill,
            };

            ((ISupportInitialize)_mediaPlayer).BeginInit();
            Controls.Add(_mediaPlayer);
            ((ISupportInitialize)_mediaPlayer).EndInit();
            _mediaPlayer.URL = Path.GetFullPath(@".\resources\media.mpg");
        }
    }
}
