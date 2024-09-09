// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Globalization;

namespace System.ComponentModel.Design.Serialization;

public abstract partial class CodeDomDesignerLoader
{
    private class ModifierConverter : TypeConverter
    {
        /// <summary>
        ///  <para>Gets a value indicating whether this converter can
        ///  convert an object in the given source type to the native type of the converter
        ///  using the context.</para>
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => GetConverter(context).CanConvertFrom(context, sourceType);

        /// <summary>
        ///  <para>Gets a value indicating whether this converter can
        ///  convert an object to the given destination type using the context.</para>
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            => GetConverter(context).CanConvertTo(context, destinationType);

        /// <summary>
        ///  <para>Converts the given object to the converter's native type.</para>
        /// </summary>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            => GetConverter(context).ConvertFrom(context, culture, value);

        /// <summary>
        ///  <para>Converts the given value object to
        ///  the specified destination type using the specified context and arguments.</para>
        /// </summary>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => GetConverter(context).ConvertTo(context, culture, value, destinationType);

        /// <summary>
        /// <para>Re-creates an <see cref="object"/> given a set of property values for the
        ///  object.</para>
        /// </summary>
        public override object? CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
            => GetConverter(context).CreateInstance(context, propertyValues);

        /// <summary>
        ///  Returns the type converter for the member attributes enum. We search the context
        ///  for a code dom provider that can provide us more information.
        /// </summary>
        private static TypeConverter GetConverter(ITypeDescriptorContext? context)
        {
            TypeConverter? modifierConverter = context?.GetService<CodeDomProvider>()?.GetConverter(typeof(MemberAttributes));
            return modifierConverter ?? TypeDescriptor.GetConverter(typeof(MemberAttributes));
        }

        /// <summary>
        ///  <para>Gets a value indicating whether changing a value on this object requires a
        ///  call to <see cref="TypeConverter.CreateInstance(IDictionary)"/> to create a new value,
        ///  using the specified context.</para>
        /// </summary>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context)
            => GetConverter(context).GetCreateInstanceSupported(context);

        /// <summary>
        ///  <para>Gets a collection of properties for
        ///  the type of array specified by the value parameter using the specified context and
        ///  attributes.</para>
        /// </summary>
        public override PropertyDescriptorCollection? GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => GetConverter(context).GetProperties(context, value, attributes);

        /// <summary>
        ///  <para>Gets a value indicating
        ///  whether this object supports properties using the
        ///  specified context.</para>
        /// </summary>
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
            => GetConverter(context).GetPropertiesSupported(context);

        /// <summary>
        ///  <para>Gets a collection of standard values for the data type this type converter is
        ///  designed for.</para>
        /// </summary>
        public override StandardValuesCollection? GetStandardValues(ITypeDescriptorContext? context)
        {
            // We restrict the set of standard values to those within the access mask.
            StandardValuesCollection? values = GetConverter(context).GetStandardValues(context);

            if (values is not null && values.Count > 0)
            {
                bool needMassage = false;

                foreach (MemberAttributes value in values)
                {
                    if ((value & MemberAttributes.AccessMask) == 0)
                    {
                        needMassage = true;
                        break;
                    }
                }

                if (needMassage)
                {
                    List<MemberAttributes> list = new(values.Count);

                    foreach (MemberAttributes value in values)
                    {
                        if ((value & MemberAttributes.AccessMask) != 0 && value != MemberAttributes.AccessMask)
                        {
                            list.Add(value);
                        }
                    }

                    values = new StandardValuesCollection(list);
                }
            }

            return values;
        }

        /// <summary>
        ///  <para>Gets a value indicating whether the collection of standard values returned from
        ///  <see cref="TypeConverter.GetStandardValues()"/> is an exclusive
        ///  list of possible values, using the specified context.</para>
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
            => GetConverter(context).GetStandardValuesExclusive(context);

        /// <summary>
        ///  <para>Gets a value indicating
        ///  whether this object
        ///  supports a standard set of values that can be picked
        ///  from a list using the specified context.</para>
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
            => GetConverter(context).GetStandardValuesSupported(context);

        /// <summary>
        ///  <para>Gets
        ///  a value indicating whether the given value object is valid for this type.</para>
        /// </summary>
        public override bool IsValid(ITypeDescriptorContext? context, object? value)
            => GetConverter(context).IsValid(context, value);
    }
}
