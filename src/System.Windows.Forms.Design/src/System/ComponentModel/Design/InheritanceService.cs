// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

/// <summary>
///  Provides a set of methods for analyzing and identifying inherited components.
/// </summary>
public class InheritanceService : IInheritanceService, IDisposable
{
    private Dictionary<IComponent, InheritanceAttribute> _inheritedComponents;

    // While we're adding an inherited component, we must be wary of components that the inherited component adds as a
    // result of being sited. These are treated as inherited as well. To track these, we keep track of the component
    // we're currently adding as well as it's inheritance attribute. During the add, we sync IComponentAdding events
    // and push in the component.
    private IComponent? _addingComponent;
    private InheritanceAttribute? _addingAttribute;

    /// <summary>
    ///  Initializes a new instance of the <see cref="InheritanceService"/> class.
    /// </summary>
    public InheritanceService()
    {
        _inheritedComponents = [];
    }

    /// <summary>
    ///  Disposes of the resources (other than memory) used by the <see cref="InheritanceService"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && _inheritedComponents is not null)
        {
            _inheritedComponents.Clear();
            _inheritedComponents = null!;
        }
    }

    /// <summary>
    ///  Adds inherited components to the <see cref="InheritanceService"/>.
    /// </summary>
    public void AddInheritedComponents(IComponent component, IContainer container)
    {
        AddInheritedComponents(component.GetType(), component, container);
    }

    /// <summary>
    ///  Adds inherited components to the <see cref="InheritanceService"/>.
    /// </summary>
    protected virtual void AddInheritedComponents(Type? type, IComponent component, IContainer container)
    {
        // We get out now if this component type is not assignable from IComponent. We only walk down to the component level.
        if (type is null || !typeof(IComponent).IsAssignableFrom(type))
        {
            return;
        }

        ISite? site = component.Site;
        IComponentChangeService? cs = null;
        INameCreationService? ncs = null;

        if (site is not null)
        {
            ncs = site.GetService<INameCreationService>();
            cs = site.GetService<IComponentChangeService>();
            if (cs is not null)
            {
                cs.ComponentAdding += OnComponentAdding;
            }
        }

        try
        {
            while (type != typeof(object))
            {
                Type reflect = TypeDescriptor.GetReflectionType(type);
                FieldInfo[] fields = reflect.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (FieldInfo field in fields)
                {
                    string name = field.Name;

                    // Get out now if this field is not assignable from IComponent.
                    Type reflectionType = GetReflectionTypeFromTypeHelper(field.FieldType);
                    if (!GetReflectionTypeFromTypeHelper(typeof(IComponent)).IsAssignableFrom(reflectionType))
                    {
                        continue;
                    }

                    // Now check the attributes of the field and get out if it isn't something that can be inherited.
                    Debug.Assert(!field.IsStatic, "Instance binding shouldn't have found this field");

                    // If the value of the field is null, then don't mess with it. If it wasn't assigned when our base
                    // class was created then we can't really use it.
                    object? value = field.GetValue(component);
                    if (value is null)
                    {
                        continue;
                    }

                    // We've been fine up to this point looking at the field. Now, however, we must check to see if this
                    // field has an AccessedThroughPropertyAttribute on it. If it does, then we must look for the property
                    // and use its name and visibility for the remainder of the scan. Should any of this bail we just use the field.
                    MemberInfo member = field;

                    object[] fieldAttrs = field.GetCustomAttributes(typeof(AccessedThroughPropertyAttribute), false);
                    if (fieldAttrs is not null && fieldAttrs.Length > 0)
                    {
                        Debug.Assert(fieldAttrs.Length == 1, "Non-inheritable attribute has more than one copy");
                        Debug.Assert(fieldAttrs[0] is AccessedThroughPropertyAttribute, "Reflection bug:  GetCustomAttributes(type) didn't discriminate by type");
                        AccessedThroughPropertyAttribute propAttr = (AccessedThroughPropertyAttribute)fieldAttrs[0];

                        PropertyInfo? fieldProp = reflect.GetProperty(propAttr.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        Debug.Assert(fieldProp is not null, "Field declared with AccessedThroughPropertyAttribute has no associated property");
                        Debug.Assert(fieldProp.PropertyType == field.FieldType, "Field declared with AccessedThroughPropertyAttribute is associated with a property with a different return type.");
                        if (fieldProp is not null && fieldProp.PropertyType == field.FieldType)
                        {
                            // If the property cannot be read, it is useless to us.
                            if (!fieldProp.CanRead)
                            {
                                continue;
                            }

                            // We never access the set for the property, so we can concentrate on just the get method.
                            member = fieldProp.GetGetMethod(true)!;
                            Debug.Assert(member is not null, "GetGetMethod for property didn't return a method, but CanRead is true");
                            name = propAttr.PropertyName;
                        }
                    }

                    // Add a user hook to add or remove members. The default hook here ignores all inherited private members.
                    bool ignoreMember = IgnoreInheritedMember(member, component);

                    // We now have an inherited member. Gather some information about it and then add it to our list.
                    // We must always add to our list, but we may not want to add it to the container. That is up to
                    // the IgnoreInheritedMember method. We add here because there are components in the world that,
                    // when sited, add their children to the container too. That's fine, but we want to make sure we
                    // account for them in the inheritance service too.

                    InheritanceAttribute attr;

                    Debug.Assert(value is IComponent, "Value of inherited field is not IComponent. How did this value get into the datatype?");

                    bool privateInherited = false;

                    if (ignoreMember)
                    {
                        // If we are ignoring this member, then always mark it as private. The designer doesn't want it;
                        // we only do this in case some other component adds this guy to the container.
                        privateInherited = true;
                    }
                    else
                    {
                        if (member is FieldInfo fi)
                        {
                            privateInherited = fi.IsPrivate | fi.IsAssembly;
                        }
                        else if (member is MethodInfo mi)
                        {
                            privateInherited = mi.IsPrivate | mi.IsAssembly;
                        }
                    }

                    if (privateInherited)
                    {
                        attr = InheritanceAttribute.InheritedReadOnly;
                    }
                    else
                    {
                        attr = InheritanceAttribute.Inherited;
                    }

                    // We only get values via IComponent Keys and don't expose any other behaviors.
                    // So only use IComponent as a key.
                    if (value is IComponent compValue)
                    {
                        bool notPresent = !_inheritedComponents.ContainsKey(compValue);
                        _inheritedComponents[compValue] = attr;

                        if (!ignoreMember && notPresent)
                        {
                            try
                            {
                                _addingComponent = compValue;
                                _addingAttribute = attr;

                                // Lets make sure this is a valid name
                                if (ncs is null || ncs.IsValidName(name))
                                {
                                    try
                                    {
                                        container.Add(compValue, name);
                                    }
                                    catch
                                    {
                                        // We do not always control the base components, and there could be a lot of rogue
                                        // base components. If there are exceptions when adding them, lets just ignore and continue.
                                    }
                                }
                            }
                            finally
                            {
                                _addingComponent = null;
                                _addingAttribute = null;
                            }
                        }
                    }
                }

                type = type.BaseType!;
            }
        }
        finally
        {
            if (cs is not null)
            {
                cs.ComponentAdding -= OnComponentAdding;
            }
        }
    }

    /// <summary>
    ///  Indicates the inherited members to ignore.
    /// </summary>
    protected virtual bool IgnoreInheritedMember(MemberInfo member, IComponent? component)
    {
        // Our default implementation ignores all private or assembly members.
        if (member is FieldInfo field)
        {
            return field.IsPrivate || field.IsAssembly;
        }
        else if (member is MethodInfo method)
        {
            return method.IsPrivate || method.IsAssembly;
        }

        Debug.Fail("Unknown member type passed to IgnoreInheritedMember");
        return true;
    }

    /// <summary>
    ///  Gets the inheritance attribute of the specified component.
    /// </summary>
    public InheritanceAttribute GetInheritanceAttribute(IComponent component)
        => _inheritedComponents.TryGetValue(component, out InheritanceAttribute? attr)
            ? attr
            : InheritanceAttribute.Default;

    private void OnComponentAdding(object? sender, ComponentEventArgs ce)
    {
        if (_addingComponent is not null && _addingComponent != ce.Component)
        {
            _inheritedComponents[ce.Component!] = InheritanceAttribute.InheritedReadOnly;

            // If this component is being added to a nested container of addingComponent, it should get the same inheritance level.
            if (sender is INestedContainer nested && nested.Owner == _addingComponent)
            {
                _inheritedComponents[ce.Component!] = _addingAttribute!;
            }
        }
    }

    [return: NotNullIfNotNull(nameof(type))]
    private static Type? GetReflectionTypeFromTypeHelper(Type? type)
    {
        if (type is not null)
        {
            TypeDescriptionProvider? targetProvider = GetTargetFrameworkProviderForType(type);
            if (targetProvider is not null)
            {
                if (targetProvider.IsSupportedType(type))
                {
                    return targetProvider.GetReflectionType(type);
                }
            }
        }

        return type;
    }

    private static TypeDescriptionProvider? GetTargetFrameworkProviderForType(Type type)
    {
        return DocumentDesigner.s_manager?.GetService<TypeDescriptionProviderService>()?.GetProvider(type);
    }
}
