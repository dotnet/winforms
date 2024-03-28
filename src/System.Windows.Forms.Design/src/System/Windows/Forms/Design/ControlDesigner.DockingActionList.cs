// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

public partial class ControlDesigner
{
    private class DockingActionList : DesignerActionList
    {
        private readonly IDesignerHost? _host;

        public DockingActionList(ControlDesigner owner)
            : base(owner.Component)
        {
            _host = GetService(typeof(IDesignerHost)) as IDesignerHost;
        }

        private string? GetActionName()
        {
            if (Component is null)
            {
                return null;
            }

            PropertyDescriptor? dockProp = TypeDescriptor.GetProperties(Component)["Dock"];
            if (dockProp is not null)
            {
                DockStyle dockStyle = (DockStyle)dockProp.GetValue(Component)!;
                if (dockStyle == DockStyle.Fill)
                {
                    return SR.DesignerShortcutUndockInParent;
                }
                else
                {
                    return SR.DesignerShortcutDockInParent;
                }
            }

            return null;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = [];
            string? actionName = GetActionName();
            if (actionName is not null)
            {
                items.Add(new DesignerActionVerbItem(new DesignerVerb(actionName, OnDockActionClick)));
            }

            return items;
        }

        private void OnDockActionClick(object? sender, EventArgs e)
        {
            if (sender is DesignerVerb designerVerb && _host is not null)
            {
                using DesignerTransaction t = _host.CreateTransaction(designerVerb.Text);

                // Set the dock prop to DockStyle.Fill
                PropertyDescriptor dockProp = TypeDescriptor.GetProperties(Component!)["Dock"]!;
                DockStyle dockStyle = (DockStyle)dockProp.GetValue(Component)!;
                dockProp.SetValue(Component, dockStyle == DockStyle.Fill ? DockStyle.None : DockStyle.Fill);
                t.Commit();
            }
        }
    }
}
