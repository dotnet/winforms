// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Xunit.Sdk;

namespace Microsoft.WinForms.Test;

/// <summary>
///  An attribute used for providing test data by combining reference assemblies with file sets.
/// </summary>
/// <remarks>
///  <para>
///   This attribute derives from MemberDataAttributeBase and enables the use of the test class's 
///   static member data combined with file set data. It ensures that the required side effects,
///   such as collecting test data files via the caller file path, are properly executed by invoking 
///   the parameterless constructor of the test class.
///  </para>
///  <para>
///   The attribute works by first retrieving base data using the inherited GetData method, and then
///   retrieves file sets by invoking a specific static method ('GetFileSets') from the base type.
///   The final output is a cross product of reference assemblies and file sets.
///  </para>
/// </remarks>
[DataDiscoverer("Xunit.Sdk.MemberDataDiscoverer", "xunit.core")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed partial class CodeTestDataAttribute : MemberDataAttributeBase
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="CodeTestDataAttribute"/> class.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Accepts the name of a public static member on the test class that provides the test data.
    ///   This member is then used in conjunction with file sets to create a complete test data set.
    ///  </para>
    /// </remarks>
    /// <param name="memberName">The name of the public static member on the test class that will provide the test data</param>
    public CodeTestDataAttribute(string memberName)
        : base(memberName, null) { }

    /// <summary>
    ///  Retrieves the test data for the specified test method.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method first validates the provided test method and its declaring type, triggers necessary
    ///   side effects by instantiating the test class, and then retrieves file set data from a static method 
    ///   on the base type. Finally, it creates a cross product of the base data and the file sets.
    ///  </para>
    ///  <para>
    ///   The result is used to feed the test method with all combinations of reference assembly data and file 
    ///   set data.
    ///  </para>
    /// </remarks>
    /// <param name="testMethod">The method for which the test data is to be retrieved.</param>
    /// <returns>A collection of object arrays representing the test data.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        ArgumentNullException.ThrowIfNull(testMethod);

        var memberType = testMethod.DeclaringType
            ?? throw new InvalidOperationException("Could not determine the declaring type of the test method.");

        // We need to instantiate the type to get to the correct file path.
        // This triggers wanted side effects in the base class constructor that are required
        // to collect test data files via [CallerFilePath].
        var constructor = memberType.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException($"The type '{memberType}' does not have a parameterless constructor.");

        _ = constructor.Invoke(null);

        // Try to get the static "GetFileSets" method from the base type:
        var baseType = memberType.BaseType
            ?? throw new InvalidOperationException($"The type '{memberType}' does not have a base type.");

#if !NET9_0 && NET9_0_OR_GREATER
        // This conditional looks weird, but is a trick to get around a mismatch
        // in compiler versions between VS-current and Arcade for the 9.0-Rel branch.
        // Problem: Open generics do not work in earlier versions than .NET 10.
        // (And no, I came up with it, and had to convince GPT 4.5 this will do, not
        //  the other way around.)
        string getFilesMethodName = nameof(RoslynAnalyzerAndCodeFixTestBase<,>.GetFileSets);
#else
        string getFilesMethodName = "GetFileSets";
#endif

        MethodInfo? getFileSetsMethod = baseType.GetMethod(
            getFilesMethodName,
            BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException(
                    $"The type '{baseType}' does not contain a static public method named 'GetFileSets'.");

        // Invoke that method to get the object array:
        IEnumerable<object> fileSets = (IEnumerable<object>) (getFileSetsMethod.Invoke(null, null)
            ?? throw new InvalidOperationException("GetFileSets method returned null or a value that couldn't be cast to IEnumerable<object[]>."));

        // This is the data, which the test class directs itself to get. 
        var baseData = base.GetData(testMethod).ToList();

        // Use LINQ to create the cross product more efficiently
        return baseData
            .SelectMany(referenceAssembly => fileSets.Select(fileSet =>
                new object[] { referenceAssembly[0], fileSet }))
            .ToArray();
    }

    /// <inheritdoc/>
    protected override object[]? ConvertDataItem(MethodInfo testMethod, object item)
    {
        if (item is not object[] array)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Property {0} on {1} yielded an item that is not an object[]",
                    MemberName,
                    MemberType ?? testMethod.DeclaringType
                )
            );
        }

        return array;
    }
}
