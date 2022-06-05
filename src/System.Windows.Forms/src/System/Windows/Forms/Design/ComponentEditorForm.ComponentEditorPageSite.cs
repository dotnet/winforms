// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace System.Windows.Forms.Design
{
    public partial class ComponentEditorForm
    {
        /// <summary>
        ///  Implements a standard version of ComponentEditorPageSite for use within a
        ///  ComponentEditorForm.
        /// </summary>
        private sealed class ComponentEditorPageSite : IComponentEditorPageSite
        {
            internal IComponent component;
            internal ComponentEditorPage pageControl;
            internal Control parent;
            internal bool isActive;
            internal bool isDirty;
            private readonly ComponentEditorForm form;

            /// <summary>
            ///  Creates the page site.
            /// </summary>
            internal ComponentEditorPageSite(Control parent, Type pageClass, IComponent component, ComponentEditorForm form)
            {
                this.component = component;
                this.parent = parent;
                isActive = false;
                isDirty = false;

                this.form = form.OrThrowIfNull();

                try
                {
                    pageControl = (ComponentEditorPage)Activator.CreateInstance(pageClass)!;
                }
                catch (TargetInvocationException e)
                {
                    Debug.Fail(e.ToString());
                    throw new TargetInvocationException(string.Format(SR.ExceptionCreatingCompEditorControl, e.ToString()), e.InnerException);
                }

                pageControl.SetSite(this);
                pageControl.SetComponent(component);
            }

            /// <summary>
            ///  Called by the ComponentEditorForm to activate / deactivate the page.
            /// </summary>
            internal bool Active
            {
                set
                {
                    if (value)
                    {
                        // make sure the page has been created
                        pageControl.CreateControl();

                        // activate it and give it focus
                        pageControl.Activate();
                    }
                    else
                    {
                        pageControl.Deactivate();
                    }

                    isActive = value;
                }
            }

            internal bool AutoCommit
            {
                get
                {
                    return pageControl.CommitOnDeactivate;
                }
            }

            internal bool Dirty
            {
                get
                {
                    return isDirty;
                }
                set
                {
                    isDirty = value;
                }
            }

            /// <summary>
            ///  Called by a page to return a parenting control for itself.
            /// </summary>
            public Control GetControl()
            {
                return parent;
            }

            /// <summary>
            ///  Called by the ComponentEditorForm to get the actual page.
            /// </summary>
            internal ComponentEditorPage GetPageControl()
            {
                return pageControl;
            }

            /// <summary>
            ///  Called by a page to mark it's contents as dirty.
            /// </summary>
            public void SetDirty()
            {
                if (isActive)
                {
                    Dirty = true;
                }

                form.SetDirty();
            }
        }
    }
}
