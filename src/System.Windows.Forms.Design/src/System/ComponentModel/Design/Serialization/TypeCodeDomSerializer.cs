// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This class performs the same tasks as a CodeDomSerializer only serializing an object through this class defines a new type.
/// </summary>
[DefaultSerializationProvider(typeof(CodeDomSerializationProvider))]
public partial class TypeCodeDomSerializer : CodeDomSerializerBase
{
    // Used only during deserialization to provide name to object mapping.
    private HybridDictionary? _nameTable;
    private Dictionary<string, OrderedCodeStatementCollection>? _statementTable;
    private static readonly Attribute[] s_designTimeFilter = [DesignOnlyAttribute.Yes];
    private static readonly object s_initMethodKey = new();
    private static TypeCodeDomSerializer? s_default;

    internal static TypeCodeDomSerializer Default => s_default ??= new TypeCodeDomSerializer();

    /// <summary>
    ///  This method deserializes a previously serialized code type declaration. The default implementation performs
    ///  the following tasks:
    ///      • Case Sensitivity Checks: It looks for a CodeDomProvider service to decide if it should treat members
    ///          as case sensitive or case insensitive.
    ///      • Statement Sorting:  All member variables and local variables from init methods are stored in a table.
    ///          Then each statement in an init method is added to a statement collection grouped according
    ///          to its left hand side. So all statements assigning or operating on a particular variable are grouped
    ///          under that variable. Variables that have no statements are discarded.
    ///      • Deserialization: Finally, the statement collections for each variable are deserialized
    ///          according to the variable. Deserialize returns an instance of the root object.
    /// </summary>
    public virtual object Deserialize(IDesignerSerializationManager manager, CodeTypeDeclaration declaration)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(declaration);

        object rootObject;

        // Determine case-sensitivity
        bool caseInsensitive = false;
        CodeDomProvider? provider = manager.GetService<CodeDomProvider>();

        if (provider is not null)
        {
            caseInsensitive = ((provider.LanguageOptions & LanguageOptions.CaseInsensitive) != 0);
        }

        // Get and initialize the document type.
        Type? baseType = null;
        string baseTypeName = declaration.Name;

        foreach (CodeTypeReference typeRef in declaration.BaseTypes)
        {
            Type? t = manager.GetType(GetTypeNameFromCodeTypeReference(manager, typeRef));
            baseTypeName = typeRef.BaseType;
            if (t is not null && !(t.IsInterface))
            {
                baseType = t;
                break;
            }
        }

        if (baseType is null)
        {
            Error(manager, string.Format(SR.SerializerTypeNotFound, baseTypeName), SR.SerializerTypeNotFound);
        }

        if (GetReflectionTypeFromTypeHelper(manager, baseType).IsAbstract)
        {
            Error(manager, string.Format(SR.SerializerTypeAbstract, baseType.FullName), SR.SerializerTypeAbstract);
        }

        ResolveNameEventHandler onResolveName = new(OnResolveName);
        manager.ResolveName += onResolveName;
        rootObject = manager.CreateInstance(baseType, null, declaration.Name, true);

        // Now that we have the root object, we create a nametable and fill it with member declarations.
        int count = declaration.Members.Count;
        _nameTable = new HybridDictionary(count, caseInsensitive);
        _statementTable = new Dictionary<string, OrderedCodeStatementCollection>(count);
        Dictionary<string, string> names = new(count);
        RootContext rootCtx = new(new CodeThisReferenceExpression(), rootObject);
        manager.Context.Push(rootCtx);
        try
        {
            StringComparison compare = caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (CodeTypeMember typeMember in declaration.Members)
            {
                if (typeMember is CodeMemberField member)
                {
                    if (!string.Equals(member.Name, declaration.Name, compare))
                    {
                        // always skip members with the same name as the type -- because that's the name we use when we resolve "base" and "this" items...
                        _nameTable[member.Name] = member;

                        if (member.Type is not null && !string.IsNullOrEmpty(member.Type.BaseType))
                        {
                            names[member.Name] = GetTypeNameFromCodeTypeReference(manager, member.Type);
                        }
                    }
                }
            }

            CodeMemberMethod[] methods = GetInitializeMethods(manager, declaration)
                ?? throw new InvalidOperationException();

            // Walk through all of our methods and search for local variables. These guys get added to our nametable too.
            foreach (CodeMemberMethod method in methods)
            {
                foreach (CodeStatement statement in method.Statements)
                {
                    if (statement is CodeVariableDeclarationStatement local)
                    {
                        _nameTable[local.Name] = statement;
                    }
                }
            }

            // The name table should come pre-populated with our root expression.
            _nameTable[declaration.Name] = rootCtx.Expression;

            // We fill a "statement table" for everything in our init methods. This statement table is a dictionary whose
            // keys contain object names and whose values contain a statement collection of all statements with a LHS
            // resolving to an object by that name. If supportGenerate is true, FillStatementTable will skip methods
            // that are marked with the tag "GeneratedStatement".
            foreach (CodeMemberMethod method in methods)
            {
                FillStatementTable(manager, _statementTable, names, method.Statements, declaration.Name);
            }

            // Interesting problem. The CodeDom parser may auto generate statements that are associated with other methods.
            // VB does this, for example, to create statements automatically for Handles clauses. The problem with this
            // technique is that we will end up with statements that are related to variables that live solely in user code
            // and not in InitializeComponent. We will attempt to construct instances of these objects with limited success.
            // To guard against this, we check to see if the manager even supports this feature, and if it does, we must look
            // out for these statements while filling the statement collections.
            PropertyDescriptor? supportGenerate = manager.Properties["SupportsStatementGeneration"];
            if (supportGenerate is not null && supportGenerate.TryGetValue(manager, out bool supportGenerateValue) && supportGenerateValue)
            {
                // Ok, we must do the more expensive work of validating the statements we get.
                foreach (string name in _nameTable.Keys)
                {
                    if (!name.Equals(declaration.Name) && _statementTable.TryGetValue(name, out OrderedCodeStatementCollection? statements))
                    {
                        bool acceptStatement = false;
                        foreach (CodeStatement statement in statements)
                        {
                            object? genFlag = statement.UserData["GeneratedStatement"];
                            if (genFlag is not true)
                            {
                                acceptStatement = true;
                                break;
                            }
                        }

                        if (!acceptStatement)
                        {
                            _statementTable.Remove(name);
                        }
                    }
                }
            }

            // Design time properties must be resolved before runtime properties to make sure that properties like "language"
            // get established before we need to read values out the resource bundle.

            // Deserialize design time properties for the root component.
            DeserializePropertiesFromResources(manager, rootObject, s_designTimeFilter);
            // sort by the order so we deserialize in the same order the objects were declared in.
            OrderedCodeStatementCollection[] statementArray = new OrderedCodeStatementCollection[_statementTable.Count];
            _statementTable.Values.CopyTo(statementArray, 0);
            Array.Sort(statementArray, StatementOrderComparer.s_default);
            // make sure we have fully deserialized everything that is referenced in the statement table. Skip the root object for last
            OrderedCodeStatementCollection? rootStatements = null;
            foreach (OrderedCodeStatementCollection statements in statementArray)
            {
                if (statements.Name.Equals(declaration.Name))
                {
                    rootStatements = statements;
                }
                else
                {
                    DeserializeName(manager, statements.Name, statements);
                }
            }

            if (rootStatements is not null)
            {
                DeserializeName(manager, rootStatements.Name, rootStatements);
            }
        }
        finally
        {
            _nameTable = null;
            _statementTable = null;
            Debug.Assert(manager.Context.Current == rootCtx, "Context stack corrupted");
            manager.ResolveName -= onResolveName;
            manager.Context.Pop();
        }

        return rootObject;
    }

    /// <summary>
    ///  This takes the given name and deserializes it from our name table. Before blindly deserializing it checks
    ///  the contents of the name table to see if the object already exists within it. We do this because
    ///  deserializing one object may call back into us through OnResolveName and deserialize another.
    /// </summary>
    private object? DeserializeName(IDesignerSerializationManager manager, string name, CodeStatementCollection? statements)
    {
        object? value;

        // If the name we're looking for isn't in our dictionary, we return null.
        // It is up to the caller to decide if this is an error or not.
        value = _nameTable![name];
        CodeObject? codeObject = value as CodeObject;
        string? typeName = null;
        CodeMemberField? field = null;

        if (codeObject is not null)
        {
            // If we fail, don't return a CodeDom element to the caller! Also clear out our nametable entry here.
            // A badly written serializer may cause a recursion here, and we want to stop it.
            value = null;
            _nameTable[name] = null;

            // What kind of code object is this?
            if (codeObject is CodeVariableDeclarationStatement declaration)
            {
                typeName = GetTypeNameFromCodeTypeReference(manager, declaration.Type);
            }
            else
            {
                field = codeObject as CodeMemberField;
                if (field is not null)
                {
                    typeName = GetTypeNameFromCodeTypeReference(manager, field.Type);
                }
                else if (manager.TryGetContext(out RootContext? rootCtx) && codeObject is CodeExpression exp && rootCtx.Expression == exp)
                {
                    value = rootCtx.Value;
                    typeName = TypeDescriptor.GetClassName(value);
                }
                else
                {
                    Debug.Fail("Unrecognized code object in nametable.");
                }
            }
        }
        else if (value is null)
        {
            // See if the container has this object. This may be necessary for visual inheritance.
            IContainer? container = manager.GetService<IContainer>();
            if (container is not null)
            {
                IComponent? comp = container.Components[name];
                if (comp is not null)
                {
                    typeName = comp.GetType().FullName;

                    // We had to go to the host here, so there isn't a nametable entry here -- push in the component
                    // here so we don't accidentally recurse when we try to deserialize this object.
                    _nameTable[name] = comp;
                }
            }
        }

        if (typeName is not null)
        {
            // Default case -- something that needs to be deserialized
            Type? type = manager.GetType(typeName);
            if (type is null)
            {
                manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerTypeNotFound, typeName), manager));
            }
            else
            {
                if (statements is null && _statementTable!.TryGetValue(name, out OrderedCodeStatementCollection? statementOut))
                {
                    statements = statementOut;
                }

                if (statements is not null && statements.Count > 0)
                {
                    CodeDomSerializer? serializer = GetSerializer(manager, type);
                    if (serializer is null)
                    {
                        // We report this as an error. This indicates that there are code statements in
                        // initialize component that we do not know how to load.
                        manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerNoSerializerForComponent, type.FullName), manager));
                    }
                    else
                    {
                        try
                        {
                            value = serializer.Deserialize(manager, statements);
                            // Search for a modifiers property, and set it.
                            if (value is not null && field is not null)
                            {
                                PropertyDescriptor? prop = TypeDescriptor.GetProperties(value)["Modifiers"];

                                if (prop is not null && prop.PropertyType == typeof(MemberAttributes))
                                {
                                    MemberAttributes modifiers = field.Attributes & MemberAttributes.AccessMask;

                                    prop.SetValue(value, modifiers);
                                }
                            }

                            _nameTable[name] = value;
                        }
                        catch (Exception ex)
                        {
                            manager.ReportError(ex);
                        }
                    }
                }
            }
        }

        return value;
    }

    /// <summary>
    ///  This method returns the method to emit all of the initialization code to for the given member.
    ///  The default implementation returns an empty constructor.
    /// </summary>
    protected virtual CodeMemberMethod GetInitializeMethod(IDesignerSerializationManager manager, CodeTypeDeclaration declaration, object value)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(declaration);
        ArgumentNullException.ThrowIfNull(value);

        if (declaration.UserData[s_initMethodKey] is not CodeConstructor ctor)
        {
            ctor = new CodeConstructor();
            declaration.UserData[s_initMethodKey] = ctor;
        }

        return ctor;
    }

    /// <summary>
    ///  This method returns an array of methods that need to be interpreted during deserialization.
    ///  The default implementation returns a single element array with the constructor in it.
    /// </summary>
    protected virtual CodeMemberMethod[] GetInitializeMethods(IDesignerSerializationManager manager, CodeTypeDeclaration declaration)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(declaration);

        foreach (CodeTypeMember member in declaration.Members)
        {
            if (member is CodeConstructor ctor && ctor.Parameters.Count == 0)
            {
                return [ctor];
            }
        }

        return [];
    }

    /// <summary>
    ///  Called by the serialization manager to resolve a name to an object.
    /// </summary>
    private void OnResolveName(object? sender, ResolveNameEventArgs e)
    {
        Debug.Assert(_nameTable is not null, "OnResolveName called and we are not deserializing!");

        // If someone else already found a value, who are we to complain?
        if (e.Value is null)
        {
            IDesignerSerializationManager manager = (IDesignerSerializationManager)sender!;
            e.Value = DeserializeName(manager, e.Name!, null);
        }
    }

    /// <summary>
    ///  This method serializes the given root object and optional collection of members to create a new type definition.
    ///  The members collection can be null or empty. If it contains values, these values will be serialized.
    ///  Values themselves may decide to serialize as either member variables or local variables.
    ///  This determination is done by looking for an extender property on the object called GenerateMember.
    ///  If <see langword="true"/>, a member is generated. Otherwise, a local variable is generated.
    ///  For convenience, the members collection can contain the root object.
    ///  In this case the root object will not also be added as a member or local variable.
    ///  The return value is a CodeTypeDeclaration that defines the root object.
    ///  The name of the type will be taken from the root object’s name, if it was a named object.
    ///  If not, a name will be fabricated from the simple type name of the root class.
    ///  The default implementation of Serialize performs the following tasks:
    ///  •    Context Seeding. The serialization context will be “seeded” with data including the RootContext,
    ///           and CodeTypeDeclaration.
    ///  •    Member Serialization. Next Serialize will walk all of the members and call SerializeToExpression.
    ///           Because serialization is done opportunistically in SerializeToExpression,
    ///           this ensures that we do not serialize twice.
    ///  •    Root Serialization. Finally, the root object is serialized and its statements are added to the
    ///           statement collection.
    ///  •    Statement Integration. After all objects have been serialized the Serialize method orders the statements
    ///           and adds them to a method returned from GetInitializeMethod. Finally, a constructor is fabricated
    ///           that calls all of the methods returned from GetInitializeMethod (this step is skipped for cases when
    ///           GetInitializeMethod returns a constructor.
    /// </summary>
    public virtual CodeTypeDeclaration Serialize(IDesignerSerializationManager manager, object root, ICollection? members)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(root);

        // As a type serializer we are responsible for creating the type declaration. Other serializers may access this
        // type declaration and add members to it, so we need to place it on the context stack. The serialization process
        // also looks at the root context to see if there is a root component. The root context is also used by the serializers
        // to add statement collections for serialized components.

        CodeTypeDeclaration docType = new(manager.GetName(root)!);
        CodeThisReferenceExpression thisRef = new();
        RootContext rootCtx = new(thisRef, root);
        StatementContext statementCtx = new();
        // Populate the statement context with a list of members we'd like to see statements for
        statementCtx.StatementCollection.Populate(root);
        if (members is not null)
        {
            statementCtx.StatementCollection.Populate(members);
        }

        docType.BaseTypes.Add(root.GetType());
        manager.Context.Push(docType);
        manager.Context.Push(rootCtx);
        manager.Context.Push(statementCtx);
        try
        {
            // Do each component, skipping us, since we handle our own serialization.
            // This looks really sweet, but is it worth it?  We take the perf hit of a quicksort + the allocation
            // overhead of 4 bytes for each component. Profiles show this as a 2% cost for a form with 100 controls.
            // Let's meet the perf goals first, then consider uncommenting this.
            if (members is not null)
            {
                foreach (object member in members)
                {
                    if (member != root)
                    {
                        // This returns an expression for the object, if possible. We ignore that. Besides returning an
                        // expression, it fills the statement table in the statement context and we're very interested
                        // in that. After serializing everything we will walk over the statement context's statement
                        // table. We will validate that each and every member we've serialized has a presence in the
                        // statement table. If it doesn't, that's an error in the member's serializer.
                        SerializeToExpression(manager, member);
                    }
                }
            }

            // Now, do the root object last.
            SerializeToExpression(manager, root);

            // After serializing everything we will walk over the statement context's statement table. We will validate
            // that each and every member we've serialized has a presence in the statement table. If it doesn't, that's
            // an error in the member's serializer.
            IntegrateStatements(manager, root, members, statementCtx, docType);
        }
        finally
        {
            Debug.Assert(manager.Context.Current == statementCtx, "Somebody messed up our context stack");
            manager.Context.Pop();
            manager.Context.Pop();
            manager.Context.Pop();
        }

        return docType;
    }

    /// <summary>
    ///  Takes the statement context and integrates all the statements into the correct methods.
    ///  Then, those methods are added to the code type declaration.
    /// </summary>
    private void IntegrateStatements(IDesignerSerializationManager manager, object root, ICollection? members, StatementContext statementCtx, CodeTypeDeclaration typeDecl)
    {
        Dictionary<string, CodeMethodMap> methodMap = [];

        // Go through all of our members and root object and fish out matching statement context info for each object.
        // The statement context will probably contain more objects than our members, because each object that returned
        // a statement collection was placed in the context. That's fine, because for each major component we serialized
        // it pushed its statement collection on the context stack and statements were added there as well, forming a
        // complete graph.

        if (members is not null)
        {
            foreach (object member in members)
            {
                if (member != root)
                { // always skip the root and do it last
                    CodeStatementCollection? statements = statementCtx.StatementCollection[member];
                    if (statements is not null)
                    {
                        CodeMemberMethod method = GetInitializeMethod(manager, typeDecl, member)
                            ?? throw new InvalidOperationException();

                        if (!methodMap.TryGetValue(method.Name, out CodeMethodMap? map))
                        {
                            map = new CodeMethodMap(method);
                            methodMap[method.Name] = map;
                        }

                        if (statements.Count > 0)
                        {
                            map.Add(statements);
                        }
                    }
                }
            }
        }

        // Finally, do the same thing for the root object.
        CodeStatementCollection? rootStatements = statementCtx.StatementCollection[root];
        if (rootStatements is not null)
        {
            CodeMemberMethod rootMethod = GetInitializeMethod(manager, typeDecl, root)
                ?? throw new InvalidOperationException();

            if (!methodMap.TryGetValue(rootMethod.Name, out CodeMethodMap? rootMap))
            {
                rootMap = new CodeMethodMap(rootMethod);
                methodMap[rootMethod.Name] = rootMap;
            }

            if (rootStatements.Count > 0)
            {
                rootMap.Add(rootStatements);
            }
        }

        // Final step -- walk through all of the sections and emit them to the type declaration.
        foreach (CodeMethodMap map in methodMap.Values)
        {
            map.Combine();
            typeDecl.Members.Add(map.Method);
        }
    }
}
