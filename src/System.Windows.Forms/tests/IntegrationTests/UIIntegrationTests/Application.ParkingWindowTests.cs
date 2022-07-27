// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ParkingWindowTests : ControlTestBase
    {
        public ParkingWindowTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public void ParkingWindow_DoesNotThrowOnGarbageCollecting()
        {
            Control.CheckForIllegalCrossThreadCalls = true;

            Form form = InitFormWithControlToGarbageCollect();

            try
            {
                // Force garbage collecting to access combobox from another (GC) thread.
                GC.Collect();

                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Assert.True(ex is null, $"Expected no exception, but got: {ex?.Message}"); // Actually need to check whether GC.Collect() does not throw exception.
            }
        }

        private Form InitFormWithControlToGarbageCollect()
        {
            Form form = new Form();
            ComboBox? comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDown
            };

            form.Controls.Add(comboBox);
            form.Show();

            // Park combobox handle in ParkingWindow.
            comboBox.Parent = null;

            // Recreate combobox handle to set parent to ParkingWindow.
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Lose the reference to combobox to allow Garbage collecting combobox.
            comboBox = null;

            return form;
        }
    }
}
