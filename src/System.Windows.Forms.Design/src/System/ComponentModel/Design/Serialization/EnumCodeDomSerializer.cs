// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  Code model serializer for enum types.
    /// </summary>
    internal class EnumCodeDomSerializer : CodeDomSerializer
    {
        private static EnumCodeDomSerializer s_defaultSerializer;

        /// <summary>
        ///  Retrieves a default static instance of this serializer.
        /// </summary>
        internal new static EnumCodeDomSerializer Default
        {
            get
            {
                if (s_defaultSerializer == null)
                {
                    s_defaultSerializer = new EnumCodeDomSerializer();
                }
                return s_defaultSerializer;
            }
        }

        /// <summary>
        ///  Serializes the given object into a CodeDom object.
        /// </summary>
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            CodeExpression expression = null;

            using (TraceScope("EnumCodeDomSerializer::" + nameof(Serialize)))
            {
                Trace("Type: {0}", (value == null ? "(null)" : value.GetType().Name));
                if (value is Enum)
                {
                    bool needCast = false;
                    Enum[] values;
                    TypeConverter converter = TypeDescriptor.GetConverter(value);
                    if (converter != null && converter.CanConvertTo(typeof(Enum[])))
                    {
                        values = (Enum[])converter.ConvertTo(value, typeof(Enum[]));
                        needCast = (values.Length > 1);
                    }
                    else
                    {
                        values = new Enum[] { (Enum)value };
                        needCast = true;
                    }

                    // EnumConverter (and anything that is overridden to support enums)
                    // should be providing us a conversion to Enum[] for flag styles.
                    // If it doesn't, we will emit a warning and just do a cast from the enum value.

                    CodeTypeReferenceExpression enumType = new CodeTypeReferenceExpression(value.GetType());

                    // If names is of length 1, then this is a simple field reference. Otherwise,
                    // it is an or-d combination of expressions.
                    //
                    TraceIf(values.Length == 1, "Single field entity.");
                    TraceIf(values.Length > 1, "Multi field entity.");

                    // We now need to serialize the enum terms as fields. We new up an EnumConverter to do
                    // that. We cannot use the type's own converter since it might have a different string
                    // representation for its values. Hardcoding is okay in this case, since all we want is
                    // the enum's field name. Simply doing ToString() will not give us any validation.
                    TypeConverter enumConverter = new EnumConverter(value.GetType());
                    foreach (Enum term in values)
                    {
                        string termString = enumConverter?.ConvertToString(term);
                        CodeExpression newExpression = !String.IsNullOrEmpty(termString) ? new CodeFieldReferenceExpression(enumType, termString) : null;

                        if (newExpression != null)
                        {
                            if (expression == null)
                            {
                                expression = newExpression;
                            }
                            else
                            {
                                expression = new CodeBinaryOperatorExpression(expression, CodeBinaryOperatorType.BitwiseOr, newExpression);
                            }
                        }
                    }

                    // If we had to combine multiple names, wrap the result in an appropriate cast.
                    //
                    if (expression != null && needCast)
                    {
                        expression = new CodeCastExpression(value.GetType(), expression);
                    }
                }
                else
                {
                    Debug.Fail("Enum serializer called for non-enum object.");
                    TraceError("Enum serializer called for non-enum object {0}", (value == null ? "(null)" : value.GetType().Name));
                }
            }

            return expression;
        }
    }
}
