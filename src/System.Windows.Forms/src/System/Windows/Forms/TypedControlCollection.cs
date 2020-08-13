// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This control collection only allows a specific type of control
    ///  into the controls collection. It optionally supports readonlyness.
    /// </summary>
    internal class TypedControlCollection : ReadOnlyControlCollection
    {
        private readonly Type _typeOfControl;
        private readonly Control _ownerControl;

        public TypedControlCollection(Control owner, Type typeOfControl, bool isReadOnly) : base(owner, isReadOnly)
        {
            _typeOfControl = typeOfControl;
            _ownerControl = owner;
        }

        public TypedControlCollection(Control owner, Type typeOfControl) : base(owner, /*isReadOnly*/false)
        {
            _typeOfControl = typeOfControl;
            _ownerControl = owner;
        }

        public override void Add(Control value)
        {
            // Check parenting first for consistency
            Control.CheckParentingCycle(_ownerControl, value);

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ReadonlyControlsCollection);
            }
            if (!_typeOfControl.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, string.Format(SR.TypedControlCollectionShouldBeOfType, _typeOfControl.Name)), value.GetType().Name);
            }

            base.Add(value);
        }
    }
}
