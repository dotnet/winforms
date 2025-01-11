// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Collections;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    private sealed partial class CodeDomSerializationStore
    {
        /// <summary>
        ///  This is a simple code dom serializer that serializes a set of objects as a unit.
        /// </summary>
        private class ComponentListCodeDomSerializer : CodeDomSerializer
        {
            internal static readonly ComponentListCodeDomSerializer s_instance = new();
            private readonly Dictionary<string, OrderedCodeStatementCollection?> _statementsTable = [];
            private readonly Dictionary<string, List<CodeExpression>> _expressions = [];
            private Dictionary<string, CodeDomComponentSerializationState>? _objectState; // only used during deserialization
            private bool _applyDefaults = true;
            private readonly HashSet<string> _nameResolveGuard = [];

            public override object Deserialize(IDesignerSerializationManager manager, object state)
            {
                throw new NotSupportedException();
            }

            private static void PopulateCompleteStatements(object? data, string name, CodeStatementCollection completeStatements, Dictionary<string, List<CodeExpression>> expressions)
            {
                if (data is null)
                {
                    return;
                }

                if (data is CodeStatementCollection statements)
                {
                    completeStatements.AddRange(statements);
                }
                else if (data is CodeStatement statement)
                {
                    completeStatements.Add(statement);
                }
                else if (data is CodeExpression expression)
                {
                    // we handle expressions a little differently since they don't have a LHS or RHS
                    // they won't show up correctly in the statement table. We will deserialize them explicitly.
                    if (!expressions.TryGetValue(name, out List<CodeExpression>? exps))
                    {
                        exps = [];
                        expressions[name] = exps;
                    }

                    exps.Add(expression);
                }
                else
                {
                    Debug.Fail($"No case for {data.GetType().Name}");
                }
            }

            /// <summary>
            ///  Deserializes the given object state. The results are contained within the serialization manager's
            ///  name table. The objectNames list is used to deserialize in the proper order,
            ///  as objectState is unordered.
            /// </summary>
            internal void Deserialize(IDesignerSerializationManager manager, Dictionary<string, CodeDomComponentSerializationState> objectState, List<string> objectNames, bool applyDefaults)
            {
                CodeStatementCollection completeStatements = [];
                _expressions.Clear();
                _applyDefaults = applyDefaults;
                foreach (string name in objectNames)
                {
                    if (objectState.TryGetValue(name, out CodeDomComponentSerializationState? state))
                    {
                        PopulateCompleteStatements(state.Code, name, completeStatements, _expressions);
                        PopulateCompleteStatements(state.Context, name, completeStatements, _expressions);
                    }
                }

                CodeStatementCollection mappedStatements = [];
                CodeMethodMap methodMap = new(mappedStatements);

                methodMap.Add(completeStatements);
                methodMap.Combine();
                _statementsTable.Clear();

                // generate statement table keyed on component name
                FillStatementTable(manager, _statementsTable, mappedStatements);

                // We need to also ensure that for every entry in the statement table we have a
                // corresponding entry in objectNames. Otherwise, we won't deserialize completely.
                HashSet<string> completeNames = [..objectNames];
                completeNames.UnionWith(_statementsTable.Keys);

                _objectState = new(objectState);

                ResolveNameEventHandler resolveNameHandler = new(OnResolveName);
                manager.ResolveName += resolveNameHandler;
                try
                {
                    foreach (string name in completeNames)
                    {
                        ResolveName(manager, name, true);
                    }
                }
                finally
                {
                    _objectState = null;
                    manager.ResolveName -= resolveNameHandler;
                }
            }

            private void OnResolveName(object? sender, ResolveNameEventArgs e)
            {
                string name = e.Name!;
                // note: this recursion guard does not fix the problem,
                // but rather avoids a stack overflow which will bring down VS and cause loss of data.
                if (!_nameResolveGuard.Add(name))
                {
                    return;
                }

                try
                {
                    IDesignerSerializationManager manager = (IDesignerSerializationManager)sender!;
                    if (ResolveName(manager, name, false))
                    {
                        e.Value = manager.GetInstance(name);
                    }
                }
                finally
                {
                    _nameResolveGuard.Remove(name);
                }
            }

            private void DeserializeDefaultProperties(IDesignerSerializationManager manager, string name, List<string>? defProps)
            {
                // Next, default properties, but only if we successfully resolved.
                if (defProps is null || !_applyDefaults)
                {
                    return;
                }

                object? comp = manager.GetInstance(name);
                if (comp is null)
                {
                    return;
                }

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
                foreach (string propName in defProps)
                {
                    PropertyDescriptor? prop = props[propName];
                    if (prop is not null && prop.CanResetValue(comp))
                    {
                        // If there is a member relationship setup for this property, we should disconnect it first.
                        // This makes sense, since if there was a previous relationship,
                        // we would have serialized it and not come here at all.
                        if (manager.TryGetService(out MemberRelationshipService? relationships) && relationships[comp, prop] != MemberRelationship.Empty)
                        {
                            relationships[comp, prop] = MemberRelationship.Empty;
                        }

                        prop.ResetValue(comp);
                    }
                }
            }

            private static void DeserializeDesignTimeProperties(IDesignerSerializationManager manager, string name, Dictionary<string, object?>? state)
            {
                if (state is null)
                {
                    return;
                }

                object? comp = manager.GetInstance(name);
                if (comp is null)
                {
                    return;
                }

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);

                foreach (KeyValuePair<string, object?> stateEntry in state)
                {
                    PropertyDescriptor? prop = props[stateEntry.Key];
                    prop?.SetValue(comp, stateEntry.Value);
                }
            }

            /// <summary>
            ///  This is used to resolve nested component references.
            ///  NestedComponents don't exist as sited components within the DesignerHost,
            ///  they are actually sited within a parent component.
            ///  This method takes the FullName defined on INestedSite and returns the component which matches it.
            ///  outerComponent is the name of the topmost component which does exist in the DesignerHost
            ///  This code also exists in VSCodeDomDesignerLoader -- please keep them in sync.
            /// </summary>
            private static IComponent? ResolveNestedName(IDesignerSerializationManager? manager, string name, [NotNullIfNotNull(nameof(manager))] out string? outerComponent)
            {
                if (manager is null)
                {
                    outerComponent = null;
                    return null;
                }

                bool moreChunks;
                // We need to resolve the first chunk using the manager. other chunks will be resolved within the nested containers.
                int curIndex = name.IndexOf('.');
                Debug.Assert(curIndex > 0, "ResolvedNestedName accepts only nested names!");
                outerComponent = name[..curIndex];
                IComponent? curComp = manager.GetInstance(outerComponent) as IComponent;

                do
                {
                    int prevIndex = curIndex;
                    curIndex = name.IndexOf('.', curIndex + 1);

                    moreChunks = curIndex != -1;
                    string compName = moreChunks
                        ? name.Substring(prevIndex + 1, curIndex)
                        : name[(prevIndex + 1)..];

                    if (string.IsNullOrEmpty(compName))
                    {
                        return null;
                    }

                    ISite? site = curComp?.Site;
                    if (!site.TryGetService(out INestedContainer? container))
                    {
                        return null;
                    }

                    curComp = container.Components[compName];
                }
                while (moreChunks);

                return curComp;
            }

            private bool ResolveName(IDesignerSerializationManager manager, string name, bool canInvokeManager)
            {
                bool resolved = false;
                // Check for a nested name. Components that are sited within NestedContainers need to be looked up
                // in their nested container, and won't be resolvable directly via the manager.
                if (name.IndexOf('.') > 0)
                {
                    IComponent? nestedComp = ResolveNestedName(manager, name, out string? parentName);
                    if (nestedComp is not null && parentName is not null)
                    {
                        manager.SetName(nestedComp, name);
                        // What is the point of this?
                        // Well, the nested components won't be in the statement table with its nested name.
                        // However, their most parent component will be, so forcing a resolve of their name
                        // will actually deserialize the nested statements.
                        ResolveName(manager, parentName, canInvokeManager);
                    }
                    else
                    {
                        Debug.Fail($"Unable to resolve nested component: {name}");
                    }
                }

                // First we check to see if the statements table contains an OrderedCodeStatementCollection for this name.
                // If it does this means we have not resolved this name yet, so we grab its
                // OrderedCodeStatementCollection and deserialize that, along with any default properties
                // and design-time properties.
                // If it doesn't contain an OrderedCodeStatementsCollection this means one of two things:
                // 1. We already resolved this name and shoved an instance in there.
                //    In this case we just return the instance
                // 2. There are no statements corresponding to this name,
                //    but there might be expressions that have never been deserialized,
                //    so we check for that and deserialize those.
                _statementsTable.TryGetValue(name, out OrderedCodeStatementCollection? statements);
                if (statements is not null)
                {
                    _statementsTable[name] = null; // prevent recursion
                    // we look through the statements to find the variableRef or fieldRef that matches this name
                    string? typeName = null;
                    foreach (CodeStatement statement in statements)
                    {
                        if (statement is CodeVariableDeclarationStatement codeVariableDeclaration)
                        {
                            typeName = codeVariableDeclaration.Type.BaseType;
                            break;
                        }
                    }

                    // next, invoke the serializer for this component
                    if (typeName is not null)
                    {
                        Type? type = manager.GetType(typeName);
                        if (type is null)
                        {
                            manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerTypeNotFound, typeName), manager));
                        }
                        else if (statements.Count > 0)
                        {
                            CodeDomSerializer? serializer = GetSerializer(manager, type);
                            if (serializer is null)
                            {
                                // We report this as an error. This indicates that there are code statements
                                // in initialize component that we do not know how to load.
                                manager.ReportError(new CodeDomSerializerException(
                                    string.Format(SR.SerializerNoSerializerForComponent, type.FullName), manager));
                            }
                            else
                            {
                                try
                                {
                                    object? instance = serializer.Deserialize(manager, statements);
                                    resolved = instance is not null;
                                    if (resolved)
                                    {
                                        _statementsTable[name] = (OrderedCodeStatementCollection?)instance;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    manager.ReportError(ex);
                                }
                            }
                        }
                    }

                    // if we can't find a typeName to get a serializer with we fallback to deserializing
                    // each statement individually using the default serializer.
                    else
                    {
                        foreach (CodeStatement cs in statements)
                        {
                            DeserializeStatement(manager, cs);
                        }

                        resolved = true;
                    }

                    if (_objectState!.Remove(name, out CodeDomComponentSerializationState? state))
                    {
                        DeserializeState(manager, name, state);
                    }

                    if (_expressions.Remove(name, out List<CodeExpression>? exps))
                    {
                        foreach (CodeExpression exp in exps)
                        {
                            DeserializeExpression(manager, name, exp);
                        }

                        resolved = true;
                    }
                }
                else
                {
                    resolved = ((IDictionary)_statementsTable)[name] is not null;
                    if (!resolved)
                    {
                        // this is condition 2 of the comment at the start of this method.
                        if (_expressions.TryGetValue(name, out List<CodeExpression>? exps))
                        {
                            foreach (CodeExpression exp in exps)
                            {
                                object? exValue = DeserializeExpression(manager, name, exp);
                                if (exValue is not null && !resolved && canInvokeManager && manager.GetInstance(name) is null)
                                {
                                    manager.SetName(exValue, name);
                                    resolved = true;
                                }
                            }
                        }

                        // Sometimes components won't be in either the statements table or the expressions table,
                        // for example, this occurs for resources during undo/redo.
                        // In these cases the component should be resolvable by the manager.
                        // Never do this when we have been asked by the serialization manager to resolve the name;
                        // otherwise we may infinitely recurse.
                        if (!resolved && canInvokeManager)
                        {
                            resolved = manager.GetInstance(name) is not null;
                        }

                        // In this case we still need to correctly deserialize default properties &  design-time only properties.
                        if (resolved && _objectState!.TryGetValue(name, out CodeDomComponentSerializationState? state))
                        {
                            DeserializeState(manager, name, state);
                        }
                    }

                    if (!resolved && canInvokeManager)
                    {
                        manager.ReportError(new CodeDomSerializerException(string.Format(SR.CodeDomComponentSerializationServiceDeserializationError, name), manager));
                        Debug.Fail($"No statements or instance for name and no lone expressions: {name}");
                    }
                }

                return resolved;
            }

            private void DeserializeState(IDesignerSerializationManager manager, string name, CodeDomComponentSerializationState state)
            {
                DeserializeDefaultProperties(manager, name, state.Properties);
                DeserializeDesignTimeProperties(manager, name, state.Resources);
                DeserializeEventResets(manager, name, state.Events);
                DeserializeModifier(manager, name, state.Modifier);
            }

            private static void DeserializeEventResets(IDesignerSerializationManager? manager, string name, List<string>? eventNames)
            {
                if (eventNames is not null && manager is not null && !string.IsNullOrEmpty(name))
                {
                    object? comp = manager.GetInstance(name);
                    if (comp is not null && manager.GetService(typeof(IEventBindingService)) is IEventBindingService ebs)
                    {
                        PropertyDescriptorCollection eventProps = ebs.GetEventProperties(TypeDescriptor.GetEvents(comp));
                        if (eventProps is not null)
                        {
                            foreach (string eventName in eventNames)
                            {
                                PropertyDescriptor? prop = eventProps[eventName];

                                prop?.SetValue(comp, null);
                            }
                        }
                    }
                }
            }

            private static void DeserializeModifier(IDesignerSerializationManager manager, string name, object? state)
            {
                if (state is null)
                {
                    return;
                }

                Debug.Assert(state is MemberAttributes, "Attempting to deserialize a null modifier");
                object? comp = manager.GetInstance(name);
                if (comp is not null)
                {
                    MemberAttributes modifierValue = (MemberAttributes)state;
                    PropertyDescriptor? modifierProp = TypeDescriptor.GetProperties(comp)["Modifiers"];
                    modifierProp?.SetValue(comp, modifierValue);
                }
            }

            public override object Serialize(IDesignerSerializationManager manager, object state)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///  For everything in the serialization manager's container, we need a variable ref,
            ///  just in case something that has changed has a reference to another object. We also
            ///  must do this for everything that we are serializing that is not marked as EntireObject.
            ///  Otherwise reference could leak and cause the entire object to be serialized.
            /// </summary>
            internal void SetupVariableReferences(IDesignerSerializationManager manager, IContainer container, Dictionary<object, ObjectData> objectData, IList shimObjectNames)
            {
                foreach (IComponent c in container.Components)
                {
                    string? name = TypeDescriptor.GetComponentName(c);
                    if (!string.IsNullOrEmpty(name))
                    {
                        bool needVar = !(objectData.TryGetValue(c, out ObjectData? data) && data.EntireObject);

                        if (needVar)
                        {
                            CodeVariableReferenceExpression var = new(name);
                            SetExpression(manager, c, var);
                            if (!shimObjectNames.Contains(name))
                            {
                                shimObjectNames.Add(name);
                            }

                            if (c.Site.TryGetService(out INestedContainer? nested) && nested.Components.Count > 0)
                            {
                                SetupVariableReferences(manager, nested, objectData, shimObjectNames);
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///  Serializes the given set of objects (contained in objectData) into the given object state dictionary.
            /// </summary>
            internal void Serialize(IDesignerSerializationManager manager, Dictionary<object, ObjectData> objectData, Dictionary<string, CodeDomComponentSerializationState> objectState, IList shimObjectNames)
            {
                if (manager.GetService<IContainer>() is { } container)
                {
                    SetupVariableReferences(manager, container, objectData, shimObjectNames);
                }

                // Next, save a statement collection for each object.
                StatementContext statementCtx = new();
                statementCtx.StatementCollection.Populate(objectData.Keys);
                manager.Context.Push(statementCtx);
                try
                {
                    foreach (ObjectData data in objectData.Values)
                    {
                        object? code = null;
                        CodeStatementCollection? ctxStatements = null;
                        Dictionary<string, object?>? resources = null;
                        CodeStatementCollection extraStatements = [];
                        manager.Context.Push(extraStatements);
                        if (manager.TryGetSerializer(data._value.GetType(), out CodeDomSerializer? serializer))
                        {
                            if (data.EntireObject)
                            {
                                if (!IsSerialized(manager, data._value))
                                {
                                    code = data.Absolute
                                        ? serializer.SerializeAbsolute(manager, data._value)
                                        : serializer.Serialize(manager, data._value);

                                    ctxStatements = statementCtx.StatementCollection[data._value];
                                    if (ctxStatements?.Count == 0)
                                    {
                                        ctxStatements = null;
                                    }

                                    if (extraStatements.Count > 0 && code is CodeStatementCollection existingStatements)
                                    {
                                        existingStatements.AddRange(extraStatements);
                                    }
                                }
                                else
                                {
                                    code = statementCtx.StatementCollection[data._value];
                                }
                            }
                            else
                            {
                                CodeStatementCollection codeStatements = [];
                                foreach (MemberData md in data.Members)
                                {
                                    if (md._member.Attributes.Contains(DesignOnlyAttribute.Yes))
                                    {
                                        // For design time properties, we write their value into a resource blob.
#pragma warning disable SYSLIB0050 // Type or member is obsolete
                                        if (md._member is PropertyDescriptor prop && prop.PropertyType.IsSerializable)
                                        {
                                            resources ??= [];

                                            resources[prop.Name] = prop.GetValue(data._value);
                                        }
#pragma warning restore SYSLIB0050
                                    }
                                    else
                                    {
                                        codeStatements.AddRange(md._absolute
                                            ? serializer.SerializeMemberAbsolute(manager, data._value, md._member)
                                            : serializer.SerializeMember(manager, data._value, md._member));
                                    }
                                }

                                code = codeStatements;
                            }
                        }

                        if (extraStatements.Count > 0)
                        {
                            if (code is CodeStatementCollection existingStatements)
                            {
                                existingStatements.AddRange(extraStatements);
                            }
                        }

                        manager.Context.Pop();
                        // And now search for default properties and events
                        List<string>? defaultPropList = null;
                        List<string>? defaultEventList = null;
                        IEventBindingService? ebs = manager.GetService<IEventBindingService>();
                        if (data.EntireObject)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(data._value);
                            foreach (PropertyDescriptor prop in props)
                            {
                                if (!prop.ShouldSerializeValue(data._value)
                                    && !prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden))
                                {
                                    if (!prop.IsReadOnly || prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content))
                                    {
                                        defaultPropList ??= new(data.Members.Count);
                                        defaultPropList.Add(prop.Name);
                                    }
                                }
                            }

                            if (ebs is not null)
                            {
                                PropertyDescriptorCollection events = ebs.GetEventProperties(TypeDescriptor.GetEvents(data._value));
                                foreach (PropertyDescriptor eventProp in events)
                                {
                                    if (eventProp is null || eventProp.IsReadOnly)
                                    {
                                        continue;
                                    }

                                    if (eventProp.GetValue(data._value) is null)
                                    {
                                        defaultEventList ??= [];

                                        defaultEventList.Add(eventProp.Name);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (MemberData md in data.Members)
                            {
                                if (md._member is PropertyDescriptor prop && !prop.ShouldSerializeValue(data._value))
                                {
                                    if (ebs?.GetEvent(prop) is not null)
                                    {
                                        Debug.Assert(prop.GetValue(data._value) is null, "ShouldSerializeValue and GetValue are differing");
                                        defaultEventList ??= [];

                                        defaultEventList.Add(prop.Name);
                                    }
                                    else
                                    {
                                        defaultPropList ??= new(data.Members.Count);
                                        defaultPropList.Add(prop.Name);
                                    }
                                }
                            }
                        }

                        // Check for non-default modifiers property
                        object? modifier = TypeDescriptor.GetProperties(data._value)["Modifiers"]?.GetValue(data._value);

                        if (code is not null || defaultPropList is not null)
                        {
                            objectState[data._name] = new CodeDomComponentSerializationState(code, ctxStatements, defaultPropList, resources, defaultEventList, modifier);
                        }
                    }
                }
                finally
                {
                    manager.Context.Pop();
                }
            }
        }
    }
}
