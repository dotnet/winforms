// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class RichTextBoxes : Form
    {
        public RichTextBoxes()
        {
            InitializeComponent();

            richTextBox1.LoadFile(File.OpenRead(@"Data\example.rtf"), RichTextBoxStreamType.RichText);
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox2.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang4105{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
{\*\generator Riched20 10.0.17134}\viewkind4\uc1 
{\field{\*\fldinst { HYPERLINK ""http://www.google.com"" }}{\fldrslt {Click here}}}
\pard\sa200\sl276\slmult1\f0\fs22\lang9  for more information.\par
}";
        }
      
        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            MessageBox.Show(this, e.LinkText, "link clicked");
        }

        private void richTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            MessageBox.Show(this, e.LinkText, "link clicked");
        }
    }
}
