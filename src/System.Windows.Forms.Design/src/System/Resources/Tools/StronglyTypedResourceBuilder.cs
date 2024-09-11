// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.CodeDom;
using System.Reflection;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Security;

namespace System.Resources.Tools;

/// <summary>
///  Provides support for strongly typed resources. This class cannot be inherited.
/// </summary>
/// <remarks>
///  <para>
///   Typically, resources separate code from content within an application. Creating and consuming these resources
///   makes it easier to develop localizable applications. In the .NET Framework, resources are usually consumed by
///   using the <see cref="ResourceManager"/> class, which contains methods that provide access to culture-specific
///   resources at run time. For more information about creating and consuming resources, see
///   <seealso href="https://docs.microsoft.com/dotnet/core/extensions/resources">Resources in Desktop Apps</seealso>.
///  </para>
///  <para>
///   Strongly typed resource support is a compile-time feature that encapsulates access to resources by creating
///   classes that contain a set of static, read-only(get) properties. This provides an alternative way to consume
///   resources instead of calling the <see cref="ResourceManager.GetString(string)"/> and
///   <see cref="ResourceManager.GetObject(string)"/> methods.
///  </para>
///  <para>
///   The basic functionality for strongly typed resource support is provided by th
///   <see cref="StronglyTypedResourceBuilder"/> class (as well as the /str command-line option in the
///   <seealso href="https://docs.microsoft.com/dotnet/framework/tools/resgen-exe-resource-file-generator">
///    Resgen.exe(Resource File Generator)
///   </seealso>).
///   The output of the <see cref="Create(IDictionary, string, string, CodeDomProvider, bool, out string[])"/> method
///   is a class that contains strongly typed properties that match the resources that are referenced in the input
///   parameter. This class provides read-only access to the resources that are available in the file processed.
///  </para>
/// </remarks>
public static partial class StronglyTypedResourceBuilder
{
    // Note - if you add a new property to the class, add logic to reject keys of that name in VerifyResourceNames.

    private const string ResourceManagerFieldName = "resourceMan";
    private const string ResourceManagerPropertyName = "ResourceManager";
    private const string CultureInfoFieldName = "resourceCulture";
    private const string CultureInfoPropertyName = "Culture";

    // When fixing up identifiers, we will replace all these chars with ReplacementChar ('_').
    private static ReadOnlySpan<char> CharsToReplace =>
    [
        ' ', '\u00A0' /* non-breaking space */, '.', ',', ';', '|', '~', '@', '#', '%', '^', '&', '*', '+', '-',
        '/', '\\', '<', '>', '?', '[', ']', '(', ')', '{', '}', '\"', '\'', ':', '!'
    ];

    private const char ReplacementChar = '_';

    private const string DocCommentSummaryStart = "<summary>";
    private const string DocCommentSummaryEnd = "</summary>";

    // Maximum size of a string resource to show in the doc comment for its property
    private const int DocCommentLengthThreshold = 512;

    /// <summary>
    ///  Generates a class file that contains strongly typed properties that match the resources referenced in
    ///  the specified collection.
    /// </summary>
    /// <param name="resourceList">
    ///  An <see cref="IDictionary"/> collection where each dictionary entry key/value pair is the name of a
    ///  resource and the value of the resource.
    /// </param>
    /// <param name="baseName">The name of the class to be generated.</param>
    /// <param name="generatedCodeNamespace">The namespace of the class to be generated.</param>
    /// <param name="codeProvider">
    ///  A <see cref="CodeDomProvider"/> class that provides the language in which the class will be generated.
    /// </param>
    /// <param name="internalClass">
    ///  <see langword="true"/> to generate an internal class; <see langword="false"/> to generate a public class.
    /// </param>
    /// <param name="unmatchable">
    ///  A <see cref="string"/> array that contains each resource name for which a property cannot be generated.
    ///  Typically, a property cannot be generated because the resource name is not a valid identifier.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///  <para>
    ///   <paramref name="resourceList"/>, <paramref name="baseName"/>, or <paramref name="codeProvider"/> is null.
    ///  </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///  <para>
    ///   <paramref name="baseName"/> can not be converted into a class name.
    ///  </para>
    ///  <para>
    ///   <paramref name="resourceList"/> contains <see cref="ResXDataNode"/> values where the key does not
    ///   match <see cref="ResXDataNode.Name"/>.
    ///  </para>
    /// </exception>
    /// <returns>A <see cref="CodeCompileUnit"/> container.</returns>
    public static CodeCompileUnit Create(
        IDictionary resourceList,
        string baseName,
        string? generatedCodeNamespace,
        CodeDomProvider codeProvider,
        bool internalClass,
        out string[]? unmatchable)
        => Create(
            resourceList,
            baseName,
            generatedCodeNamespace,
            resourcesNamespace: null,
            codeProvider,
            internalClass,
            out unmatchable);

    /// <summary>
    ///  Generates a class file that contains strongly typed properties that match the resources referenced in the
    ///  specified collection.
    /// </summary>
    /// <param name="resourceList">
    ///  An <see cref="IDictionary"/> collection where each dictionary entry key/value pair is the name of a
    ///  resource and the value of the resource.
    /// </param>
    /// <param name="baseName">The name of the class to be generated.</param>
    /// <param name="generatedCodeNamespace">The namespace of the class to be generated.</param>
    /// <param name="resourcesNamespace">The namespace of the resource to be generated.</param>
    /// <param name="codeProvider">
    ///  A <see cref="CodeDomProvider"/> class that provides the language in which the class will be generated.
    /// </param>
    /// <param name="internalClass">
    ///  <see langword="true"/> to generate an internal class; <see langword="false"/> to generate a public class.
    /// </param>
    /// <param name="unmatchable">
    ///  A <see cref="string"/> array that contains each resource name for which a property cannot be generated.
    ///  Typically, a property cannot be generated because the resource name is not a valid identifier.
    /// </param>
    /// <returns>A <see cref="CodeCompileUnit"/> container.</returns>
    /// <exception cref="ArgumentNullException">
    ///  <para>
    ///   <paramref name="resourceList"/>, <paramref name="baseName"/>, or <paramref name="codeProvider"/> is null.
    ///  </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///  <para>
    ///   <paramref name="baseName"/> can not be converted into a class name.
    ///  </para>
    ///  <para>
    ///   <paramref name="resourceList"/> contains <see cref="ResXDataNode"/> values where the key does not
    ///   match <see cref="ResXDataNode.Name"/>.
    ///  </para>
    /// </exception>
    public static CodeCompileUnit Create(
        IDictionary resourceList,
        string baseName,
        string? generatedCodeNamespace,
        string? resourcesNamespace,
        CodeDomProvider codeProvider,
        bool internalClass,
        out string[]? unmatchable)
    {
        ArgumentNullException.ThrowIfNull(resourceList);

        Dictionary<string, ResourceData> resourceTypes = new(StringComparer.InvariantCultureIgnoreCase);
        foreach (DictionaryEntry entry in resourceList)
        {
            ResourceData data;
            if (entry.Value is ResXDataNode node)
            {
                string keyname = (string)entry.Key;
                if (keyname != node.Name)
                {
                    throw new ArgumentException(string.Format(SR.MismatchedResourceName, keyname, node.Name));
                }

                string typeName = node.GetValueTypeName((AssemblyName[]?)null)!;
                Type? type = Type.GetType(typeName);
                string? valueAsString = node.GetValue((AssemblyName[]?)null)!.ToString();
                data = new ResourceData(type, valueAsString);
            }
            else
            {
                // If the object is null, we don't have a good way of guessing the type. Use Object. This will be
                // rare after WinForms gets away from their resource pull model in Whidbey M3.
                Type type = (entry.Value is null) ? typeof(object) : entry.Value.GetType();
                data = new ResourceData(type, entry.Value?.ToString());
            }

            resourceTypes.Add((string)entry.Key, data);
        }

        // Note we still need to verify the resource names are valid language keywords, etc. So there's no point to
        // duplicating the code above.

        return InternalCreate(
            resourceTypes,
            baseName,
            generatedCodeNamespace,
            resourcesNamespace,
            codeProvider,
            internalClass,
            out unmatchable);
    }

    /// <summary>
    ///  Generates a class file that contains strongly typed properties that match the resources in the specified
    ///  .resx file.
    /// </summary>
    /// <param name="resxFile">The name of a .resx file used as input.</param>
    /// <param name="baseName">The name of the class to be generated.</param>
    /// <param name="generatedCodeNamespace">The namespace of the class to be generated.</param>
    /// <param name="codeProvider">
    ///  A <see cref="CodeDomProvider"/> class that provides the language in which the class will be generated.
    /// </param>
    /// <param name="internalClass">
    ///  <see langword="true"/> to generate an internal class; <see langword="false"/> to generate a public class.
    /// </param>
    /// <param name="unmatchable">
    ///  A <see cref="string"/> array that contains each resource name for which a property cannot be generated.
    ///  Typically, a property cannot be generated because the resource name is not a valid identifier.
    /// </param>
    /// <returns>A <see cref="CodeCompileUnit"/> container.</returns>
    public static CodeCompileUnit Create(
        string resxFile,
        string baseName,
        string? generatedCodeNamespace,
        CodeDomProvider codeProvider,
        bool internalClass,
        out string[]? unmatchable)
        => Create(
            resxFile,
            baseName,
            generatedCodeNamespace,
            resourcesNamespace: null,
            codeProvider,
            internalClass,
            out unmatchable);

    /// <summary>
    ///  Generates a class file that contains strongly typed properties that match the resources in the specified
    ///  .resx file.
    /// </summary>
    /// <param name="resxFile">The name of a .resx file used as input.</param>
    /// <param name="baseName">The name of the class to be generated.</param>
    /// <param name="generatedCodeNamespace">The namespace of the class to be generated.</param>
    /// <param name="resourcesNamespace">The namespace of the resource to be generated.</param>
    /// <param name="codeProvider">
    ///  A <see cref="CodeDomProvider"/> class that provides the language in which the class will be generated.
    /// </param>
    /// <param name="internalClass">
    ///  <see langword="true"/> to generate an internal class; <see langword="false"/> to generate a public class.
    /// </param>
    /// <param name="unmatchable">
    ///  A <see cref="string"/> array that contains each resource name for which a property cannot be generated.
    ///  Typically, a property cannot be generated because the resource name is not a valid identifier.
    /// </param>
    /// <returns>A <see cref="CodeCompileUnit"/> container.</returns>
    public static CodeCompileUnit Create(
        string resxFile,
        string baseName,
        string? generatedCodeNamespace,
        string? resourcesNamespace,
        CodeDomProvider codeProvider,
        bool internalClass,
        out string[]? unmatchable)
    {
        ArgumentNullException.ThrowIfNull(resxFile);

        // Read the resources from a ResX file into a dictionary - name & type name
        Dictionary<string, ResourceData> resourceList = new(StringComparer.InvariantCultureIgnoreCase);
        using (ResXResourceReader reader = new(resxFile))
        {
            reader.UseResXDataNodes = true;
            foreach (DictionaryEntry entry in reader)
            {
                ResXDataNode node = (ResXDataNode)entry.Value!;
                ResourceData data = new(
                    Type.GetType(node.GetValueTypeName(names: null)!),
                    node.GetValue(names: null)!.ToString());
                resourceList.Add((string)entry.Key, data);
            }
        }

        // Note we still need to verify the resource names are valid language
        // keywords, etc. So there's no point to duplicating the code above.

        return InternalCreate(
            resourceList,
            baseName,
            generatedCodeNamespace,
            resourcesNamespace,
            codeProvider,
            internalClass,
            out unmatchable);
    }

    /// <exception cref="ArgumentException">
    ///  <paramref name="baseName"/> can not be converted into a valid class name.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="baseName"/> or <paramref name="codeProvider"/> is null.
    /// </exception>
    private static CodeCompileUnit InternalCreate(
        Dictionary<string, ResourceData> resourceList,
        string baseName,
        string? generatedCodeNamespace,
        string? resourcesNamespace,
        CodeDomProvider codeProvider,
        bool internalClass,
        out string[] unmatchable)
    {
        ArgumentNullException.ThrowIfNull(baseName);
        ArgumentNullException.ThrowIfNull(codeProvider);

        // Keep a list of errors describing known strings that couldn't be fixed up (like "4"), as well as listing
        // all duplicate resources that were fixed up to the same name (like "A B" and "A-B" both going to "A_B").
        List<string> errors = [];

        // Verify the resource names are valid property names, and they don't conflict. This includes checking for
        // language-specific keywords, translating spaces to underscores, etc.
        var cleanedResourceList = VerifyResourceNames(resourceList, codeProvider, errors, out Dictionary<string, string> reverseFixupTable);

        // Verify the class name is legal.
        string className = baseName;

        // Attempt to fix up class name, and throw an exception if it fails.
        if (!codeProvider.IsValidIdentifier(className))
        {
            string? fixedClassName = VerifyResourceName(className, codeProvider);
            if (fixedClassName is not null)
            {
                className = fixedClassName;
            }
        }

        if (!codeProvider.IsValidIdentifier(className))
        {
            throw new ArgumentException(string.Format(SR.InvalidIdentifier, className));
        }

        // If we have a namespace, verify the namespace is legal, attempting to fix it up if needed.
        if (!string.IsNullOrEmpty(generatedCodeNamespace))
        {
            if (!codeProvider.IsValidIdentifier(generatedCodeNamespace))
            {
                string? fixedNamespace = VerifyResourceName(generatedCodeNamespace, codeProvider, true);
                if (fixedNamespace is not null)
                {
                    generatedCodeNamespace = fixedNamespace;
                }
            }

            // Note we cannot really ensure that the generated code namespace is a valid identifier, as namespaces
            // can have '.' and '::', but identifiers cannot.
        }

        CodeCompileUnit codeCompileUnit = new();
        codeCompileUnit.ReferencedAssemblies.Add("System.Runtime.dll");

        codeCompileUnit.UserData.Add("AllowLateBound", value: false);
        codeCompileUnit.UserData.Add("RequireVariableDeclaration", value: true);

        CodeNamespace codeNamespace = new(generatedCodeNamespace);
        codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
        codeCompileUnit.Namespaces.Add(codeNamespace);

        // Generate class
        CodeTypeDeclaration classType = new(className);
        codeNamespace.Types.Add(classType);
        AddGeneratedCodeAttributeforMember(classType);

        TypeAttributes attributes = internalClass ? TypeAttributes.NotPublic : TypeAttributes.Public;
        classType.TypeAttributes = attributes;
        classType.Comments.Add(new(DocCommentSummaryStart, docComment: true));
        classType.Comments.Add(new(SR.ClassDocComment, docComment: true));
        classType.Comments.Add(new(DocCommentSummaryEnd, docComment: true));

        // [global::System.Diagnostics.DebuggerNonUserCode]
        classType.CustomAttributes.Add(new(new CodeTypeReference(typeof(DebuggerNonUserCodeAttribute))
        {
            Options = CodeTypeReferenceOptions.GlobalReference
        }));

        // [global::System.Runtime.CompilerServices.CompilerGenerated]
        classType.CustomAttributes.Add(new(new CodeTypeReference(typeof(CompilerGeneratedAttribute))
        {
            Options = CodeTypeReferenceOptions.GlobalReference
        }));

        // Figure out some basic restrictions to the code generation
        bool useStatic = internalClass || codeProvider.Supports(GeneratorSupport.PublicStaticMembers);

        // Portable libraries which target .NET for Windows Store apps or WindowsPhone V8 applications are using
        // version of CLR which had refactored type information into a separate assembly - System.Reflection, as a
        // result type "Type" does not implement "Assembly" get property.
        bool useTypeInfo = codeProvider is ITargetAwareCodeDomProvider targetAwareProvider
            && !targetAwareProvider.SupportsProperty(typeof(Type), "Assembly", isWritable: false);

        if (useTypeInfo)
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Reflection"));
        }

        EmitBasicClassMembers(
            classType,
            generatedCodeNamespace,
            baseName,
            resourcesNamespace,
            internalClass,
            useStatic,
            useTypeInfo);

        // Now for each resource, add a property
        foreach ((string propertyName, ResourceData resource) in cleanedResourceList)
        {
            // The resourceName will be the original value, before fixups, if any.
            reverseFixupTable.TryGetValue(propertyName, out string? resourceName);
            resourceName ??= propertyName;
            if (!DefineResourceFetchingProperty(
                propertyName,
                resourceName,
                resource,
                classType,
                internalClass,
                useStatic))
            {
                errors.Add(propertyName);
            }
        }

        unmatchable = [.. errors];

        // Validate the generated class now
        CodeGenerator.ValidateIdentifiers(codeCompileUnit);

        return codeCompileUnit;
    }

    private static void AddGeneratedCodeAttributeforMember(CodeTypeMember typeMember)
    {
        CodeAttributeDeclaration generatedCodeAttrib = new(new CodeTypeReference(typeof(GeneratedCodeAttribute)));
        generatedCodeAttrib.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
        CodeAttributeArgument toolArg = new(new CodePrimitiveExpression(typeof(StronglyTypedResourceBuilder).FullName));

        // This historically was the VS version, now pick up the .NET version
        CodeAttributeArgument versionArg =
            new(new CodePrimitiveExpression(
                new AssemblyName(typeof(StronglyTypedResourceBuilder).Assembly.FullName!).Version!.ToString()));

        generatedCodeAttrib.Arguments.Add(toolArg);
        generatedCodeAttrib.Arguments.Add(versionArg);

        typeMember.CustomAttributes.Add(generatedCodeAttrib);
    }

    private static void EmitBasicClassMembers(
        CodeTypeDeclaration classDeclaration,
        string? nameSpace,
        string baseName,
        string? resourcesNamespace,
        bool internalClass,
        bool useStatic,
        bool useTypeInfo)
    {
        const string tempVariableName = "temp";
        string resourceManagerConstructorParameter;

        if (resourcesNamespace is not null)
        {
            resourceManagerConstructorParameter = resourcesNamespace.Length > 0
                ? $"{resourcesNamespace}.{baseName}"
                : baseName;
        }
        else if (!string.IsNullOrEmpty(nameSpace))
        {
            resourceManagerConstructorParameter = $"{nameSpace}.{baseName}";
        }
        else
        {
            resourceManagerConstructorParameter = baseName;
        }

        // Emit class comments
        classDeclaration.Comments.Add(new(SR.ClassComments1));
        classDeclaration.Comments.Add(new(SR.ClassComments2));
        classDeclaration.Comments.Add(new(SR.ClassComments3));
        classDeclaration.Comments.Add(new(SR.ClassComments4));

        CodeAttributeDeclaration suppressAttribute = new(new CodeTypeReference(typeof(SuppressMessageAttribute)));
        suppressAttribute.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
        suppressAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("Microsoft.Performance")));
        suppressAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("CA1811:AvoidUncalledPrivateCode")));

        // Emit the constructor - make it protected even if it is a "static" class to allow subclassing
        CodeConstructor constructor = new();
        constructor.CustomAttributes.Add(suppressAttribute);
        constructor.Attributes = useStatic || internalClass ? MemberAttributes.FamilyAndAssembly : MemberAttributes.Public;
        classDeclaration.Members.Add(constructor);

        // Emit the resource manager field.
        CodeTypeReference resourceManagerType = new(typeof(ResourceManager), CodeTypeReferenceOptions.GlobalReference);
        CodeMemberField field = new(resourceManagerType, ResourceManagerFieldName)
        {
            Attributes = useStatic ? MemberAttributes.Static | MemberAttributes.Private : MemberAttributes.Private
        };

        classDeclaration.Members.Add(field);

        // Emit the resource culture field, and leave it set to null.
        CodeTypeReference cultureInfoType = new(typeof(CultureInfo), CodeTypeReferenceOptions.GlobalReference);
        field = new CodeMemberField(cultureInfoType, CultureInfoFieldName)
        {
            Attributes = useStatic ? MemberAttributes.Static | MemberAttributes.Private : MemberAttributes.Private
        };

        classDeclaration.Members.Add(field);

        // Emit the ResourceManager property
        CodeMemberProperty resourceManagerProperty = new CodeMemberProperty
        {
            Name = ResourceManagerPropertyName,
            HasGet = true,
            HasSet = false,
            Type = resourceManagerType,
            Attributes = internalClass ? MemberAttributes.Assembly : MemberAttributes.Public
        };

        if (useStatic)
        {
            resourceManagerProperty.Attributes |= MemberAttributes.Static;
        }

        classDeclaration.Members.Add(resourceManagerProperty);

        // Mark the ResourceManager property as EditorBrowsableState.Advanced
        CodeTypeReference editorBrowsableStateType = new(typeof(EditorBrowsableState))
        {
            Options = CodeTypeReferenceOptions.GlobalReference
        };

        CodeAttributeArgument editorBrowsableStateAdvanced = new(
            new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(editorBrowsableStateType), "Advanced"));
        CodeAttributeDeclaration editorBrowsableAdvancedAttribute = new(
            "System.ComponentModel.EditorBrowsableAttribute",
            editorBrowsableStateAdvanced);
        editorBrowsableAdvancedAttribute.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
        resourceManagerProperty.CustomAttributes.Add(editorBrowsableAdvancedAttribute);

        // Emit the Culture property (read/write)
        CodeMemberProperty culture = new()
        {
            Name = CultureInfoPropertyName,
            HasGet = true,
            HasSet = true,
            Type = cultureInfoType,
            Attributes = internalClass ? MemberAttributes.Assembly : MemberAttributes.Public
        };

        classDeclaration.Members.Add(culture);

        if (useStatic)
        {
            culture.Attributes |= MemberAttributes.Static;
        }

        // Mark the Culture property as advanced
        culture.CustomAttributes.Add(editorBrowsableAdvancedAttribute);

        // Emit the following:
        //
        // if (resourceMan == null) {
        //    ResourceManager temp = new ResourceManager("<resources-name-with-namespace>", typeof("<class-name>").Assembly);
        //    resourceMan = temp;
        // }
        //
        // return resourceMan;

        CodeFieldReferenceExpression resourceManagerField = new(null, ResourceManagerFieldName);
        CodeMethodReferenceExpression objectEqualsMethod =
            new(new CodeTypeReferenceExpression(typeof(object)), "ReferenceEquals");

        CodeMethodInvokeExpression isResourceManagerNull = new(
            objectEqualsMethod,
            resourceManagerField,
            new CodePrimitiveExpression(null));

        CodePropertyReferenceExpression getAssemblyProperty;

        if (useTypeInfo)
        {
            // typeof(<class-name>).GetTypeInfo().Assembly
            CodeMethodInvokeExpression getTypeInfo = new(
                new CodeTypeOfExpression(new CodeTypeReference(classDeclaration.Name)),
                "GetTypeInfo",
                []);
            getAssemblyProperty = new(getTypeInfo, "Assembly");
        }
        else
        {
            // typeof(<class-name>).Assembly
            getAssemblyProperty = new(
                new CodeTypeOfExpression(new CodeTypeReference(classDeclaration.Name)),
                "Assembly");
        }

        // new ResourceManager(resMgrCtorParam, typeof(<class-name>).Assembly);
        //   - or -
        // new ResourceManager(resMgrCtorParam, typeof(<class-name>).GetTypeInfo().Assembly);
        CodeObjectCreateExpression newResourceManager = new(
            resourceManagerType,
            new CodePrimitiveExpression(resourceManagerConstructorParameter),
            getAssemblyProperty);

        CodeStatement[] assignNewResourceManager =
        [
            new CodeVariableDeclarationStatement(resourceManagerType, tempVariableName, newResourceManager),
            new CodeAssignStatement(resourceManagerField, new CodeVariableReferenceExpression(tempVariableName))
        ];

        resourceManagerProperty.GetStatements.Add(new CodeConditionStatement(isResourceManagerNull, assignNewResourceManager));
        resourceManagerProperty.GetStatements.Add(new CodeMethodReturnStatement(resourceManagerField));

        // Add a doc comment to the ResourceManager property
        resourceManagerProperty.Comments.Add(new(DocCommentSummaryStart, docComment: true));
        resourceManagerProperty.Comments.Add(new(SR.ResMgrPropertyComment, docComment: true));
        resourceManagerProperty.Comments.Add(new(DocCommentSummaryEnd, docComment: true));

        // Emit code for Culture property
        CodeFieldReferenceExpression cultureInfoField = new(targetObject: null, CultureInfoFieldName);
        culture.GetStatements.Add(new CodeMethodReturnStatement(cultureInfoField));

        CodePropertySetValueReferenceExpression newCulture = new();
        culture.SetStatements.Add(new CodeAssignStatement(cultureInfoField, newCulture));

        // Add a doc comment to Culture property
        culture.Comments.Add(new(DocCommentSummaryStart, docComment: true));
        culture.Comments.Add(new(SR.CulturePropertyComment1, docComment: true));
        culture.Comments.Add(new(SR.CulturePropertyComment2, docComment: true));
        culture.Comments.Add(new(DocCommentSummaryEnd, docComment: true));
    }

    /// <summary>
    ///  Truncates <paramref name="commentString"/> if it is too long and ensures it is safely encoded for XML.
    /// </summary>
    [return: NotNullIfNotNull(nameof(commentString))]
    private static string? TruncateAndFormatCommentStringForOutput(string? commentString)
    {
        if (commentString is not null)
        {
            // Stop at some length
            if (commentString.Length > DocCommentLengthThreshold)
            {
                commentString = string.Format(SR.StringPropertyTruncatedComment, commentString[..DocCommentLengthThreshold]);
            }

            // Encode the comment so it is safe for xml.
            commentString = SecurityElement.Escape(commentString);
        }

        return commentString;
    }

    /// <summary>
    ///  Defines a resource fetching property.
    /// </summary>
    /// <remarks>
    ///  <para>Special cases static vs. non-static, as well as public vs. internal.</para>
    /// </remarks>
    private static bool DefineResourceFetchingProperty(
        string propertyName,
        string resourceName,
        ResourceData data,
        CodeTypeDeclaration classDeclaration,
        bool internalClass,
        bool useStatic)
    {
        // Defines a property like this:
        //
        // {public|internal} {static} Point MyPoint {
        //     get {
        //          Object obj = ResourceManager.GetObject("MyPoint", _resCulture);
        //          return (Point) obj; }
        // }

        Type? type = data.Type;

        if (type is null)
        {
            return false;
        }

        CodeMemberProperty property = new()
        {
            Name = propertyName,
            HasGet = true,
            HasSet = false
        };

        if (type == typeof(MemoryStream))
        {
            type = typeof(UnmanagedMemoryStream);
        }

        // Ensure type is publicly visible. This is necessary to ensure uers can access classes via a base type.
        //
        // Imagine a class like Image or Stream as a publicly available base class, then an internal type like
        // MyBitmap or __UnmanagedMemoryStream as an internal implementation for that base class. For publicly
        // available strongly typed resource classes, we must return the public type. For simplicity, we'll do that
        // for internal strongly typed resource classes as well. Ideally we'd also like to check for interfaces
        // like IList, but it isn't clear how to do that without special casing collection interfaces & ignoring
        // serialization interfaces or IDisposable.
        while (!type.IsPublic)
        {
            type = type.BaseType!;
        }

        CodeTypeReference valueType = new(type);
        property.Type = valueType;
        property.Attributes = internalClass ? MemberAttributes.Assembly : MemberAttributes.Public;

        if (useStatic)
        {
            property.Attributes |= MemberAttributes.Static;
        }

        // For Strings, emit this:
        //    return ResourceManager.GetString("name", _resCulture);
        // For Streams, emit this:
        //    return ResourceManager.GetStream("name", _resCulture);
        // For Objects, emit this:
        //    Object obj = ResourceManager.GetObject("name", _resCulture);
        //    return (MyValueType) obj;
        CodePropertyReferenceExpression resourceManagerProperty = new(targetObject: null, "ResourceManager");
        CodeFieldReferenceExpression cultureInfoField = new(useStatic
            ? null
            : new CodeThisReferenceExpression(), CultureInfoFieldName);

        bool isString = type == typeof(string);
        bool isStream = type == typeof(UnmanagedMemoryStream) || type == typeof(MemoryStream);
        string? valueAsString = TruncateAndFormatCommentStringForOutput(data.ValueAsString);
        string typeName = string.Empty;

        if (!isString)
        {
            // Stream or Object
            typeName = TruncateAndFormatCommentStringForOutput(type.ToString());
        }

        string getMethodName;
        if (isString)
        {
            getMethodName = "GetString";
        }
        else if (isStream)
        {
            getMethodName = "GetStream";
        }
        else
        {
            getMethodName = "GetObject";
        }

        string text;
        if (isString)
        {
            text = string.Format(SR.StringPropertyComment, valueAsString);
        }
        else
        {
            // Stream or Object
            if (valueAsString is null || string.Equals(typeName, valueAsString))
            {
                // If the type did not override ToString(), ToString() just returns the type name.
                text = string.Format(SR.NonStringPropertyComment, typeName);
            }
            else
            {
                text = string.Format(SR.NonStringPropertyDetailedComment, typeName, valueAsString);
            }
        }

        property.Comments.Add(new(DocCommentSummaryStart, docComment: true));
        property.Comments.Add(new(text, docComment: true));
        property.Comments.Add(new(DocCommentSummaryEnd, docComment: true));

        CodeExpression getValue = new CodeMethodInvokeExpression(
            resourceManagerProperty,
            getMethodName,
            new CodePrimitiveExpression(resourceName),
            cultureInfoField);

        CodeMethodReturnStatement returnStatement;
        if (isString || isStream)
        {
            returnStatement = new(getValue);
        }
        else
        {
            CodeVariableDeclarationStatement returnObject = new(typeof(object), "obj", getValue);
            property.GetStatements.Add(returnObject);
            returnStatement = new(new CodeCastExpression(valueType, new CodeVariableReferenceExpression("obj")));
        }

        property.GetStatements.Add(returnStatement);

        classDeclaration.Members.Add(property);
        return true;
    }

    /// <summary>
    ///  Generates a valid resource string based on the specified input string and code provider.
    /// </summary>
    /// <param name="key">The string to verify and, if necessary, convert to a valid resource name.</param>
    /// <param name="provider">A <see cref="CodeDomProvider"/> object that specifies the target language to use.</param>
    /// <returns>
    ///  A valid resource name derived from the <paramref name="key"/> parameter. Any invalid tokens are replaced
    ///  with the underscore (_) character, or <see langword="null"/> if the derived string still contains invalid
    ///  characters according to the language specified by the <paramref name="provider"/> parameter.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///  Thrown when <paramref name="key"/> or <paramref name="provider"/> are null.
    /// </exception>
    public static string? VerifyResourceName(string key, CodeDomProvider provider)
        => VerifyResourceName(key, provider, isNameSpace: false);

    // Once CodeDom provides a way to verify a namespace name, revisit this method.
    private static string? VerifyResourceName(string key, CodeDomProvider provider, bool isNameSpace)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(provider);

        foreach (char c in CharsToReplace)
        {
            // For namespaces, allow . and ::
            if (!(isNameSpace && c is '.' or ':'))
            {
                key = key.Replace(c, ReplacementChar);
            }
        }

        if (provider.IsValidIdentifier(key))
        {
            return key;
        }

        // Now try fixing up keywords like "for".
        key = provider.CreateValidIdentifier(key);
        if (provider.IsValidIdentifier(key))
        {
            return key;
        }

        // Make one last ditch effort by prepending _. This fixes keys that start with a number.
        key = $"_{key}";
        return provider.IsValidIdentifier(key) ? key : null;
    }

    private static SortedList<string, ResourceData> VerifyResourceNames(
        Dictionary<string, ResourceData> resourceList,
        CodeDomProvider codeProvider,
        List<string> errors,
        out Dictionary<string, string> reverseFixupTable)
    {
        reverseFixupTable = new(0, StringComparer.InvariantCultureIgnoreCase);
        SortedList<string, ResourceData> cleanedResourceList = new(resourceList.Count, StringComparer.InvariantCultureIgnoreCase);

        foreach (KeyValuePair<string, ResourceData> entry in resourceList)
        {
            string key = entry.Key;

            // Disallow a property named ResourceManager or Culture - we add those. Any other properties we add
            // also must be listed here. Also disallow resource values of type Void.
            if (string.Equals(key, ResourceManagerPropertyName) ||
                string.Equals(key, CultureInfoPropertyName) ||
                typeof(void) == entry.Value.Type)
            {
                errors.Add(key);
                continue;
            }

            // Ignore WinForms design time and hierarchy information. Skip resources starting with $ or >>, like
            // "$this.Text", ">>$this.Name" or ">>treeView1.Parent".
            if ((key.Length > 0 && key[0] == '$') ||
                (key.Length > 1 && key[0] == '>' && key[1] == '>'))
            {
                continue;
            }

            if (!codeProvider.IsValidIdentifier(key))
            {
                string? newKey = VerifyResourceName(key, codeProvider, isNameSpace: false);
                if (newKey is null)
                {
                    errors.Add(key);
                    continue;
                }

                // Now see if we've already mapped another key to the same name.
                if (reverseFixupTable.TryGetValue(newKey, out string? oldDuplicateKey))
                {
                    // We can't handle this key nor the previous one. Remove the old one.
                    if (!errors.Contains(oldDuplicateKey))
                    {
                        errors.Add(oldDuplicateKey);
                    }

                    cleanedResourceList.Remove(newKey);

                    errors.Add(key);
                    continue;
                }

                reverseFixupTable[newKey] = key;
                key = newKey;
            }

            ResourceData value = entry.Value;
            if (!cleanedResourceList.TryAdd(key, value))
            {
                // There was a case-insensitive conflict between two keys. Or possibly one key was fixed up in a
                // way that conflicts with another key (ie, "A B" and "A_B").
                if (reverseFixupTable.Remove(key, out string? fixedUp))
                {
                    if (!errors.Contains(fixedUp))
                    {
                        errors.Add(fixedUp);
                    }
                }

                errors.Add(entry.Key);
                cleanedResourceList.Remove(key);
            }
        }

        return cleanedResourceList;
    }
}
