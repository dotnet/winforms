// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class PropertyGrid
    {
        /// <summary>
        ///  Represents the PropertyGrid accessibility object.
        ///  Is used only in Accessibility Improvements of level3 to show correct accessible hierarchy.
        /// </summary>
        internal class PropertyGridAccessibleObject : Control.ControlAccessibleObject
        {
            private readonly PropertyGrid _owningPropertyGrid;

            /// <summary>
            ///  Initializes new instance of PropertyGridAccessibleObject
            /// </summary>
            /// <param name="owningPropertyGrid">The PropertyGrid owning control.</param>
            public PropertyGridAccessibleObject(PropertyGrid owningPropertyGrid) : base(owningPropertyGrid)
            {
                _owningPropertyGrid = owningPropertyGrid;
            }

            /// <summary>
            ///  Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </summary>
            /// <param name="x">x coordinate of point to check</param>
            /// <param name="y">y coordinate of point to check</param>
            /// <returns>Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </returns>
            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                if (!_owningPropertyGrid.IsHandleCreated)
                {
                    return null;
                }

                Point clientPoint = _owningPropertyGrid.PointToClient(new Point((int)x, (int)y));

                Control element = _owningPropertyGrid.GetElementFromPoint(clientPoint);
                if (element is not null)
                {
                    return element.AccessibilityObject;
                }

                return base.ElementProviderFromPoint(x, y);
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetChildFragment(0);
                    case UiaCore.NavigateDirection.LastChild:
                        var childFragmentCount = GetChildFragmentCount();
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
            internal UiaCore.IRawElementProviderFragment ChildFragmentNavigate(AccessibleObject childFragment, UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return this;
                    case UiaCore.NavigateDirection.NextSibling:
                        int fragmentCount = GetChildFragmentCount();
                        int childFragmentIndex = GetChildFragmentIndex(childFragment);
                        int nextChildFragmentIndex = childFragmentIndex + 1;
                        if (fragmentCount > nextChildFragmentIndex)
                        {
                            return GetChildFragment(nextChildFragmentIndex);
                        }

                        return null;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        fragmentCount = GetChildFragmentCount();
                        childFragmentIndex = GetChildFragmentIndex(childFragment);
                        if (childFragmentIndex > 0)
                        {
                            return GetChildFragment(childFragmentIndex - 1);
                        }

                        return null;
                }

                return null;
            }

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            /// <summary>
            ///  Gets the accessible child corresponding to the specified index.
            /// </summary>
            /// <param name="index">The child index.</param>
            /// <returns>The accessible child.</returns>
            internal AccessibleObject GetChildFragment(int index)
            {
                if (index < 0)
                {
                    return null;
                }

                if (_owningPropertyGrid.ToolbarVisible)
                {
                    if (index == 0)
                    {
                        return _owningPropertyGrid.ToolbarAccessibleObject;
                    }

                    index--;
                }

                if (_owningPropertyGrid.GridViewVisible)
                {
                    if (index == 0)
                    {
                        return _owningPropertyGrid.GridViewAccessibleObject;
                    }

                    index--;
                }

                if (_owningPropertyGrid.CommandsVisible)
                {
                    if (index == 0)
                    {
                        return _owningPropertyGrid.CommandsPaneAccessibleObject;
                    }

                    index--;
                }

                if (_owningPropertyGrid.HelpVisible)
                {
                    if (index == 0)
                    {
                        return _owningPropertyGrid.HelpPaneAccessibleObject;
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

                if (_owningPropertyGrid.ToolbarVisible)
                {
                    childCount++;
                }

                if (_owningPropertyGrid.GridViewVisible)
                {
                    childCount++;
                }

                if (_owningPropertyGrid.CommandsVisible)
                {
                    childCount++;
                }

                if (_owningPropertyGrid.HelpVisible)
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
            internal override UiaCore.IRawElementProviderFragment GetFocus()
            {
                return GetFocused();
            }

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
                    AccessibleObject childFragment = GetChildFragment(i);
                    if (childFragment == controlAccessibleObject)
                    {
                        return i;
                    }
                }

                return -1;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.NamePropertyId => Name,
                    _ => base.GetPropertyValue(propertyID),
                };
        }
    }
}
