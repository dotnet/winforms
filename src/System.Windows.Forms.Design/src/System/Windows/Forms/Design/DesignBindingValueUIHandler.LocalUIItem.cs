// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal partial class DesignBindingValueUIHandler
{
    private class LocalUIItem : PropertyValueUIItem
    {
        private readonly Binding binding;

        internal LocalUIItem(DesignBindingValueUIHandler handler, Binding binding) : base(handler.DataBitmap, new PropertyValueUIItemInvokeHandler(OnPropertyValueUIItemInvoke), GetToolTip(binding))
        {
            this.binding = binding;
        }

        internal Binding Binding
        {
            get
            {
                return binding;
            }
        }

        private static string GetToolTip(Binding binding)
        {
            string name = "";
            if (binding.DataSource is IComponent comp)
            {
                if (comp.Site is not null)
                {
                    name = comp.Site.Name;
                }
            }

            if (name.Length == 0)
            {
                name = "(List)";
            }

            return $"{name} - {binding.BindingMemberInfo.BindingMember}";
        }
    }
}
