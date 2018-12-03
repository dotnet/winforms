// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///     This base class is used as a shared base between CodeDomSerializer and TypeCodeDomSerializer.
    ///     It is not meant to be publicly derived from.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class CodeDomSerializerBase
    {
        /// <summary>
        ///     Internal constructor so only we can derive from this class.
        /// </summary>
        internal CodeDomSerializerBase()
        {
        }

        /// <summary>
        ///     This method is invoked during deserialization to obtain an instance of an object.  When this is called, an instance
        ///     of the requested type should be returned.  The default implementation invokes manager.CreateInstance.
        /// </summary>
        protected virtual object DeserializeInstance(IDesignerSerializationManager manager, Type type,
            object[] parameters, string name, bool addToContainer)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Return a target framework-aware TypeDescriptionProvider which can be used for type filtering
        /// </summary>
        protected static TypeDescriptionProvider GetTargetFrameworkProvider(IServiceProvider provider, object instance)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Get a faux type which is generated from the metadata, which is
        ///     looked up on the target framerwork assembly. Be careful to not use mix
        ///     this type with runtime types in comparisons!
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification =
            "we pass serialization manager to Error")]
        protected static Type GetReflectionTypeFromTypeHelper(IDesignerSerializationManager manager, Type type)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Get a faux type which is generated based on the metadata
        ///     looked up on the target framework assembly.
        ///     Never pass a type returned from GetReflectionType to runtime APIs that need a type.
        ///     Call GetRuntimeType first to unwrap the reflection type.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification =
            "we pass serialization manager to Error")]
        protected static Type GetReflectionTypeHelper(IDesignerSerializationManager manager, object instance)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Get properties collection as defined in the project target framework
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification =
            "we pass serialization manager to Error")]
        protected static PropertyDescriptorCollection GetPropertiesHelper(IDesignerSerializationManager manager,
            object instance, Attribute[] attributes)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Get events collection as defined in the project target framework
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification =
            "we pass serialization manager to Error")]
        protected static EventDescriptorCollection GetEventsHelper(IDesignerSerializationManager manager,
            object instance, Attribute[] attributes)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Get attributes collection as defined in the project target framework
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification =
            "we pass serialization manager to Error")]
        protected static AttributeCollection GetAttributesHelper(IDesignerSerializationManager manager, object instance)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Get attributes collection as defined in the project target framework
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification =
            "we pass serialization manager to Error")]
        protected static AttributeCollection GetAttributesFromTypeHelper(IDesignerSerializationManager manager,
            Type type)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method will inspect all of the properties on the given object fitting the filter, and check for that
        ///     property in a resource blob.  This is useful for deserializing properties that cannot be represented
        ///     in code, such as design-time properties.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        protected void DeserializePropertiesFromResources(IDesignerSerializationManager manager, object value,
            Attribute[] filter)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This is a helper method that will deserialize a given statement.  It deserializes
        ///     the statement by interpreting and executing the CodeDom statement.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        protected void DeserializeStatement(IDesignerSerializationManager manager, CodeStatement statement)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method returns an expression representing the given object.  It may return null, indicating that
        ///     no expression has been set that describes the object.  Expressions are aquired in one of three ways:
        ///     1.   The expression could be the result of a prior SetExpression call.
        ///     2.   The expression could have been found in the RootContext.
        ///     3.   The expression could be derived through IReferenceService.
        ///     4.   The current expression on the context stack has a PresetValue == value.
        ///     To derive expressions through IReferenceService, GetExpression asks the reference service if there
        ///     is a name for the given object.  If the expression service returns a valid name, it checks to see if
        ///     there is a '.' in the name.  This indicates that the expression service found this object as the return
        ///     value of a read only property on another object.  If there is a '.', GetExpression will split the reference
        ///     into sub-parts.  The leftmost part is a name that will be evalulated via manager.GetInstance.  For each
        ///     subsequent part, a property reference expression will be built.  The final expression will then be returned.
        ///     If the object did not have an expression set, or the object was not found in the reference service, null will
        ///     be returned from GetExpression, indicating there is no existing expression for the object.
        /// </summary>
        protected CodeExpression GetExpression(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the serializer for the given value.  This is cognizant that instance
        ///     attributes may be different from type attributes and will use a custom serializer
        ///     on the instance if it is present.  If not, it will delegate to the serialization
        ///     manager.
        /// </summary>
        protected CodeDomSerializer GetSerializer(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the serializer for the given value.  This is cognizant that instance
        ///     attributes may be different from type attributes and will use a custom serializer
        ///     on the instance if it is present.  If not, it will delegate to the serialization
        ///     manager.
        /// </summary>
        protected CodeDomSerializer GetSerializer(IDesignerSerializationManager manager, Type valueType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
        protected bool IsSerialized(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method returns true if the given value has been serialized before.  For an object to
        ///     be considered serialized either it or another serializer must have called SetExpression, creating
        ///     a relationship between that object and a referring expression.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
        protected bool IsSerialized(IDesignerSerializationManager manager, object value, bool honorPreset)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method can be used to serialize an expression that represents the creation of the given object.
        ///     It is aware of instance descriptors and will return true for isComplete if the entire configuration for the
        ///     instance could be achieved.
        /// </summary>
        protected CodeExpression SerializeCreationExpression(IDesignerSerializationManager manager, object value,
            out bool isComplete)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method returns a unique name for the given object.  It first calls GetName from the serialization
        ///     manager, and if this does not return a name if fabricates a name for the object.  To fabricate a name
        ///     it uses the INameCreationService to create valid names.  If the service is not available instead the
        ///     method will fabricate a name based on the short type name combined with an index number to make
        ///     it unique. The resulting name is associated with the serialization manager by calling SetName before
        ///     the new name is returned.
        /// </summary>
        protected string GetUniqueName(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This serializes a single event for the given object.
        /// </summary>
        protected void SerializeEvent(IDesignerSerializationManager manager, CodeStatementCollection statements,
            object value, EventDescriptor descriptor)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This serializes all events for the given object.
        /// </summary>
        protected void SerializeEvents(IDesignerSerializationManager manager, CodeStatementCollection statements,
            object value, params Attribute[] filter)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This serializes all properties for the given object, using the provided filter.
        /// </summary>
        protected void SerializeProperties(IDesignerSerializationManager manager, CodeStatementCollection statements,
            object value, Attribute[] filter)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method will inspect all of the properties on the given object fitting the filter, and check for that
        ///     property in a resource blob.  This is useful for deserializing properties that cannot be represented
        ///     in code, such as design-time properties.
        /// </summary>
        protected void SerializePropertiesToResources(IDesignerSerializationManager manager,
            CodeStatementCollection statements, object value, Attribute[] filter)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This serializes the given proeprty for the given object.
        /// </summary>
        protected void SerializeProperty(IDesignerSerializationManager manager, CodeStatementCollection statements,
            object value, PropertyDescriptor propertyToSerialize)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Writes the given resource value under the given name.  The resource is written to the
        ///     current CultureInfo the user is using to design with.
        /// </summary>
        protected void SerializeResource(IDesignerSerializationManager manager, string resourceName, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Writes the given resource value under the given name.  The resource is written to the
        ///     invariant culture.
        /// </summary>
        protected void SerializeResourceInvariant(IDesignerSerializationManager manager, string resourceName,
            object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This is a helper method that serializes a value to an expression.  It will return a CodeExpression if the
        ///     value can be serialized, or null if it can't.  SerializeToExpression uses the following rules for serializing
        ///     types:
        ///     1.   It first calls GetExpression to see if an expression has already been created for the object.  If so, it
        ///     returns the existing expression.
        ///     2.   It then locates the object's serializer, and asks it to serialize.
        ///     3.   If the return value of the object's serializer is a CodeExpression, the expression is returned.
        ///     4.   It finally makes one last call to GetExpression to see if the serializer added an expression.
        ///     5.   Finally, it returns null.
        ///     If no expression could be created and no suitable serializer could be found, an error will be
        ///     reported through the serialization manager.  No error will be reported if a serializer was found
        ///     but it failed to produce an expression.  It is assumed that the serializer either already reported
        ///     the error, or does not wish to serialize the object.
        /// </summary>
        protected CodeExpression SerializeToExpression(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Serializes the given object to a resource and returns a code expression that represents the resource.
        ///     This will return null if the value cannot be serialized.  If ensureInvariant is true, this will ensure that
        ///     new values make their way into the invariant culture.  Normally, this is desirable. Otherwise a resource
        ///     GetValue call could fail if reading from a culture that doesn't have a value.  You should only pass
        ///     false to ensureInvariant when you intend to read resources differently than directly asking for a value.
        ///     The default value of insureInvariant is true.
        /// </summary>
        protected CodeExpression SerializeToResourceExpression(IDesignerSerializationManager manager, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Serializes the given object to a resource and returns a code expression that represents the resource.
        ///     This will return null if the value cannot be serialized.  If ensureInvariant is true, this will ensure that
        ///     new values make their way into the invariant culture.  Normally, this is desirable. Otherwise a resource
        ///     GetValue call could fail if reading from a culture that doesn't have a value.  You should only pass
        ///     false to ensureInvariant when you intend to read resources differently than directly asking for a value.
        ///     The default value of insureInvariant is true.
        /// </summary>
        protected CodeExpression SerializeToResourceExpression(IDesignerSerializationManager manager, object value,
            bool ensureInvariant)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
        protected void SetExpression(IDesignerSerializationManager manager, object value, CodeExpression expression)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This is a helper method that associates a CodeExpression with an object.  Objects that have been associated
        ///     with expressions in this way are accessible through the GetExpression method.  SetExpression stores its
        ///     expression table as an appended object on the context stack so it is accessible by any serializer's
        ///     GetExpression method.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
        protected void SetExpression(IDesignerSerializationManager manager, object value, CodeExpression expression,
            bool isPreset)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
