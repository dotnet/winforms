using System.Drawing;
using System.Windows.Forms;

namespace TestConsole;

public partial class MainForm
{
    internal class MyScrollableControl : ScrollableControl
    {
        internal void InjectControl(MyUserControl control)
        {
            Controls.Add(control);
        }

        internal MyScrollableControl()
        {
            BackColor = Color.Red;
            AutoScrollMinSize = new Size(800, 800);
            DoubleBuffered = true;
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.Clear(BackColor);
        }
    }
}
