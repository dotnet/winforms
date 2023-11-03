// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms;

internal partial class PropertyGridToolStrip
{
    /// <summary>
    ///  Represents the PropertyGridToolStrip control accessibility object.
    /// </summary>
    internal class PropertyGridToolStripAccessibleObject : ToolStripAccessibleObject
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

        /// <summary>
        ///  Request to return the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
        {
            if (_parentPropertyGrid.TryGetTarget(out PropertyGrid? target)
                && target.IsHandleCreated
                && target.AccessibilityObject is PropertyGrid.PropertyGridAccessibleObject propertyGridAccessibleObject)
            {
                UiaCore.IRawElementProviderFragment? navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
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
    }
}
