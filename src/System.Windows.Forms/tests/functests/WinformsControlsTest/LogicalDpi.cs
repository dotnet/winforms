using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public class UserPaintControl : Control
    {
        private Point mousePosition;

        public UserPaintControl()
        {
            this.LogicalDpiScaling = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            Graphics g = pevent.ApplyLogicalToDeviceScaling();
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(whiteBrush, pevent.LogicalClipRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Apply graphics scaling 
            Graphics g = e.ApplyLogicalToDeviceScaling();

            using (SolidBrush redBrush = new SolidBrush(Color.Red))
            {
                using (Pen bluePen = new Pen(Color.Blue))
                {
                    g.DrawRectangle(bluePen, 0, 0, this.LogicalWidth - 1, this.LogicalHeight - 1);
                    g.DrawString("Current dpi: " + this.CurrentDpi, this.FontWithLogicalSize, redBrush, 0, 0);
                    g.DrawString("Mouse: " + mousePosition.ToString(), this.FontWithLogicalSize, redBrush, 0, 20);

                    g.FillRectangle(redBrush, this.LogicalWidth - 51, this.LogicalHeight - 51, 50, 50);
                }
                 
            }    
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mousePosition = e.LogicalLocation;

            // Invalidate only the rectangle containing the mouse coordinates
            InvalidateLogicalRect(new Rectangle(0, 20, this.LogicalWidth, 20));
        }

        public override bool GetPreferredLogicalSize(Size proposedSize, out Size preferredSize)
        {
            preferredSize = new Size(200, 200);
            return true;
        }
    }

    public partial class LogicalDpi : Form
    {
        public LogicalDpi()
        {
            // Set the font here based on the default font
            this.FontWithLogicalSize = this.Font;

            InitializeComponent();

            // Add some buttons
            Button b;
            for (int i = 0; i < 3; i++)
            {
                b = new Button();
                b.LogicalDpiScaling = true;
                b.LogicalLocation = new System.Drawing.Point(20 + i * 70, 20);
                b.AutoSize = false;
                b.LogicalSize = new System.Drawing.Size(60, 23);
                b.Text = "Button " + (i + 1).ToString();
                Controls.Add(b);
            }

            // Control with custom drawing
            UserPaintControl control = new UserPaintControl();
            control.LogicalLocation = new Point(20, 50);
            control.AutoSize = true;
            Controls.Add(control);
        }
    }
}
