using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    internal class FormOwnerTestForm : Form
    {
        public FormOwnerTestForm()
        {
            this.Shown += (object sender, EventArgs e) => RunFormOwnerMemoryLeakTest();
        }

        private void RunFormOwnerMemoryLeakTest()
        {
            long memoryStart = Process.GetCurrentProcess().WorkingSet64;

            List<Form> childForms = new();
            for (int i = 0; i < 500; i++)
            {
                MemoryTestParentForm parent = new();
                parent.Show();
                childForms.Add(parent.CreateChildFormWithMemory());
                parent.Close();
            }

            childForms.ForEach(childForm => childForm.Close());
            childForms.Clear();

            long memoryBeforeGC = Process.GetCurrentProcess().WorkingSet64;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long memoryAfterGC = Process.GetCurrentProcess().WorkingSet64;

            Controls.Add(new Label()
            {
                Text = $"Memory Usage:\nBefore: {memoryStart:N0} bytes\nDuring: {memoryBeforeGC:N0} bytes\nAfter: {memoryAfterGC:N0} bytes",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
        }

        public class MemoryTestParentForm : Form
        {
            private byte[] array;

            public Form CreateChildFormWithMemory()
            {
                // Create a byte array to consume memory on the parent so the leak is more obvious.
                this.array = new byte[1024 * 1024];
                Array.Clear(array, 0, this.array.Length);
                Form child = new();

                // Show the child and pass the parent as the owner, this is where the leak can happen.
                child.Show(this);
                return child;
            }
        }
    }
}
