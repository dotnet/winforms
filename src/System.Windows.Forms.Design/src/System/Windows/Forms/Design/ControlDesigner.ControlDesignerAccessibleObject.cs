// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    public partial class ControlDesigner
    {
        public class ControlDesignerAccessibleObject : AccessibleObject
        {
            private readonly ControlDesigner _designer;
            private readonly Control _control;
            private IDesignerHost _host;
            private ISelectionService _selSvc;

            public ControlDesignerAccessibleObject(ControlDesigner designer, Control control)
            {
                _designer = designer;
                _control = control;
            }

            public override Rectangle Bounds
            {
                get => _control.AccessibilityObject.Bounds;
            }

            public override string Description
            {
                get => _control.AccessibilityObject.Description;
            }

            private IDesignerHost DesignerHost
            {
                get
                {
                    if (_host is null)
                    {
                        _host = (IDesignerHost)_designer.GetService(typeof(IDesignerHost));
                    }
                    return _host;
                }
            }

            public override string DefaultAction
            {
                get => "";
            }

            public override string Name
            {
                get => _control.Name;
            }

            public override AccessibleObject Parent
            {
                get => _control.AccessibilityObject.Parent;
            }

            public override AccessibleRole Role
            {
                get => _control.AccessibilityObject.Role;
            }

            private ISelectionService SelectionService
            {
                get
                {
                    if (_selSvc is null)
                    {
                        _selSvc = (ISelectionService)_designer.GetService(typeof(ISelectionService));
                    }
                    return _selSvc;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = _control.AccessibilityObject.State;
                    ISelectionService s = SelectionService;
                    if (s != null)
                    {
                        if (s.GetComponentSelected(_control))
                        {
                            state |= AccessibleStates.Selected;
                        }
                        if (s.PrimarySelection == _control)
                        {
                            state |= AccessibleStates.Focused;
                        }
                    }
                    return state;
                }
            }

            public override string Value
            {
                get => _control.AccessibilityObject.Value;
            }

            public override AccessibleObject GetChild(int index)
            {
                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"ControlDesignerAccessibleObject.GetChild({index})");

                if (_control.AccessibilityObject.GetChild(index) is Control.ControlAccessibleObject childAccObj)
                {
                    AccessibleObject cao = GetDesignerAccessibleObject(childAccObj);
                    if (cao != null)
                    {
                        return cao;
                    }
                }
                return _control.AccessibilityObject.GetChild(index);
            }

            public override int GetChildCount() => _control.AccessibilityObject.GetChildCount();

            private AccessibleObject GetDesignerAccessibleObject(Control.ControlAccessibleObject cao)
            {
                if (cao is null)
                {
                    return null;
                }
                if (DesignerHost.GetDesigner(cao.Owner) is ControlDesigner ctlDesigner)
                {
                    return ctlDesigner.AccessibilityObject;
                }
                return null;
            }

            public override AccessibleObject GetFocused()
            {
                if ((State & AccessibleStates.Focused) != 0)
                {
                    return this;
                }
                return base.GetFocused();
            }

            public override AccessibleObject GetSelected()
            {
                if ((State & AccessibleStates.Selected) != 0)
                {
                    return this;
                }
                return base.GetFocused();
            }

            public override AccessibleObject HitTest(int x, int y) => _control.AccessibilityObject.HitTest(x, y);
        }
    }
}
