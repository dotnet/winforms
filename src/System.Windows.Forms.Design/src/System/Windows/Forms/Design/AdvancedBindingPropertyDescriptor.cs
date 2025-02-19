// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

/// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor"]/*' />
/// <devdoc>
///    <para>Provides a property description of an advanced binding object.</para>
/// </devdoc>
internal class AdvancedBindingPropertyDescriptor : PropertyDescriptor
{

    internal static AdvancedBindingEditor s_advancedBindingEditor = new AdvancedBindingEditor();
    internal static AdvancedBindingTypeConverter s_advancedBindingTypeConverter = new AdvancedBindingTypeConverter();
    internal AdvancedBindingPropertyDescriptor() : base(SR.AdvancedBindingPropertyDescName, null)
    {
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.ComponentType"]/*' />
    /// <devdoc>
    ///    <para>Gets the type of component this property is bound to.</para>
    /// </devdoc>
    public override Type ComponentType
    {
        get
        {
            return typeof(ControlBindingsCollection);
        }
    }

    public override AttributeCollection Attributes
    {
        get
        {
            return new AttributeCollection([new SRDescriptionAttribute(SR.AdvancedBindingPropertyDescriptorDesc),
                                                               NotifyParentPropertyAttribute.Yes,
                                                               new MergablePropertyAttribute(false)]);
        }
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.IsReadOnly"]/*' />
    /// <devdoc>
    ///    <para>Indicates whether this property is read-only.</para>
    /// </devdoc>
    public override bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.PropertyType"]/*' />
    /// <devdoc>
    ///    <para>Gets the type of the property.</para>
    /// </devdoc>
    public override Type PropertyType
    {
        get
        {
            return typeof(object);
        }
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.TypeConverter"]/*' />
    /// <devdoc>
    ///    <para>Gets the type converter.</para>
    /// </devdoc>
    public override TypeConverter Converter
    {
        get
        {
            if (s_advancedBindingTypeConverter is null)
            {
                s_advancedBindingTypeConverter = new AdvancedBindingTypeConverter();
            }

            return s_advancedBindingTypeConverter;
        }
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.GetEditor"]/*' />
    /// <devdoc>
    ///    <para>Gets an editor of the specified type.</para>
    /// </devdoc>
    public override object GetEditor(Type type)
    {
        if (type == typeof(System.Drawing.Design.UITypeEditor))
        {
            return s_advancedBindingEditor;
        }

        return base.GetEditor(type);
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.CanResetValue"]/*' />
    /// <devdoc>
    ///    <para>Indicates whether resetting the component will change the value of the 
    ///       component.</para>
    /// </devdoc>
    public override bool CanResetValue(object component)
    {
        return false;
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.FillAttributes"]/*' />
    /// <devdoc>
    ///    <para>In an derived class, adds the attributes of the inherited class to the
    ///       specified list of attributes in the parent class.</para>
    /// </devdoc>
    protected override void FillAttributes(System.Collections.IList attributeList)
    {
        attributeList.Add(RefreshPropertiesAttribute.All);
        base.FillAttributes(attributeList);
    }


    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.GetValue"]/*' />
    /// <devdoc>
    ///    <para>Gets the current value of the property on the specified 
    ///       component.</para>
    /// </devdoc>
    public override object GetValue(object component)
    {
        Debug.Assert(component is ControlBindingsCollection, "we only deal w/ bindings collection");
        return component;
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.ResetValue"]/*' />
    /// <devdoc>
    ///    <para>Resets the value of the property on the specified component.</para>
    /// </devdoc>
    public override void ResetValue(object component)
    {
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.SetValue"]/*' />
    /// <devdoc>
    ///    <para>Sets the value of the property on the specified component to the specified 
    ///       value.</para>
    /// </devdoc>
    public override void SetValue(object component, object value)
    {
    }

    /// <include file='doc\AdvancedBindingPropertyDescriptor.uex' path='docs/doc[@for="AdvancedBindingPropertyDescriptor.ShouldSerializeValue"]/*' />
    /// <devdoc>
    ///    <para>Indicates whether the value of this property should be persisted.</para>
    /// </devdoc>
    public override bool ShouldSerializeValue(object component)
    {
        return false;
    }


    internal class AdvancedBindingTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Debug.Assert(value is ControlBindingsCollection, "we only deal w/ bindings collection");
            if (destinationType == typeof(String))
            {
                return String.Empty;
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
