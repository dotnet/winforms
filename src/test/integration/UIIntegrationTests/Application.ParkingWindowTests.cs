// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.UITests;

// Migrated from unit tests; see issue #4500.
public class ParkingWindowTests : ControlTestBase
{
    public ParkingWindowTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public void ParkingWindow_DoesNotThrowOnGarbageCollecting()
    {
        bool original = Control.CheckForIllegalCrossThreadCalls;
        Control.CheckForIllegalCrossThreadCalls = true;

        try
        {
            using Form form = InitFormWithControlToGarbageCollect();

            // Access ComboBox from the GC thread; test passes if this does not throw.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        finally
        {
            Control.CheckForIllegalCrossThreadCalls = original;
        }
    }

    private static Form InitFormWithControlToGarbageCollect()
    {
        Form form = new();
        ComboBox comboBox = new()
        {
            DropDownStyle = ComboBoxStyle.DropDown
        };

        form.Controls.Add(comboBox);
        form.Show();

        // Park ComboBox handle in ParkingWindow.
        comboBox.Parent = null;

        // Recreate ComboBox handle to set parent to ParkingWindow.
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        return form;
    }
}
