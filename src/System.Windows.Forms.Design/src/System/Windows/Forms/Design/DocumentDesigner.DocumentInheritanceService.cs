// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;

namespace System.Windows.Forms.Design
{
    public partial class DocumentDesigner
    {
        /// <summary>
        ///  Document designer's version of the inheritance service.  For UI
        ///  components, we will allow private controls if those controls are
        ///  children of our document, since they will be visible.
        /// </summary>
        private class DocumentInheritanceService : InheritanceService
        {
            private readonly DocumentDesigner designer;

            /// <summary>
            ///  Creates a new document inheritance service.
            /// </summary>
            public DocumentInheritanceService(DocumentDesigner designer)
            {
                this.designer = designer;
            }

            /// <summary>
            ///  <para>Indicates the inherited members to ignore.</para>
            /// </summary>
            protected override bool IgnoreInheritedMember(MemberInfo member, IComponent component)
            {
                FieldInfo field = member as FieldInfo;
                MethodInfo method = member as MethodInfo;
                // We allow private members if they are controls on our design surface or
                // derive from Menu.
                //
                bool privateMember;
                Type memberType;
                if (field != null)
                {
                    privateMember = field.IsPrivate || field.IsAssembly;
                    memberType = field.FieldType;
                }
                else if (method != null)
                {
                    privateMember = method.IsPrivate || method.IsAssembly;
                    memberType = method.ReturnType;
                }
                else
                {
                    Debug.Fail("Unknown member type passed to IgnoreInheritedMember");
                    return true;
                }

                if (privateMember)
                {
                    if (typeof(Control).IsAssignableFrom(memberType))
                    {
                        // See if this member is a child of our document...
                        //
                        Control child = null;
                        if (field != null)
                        {
                            child = (Control)field.GetValue(component);
                        }
                        else if (method != null)
                        {
                            child = (Control)method.Invoke(component, null);
                        }

                        Control parent = designer.Control;

                        while (child != null && child != parent)
                        {
                            child = child.Parent;
                        }

                        // If it is a child of our designer, we don't want to ignore this member.
                        //
                        if (child != null)
                        {
                            return false;
                        }
                    }
                }

                return base.IgnoreInheritedMember(member, component);
            }
        }
    }
}
