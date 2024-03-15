// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Moq;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class FeatureSupportTests
{
    public static IEnumerable<object[]> IsPresent_DefaultMinimumVersion_TestData()
    {
        yield return new object[] { null, new(), false };
        yield return new object[] { new Version(0, 0, 0, 0), new(), true };
        yield return new object[] { new Version(1, 2, 3, 4), new(), true };
    }

    [Theory]
    [MemberData(nameof(IsPresent_DefaultMinimumVersion_TestData))]
    public void FeatureSupport_IsPresent_InvokeDefaultMinimVersion_ReturnsExpected(Version versionPresent, object feature, bool expected)
    {
        Mock<FeatureSupport> featureSupport = new(MockBehavior.Strict);
        featureSupport
            .Setup(s => s.IsPresent(feature))
            .CallBase();
        featureSupport
            .Setup(s => s.IsPresent(feature, new Version(0, 0, 0, 0)))
            .CallBase();
        featureSupport
            .Setup(s => s.GetVersionPresent(feature))
            .Returns(versionPresent);
        Assert.Equal(expected, featureSupport.Object.IsPresent(feature));
    }

    public static IEnumerable<object[]> IsPresent_CustomMinimumVersion_TestData()
    {
        yield return new object[] { null, new(), new Version(1, 2, 3, 4), false };
        yield return new object[] { null, new(), null, false };
        yield return new object[] { new Version(0, 0, 0, 0), new(), new Version(0, 0, 0, 0), true };
        yield return new object[] { new Version(0, 0, 0, 0), new(), new Version(1, 2, 3, 4), false };
        yield return new object[] { new Version(0, 0, 0, 0), new(), null, true };
        yield return new object[] { new Version(1, 2, 3, 4), new(), new Version(1, 2, 3, 4), true };
        yield return new object[] { new Version(1, 2, 3, 4), new(), new Version(1, 2, 3, 4), true };
        yield return new object[] { new Version(1, 2, 3, 4), new(), new Version(2, 3, 4, 5), false };
        yield return new object[] { new Version(1, 2, 3, 4), new(), null, true };
    }

    [Theory]
    [MemberData(nameof(IsPresent_CustomMinimumVersion_TestData))]
    public void FeatureSupport_IsPresent_InvokeCustomMinimumVersion_ReturnsExpected(Version versionPresent, object feature, Version minimumVersion, bool expected)
    {
        Mock<FeatureSupport> featureSupport = new(MockBehavior.Strict);
        featureSupport
            .Setup(s => s.IsPresent(feature, minimumVersion))
            .CallBase();
        featureSupport
            .Setup(s => s.GetVersionPresent(feature))
            .Returns(versionPresent);
        Assert.Equal(expected, featureSupport.Object.IsPresent(feature, minimumVersion));
    }

    public static IEnumerable<object[]> GetVersionPresent_TestData()
    {
        foreach (string invalidType in new string[] { null, string.Empty, "NoSuchType" })
        {
            yield return new object[] { invalidType, null, null };
            yield return new object[] { invalidType, string.Empty, null };
            yield return new object[] { invalidType, "NoSuchField", null };
        }

        yield return new object[] { typeof(NonIFeatureSupportClass).AssemblyQualifiedName, nameof(NonIFeatureSupportClass.s_field), null };

        yield return new object[] { typeof(IFeatureSupportClass).FullName, nameof(IFeatureSupportClass.s_field), null };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "NoSuchField", null };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "_privateField", null };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "s_privateField", null };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, nameof(IFeatureSupportClass.s_field), new Version(1, 2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(GetVersionPresent_TestData))]
    public void FeatureSupport_GetVersionPresent_Invoke_ReturnsExpected(string featureClassName, string featureConstName, Version expected)
    {
        Assert.Equal(expected, FeatureSupport.GetVersionPresent(featureClassName, featureConstName));
    }

    [Theory]
    [InlineData(typeof(IFeatureSupportClass), nameof(IFeatureSupportClass._field))]
    [InlineData(typeof(NonIFeatureSupportClass), nameof(NonIFeatureSupportClass._field))]
    public void FeatureSupport_GetVersionPresent_NonStaticField_ThrowsTargetException(Type featureClassType, string featureConstName)
    {
        Assert.Throws<TargetException>(() => FeatureSupport.GetVersionPresent(featureClassType.AssemblyQualifiedName, featureConstName));
    }

    public static IEnumerable<object[]> IsPresent_DefaultMinimumVersionClassName_TestData()
    {
        foreach (string invalidType in new string[] { null, string.Empty, "NoSuchType" })
        {
            yield return new object[] { invalidType, null, false };
            yield return new object[] { invalidType, string.Empty, false };
            yield return new object[] { invalidType, "NoSuchField", false };
        }

        yield return new object[] { typeof(NonIFeatureSupportClass).AssemblyQualifiedName, nameof(NonIFeatureSupportClass.s_field), false };

        yield return new object[] { typeof(IFeatureSupportClass).FullName, nameof(IFeatureSupportClass.s_field), false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "NoSuchField", false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "_privateField", false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "s_privateField", false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, nameof(IFeatureSupportClass.s_zeroVersionField), true };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, nameof(IFeatureSupportClass.s_nonZeroVersionField), true };
    }

    [Theory]
    [MemberData(nameof(IsPresent_DefaultMinimumVersionClassName_TestData))]
    public void FeatureSupport_IsPresent_InvokeDefaultMinimumVersionClassName_ReturnsExpected(string featureClassName, string featureConstName, bool expected)
    {
        Assert.Equal(expected, FeatureSupport.IsPresent(featureClassName, featureConstName));
    }

    public static IEnumerable<object[]> IsPresent_CustomMinimumVersionClassName_TestData()
    {
        foreach (string invalidType in new string[] { null, string.Empty, "NoSuchType" })
        {
            yield return new object[] { invalidType, null, new Version(1, 2, 3, 4), false };
            yield return new object[] { invalidType, string.Empty, new Version(1, 2, 3, 4), false };
            yield return new object[] { invalidType, "NoSuchField", new Version(1, 2, 3, 4), false };
        }

        yield return new object[] { typeof(NonIFeatureSupportClass).AssemblyQualifiedName, nameof(NonIFeatureSupportClass.s_field), new Version(1, 2, 3, 4), false };

        yield return new object[] { typeof(IFeatureSupportClass).FullName, nameof(IFeatureSupportClass.s_field), new Version(1, 2, 3, 4), false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "NoSuchField", new Version(1, 2, 3, 4), false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "_privateField", new Version(1, 2, 3, 4), false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, "s_privateField", new Version(1, 2, 3, 4), false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, nameof(IFeatureSupportClass.s_zeroVersionField), new Version(0, 0, 0, 0), true };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, nameof(IFeatureSupportClass.s_zeroVersionField), new Version(1, 2, 3, 4), false };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, nameof(IFeatureSupportClass.s_nonZeroVersionField), new Version(0, 0, 0, 0), true };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, nameof(IFeatureSupportClass.s_nonZeroVersionField), new Version(1, 2, 3, 4), true };
        yield return new object[] { typeof(IFeatureSupportClass).AssemblyQualifiedName, nameof(IFeatureSupportClass.s_nonZeroVersionField), new Version(2, 3, 4, 5), false };
    }

    [Theory]
    [MemberData(nameof(IsPresent_CustomMinimumVersionClassName_TestData))]
    public void FeatureSupport_IsPresent_InvokeCustomMinimumVersionClassName_ReturnsExpected(string featureClassName, string featureConstName, Version minimumVersion, bool expected)
    {
        Assert.Equal(expected, FeatureSupport.IsPresent(featureClassName, featureConstName, minimumVersion));
    }

    [Theory]
    [InlineData(typeof(IFeatureSupportClass), nameof(IFeatureSupportClass._field))]
    [InlineData(typeof(NonIFeatureSupportClass), nameof(NonIFeatureSupportClass._field))]
    public void FeatureSupport_IsPresent_NonStaticField_ThrowsTargetException(Type featureClassType, string featureConstName)
    {
        Assert.Throws<TargetException>(() => FeatureSupport.IsPresent(featureClassType.AssemblyQualifiedName, featureConstName));
        Assert.Throws<TargetException>(() => FeatureSupport.IsPresent(featureClassType.AssemblyQualifiedName, featureConstName, new Version(1, 2, 3, 4)));
    }

    private class IFeatureSupportClass : IFeatureSupport
    {
        public bool IsPresent(object feature) => throw new NotImplementedException();

        public bool IsPresent(object feature, Version minimumVersion)
        {
            if (feature == s_zeroVersionField)
            {
                return new Version(0, 0, 0, 0) >= minimumVersion;
            }
            else if (feature == s_nonZeroVersionField)
            {
                return new Version(1, 2, 3, 4) >= minimumVersion;
            }

            throw new InvalidOperationException();
        }

        public Version GetVersionPresent(object feature)
        {
            Assert.Equal(s_field, feature);
            return new Version(1, 2, 3, 4);
        }

        public object _field = new();
        public static object s_field = new();

        public static object s_zeroVersionField = new();
        public static object s_nonZeroVersionField = new();

#pragma warning disable CA1823  // Unused field
#pragma warning disable IDE0052 // Remove unread private members
        private readonly object _privateField = new();
        private static readonly object s_privateField = new();
#pragma warning restore CA1823
#pragma warning restore IDE0052
    }

    private class NonIFeatureSupportClass
    {
        public object _field = new();
        public static object s_field = new();
    }
}
