// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    internal partial class DesignBindingValueUIHandler
    {
        class LocalUIItem : PropertyValueUIItem
        {
            readonly Binding binding;

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

            static string GetToolTip(Binding binding)
            {
                string name = "";
                if (binding.DataSource is IComponent comp)
                {
                    if (comp.Site != null)
                    {
                        name = comp.Site.Name;
                    }
                }

                if (name.Length == 0)
                {
                    name = "(List)";
                }

                name += " - " + binding.BindingMemberInfo.BindingMember;
                return name;
            }
        }
    }
}
