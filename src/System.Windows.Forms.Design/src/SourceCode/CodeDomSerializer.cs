// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///     The is a base class that can be used to serialize an object graph to a series of
    ///     CodeDom statements.
    /// </summary>
    [DefaultSerializationProvider(typeof(CodeDomSerializationProvider))]
    public class CodeDomSerializer : CodeDomSerializerBase
    {
        /// <summary>
        ///     Determines which statement group the given statement should belong to.  The expression parameter
        ///     is an expression that the statement has been reduced to, and targetType represents the type
        ///     of this statement.  This method returns the name of the component this statement should be grouped
        ///     with.
        /// </summary>
        public virtual string GetTargetComponentName(CodeStatement statement, CodeExpression expression,
            Type targetType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Deserilizes the given CodeDom object into a real object.  This
        ///     will use the serialization manager to create objects and resolve
        ///     data types.  The root of the object graph is returned.
        /// </summary>
        public virtual object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method deserializes a single statement.  It is equivalent of calling
        ///     DeserializeStatement, except that it returns an object instance if the
        ///     resulting statement was a variable assign statement, a variable
        ///     declaration with an init expression, or a field assign statement.
        /// </summary>
        protected object DeserializeStatementToInstance(IDesignerSerializationManager manager, CodeStatement statement)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Serializes the given object into a CodeDom object.
        /// </summary>
        public virtual object Serialize(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Serializes the given object into a CodeDom object.
        /// </summary>
        public virtual object SerializeAbsolute(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This serializes the given member on the given object.
        /// </summary>
        public virtual CodeStatementCollection SerializeMember(IDesignerSerializationManager manager,
            object owningObject, MemberDescriptor member)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This serializes the given member on the given object.
        /// </summary>
        public virtual CodeStatementCollection SerializeMemberAbsolute(IDesignerSerializationManager manager,
            object owningObject, MemberDescriptor member)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This serializes the given value to an expression.  It will return null if the value could not be
        ///     serialized.  This is similar to SerializeToExpression, except that it will stop
        ///     if it cannot obtain a simple reference expression for the value.  Call this method
        ///     when you expect the resulting expression to be used as a parameter or target
        ///     of a statement.
        /// </summary>
        [Obsolete(
            "This method has been deprecated. Use SerializeToExpression or GetExpression instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        protected CodeExpression SerializeToReferenceExpression(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
