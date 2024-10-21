// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable enable

using System.Drawing;
using System.Reflection.Metadata;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms.Tests;

public unsafe class NativeToWinFormsAdapterTests
{
    private static readonly string[] s_restrictedClipboardFormats =
    [
        DataFormats.CommaSeparatedValue,
        DataFormats.Dib,
        DataFormats.Dif,
        DataFormats.PenData,
        DataFormats.Riff,
        DataFormats.Tiff,
        DataFormats.WaveAudio,
        DataFormats.SymbolicLink,
        DataFormats.StringFormat,
        DataFormats.Bitmap,
        DataFormats.EnhancedMetafile,
        DataFormats.FileDrop,
        DataFormats.Html,
        DataFormats.MetafilePict,
        DataFormats.OemText,
        DataFormats.Palette,
        DataFormats.Rtf,
        DataFormats.Text,
        DataFormats.UnicodeText,
        "FileName",
        "FileNameW",
        "System.Drawing.Bitmap"
    ];

    private static readonly string[] s_unboundedClipboardFormats =
    [
        DataFormats.Serializable,
        "something custom"
    ];

    public static TheoryData<string> RestrictedFormat_TheoryData() => s_restrictedClipboardFormats.ToTheoryData();

    public static TheoryData<string> UnboundedFormat_TheoryData() => s_unboundedClipboardFormats.ToTheoryData();

    [Serializable]
    private class TestData : AbstractBase
    {
        public TestData(Point point)
        {
            Location = point;
            Inner = new InnerData("inner");
        }

        public Point Location;
        public InnerData? Inner;

        public override void DoStuff() { }

        [Serializable]
        internal class InnerData
        {
            public InnerData(string text)
            {
                Text = text;
            }

            public string Text;
        }
    }

    internal abstract class AbstractBase
    {
        public abstract void DoStuff();
    }

    private static Type TestDataResolver(TypeName typeName)
    {
        (string name, Type type)[] allowedTypes =
        [
            (typeof(TestData.InnerData).FullName!, typeof(TestData.InnerData)),
            (typeof(AbstractBase).FullName!, typeof(AbstractBase)),
        ];

        string fullName = typeName.FullName;
        foreach (var (name, type) in allowedTypes)
        {
            // Namespace-qualified type name.
            if (name == fullName)
            {
                return type;
            }
        }

        throw new NotSupportedException($"Can't resolve {fullName}");
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat_TheoryData))]
    [MemberData(nameof(RestrictedFormat_TheoryData))]
    public void TryGetData_AsObject_Fail(string format)
    {
        DataObject native = new();
        native.SetData(format, 1);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        dataObject.TryGetData(format, out object? value).Should().BeFalse();
        value.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat_TheoryData))]
    [MemberData(nameof(RestrictedFormat_TheoryData))]
    public void TryGetData_AsInterface_Fail(string format)
    {
        DataObject native = new();
        native.SetData(format, new List<int>() { 1 });
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        dataObject.TryGetData(format, out IList<int>? list).Should().BeFalse();
        list.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat_TheoryData))]
    public void TryGetData_AsConcreteType(string format)
    {
        DataObject native = new();
        List<int> value = new() { 1 };
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        dataObject.TryGetData(format, out List<int>? list).Should().BeTrue();
        list.Should().BeEquivalentTo(value);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat_TheoryData))]
    [MemberData(nameof(RestrictedFormat_TheoryData))]
    public void TryGetData_AsAbstract_Fail(string format)
    {
        DataObject native = new();
        using BinaryFormatterScope scope = new(enable: true);
        native.SetData(format, new TestData(Point.Empty));
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        dataObject.TryGetData(format, out AbstractBase? testData).Should().BeFalse();
        testData.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(RestrictedFormat_TheoryData))]
    public void TryGetData_AsAbstractWithResolver_Fail(string format)
    {
        DataObject native = new();
        TestData value = new(Point.Empty);
        using BinaryFormatterScope scope = new(enable: true);
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        // This fails either in validation of restricted format against the requested type
        // or in deserialization because restricted format is not letting through custom types.
        dataObject.TryGetData(format, TestDataResolver, autoConvert: true, out AbstractBase? testData).Should().BeFalse();
        testData.Should().BeNull();
    }
}
