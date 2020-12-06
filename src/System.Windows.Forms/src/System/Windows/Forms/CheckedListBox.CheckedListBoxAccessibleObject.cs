// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class CheckedListBox
    {
        internal class CheckedListBoxAccessibleObject : ControlAccessibleObject
        {
            public CheckedListBoxAccessibleObject(CheckedListBox owner) : base(owner)
            {
            }

            private CheckedListBox CheckedListBox
            {
                get
                {
                    return (CheckedListBox)Owner;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index >= 0 && index < CheckedListBox.Items.Count)
                {
                    return new CheckedListBoxItemAccessibleObject(CheckedListBox.GetItemText(CheckedListBox.Items[index]), index, this);
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                return CheckedListBox.Items.Count;
            }

            public override AccessibleObject GetFocused()
            {
                if (!CheckedListBox.IsHandleCreated)
                {
                    return null;
                }

                int index = CheckedListBox.FocusedIndex;

                if (index >= 0)
                {
                    return GetChild(index);
                }

                return null;
            }

            public override AccessibleObject GetSelected()
            {
                if (!CheckedListBox.IsHandleCreated)
                {
                    return null;
                }

                int index = CheckedListBox.SelectedIndex;

                if (index >= 0)
                {
                    return GetChild(index);
                }

                return null;
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                if (!CheckedListBox.IsHandleCreated)
                {
                    return null;
                }

                // Within a child element?
                //
                int count = GetChildCount();

                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject child = GetChild(index);

                    if (child.Bounds.Contains(x, y))
                    {
                        return child;
                    }
                }

                // Within the CheckedListBox bounds?
                //
                if (Bounds.Contains(x, y))
                {
                    return this;
                }

                return null;
            }

            public override AccessibleObject Navigate(AccessibleNavigation direction)
            {
                if (GetChildCount() > 0)
                {
                    if (direction == AccessibleNavigation.FirstChild)
                    {
                        return GetChild(0);
                    }
                    if (direction == AccessibleNavigation.LastChild)
                    {
                        return GetChild(GetChildCount() - 1);
                    }
                }
                return base.Navigate(direction);
            }
        }
    }
}
