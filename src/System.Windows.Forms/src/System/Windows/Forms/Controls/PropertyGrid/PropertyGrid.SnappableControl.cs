// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class PropertyGrid
{
    internal abstract class SnappableControl : Control
    {
        protected PropertyGrid OwnerPropertyGrid { get; }
        internal bool UserSized { get; set; }

        public abstract int GetOptimalHeight(int width);
        public abstract int SnapHeightRequest(int newHeight);

        public SnappableControl(PropertyGrid ownerPropertyGrid)
        {
            OwnerPropertyGrid = ownerPropertyGrid;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        [AllowNull]
        public override Cursor Cursor
        {
            get => Cursors.Default;
            set => base.Cursor = value;
        }

        protected override void OnControlAdded(ControlEventArgs ce)
        {
        }

        public Color BorderColor { get; set; } = Application.SystemColors.ControlDark;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle r = ClientRectangle;
            r.Width--;
            r.Height--;

            using var borderPen = BorderColor.GetCachedPenScope();
            e.Graphics.DrawRectangle(borderPen, r);
        }
    }
}
