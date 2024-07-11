// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
internal class FormOwnerTestForm : Form
{
    public FormOwnerTestForm()
    {
        Shown += (object sender, EventArgs e) => RunFormOwnerMemoryLeakTest();
    }

    /// <summary>
    /// This method runs a loop to open and close a parent form.
    /// The parent form opens a child form.show(IWin32Window) overload.
    /// The parent form is then closed, which also closes the child.
    /// The output shows how much memory was used in the process and
    /// can be used to check if a memory leak is present.
    /// This is used to test the bug: https://github.com/dotnet/winforms/issues/530
    /// </summary>
    private void RunFormOwnerMemoryLeakTest()
    {
        long memoryStart = GC.GetTotalMemory(false);

        List<Form> childForms = [];
        for (int i = 0; i < 500; i++)
        {
            MemoryTestParentForm parent = new();
            parent.Show();
            childForms.Add(parent.CreateChildFormWithMemory());
            parent.Close();
        }

        childForms.ForEach(childForm => childForm.Close());
        childForms.Clear();

        long memoryBeforeGC = GC.GetTotalMemory(false);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        long memoryAfterGC = GC.GetTotalMemory(true);

        Controls.Add(new Label()
        {
            Text = $"Memory Usage:\nBefore: {memoryStart:N0} bytes\nDuring: {memoryBeforeGC:N0} bytes\nAfter: {memoryAfterGC:N0} bytes",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        });
    }

    public class MemoryTestParentForm : Form
    {
        private byte[] _array;

        public Form CreateChildFormWithMemory()
        {
            // Create a byte array to consume memory on the parent so the leak is more obvious.
            _array = new byte[1024 * 1024];
            Array.Clear(_array, 0, _array.Length);
            Form child = new();

            // Show the child and pass the parent as the owner, this is where the leak can happen.
            child.Show(this);
            return child;
        }
    }
}
