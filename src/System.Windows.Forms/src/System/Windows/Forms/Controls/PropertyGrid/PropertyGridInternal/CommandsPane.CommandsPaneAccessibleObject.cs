// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class CommandsPane
{
    /// <summary>
    ///  Represents the <see cref="CommandsPane"/> accessible object.
    /// </summary>
    internal sealed class CommandsPaneAccessibleObject : ControlAccessibleObject
    {
        private readonly WeakReference<PropertyGrid> _parentPropertyGrid;

        /// <summary>
        ///  Initializes new instance of <see cref="CommandsPaneAccessibleObject"/>.
        /// </summary>
        /// <param name="owningCommandsPane">The owning <see cref="CommandsPane"/> control.</param>
        /// <param name="parentPropertyGrid">The parent <see cref="PropertyGrid"/> control.</param>
        public CommandsPaneAccessibleObject(CommandsPane owningCommandsPane, PropertyGrid parentPropertyGrid) : base(owningCommandsPane)
        {
            _parentPropertyGrid = new(parentPropertyGrid);
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (_parentPropertyGrid.TryGetTarget(out PropertyGrid? target)
                && target.AccessibilityObject is PropertyGrid.PropertyGridAccessibleObject propertyGridAccessibleObject)
            {
                IRawElementProviderFragment.Interface? navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
                if (navigationTarget is not null)
                {
                    return navigationTarget;
                }
            }

            return base.FragmentNavigate(direction);
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        public override string? Name
            => this.TryGetOwnerAs(out CommandsPane? owner)
                ? owner.AccessibleName ?? (_parentPropertyGrid.TryGetTarget(out PropertyGrid? target)
                    ? target.AccessibilityObject.Name
                    : null)
                : null;

        private protected override bool IsInternal => true;
    }
}
