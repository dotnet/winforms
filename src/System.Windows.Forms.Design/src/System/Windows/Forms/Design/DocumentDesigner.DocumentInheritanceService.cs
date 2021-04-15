// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace System.Windows.Forms.Design
{
    public partial class DocumentDesigner
    {
        /// <devdoc>
        ///      Document designer's version of the inheritance service.  For UI
        ///      components, we will allow private controls if those controls are
        ///      children of our document, since they will be visible.
        /// </devdoc>
        private class DocumentInheritanceService : InheritanceService
        {
            private DocumentDesigner designer;

            /// <include file='doc\DocumentDesigner.uex' path='docs/doc[@for="DocumentDesigner.DocumentInheritanceService.DocumentInheritanceService"]/*' />
            /// <devdoc>
            ///      Creates a new document inheritance service.
            /// </devdoc>
            public DocumentInheritanceService(DocumentDesigner designer)
            {
                this.designer = designer;
            }

            /// <include file='doc\DocumentDesigner.uex' path='docs/doc[@for="DocumentDesigner.DocumentInheritanceService.IgnoreInheritedMember"]/*' />
            /// <devdoc>
            ///    <para>Indicates the inherited members to ignore.</para>
            /// </devdoc>
            protected override bool IgnoreInheritedMember(MemberInfo member, IComponent component)
            {

                // We allow private members if they are controls on our design surface or
                // derive from Menu.
                //
                bool privateMember = false;
                Type memberType = null;

                FieldInfo field = member as FieldInfo;
                MethodInfo method = member as MethodInfo;
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
                    else if (typeof(Menu).IsAssignableFrom(memberType))
                    {
                        object menu = null;
                        if (field != null)
                        {
                            menu = field.GetValue(component);
                        }
                        else if (method != null)
                        {
                            menu = method.Invoke(component, null);
                        }
                        if (menu != null)
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
