// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms
{
    internal class Formatter
    {
        private static readonly Type stringType = typeof(string);
        private static readonly Type booleanType = typeof(bool);
        private static readonly Type checkStateType = typeof(CheckState);
        private static readonly object parseMethodNotFound = new object();
        private static readonly object defaultDataSourceNullValue = System.DBNull.Value;

        /// <summary>
        ///  Converts a binary value into a format suitable for display to the end user.
        ///  Used when pushing a value from a back-end data source into a data-bound property on a control.
        ///
        ///  The real conversion work happens inside FormatObjectInternal(). Before calling FormatObjectInternal(),
        ///  we check for any use of nullable types or values (eg. <see cref="Nullable{T}"/>) and 'unwrap'
        ///  them to get at the real types and values, which are then used in the actual conversion.
        ///  If the caller is expecting a nullable value back, we must also re-wrap the final result
        ///  inside a nullable value before returning.
        /// </summary>
        public static object FormatObject(object value,
                                          Type targetType,
                                          TypeConverter sourceConverter,
                                          TypeConverter targetConverter,
                                          string formatString,
                                          IFormatProvider formatInfo,
                                          object formattedNullValue,
                                          object dataSourceNullValue)
        {
            //
            // On the way in, see if value represents 'null' for this back-end field type, and substitute DBNull.
            // For most types, 'null' is actually represented by DBNull. But for a nullable type, its represented
            // by an instance of that type with no value. And for business objects it may be represented by a
            // simple null reference.
            //

            if (Formatter.IsNullData(value, dataSourceNullValue))
            {
                value = System.DBNull.Value;
            }

            //
            // Strip away any use of nullable types (eg. Nullable<int>), leaving just the 'real' types
            //

            Type oldTargetType = targetType;

            targetType = NullableUnwrap(targetType);
            sourceConverter = NullableUnwrap(sourceConverter);
            targetConverter = NullableUnwrap(targetConverter);

            bool isNullableTargetType = (targetType != oldTargetType);

            //
            // Call the 'real' method to perform the conversion
            //

            object result = FormatObjectInternal(value, targetType, sourceConverter, targetConverter, formatString, formatInfo, formattedNullValue);

            if (oldTargetType.IsValueType && result is null && !isNullableTargetType)
            {
                throw new FormatException(GetCantConvertMessage(value, targetType));
            }
            return result;
        }

        /// <summary>
        ///
        ///  Converts a value into a format suitable for display to the end user.
        ///
        ///  - Converts DBNull or null into a suitable formatted representation of 'null'
        ///  - Performs some special-case conversions (eg. Boolean to CheckState)
        ///  - Uses TypeConverters or IConvertible where appropriate
        ///  - Throws a FormatException is no suitable conversion can be found
        /// </summary>
        private static object FormatObjectInternal(object value,
                                                   Type targetType,
                                                   TypeConverter sourceConverter,
                                                   TypeConverter targetConverter,
                                                   string formatString,
                                                   IFormatProvider formatInfo,
                                                   object formattedNullValue)
        {
            if (value == System.DBNull.Value || value is null)
            {
                //
                // Convert DBNull to the formatted representation of 'null' (if possible)
                //
                if (formattedNullValue != null)
                {
                    return formattedNullValue;
                }

                //
                // Convert DBNull or null to a specific 'known' representation of null (otherwise fail)
                //
                if (targetType == stringType)
                {
                    return string.Empty;
                }

                if (targetType == checkStateType)
                {
                    return CheckState.Indeterminate;
                }

                // Just pass null through: if this is a value type, it's been unwrapped here, so we return null
                // and the caller has to wrap if appropriate.
                return null;
            }

            //
            // Special case conversions
            //

            if (targetType == stringType)
            {
                if (value is IFormattable && !string.IsNullOrEmpty(formatString))
                {
                    return (value as IFormattable).ToString(formatString, formatInfo);
                }
            }

            //The converters for properties should take precedence.  Unfortunately, we don't know whether we have one.  Check vs. the
            //type's TypeConverter.  We're punting the case where the property-provided converter is the same as the type's converter.
            Type sourceType = value.GetType();
            TypeConverter sourceTypeTypeConverter = TypeDescriptor.GetConverter(sourceType);
            if (sourceConverter != null && sourceConverter != sourceTypeTypeConverter && sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(null, GetFormatterCulture(formatInfo), value, targetType);
            }

            TypeConverter targetTypeTypeConverter = TypeDescriptor.GetConverter(targetType);
            if (targetConverter != null && targetConverter != targetTypeTypeConverter && targetConverter.CanConvertFrom(sourceType))
            {
                return targetConverter.ConvertFrom(null, GetFormatterCulture(formatInfo), value);
            }

            if (targetType == checkStateType)
            {
                if (sourceType == booleanType)
                {
                    return ((bool)value) ? CheckState.Checked : CheckState.Unchecked;
                }
                else
                {
                    if (sourceConverter is null)
                    {
                        sourceConverter = sourceTypeTypeConverter;
                    }
                    if (sourceConverter != null && sourceConverter.CanConvertTo(booleanType))
                    {
                        return (bool)sourceConverter.ConvertTo(null, GetFormatterCulture(formatInfo), value, booleanType)
                            ? CheckState.Checked : CheckState.Unchecked;
                    }
                }
            }

            if (targetType.IsAssignableFrom(sourceType))
            {
                return value;
            }

            //
            // If explicit type converters not provided, supply default ones instead
            //

            if (sourceConverter is null)
            {
                sourceConverter = sourceTypeTypeConverter;
            }

            if (targetConverter is null)
            {
                targetConverter = targetTypeTypeConverter;
            }

            //
            // Standardized conversions
            //

            if (sourceConverter != null && sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(null, GetFormatterCulture(formatInfo), value, targetType);
            }
            else if (targetConverter != null && targetConverter.CanConvertFrom(sourceType))
            {
                return targetConverter.ConvertFrom(null, GetFormatterCulture(formatInfo), value);
            }
            else if (value is IConvertible)
            {
                return ChangeType(value, targetType, formatInfo);
            }

            //
            // Fail if no suitable conversion found
            //

            throw new FormatException(GetCantConvertMessage(value, targetType));
        }

        /// <summary>
        ///  Converts a value entered by the end user (through UI) into the corresponding binary value.
        ///  Used when pulling input from a data-bound property on a control to store in a back-end data source.
        ///
        ///  The real conversion work happens inside ParseObjectInternal(). Before calling ParseObjectInternal(),
        ///  we check for any use of nullable types or values (eg. <see cref="Nullable{T}"/>) and 'unwrap'
        ///  them to get at the real types and values, which are then used in the actual conversion.
        ///  If the caller is expecting a nullable value back, we must also re-wrap the final result
        ///  inside a nullable value before returning.
        /// </summary>
        public static object ParseObject(object value,
                                         Type targetType,
                                         Type sourceType,
                                         TypeConverter targetConverter,
                                         TypeConverter sourceConverter,
                                         IFormatProvider formatInfo,
                                         object formattedNullValue,
                                         object dataSourceNullValue)
        {
            //
            // Strip away any use of nullable types (eg. Nullable<int>), leaving just the 'real' types
            //

            Type oldTargetType = targetType;

            sourceType = NullableUnwrap(sourceType);
            targetType = NullableUnwrap(targetType);
            sourceConverter = NullableUnwrap(sourceConverter);
            targetConverter = NullableUnwrap(targetConverter);

            bool isNullableTargetType = (targetType != oldTargetType);

            //
            // Call the 'real' method to perform the conversion
            //

            object result = ParseObjectInternal(value, targetType, sourceType, targetConverter, sourceConverter, formatInfo, formattedNullValue);

            //
            // On the way out, substitute DBNull with the appropriate representation of 'null' for the final target type.
            // For most types, this is just DBNull. But for a nullable type, its an instance of that type with no value.
            //

            if (result == System.DBNull.Value)
            {
                return Formatter.NullData(oldTargetType, dataSourceNullValue);
            }

            return result;
        }

        /// <summary>
        ///
        ///  Converts a value entered by the end user (through UI) into the corresponding binary value.
        ///
        ///  - Converts formatted representations of 'null' into DBNull
        ///  - Performs some special-case conversions (eg. CheckState to Boolean)
        ///  - Uses TypeConverters or IConvertible where appropriate
        ///  - Throws a FormatException is no suitable conversion can be found
        /// </summary>
        private static object ParseObjectInternal(object value,
                                                  Type targetType,
                                                  Type sourceType,
                                                  TypeConverter targetConverter,
                                                  TypeConverter sourceConverter,
                                                  IFormatProvider formatInfo,
                                                  object formattedNullValue)
        {
            //
            // Convert the formatted representation of 'null' to DBNull (if possible)
            //

            if (EqualsFormattedNullValue(value, formattedNullValue, formatInfo) || value == System.DBNull.Value)
            {
                return System.DBNull.Value;
            }

            //
            // Special case conversions
            //

            TypeConverter targetTypeTypeConverter = TypeDescriptor.GetConverter(targetType);
            if (targetConverter != null && targetTypeTypeConverter != targetConverter && targetConverter.CanConvertFrom(sourceType))
            {
                return targetConverter.ConvertFrom(null, GetFormatterCulture(formatInfo), value);
            }

            TypeConverter sourceTypeTypeConverter = TypeDescriptor.GetConverter(sourceType);
            if (sourceConverter != null && sourceTypeTypeConverter != sourceConverter && sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(null, GetFormatterCulture(formatInfo), value, targetType);
            }

            if (value is string)
            {
                // If target type has a suitable Parse method, use that to parse strings
                object parseResult = InvokeStringParseMethod(value, targetType, formatInfo);
                if (parseResult != parseMethodNotFound)
                {
                    return parseResult;
                }
            }
            else if (value is CheckState state)
            {
                if (state == CheckState.Indeterminate)
                {
                    return DBNull.Value;
                }
                // Explicit conversion from CheckState to Boolean
                if (targetType == booleanType)
                {
                    return (state == CheckState.Checked);
                }
                if (targetConverter is null)
                {
                    targetConverter = targetTypeTypeConverter;
                }
                if (targetConverter != null && targetConverter.CanConvertFrom(booleanType))
                {
                    return targetConverter.ConvertFrom(null, GetFormatterCulture(formatInfo), state == CheckState.Checked);
                }
            }
            else if (value != null && targetType.IsAssignableFrom(value.GetType()))
            {
                // If value is already of a compatible type, just go ahead and use it
                return value;
            }

            //
            // If explicit type converters not provided, supply default ones instead
            //

            if (targetConverter is null)
            {
                targetConverter = targetTypeTypeConverter;
            }

            if (sourceConverter is null)
            {
                sourceConverter = sourceTypeTypeConverter;
            }

            //
            // Standardized conversions
            //

            if (targetConverter != null && targetConverter.CanConvertFrom(sourceType))
            {
                return targetConverter.ConvertFrom(null, GetFormatterCulture(formatInfo), value);
            }
            else if (sourceConverter != null && sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(null, GetFormatterCulture(formatInfo), value, targetType);
            }
            else if (value is IConvertible)
            {
                return ChangeType(value, targetType, formatInfo);
            }

            //
            // Fail if no suitable conversion found
            //

            throw new FormatException(GetCantConvertMessage(value, targetType));
        }

        /// <summary>
        ///  Converts a value to the specified type using Convert.ChangeType()
        /// </summary>
        private static object ChangeType(object value, Type type, IFormatProvider formatInfo)
        {
            try
            {
                if (formatInfo is null)
                {
                    formatInfo = CultureInfo.CurrentCulture;
                }

                return Convert.ChangeType(value, type, formatInfo);
            }
            catch (InvalidCastException ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        /// <summary>
        ///  Indicates whether the specified value matches the display-formatted representation of 'null data' for a given binding.
        /// </summary>
        private static bool EqualsFormattedNullValue(object value, object formattedNullValue, IFormatProvider formatInfo)
        {
            if (formattedNullValue is string formattedNullValueStr && value is string valueStr)
            {
                // Use same optimization as in WindowsFormsUtils.SafeCompareStrings(...). This addresses
                if (formattedNullValueStr.Length != valueStr.Length)
                {
                    return false;
                }
                // Always do a case insensitive comparison for strings
                return string.Compare(valueStr, formattedNullValueStr, true, GetFormatterCulture(formatInfo)) == 0;
            }
            else
            {
                // Otherwise perform default comparison based on object types
                return Object.Equals(value, formattedNullValue);
            }
        }

        /// <summary>
        ///  Returns the FormatException message used when formatting/parsing fails to find any suitable conversion
        /// </summary>
        private static string GetCantConvertMessage(object value, Type targetType)
        {
            string stringResId = (value is null) ? SR.Formatter_CantConvertNull : SR.Formatter_CantConvert;
            return string.Format(CultureInfo.CurrentCulture, stringResId, value, targetType.Name);
        }

        /// <summary>
        ///  Determines the correct culture to use during formatting and parsing
        /// </summary>
        private static CultureInfo GetFormatterCulture(IFormatProvider formatInfo)
        {
            if (formatInfo is CultureInfo)
            {
                return formatInfo as CultureInfo;
            }
            else
            {
                return CultureInfo.CurrentCulture;
            }
        }

        /// <summary>
        ///  Converts a value to the specified type using best Parse() method on that type
        /// </summary>
        public static object InvokeStringParseMethod(object value, Type targetType, IFormatProvider formatInfo)
        {
            try
            {
                MethodInfo mi;

                mi = targetType.GetMethod("Parse",
                                        BindingFlags.Public | BindingFlags.Static,
                                        null,
                                        new Type[] { stringType, typeof(NumberStyles), typeof(IFormatProvider) },
                                        null);
                if (mi != null)
                {
                    return mi.Invoke(null, new object[] { (string)value, NumberStyles.Any, formatInfo });
                }

                mi = targetType.GetMethod("Parse",
                                        BindingFlags.Public | BindingFlags.Static,
                                        null,
                                        new Type[] { stringType, typeof(IFormatProvider) },
                                        null);
                if (mi != null)
                {
                    return mi.Invoke(null, new object[] { (string)value, formatInfo });
                }

                mi = targetType.GetMethod("Parse",
                                        BindingFlags.Public | BindingFlags.Static,
                                        null,
                                        new Type[] { stringType },
                                        null);
                if (mi != null)
                {
                    return mi.Invoke(null, new object[] { (string)value });
                }

                return parseMethodNotFound;
            }
            catch (TargetInvocationException ex)
            {
                throw new FormatException(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        ///  Indicates whether a given value represents 'null' for data source fields of the same type.
        /// </summary>
        public static bool IsNullData(object value, object dataSourceNullValue)
        {
            return value is null ||
                   value == System.DBNull.Value ||
                   Object.Equals(value, NullData(value.GetType(), dataSourceNullValue));
        }

        /// <summary>
        ///  Returns the default representation of 'null' for a given data source field type.
        /// </summary>
        public static object NullData(Type type, object dataSourceNullValue)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // For nullable types, null is represented by an instance of that type with no assigned value.
                // The value could also be DBNull.Value (the default for dataSourceNullValue).
                if (dataSourceNullValue is null || dataSourceNullValue == DBNull.Value)
                {
                    // We don't have a special value that represents null on the data source:
                    // use the Nullable<T>'s representation
                    return null;
                }
                else
                {
                    return dataSourceNullValue;
                }
            }
            else
            {
                // For all other types, the default representation of null is defined by
                // the caller (this will usually be System.DBNull.Value for ADO.NET data
                // sources, or a null reference for 'business object' data sources).
                return dataSourceNullValue;
            }
        }

        /// <summary>
        ///  Extract the inner type from a nullable type
        /// </summary>
        private static Type NullableUnwrap(Type type)
        {
            if (type == stringType) // ...performance optimization for the most common case
            {
                return stringType;
            }

            Type underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType ?? type;
        }

        /// <summary>
        ///  Extract the inner type converter from a nullable type converter
        /// </summary>
        private static TypeConverter NullableUnwrap(TypeConverter typeConverter)
        {
            return (typeConverter is NullableConverter nullableConverter) ? nullableConverter.UnderlyingTypeConverter : typeConverter;
        }

        public static object GetDefaultDataSourceNullValue(Type type)
        {
            return (type != null && !type.IsValueType) ? null : defaultDataSourceNullValue;
        }
    }
}
