// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class HelpPane
    {
        /// <summary>
        ///  Represents the <see cref="HelpPane"/> accessible object.
        /// </summary>
        internal class HelpPaneAccessibleObject : ControlAccessibleObject
        {
            private readonly PropertyGrid _parentPropertyGrid;

            /// <summary>
            ///  Initializes new instance of the <see cref="HelpPaneAccessibleObject"/>.
            /// </summary>
            /// <param name="owningHelpPane">The owning <see cref="HelpPane"/> control.</param>
            /// <param name="parentPropertyGrid">The parent <see cref="PropertyGrid"/> control.</param>
            public HelpPaneAccessibleObject(HelpPane owningHelpPane, PropertyGrid parentPropertyGrid) : base(owningHelpPane)
            {
                _parentPropertyGrid = parentPropertyGrid;
            }

            /// <inheritdoc />
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

            /// <inheritdoc />
            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.PaneControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    _ => base.GetPropertyValue(propertyID)
                };

            public override string Name => Owner?.AccessibleName
                ?? string.Format(SR.PropertyGridHelpPaneAccessibleNameTemplate, _parentPropertyGrid?.AccessibilityObject.Name);
        }
    }
}
