// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;

/// <summary>
/// Summary description for DerivedGridView.
/// </summary>
/// 
//[System.ComponentModel.DesignerCategory("code")]

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class DerivedDataGridView : DataGridView
    {
        public DerivedDataGridView()
        {
            this.DataError += new DataGridViewDataErrorEventHandler(DerivedDataGridView_DataError);
        }


        void DerivedDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            throw new DataGridViewException("Unexpected exception", e.Exception);
        }
    }
}
