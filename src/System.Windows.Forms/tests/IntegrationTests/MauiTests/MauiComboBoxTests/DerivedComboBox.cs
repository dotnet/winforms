// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class DerivedComboBox : ComboBox
    {
        public DerivedComboBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
            FormattingEnabled = true;
        }

        public List<MeasureItemEventArgs> MeasureItemEventArgs { get; } = new List<MeasureItemEventArgs>();

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            MeasureItemEventArgs.Add(e);
        }
    }
}
