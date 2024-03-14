// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public sealed partial class MdiClient
{
    /// <summary>
    ///  Collection of controls.
    /// </summary>
    public new class ControlCollection : Control.ControlCollection
    {
        private readonly MdiClient _owner;

        public ControlCollection(MdiClient owner) : base(owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///  <para>
        ///   Adds a control to the MDI Container. This child must be
        ///   a Form that is marked as an MDI Child to be added to the
        ///   container. You should not call this directly, but rather
        ///   set the child form's (ctl) MDIParent property:
        ///  </para>
        ///  <code>
        ///   // Wrong
        ///   Form child = new ChildForm();
        ///   this.MdiClient.Add(child);
        ///
        ///   // Right
        ///   Form child = new ChildForm();
        ///   child.MdiParent = this;
        ///  </code>
        /// </summary>
        public override void Add(Control? value)
        {
            if (value is null)
            {
                return;
            }

            if (value is not Form form || !form.IsMdiChild)
            {
                throw new ArgumentException(SR.MDIChildAddToNonMDIParent, nameof(value));
            }

            if (_owner.CreateThreadId != value.CreateThreadId)
            {
                throw new ArgumentException(SR.AddDifferentThreads, nameof(value));
            }

            _owner._children.Add(form);
            base.Add(value);
        }

        /// <summary>
        ///  Removes a child control.
        /// </summary>
        public override void Remove(Control? value)
        {
            if (value is Form form)
            {
                _owner._children.Remove(form);
            }

            base.Remove(value);
        }
    }
}
