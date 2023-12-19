// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

internal partial class PropertyGridToolStrip
{
    /// <summary>
    ///  Represents the PropertyGridToolStrip control accessibility object.
    /// </summary>
    internal sealed class PropertyGridToolStripAccessibleObject : ToolStripAccessibleObject
    {
        private readonly WeakReference<PropertyGrid> _parentPropertyGrid;

        /// <summary>
        ///  Constructs new instance of PropertyGridToolStripAccessibleObject
        /// </summary>
        /// <param name="owningPropertyGridToolStrip">The PropertyGridToolStrip owning control.</param>
        /// <param name="parentPropertyGrid">The parent PropertyGrid control.</param>
        public PropertyGridToolStripAccessibleObject(PropertyGridToolStrip owningPropertyGridToolStrip, PropertyGrid parentPropertyGrid) : base(owningPropertyGridToolStrip)
        {
            _parentPropertyGrid = new(parentPropertyGrid);
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (_parentPropertyGrid.TryGetTarget(out PropertyGrid? target)
                && target.IsHandleCreated
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
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ToolBarControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        public override string? Name
        {
            get
            {
                if (this.TryGetOwnerAs(out PropertyGridToolStrip? owner) && owner.AccessibleName is { } name)
                {
                    return name;
                }

                _parentPropertyGrid.TryGetTarget(out PropertyGrid? target);
                return target?.AccessibilityObject.Name;
            }
        }

        private protected override bool IsInternal => true;

        internal override bool CanGetNameInternal =>
            (!this.TryGetOwnerAs(out PropertyGridToolStrip? owner) || owner.AccessibleName is null)
            && _parentPropertyGrid.TryGetTarget(out PropertyGrid? target)
            && target.AccessibilityObject.CanGetNameInternal;

        internal override BSTR GetNameInternal()
        {
            _parentPropertyGrid.TryGetTarget(out PropertyGrid? target);
            Debug.Assert(target is not null);
            return target is not null ? target.AccessibilityObject.GetNameInternal() : default;
        }
    }
}
