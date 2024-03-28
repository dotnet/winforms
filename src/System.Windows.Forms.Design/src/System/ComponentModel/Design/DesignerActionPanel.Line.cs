// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private abstract class Line
    {
        protected readonly List<Control> AddedControls = [];

        protected Line(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
        {
            ServiceProvider = serviceProvider;
            ActionPanel = actionPanel.OrThrowIfNull();
        }

        protected DesignerActionPanel ActionPanel { get; }

        public abstract string FocusId
        {
            get;
        }

        protected IServiceProvider ServiceProvider { get; }

        internal List<Control> GetControls()
        {
            // Tag all the controls with the Line so we know who owns it
            foreach (Control c in AddedControls)
            {
                c.Tag = this;
            }

            return AddedControls;
        }

        public abstract void Focus();

        public abstract Size LayoutControls(int top, int width, bool measureOnly);

        public virtual void PaintLine(Graphics g, int lineWidth, int lineHeight)
        {
        }

        protected internal virtual bool ProcessDialogKey(Keys keyData) => false;

        internal void RemoveControls(ControlCollection controls)
        {
            foreach (Control c in AddedControls)
            {
                c.Tag = null;
                controls.Remove(c);
            }
        }

        internal abstract void UpdateActionItem(LineInfo lineInfo, ToolTip toolTip, ref int currentTabIndex);
    }
}
