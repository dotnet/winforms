// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class PropertyGrid
{
    /// <summary>
    ///  Represents the PropertyGrid accessibility object.
    ///  Is used only in Accessibility Improvements of level3 to show correct accessible hierarchy.
    /// </summary>
    internal class PropertyGridAccessibleObject : ControlAccessibleObject
    {
        /// <summary>
        ///  Initializes new instance of PropertyGridAccessibleObject
        /// </summary>
        /// <param name="owningPropertyGrid">The PropertyGrid owning control.</param>
        public PropertyGridAccessibleObject(PropertyGrid owningPropertyGrid) : base(owningPropertyGrid) { }

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
        {
            if (!this.IsOwnerHandleCreated(out PropertyGrid? owningPropertyGrid))
            {
                return null;
            }

            Point clientPoint = owningPropertyGrid.PointToClient(new Point((int)x, (int)y));

            Control? element = owningPropertyGrid.GetElementFromPoint(clientPoint);
            if (element is not null)
            {
                return element.AccessibilityObject;
            }

            return base.ElementProviderFromPoint(x, y);
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            switch (direction)
            {
                case NavigateDirection.NavigateDirection_FirstChild:
                    return GetChildFragment(0);
                case NavigateDirection.NavigateDirection_LastChild:
                    int childFragmentCount = GetChildFragmentCount();
                    if (childFragmentCount > 0)
                    {
                        return GetChildFragment(childFragmentCount - 1);
                    }

                    break;
            }

            return base.FragmentNavigate(direction);
        }

        /// <summary>
        ///  Request to return the element in the specified direction regarding the provided child element.
        /// </summary>
        /// <param name="childFragment">The child element regarding which the target element is searched.</param>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal IRawElementProviderFragment.Interface? ChildFragmentNavigate(AccessibleObject childFragment, NavigateDirection direction)
        {
            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                    return this;
                case NavigateDirection.NavigateDirection_NextSibling:
                    int fragmentCount = GetChildFragmentCount();
                    int childFragmentIndex = GetChildFragmentIndex(childFragment);
                    int nextChildFragmentIndex = childFragmentIndex + 1;
                    if (fragmentCount > nextChildFragmentIndex)
                    {
                        return GetChildFragment(nextChildFragmentIndex);
                    }

                    return null;
                case NavigateDirection.NavigateDirection_PreviousSibling:
                    childFragmentIndex = GetChildFragmentIndex(childFragment);
                    if (childFragmentIndex > 0)
                    {
                        return GetChildFragment(childFragmentIndex - 1);
                    }

                    return null;
            }

            return null;
        }

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

        /// <summary>
        ///  Gets the accessible child corresponding to the specified index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>The accessible child.</returns>
        internal AccessibleObject? GetChildFragment(int index)
        {
            if (!this.TryGetOwnerAs(out PropertyGrid? owningPropertyGrid) || index < 0)
            {
                return null;
            }

            if (owningPropertyGrid.ToolbarVisible)
            {
                if (index == 0)
                {
                    return owningPropertyGrid.ToolbarAccessibleObject;
                }

                index--;
            }

            if (owningPropertyGrid.GridViewVisible)
            {
                if (index == 0)
                {
                    return owningPropertyGrid.GridViewAccessibleObject;
                }

                index--;
            }

            if (owningPropertyGrid.CommandsVisible)
            {
                if (index == 0)
                {
                    return owningPropertyGrid.CommandsPaneAccessibleObject;
                }

                index--;
            }

            if (owningPropertyGrid.HelpVisible)
            {
                if (index == 0)
                {
                    return owningPropertyGrid.HelpPaneAccessibleObject;
                }
            }

            return null;
        }

        /// <summary>
        ///  Gets the number of children belonging to an accessible object.
        /// </summary>
        /// <returns>The number of children.</returns>
        internal int GetChildFragmentCount()
        {
            int childCount = 0;

            if (!this.TryGetOwnerAs(out PropertyGrid? owningPropertyGrid))
            {
                return childCount;
            }

            if (owningPropertyGrid.ToolbarVisible)
            {
                childCount++;
            }

            if (owningPropertyGrid.GridViewVisible)
            {
                childCount++;
            }

            if (owningPropertyGrid.CommandsVisible)
            {
                childCount++;
            }

            if (owningPropertyGrid.HelpVisible)
            {
                childCount++;
            }

            return childCount;
        }

        /// <summary>
        ///  Return the element in this fragment which has the keyboard focus,
        /// </summary>
        /// <returns>Return the element in this fragment which has the keyboard focus,
        ///  if any; otherwise return null.</returns>
        internal override IRawElementProviderFragment.Interface? GetFocus() => GetFocused();

        /// <summary>
        ///  Gets the child control index.
        /// </summary>
        /// <param name="controlAccessibleObject">The control accessible object which index should be found.</param>
        /// <returns>The child accessible index or -1 if not found.</returns>
        internal int GetChildFragmentIndex(AccessibleObject controlAccessibleObject)
        {
            int childFragmentCount = GetChildFragmentCount();
            for (int i = 0; i < childFragmentCount; i++)
            {
                AccessibleObject? childFragment = GetChildFragment(i);
                if (childFragment == controlAccessibleObject)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
