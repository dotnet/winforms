// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Control's provide their own serializer so they can write out resource hierarchy
    ///  information.  We delegate nearly everything to our base class's serializer.
    /// </summary>
    internal class ControlCodeDomSerializer : CodeDomSerializer
    {
        /// <summary>
        ///  Deserilizes the given CodeDom object into a real object.  This
        ///  will use the serialization manager to create objects and resolve
        ///  data types.  The root of the object graph is returned.
        /// </summary>
        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            if (manager == null || codeObject == null)
            {
                throw new ArgumentNullException(manager == null ? "manager" : "codeObject");
            }

            //Attempt to suspend all components within the icontainer
            IContainer container = (IContainer)manager.GetService(typeof(IContainer));
            ArrayList suspendedComponents = null;

            if (container != null)
            {
                suspendedComponents = new ArrayList(container.Components.Count);

                foreach (IComponent comp in container.Components)
                {
                    Control control = comp as Control;

                    if (control != null)
                    {
                        control.SuspendLayout();

                        //Add this control to our suspended components list so we can resume later
                        suspendedComponents.Add(control);
                    }
                }
            }

            object objectGraphData = null;

            try
            {
                // Find our base class's serializer.  
                CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));

                if (serializer == null)
                {
                    Debug.Fail("Unable to find a CodeDom serializer for 'Component'. Has someone tampered with the serialization providers?");

                    return null;
                }

                objectGraphData = serializer.Deserialize(manager, codeObject);
            }
            finally
            {
                //resume all suspended comps we found earlier
                if (suspendedComponents != null)
                {
                    foreach (Control c in suspendedComponents)
                    {
                        // Dev10 Bug #462211: Controls in design time may change their size due to incorrectly
                        // calculated anchor info.
                        // UNDONE: c.ResumeLayout(false) because it regressed layouts with Dock property
                        // see Dev11 bug 117530 DTS Winforms: Upgraded project -Control location and size are changed in the designer gen'd code
                        c.ResumeLayout(true /*performLayout*/);
                    }
                }
            }

            return objectGraphData;
        }

        private bool HasAutoSizedChildren(Control parent)
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

        private bool HasMixedInheritedChildren(Control parent)
        {
            bool inheritedChildren = false;
            bool nonInheritedChildren = false;

            foreach (Control c in parent.Controls)
            {
                InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(c)[typeof(InheritanceAttribute)];

                if (ia != null && ia.InheritanceLevel != InheritanceLevel.NotInherited)
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

            foreach (Control c in parent.Controls)
            {
                if (c.Site != null && c.Site.DesignMode)
                {
                    // We only emit Size/Location information for controls that are sited and not inherrited readonly.
                    InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(c)[typeof(InheritanceAttribute)];

                    if (ia != null && ia.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
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
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            if (manager == null || value == null)
            {
                throw new ArgumentNullException(manager == null ? "manager" : "value");
            }

            // Find our base class's serializer.  
            CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));

            if (serializer == null)
            {
                Debug.Fail("Unable to find a CodeDom serializer for 'Component'.  Has someone tampered with the serialization providers?");

                return null;
            }

            // Now ask it to serializer
            object retVal = serializer.Serialize(manager, value);
            InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(value)[typeof(InheritanceAttribute)];
            InheritanceLevel inheritanceLevel = InheritanceLevel.NotInherited;

            if (inheritanceAttribute != null)
            {
                inheritanceLevel = inheritanceAttribute.InheritanceLevel;
            }

            if (inheritanceLevel != InheritanceLevel.InheritedReadOnly)
            {
                // Next, see if we are in localization mode.  If we are, and if we can get
                // to a ResourceManager through the service provider, then emit the hierarchy information for
                // this object.  There is a small fragile assumption here:  The resource manager is demand
                // created, so if all of the properties of this control had default values it is possible
                // there will be no resource manager for us.  I'm letting that slip a bit, however, because
                // for Control classes, we always emit at least the location / size information for the
                // control.
                IDesignerHost host = (IDesignerHost)manager.GetService(typeof(IDesignerHost));

                if (host != null)
                {
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(host.RootComponent)["Localizable"];

                    if (prop != null && prop.PropertyType == typeof(bool) && ((bool)prop.GetValue(host.RootComponent)))
                    {
                        SerializeControlHierarchy(manager, host, value);
                    }
                }

                CodeStatementCollection csCollection = retVal as CodeStatementCollection;
                
                if (csCollection != null)
                {
                    Control control = (Control)value;

                    // Serialize a suspend / resume pair.  We always serialize this
                    // for the root component
                    if ((host != null && control == host.RootComponent) || HasSitedNonReadonlyChildren(control))
                    {
                        SerializeSuspendLayout(manager, csCollection, value);
                        SerializeResumeLayout(manager, csCollection, value);
                        ControlDesigner controlDesigner = host.GetDesigner(control) as ControlDesigner;
                        
                        if (HasAutoSizedChildren(control) || (controlDesigner != null && controlDesigner.SerializePerformLayout))
                        {
                            SerializePerformLayout(manager, csCollection, value);
                        }
                    }

                    // And now serialize the correct z-order relationships for the controls.  We only need to 
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
        private void SerializeControlHierarchy(IDesignerSerializationManager manager, IDesignerHost host, object value)
        {
            Control control = value as Control;

            if (control != null)
            {
                // Object name
                string name;
                IMultitargetHelperService mthelperSvc = host.GetService(typeof(IMultitargetHelperService)) as IMultitargetHelperService;

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
                        string componentName = manager.GetName(component);
                        string componentTypeName = mthelperSvc == null ? component.GetType().AssemblyQualifiedName : mthelperSvc.GetAssemblyQualifiedName(component.GetType());

                        if (componentName != null)
                        {
                            SerializeResourceInvariant(manager, ">>" + componentName + ".Name", componentName);
                            SerializeResourceInvariant(manager, ">>" + componentName + ".Type", componentTypeName);
                        }
                    }
                }
                else
                {
                    name = manager.GetName(value);

                    // if we get null back, this must be an unsited control
                    if (name == null)
                    {
                        Debug.Assert(!(value is IComponent) || ((IComponent)value).Site == null, "Unnamed, sited control in hierarchy");
                        return;
                    }
                }

                SerializeResourceInvariant(manager, ">>" + name + ".Name", manager.GetName(value));

                // Object type
                SerializeResourceInvariant(manager, ">>" + name + ".Type", mthelperSvc == null ? control.GetType().AssemblyQualifiedName : mthelperSvc.GetAssemblyQualifiedName(control.GetType()));

                // Parent
                Control parent = control.Parent;
                
                if (parent != null && parent.Site != null)
                {
                    string parentName;

                    if (parent == host.RootComponent)
                    {
                        parentName = "$this";
                    }
                    else
                    {
                        parentName = manager.GetName(parent);
                    }

                    if (parentName != null)
                    {
                        SerializeResourceInvariant(manager, ">>" + name + ".Parent", parentName);
                    }

                    // Z-Order
                    for (int i = 0; i < parent.Controls.Count; i++)
                    {
                        if (parent.Controls[i] == control)
                        {
                            SerializeResourceInvariant(manager, ">>" + name + ".ZOrder", i.ToString(CultureInfo.InvariantCulture));
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
        ///  Serializes a method invocation on the control being serialized.  Used to serialize Suspend/ResumeLayout pairs, etc.
        /// </summary>
        private void SerializeMethodInvocation(IDesignerSerializationManager manager, CodeStatementCollection statements, object control, string methodName, CodeExpressionCollection parameters, Type[] paramTypes, StatementOrdering ordering)
        {
            using (TraceScope("ControlCodeDomSerializer::SerializeMethodInvocation(" + methodName + ")"))
            {
                string name = manager.GetName(control);
                Trace(name + "." + methodName);

                // Use IReflect to see if this method name exists on the control.  
                paramTypes = ToTargetTypes(control, paramTypes);
                MethodInfo mi = TypeDescriptor.GetReflectionType(control).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, paramTypes, null);
                
                if (mi != null)
                {
                    CodeExpression field = SerializeToExpression(manager, control);
                    CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(field, methodName);
                    CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression();
                    methodInvoke.Method = method;
                    
                    if (parameters != null)
                    {
                        methodInvoke.Parameters.AddRange(parameters);
                    }

                    CodeExpressionStatement statement = new CodeExpressionStatement(methodInvoke);
                    
                    switch (ordering)
                    {
                        case StatementOrdering.Prepend:
                            statement.UserData["statement-ordering"] = "begin";
                            break;
                        case StatementOrdering.Append:
                            statement.UserData["statement-ordering"] = "end";
                            break;
                        default:
                            Debug.Fail("Unsupported statement ordering: " + ordering);
                            break;
                    }

                    statements.Add(statement);
                }
            }
        }

        private void SerializePerformLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
        {
            SerializeMethodInvocation(manager, statements, control, "PerformLayout", null, new Type[0], StatementOrdering.Append);
        }

        private void SerializeResumeLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
        {
            CodeExpressionCollection parameters = new CodeExpressionCollection();
            parameters.Add(new CodePrimitiveExpression(false));
            Type[] paramTypes = { typeof(bool) };
            SerializeMethodInvocation(manager, statements, control, "ResumeLayout", parameters, paramTypes, StatementOrdering.Append);
        }

        private void SerializeSuspendLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
        {
            SerializeMethodInvocation(manager, statements, control, "SuspendLayout", null, new Type[0], StatementOrdering.Prepend);
        }

        /// <summary>
        ///  Serializes a series of SetChildIndex() statements for each control iln a child control collection in
        ///  reverse order.
        /// </summary>
        private void SerializeZOrder(IDesignerSerializationManager manager, CodeStatementCollection statements, Control control)
        {
            using (TraceScope("ControlCodeDomSerializer::SerializeZOrder()"))
            {
                // Push statements in reverse order so the first guy in the
                // collection is the last one to be brought to the front.
                for (int i = control.Controls.Count - 1; i >= 0; i--)
                {
                    // Only serialize this control if it is (a) sited and
                    // (b) not being privately inherited
                    Control child = control.Controls[i];

                    if (child.Site == null || child.Site.Container != control.Site.Container)
                    {
                        continue;
                    }

                    InheritanceAttribute attr = (InheritanceAttribute)TypeDescriptor.GetAttributes(child)[typeof(InheritanceAttribute)];
                    
                    if (attr.InheritanceLevel == InheritanceLevel.InheritedReadOnly)
                    {
                        continue;
                    }

                    // Create the "control.Controls.SetChildIndex" call
                    CodeExpression controlsCollection = new CodePropertyReferenceExpression(SerializeToExpression(manager, control), "Controls");
                    CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(controlsCollection, "SetChildIndex");
                    CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression();
                    methodInvoke.Method = method;

                    // Fill in parameters
                    CodeExpression childControl = SerializeToExpression(manager, child);
                    methodInvoke.Parameters.Add(childControl);
                    methodInvoke.Parameters.Add(SerializeToExpression(manager, 0));
                    CodeExpressionStatement statement = new CodeExpressionStatement(methodInvoke);
                    statements.Add(statement);
                }
            }
        }
    }
}
