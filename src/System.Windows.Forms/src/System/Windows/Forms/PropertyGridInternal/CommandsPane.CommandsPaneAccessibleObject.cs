﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class CommandsPane
{
    /// <summary>
    ///  Represents the <see cref="CommandsPane"/> accessible object.
    /// </summary>
    internal class CommandsPaneAccessibleObject : ControlAccessibleObject
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

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (_parentPropertyGrid.TryGetTarget(out PropertyGrid? target)
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

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.PaneControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        public override string? Name
            => this.TryGetOwnerAs(out CommandsPane? owner)
                ? owner.AccessibleName ?? (_parentPropertyGrid.TryGetTarget(out PropertyGrid? target)
                    ? target.AccessibilityObject.Name
                    : null)
                : null;
    }
}
