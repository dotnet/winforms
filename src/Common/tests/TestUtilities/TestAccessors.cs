// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

/// <summary>
///  Extension methods for associating internals test accessors with
///  types being tested.
/// </summary>
/// <remarks>
///  <para>In the System namespace for implicit discovery.</para>
/// </remarks>
public static partial class TestAccessors
{
    // Need to pass a null parameter when constructing a static instance
    // of TestAccessor. As this is pretty common and never changes, caching
    // the array here.
    private static readonly object?[] s_nullObjectParam = [null];

    /// <summary>
    ///  Extension that creates a generic internals test accessor for a
    ///  given instance or Type class (if only accessing statics).
    /// </summary>
    /// <param name="instanceOrType">
    ///  Instance or Type class (if only accessing statics).
    /// </param>
    /// <remarks>
    ///  <para>
    ///   Use <see cref="ITestAccessor.CreateDelegate">CreateDelegate</see> to deal with methods that take spans or
    ///   other ref structs. For other members, use the dynamic accessor:
    ///  </para>
    ///  <code>
    ///   <![CDATA[
    ///   Version version = new Version(4, 1);
    ///    Assert.Equal(4, version.TestAccessor().Dynamic._Major));
    ///
    ///    // Or
    ///
    ///    dynamic accessor = version.TestAccessor().Dynamic;
    ///    Assert.Equal(4, accessor._Major));
    ///
    ///    // Or
    ///
    ///    Version version2 = new Version("4.1");
    ///    dynamic accessor = typeof(Version).TestAccessor().Dynamic;
    ///    Assert.Equal(version2, accessor.Parse("4.1")));
    ///   ]]>
    ///  </code>
    /// </remarks>
    public static ITestAccessor TestAccessor(this object instanceOrType)
    {
        ITestAccessor? testAccessor = instanceOrType is Type type
            ? (ITestAccessor?)Activator.CreateInstance(
                typeof(TestAccessor<>).MakeGenericType(type),
                s_nullObjectParam)
            : (ITestAccessor?)Activator.CreateInstance(
                typeof(TestAccessor<>).MakeGenericType(instanceOrType.GetType()),
                instanceOrType);

        return testAccessor
            ?? throw new ArgumentException("Cannot create TestAccessor for Nullable<T> instances with no value.");
    }
}
