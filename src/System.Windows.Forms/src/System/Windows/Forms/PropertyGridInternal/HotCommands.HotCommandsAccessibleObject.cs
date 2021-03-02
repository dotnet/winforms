// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class HotCommands
    {
        /// <summary>
        ///  Represents the hot commands control accessible object.
        /// </summary>
        internal class HotCommandsAccessibleObject : Control.ControlAccessibleObject
        {
            private readonly PropertyGrid _parentPropertyGrid;

            /// <summary>
            ///  Initializes new instance of DocCommentAccessibleObject.
            /// </summary>
            /// <param name="owningHotCommands">The owning HotCommands control.</param>
            /// <param name="parentPropertyGrid">The parent PropertyGrid control.</param>
            public HotCommandsAccessibleObject(HotCommands owningHotCommands, PropertyGrid parentPropertyGrid) : base(owningHotCommands)
            {
                _parentPropertyGrid = parentPropertyGrid;
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (_parentPropertyGrid.AccessibilityObject is PropertyGrid.PropertyGridAccessibleObject propertyGridAccessibleObject)
                {
                    UiaCore.IRawElementProviderFragment navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
                    if (navigationTarget is not null)
                    {
                        return navigationTarget;
                    }
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Request value of specified property from an element.
            /// </summary>
            /// <param name="propertyID">Identifier indicating the property to return</param>
            /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.PaneControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    _ => base.GetPropertyValue(propertyID)
                };

            public override string? Name => Owner?.AccessibleName ?? _parentPropertyGrid?.AccessibilityObject.Name;
        }
    }
}
