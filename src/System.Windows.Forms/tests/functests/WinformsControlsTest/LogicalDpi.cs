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
        private Font drawingFont;

        public UserPaintControl()
        {
            this.LogicalDpiScaling = true;
            // Font needs to be set to a pixel size to avoid scaling issues
            // We can assume 96dpi here if logical dpi scaling is enabled
            this.drawingFont = new Font("Arial", (int)(11f * 96 / 72), GraphicsUnit.Pixel);
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
            e.ApplyLogicalToDeviceScaling();

            Graphics g = e.Graphics;
            using (SolidBrush redBrush = new SolidBrush(Color.Red))
            {
                using (Pen bluePen = new Pen(Color.Blue))
                {
                    g.DrawRectangle(bluePen, 0, 0, this.LogicalWidth - 1, this.LogicalHeight - 1);
                    g.DrawString("Current dpi: " + this.CurrentDpi, drawingFont, redBrush, 0, 0);
                    g.DrawString("Mouse: " + mousePosition.ToString(), drawingFont, redBrush, 0, 20);

                    g.FillRectangle(redBrush, this.LogicalWidth - 51, this.LogicalHeight - 51, 50, 50);
                }
                 
            }    
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mousePosition = e.LogicalLocation;
            Invalidate();
        }
    }

    public partial class LogicalDpi : Form
    {
        public LogicalDpi()
        {
            InitializeComponent();

            // Set the font here in pixel size as a baseline for further scaling
            this.Font = new Font(this.Font.FontFamily, this.LogicalToDeviceUnits((int)(8.25f * 96 / 72)), this.Font.Style, GraphicsUnit.Pixel);

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
            control.LogicalSize = new System.Drawing.Size(200, 200);
            Controls.Add(control);
        }
    }
}
