﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private sealed class MethodLine : Line
        {
            private DesignerActionList _actionList;
            private DesignerActionMethodItem _methodItem;
            private MethodItemLinkLabel _linkLabel;
            public MethodLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            public sealed override string FocusId
            {
                get => "METHOD:" + _actionList.GetType().FullName + "." + _methodItem.MemberName;
            }

            protected override void AddControls(List<Control> controls)
            {
                _linkLabel = new MethodItemLinkLabel
                {
                    ActiveLinkColor = ActionPanel.ActiveLinkColor,
                    AutoSize = false,
                    BackColor = Color.Transparent,
                    LinkBehavior = LinkBehavior.HoverUnderline,
                    LinkColor = ActionPanel.LinkColor,
                    TextAlign = ContentAlignment.MiddleLeft,
                    UseMnemonic = false,
                    VisitedLinkColor = ActionPanel.LinkColor
                };
                _linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(OnLinkLabelLinkClicked);
                controls.Add(_linkLabel);
            }

            public sealed override void Focus()
            {
                _linkLabel.Focus();
            }

            public override Size LayoutControls(int top, int width, bool measureOnly)
            {
                Size linkLabelSize = _linkLabel.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
                if (!measureOnly)
                {
                    _linkLabel.Location = new Point(LineLeftMargin, top + LineVerticalPadding / 2);
                    _linkLabel.Size = linkLabelSize;
                }

                return linkLabelSize + new Size(LineLeftMargin + LineRightMargin, LineVerticalPadding);
            }

            private void OnLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            {
                Debug.Assert(!ActionPanel.InMethodInvoke, "Nested method invocation");
                ActionPanel.InMethodInvoke = true;
                try
                {
                    _methodItem.Invoke();
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                    {
                        ex = ex.InnerException;
                    }

                    //NOTE: We had code to rethrow if this was one of [NullReferenceException, StackOverflowException, OutOfMemoryException,
                    //ThreadAbortException].  Removing this rethrow.  StackOverflow and ThreadAbort can't be meaningfully caught, and
                    //NullRef and OutOfMemory really shouldn't be caught.  Out of these, OOM is the most correct one to call, but OOM is
                    //thrown by GDI+ for pretty much any problem, so isn't reliable as an actual indicator that you're out of memory.  If
                    //you really are out of memory, it's very likely you'll get another OOM shortly.
                    ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_ErrorInvokingAction, _methodItem.DisplayName, Environment.NewLine + ex.Message));
                }
                finally
                {
                    ActionPanel.InMethodInvoke = false;
                }
            }

            internal override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
                _actionList = actionList;
                _methodItem = (DesignerActionMethodItem)actionItem;
                toolTip.SetToolTip(_linkLabel, _methodItem.Description);
                _linkLabel.Text = StripAmpersands(_methodItem.DisplayName);
                _linkLabel.AccessibleDescription = actionItem.Description;
                _linkLabel.TabIndex = currentTabIndex++;
            }

            private sealed class MethodItemLinkLabel : LinkLabel
            {
                protected override bool ProcessDialogKey(Keys keyData)
                {
                    if ((keyData & Keys.Control) == Keys.Control)
                    {
                        Keys keyCode = keyData & Keys.KeyCode;
                        switch (keyCode)
                        {
                            case Keys.Tab:
                                // We specifically ignore Ctrl+Tab because it prevents the window switcher dialog from showing up in VS. Normally the key combination is only needed when a LinkLabel contains multiple links, but that can't happen inside the DesignerActionPanel.
                                return false;
                        }
                    }

                    return base.ProcessDialogKey(keyData);
                }
            }
        }
    }
}
