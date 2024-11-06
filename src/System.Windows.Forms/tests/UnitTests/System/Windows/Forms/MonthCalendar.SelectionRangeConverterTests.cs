// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms.Tests;

public class MonthCalendar_SelectionRangeConverterTests
{
    private readonly SelectionRange _range = new(new DateTime(2022, 1, 1), new DateTime(2022, 12, 31));

    private readonly SelectionRangeConverter _converter;

    public MonthCalendar_SelectionRangeConverterTests()
    {
        _converter = new SelectionRangeConverter();
    }

    private static IDictionary CreatePropertyValues(object start, object end) => new Dictionary<string, object>
    {
        { "Start", start },
        { "End", end }
    };

    [WinFormsTheory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(int), false)]
    public void CanConvertFrom_ReturnsExpected(Type type, bool expected)
    {
        _converter.CanConvertFrom(null, type).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(int), false)]
    public void CanConvertTo_ReturnsExpected(Type type, bool expected)
    {
        _converter.CanConvertTo(null, type).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData("2022-01-01, 2022-12-31", "2022-01-01", "2022-12-31")]
    [InlineData("2022-01-01", "2022-01-01", "2022-01-01")]
    public void ConvertFromString_ReturnsCorrectSelectionRange(string value, string expectedStart, string expectedEnd)
    {
        if (!value.Contains(','))
        {
            // If not, append a comma and the same date to make it a range
            value += $", {value}";
        }

        SelectionRange range = (SelectionRange)_converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

        range.Start.Should().Be(DateTime.Parse(expectedStart));
        range.End.Should().Be(DateTime.Parse(expectedEnd));
    }

    [WinFormsFact]
    public void ConvertFromDateTime_ReturnsSelectionRangeWithSameStartAndEnd()
    {
        DateTime value = new(2022, 1, 1);
        SelectionRange range = (SelectionRange)_converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

        range.Start.Should().Be(value);
        range.End.Should().Be(value);
    }

    [WinFormsFact]
    public void ConvertFromInvalidString_ThrowsArgumentException()
    {
        string value = "invalid";
        Action act = () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

        act.Should().Throw<ArgumentException>();
    }

    public static readonly TheoryData<string, Type, object> s_convertToData = new()
    {
        { "2022-01-01, 2022-12-31", typeof(string), "2022-01-01, 2022-12-31" },
        { "2022-01-01", typeof(DateTime), new DateTime(2022, 1, 1) }
    };

    [WinFormsTheory]
    [MemberData(nameof(s_convertToData))]
    public void ConvertTo_ReturnsExpected(string dateValue, Type targetType, object expected)
    {
        string[] dates = dateValue.Split(',');
        DateTime start = DateTime.Parse(dates[0]);
        DateTime end = dates.Length > 1 ? DateTime.Parse(dates[1]) : start;
        SelectionRange range = new(start, end);

        object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, range, targetType);

        result.Should().Be(expected);
    }

    [WinFormsFact]
    public void ConvertTo_InstanceDescriptor_ReturnsCorrectInstanceDescriptor()
    {
        InstanceDescriptor descriptor = (InstanceDescriptor)_converter.ConvertTo(null, CultureInfo.InvariantCulture, _range, typeof(InstanceDescriptor));
        SelectionRange result = (SelectionRange)descriptor.Invoke();

        result.Start.Should().Be(new DateTime(2022, 1, 1));
        result.End.Should().Be(new DateTime(2022, 12, 31));
    }

    [WinFormsFact]
    public void ConvertTo_UnsupportedType_ThrowsNotSupportedException()
    {
        Action act = () => _converter.ConvertTo(null, CultureInfo.InvariantCulture, _range, typeof(int));

        act.Should().Throw<NotSupportedException>();
    }

    public static readonly TheoryData<object, object> s_test_TheoryData = new()
    {
        { "invalid", new DateTime(2022, 12, 31) },
        { null, new DateTime(2022, 12, 31) },
        { new DateTime(2022, 1, 1), new DateTime(2022, 12, 31) }
    };

    [WinFormsTheory]
    [MemberData(nameof(s_test_TheoryData))]
    public void CreateInstance_InvalidStart_ReturnExcepted(object startDate, object endDate)
    {
        IDictionary propertyValues = CreatePropertyValues(startDate, endDate);
        Func<SelectionRange> func = () => (SelectionRange)_converter.CreateInstance(null, propertyValues);
        if (startDate is not DateTime startTime || endDate is not DateTime endTime)
        {
            func.Should().Throw<ArgumentException>();
        }
        else
        {
            SelectionRange range = func();
            range.Start.Should().Be(startTime);
            range.End.Should().Be(endTime);
        }
    }

    [WinFormsFact]
    public void GetCreateInstanceSupported_ReturnsTrue()
    {
        _converter.GetCreateInstanceSupported(context: null).Should().BeTrue();
    }

    [WinFormsFact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        SelectionRange range = new SelectionRange(DateTime.Now, DateTime.Now.AddDays(1));
        PropertyDescriptorCollection props = _converter.GetProperties(null, range, null);

        props.Count.Should().Be(2);
        props[0].Name.Should().Be("Start");
        props[1].Name.Should().Be("End");
    }

    [WinFormsFact]
    public void GetPropertiesSupported_ReturnsTrue()
    {
        _converter.GetPropertiesSupported(context: null).Should().BeTrue();
    }
}
