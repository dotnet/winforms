// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms
{
    public class ListBindingConverter : TypeConverter
    {
        private static Type[] ctorTypes = null;  // the list of type of our ctor parameters.
        private static string[] ctorParamProps = null; // the name of each property to check to see if we need to init with a ctor.

        /// <summary>
        ///  Creates our array of types on demand.
        /// </summary>
        private static Type[] ConstructorParamaterTypes
        {
            get
            {
                if (ctorTypes == null)
                {
                    ctorTypes = new Type[] { typeof(string), typeof(object), typeof(string), typeof(bool), typeof(DataSourceUpdateMode), typeof(object), typeof(string), typeof(IFormatProvider) };
                }
                return ctorTypes;
            }
        }

        /// <summary>
        ///  Creates our array of param names on demand.
        /// </summary>
        private static string[] ConstructorParameterProperties
        {
            get
            {
                if (ctorParamProps == null)
                {
                    ctorParamProps = new string[] { null, null, null, "FormattingEnabled", "DataSourceUpdateMode", "NullValue", "FormatString", "FormatInfo", };
                }
                return ctorParamProps;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether this converter can
        ///  convert an object to the given destination type using the context.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///  Converts the given object to another type.  The most common types to convert
        ///  are to and from a string object.  The default implementation will make a call
        ///  to ToString on the object if the object is valid and if the destination
        ///  type is string.  If this cannot convert to the desitnation type, this will
        ///  throw a NotSupportedException.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(InstanceDescriptor) && value is Binding)
            {
                Binding b = (Binding)value;
                return GetInstanceDescriptorFromValues(b);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///  Creates an instance of this type given a set of property values
        ///  for the object.  This is useful for objects that are immutable, but still
        ///  want to provide changable properties.
        /// </summary>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            try
            {
                return new Binding((string)propertyValues["PropertyName"],
                                           propertyValues["DataSource"],
                                   (string)propertyValues["DataMember"]);
            }
            catch (InvalidCastException invalidCast)
            {
                throw new ArgumentException(SR.PropertyValueInvalidEntry, invalidCast);
            }
            catch (NullReferenceException nullRef)
            {
                throw new ArgumentException(SR.PropertyValueInvalidEntry, nullRef);
            }
        }

        /// <summary>
        ///  Determines if changing a value on this object should require a call to
        ///  CreateInstance to create a new value.
        /// </summary>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///  Gets the best matching ctor for a given binding and fills it out, based on the
        ///  state of the Binding and the optimal ctor.
        /// </summary>
        private InstanceDescriptor GetInstanceDescriptorFromValues(Binding b)
        {
            // The BindingFormattingDialog turns on Binding::FormattingEnabled property.
            // however, when the user data binds a property using the PropertyBrowser, Binding::FormattingEnabled is set to false
            // The Binding class is not a component class, so we don't have the ComponentInitialize method where we can set FormattingEnabled to true
            // so we set it here.
            b.FormattingEnabled = true;

            bool isComplete = true;
            int lastItem = ConstructorParameterProperties.Length - 1;

            for (; lastItem >= 0; lastItem--)
            {

                // null means no prop is available, we quit here.
                //
                if (ConstructorParameterProperties[lastItem] == null)
                {
                    break;
                }

                // get the property and see if it needs to be serialized.
                //
                PropertyDescriptor prop = TypeDescriptor.GetProperties(b)[ConstructorParameterProperties[lastItem]];
                if (prop != null && prop.ShouldSerializeValue(b))
                {
                    break;
                }
            }

            // now copy the type array up to the point we quit.
            //
            Type[] ctorParams = new Type[lastItem + 1];
            Array.Copy(ConstructorParamaterTypes, 0, ctorParams, 0, ctorParams.Length);

            // Get the ctor info.
            //
            ConstructorInfo ctor = typeof(Binding).GetConstructor(ctorParams);
            Debug.Assert(ctor != null, "Failed to find Binding ctor for types!");
            if (ctor == null)
            {
                isComplete = false;
                ctor = typeof(Binding).GetConstructor(new Type[] {
                   typeof(string),
                   typeof(object),
                   typeof(string)});
            }

            // now fill in the values.
            //
            object[] values = new object[ctorParams.Length];

            for (int i = 0; i < values.Length; i++)
            {
                object val = null;
                switch (i)
                {
                    case 0:
                        val = b.PropertyName;
                        break;
                    case 1:
                        val = b.DataSource;
                        break;
                    case 2:
                        val = b.BindingMemberInfo.BindingMember;
                        break;
                    default:
                        val = TypeDescriptor.GetProperties(b)[ConstructorParameterProperties[i]].GetValue(b);
                        break;
                }
                values[i] = val;
            }
            return new InstanceDescriptor(ctor, values, isComplete);
        }
    }
}
