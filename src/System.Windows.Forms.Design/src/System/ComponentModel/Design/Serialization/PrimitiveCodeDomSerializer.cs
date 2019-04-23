// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization {
    
    /// <summary>
    ///     Code model serializer for primitive types.
    /// </summary>
    internal class PrimitiveCodeDomSerializer : CodeDomSerializer {
    
        private static readonly string JSharpFileExtension = ".jsl";
        private static PrimitiveCodeDomSerializer defaultSerializer;
        
        /// <summary>
        ///     Retrieves a default static instance of this serializer.
        /// </summary>
        internal new static PrimitiveCodeDomSerializer Default {
            get {
                if (defaultSerializer == null) {
                    defaultSerializer = new PrimitiveCodeDomSerializer();
                }
                return defaultSerializer;
            }
        }
        
        /// <summary>
        ///     Serializes the given object into a CodeDom object.
        /// </summary>
        public override object Serialize(IDesignerSerializationManager manager, object value) {
            using (TraceScope("PrimitiveCodeDomSerializer::Serialize")) {
                Trace("Value: {0}",  (value == null ? "(null)" : value.ToString()));
            }

            CodeExpression expression = new CodePrimitiveExpression(value);

            if (value != null) {
                if (value is bool || value is char || value is int || value is float || value is double) {
                    
                    // Hack for J#, since they don't support auto-boxing of value types yet.
                    CodeDomProvider codeProvider = manager.GetService(typeof(CodeDomProvider)) as CodeDomProvider;
                    if (codeProvider != null && String.Equals(codeProvider.FileExtension, JSharpFileExtension)) {
                        // See if we are boxing - if so, insert a cast.
                        ExpressionContext cxt = manager.Context[typeof(ExpressionContext)] as ExpressionContext;
                        Debug.Assert(cxt != null, "No expression context on stack - J# boxing cast will not be inserted");
                        if (cxt != null) {
                            if (cxt.ExpressionType == typeof(object)) {
                                expression = new CodeCastExpression(value.GetType(), expression);
                                expression.UserData.Add("CastIsBoxing", true);
                            }
                        }
                    }
                }
                else if (value is string) {
                    string stringValue = value as string;
                    if (stringValue != null && stringValue.Length > 200) {
                        expression = SerializeToResourceExpression(manager, stringValue);
                    }
                }
                else {
                    // Generate a cast for all other types because we won't parse them properly otherwise 
                    // because we won't know to convert them to the narrow form.
                    expression = new CodeCastExpression(new CodeTypeReference(value.GetType()), expression);
                }
            }

            return expression;
        }
    }
}

