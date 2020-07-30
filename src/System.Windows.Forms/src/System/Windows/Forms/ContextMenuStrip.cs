// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class is just a conceptual wrapper around ToolStripDropDownMenu.
    /// </summary>
    [DefaultEvent(nameof(Opening))]
    [SRDescription(nameof(SR.DescriptionContextMenuStrip))]
    public class ContextMenuStrip : ToolStripDropDownMenu
    {
        public ContextMenuStrip(IContainer container) : base()
        {
            // this constructor ensures ContextMenuStrip is disposed properly since its not parented to the form.
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            container.Add(this);
        }

        public ContextMenuStrip()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ContextMenuStripSourceControlDescr))]
        public Control SourceControl
        {
            get
            {
                return SourceControlInternal;
            }
        }

        // minimal Clone implementation for DGV support only.
        internal ContextMenuStrip Clone()
        {
            // VERY limited support for cloning.

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            // copy over events
            contextMenuStrip.Events.AddHandlers(Events);

            contextMenuStrip.AutoClose = AutoClose;
            contextMenuStrip.AutoSize = AutoSize;
            contextMenuStrip.Bounds = Bounds;
            contextMenuStrip.ImageList = ImageList;
            contextMenuStrip.ShowCheckMargin = ShowCheckMargin;
            contextMenuStrip.ShowImageMargin = ShowImageMargin;

            // copy over relevant properties

            for (int i = 0; i < Items.Count; i++)
            {
                ToolStripItem item = Items[i];

                if (item is ToolStripSeparator)
                {
                    contextMenuStrip.Items.Add(new ToolStripSeparator());
                }
                else if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                    contextMenuStrip.Items.Add(menuItem.Clone());
                }
            }
            return contextMenuStrip;
        }

        // internal overload so we know whether or not to show mnemonics.
        internal void ShowInternal(Control source, Point location, bool isKeyboardActivated)
        {
            Show(source, location);

            // if we were activated by keyboard - show mnemonics.
            if (isKeyboardActivated)
            {
                ToolStripManager.ModalMenuFilter.Instance.ShowUnderlines = true;
            }
        }

        internal void ShowInTaskbar(int x, int y)
        {
            // we need to make ourselves a topmost window
            WorkingAreaConstrained = false;
            Rectangle bounds = CalculateDropDownLocation(new Point(x, y), ToolStripDropDownDirection.AboveLeft);
            Rectangle screenBounds = Screen.FromRectangle(bounds).Bounds;
            if (bounds.Y < screenBounds.Y)
            {
                bounds = CalculateDropDownLocation(new Point(x, y), ToolStripDropDownDirection.BelowLeft);
            }
            else if (bounds.X < screenBounds.X)
            {
                bounds = CalculateDropDownLocation(new Point(x, y), ToolStripDropDownDirection.AboveRight);
            }
            bounds = WindowsFormsUtils.ConstrainToBounds(screenBounds, bounds);

            Show(bounds.X, bounds.Y);
        }

        protected override void SetVisibleCore(bool visible)
        {
            if (!visible)
            {
                WorkingAreaConstrained = true;
            }
            base.SetVisibleCore(visible);
        }
    }
}
