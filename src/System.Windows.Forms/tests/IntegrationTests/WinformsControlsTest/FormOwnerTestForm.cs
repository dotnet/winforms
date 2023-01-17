using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    internal class FormOwnerTestForm : Form
    {
        public FormOwnerTestForm()
        {
            this.Shown += (object sender, EventArgs e) => ShowMemoryTest();
        }

        private void ShowMemoryTest()
        {
            long a = Process.GetCurrentProcess().WorkingSet64;

            var children = new List<Form>();
            for (int i = 0; i < 500; i++)
            {
                MemoryTestParentForm parent = new MemoryTestParentForm();
                parent.Show();
                children.Add(parent.Yum());
                parent.Close();
            }

            children.ForEach(cf => cf.Close());
            children.Clear();

            long b = Process.GetCurrentProcess().WorkingSet64;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long c = Process.GetCurrentProcess().WorkingSet64;

            Controls.Add(new Label()
            {
                Text = $"Memory Usage:\nBefore: {a:N0} bytes\nDuring: {b:N0} bytes\nAfter: {c:N0} bytes",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
        }

        public class MemoryTestParentForm : Form
        {
            private byte[] array;

            public Form Yum()
            {
                this.array = new byte[1024 * 1024];
                Array.Clear(array, 0, this.array.Length);

                Form child = new Form();
                child.Show(this);

                return child;
            }
        }
    }
}
