// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Microsoft.DotNet.RemoteExecutor;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ParkingWindowTests
    {
        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void ParkingWindow_DoesNotThrowOnGarbageCollecting()
        {
            using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
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
                    Assert.True(ex is null, "Expected no exception, but got: " + ex.Message); // Actually need to check whether GC.Collect() does not throw exception.
                }
            });

            // verify the remote process succeeded
            Assert.Equal(0, invokerHandle.ExitCode);
        }

        private Form InitFormWithControlToGarbageCollect()
        {
            Form form = new Form();
            ComboBox comboBox = new ComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;

            form.Controls.Add(comboBox);
            form.Show();

            // Park combobox handle in ParkingWindow.
            comboBox.Parent = null;

            // Recreate comobox handle to set parent to ParkingWindow.
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Lose the reference to combobox to allow Garbage collecting combobox.
            comboBox = null;

            return form;
        }
    }
}
