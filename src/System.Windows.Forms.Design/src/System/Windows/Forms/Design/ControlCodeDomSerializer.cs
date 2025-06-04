// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace System.Windows.Forms.Design;

/// <summary>
///  Control's provide their own serializer so they can write out resource hierarchy
///  information. We delegate nearly everything to our base class's serializer.
/// </summary>
internal class ControlCodeDomSerializer : CodeDomSerializer
{
    /// <summary>
    ///  Deserializes the given CodeDom object into a real object. This
    ///  will use the serialization manager to create objects and resolve
    ///  data types. The root of the object graph is returned.
    /// </summary>
    public override object? Deserialize(IDesignerSerializationManager manager, object codeObject)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(codeObject);

        // Attempt to suspend all components within the icontainer
        IContainer? container = manager.GetService<IContainer>();
        List<Control>? suspendedComponents = null;

        if (container is not null)
        {
            suspendedComponents = new(container.Components.Count);

            foreach (IComponent component in container.Components)
            {
                if (component is Control control)
                {
                    control.SuspendLayout();

                    // Add this control to our suspended components list so we can resume later
                    suspendedComponents.Add(control);
                }
            }
        }

        object? objectGraphData = null;

        try
        {
            // Find our base class's serializer.
            if (!manager.TryGetSerializer(typeof(Component), out CodeDomSerializer? serializer))
            {
                Debug.Fail("Unable to find a CodeDom serializer for 'Component'. Has someone tampered with the serialization providers?");

                return null;
            }

            objectGraphData = serializer.Deserialize(manager, codeObject);
        }
        finally
        {
            // resume all suspended comps we found earlier
            if (suspendedComponents is not null)
            {
                foreach (Control control in suspendedComponents)
                {
                    // .NET Framework 4.0 (Dev10 #462211): Controls in design time may change their size due to incorrectly
                    // calculated anchor info.
                    // UNDONE: c.ResumeLayout(false) because it regressed layouts with Dock property
                    // see Dev11 bug 117530 DTS Winforms: Upgraded project -Control location and size are changed in the designer gen'd code
                    control.ResumeLayout(performLayout: true);
                }
            }
        }

        return objectGraphData;
    }

    private static bool HasAutoSizedChildren(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            if (child.AutoSize)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasMixedInheritedChildren(Control parent)
    {
        bool inheritedChildren = false;
        bool nonInheritedChildren = false;

        foreach (Control control in parent.Controls)
        {
            if (TypeDescriptorHelper.TryGetAttribute(control, out InheritanceAttribute? inheritanceAttribute)
                && inheritanceAttribute.InheritanceLevel != InheritanceLevel.NotInherited)
            {
                inheritedChildren = true;
            }
            else
            {
                nonInheritedChildren = true;
            }

            if (inheritedChildren && nonInheritedChildren)
            {
                return true;
            }
        }

        return false;
    }

    protected virtual bool HasSitedNonReadonlyChildren(Control parent)
    {
        if (!parent.HasChildren)
        {
            return false;
        }

        foreach (Control control in parent.Controls)
        {
            if (control.Site is not null && control.Site.DesignMode)
            {
                // We only emit Size/Location information for controls that are sited and not inherited readonly.
                if (TypeDescriptorHelper.TryGetAttribute(control, out InheritanceAttribute? inheritanceAttribute)
                    && inheritanceAttribute.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object.
    /// </summary>
    public override object? Serialize(IDesignerSerializationManager manager, object value)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        // Find our base class's serializer.
        if (!manager.TryGetSerializer(typeof(Component), out CodeDomSerializer? serializer))
        {
            Debug.Fail("Unable to find a CodeDom serializer for 'Component'. Has someone tampered with the serialization providers?");

            return null;
        }

        // Now ask it to serializer
        object? retVal = serializer.Serialize(manager, value);
        InheritanceLevel inheritanceLevel = InheritanceLevel.NotInherited;

        if (TypeDescriptorHelper.TryGetAttribute(value, out InheritanceAttribute? inheritanceAttribute))
        {
            inheritanceLevel = inheritanceAttribute.InheritanceLevel;
        }

        if (inheritanceLevel != InheritanceLevel.InheritedReadOnly)
        {
            // Next, see if we are in localization mode. If we are, and if we can get
            // to a ResourceManager through the service provider, then emit the hierarchy information for
            // this object. There is a small fragile assumption here:  The resource manager is demand
            // created, so if all of the properties of this control had default values it is possible
            // there will be no resource manager for us. I'm letting that slip a bit, however, because
            // for Control classes, we always emit at least the location / size information for the
            // control.
            IDesignerHost? host = manager.GetService<IDesignerHost>();

            if (host is not null)
            {
                if (TypeDescriptorHelper.TryGetPropertyValue(host.RootComponent, "Localizable", out bool b) && b)
                {
                    SerializeControlHierarchy(manager, host, value);
                }
            }

            if (retVal is CodeStatementCollection csCollection)
            {
                Control control = (Control)value;

                // Serialize a suspend / resume pair. We always serialize this
                // for the root component
                if ((host is not null && control == host.RootComponent) || HasSitedNonReadonlyChildren(control))
                {
                    SerializeSuspendLayout(manager, csCollection, value);
                    SerializeResumeLayout(manager, csCollection, value);
                    ControlDesigner? controlDesigner = host?.GetDesigner(control) as ControlDesigner;

                    if (HasAutoSizedChildren(control) || (controlDesigner is not null && controlDesigner.SerializePerformLayout))
                    {
                        SerializePerformLayout(manager, csCollection, value);
                    }
                }

                // And now serialize the correct z-order relationships for the controls. We only need to
                // do this if there are controls in the collection that are inherited.
                if (HasMixedInheritedChildren(control))
                {
                    SerializeZOrder(manager, csCollection, control);
                }
            }
        }

        return retVal;
    }

    /// <summary>
    ///  This writes out our control hierarchy information if there is a resource manager available for us to write to.
    /// </summary>
    private void SerializeControlHierarchy(IDesignerSerializationManager manager, IDesignerHost host, object? value)
    {
        if (value is Control control)
        {
            // Object name
            string? name;
            IMultitargetHelperService? multiTargetHelper = host.GetService<IMultitargetHelperService>();

            if (control == host.RootComponent)
            {
                name = "$this";

                // For the root component, we also take this as
                // an opportunity to emit information for all non-visual components in the container too.
                foreach (IComponent component in host.Container.Components)
                {
                    // Skip controls
                    if (component is Control)
                    {
                        continue;
                    }

                    // Skip privately inherited components
                    if (TypeDescriptor.GetAttributes(component).Contains(InheritanceAttribute.InheritedReadOnly))
                    {
                        continue;
                    }

                    // Now emit the data
                    string? componentName = manager.GetName(component);

                    if (componentName is not null)
                    {
                        SerializeResourceInvariant(manager, $">>{componentName}.Name", componentName);

                        string? componentTypeName = multiTargetHelper is null ? component.GetType().AssemblyQualifiedName : multiTargetHelper.GetAssemblyQualifiedName(component.GetType());
                        SerializeResourceInvariant(manager, $">>{componentName}.Type", componentTypeName);
                    }
                }
            }
            else
            {
                name = manager.GetName(value);

                // if we get null back, this must be an un-sited control
                if (name is null)
                {
                    Debug.Assert(value is not IComponent { Site: not null }, "Unnamed, sited control in hierarchy");
                    return;
                }
            }

            SerializeResourceInvariant(manager, $">>{name}.Name", manager.GetName(value));

            // Object type
            SerializeResourceInvariant(manager, $">>{name}.Type", multiTargetHelper is null ? control.GetType().AssemblyQualifiedName : multiTargetHelper.GetAssemblyQualifiedName(control.GetType()));

            // Parent
            Control? parent = control.Parent;

            if (parent is not null && parent.Site is not null)
            {
                string? parentName;

                if (parent == host.RootComponent)
                {
                    parentName = "$this";
                }
                else
                {
                    parentName = manager.GetName(parent);
                }

                if (parentName is not null)
                {
                    SerializeResourceInvariant(manager, $">>{name}.Parent", parentName);
                }

                // Z-Order
                for (int i = 0; i < parent.Controls.Count; i++)
                {
                    if (parent.Controls[i] == control)
                    {
                        SerializeResourceInvariant(manager, $">>{name}.ZOrder", i);
                        break;
                    }
                }
            }
        }
    }

    private enum StatementOrdering
    {
        Prepend,
        Append
    }

    private static Type ToTargetType(object context, Type runtimeType)
    {
        return TypeDescriptor.GetProvider(context).GetReflectionType(runtimeType);
    }

    private static Type[] ToTargetTypes(object context, Type[] runtimeTypes)
    {
        Type[] types = new Type[runtimeTypes.Length];

        for (int i = 0; i < runtimeTypes.Length; i++)
        {
            types[i] = ToTargetType(context, runtimeTypes[i]);
        }

        return types;
    }

    /// <summary>
    ///  Serializes a method invocation on the control being serialized. Used to serialize Suspend/ResumeLayout pairs, etc.
    /// </summary>
    private void SerializeMethodInvocation(
        IDesignerSerializationManager manager,
        CodeStatementCollection statements,
        object control,
        string methodName,
        CodeExpressionCollection? parameters,
        Type[] paramTypes,
        StatementOrdering ordering)
    {
        // Not doing anything with the name, keeping the access for compat.
        _ = manager.GetName(control);

        // Use IReflect to see if this method name exists on the control.
        paramTypes = ToTargetTypes(control, paramTypes);
        if (TypeDescriptor.GetReflectionType(control).GetMethod(
            methodName,
            BindingFlags.Public | BindingFlags.Instance,
            binder: null,
            paramTypes,
            modifiers: null) is null)
        {
            return;
        }

        CodeExpression? field = SerializeToExpression(manager, control);
        CodeMethodReferenceExpression method = new(field, methodName);
        CodeMethodInvokeExpression methodInvoke = new()
        {
            Method = method
        };

        if (parameters is not null)
        {
            methodInvoke.Parameters.AddRange(parameters);
        }

        CodeExpressionStatement statement = new(methodInvoke);

        switch (ordering)
        {
            case StatementOrdering.Prepend:
                statement.UserData["statement-ordering"] = "begin";
                break;
            case StatementOrdering.Append:
                statement.UserData["statement-ordering"] = "end";
                break;
            default:
                Debug.Fail($"Unsupported statement ordering: {ordering}");
                break;
        }

        statements.Add(statement);
    }

    private void SerializePerformLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
    {
        SerializeMethodInvocation(manager, statements, control, "PerformLayout", parameters: null, [], StatementOrdering.Append);
    }

    private void SerializeResumeLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
    {
        CodeExpressionCollection parameters = new();
        parameters.Add(new CodePrimitiveExpression(false));
        Type[] paramTypes = [typeof(bool)];
        SerializeMethodInvocation(manager, statements, control, "ResumeLayout", parameters, paramTypes, StatementOrdering.Append);
    }

    private void SerializeSuspendLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
    {
        SerializeMethodInvocation(manager, statements, control, "SuspendLayout", parameters: null, [], StatementOrdering.Prepend);
    }

    /// <summary>
    ///  Serializes a series of SetChildIndex() statements for each control iln a child control collection in
    ///  reverse order.
    /// </summary>
    private void SerializeZOrder(IDesignerSerializationManager manager, CodeStatementCollection statements, Control control)
    {
        // Push statements in reverse order so the first guy in the
        // collection is the last one to be brought to the front.
        for (int i = control.Controls.Count - 1; i >= 0; i--)
        {
            // Only serialize this control if it is (a) sited and
            // (b) not being privately inherited
            Control child = control.Controls[i];

            if (child.Site is null || child.Site.Container != control.Site!.Container)
            {
                continue;
            }

            InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(child)[typeof(InheritanceAttribute)]!;

            if (inheritanceAttribute.InheritanceLevel == InheritanceLevel.InheritedReadOnly)
            {
                continue;
            }

            // Create the "control.Controls.SetChildIndex" call
            CodeExpression controlsCollection = new CodePropertyReferenceExpression(SerializeToExpression(manager, control), "Controls");
            CodeMethodReferenceExpression method = new(controlsCollection, "SetChildIndex");
            CodeMethodInvokeExpression methodInvoke = new()
            {
                Method = method
            };

            // Fill in parameters
            CodeExpression? childControl = SerializeToExpression(manager, child);
            methodInvoke.Parameters.Add(childControl);
            methodInvoke.Parameters.Add(SerializeToExpression(manager, 0));
            CodeExpressionStatement statement = new(methodInvoke);
            statements.Add(statement);
        }
    }
}
