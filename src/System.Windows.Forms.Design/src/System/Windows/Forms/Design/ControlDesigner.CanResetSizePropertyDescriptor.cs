// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms.Design;

public partial class ControlDesigner
{
    private class CanResetSizePropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor _basePropDesc;

        public CanResetSizePropertyDescriptor(PropertyDescriptor pd) : base(pd) => _basePropDesc = pd;

        public override Type ComponentType => _basePropDesc.ComponentType;

        public override string DisplayName => _basePropDesc.DisplayName;

        public override bool IsReadOnly => _basePropDesc.IsReadOnly;

        public override Type PropertyType => _basePropDesc.PropertyType;

        // Since we can't get to the DefaultSize property, we use the existing ShouldSerialize logic.
        public override bool CanResetValue(object component) => _basePropDesc.ShouldSerializeValue(component);

        public override object GetValue(object component) => _basePropDesc.GetValue(component);

        public override void ResetValue(object component) => _basePropDesc.ResetValue(component);

        public override void SetValue(object component, object value) => _basePropDesc.SetValue(component, value);

        // we always want to serialize values.
        public override bool ShouldSerializeValue(object component) => true;
    }
}
