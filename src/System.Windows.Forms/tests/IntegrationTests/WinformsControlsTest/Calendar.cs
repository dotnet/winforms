// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class Calendar : Form
    {
        public Calendar()
        {
            InitializeComponent();
            monthCalendar1.Size = new Size(2000, 2000);
            monthCalendar1.MinDate = new DateTime(2019, 12, 20);
        }
    }
}
