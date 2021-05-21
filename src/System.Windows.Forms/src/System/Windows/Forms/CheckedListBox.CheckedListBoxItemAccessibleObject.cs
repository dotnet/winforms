// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms
{
    public partial class CheckedListBox
    {
        internal class CheckedListBoxItemAccessibleObject : AccessibleObject
        {
            private string _name;
            private readonly int _index;
            private readonly CheckedListBoxAccessibleObject _parent;

            public CheckedListBoxItemAccessibleObject(string name, int index, CheckedListBoxAccessibleObject parent) : base()
            {
                _name = name;
                _parent = parent;
                _index = index;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!ParentCheckedListBox.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    Rectangle rect = ParentCheckedListBox.GetItemRectangle(_index);

                    if (rect.IsEmpty)
                    {
                        return rect;
                    }

                    // Translate rect to screen coordinates
                    //
                    rect = ParentCheckedListBox.RectangleToScreen(rect);
                    Rectangle visibleArea = ParentCheckedListBox.RectangleToScreen(ParentCheckedListBox.ClientRectangle);

                    if (visibleArea.Bottom < rect.Bottom)
                    {
                        rect.Height = visibleArea.Bottom - rect.Top;
                    }

                    rect.Width = visibleArea.Width;

                    return rect;
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (!ParentCheckedListBox.IsHandleCreated)
                    {
                        return string.Empty;
                    }

                    if (ParentCheckedListBox.GetItemChecked(_index))
                    {
                        return SR.AccessibleActionUncheck;
                    }
                    else
                    {
                        return SR.AccessibleActionCheck;
                    }
                }
            }

            private CheckedListBox ParentCheckedListBox
            {
                get
                {
                    return (CheckedListBox)_parent.Owner;
                }
            }

            public override string Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return _parent;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.CheckButton;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (!ParentCheckedListBox.IsHandleCreated)
                    {
                        return AccessibleStates.None;
                    }

                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Checked state
                    //
                    switch (ParentCheckedListBox.GetItemCheckState(_index))
                    {
                        case CheckState.Checked:
                            state |= AccessibleStates.Checked;
                            break;
                        case CheckState.Indeterminate:
                            state |= AccessibleStates.Indeterminate;
                            break;
                        case CheckState.Unchecked:
                            // No accessible state corresponding to unchecked
                            break;
                    }

                    // Selected state
                    //
                    if (ParentCheckedListBox.SelectedIndex == _index)
                    {
                        state |= AccessibleStates.Selected | AccessibleStates.Focused;
                    }

                    if (ParentCheckedListBox.Focused && ParentCheckedListBox.SelectedIndex == -1)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    return ParentCheckedListBox.GetItemChecked(_index).ToString();
                }
            }

            public override void DoDefaultAction()
            {
                if (!ParentCheckedListBox.IsHandleCreated)
                {
                    return;
                }

                ParentCheckedListBox.SetItemChecked(_index, !ParentCheckedListBox.GetItemChecked(_index));
            }

            public override AccessibleObject Navigate(AccessibleNavigation direction)
            {
                // Down/Next
                //
                if (direction == AccessibleNavigation.Down ||
                    direction == AccessibleNavigation.Next)
                {
                    if (_index < _parent.GetChildCount() - 1)
                    {
                        return _parent.GetChild(_index + 1);
                    }
                }

                // Up/Previous
                //
                if (direction == AccessibleNavigation.Up ||
                    direction == AccessibleNavigation.Previous)
                {
                    if (_index > 0)
                    {
                        return _parent.GetChild(_index - 1);
                    }
                }

                return base.Navigate(direction);
            }

            public override void Select(AccessibleSelection flags)
            {
                try
                {
                    if (ParentCheckedListBox.IsHandleCreated)
                    {
                        ParentCheckedListBox.AccessibilityObject.GetSystemIAccessibleInternal()?.accSelect((int)flags, _index + 1);
                    }
                }
                catch (ArgumentException)
                {
                    // In Everett, the CheckedListBox accessible children did not have any selection capability.
                    // In Whidbey, they delegate the selection capability to OLEACC.
                    // However, OLEACC does not deal w/ several Selection flags: ExtendSelection, AddSelection, RemoveSelection.
                    // OLEACC instead throws an ArgumentException.
                    // Since Whidbey API's should not throw an exception in places where Everett API's did not, we catch
                    // the ArgumentException and fail silently.
                }
            }
        }
    }
}
