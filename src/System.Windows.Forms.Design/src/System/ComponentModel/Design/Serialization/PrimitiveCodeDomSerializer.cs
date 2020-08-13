// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  Code model serializer for primitive types.
    /// </summary>
    internal class PrimitiveCodeDomSerializer : CodeDomSerializer
    {
        private static PrimitiveCodeDomSerializer s_defaultSerializer;

        /// <summary>
        ///  Retrieves a default static instance of this serializer.
        /// </summary>
        internal new static PrimitiveCodeDomSerializer Default
        {
            get
            {
                if (s_defaultSerializer is null)
                {
                    s_defaultSerializer = new PrimitiveCodeDomSerializer();
                }
                return s_defaultSerializer;
            }
        }

        /// <summary>
        ///  Serializes the given object into a CodeDom object.
        /// </summary>
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            using (TraceScope("PrimitiveCodeDomSerializer::" + nameof(Serialize)))
            {
                Trace("Value: {0}", (value is null ? "(null)" : value.ToString()));
            }

            CodeExpression expression = new CodePrimitiveExpression(value);

            if (value is null)
            {
                return expression;
            }

            if (value is string stringValue)
            {
                if (stringValue.Length > 200)
                {
                    expression = SerializeToResourceExpression(manager, stringValue);
                }
                return expression;
            }

            if (!(value is bool || value is char || value is int || value is float || value is double))
            {
                // Generate a cast for all other types because we won't parse them properly otherwise
                // because we won't know to convert them to the narrow form.
                expression = new CodeCastExpression(new CodeTypeReference(value.GetType()), expression);
            }

            return expression;
        }
    }
}
