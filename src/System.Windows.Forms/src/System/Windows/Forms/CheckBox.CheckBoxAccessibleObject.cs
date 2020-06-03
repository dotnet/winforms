// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class CheckBox
    {
        [ComVisible(true)]
        public class CheckBoxAccessibleObject : ButtonBaseAccessibleObject
        {
            public CheckBoxAccessibleObject(Control owner) : base(owner)
            {
            }

            public override string DefaultAction
            {
                get
                {
                    string defaultAction = Owner.AccessibleDefaultActionDescription;
                    if (defaultAction != null)
                    {
                        return defaultAction;
                    }

                    if (((CheckBox)Owner).Checked)
                    {
                        return SR.AccessibleActionUncheck;
                    }
                    else
                    {
                        return SR.AccessibleActionCheck;
                    }
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    return AccessibleRole.CheckButton;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    switch (((CheckBox)Owner).CheckState)
                    {
                        case CheckState.Checked:
                            return AccessibleStates.Checked | base.State;
                        case CheckState.Indeterminate:
                            return AccessibleStates.Indeterminate | base.State;
                    }

                    return base.State;
                }
            }

            public override void DoDefaultAction()
            {
                CheckBox cb = Owner as CheckBox;

                if (cb != null)
                {
                    cb.AccObjDoDefaultAction = true;
                }

                try
                {
                    base.DoDefaultAction();
                }
                finally
                {
                    if (cb != null)
                    {
                        cb.AccObjDoDefaultAction = false;
                    }
                }
            }
        }
    }
}
