// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private abstract class Line
        {
            private readonly DesignerActionPanel _actionPanel;
            private List<Control> _addedControls;
            private readonly IServiceProvider _serviceProvider;

            public Line(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            {
                _serviceProvider = serviceProvider;
                _actionPanel = actionPanel.OrThrowIfNull();
            }

            protected DesignerActionPanel ActionPanel
            {
                get => _actionPanel;
            }

            public abstract string FocusId
            {
                get;
            }

            protected IServiceProvider ServiceProvider
            {
                get => _serviceProvider;
            }

            protected abstract void AddControls(List<Control> controls);

            internal List<Control> GetControls()
            {
                _addedControls = new List<Control>();
                AddControls(_addedControls);
                // Tag all the controls with the Line so we know who owns it
                foreach (Control c in _addedControls)
                {
                    c.Tag = this;
                }

                return _addedControls;
            }

            public abstract void Focus();

            public abstract Size LayoutControls(int top, int width, bool measureOnly);

            public virtual void PaintLine(Graphics g, int lineWidth, int lineHeight)
            {
            }

            protected internal virtual bool ProcessDialogKey(Keys keyData) => false;

            internal void RemoveControls(ControlCollection controls)
            {
                for (int i = 0; i < _addedControls.Count; i++)
                {
                    Control c = _addedControls[i];
                    c.Tag = null;
                    controls.Remove(c);
                }
            }

            internal abstract void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex);
        }
    }
}
