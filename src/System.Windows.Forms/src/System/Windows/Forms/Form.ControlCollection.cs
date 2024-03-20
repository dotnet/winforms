// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Form
{
    /// <summary>
    ///  Represents a collection of controls on the form.
    /// </summary>
    public new class ControlCollection : Control.ControlCollection
    {
        private readonly Form _owner;

        /*C#r:protected*/

        /// <summary>
        ///  Initializes a new instance of the ControlCollection class.
        /// </summary>
        public ControlCollection(Form owner)
            : base(owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///  Adds a control to the form.
        /// </summary>
        public override void Add(Control? value)
        {
            if (value is MdiClient client && _owner._ctlClient is null)
            {
                if (!_owner.TopLevel && !_owner.DesignMode)
                {
                    throw new ArgumentException(SR.MDIContainerMustBeTopLevel, nameof(value));
                }

                _owner.AutoScroll = false;
                if (_owner.IsMdiChild)
                {
                    throw new ArgumentException(SR.FormMDIParentAndChild, nameof(value));
                }

                _owner._ctlClient = client;
            }

            // make sure we don't add a form that has a valid mdi parent
            if (value is Form form && form.MdiParentInternal is not null)
            {
                throw new ArgumentException(SR.FormMDIParentCannotAdd, nameof(value));
            }

            base.Add(value);

            _owner._ctlClient?.SendToBack();
        }

        /// <summary>
        ///  Removes a control from the form.
        /// </summary>
        public override void Remove(Control? value)
        {
            if (value == _owner._ctlClient)
            {
                _owner._ctlClient = null;
            }

            base.Remove(value);
        }
    }
}
