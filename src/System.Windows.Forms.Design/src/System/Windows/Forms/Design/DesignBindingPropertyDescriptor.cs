// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

/// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor"]/*' />
/// <devdoc>
///    <para>Provides a property descriptor for design time data binding properties.</para>
/// </devdoc>
internal class DesignBindingPropertyDescriptor : PropertyDescriptor
{
    private static TypeConverter s_designBindingConverter = new DesignBindingConverter();
    private PropertyDescriptor _property;
    private bool _readOnly;

    // base.AttributeArray ends up calling the virtual FillAttributes, but we do not override it, so we should be okay.
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    internal DesignBindingPropertyDescriptor(PropertyDescriptor property, Attribute[] attrs, bool readOnly) : base(property.Name, attrs)
    {
        _property = property;
        _readOnly = readOnly;

        if (base.AttributeArray is not null && base.AttributeArray.Length > 0)
        {
            Attribute[] newAttrs = new Attribute[AttributeArray.Length + 2];
            AttributeArray.CopyTo(newAttrs, 0);
            newAttrs[AttributeArray.Length - 1] = NotifyParentPropertyAttribute.Yes;
            newAttrs[AttributeArray.Length] = RefreshPropertiesAttribute.Repaint;
            base.AttributeArray = newAttrs;
        }
        else
        {
            base.AttributeArray = [NotifyParentPropertyAttribute.Yes, RefreshPropertiesAttribute.Repaint];
        }
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.ComponentType"]/*' />
    /// <devdoc>
    ///    <para>Gets or sets the type of the component that owns the property.</para>
    /// </devdoc>
    public override Type ComponentType
    {
        get
        {
            return typeof(ControlBindingsCollection);
        }
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.Converter"]/*' />
    /// <devdoc>
    ///    <para>Gets or sets the type converter.</para>
    /// </devdoc>
    public override TypeConverter Converter
    {
        get
        {
            return s_designBindingConverter;
        }
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.IsReadOnly"]/*' />
    /// <devdoc>
    ///    <para>Indicates whether the property is read-only.</para>
    /// </devdoc>
    public override bool IsReadOnly
    {
        get
        {
            return _readOnly;
        }
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.PropertyType"]/*' />
    /// <devdoc>
    ///    <para>Gets or sets the type of the property.</para>
    /// </devdoc>
    public override Type PropertyType
    {
        get
        {
            return typeof(DesignBinding);
        }
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.CanResetValue"]/*' />
    /// <devdoc>
    ///    <para>Gets a value indicating whether the specified component can reset the value 
    ///       of the property.</para>
    /// </devdoc>
    public override bool CanResetValue(object component)
    {
        return !GetBinding((ControlBindingsCollection)component, _property).IsNull;
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.GetValue"]/*' />
    /// <devdoc>
    ///    <para>Gets a value from the specified component.</para>
    /// </devdoc>
    public override object GetValue(object component)
    {
        return GetBinding((ControlBindingsCollection)component, _property);
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.ResetValue"]/*' />
    /// <devdoc>
    ///    <para>Resets the value of the specified component.</para>
    /// </devdoc>
    public override void ResetValue(object component)
    {
        SetBinding((ControlBindingsCollection)component, _property, DesignBinding.Null);
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.SetValue"]/*' />
    /// <devdoc>
    ///    <para>Sets the specified value for the specified component.</para>
    /// </devdoc>
    public override void SetValue(object component, object value)
    {
        SetBinding((ControlBindingsCollection)component, _property, (DesignBinding)value);
        OnValueChanged(component, EventArgs.Empty);
    }

    /// <include file='doc\DesignBindingPropertyDescriptor.uex' path='docs/doc[@for="DesignBindingPropertyDescriptor.ShouldSerializeValue"]/*' />
    /// <devdoc>
    ///    <para>Indicates whether the specified component should persist the value.</para>
    /// </devdoc>
    public override bool ShouldSerializeValue(object component)
    {
        return false;
    }

    private static void SetBinding(ControlBindingsCollection bindings, PropertyDescriptor property, DesignBinding designBinding)
    {
        // this means it couldn't be parsed.
        if (designBinding is null)
            return;
        Binding listBinding = bindings[property.Name];
        if (listBinding is not null)
        {
            bindings.Remove(listBinding);
        }

        if (!designBinding.IsNull)
        {
            bindings.Add(property.Name, designBinding.DataSource, designBinding.DataMember);
        }
    }

    private static DesignBinding GetBinding(ControlBindingsCollection bindings, PropertyDescriptor property)
    {
        Binding listBinding = bindings[property.Name];
        if (listBinding is null)
            return DesignBinding.Null;
        else
            return new DesignBinding(listBinding.DataSource, listBinding.BindingMemberInfo.BindingMember);
    }
}
