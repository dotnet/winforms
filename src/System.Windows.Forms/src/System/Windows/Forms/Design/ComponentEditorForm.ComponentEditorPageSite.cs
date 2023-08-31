// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.Design;

public partial class ComponentEditorForm
{
    /// <summary>
    ///  Implements a standard version of ComponentEditorPageSite for use within a
    ///  ComponentEditorForm.
    /// </summary>
    private sealed class ComponentEditorPageSite : IComponentEditorPageSite
    {
        internal IComponent _component;
        internal ComponentEditorPage _pageControl;
        internal Control _parent;
        internal bool _isActive;
        internal bool _isDirty;
        private readonly ComponentEditorForm _form;

        /// <summary>
        ///  Creates the page site.
        /// </summary>
        internal ComponentEditorPageSite(Control parent, Type pageClass, IComponent component, ComponentEditorForm form)
        {
            _component = component;
            _parent = parent;
            _isActive = false;
            _isDirty = false;

            _form = form.OrThrowIfNull();

            try
            {
                _pageControl = (ComponentEditorPage)Activator.CreateInstance(pageClass)!;
            }
            catch (TargetInvocationException e)
            {
                Debug.Fail(e.ToString());
                throw new TargetInvocationException(string.Format(SR.ExceptionCreatingCompEditorControl, e), e.InnerException);
            }

            _pageControl.SetSite(this);
            _pageControl.SetComponent(component);
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
                    _pageControl.CreateControl();

                    // activate it and give it focus
                    _pageControl.Activate();
                }
                else
                {
                    _pageControl.Deactivate();
                }

                _isActive = value;
            }
        }

        internal bool AutoCommit
        {
            get
            {
                return _pageControl.CommitOnDeactivate;
            }
        }

        internal bool Dirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;
            }
        }

        /// <summary>
        ///  Called by a page to return a parenting control for itself.
        /// </summary>
        public Control GetControl()
        {
            return _parent;
        }

        /// <summary>
        ///  Called by the ComponentEditorForm to get the actual page.
        /// </summary>
        internal ComponentEditorPage GetPageControl()
        {
            return _pageControl;
        }

        /// <summary>
        ///  Called by a page to mark it's contents as dirty.
        /// </summary>
        public void SetDirty()
        {
            if (_isActive)
            {
                Dirty = true;
            }

            _form.SetDirty();
        }
    }
}
