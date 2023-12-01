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
        private readonly MdiClient owner;

        /*C#r: protected*/

        public ControlCollection(MdiClient owner)
        : base(owner)
        {
            this.owner = owner;
        }

        /// <summary>
        ///  <para>
        ///  Adds a control to the MDI Container. This child must be
        ///  a Form that is marked as an MDI Child to be added to the
        ///  container. You should not call this directly, but rather
        ///  set the child form's (ctl) MDIParent property:
        ///  </para>
        /// <code>
        ///  //     wrong
        ///  Form child = new ChildForm();
        ///  this.getMdiClient().add(child);
        ///  //     right
        ///  Form child = new ChildForm();
        ///  child.setMdiParent(this);
        /// </code>
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

            if (owner.CreateThreadId != value.CreateThreadId)
            {
                throw new ArgumentException(SR.AddDifferentThreads, nameof(value));
            }

            owner._children.Add(form);
            base.Add(value);
        }

        /// <summary>
        ///  Removes a child control.
        /// </summary>
        public override void Remove(Control? value)
        {
            if (value is Form form)
            {
                owner._children.Remove(form);
            }

            base.Remove(value);
        }
    }
}
