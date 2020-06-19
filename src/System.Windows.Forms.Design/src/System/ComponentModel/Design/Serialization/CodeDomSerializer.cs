// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Diagnostics;
using System.Globalization;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  The is a base class that can be used to serialize an object graph to a series of
    ///  CodeDom statements.
    /// </summary>
    [DefaultSerializationProvider(typeof(CodeDomSerializationProvider))]
    public class CodeDomSerializer : CodeDomSerializerBase
    {
        private static CodeDomSerializer s_default;
        private static readonly Attribute[] _runTimeFilter = new Attribute[] { DesignOnlyAttribute.No };
        private static readonly Attribute[] _designTimeFilter = new Attribute[] { DesignOnlyAttribute.Yes };
        private static readonly CodeThisReferenceExpression _thisRef = new CodeThisReferenceExpression();

        internal static CodeDomSerializer Default
        {
            get
            {
                if (s_default is null)
                {
                    s_default = new CodeDomSerializer();
                }

                return s_default;
            }
        }

        /// <summary>
        ///  Determines which statement group the given statement should belong to.  The expression parameter
        ///  is an expression that the statement has been reduced to, and targetType represents the type
        ///  of this statement.  This method returns the name of the component this statement should be grouped
        ///  with.
        /// </summary>
        public virtual string GetTargetComponentName(CodeStatement statement, CodeExpression expression, Type targetType)
        {
            string name = null;
            if (expression is CodeVariableReferenceExpression variableReferenceEx)
            {
                name = variableReferenceEx.VariableName;
            }
            else if (expression is CodeFieldReferenceExpression fieldReferenceEx)
            {
                name = fieldReferenceEx.FieldName;
            }
            return name;
        }

        /// <summary>
        ///  Deserilizes the given CodeDom object into a real object.  This
        ///  will use the serialization manager to create objects and resolve
        ///  data types.  The root of the object graph is returned.
        /// </summary>
        public virtual object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            object instance = null;
            if (manager is null || codeObject is null)
            {
                throw new ArgumentNullException(manager is null ? "manager" : "codeObject");
            }

            using (TraceScope("CodeDomSerializer::Deserialize"))
            {
                // What is the code object?  We support an expression, a statement or a collection of statements
                if (codeObject is CodeExpression expression)
                {
                    instance = DeserializeExpression(manager, null, expression);
                }
                else
                {
                    if (codeObject is CodeStatementCollection statements)
                    {
                        foreach (CodeStatement element in statements)
                        {
                            // If we do not yet have an instance, we will need to pick through the  statements and see if we can find one.
                            if (instance is null)
                            {
                                instance = DeserializeStatementToInstance(manager, element);
                                if (instance != null)
                                {
                                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(instance, new Attribute[] { BrowsableAttribute.Yes });
                                    foreach (PropertyDescriptor prop in props)
                                    {
                                        if (!prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden) &&
                                            prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content) &&
                                            !(manager.GetSerializer(prop.PropertyType, typeof(CodeDomSerializer)) is CollectionCodeDomSerializer))
                                        {
                                            ResetBrowsableProperties(prop.GetValue(instance));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DeserializeStatement(manager, element);
                            }
                        }
                    }
                    else
                    {
                        if (!(codeObject is CodeStatement statement))
                        {
                            Debug.Fail("CodeDomSerializer::Deserialize requires a CodeExpression, CodeStatement or CodeStatementCollection to parse");
                            string supportedTypes = string.Format(CultureInfo.CurrentCulture, "{0}, {1}, {2}", typeof(CodeExpression).Name, typeof(CodeStatement).Name, typeof(CodeStatementCollection).Name);
                            throw new ArgumentException(string.Format(SR.SerializerBadElementTypes, codeObject.GetType().Name, supportedTypes));
                        }
                    }
                }
            }
            return instance;
        }

        /// <summary>
        ///  This method deserializes a single statement.  It is equivalent of calling
        ///  DeserializeStatement, except that it returns an object instance if the
        ///  resulting statement was a variable assign statement, a variable
        ///  declaration with an init expression, or a field assign statement.
        /// </summary>
        protected object DeserializeStatementToInstance(IDesignerSerializationManager manager, CodeStatement statement)
        {
            object instance = null;
            if (statement is CodeAssignStatement assign)
            {
                if (assign.Left is CodeFieldReferenceExpression fieldRef)
                {
                    Trace("Assigning instance to field {0}", fieldRef.FieldName);
                    instance = DeserializeExpression(manager, fieldRef.FieldName, assign.Right);
                }
                else
                {
                    if (assign.Left is CodeVariableReferenceExpression varRef)
                    {
                        Trace("Assigning instance to variable {0}", varRef.VariableName);
                        instance = DeserializeExpression(manager, varRef.VariableName, assign.Right);
                    }
                    else
                    {
                        DeserializeStatement(manager, assign);
                    }
                }
            }
            else if (statement is CodeVariableDeclarationStatement varDecl && varDecl.InitExpression != null)
            {
                Trace("Initializing variable declaration for variable {0}", varDecl.Name);
                instance = DeserializeExpression(manager, varDecl.Name, varDecl.InitExpression);
            }
            else
            {
                // This statement isn't one that will return a named object.  Deserialize it normally.
                DeserializeStatement(manager, statement);
            }
            return instance;
        }

        /// <summary>
        ///  Serializes the given object into a CodeDom object.
        /// </summary>
        public virtual object Serialize(IDesignerSerializationManager manager, object value)
        {
            object result = null;
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (TraceScope("CodeDomSerializer::" + nameof(Serialize)))
            {
                Trace("Type: {0}", value.GetType().Name);

                if (value is Type)
                {
                    result = new CodeTypeOfExpression((Type)value);
                }
                else
                {
                    bool isComplete = false;
                    bool isPreset;
                    CodeExpression expression = SerializeCreationExpression(manager, value, out bool isCompleteExpression);
                    // if the object is not a component we will honor the return value from SerializeCreationExpression.  For compat reasons we ignore the value if the object is a component.
                    if (!(value is IComponent))
                    {
                        isComplete = isCompleteExpression;
                    }

                    // We need to find out if SerializeCreationExpression returned a preset expression.
                    if (manager.Context[typeof(ExpressionContext)] is ExpressionContext cxt && object.ReferenceEquals(cxt.PresetValue, value))
                    {
                        isPreset = true;
                    }
                    else
                    {
                        isPreset = false;
                    }

                    TraceIf(expression is null, "Unable to create object; aborting.");
                    // Short circuit common cases
                    if (expression != null)
                    {
                        if (isComplete)
                        {
                            Trace("Single expression : {0}", expression);
                            result = expression;
                        }
                        else
                        {
                            // Ok, we have an incomplete expression. That means we've created the object but we will need to set properties on it to configure it.  Therefore, we need to have a variable reference to it unless we were given a preset expression already.
                            CodeStatementCollection statements = new CodeStatementCollection();

                            if (isPreset)
                            {
                                SetExpression(manager, value, expression, true);
                            }
                            else
                            {
                                CodeExpression variableReference;
                                string varName = GetUniqueName(manager, value);
                                string varTypeName = TypeDescriptor.GetClassName(value);

                                CodeVariableDeclarationStatement varDecl = new CodeVariableDeclarationStatement(varTypeName, varName);
                                Trace("Generating local : {0}", varName);
                                varDecl.InitExpression = expression;
                                statements.Add(varDecl);
                                variableReference = new CodeVariableReferenceExpression(varName);
                                SetExpression(manager, value, variableReference);
                            }
                            // Finally, we need to walk properties and events for this object
                            SerializePropertiesToResources(manager, statements, value, _designTimeFilter);
                            SerializeProperties(manager, statements, value, _runTimeFilter);
                            SerializeEvents(manager, statements, value, _runTimeFilter);
                            result = statements;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  Serializes the given object into a CodeDom object.
        /// </summary>
        public virtual object SerializeAbsolute(IDesignerSerializationManager manager, object value)
        {
            object data;
            SerializeAbsoluteContext abs = new SerializeAbsoluteContext();
            manager.Context.Push(abs);
            try
            {
                data = Serialize(manager, value);
            }
            finally
            {
                Debug.Assert(manager.Context.Current == abs, "Serializer added a context it didn't remove.");
                manager.Context.Pop();
            }
            return data;
        }

        /// <summary>
        ///  This serializes the given member on the given object.
        /// </summary>
        public virtual CodeStatementCollection SerializeMember(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (owningObject is null)
            {
                throw new ArgumentNullException(nameof(owningObject));
            }

            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            CodeStatementCollection statements = new CodeStatementCollection();
            // See if we have an existing expression for this member.  If not, fabricate one
            CodeExpression expression = GetExpression(manager, owningObject);
            if (expression is null)
            {
                string name = GetUniqueName(manager, owningObject);
                expression = new CodeVariableReferenceExpression(name);
                SetExpression(manager, owningObject, expression);
            }

            if (member is PropertyDescriptor property)
            {
                SerializeProperty(manager, statements, owningObject, property);
            }
            else
            {
                if (member is EventDescriptor evt)
                {
                    SerializeEvent(manager, statements, owningObject, evt);
                }
                else
                {
                    throw new NotSupportedException(string.Format(SR.SerializerMemberTypeNotSerializable, member.GetType().FullName));
                }
            }
            return statements;
        }

        /// <summary>
        ///  This serializes the given member on the given object.
        /// </summary>
        public virtual CodeStatementCollection SerializeMemberAbsolute(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (owningObject is null)
            {
                throw new ArgumentNullException(nameof(owningObject));
            }

            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            CodeStatementCollection statements;
            SerializeAbsoluteContext abs = new SerializeAbsoluteContext(member);
            manager.Context.Push(abs);

            try
            {
                statements = SerializeMember(manager, owningObject, member);
            }
            finally
            {
                Debug.Assert(manager.Context.Current == abs, "Serializer added a context it didn't remove.");
                manager.Context.Pop();
            }
            return statements;
        }

        /// <summary>
        ///  This serializes the given value to an expression.  It will return null if the value could not be
        ///  serialized.  This is similar to SerializeToExpression, except that it will stop
        ///  if it cannot obtain a simple reference expression for the value.  Call this method
        ///  when you expect the resulting expression to be used as a parameter or target
        ///  of a statement.
        /// </summary>
        [Obsolete("This method has been deprecated. Use SerializeToExpression or GetExpression instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        protected CodeExpression SerializeToReferenceExpression(IDesignerSerializationManager manager, object value)
        {
            CodeExpression expression = null;
            using (TraceScope("CodeDomSerializer::" + nameof(SerializeToReferenceExpression)))
            {
                // First - try GetExpression
                expression = GetExpression(manager, value);
                // Next, we check for a named IComponent, and return a reference to it.
                if (expression is null && value is IComponent)
                {
                    string name = manager.GetName(value);
                    bool referenceName = false;
                    if (name is null)
                    {
                        IReferenceService referenceService = (IReferenceService)manager.GetService(typeof(IReferenceService));
                        if (referenceService != null)
                        {
                            name = referenceService.GetName(value);
                            referenceName = name != null;
                        }
                    }

                    if (name != null)
                    {
                        Trace("Object is reference ({0}) Creating reference expression", name);
                        // Check to see if this is a reference to the root component.  If it is, then use "this".
                        RootContext root = (RootContext)manager.Context[typeof(RootContext)];
                        if (root != null && root.Value == value)
                        {
                            expression = root.Expression;
                        }
                        else if (referenceName && name.IndexOf('.') != -1)
                        {
                            // if it's a reference name with a dot, we've actually got a property here...
                            int dotIndex = name.IndexOf('.');
                            expression = new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(_thisRef, name.Substring(0, dotIndex)), name.Substring(dotIndex + 1));
                        }
                        else
                        {
                            expression = new CodeFieldReferenceExpression(_thisRef, name);
                        }
                    }
                }
            }
            return expression;
        }
        private void ResetBrowsableProperties(object instance)
        {
            if (instance is null)
            {
                return;
            }

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(instance, new Attribute[] { BrowsableAttribute.Yes });
            foreach (PropertyDescriptor prop in props)
            {
                if (!prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden))
                {
                    if (prop.CanResetValue(instance))
                    {
                        try
                        {
                            prop.ResetValue(instance);
                        }
                        catch (ArgumentException e)
                        {
                            Debug.Assert(false, e.Message);
                        }
                    }
                    else if (prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content))
                    {
                        ResetBrowsableProperties(prop.GetValue(instance));
                    }
                }
            }
        }
    }
}
