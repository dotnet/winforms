// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NativeHost
{
    [ComVisible(true)]
    [Guid("54479E5D-EABC-448C-A767-EAFF17BC28C9")]
    [ComDefaultInterface(typeof(IManagedControl))]
    public partial class ManagedControl : UserControl, IManagedControl
    {
        public ManagedControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello world from .NET");
        }
    }

    [ComVisible(true)]
    [Guid("3223D73E-286A-462C-AF8D-392D472673BF")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IManagedControl { }
}
