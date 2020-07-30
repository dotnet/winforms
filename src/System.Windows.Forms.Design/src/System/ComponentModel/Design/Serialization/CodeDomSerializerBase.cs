// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This base class is used as a shared base between CodeDomSerializer and TypeCodeDomSerializer.
    ///  It is not meant to be publicly derived from.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class CodeDomSerializerBase
    {
        private static readonly Attribute[] runTimeProperties = new Attribute[] { DesignOnlyAttribute.No };
        private static readonly TraceSwitch traceSerialization = new TraceSwitch("DesignerSerialization", "Trace design time serialization");
#pragma warning disable CS0649
        private static Stack traceScope;
#pragma warning restore CS0649
        /// <summary>
        ///  Internal constructor so only we can derive from this class.
        /// </summary>
        internal CodeDomSerializerBase()
        {
        }

        /// <summary>
        ///  This method is invoked during deserialization to obtain an instance of an object.  When this is called, an instance
        ///  of the requested type should be returned.  The default implementation invokes manager.CreateInstance.
        /// </summary>
        protected virtual object DeserializeInstance(IDesignerSerializationManager manager, Type type, object[] parameters, string name, bool addToContainer)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return manager.CreateInstance(type, parameters, name, addToContainer);
        }

        /// <summary>
        ///  This routine returns the correct typename given a CodeTypeReference.  It expands the child typenames
        ///  and builds up the clr formatted generic name.  If its not a generic, it just returns BaseType.
        /// </summary>
        internal static string GetTypeNameFromCodeTypeReference(IDesignerSerializationManager manager, CodeTypeReference typeref)
        {
            //we do this to avoid an extra gettype for the usual nongeneric case.
            if (typeref.TypeArguments is null || typeref.TypeArguments.Count == 0)
            {
                return typeref.BaseType;
            }

            return GetTypeNameFromCodeTypeReferenceHelper(manager, typeref);
        }
        private static string GetTypeNameFromCodeTypeReferenceHelper(IDesignerSerializationManager manager, CodeTypeReference typeref)
        {
            if (typeref.TypeArguments is null || typeref.TypeArguments.Count == 0)
            {
                Type t = manager.GetType(typeref.BaseType);
                //we use the assemblyqualifiedname where we can so that GetType will find it correctly.
                if (t != null)
                {
                    // get type which exists in the target framework if any
                    return GetReflectionTypeFromTypeHelper(manager, t).AssemblyQualifiedName;
                }
                return typeref.BaseType;
            }

            //create the MyGeneric`2[ part
            StringBuilder typename = new StringBuilder(typeref.BaseType);
            if (!typeref.BaseType.Contains('`'))
            {
                typename.Append('`');
                typename.Append(typeref.TypeArguments.Count);
            }
            typename.Append('[');

            bool first = true;

            //now create each sub-argument part.
            foreach (CodeTypeReference childref in typeref.TypeArguments)
            {
                if (!first)
                {
                    typename.Append(',');
                }
                typename.Append('[');
                typename.Append(GetTypeNameFromCodeTypeReferenceHelper(manager, childref));
                typename.Append(']');
                first = false;
            }
            typename.Append(']');

            //otherwise, we have a generic and we need to format it.
            return typename.ToString();
        }

        /// <summary>
        ///  Return a target framework-aware TypeDescriptionProvider which can be used for type filtering
        /// </summary>
        protected static TypeDescriptionProvider GetTargetFrameworkProvider(IServiceProvider provider, object instance)
        {
            // service will be null outside the VisualStudio
            if (provider.GetService(typeof(TypeDescriptionProviderService)) is TypeDescriptionProviderService service)
            {
                return service.GetProvider(instance);
            }
            return null;
        }

        /// <summary>
        ///  Get a faux type which is generated from the metadata, which is
        ///  looked up on the target framerwork assembly. Be careful to not use mix
        ///  this type with runtime types in comparisons!
        /// </summary>
        protected static Type GetReflectionTypeFromTypeHelper(IDesignerSerializationManager manager, Type type)
        {
            if (type is null || manager is null)
            {
                Debug.Fail("GetReflectionTypeFromTypeHelper does not accept null arguments.");
                return null;
            }

            TypeDescriptionProvider targetProvider = GetTargetFrameworkProviderForType(manager, type);
            if (targetProvider != null)
            {
                if (targetProvider.IsSupportedType(type))
                {
                    return targetProvider.GetReflectionType(type);
                }
                Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, type.FullName), SR.SerializerUndeclaredName);
            }
            return TypeDescriptor.GetReflectionType(type);
        }

        internal static void Error(IDesignerSerializationManager manager, string exceptionText, string helpLink)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (exceptionText is null)
            {
                throw new ArgumentNullException(nameof(exceptionText));
            }

            CodeStatement statement = (CodeStatement)manager.Context[typeof(CodeStatement)];
            CodeLinePragma linePragma = null;
            if (statement != null)
            {
                linePragma = statement.LinePragma;
            }

            Exception exception = new CodeDomSerializerException(exceptionText, linePragma)
            {
                HelpLink = helpLink
            };
            throw exception;
        }

        private static TypeDescriptionProvider GetTargetFrameworkProviderForType(IServiceProvider provider, Type type)
        {
            // service will be null outside the VisualStudio
            if (provider.GetService(typeof(TypeDescriptionProviderService)) is TypeDescriptionProviderService service)
            {
                return service.GetProvider(type);
            }
            return null;
        }

        /// <summary>
        ///  Get a faux type which is generated based on the metadata
        ///  looked up on the target framework assembly.
        ///  Never pass a type returned from GetReflectionType to runtime APIs that need a type.
        ///  Call GetRuntimeType first to unwrap the reflection type.
        /// </summary>
        protected static Type GetReflectionTypeHelper(IDesignerSerializationManager manager, object instance)
        {
            if (instance is null || manager is null)
            {
                Debug.Fail("GetReflectionTypeHelper does not accept null arguments.");
                return null;
            }

            Type type = instance.GetType();
            if (type.IsValueType)
            {
                TypeDescriptionProvider targetProvider = GetTargetFrameworkProvider(manager, instance);
                if (targetProvider != null)
                {
                    if (targetProvider.IsSupportedType(type))
                    {
                        return targetProvider.GetReflectionType(instance);
                    }
                    Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, instance.GetType().FullName), SR.SerializerUndeclaredName);
                }
            }

            return TypeDescriptor.GetReflectionType(instance);
        }

        /// <summary>
        ///  Get properties collection as defined in the project target framework
        /// </summary>
        protected static PropertyDescriptorCollection GetPropertiesHelper(IDesignerSerializationManager manager, object instance, Attribute[] attributes)
        {
            if (instance is null || manager is null)
            {
                Debug.Fail("GetPropertiesHelper does not accept null arguments.");
                return null;
            }

            if (instance.GetType().IsValueType)
            {
                TypeDescriptionProvider targetProvider = GetTargetFrameworkProvider(manager, instance);
                if (targetProvider != null)
                {
                    // target framework provider is null at runtime
                    if (targetProvider.IsSupportedType(instance.GetType()))
                    {
                        ICustomTypeDescriptor targetAwareDescriptor = targetProvider.GetTypeDescriptor(instance);
                        if (targetAwareDescriptor != null)
                        {
                            if (attributes is null)
                            {
                                return targetAwareDescriptor.GetProperties();
                            }
                            return targetAwareDescriptor.GetProperties(attributes);
                        }
                    }
                    else
                    {
                        Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, instance.GetType().FullName), SR.SerializerUndeclaredName);
                    }
                }
            }

            if (attributes is null)
            {
                return TypeDescriptor.GetProperties(instance);
            }
            return TypeDescriptor.GetProperties(instance, attributes);
        }

        /// <summary>
        ///  Get events collection as defined in the project target framework
        /// </summary>
        protected static EventDescriptorCollection GetEventsHelper(IDesignerSerializationManager manager, object instance, Attribute[] attributes)
        {
            if (instance is null || manager is null)
            {
                Debug.Fail("GetEventsHelper does not accept null arguments.");
                return null;
            }

            if (instance.GetType().IsValueType)
            {
                TypeDescriptionProvider targetProvider = GetTargetFrameworkProvider(manager, instance);
                if (targetProvider != null)
                {
                    if (targetProvider.IsSupportedType(instance.GetType()))
                    {
                        ICustomTypeDescriptor targetAwareDescriptor = targetProvider.GetTypeDescriptor(instance);
                        if (targetAwareDescriptor != null)
                        {
                            if (attributes is null)
                            {
                                return targetAwareDescriptor.GetEvents();
                            }
                            return targetAwareDescriptor.GetEvents(attributes);
                        }
                    }
                    else
                    {
                        Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, instance.GetType().FullName), SR.SerializerUndeclaredName);
                    }
                }
            }

            if (attributes is null)
            {
                return TypeDescriptor.GetEvents(instance);
            }

            return TypeDescriptor.GetEvents(instance, attributes);
        }

        /// <summary>
        ///  Get attributes collection as defined in the project target framework
        /// </summary>
        protected static AttributeCollection GetAttributesHelper(IDesignerSerializationManager manager, object instance)
        {
            if (instance is null || manager is null)
            {
                Debug.Fail("GetAttributesHelper does not accept null arguments.");
                return null;
            }
            if (instance.GetType().IsValueType)
            {
                TypeDescriptionProvider targetProvider = GetTargetFrameworkProvider(manager, instance);
                if (targetProvider != null)
                {
                    if (targetProvider.IsSupportedType(instance.GetType()))
                    {
                        ICustomTypeDescriptor targetAwareDescriptor = targetProvider.GetTypeDescriptor(instance);
                        if (targetAwareDescriptor != null)
                        {
                            return targetAwareDescriptor.GetAttributes();
                        }
                    }
                    else
                    {
                        Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, instance.GetType().FullName), SR.SerializerUndeclaredName);
                    }
                }
            }
            return TypeDescriptor.GetAttributes(instance);
        }

        /// <summary>
        ///  Get attributes collection as defined in the project target framework
        /// </summary>
        protected static AttributeCollection GetAttributesFromTypeHelper(IDesignerSerializationManager manager, Type type)
        {
            if (type is null || manager is null)
            {
                Debug.Fail("GetAttributesFromTypeHelper does not accept null arguments.");
                return null;
            }

            if (type.IsValueType)
            {
                TypeDescriptionProvider targetProvider = GetTargetFrameworkProviderForType(manager, type);
                if (targetProvider != null)
                {
                    if (targetProvider.IsSupportedType(type))
                    {
                        ICustomTypeDescriptor targetAwareDescriptor = targetProvider.GetTypeDescriptor(type);
                        if (targetAwareDescriptor != null)
                        {
                            return targetAwareDescriptor.GetAttributes();
                        }
                    }
                    else
                    {
                        Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, type.FullName), SR.SerializerUndeclaredName);
                    }
                }
            }
            return TypeDescriptor.GetAttributes(type);
        }

        /// <summary>
        ///  This method will inspect all of the properties on the given object fitting the filter, and check for that
        ///  property in a resource blob.  This is useful for deserializing properties that cannot be represented
        ///  in code, such as design-time properties.
        /// </summary>
        protected void DeserializePropertiesFromResources(IDesignerSerializationManager manager, object value, Attribute[] filter)
        {
            using (TraceScope("ComponentCodeDomSerializerBase::DeserializePropertiesFromResources"))
            {
                // It is much faster to dig through the resources first, and then map these resources to properties than it is to filter properties at each turn.  Why?  Because filtering properties requires a separate filter call for each object (because designers get a chance to filter, the cache is per-component), while resources are loaded once per document.
                IDictionaryEnumerator de = ResourceCodeDomSerializer.Default.GetMetadataEnumerator(manager);
                if (de is null)
                {
                    de = ResourceCodeDomSerializer.Default.GetEnumerator(manager, CultureInfo.InvariantCulture);
                }

                if (de != null)
                {
                    string ourObjectName;
                    if (manager.Context[typeof(RootContext)] is RootContext root && root.Value == value)
                    {
                        ourObjectName = "$this";
                    }
                    else
                    {
                        ourObjectName = manager.GetName(value);
                    }

                    PropertyDescriptorCollection ourProperties = GetPropertiesHelper(manager, value, null);
                    while (de.MoveNext())
                    {
                        string resourceName = de.Key as string;
                        Debug.Assert(resourceName != null, "non-string keys in dictionary entry");
                        int dotIndex = resourceName.IndexOf('.');
                        if (dotIndex == -1)
                        {
                            continue;
                        }

                        string objectName = resourceName.Substring(0, dotIndex);
                        // Skip now if this isn't a value for our object.
                        if (!objectName.Equals(ourObjectName))
                        {
                            continue;
                        }

                        string propertyName = resourceName.Substring(dotIndex + 1);

                        // Now locate the property by this name.
                        PropertyDescriptor property = ourProperties[propertyName];
                        if (property is null)
                        {
                            continue;
                        }

                        // This property must have matching attributes.
                        bool passFilter = true;
                        if (filter != null)
                        {
                            AttributeCollection propAttributes = property.Attributes;
                            foreach (Attribute a in filter)
                            {
                                if (!propAttributes.Contains(a))
                                {
                                    passFilter = false;
                                    break;
                                }
                            }
                        }

                        // If this property passes inspection, then set it.
                        if (passFilter)
                        {
                            object resourceObject = de.Value;
                            Trace("Resource: {0}, value: {1}", resourceName, (resourceObject ?? "(null)"));
                            try
                            {
                                property.SetValue(value, resourceObject);
                            }
                            catch (Exception e)
                            {
                                manager.ReportError(e);
                            }
                        }
                    }
                }
            }
        }
        internal static IDisposable TraceScope(string name)
        {
#if DEBUG
            if (traceScope is null)
            {
                traceScope = new Stack();
            }

            Trace(name);
            traceScope.Push(name);
#endif
            return new TracingScope();
        }

        [Conditional("DEBUG")]
        internal static void TraceIf(bool condition, string message, params object[] values)
        {
            if (condition)
            {
                Trace(message, values);
            }
        }

        [Conditional("DEBUG")]
        internal static void Trace(string message, params object[] values)
        {
            if (traceSerialization.TraceVerbose)
            {
                int indent = 0;
                int oldIndent = Debug.IndentLevel;

                if (traceScope != null)
                {
                    indent = traceScope.Count;
                }

                try
                {
                    Debug.IndentLevel = indent;
                    Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, message, values));
                }
                finally
                {
                    Debug.IndentLevel = oldIndent;
                }
            }
        }

        private struct TracingScope : IDisposable
        {
            public void Dispose()
            {
                if (traceScope != null)
                {
                    traceScope.Pop();
                }
            }
        }

        /// <summary>
        ///  This is a helper method that will deserialize a given statement.  It deserializes
        ///  the statement by interpreting and executing the CodeDom statement.
        /// </summary>
        protected void DeserializeStatement(IDesignerSerializationManager manager, CodeStatement statement)
        {
            using (TraceScope("CodeDomSerializerBase::" + nameof(DeserializeStatement)))
            {
                Trace("Statement : {0}", statement.GetType().Name);

                // Push this statement onto the context stack.  This allows any serializers handling an expression to know what it was connected to.
                manager.Context.Push(statement);
                try
                {
                    // Perf: is -> as changes, change ordering based on possibility of occurance
                    // Please excuse the bad formatting, but I think it is more readable this way than nested indenting.
                    if (statement is CodeAssignStatement cas)
                    {
                        DeserializeAssignStatement(manager, cas);
                    }
                    else
                    {
                        if (statement is CodeVariableDeclarationStatement cvds)
                        {
                            DeserializeVariableDeclarationStatement(manager, cvds);
                        }
                        else if (statement is CodeCommentStatement)
                        {
                            // do nothing for comments.  This just supresses the debug warning
                        }
                        else
                        {
                            CodeExpressionStatement ces = statement as CodeExpressionStatement;
                            if (ces != null)
                            {
                                DeserializeExpression(manager, null, ces.Expression);
                            }
                            else
                            {
                                if (statement is CodeMethodReturnStatement cmrs)
                                {
                                    DeserializeExpression(manager, null, ces.Expression);
                                }
                                else
                                {
                                    if (statement is CodeAttachEventStatement caes)
                                    {
                                        DeserializeAttachEventStatement(manager, caes);
                                    }
                                    else
                                    {
                                        if (statement is CodeRemoveEventStatement cres)
                                        {
                                            DeserializeDetachEventStatement(manager, cres);
                                        }
                                        else
                                        {
                                            if (statement is CodeLabeledStatement cls)
                                            {
                                                DeserializeStatement(manager, cls.Statement);
                                            }
                                            else
                                            {
                                                TraceWarning("Unrecognized statement type: {0}", statement.GetType().Name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (CheckoutException)
                {
                    throw; // we want to propagate those all the way up
                }
                catch (Exception e)
                {
                    // Since we always go through reflection, don't  show what our engine does, show what caused  the problem.
                    if (e is TargetInvocationException)
                    {
                        e = e.InnerException;
                    }

                    if (!(e is CodeDomSerializerException) && statement.LinePragma != null)
                    {
                        e = new CodeDomSerializerException(e, statement.LinePragma);
                    }

                    manager.ReportError(e);
                }
                finally
                {
                    Debug.Assert(manager.Context.Current == statement, "Someone corrupted the context stack");
                    manager.Context.Pop();
                }
            }
        }

        private void DeserializeVariableDeclarationStatement(IDesignerSerializationManager manager, CodeVariableDeclarationStatement statement)
        {
            using (TraceScope("CodeDomSerializerBase::" + nameof(DeserializeVariableDeclarationStatement)))
            {
                if (statement.InitExpression != null)
                {
                    Trace("Processing init expression");
                    DeserializeExpression(manager, statement.Name, statement.InitExpression);
                }
            }
        }

        private void DeserializeDetachEventStatement(IDesignerSerializationManager manager, CodeRemoveEventStatement statement)
        {
            using (TraceScope("CodeDomSerializerBase::" + nameof(DeserializeDetachEventStatement)))
            {
                object eventListener = DeserializeExpression(manager, null, statement.Listener);
                TraceErrorIf(!(eventListener is CodeDelegateCreateExpression), "Unable to simplify event attach RHS to a delegate create.");
                if (eventListener is CodeDelegateCreateExpression delegateCreate)
                {
                    // We only support binding methods to the root object.
                    object eventAttachObject = DeserializeExpression(manager, null, delegateCreate.TargetObject);
                    RootContext rootExp = (RootContext)manager.Context[typeof(RootContext)];
                    bool isRoot = rootExp is null || (rootExp != null && rootExp.Value == eventAttachObject);
                    TraceWarningIf(!isRoot, "Event is bound to an object other than the root.  We do not support this.");
                    if (isRoot)
                    {
                        // Now deserialize the LHS of the event attach to discover the guy whose event we are attaching.
                        object targetObject = DeserializeExpression(manager, null, statement.Event.TargetObject);
                        TraceErrorIf(targetObject is CodeExpression, "Unable to simplify event attach LHS to an object reference.");
                        if (!(targetObject is CodeExpression))
                        {
                            EventDescriptor evt = GetEventsHelper(manager, targetObject, null)[statement.Event.EventName];
                            if (evt != null)
                            {
                                IEventBindingService evtSvc = (IEventBindingService)manager.GetService(typeof(IEventBindingService));
                                if (evtSvc != null)
                                {
                                    PropertyDescriptor prop = evtSvc.GetEventProperty(evt);
                                    prop.SetValue(targetObject, null);
                                }
                            }
                            else
                            {
                                TraceError("Object {0} does not have a event {1}", targetObject.GetType().Name, statement.Event.EventName);
                                Error(manager, string.Format(SR.SerializerNoSuchEvent, targetObject.GetType().FullName, statement.Event.EventName), SR.SerializerNoSuchEvent);
                            }
                        }
                    }
                }
            }
        }

        private void DeserializeAssignStatement(IDesignerSerializationManager manager, CodeAssignStatement statement)
        {
            using (TraceScope("CodeDomSerializerBase::" + nameof(DeserializeAssignStatement)))
            {
                // Since we're doing an assignment into something, we need to know what that something is.  It can be a property, a variable, or a member. Anything else is invalid.
                //Perf: is -> as changes, change ordering based on possibility of occurrence
                CodeExpression expression = statement.Left;

                Trace("Processing LHS");
                if (expression is CodePropertyReferenceExpression propertyReferenceEx)
                {
                    DeserializePropertyAssignStatement(manager, statement, propertyReferenceEx, true);
                }
                else if (expression is CodeFieldReferenceExpression fieldReferenceEx)
                {
                    Trace("LHS is field : {0}", fieldReferenceEx.FieldName);
                    object lhs = DeserializeExpression(manager, fieldReferenceEx.FieldName, fieldReferenceEx.TargetObject);
                    if (lhs != null)
                    {
                        RootContext root = (RootContext)manager.Context[typeof(RootContext)];

                        if (root != null && root.Value == lhs)
                        {
                            Trace("Processing RHS");
                            object rhs = DeserializeExpression(manager, fieldReferenceEx.FieldName, statement.Right);
                            if (rhs is CodeExpression)
                            {
                                TraceError("Unable to simplify statement to anything better than: {0}", rhs.GetType().Name);
                                return;
                            }
                        }
                        else
                        {
                            FieldInfo f;
                            object instance;
                            Type type = lhs as Type;

                            if (type != null)
                            {
                                instance = null;
                                f = GetReflectionTypeFromTypeHelper(manager, type).GetField(fieldReferenceEx.FieldName, BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
                            }
                            else
                            {
                                instance = lhs;
                                f = GetReflectionTypeHelper(manager, lhs).GetField(fieldReferenceEx.FieldName, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
                            }

                            if (f != null)
                            {
                                Trace("Processing RHS");
                                object rhs = DeserializeExpression(manager, fieldReferenceEx.FieldName, statement.Right);
                                if (rhs is CodeExpression)
                                {
                                    TraceError("Unable to simplify statement to anything better than: {0}", rhs.GetType().Name);
                                    return;
                                }

                                if (rhs is IConvertible ic)
                                {
                                    // f.FieldType is a type from the reflection (or project target) universe, while rhs is a runtime type (exists in the Visual Studio framework)
                                    // they need to be converted to the same universe for comparison to work.
                                    // If TargetFrameworkProvider is not available, then we are working with runtime types.
                                    Type fieldType = f.FieldType;
                                    TypeDescriptionProvider tdp = GetTargetFrameworkProviderForType(manager, fieldType);
                                    if (tdp != null)
                                    {
                                        fieldType = tdp.GetRuntimeType(fieldType);
                                    }
                                    if (fieldType != rhs.GetType())
                                    {
                                        try
                                        {
                                            rhs = ic.ToType(fieldType, null);
                                        }
                                        catch
                                        {
                                            // oh well...
                                        }
                                    }
                                }

                                f.SetValue(instance, rhs);
                            }
                            else
                            {
                                //lets try it as a property:
                                CodePropertyReferenceExpression propRef = new CodePropertyReferenceExpression
                                {
                                    TargetObject = fieldReferenceEx.TargetObject,
                                    PropertyName = fieldReferenceEx.FieldName
                                };
                                if (!DeserializePropertyAssignStatement(manager, statement, propRef, false))
                                {
                                    TraceError("Object {0} does not have a field {1}", lhs.GetType().Name, fieldReferenceEx.FieldName);
                                    Error(manager, string.Format(SR.SerializerNoSuchField, lhs.GetType().FullName, fieldReferenceEx.FieldName), SR.SerializerNoSuchField);
                                }
                            }
                        }
                    }
                    else
                    {
                        TraceWarning("Could not find target object for field {0}", fieldReferenceEx.FieldName);
                    }
                }
                else if (expression is CodeVariableReferenceExpression variableReferenceEx)
                {
                    // This is the easiest.  Just relate the RHS object to the name of the variable.
                    Trace("Processing RHS");
                    object rhs = DeserializeExpression(manager, variableReferenceEx.VariableName, statement.Right);
                    if (rhs is CodeExpression)
                    {
                        TraceError("Unable to simplify statement to anything better than: {0}", rhs.GetType().Name);
                        return;
                    }
                    manager.SetName(rhs, variableReferenceEx.VariableName);
                }
                else if (expression is CodeArrayIndexerExpression arrayIndexerEx)
                {
                    int[] indexes = new int[arrayIndexerEx.Indices.Count];
                    Trace("LHS is Array Indexer with dims {0}", indexes.Length);
                    object array = DeserializeExpression(manager, null, arrayIndexerEx.TargetObject);
                    bool indexesOK = true;

                    // The indexes have to be of type int32. If they're not, then we cannot assign to this array.
                    for (int i = 0; i < indexes.Length; i++)
                    {
                        object index = DeserializeExpression(manager, null, arrayIndexerEx.Indices[i]);
                        if (index is IConvertible ic)
                        {
                            indexes[i] = ic.ToInt32(null);
                            Trace("[{0}] == {1}", i, indexes[i]);
                        }
                        else
                        {
                            TraceWarning("Index {0} could not be converted to int.  Type: {1}", i, (index is null ? "(null)" : index.GetType().Name));
                            indexesOK = false;
                            break;
                        }
                    }

                    if (array is Array arr && indexesOK)
                    {
                        Trace("Processing RHS");
                        object rhs = DeserializeExpression(manager, null, statement.Right);
                        if (rhs is CodeExpression)
                        {
                            TraceError("Unable to simplify statement to anything better than: {0}", rhs.GetType().Name);
                            return;
                        }
                        arr.SetValue(rhs, indexes);
                    }
                    else
                    {
                        TraceErrorIf(!(array is Array), "Array resovled to something other than an array: {0}", (array is null ? "(null)" : array.GetType().Name));
                        TraceErrorIf(!indexesOK, "Indexes to array could not be converted to int32.");
                    }
                }
            }
        }

        [Conditional("DEBUG")]
        internal static void TraceError(string message, params object[] values)
        {
            if (traceSerialization.TraceError)
            {
                string scope = string.Empty;
                if (traceScope != null)
                {
                    foreach (string scopeName in traceScope)
                    {
                        if (scope.Length > 0)
                        {
                            scope = "/" + scope;
                        }
                        scope = scopeName + scope;
                    }
                }

                Debug.WriteLine("***************************************************");
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "*** ERROR :{0}", string.Format(CultureInfo.CurrentCulture, message, values)));
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "*** SCOPE :{0}", scope));
                Debug.WriteLine("***************************************************");
            }
        }

        /// <summary>
        ///  This is a helper method that will deserialize a given expression.  It deserializes
        ///  the statement by interpreting and executing the CodeDom expression, returning
        ///  the results.
        /// </summary>
        protected object DeserializeExpression(IDesignerSerializationManager manager, string name, CodeExpression expression)
        {
            object result = expression;
            using (TraceScope("CodeDomSerializerBase::" + nameof(DeserializeExpression)))
            {
                // Perf: is -> as changes, change ordering based on possibility of occurance
                // If you are adding to this, use as instead of is + cast and order new expressions in order of frequency in typical user code.
                CodePropertyReferenceExpression propertyReferenceEx;
                CodeTypeReferenceExpression typeReferenceEx;
                CodeObjectCreateExpression objectCreateEx;
                CodeArgumentReferenceExpression argumentReferenceEx;
                CodeFieldReferenceExpression fieldReferenceEx;
                CodeMethodInvokeExpression methodInvokeEx;
                CodeVariableReferenceExpression variableReferenceEx;
                CodeCastExpression castEx;
                CodeArrayCreateExpression arrayCreateEx;
                CodeArrayIndexerExpression arrayIndexerEx;
                CodeBinaryOperatorExpression binaryOperatorEx;
                CodeDelegateInvokeExpression delegateInvokeEx;
                CodeDirectionExpression directionEx;
                CodeIndexerExpression indexerEx;
                CodeParameterDeclarationExpression parameterDeclaration;
                CodeTypeOfExpression typeOfExpression;

                while (result != null)
                {
                    if (result is CodePrimitiveExpression primitiveEx)
                    {
                        Trace("Primitive.  Value: {0}", (primitiveEx.Value ?? "(null)"));
                        result = primitiveEx.Value;
                        break;
                    }
                    else if ((propertyReferenceEx = result as CodePropertyReferenceExpression) != null)
                    {
                        result = DeserializePropertyReferenceExpression(manager, propertyReferenceEx, true);
                        break;
                    }
                    else if (result is CodeThisReferenceExpression)
                    { //(is -> as doesn't help here, since the cast is different)
                        Trace("'this' reference");
                        RootContext rootExp = (RootContext)manager.Context[typeof(RootContext)];
                        if (rootExp != null)
                        {
                            result = rootExp.Value;
                        }
                        else
                        {
                            // Last ditch effort.  Some things have to code gen against "this", such as event wireups.  Those are always bounda against the root component.
                            if (manager.GetService(typeof(IDesignerHost)) is IDesignerHost host)
                            {
                                result = host.RootComponent;
                            }
                        }

                        if (result is null)
                        {
                            TraceError("CodeThisReferenceExpression not handled because there is no root context or the root context did not contain an instance.");
                            Error(manager, SR.SerializerNoRootExpression, SR.SerializerNoRootExpression);
                        }

                        break;
                    }
                    else if ((typeReferenceEx = result as CodeTypeReferenceExpression) != null)
                    {
                        Trace("TypeReference : {0}", typeReferenceEx.Type.BaseType);
                        result = manager.GetType(GetTypeNameFromCodeTypeReference(manager, typeReferenceEx.Type));
                        break;
                    }
                    else if ((objectCreateEx = result as CodeObjectCreateExpression) != null)
                    {
                        Trace("Object create");
                        result = null;
                        Type type = manager.GetType(GetTypeNameFromCodeTypeReference(manager, objectCreateEx.CreateType));
                        if (type != null)
                        {
                            object[] parameters = new object[objectCreateEx.Parameters.Count];
                            bool paramsOk = true;
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                parameters[i] = DeserializeExpression(manager, null, objectCreateEx.Parameters[i]);
                                if (parameters[i] is CodeExpression)
                                {
                                    // Before we bail on this parameter, see if the type is a delegate.  If we are creating a delegate we may be able to bind to the method after all.
                                    if (typeof(Delegate).IsAssignableFrom(type) && parameters.Length == 1 && parameters[i] is CodeMethodReferenceExpression)
                                    {
                                        CodeMethodReferenceExpression methodRef = (CodeMethodReferenceExpression)parameters[i];

                                        // Only do this if our target is not the root context.
                                        if (!(methodRef.TargetObject is CodeThisReferenceExpression))
                                        {
                                            object target = DeserializeExpression(manager, null, methodRef.TargetObject);
                                            if (!(target is CodeExpression))
                                            {
                                                // Search for a matching method sig.  Must be public since we don't own this object
                                                MethodInfo delegateInvoke = type.GetMethod("Invoke");
                                                if (delegateInvoke != null)
                                                {
                                                    ParameterInfo[] delegateParams = delegateInvoke.GetParameters();
                                                    Type[] paramTypes = new Type[delegateParams.Length];
                                                    for (int idx = 0; idx < paramTypes.Length; idx++)
                                                    {
                                                        paramTypes[idx] = delegateParams[i].ParameterType;
                                                    }
                                                    MethodInfo mi = GetReflectionTypeHelper(manager, target).GetMethod(methodRef.MethodName, paramTypes);
                                                    if (mi != null)
                                                    {
                                                        // MethodInfo from the reflection Universe might not implement MethodHandle property, once we know that the method is available, get it from the runtime type.
                                                        mi = target.GetType().GetMethod(methodRef.MethodName, paramTypes);
                                                        result = Activator.CreateInstance(type, new object[] { target, mi.MethodHandle.GetFunctionPointer() });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // Technically, the paramters are not OK.  Our special case above, if successful, would have produced a "result" object for us.
                                    paramsOk = false;
                                    break;
                                }
                            }

                            if (paramsOk)
                            {
                                // Create an instance of the object.  If the caller provided a name, then ask the manager to add this object to the container.
                                result = DeserializeInstance(manager, type, parameters, name, (name != null));
                            }
                        }
                        else
                        {
                            TraceError("Type {0} could not be loaded", objectCreateEx.CreateType.BaseType);
                            Error(manager, string.Format(SR.SerializerTypeNotFound, objectCreateEx.CreateType.BaseType), SR.SerializerTypeNotFound);
                        }
                        break;
                    }
                    else if ((argumentReferenceEx = result as CodeArgumentReferenceExpression) != null)
                    {
                        Trace("Named argument reference : {0}", argumentReferenceEx.ParameterName);
                        result = manager.GetInstance(argumentReferenceEx.ParameterName);
                        if (result is null)
                        {
                            TraceError("Parameter {0} does not exist", argumentReferenceEx.ParameterName);
                            Error(manager, string.Format(SR.SerializerUndeclaredName, argumentReferenceEx.ParameterName), SR.SerializerUndeclaredName);
                        }
                        break;
                    }
                    else if ((fieldReferenceEx = result as CodeFieldReferenceExpression) != null)
                    {
                        Trace("Field reference : {0}", fieldReferenceEx.FieldName);
                        object target = DeserializeExpression(manager, null, fieldReferenceEx.TargetObject);
                        if (target != null && !(target is CodeExpression))
                        {
                            // If the target is the root object, then this won't be found through reflection.  Instead, ask the manager for the field by name.
                            RootContext rootExp = (RootContext)manager.Context[typeof(RootContext)];
                            if (rootExp != null && rootExp.Value == target)
                            {
                                object namedObject = manager.GetInstance(fieldReferenceEx.FieldName);
                                if (namedObject != null)
                                {
                                    result = namedObject;
                                }
                                else
                                {
                                    TraceError("Field {0} could not be resolved", fieldReferenceEx.FieldName);
                                    Error(manager, string.Format(SR.SerializerUndeclaredName, fieldReferenceEx.FieldName), SR.SerializerUndeclaredName);
                                }
                            }
                            else
                            {
                                FieldInfo field;
                                object instance;
                                Type t = target as Type;
                                if (t != null)
                                {
                                    instance = null;
                                    field = GetReflectionTypeFromTypeHelper(manager, t).GetField(fieldReferenceEx.FieldName, BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
                                }
                                else
                                {
                                    instance = target;
                                    field = GetReflectionTypeHelper(manager, target).GetField(fieldReferenceEx.FieldName, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
                                }

                                if (field != null)
                                {
                                    result = field.GetValue(instance);
                                }
                                else
                                {
                                    //lets try it as a property:
                                    CodePropertyReferenceExpression propRef = new CodePropertyReferenceExpression
                                    {
                                        TargetObject = fieldReferenceEx.TargetObject,
                                        PropertyName = fieldReferenceEx.FieldName
                                    };

                                    result = DeserializePropertyReferenceExpression(manager, propRef, false);
                                    if (result == fieldReferenceEx)
                                    {
                                        Error(manager, string.Format(SR.SerializerUndeclaredName, fieldReferenceEx.FieldName), SR.SerializerUndeclaredName);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Error(manager, string.Format(SR.SerializerFieldTargetEvalFailed, fieldReferenceEx.FieldName), SR.SerializerFieldTargetEvalFailed);
                        }

                        TraceWarningIf(result == fieldReferenceEx, "Could not resolve field {0} to an object instance.", fieldReferenceEx.FieldName);
                        break;
                    }
                    else if ((methodInvokeEx = result as CodeMethodInvokeExpression) != null)
                    {
                        Trace("Method invoke : {0}", methodInvokeEx.Method.MethodName);

                        object targetObject = DeserializeExpression(manager, null, methodInvokeEx.Method.TargetObject);

                        if (targetObject != null)
                        {
                            object[] parameters = new object[methodInvokeEx.Parameters.Count];
                            bool paramsOk = true;

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                parameters[i] = DeserializeExpression(manager, null, methodInvokeEx.Parameters[i]);
                                if (parameters[i] is CodeExpression)
                                {
                                    paramsOk = false;
                                    break;
                                }
                            }

                            if (paramsOk)
                            {
                                IComponentChangeService compChange = (IComponentChangeService)manager.GetService(typeof(IComponentChangeService));
                                Type t = targetObject as Type;

                                if (t != null)
                                {
                                    result = GetReflectionTypeFromTypeHelper(manager, t).InvokeMember(methodInvokeEx.Method.MethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, parameters, null, null, null);
                                }
                                else
                                {
                                    if (compChange != null)
                                    {
                                        compChange.OnComponentChanging(targetObject, null);
                                    }

                                    try
                                    {
                                        result = GetReflectionTypeHelper(manager, targetObject).InvokeMember(methodInvokeEx.Method.MethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, targetObject, parameters, null, null, null);
                                    }
                                    catch (MissingMethodException)
                                    {
                                        // We did not find the method directly. Let's see if we can find it
                                        // as an private implemented interface name.
                                        //

                                        if (methodInvokeEx.Method.TargetObject is CodeCastExpression castExpr)
                                        {
                                            Type castType = manager.GetType(GetTypeNameFromCodeTypeReference(manager, castExpr.TargetType));

                                            if (castType != null && castType.IsInterface)
                                            {
                                                result = GetReflectionTypeFromTypeHelper(manager, castType).InvokeMember(methodInvokeEx.Method.MethodName, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, targetObject, parameters, null, null, null);
                                            }
                                            else
                                            {
                                                throw;
                                            }
                                        }
                                        else
                                        {
                                            throw;
                                        }
                                    }
                                    if (compChange != null)
                                    {
                                        compChange.OnComponentChanged(targetObject, null, null, null);
                                    }
                                }
                            }
                            else if (parameters.Length == 1 && parameters[0] is CodeDelegateCreateExpression)
                            {
                                string methodName = methodInvokeEx.Method.MethodName;

                                if (methodName.StartsWith("add_"))
                                {
                                    methodName = methodName.Substring(4);
                                    DeserializeAttachEventStatement(manager, new CodeAttachEventStatement(methodInvokeEx.Method.TargetObject, methodName, (CodeExpression)parameters[0]));
                                    result = null;
                                }
                            }
                        }

                        break;
                    }
                    else if ((variableReferenceEx = result as CodeVariableReferenceExpression) != null)
                    {
                        Trace("Variable reference : {0}", variableReferenceEx.VariableName);
                        result = manager.GetInstance(variableReferenceEx.VariableName);
                        if (result is null)
                        {
                            TraceError("Variable {0} does not exist", variableReferenceEx.VariableName);
                            Error(manager, string.Format(SR.SerializerUndeclaredName, variableReferenceEx.VariableName), SR.SerializerUndeclaredName);
                        }

                        break;
                    }
                    else if ((castEx = result as CodeCastExpression) != null)
                    {
                        Trace("Cast : {0}", castEx.TargetType.BaseType);
                        result = DeserializeExpression(manager, name, castEx.Expression);
                        if (result is IConvertible ic)
                        {
                            Type targetType = manager.GetType(GetTypeNameFromCodeTypeReference(manager, castEx.TargetType));
                            if (targetType != null)
                            {
                                result = ic.ToType(targetType, null);
                            }
                        }
                        break;
                    }
                    else if (result is CodeBaseReferenceExpression)
                    { //(is -> as doesn't help here, since the cast is different)
                        RootContext rootExp = (RootContext)manager.Context[typeof(RootContext)];
                        if (rootExp != null)
                        {
                            result = rootExp.Value;
                        }
                        else
                        {
                            result = null;
                        }
                        break;
                    }
                    else if ((arrayCreateEx = result as CodeArrayCreateExpression) != null)
                    {
                        Trace("Array create : {0}", arrayCreateEx.CreateType.BaseType);
                        Type arrayType = manager.GetType(GetTypeNameFromCodeTypeReference(manager, arrayCreateEx.CreateType));
                        Array array = null;

                        if (arrayType != null)
                        {
                            if (arrayCreateEx.Initializers.Count > 0)
                            {
                                Trace("{0} initializers", arrayCreateEx.Initializers.Count);
                                // Passed an array of initializers.  Use this to create the array.  Note that we use an  ArrayList here and add elements as we create them. It is possible that an element cannot be resolved. This is an error, but we do not want to tank the entire array.  If we kicked out the entire statement, a missing control would cause all controls on a form to vanish.
                                ArrayList arrayList = new ArrayList(arrayCreateEx.Initializers.Count);

                                foreach (CodeExpression initializer in arrayCreateEx.Initializers)
                                {
                                    try
                                    {
                                        object o = DeserializeExpression(manager, null, initializer);

                                        if (!(o is CodeExpression))
                                        {
                                            if (!arrayType.IsInstanceOfType(o))
                                            {
                                                o = Convert.ChangeType(o, arrayType, CultureInfo.InvariantCulture);
                                            }

                                            arrayList.Add(o);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        manager.ReportError(ex);
                                    }
                                }

                                array = Array.CreateInstance(arrayType, arrayList.Count);
                                arrayList.CopyTo(array, 0);
                            }
                            else if (arrayCreateEx.SizeExpression != null)
                            {
                                object o = DeserializeExpression(manager, name, arrayCreateEx.SizeExpression);
                                Debug.Assert(o is IConvertible, "Array size expression could not be resolved to IConvertible: " + (o is null ? "(null)" : o.GetType().Name));

                                if (o is IConvertible ic)
                                {
                                    int size = ic.ToInt32(null);
                                    Trace("Initialized with expression that simplified to {0}", size);
                                    array = Array.CreateInstance(arrayType, size);
                                }
                            }
                            else
                            {
                                Trace("Initialized with size {0}", arrayCreateEx.Size);
                                array = Array.CreateInstance(arrayType, arrayCreateEx.Size);
                            }
                        }
                        else
                        {
                            TraceError("Type could not be resolved: {0}", arrayCreateEx.CreateType.BaseType);
                            Error(manager, string.Format(SR.SerializerTypeNotFound, arrayCreateEx.CreateType.BaseType), SR.SerializerTypeNotFound);
                        }

                        result = array;
                        if (result != null && name != null)
                        {
                            manager.SetName(result, name);
                        }

                        break;
                    }
                    else if ((arrayIndexerEx = result as CodeArrayIndexerExpression) != null)
                    {
                        Trace("Array indexer");

                        // For this, assume in any error we return a null.  The only errors
                        // here should come from a mal-formed expression.
                        //
                        result = null;

                        if (DeserializeExpression(manager, name, arrayIndexerEx.TargetObject) is Array array)
                        {
                            int[] indexes = new int[arrayIndexerEx.Indices.Count];

                            Trace("Dims: {0}", indexes.Length);

                            bool indexesOK = true;

                            // The indexes have to be of type int32.  If they're not, then
                            // we cannot assign to this array.
                            //
                            for (int i = 0; i < indexes.Length; i++)
                            {
                                IConvertible index = DeserializeExpression(manager, name, arrayIndexerEx.Indices[i]) as IConvertible;

                                if (index != null)
                                {
                                    indexes[i] = index.ToInt32(null);
                                    Trace("[{0}] == {1}", i, indexes[i]);
                                }
                                else
                                {
                                    TraceWarning("Index {0} could not be converted to int.  Type: {1}", i, (index is null ? "(null)" : index.GetType().Name));
                                    indexesOK = false;
                                    break;
                                }
                            }

                            if (indexesOK)
                            {
                                result = array.GetValue(indexes);
                            }
                        }

                        break;
                    }
                    else if ((binaryOperatorEx = result as CodeBinaryOperatorExpression) != null)
                    {
                        Trace("Binary operator : {0}", binaryOperatorEx.Operator);

                        object left = DeserializeExpression(manager, null, binaryOperatorEx.Left);
                        object right = DeserializeExpression(manager, null, binaryOperatorEx.Right);

                        // We assign the result to an arbitrary value here in case the operation could
                        // not be performed.
                        //
                        result = left;

                        if (left is IConvertible icLeft && right is IConvertible icRight)
                        {
                            result = ExecuteBinaryExpression(icLeft, icRight, binaryOperatorEx.Operator);
                        }
                        else
                        {
                            TraceWarning("Could not simplify left and right binary operators to IConvertible.");
                        }
                        break;
                    }
                    else if ((delegateInvokeEx = result as CodeDelegateInvokeExpression) != null)
                    {
                        Trace("Delegate invoke");
                        object targetObject = DeserializeExpression(manager, null, delegateInvokeEx.TargetObject);
                        if (targetObject is Delegate del)
                        {
                            object[] parameters = new object[delegateInvokeEx.Parameters.Count];
                            bool paramsOk = true;
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                parameters[i] = DeserializeExpression(manager, null, delegateInvokeEx.Parameters[i]);
                                if (parameters[i] is CodeExpression)
                                {
                                    paramsOk = false;
                                    break;
                                }
                            }

                            if (paramsOk)
                            {
                                Trace("Invoking {0} with {1} parameters", targetObject.GetType().Name, parameters.Length);
                                del.DynamicInvoke(parameters);
                            }
                        }

                        break;
                    }
                    else if ((directionEx = result as CodeDirectionExpression) != null)
                    {
                        Trace("Direction operator");
                        result = DeserializeExpression(manager, name, directionEx.Expression);
                        break;
                    }
                    else if ((indexerEx = result as CodeIndexerExpression) != null)
                    {
                        Trace("Indexer");
                        // For this, assume in any error we return a null.  The only errors here should come from a mal-formed expression.
                        result = null;
                        object targetObject = DeserializeExpression(manager, null, indexerEx.TargetObject);
                        if (targetObject != null)
                        {
                            object[] indexes = new object[indexerEx.Indices.Count];
                            Trace("Indexes: {0}", indexes.Length);
                            bool indexesOK = true;
                            for (int i = 0; i < indexes.Length; i++)
                            {
                                indexes[i] = DeserializeExpression(manager, null, indexerEx.Indices[i]);
                                if (indexes[i] is CodeExpression)
                                {
                                    TraceWarning("Index {0} could not be simplified to an object.", i);
                                    indexesOK = false;
                                    break;
                                }
                            }

                            if (indexesOK)
                            {
                                result = GetReflectionTypeHelper(manager, targetObject).InvokeMember("Item", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, targetObject, indexes, null, null, null);
                            }
                        }
                        break;
                    }
                    else if (result is CodeSnippetExpression)
                    {
                        Trace("Snippet");
                        result = null;
                        break;
                    }
                    else if ((parameterDeclaration = result as CodeParameterDeclarationExpression) != null)
                    {
                        Trace("Parameter declaration");
                        result = manager.GetType(GetTypeNameFromCodeTypeReference(manager, parameterDeclaration.Type));
                        break;
                    }
                    else if ((typeOfExpression = result as CodeTypeOfExpression) != null)
                    {
                        Trace("Typeof({0})", typeOfExpression.Type.BaseType);
                        string type = GetTypeNameFromCodeTypeReference(manager, typeOfExpression.Type);
                        // add the array ranks so we get the right type of this thing.
                        for (int i = 0; i < typeOfExpression.Type.ArrayRank; i++)
                        {
                            type += "[]";
                        }

                        result = manager.GetType(type);
                        if (result is null)
                        {
                            TraceError("Type could not be resolved: {0}", type);
                            Error(manager, string.Format(SR.SerializerTypeNotFound, type), SR.SerializerTypeNotFound);
                        }
                        break;
                    }
                    else if (result is CodeEventReferenceExpression || result is CodeMethodReferenceExpression || result is CodeDelegateCreateExpression)
                    {
                        // These are all the expressions we know about, but expect to return to the caller because we cannot simplify them.
                        break;
                    }
                    else
                    {
                        // All expression evaluation happens above.  This codepath indicates that we got some sort of junk in the evalualtor,  or that someone assigned result to a value without breaking out of the loop.
                        Debug.Fail("Unrecognized expression type: " + result.GetType().Name);
                        break;
                    }
                }
            }
            return result;
        }

        private void DeserializeAttachEventStatement(IDesignerSerializationManager manager, CodeAttachEventStatement statement)
        {
            using (TraceScope("CodeDomSerializerBase::" + nameof(DeserializeAttachEventStatement)))
            {
                string handlerMethodName = null;
                object eventAttachObject = null;

                // Get the target information
                object targetObject = DeserializeExpression(manager, null, statement.Event.TargetObject);
                string eventName = statement.Event.EventName;
                Debug.Assert(targetObject != null, "Failed to get target object for event attach");
                Debug.Assert(eventName != null, "Failed to get eventName for event attach");
                if (eventName is null || targetObject is null)
                {
                    return;
                }

                if (statement.Listener is CodeObjectCreateExpression objCreate)
                {
                    // now walk into the CodeObjectCreateExpression and get the parameters so we can  get the name of the method, e.g. button1_Click
                    if (objCreate.Parameters.Count == 1)
                    {
                        // if this is a delegate create (new EventHandler(this.button1_Click)), then the first parameter should be a method ref.
                        if (objCreate.Parameters[0] is CodeMethodReferenceExpression methodRef)
                        {
                            handlerMethodName = methodRef.MethodName;
                            eventAttachObject = DeserializeExpression(manager, null, methodRef.TargetObject);
                        }
                    }
                    else
                    {
                        Debug.Fail("Encountered delegate object create with more or less than 1 parameter?");
                    }
                }
                else
                {
                    object eventListener = DeserializeExpression(manager, null, statement.Listener);
                    if (eventListener is CodeDelegateCreateExpression delegateCreate)
                    {
                        eventAttachObject = DeserializeExpression(manager, null, delegateCreate.TargetObject);
                        handlerMethodName = delegateCreate.MethodName;
                    }
                }

                RootContext rootExp = (RootContext)manager.Context[typeof(RootContext)];
                bool isRoot = rootExp is null || (rootExp != null && rootExp.Value == eventAttachObject);

                if (handlerMethodName is null)
                {
                    TraceError("Unable to retrieve handler method and object for delegate create.");
                }
                else
                {
                    // We only support binding methods to the root object.
                    //
                    TraceWarningIf(!isRoot, "Event is bound to an object other than the root.  We do not support this.");
                    if (isRoot)
                    {
                        // Now deserialize the LHS of the event attach to discover the guy whose
                        // event we are attaching.
                        //
                        TraceErrorIf(targetObject is CodeExpression, "Unable to simplify event attach LHS to an object reference.");
                        if (!(targetObject is CodeExpression))
                        {
                            EventDescriptor evt = GetEventsHelper(manager, targetObject, null)[eventName];

                            if (evt != null)
                            {
                                IEventBindingService evtSvc = (IEventBindingService)manager.GetService(typeof(IEventBindingService));

                                if (evtSvc != null)
                                {
                                    PropertyDescriptor prop = evtSvc.GetEventProperty(evt);

                                    prop.SetValue(targetObject, handlerMethodName);
                                    Trace("Attached event {0}.{1} to {2}", targetObject.GetType().Name, eventName, handlerMethodName);
                                }
                            }
                            else
                            {
                                TraceError("Object {0} does not have a event {1}", targetObject.GetType().Name, eventName);
                                Error(manager, string.Format(SR.SerializerNoSuchEvent, targetObject.GetType().FullName, eventName), SR.SerializerNoSuchEvent);
                            }
                        }
                    }
                }
            }
        }

        private object ExecuteBinaryExpression(IConvertible left, IConvertible right, CodeBinaryOperatorType op)
        {
            // "Binary" operator type is actually a combination of several types of operators: boolean, binary  and math.  Group them into categories here.
            CodeBinaryOperatorType[] booleanOperators = new CodeBinaryOperatorType[]
            {
                CodeBinaryOperatorType.IdentityInequality,
                CodeBinaryOperatorType.IdentityEquality,
                CodeBinaryOperatorType.ValueEquality,
                CodeBinaryOperatorType.BooleanOr,
                CodeBinaryOperatorType.BooleanAnd,
                CodeBinaryOperatorType.LessThan,
                CodeBinaryOperatorType.LessThanOrEqual,
                CodeBinaryOperatorType.GreaterThan,
                CodeBinaryOperatorType.GreaterThanOrEqual
            };

            CodeBinaryOperatorType[] mathOperators = new CodeBinaryOperatorType[]
            {
                CodeBinaryOperatorType.Add,
                CodeBinaryOperatorType.Subtract,
                CodeBinaryOperatorType.Multiply,
                CodeBinaryOperatorType.Divide,
                CodeBinaryOperatorType.Modulus
            };

            CodeBinaryOperatorType[] binaryOperators = new CodeBinaryOperatorType[]
            {
                CodeBinaryOperatorType.BitwiseOr,
                CodeBinaryOperatorType.BitwiseAnd
            };

            // Figure out what kind of expression we have.
            for (int i = 0; i < binaryOperators.Length; i++)
            {
                if (op == binaryOperators[i])
                {
                    return ExecuteBinaryOperator(left, right, op);
                }
            }

            for (int i = 0; i < mathOperators.Length; i++)
            {
                if (op == mathOperators[i])
                {
                    return ExecuteMathOperator(left, right, op);
                }
            }

            for (int i = 0; i < booleanOperators.Length; i++)
            {
                if (op == booleanOperators[i])
                {
                    return ExecuteBooleanOperator(left, right, op);
                }
            }

            Debug.Fail("Unsupported binary operator type: " + op.ToString());
            return left;
        }
        private object ExecuteBinaryOperator(IConvertible left, IConvertible right, CodeBinaryOperatorType op)
        {
            TypeCode leftType = left.GetTypeCode();
            TypeCode rightType = right.GetTypeCode();

            // The compatible types are listed in order from lowest bitness to highest.  We must operate on the highest bitness to keep fidelity.
            TypeCode[] compatibleTypes = new TypeCode[]
            {
                TypeCode.Byte,
                TypeCode.Char,
                TypeCode.Int16,
                TypeCode.UInt16,
                TypeCode.Int32,
                TypeCode.UInt32,
                TypeCode.Int64,
                TypeCode.UInt64
            };

            int leftTypeIndex = -1;
            int rightTypeIndex = -1;

            for (int i = 0; i < compatibleTypes.Length; i++)
            {
                if (leftType == compatibleTypes[i])
                {
                    leftTypeIndex = i;
                }
                if (rightType == compatibleTypes[i])
                {
                    rightTypeIndex = i;
                }
                if (leftTypeIndex != -1 && rightTypeIndex != -1)
                {
                    break;
                }
            }

            if (leftTypeIndex == -1 || rightTypeIndex == -1)
            {
                Debug.Fail("Could not convert left or right side to binary-compatible value.");
                return left;
            }

            int maxIndex = Math.Max(leftTypeIndex, rightTypeIndex);
            object result = left;
            switch (compatibleTypes[maxIndex])
            {
                case TypeCode.Byte:
                    {
                        byte leftValue = left.ToByte(null);
                        byte rightValue = right.ToByte(null);
                        if (op == CodeBinaryOperatorType.BitwiseOr)
                        {
                            result = leftValue | rightValue;
                        }
                        else
                        {
                            result = leftValue & rightValue;
                        }
                        break;
                    }
                case TypeCode.Char:
                    {
                        char leftValue = left.ToChar(null);
                        char rightValue = right.ToChar(null);
                        if (op == CodeBinaryOperatorType.BitwiseOr)
                        {
                            result = leftValue | rightValue;
                        }
                        else
                        {
                            result = leftValue & rightValue;
                        }
                        break;
                    }
                case TypeCode.Int16:
                    {
                        short leftValue = left.ToInt16(null);
                        short rightValue = right.ToInt16(null);
                        if (op == CodeBinaryOperatorType.BitwiseOr)
                        {
                            result = (short)((ushort)leftValue | (ushort)rightValue);
                        }
                        else
                        {
                            result = leftValue & rightValue;
                        }
                        break;
                    }
                case TypeCode.UInt16:
                    {
                        ushort leftValue = left.ToUInt16(null);
                        ushort rightValue = right.ToUInt16(null);
                        if (op == CodeBinaryOperatorType.BitwiseOr)
                        {
                            result = leftValue | rightValue;
                        }
                        else
                        {
                            result = leftValue & rightValue;
                        }
                        break;
                    }
                case TypeCode.Int32:
                    {
                        int leftValue = left.ToInt32(null);
                        int rightValue = right.ToInt32(null);
                        if (op == CodeBinaryOperatorType.BitwiseOr)
                        {
                            result = leftValue | rightValue;
                        }
                        else
                        {
                            result = leftValue & rightValue;
                        }
                        break;
                    }
                case TypeCode.UInt32:
                    {
                        uint leftValue = left.ToUInt32(null);
                        uint rightValue = right.ToUInt32(null);
                        if (op == CodeBinaryOperatorType.BitwiseOr)
                        {
                            result = leftValue | rightValue;
                        }
                        else
                        {
                            result = leftValue & rightValue;
                        }
                        break;
                    }
                case TypeCode.Int64:
                    {
                        long leftValue = left.ToInt64(null);
                        long rightValue = right.ToInt64(null);
                        if (op == CodeBinaryOperatorType.BitwiseOr)
                        {
                            result = leftValue | rightValue;
                        }
                        else
                        {
                            result = leftValue & rightValue;
                        }
                        break;
                    }
                case TypeCode.UInt64:
                    {
                        ulong leftValue = left.ToUInt64(null);
                        ulong rightValue = right.ToUInt64(null);
                        if (op == CodeBinaryOperatorType.BitwiseOr)
                        {
                            result = leftValue | rightValue;
                        }
                        else
                        {
                            result = leftValue & rightValue;
                        }
                        break;
                    }
            }

            if (result != left && left is Enum)
            {
                // For enums, try to convert back to the original type
                result = Enum.ToObject(left.GetType(), result);
            }
            return result;
        }

        private object ExecuteBooleanOperator(IConvertible left, IConvertible right, CodeBinaryOperatorType op)
        {
            bool result = false;
            switch (op)
            {
                case CodeBinaryOperatorType.IdentityInequality:
                    result = (left != right);
                    break;
                case CodeBinaryOperatorType.IdentityEquality:
                    result = (left == right);
                    break;
                case CodeBinaryOperatorType.ValueEquality:
                    result = left.Equals(right);
                    break;
                case CodeBinaryOperatorType.BooleanOr:
                    result = (left.ToBoolean(null) || right.ToBoolean(null));
                    break;
                case CodeBinaryOperatorType.BooleanAnd:
                    result = (left.ToBoolean(null) && right.ToBoolean(null));
                    break;
                case CodeBinaryOperatorType.LessThan:
                    // Not doing these at design time.
                    break;
                case CodeBinaryOperatorType.LessThanOrEqual:
                    // Not doing these at design time.
                    break;
                case CodeBinaryOperatorType.GreaterThan:
                    // Not doing these at design time.
                    break;
                case CodeBinaryOperatorType.GreaterThanOrEqual:
                    // Not doing these at design time.
                    break;
                default:
                    Debug.Fail("Should never get here!");
                    break;
            }

            return result;
        }

        private object ExecuteMathOperator(IConvertible left, IConvertible right, CodeBinaryOperatorType op)
        {
            if (op == CodeBinaryOperatorType.Add)
            {
                string leftString = left as string;
                string rightString = right as string;

                if (leftString is null && left is Char)
                {
                    leftString = left.ToString();
                }

                if (rightString is null && right is Char)
                {
                    rightString = right.ToString();
                }

                if (leftString != null && rightString != null)
                {
                    return leftString + rightString;
                }
                else
                {
                    Debug.Fail("Addition operator not supported for this type");
                }
            }
            else
            {
                Debug.Fail("Math operators are not supported");
            }
            return left;
        }

        private object DeserializePropertyReferenceExpression(IDesignerSerializationManager manager, CodePropertyReferenceExpression propertyReferenceEx, bool reportError)
        {
            object result = propertyReferenceEx;
            Trace("Property reference : {0}", propertyReferenceEx.PropertyName);
            object target = DeserializeExpression(manager, null, propertyReferenceEx.TargetObject);
            if (target != null && !(target is CodeExpression))
            {
                // if it's a type, then we've got ourselves a static field...
                if (!(target is Type))
                {
                    PropertyDescriptor prop = GetPropertiesHelper(manager, target, null)[propertyReferenceEx.PropertyName];
                    if (prop != null)
                    {
                        result = prop.GetValue(target);
                    }
                    else
                    {
                        // This could be a protected property on the base class.  Make sure we're  actually accessing through the base class, and then get the property directly from reflection.
                        if (GetExpression(manager, target) is CodeThisReferenceExpression)
                        {
                            PropertyInfo propInfo = GetReflectionTypeHelper(manager, target).GetProperty(propertyReferenceEx.PropertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (propInfo != null)
                            {
                                result = propInfo.GetValue(target, null);
                            }
                        }
                    }
                }
                else
                {
                    PropertyInfo prop = GetReflectionTypeFromTypeHelper(manager, (Type)target).GetProperty(propertyReferenceEx.PropertyName, BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public);
                    if (prop != null)
                    {
                        result = prop.GetValue(null, null);
                    }
                }

                if (result == propertyReferenceEx && reportError)
                {
                    string typeName = (target is Type) ? ((Type)target).FullName : GetReflectionTypeHelper(manager, target).FullName;
                    Error(manager, string.Format(SR.SerializerNoSuchProperty, typeName, propertyReferenceEx.PropertyName), SR.SerializerNoSuchProperty);
                }
            }
            TraceWarningIf(result == propertyReferenceEx, "Could not resolve property {0} to an object instance.", propertyReferenceEx.PropertyName);
            return result;
        }

        [Conditional("DEBUG")]
        internal static void TraceErrorIf(bool condition, string message, params object[] values)
        {
            if (condition)
            {
                TraceError(message, values);
            }
        }

        [Conditional("DEBUG")]
        internal static void TraceWarningIf(bool condition, string message, params object[] values)
        {
            if (condition)
            {
                TraceWarning(message, values);
            }
        }

        [Conditional("DEBUG")]
        internal static void TraceWarning(string message, params object[] values)
        {
            if (traceSerialization.TraceWarning)
            {
                string scope = string.Empty;
                if (traceScope != null)
                {
                    foreach (string scopeName in traceScope)
                    {
                        if (scope.Length > 0)
                        {
                            scope = "/" + scope;
                        }
                        scope = scopeName + scope;
                    }
                }

                Debug.WriteLine("***************************************************");
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "*** WARNING :{0}", string.Format(CultureInfo.CurrentCulture, message, values)));
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "*** SCOPE :{0}", scope));
                Debug.WriteLine("***************************************************");
            }
        }

        private bool DeserializePropertyAssignStatement(IDesignerSerializationManager manager, CodeAssignStatement statement,
            CodePropertyReferenceExpression propertyReferenceEx, bool reportError)
        {
            Trace("LHS is property : {0}", propertyReferenceEx.PropertyName);
            object lhs = DeserializeExpression(manager, null, propertyReferenceEx.TargetObject);

            if (lhs != null && !(lhs is CodeExpression))
            {
                // Property assignments must go through our type descriptor system. However, we do not support parameterized properties.  If there are any parameters on the property, we do not perform the assignment.
                PropertyDescriptorCollection properties = GetPropertiesHelper(manager, lhs, runTimeProperties);
                PropertyDescriptor p = properties[propertyReferenceEx.PropertyName];
                if (p != null)
                {
                    Trace("Processing RHS");
                    object rhs = DeserializeExpression(manager, null, statement.Right);
                    if (rhs is CodeExpression)
                    {
                        TraceError("Unable to simplify statement to anything better than: {0}", rhs.GetType().Name);
                        return false;
                    }

                    if (rhs is IConvertible ic && p.PropertyType != rhs.GetType())
                    {
                        try
                        {
                            rhs = ic.ToType(p.PropertyType, null);
                        }
                        catch
                        {
                        }
                    }

                    //We need to ensure that no virtual types leak into the runtime code
                    //So if we ever assign a property value to a Type -- we make sure that the Type is a
                    // real System.Type.
                    Type rhsType = rhs as Type;
                    if (rhsType != null && rhsType.UnderlyingSystemType != null)
                    {
                        rhs = rhsType.UnderlyingSystemType; //unwrap this "type" that came because it was not actually a real bcl type.
                    }

                    // Next: see if the RHS of this expression was a property reference too.  If it was, then
                    // we will look for a MemberRelationshipService to record the relationship between these
                    // two properties, if supported.
                    // We need to setup this MemberRelationship before we actually set the property value.
                    // If we do it the other way around the new property value will be pushed into the old
                    // relationship, which isn't a problem during normal serialization (since it not very
                    // likely the property has already been assigned to), but it does affect undo.
                    MemberRelationship oldRelation = MemberRelationship.Empty;
                    MemberRelationshipService relationships = null;
                    if (statement.Right is CodePropertyReferenceExpression)
                    {
                        relationships = manager.GetService(typeof(MemberRelationshipService)) as MemberRelationshipService;

                        if (relationships != null)
                        {
                            CodePropertyReferenceExpression rhsPropRef = (CodePropertyReferenceExpression)statement.Right;
                            object rhsPropTarget = DeserializeExpression(manager, null, rhsPropRef.TargetObject);
                            PropertyDescriptor rhsProp = GetPropertiesHelper(manager, rhsPropTarget, null)[rhsPropRef.PropertyName];

                            if (rhsProp != null)
                            {
                                MemberRelationship source = new MemberRelationship(lhs, p);
                                MemberRelationship target = new MemberRelationship(rhsPropTarget, rhsProp);

                                oldRelation = relationships[source];

                                if (relationships.SupportsRelationship(source, target))
                                {
                                    relationships[source] = target;
                                }
                            }
                        }
                    }
                    else
                    {
                        relationships = manager.GetService(typeof(MemberRelationshipService)) as MemberRelationshipService;

                        if (relationships != null)
                        {
                            oldRelation = relationships[lhs, p];
                            relationships[lhs, p] = MemberRelationship.Empty;
                        }
                    }

                    try
                    {
                        p.SetValue(lhs, rhs);
                    }
                    catch
                    {
                        if (relationships != null)
                        {
                            relationships[lhs, p] = oldRelation;
                        }
                        throw;
                    }
                    return true;
                }
                else
                {
                    if (reportError)
                    {
                        TraceError("Object {0} does not have a property {1}", lhs.GetType().Name, propertyReferenceEx.PropertyName);
                        Error(manager, string.Format(SR.SerializerNoSuchProperty, lhs.GetType().FullName, propertyReferenceEx.PropertyName), SR.SerializerNoSuchProperty);
                    }
                }
            }
            else
            {
                TraceWarning("Could not find target object for property {0}", propertyReferenceEx.PropertyName);
            }
            return false;
        }

        /// <summary>
        ///  This method returns an expression representing the given object.  It may return null, indicating that
        ///  no expression has been set that describes the object.  Expressions are acquired in one of three ways:
        ///  1.   The expression could be the result of a prior SetExpression call.
        ///  2.   The expression could have been found in the RootContext.
        ///  3.   The expression could be derived through IReferenceService.
        ///  4.   The current expression on the context stack has a PresetValue == value.
        ///  To derive expressions through IReferenceService, GetExpression asks the reference service if there
        ///  is a name for the given object.  If the expression service returns a valid name, it checks to see if
        ///  there is a '.' in the name.  This indicates that the expression service found this object as the return
        ///  value of a read only property on another object.  If there is a '.', GetExpression will split the reference
        ///  into sub-parts.  The leftmost part is a name that will be evalulated via manager.GetInstance.  For each
        ///  subsequent part, a property reference expression will be built.  The final expression will then be returned.
        ///  If the object did not have an expression set, or the object was not found in the reference service, null will
        ///  be returned from GetExpression, indicating there is no existing expression for the object.
        /// </summary>
        protected CodeExpression GetExpression(IDesignerSerializationManager manager, object value)
        {
            CodeExpression expression = null;

            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Trace("GetExpression called for object {0}", value.ToString());

            // Is the expression part of a prior SetExpression call?

            if (manager.Context[typeof(ExpressionTable)] is ExpressionTable table)
            {
                expression = table.GetExpression(value);
                TraceIf(expression != null, "Resolved through expression table : {0}", expression);
            }

            // Check to see if this object represents the root context.
            if (expression is null)
            {
                if (manager.Context[typeof(RootContext)] is RootContext rootEx && object.ReferenceEquals(rootEx.Value, value))
                {
                    expression = rootEx.Expression;
                    TraceIf(expression != null, "Resolved through root expression context : {0}", expression);
                }
            }

            // Now check IReferenceService.
            if (expression is null)
            {
                // perf: first try to retrieve objectName from DesignerSerializationManager
                // only then involve reference service if needed
                // this is done to avoid unnecessary ensuring\creating references

                string objectName = manager.GetName(value);
                if (objectName is null || objectName.IndexOf('.') != -1)
                {
                    if (manager.GetService(typeof(IReferenceService)) is IReferenceService refSvc)
                    {
                        objectName = refSvc.GetName(value);
                        if (objectName != null && objectName.IndexOf('.') != -1)
                        {
                            Trace("Resolving through IReferenceService : {0}", objectName);

                            // This object name is built from sub objects.  Assemble the graph of sub objects.
                            string[] nameParts = objectName.Split('.');

                            Debug.Assert(nameParts.Length > 0, "How can we fail to split when IndexOf succeeded?");

                            object baseInstance = manager.GetInstance(nameParts[0]);

                            TraceWarningIf(baseInstance is null, "Manager can't return an instance for object {0}", nameParts[0]);
                            if (baseInstance != null)
                            {
                                CodeExpression baseExpression = SerializeToExpression(manager, baseInstance);

                                TraceWarningIf(baseExpression is null, "Unable to serialize object {0} to an expression.", baseInstance);
                                if (baseExpression != null)
                                {
                                    for (int idx = 1; idx < nameParts.Length; idx++)
                                    {
                                        baseExpression = new CodePropertyReferenceExpression(baseExpression, nameParts[idx]);
                                    }

                                    expression = baseExpression;
                                }
                            }
                        }
                    }
                }
            }

            // Finally, the expression context.
            if (expression is null)
            {
                if (manager.Context[typeof(ExpressionContext)] is ExpressionContext cxt && object.ReferenceEquals(cxt.PresetValue, value))
                {
                    expression = cxt.Expression;
                }
            }

            if (expression != null)
            {
                // set up cache dependencies
                // we check to see if there is anything on the stack
                // if there is we make the parent entry a dependency of the current entry
                ComponentCache.Entry parentEntry = (ComponentCache.Entry)manager.Context[typeof(ComponentCache.Entry)];
                ComponentCache cache = (ComponentCache)manager.Context[typeof(ComponentCache)];

                if (parentEntry != null && parentEntry.Component != value /* don't make ourselves dependent with ourselves */ && cache != null)
                {
                    ComponentCache.Entry entry = null;
                    entry = cache.GetEntryAll(value);
                    if (entry != null && parentEntry.Component != null)
                    {
                        entry.AddDependency(parentEntry.Component);
                    }
                }
            }

            return expression;
        }

        /// <summary>
        ///  Returns the serializer for the given value.  This is cognizant that instance
        ///  attributes may be different from type attributes and will use a custom serializer
        ///  on the instance if it is present.  If not, it will delegate to the serialization
        ///  manager.
        /// </summary>
        protected CodeDomSerializer GetSerializer(IDesignerSerializationManager manager, object value)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (value != null)
            {
                AttributeCollection valueAttributes = GetAttributesHelper(manager, value);
                AttributeCollection typeAttributes = GetAttributesFromTypeHelper(manager, value.GetType());

                if (valueAttributes.Count != typeAttributes.Count)
                {
                    // Ok, someone has stuffed custom attributes on this instance.  Since the serialization manager only takes types, we've got to see if one of these custom attributes is a designer serializer attribute.
                    string valueSerializerTypeName = null;
                    Type desiredSerializerType = typeof(CodeDomSerializer);
                    DesignerSerializationManager vsManager = manager as DesignerSerializationManager;
                    foreach (Attribute a in valueAttributes)
                    {
                        if (a is DesignerSerializerAttribute da)
                        {
                            Type realSerializerType;
                            if (vsManager != null)
                            {
                                realSerializerType = vsManager.GetRuntimeType(da.SerializerBaseTypeName);
                            }
                            else
                            {
                                realSerializerType = manager.GetType(da.SerializerBaseTypeName);
                            }

                            if (realSerializerType == desiredSerializerType)
                            {
                                valueSerializerTypeName = da.SerializerTypeName;
                                break;
                            }
                        }
                    }

                    // If we got a value serializer, we've got to do the same thing here for the type serializer.  We only care if the two are different
                    if (valueSerializerTypeName != null)
                    {
                        foreach (Attribute a in typeAttributes)
                        {
                            if (a is DesignerSerializerAttribute da)
                            {
                                Type realSerializerType;
                                if (vsManager != null)
                                {
                                    realSerializerType = vsManager.GetRuntimeType(da.SerializerBaseTypeName);
                                }
                                else
                                {
                                    realSerializerType = manager.GetType(da.SerializerBaseTypeName);
                                }
                                if (realSerializerType == desiredSerializerType)
                                {
                                    // Ok, we found a serializer. If it matches the one we found for the value, then we can still use the default implementation.
                                    if (valueSerializerTypeName.Equals(da.SerializerTypeName))
                                    {
                                        valueSerializerTypeName = null;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // Finally, if we got a value serializer, we need to create it and use it.
                    if (valueSerializerTypeName != null)
                    {
                        Type serializerType = vsManager != null ? vsManager.GetRuntimeType(valueSerializerTypeName) :
                                                                  manager.GetType(valueSerializerTypeName);
                        if (serializerType != null && desiredSerializerType.IsAssignableFrom(serializerType))
                        {
                            return (CodeDomSerializer)Activator.CreateInstance(serializerType);
                        }
                    }
                }
            }

            // for serializing null, we pass null to the serialization manager otherwise, external IDesignerSerializationProviders wouldn't be given a chance to  serialize null their own special way.
            Type t = null;
            if (value != null)
            {
                t = value.GetType();
            }
            return (CodeDomSerializer)manager.GetSerializer(t, typeof(CodeDomSerializer));
        }

        /// <summary>
        ///  Returns the serializer for the given value.  This is cognizant that instance
        ///  attributes may be different from type attributes and will use a custom serializer
        ///  on the instance if it is present.  If not, it will delegate to the serialization
        ///  manager.
        /// </summary>
        protected CodeDomSerializer GetSerializer(IDesignerSerializationManager manager, Type valueType)
        {
            return manager.GetSerializer(valueType, typeof(CodeDomSerializer)) as CodeDomSerializer;
        }

        protected bool IsSerialized(IDesignerSerializationManager manager, object value)
        {
            return IsSerialized(manager, value, false);
        }

        /// <summary>
        ///  This method returns true if the given value has been serialized before.  For an object to
        ///  be considered serialized either it or another serializer must have called SetExpression, creating
        ///  a relationship between that object and a referring expression.
        /// </summary>
        protected bool IsSerialized(IDesignerSerializationManager manager, object value, bool honorPreset)
        {
            bool hasExpression = false;
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // Is the expression part of a prior SetExpression call?
            if (manager.Context[typeof(ExpressionTable)] is ExpressionTable table && table.GetExpression(value) != null && (!honorPreset || !table.ContainsPresetExpression(value)))
            {
                hasExpression = true;
            }
            Trace("IsSerialized called for object {0} : {1}", value, hasExpression);
            return hasExpression;
        }

        /// <summary>
        ///  This method can be used to serialize an expression that represents the creation of the given object.
        ///  It is aware of instance descriptors and will return true for isComplete if the entire configuration for the
        ///  instance could be achieved.
        /// </summary>
        protected CodeExpression SerializeCreationExpression(IDesignerSerializationManager manager, object value, out bool isComplete)
        {
            isComplete = false;
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            TypeConverter converter = TypeDescriptor.GetConverter(value);
            // See if there is an ExpressionContext with a preset value we're interested in.  If so, that will dictate our creation expression.
            if (manager.Context[typeof(ExpressionContext)] is ExpressionContext cxt && object.ReferenceEquals(cxt.PresetValue, value))
            {
                CodeExpression expression = cxt.Expression;
                //Okay, we found a preset creation expression. We just need to find if it isComplete.
                if (converter.CanConvertTo(typeof(InstanceDescriptor)))
                {
                    if (converter.ConvertTo(value, typeof(InstanceDescriptor)) is InstanceDescriptor descriptor && descriptor.MemberInfo != null)
                    {
                        isComplete = descriptor.IsComplete;
                    }
                }
                return expression;
            }

            // See if there is an instance descriptor for this type.
            if (converter.CanConvertTo(typeof(InstanceDescriptor)))
            {
                if (converter.ConvertTo(value, typeof(InstanceDescriptor)) is InstanceDescriptor descriptor && descriptor.MemberInfo != null)
                {
                    isComplete = descriptor.IsComplete;
                    return SerializeInstanceDescriptor(manager, value, descriptor);
                }
            }

            // see if this thing is serialiable
            if (GetReflectionTypeHelper(manager, value).IsSerializable && !(value is IComponent && ((IComponent)value).Site != null))
            {
                CodeExpression expression = SerializeToResourceExpression(manager, value);
                TraceIf(expression != null, "Serialized value as a resource.");
                if (expression != null)
                {
                    isComplete = true;
                    return expression;
                }
            }

            // No instance descriptor. See if we can get to a public constructor that takes no arguments
            ConstructorInfo ctor = GetReflectionTypeHelper(manager, value).GetConstructor(Array.Empty<Type>());
            if (ctor != null)
            {
                isComplete = false;
                return new CodeObjectCreateExpression(TypeDescriptor.GetClassName(value), Array.Empty<CodeExpression>());
            }
            // Nothing worked.
            return null;
        }

        private CodeExpression SerializeInstanceDescriptor(IDesignerSerializationManager manager, object value, InstanceDescriptor descriptor)
        {
            CodeExpression expression = null;
            using (TraceScope("CodeDomSerializerBase::SerializeInstanceDescriptor"))
            {
                Trace("Member : {0}, args : {1}", descriptor.MemberInfo.Name, descriptor.Arguments.Count);
                // Serialize all of the arguments.
                CodeExpression[] arguments = new CodeExpression[descriptor.Arguments.Count];
                object[] argumentValues = new object[arguments.Length];
                ParameterInfo[] parameters = null;

                if (arguments.Length > 0)
                {
                    descriptor.Arguments.CopyTo(argumentValues, 0);
                    MethodBase mi = descriptor.MemberInfo as MethodBase;
                    if (mi != null)
                    {
                        parameters = mi.GetParameters();
                    }
                }

                bool paramsOk = true;
                for (int i = 0; i < arguments.Length; i++)
                {
                    Debug.Assert(argumentValues != null && parameters != null, "These should have been allocated when the argument array was created.");
                    object arg = argumentValues[i];
                    CodeExpression exp = null;
                    ExpressionContext newCxt = null;

                    // If there is an ExpressionContext on the stack, we need to fix up its type to be the parameter type, so the argument objects get serialized correctly.
                    if (manager.Context[typeof(ExpressionContext)] is ExpressionContext cxt)
                    {
                        newCxt = new ExpressionContext(cxt.Expression, parameters[i].ParameterType, cxt.Owner);
                        manager.Context.Push(newCxt);
                    }

                    try
                    {
                        exp = SerializeToExpression(manager, arg);
                    }
                    finally
                    {
                        if (newCxt != null)
                        {
                            Debug.Assert(manager.Context.Current == newCxt, "Context stack corrupted.");
                            manager.Context.Pop();
                        }
                    }

                    if (exp != null)
                    {
                        // Assign over.  See if we need a cast first.
                        if (arg != null && !parameters[i].ParameterType.IsAssignableFrom(arg.GetType()))
                        {
                            exp = new CodeCastExpression(parameters[i].ParameterType, exp);
                        }
                        arguments[i] = exp;
                    }
                    else
                    {
                        TraceWarning("Parameter {0} in instance descriptor call {1} could not be serialized.", i, descriptor.GetType().Name);
                        paramsOk = false;
                        break;
                    }
                }

                if (paramsOk)
                {
                    Type expressionType = descriptor.MemberInfo.DeclaringType;
                    CodeTypeReference typeRef = new CodeTypeReference(expressionType);

                    if (descriptor.MemberInfo is ConstructorInfo)
                    {
                        expression = new CodeObjectCreateExpression(typeRef, arguments);
                    }
                    else if (descriptor.MemberInfo is MethodInfo)
                    {
                        CodeTypeReferenceExpression typeRefExp = new CodeTypeReferenceExpression(typeRef);
                        CodeMethodReferenceExpression methodRef = new CodeMethodReferenceExpression(typeRefExp, descriptor.MemberInfo.Name);
                        expression = new CodeMethodInvokeExpression(methodRef, arguments);
                        expressionType = ((MethodInfo)descriptor.MemberInfo).ReturnType;
                    }
                    else if (descriptor.MemberInfo is PropertyInfo)
                    {
                        CodeTypeReferenceExpression typeRefExp = new CodeTypeReferenceExpression(typeRef);
                        CodePropertyReferenceExpression propertyRef = new CodePropertyReferenceExpression(typeRefExp, descriptor.MemberInfo.Name);
                        Debug.Assert(arguments.Length == 0, "Property serialization does not support arguments");
                        expression = propertyRef;
                        expressionType = ((PropertyInfo)descriptor.MemberInfo).PropertyType;
                    }
                    else if (descriptor.MemberInfo is FieldInfo)
                    {
                        Debug.Assert(arguments.Length == 0, "Field serialization does not support arguments");
                        CodeTypeReferenceExpression typeRefExp = new CodeTypeReferenceExpression(typeRef);
                        expression = new CodeFieldReferenceExpression(typeRefExp, descriptor.MemberInfo.Name);
                        expressionType = ((FieldInfo)descriptor.MemberInfo).FieldType;
                    }
                    else
                    {
                        Debug.Fail("Unrecognized reflection type in instance descriptor: " + descriptor.MemberInfo.GetType().Name);
                    }

                    // Finally, check to see if our value is assignable from the expression type.  If not,  then supply a cast.  The value may be an internal or protected type; if it is, then walk up its hierarchy until we find one that is public.
                    Type targetType = value.GetType();
                    while (!targetType.IsPublic)
                    {
                        targetType = targetType.BaseType;
                    }

                    if (!targetType.IsAssignableFrom(expressionType))
                    {
                        Trace("Supplying cast from {0} to {1}.", expressionType.Name, targetType.Name);
                        expression = new CodeCastExpression(targetType, expression);
                    }
                }
            }
            return expression;
        }

        /// <summary>
        ///  This method returns a unique name for the given object.  It first calls GetName from the serialization
        ///  manager, and if this does not return a name if fabricates a name for the object.  To fabricate a name
        ///  it uses the INameCreationService to create valid names.  If the service is not available instead the
        ///  method will fabricate a name based on the short type name combined with an index number to make
        ///  it unique. The resulting name is associated with the serialization manager by calling SetName before
        ///  the new name is returned.
        /// </summary>
        protected string GetUniqueName(IDesignerSerializationManager manager, object value)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string name = manager.GetName(value);
            if (name is null)
            {
                string baseName;
                Type targetType = GetReflectionTypeHelper(manager, value);
                INameCreationService ns = manager.GetService(typeof(INameCreationService)) as INameCreationService;
                TraceWarningIf(ns is null, "Need to generate a unique name but we have no name creation service.");
                if (ns != null)
                {
                    baseName = ns.CreateName(null, targetType);
                }
                else
                {
                    baseName = targetType.Name.ToLower(CultureInfo.InvariantCulture);
                }

                int suffixIndex = 1;
                ComponentCache cache = manager.Context[typeof(ComponentCache)] as ComponentCache;
                // Declare this name to the serializer.  If there is already a name defined, keep trying.
                while (true)
                {
                    name = string.Format(CultureInfo.CurrentCulture, "{0}{1}", baseName, suffixIndex);
                    if (manager.GetInstance(name) is null && (cache is null || !cache.ContainsLocalName(name)))
                    {
                        manager.SetName(value, name);
                        if (manager.Context[typeof(ComponentCache.Entry)] is ComponentCache.Entry entry)
                        {
                            entry.AddLocalName(name);
                        }
                        break;
                    }
                    suffixIndex++;
                }
            }
            return name;
        }

        /// <summary>
        ///  This serializes a single event for the given object.
        /// </summary>
        protected void SerializeEvent(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, EventDescriptor descriptor)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            using (TraceScope("CodeDomSerializerBase::" + nameof(SerializeEvent)))
            {
                Trace("Name: {0}", descriptor.Name);
                // Now look for a MemberCodeDomSerializer for the property.  If we can't find one, then we can't serialize the property
                manager.Context.Push(statements);
                manager.Context.Push(descriptor);
                try
                {
                    MemberCodeDomSerializer memberSerializer = (MemberCodeDomSerializer)manager.GetSerializer(descriptor.GetType(), typeof(MemberCodeDomSerializer));

                    TraceErrorIf(memberSerializer is null, "Event {0} cannot be serialized because it has no serializer.", descriptor.Name);
                    if (memberSerializer != null && memberSerializer.ShouldSerialize(manager, value, descriptor))
                    {
                        memberSerializer.Serialize(manager, value, descriptor, statements);
                    }
                }
                finally
                {
                    Debug.Assert(manager.Context.Current == descriptor, "Context stack corrupted.");
                    manager.Context.Pop();
                    manager.Context.Pop();
                }
            }
        }

        /// <summary>
        ///  This serializes all events for the given object.
        /// </summary>
        protected void SerializeEvents(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, params Attribute[] filter)
        {
            Trace("CodeDomSerializerBase::" + nameof(SerializeEvents));
            EventDescriptorCollection events = GetEventsHelper(manager, value, filter).Sort();
            foreach (EventDescriptor evt in events)
            {
                SerializeEvent(manager, statements, value, evt);
            }
        }

        /// <summary>
        ///  This serializes all properties for the given object, using the provided filter.
        /// </summary>
        protected void SerializeProperties(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, Attribute[] filter)
        {
            using (TraceScope("CodeDomSerializerBase::" + nameof(SerializeProperties)))
            {
                PropertyDescriptorCollection properties = GetFilteredProperties(manager, value, filter).Sort();
                InheritanceAttribute inheritance = (InheritanceAttribute)GetAttributesHelper(manager, value)[typeof(InheritanceAttribute)];

                if (inheritance is null)
                {
                    inheritance = InheritanceAttribute.NotInherited;
                }

                manager.Context.Push(inheritance);
                try
                {
                    foreach (PropertyDescriptor property in properties)
                    {
                        if (!property.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden))
                        {
                            SerializeProperty(manager, statements, value, property);
                        }
                    }
                }
                finally
                {
                    Debug.Assert(manager.Context.Current == inheritance, "Sombody messed up our context stack.");
                    manager.Context.Pop();
                }
            }
        }
        private PropertyDescriptorCollection GetFilteredProperties(IDesignerSerializationManager manager, object value, Attribute[] filter)
        {
            PropertyDescriptorCollection props = GetPropertiesHelper(manager, value, filter);
            if (value is IComponent comp)
            {
                if (((IDictionary)props).IsReadOnly)
                {
                    PropertyDescriptor[] propArray = new PropertyDescriptor[props.Count];
                    props.CopyTo(propArray, 0);
                    props = new PropertyDescriptorCollection(propArray);
                }

                PropertyDescriptor filterProp = manager.Properties["FilteredProperties"];
                if (filterProp != null)
                {
                    if (filterProp.GetValue(manager) is ITypeDescriptorFilterService filterSvc)
                    {
                        filterSvc.FilterProperties(comp, props);
                    }
                }
            }
            return props;
        }

        /// <summary>
        ///  This method will inspect all of the properties on the given object fitting the filter, and check for that
        ///  property in a resource blob.  This is useful for deserializing properties that cannot be represented
        ///  in code, such as design-time properties.
        /// </summary>
        protected void SerializePropertiesToResources(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, Attribute[] filter)
        {
            using (TraceScope("ComponentCodeDomSerializerBase::" + nameof(SerializePropertiesToResources)))
            {
                PropertyDescriptorCollection props = GetPropertiesHelper(manager, value, filter);
                manager.Context.Push(statements);
                try
                {
                    CodeExpression target = SerializeToExpression(manager, value);
                    if (target != null)
                    {
                        CodePropertyReferenceExpression propertyRef = new CodePropertyReferenceExpression(target, string.Empty);
                        foreach (PropertyDescriptor property in props)
                        {
                            TraceWarningIf(property.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content), "PersistContents property " + property.Name + " cannot be serialized to resources.");
                            ExpressionContext tree = new ExpressionContext(propertyRef, property.PropertyType, value);
                            manager.Context.Push(tree);
                            try
                            {
                                if (property.Attributes.Contains(DesignerSerializationVisibilityAttribute.Visible))
                                {
                                    propertyRef.PropertyName = property.Name;
                                    Trace("Property : {0}", property.Name);

                                    string name;
                                    if (target is CodeThisReferenceExpression)
                                    {
                                        name = "$this";
                                    }
                                    else
                                    {
                                        name = manager.GetName(value);
                                    }

                                    name = string.Format(CultureInfo.CurrentCulture, "{0}.{1}", name, property.Name);
                                    ResourceCodeDomSerializer.Default.SerializeMetadata(manager, name, property.GetValue(value), property.ShouldSerializeValue(value));
                                }
                            }
                            finally
                            {
                                Debug.Assert(manager.Context.Current == tree, "Context stack corrupted.");
                                manager.Context.Pop();
                            }
                        }
                    }
                }
                finally
                {
                    Debug.Assert(manager.Context.Current == statements, "Context stack corrupted.");
                    manager.Context.Pop();
                }
            }
        }

        /// <summary>
        ///  This serializes the given proeprty for the given object.
        /// </summary>
        protected void SerializeProperty(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, PropertyDescriptor propertyToSerialize)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (propertyToSerialize is null)
            {
                throw new ArgumentNullException(nameof(propertyToSerialize));
            }

            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            Trace("CodeDomSerializerBase::" + nameof(SerializeProperty) + " {0}", propertyToSerialize.Name);
            // Now look for a MemberCodeDomSerializer for the property.  If we can't find one, then we can't serialize the property
            manager.Context.Push(statements);
            manager.Context.Push(propertyToSerialize);
            try
            {
                MemberCodeDomSerializer memberSerializer = (MemberCodeDomSerializer)manager.GetSerializer(propertyToSerialize.GetType(), typeof(MemberCodeDomSerializer));
                TraceErrorIf(memberSerializer is null, "Property {0} cannot be serialized because it has no serializer.", propertyToSerialize.Name);
                if (memberSerializer != null && memberSerializer.ShouldSerialize(manager, value, propertyToSerialize))
                {
                    memberSerializer.Serialize(manager, value, propertyToSerialize, statements);
                }
            }
            finally
            {
                Debug.Assert(manager.Context.Current == propertyToSerialize, "Context stack corrupted.");
                manager.Context.Pop();
                manager.Context.Pop();
            }
        }

        /// <summary>
        ///  Writes the given resource value under the given name.  The resource is written to the
        ///  current CultureInfo the user is using to design with.
        /// </summary>
        protected void SerializeResource(IDesignerSerializationManager manager, string resourceName, object value)
        {
            ResourceCodeDomSerializer.Default.WriteResource(manager, resourceName, value);
        }

        /// <summary>
        ///  Writes the given resource value under the given name.  The resource is written to the
        ///  invariant culture.
        /// </summary>
        protected void SerializeResourceInvariant(IDesignerSerializationManager manager, string resourceName, object value)
        {
            ResourceCodeDomSerializer.Default.WriteResourceInvariant(manager, resourceName, value);
        }

        /// <summary>
        ///  This is a helper method that serializes a value to an expression.  It will return a CodeExpression if the
        ///  value can be serialized, or null if it can't.  SerializeToExpression uses the following rules for serializing
        ///  types:
        ///  1.   It first calls GetExpression to see if an expression has already been created for the object.  If so, it
        ///  returns the existing expression.
        ///  2.   It then locates the object's serializer, and asks it to serialize.
        ///  3.   If the return value of the object's serializer is a CodeExpression, the expression is returned.
        ///  4.   It finally makes one last call to GetExpression to see if the serializer added an expression.
        ///  5.   Finally, it returns null.
        ///  If no expression could be created and no suitable serializer could be found, an error will be
        ///  reported through the serialization manager.  No error will be reported if a serializer was found
        ///  but it failed to produce an expression.  It is assumed that the serializer either already reported
        ///  the error, or does not wish to serialize the object.
        /// </summary>
        protected CodeExpression SerializeToExpression(IDesignerSerializationManager manager, object value)
        {
            CodeExpression expression = null;

            using (TraceScope("CodeDomSerializerBase::" + nameof(SerializeToExpression)))
            {
                // We do several things here:
                // First, we check to see if there is already an expression for this object by calling IsSerialized / GetExpression.
                // Failing that we check GetLegacyExpression to see if we are working with an old serializer.
                // Failing that, we invoke the object's serializer.  If that serializer returned a CodeExpression, we will use it.
                // If the serializer did not return a code expression, we call GetExpression one last time to see if the serializer added an expression.  If it did, we use it. Otherwise we return null.
                // If the serializer was invoked and it created one or more statements those statements will be added to a statement collection.  Additionally, if there is a statement context that contains a statement table for this object we will push that statement table onto the context stack in  case someone else needs statements.
                if (value != null)
                {
                    if (IsSerialized(manager, value))
                    {
                        expression = GetExpression(manager, value);
                        TraceIf(expression != null, "Existing expression found : {0}", expression);
                    }
                    else
                    {
                        expression = GetLegacyExpression(manager, value);
                        if (expression != null)
                        {
                            TraceWarning("Using legacy expression guard to prevent recursion.  Serializer for {0} should be rewritten to handle GetExpression / SetExpression.", value);
                            SetExpression(manager, value, expression);
                        }
                    }
                }

                if (expression is null)
                {
                    CodeDomSerializer serializer = GetSerializer(manager, value);
                    if (serializer != null)
                    {
                        Trace("Invoking serializer {0}", serializer.GetType().Name);
                        CodeStatementCollection saveStatements = null;
                        if (value != null)
                        {
                            // The Whidbey model for serializing a complex object is to call SetExpression with the object's reference expression and then  call on the various Serialize Property / Event methods.  This is incompatible with legacy code, and if not handled legacy code may serialize incorrectly or even stack fault.  To handle this, we keep a private "Legacy Expression Table".  This is a table that we fill in here.  We don't fill in the actual legacy expression here.  Rather,  we fill it with a marker value and obtain the legacy expression  above in GetLegacyExpression.  If we hit this case, we then save the expression in GetExpression so that future calls to IsSerialized will succeed.
                            SetLegacyExpression(manager, value);
                            if (manager.Context[typeof(StatementContext)] is StatementContext statementCxt)
                            {
                                saveStatements = statementCxt.StatementCollection[value];
                            }

                            if (saveStatements != null)
                            {
                                manager.Context.Push(saveStatements);
                            }
                        }

                        object result = null;
                        try
                        {
                            result = serializer.Serialize(manager, value);
                        }
                        finally
                        {
                            if (saveStatements != null)
                            {
                                Debug.Assert(manager.Context.Current == saveStatements, "Context stack corrupted.");
                                manager.Context.Pop();
                            }
                        }

                        expression = result as CodeExpression;
                        if (expression is null && value != null)
                        {
                            expression = GetExpression(manager, value);
                        }

                        // If the result is a statement or a group of statements, we need to see if there is a code statement collection on the stack we can push the statements into.
                        CodeStatementCollection statements = result as CodeStatementCollection;
                        if (statements is null)
                        {
                            if (result is CodeStatement statement)
                            {
                                statements = new CodeStatementCollection
                                {
                                    statement
                                };
                            }
                        }

                        if (statements != null)
                        {
                            Trace("Serialization produced additional statements");
                            // See if we have a place for these statements to be stored.  If not, then check the context.
                            if (saveStatements is null)
                            {
                                saveStatements = manager.Context[typeof(CodeStatementCollection)] as CodeStatementCollection;
                            }

                            if (saveStatements != null)
                            {
                                Trace("Saving in context stack statement collection");
                                Debug.Assert(saveStatements != statements, "The serializer returned the same collection that exists on the context stack.");
                                saveStatements.AddRange(statements);
                            }
                            else
                            {
                                // If we got here we will be losing data because we have no avenue to save these statements.  Inform the user.
                                string valueName = "(null)";
                                if (value != null)
                                {
                                    valueName = manager.GetName(value);
                                    if (valueName is null)
                                    {
                                        valueName = value.GetType().Name;
                                    }
                                }
                                TraceError("Serialization produced a set of statements but there is no statement collection on the stack to receive them.");
                                manager.ReportError(string.Format(SR.SerializerLostStatements, valueName));
                            }
                        }
                    }
                    else
                    {
                        TraceError("No serializer for data type: {0}", (value is null ? "(null)" : value.GetType().Name));
                        manager.ReportError(string.Format(SR.SerializerNoSerializerForComponent, value.GetType().FullName));
                    }
                }
            }
            return expression;
        }

        private CodeExpression GetLegacyExpression(IDesignerSerializationManager manager, object value)
        {
            CodeExpression expression = null;
            if (manager.Context[typeof(LegacyExpressionTable)] is LegacyExpressionTable table)
            {
                object exp = table[value];
                if (exp == value)
                {
                    // Sentinel.  Compute an actual legacy expression to store.
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
                        if (root != null)
                        {
                            if (root.Value == value)
                            {
                                expression = root.Expression;
                            }
                            else if (referenceName && name.IndexOf('.') != -1)
                            {
                                // if it's a reference name with a dot, we've actually got a property here...
                                int dotIndex = name.IndexOf('.');

                                expression = new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(root.Expression, name.Substring(0, dotIndex)), name.Substring(dotIndex + 1));
                            }
                            else
                            {
                                expression = new CodeFieldReferenceExpression(root.Expression, name);
                            }
                        }
                        else
                        {
                            // A variable reference
                            if (referenceName && name.IndexOf('.') != -1)
                            {
                                // if it's a reference name with a dot, we've actually got a property here...
                                int dotIndex = name.IndexOf('.');
                                expression = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(name.Substring(0, dotIndex)), name.Substring(dotIndex + 1));
                            }
                            else
                            {
                                expression = new CodeVariableReferenceExpression(name);
                            }
                        }
                    }
                    table[value] = expression;
                }
                else
                {
                    expression = exp as CodeExpression;
                }
            }
            return expression;
        }

        private void SetLegacyExpression(IDesignerSerializationManager manager, object value)
        {
            if (value is IComponent)
            {
                LegacyExpressionTable table = (LegacyExpressionTable)manager.Context[typeof(LegacyExpressionTable)];
                if (table is null)
                {
                    table = new LegacyExpressionTable();
                    manager.Context.Append(table);
                }
                table[value] = value;
            }
        }
        private class LegacyExpressionTable : Hashtable
        {
        }

        /// <summary>
        ///  Serializes the given object to a resource and returns a code expression that represents the resource.
        ///  This will return null if the value cannot be serialized.  If ensureInvariant is true, this will ensure that
        ///  new values make their way into the invariant culture.  Normally, this is desirable. Otherwise a resource
        ///  GetValue call could fail if reading from a culture that doesn't have a value.  You should only pass
        ///  false to ensureInvariant when you intend to read resources differently than directly asking for a value.
        ///  The default value of insureInvariant is true.
        /// </summary>
        protected CodeExpression SerializeToResourceExpression(IDesignerSerializationManager manager, object value)
        {
            return SerializeToResourceExpression(manager, value, true);
        }

        /// <summary>
        ///  Serializes the given object to a resource and returns a code expression that represents the resource.
        ///  This will return null if the value cannot be serialized.  If ensureInvariant is true, this will ensure that
        ///  new values make their way into the invariant culture.  Normally, this is desirable. Otherwise a resource
        ///  GetValue call could fail if reading from a culture that doesn't have a value.  You should only pass
        ///  false to ensureInvariant when you intend to read resources differently than directly asking for a value.
        ///  The default value of insureInvariant is true.
        /// </summary>
        protected CodeExpression SerializeToResourceExpression(IDesignerSerializationManager manager, object value, bool ensureInvariant)
        {
            CodeExpression result = null;
            if (value is null || value.GetType().IsSerializable)
            {
                CodeStatementCollection saveStatements = null;
                if (value != null)
                {
                    if (manager.Context[typeof(StatementContext)] is StatementContext statementCxt)
                    {
                        saveStatements = statementCxt.StatementCollection[value];
                    }

                    if (saveStatements != null)
                    {
                        manager.Context.Push(saveStatements);
                    }
                }

                try
                {
                    result = ResourceCodeDomSerializer.Default.Serialize(manager, value, false, ensureInvariant) as CodeExpression;
                }
                finally
                {
                    if (saveStatements != null)
                    {
                        Debug.Assert(manager.Context.Current == saveStatements, "Context stack corrupted.");
                        manager.Context.Pop();
                    }
                }
            }
            return result;
        }

        protected void SetExpression(IDesignerSerializationManager manager, object value, CodeExpression expression)
        {
            SetExpression(manager, value, expression, false);
        }

        /// <summary>
        ///  This is a helper method that associates a CodeExpression with an object.  Objects that have been associated
        ///  with expressions in this way are accessible through the GetExpression method.  SetExpression stores its
        ///  expression table as an appended object on the context stack so it is accessible by any serializer's
        ///  GetExpression method.
        /// </summary>
        protected void SetExpression(IDesignerSerializationManager manager, object value, CodeExpression expression, bool isPreset)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            ExpressionTable table = (ExpressionTable)manager.Context[typeof(ExpressionTable)];
            if (table is null)
            {
                table = new ExpressionTable();
                manager.Context.Append(table);
            }
            Trace("Set expression {0} for object {1}", expression, value);
            // in debug builds, save off who performed this set expression.  It's very valuable to know.
#if DEBUG
            if (traceSerialization.TraceVerbose)
            {
                expression.UserData["StackTrace"] = Environment.StackTrace;
            }
            else
            {
                // Default to off as snapping the stack takes too much time in DEBUG to leave on permanently.
                expression.UserData["StackTrace"] = "To enable - turn on DesignerSerialization TraceSwitch";
            }

            CodeExpression existingExpression = table.GetExpression(value);
            if (existingExpression != null && !isPreset)
            {
                Debug.Fail("There shouldn't be an expression already associated with this object : " + manager.GetName(value));
                if (!(existingExpression.UserData["StackTrace"] is string stack))
                {
                    stack = "unknown";
                }
                TraceWarning("Duplicate expression on context stack for value {0}.  Original expression callstack: {1}", value, stack);
            }
#endif
            table.SetExpression(value, expression, isPreset);
        }

        internal static void FillStatementTable(IDesignerSerializationManager manager, IDictionary table, CodeStatementCollection statements)
        {
            FillStatementTable(manager, table, null, statements, null);
        }

        internal static void FillStatementTable(IDesignerSerializationManager manager, IDictionary table, Dictionary<string, string> names, CodeStatementCollection statements, string className)
        {
            using (TraceScope("CodeDomSerializerBase::" + nameof(FillStatementTable)))
            {
                // Look in the method body to try to find statements with a LHS that points to a name in our nametable.
                foreach (CodeStatement statement in statements)
                {
                    CodeExpression expression = null;
                    if (statement is CodeAssignStatement assign)
                    {
                        Trace("Processing CodeAssignStatement");
                        expression = assign.Left;
                    }
                    else if (statement is CodeAttachEventStatement attachEvent)
                    {
                        Trace("Processing CodeAttachEventStatement");
                        expression = attachEvent.Event;
                    }
                    else if (statement is CodeRemoveEventStatement removeEvent)
                    {
                        Trace("Processing CodeRemoveEventStatement");
                        expression = removeEvent.Event;
                    }
                    else if (statement is CodeExpressionStatement expressionStmt)
                    {
                        Trace("Processing CodeExpressionStatement");
                        expression = expressionStmt.Expression;
                    }
                    else if (statement is CodeVariableDeclarationStatement variableDecl)
                    {
                        Trace("Processing CodeVariableDeclarationStatement");
                        AddStatement(table, variableDecl.Name, variableDecl);
                        if (names != null && variableDecl.Type != null && !string.IsNullOrEmpty(variableDecl.Type.BaseType))
                        {
                            names[variableDecl.Name] = GetTypeNameFromCodeTypeReference(manager, variableDecl.Type);
                        }
                        expression = null;
                    }

                    // Expressions we look for.
                    CodeDelegateCreateExpression delegateCreateEx;
                    CodeDelegateInvokeExpression delegateInvokeEx;
                    CodeDirectionExpression directionEx;
                    CodeEventReferenceExpression eventReferenceEx;
                    CodeMethodInvokeExpression methodInvokeEx;
                    CodeMethodReferenceExpression methodReferenceEx;
                    CodeArrayIndexerExpression arrayIndexerEx;
                    CodeFieldReferenceExpression fieldReferenceEx;
                    CodePropertyReferenceExpression propertyReferenceEx;
                    CodeVariableReferenceExpression variableReferenceEx;

                    if (expression != null)
                    {
                        // Simplify the expression as much as we can, looking for our target object in the process.  If we find an expression that refers to our target object, we're done and can move on to the next statement.
                        while (true)
                        {
                            if (expression is CodeCastExpression castEx)
                            {
                                Trace("Simplifying CodeCastExpression");
                                expression = castEx.Expression;
                            }
                            else if ((delegateCreateEx = expression as CodeDelegateCreateExpression) != null)
                            {
                                Trace("Simplifying CodeDelegateCreateExpression");
                                expression = delegateCreateEx.TargetObject;
                            }
                            else if ((delegateInvokeEx = expression as CodeDelegateInvokeExpression) != null)
                            {
                                Trace("Simplifying CodeDelegateInvokeExpression");
                                expression = delegateInvokeEx.TargetObject;
                            }
                            else if ((directionEx = expression as CodeDirectionExpression) != null)
                            {
                                Trace("Simplifying CodeDirectionExpression");
                                expression = directionEx.Expression;
                            }
                            else if ((eventReferenceEx = expression as CodeEventReferenceExpression) != null)
                            {
                                Trace("Simplifying CodeEventReferenceExpression");
                                expression = eventReferenceEx.TargetObject;
                            }
                            else if ((methodInvokeEx = expression as CodeMethodInvokeExpression) != null)
                            {
                                Trace("Simplifying CodeMethodInvokeExpression");
                                expression = methodInvokeEx.Method;
                            }
                            else if ((methodReferenceEx = expression as CodeMethodReferenceExpression) != null)
                            {
                                Trace("Simplifying CodeMethodReferenceExpression");
                                expression = methodReferenceEx.TargetObject;
                            }
                            else if ((arrayIndexerEx = expression as CodeArrayIndexerExpression) != null)
                            {
                                Trace("Simplifying CodeArrayIndexerExpression");
                                expression = arrayIndexerEx.TargetObject;
                            }
                            else if ((fieldReferenceEx = expression as CodeFieldReferenceExpression) != null)
                            {
                                // For fields we need to check to see if the field name is equal to the target object. If it is, then we have the expression we want.  We can add the statement here and then break out of our loop.
                                // Note:  We cannot validate that this is a name in our nametable.  The nametable only contains names we have discovered through code parsing and will not include data from any inherited objects.  We accept the field now, and then fail later if we try to resolve it to an object and we can't find it.
                                bool addedStatement = false;
                                if (fieldReferenceEx.TargetObject is CodeThisReferenceExpression)
                                {
                                    Type type = GetType(manager, fieldReferenceEx.FieldName, names);
                                    if (type != null)
                                    {
                                        if (manager.GetSerializer(type, typeof(CodeDomSerializer)) is CodeDomSerializer serializer)
                                        {
                                            string componentName = serializer.GetTargetComponentName(statement, expression, type);
                                            if (!string.IsNullOrEmpty(componentName))
                                            {
                                                AddStatement(table, componentName, statement);
                                                addedStatement = true;
                                            }
                                        }
                                    }
                                    if (!addedStatement)
                                    {
                                        // we still want to do this in case of the "Note" above.
                                        AddStatement(table, fieldReferenceEx.FieldName, statement);
                                    }
                                    break;
                                }
                                else
                                {
                                    Trace("Simplifying CodeFieldReferenceExpression");
                                    expression = fieldReferenceEx.TargetObject;
                                }
                            }
                            else if ((propertyReferenceEx = expression as CodePropertyReferenceExpression) != null)
                            {
                                // For properties we need to check to see if the property name is equal to the target object. If it is, then we have the expression we want.  We can add the statement here and then break out of our loop.
                                if (propertyReferenceEx.TargetObject is CodeThisReferenceExpression && (names is null || names.ContainsKey(propertyReferenceEx.PropertyName)))
                                {
                                    AddStatement(table, propertyReferenceEx.PropertyName, statement);
                                    break;
                                }
                                else
                                {
                                    Trace("Simplifying CodePropertyReferenceExpression");
                                    expression = propertyReferenceEx.TargetObject;
                                }
                            }
                            else if ((variableReferenceEx = expression as CodeVariableReferenceExpression) != null)
                            {
                                // For variables we need to check to see if the variable name is equal to the target object. If it is, then we have the expression we want.  We can add the statement here and then break out of our loop.
                                bool statementAdded = false;
                                if (names != null)
                                {
                                    Type type = GetType(manager, variableReferenceEx.VariableName, names);
                                    if (type != null)
                                    {
                                        if (manager.GetSerializer(type, typeof(CodeDomSerializer)) is CodeDomSerializer serializer)
                                        {
                                            string componentName = serializer.GetTargetComponentName(statement, expression, type);
                                            if (!string.IsNullOrEmpty(componentName))
                                            {
                                                AddStatement(table, componentName, statement);
                                                statementAdded = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    AddStatement(table, variableReferenceEx.VariableName, statement);
                                    statementAdded = true;
                                }
                                if (!statementAdded)
                                {
                                    TraceError("Variable {0} used before it was declared.", variableReferenceEx.VariableName);
                                    manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerUndeclaredName, variableReferenceEx.VariableName), manager));
                                }
                                break;
                            }
                            else if (expression is CodeThisReferenceExpression || expression is CodeBaseReferenceExpression)
                            {
                                // We cannot go any further than "this".  So, we break out of the loop.  We file this statement under the root object.
                                Debug.Assert(className != null, "FillStatementTable expected a valid className but received null");
                                if (className != null)
                                {
                                    AddStatement(table, className, statement);
                                }
                                break;
                            }
                            else
                            {
                                // We cannot simplify this expression any further, so we stop looping.
                                break;
                            }
                        }
                    }
                }
            }
        }
        internal static Type GetType(IDesignerSerializationManager manager, string name, Dictionary<string, string> names)
        {
            Type type = null;
            if (names != null && names.ContainsKey(name))
            {
                string typeName = names[name];
                if (manager != null && !string.IsNullOrEmpty(typeName))
                {
                    type = manager.GetType(typeName);
                }
            }
            return type;
        }

        private static void AddStatement(IDictionary table, string name, CodeStatement statement)
        {
            OrderedCodeStatementCollection statements;
            if (table.Contains(name))
            {
                statements = (OrderedCodeStatementCollection)table[name];
            }
            else
            {
                // push in an order key so we know what position this item was in the list of declarations. this allows us to preserve ZOrder.
                statements = new OrderedCodeStatementCollection
                {
                    Order = table.Count,
                    Name = name
                };
                table[name] = statements;
            }
            statements.Add(statement);
        }

        internal class OrderedCodeStatementCollection : CodeStatementCollection
        {
            public int Order;
            public string Name;
        }
    }
}
