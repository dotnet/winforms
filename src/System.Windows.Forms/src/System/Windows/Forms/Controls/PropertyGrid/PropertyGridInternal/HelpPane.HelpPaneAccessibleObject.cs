// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class HelpPane
{
    /// <summary>
    ///  Represents the <see cref="HelpPane"/> accessible object.
    /// </summary>
    internal sealed class HelpPaneAccessibleObject : ControlAccessibleObject
    {
        private readonly WeakReference<PropertyGrid> _parentPropertyGrid;

        /// <summary>
        ///  Initializes new instance of the <see cref="HelpPaneAccessibleObject"/>.
        /// </summary>
        /// <param name="owningHelpPane">The owning <see cref="HelpPane"/> control.</param>
        /// <param name="parentPropertyGrid">The parent <see cref="PropertyGrid"/> control.</param>
        public HelpPaneAccessibleObject(HelpPane owningHelpPane, PropertyGrid parentPropertyGrid) : base(owningHelpPane)
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

        public override string Name
        {
            get
            {
                if (this.TryGetOwnerAs(out Control? owner))
                {
                    if (owner.Name is { } name)
                    {
                        return name;
                    }
                }

                return _parentPropertyGrid.TryGetTarget(out PropertyGrid? target)
                    ? string.Format(SR.PropertyGridHelpPaneAccessibleNameTemplate, target.AccessibilityObject.Name)
                    : string.Empty;
            }
        }

        private protected override bool IsInternal => true;

        internal override bool CanGetNameInternal => false;
    }
}
