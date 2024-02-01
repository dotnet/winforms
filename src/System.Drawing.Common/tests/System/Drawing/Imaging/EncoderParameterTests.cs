﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging.Tests;

public class EncoderParameterTests
{
    private static readonly Encoder s_anyEncoder = Encoder.ChrominanceTable;

    private void CheckEncoderParameter(EncoderParameter encoderParameter, Encoder expectedEncoder, EncoderParameterValueType expectedType, int expectedNumberOfValues)
    {
        Assert.Equal(expectedEncoder.Guid, encoderParameter.Encoder.Guid);
        Assert.Equal(expectedType, encoderParameter.ValueType);
        Assert.Equal(expectedType, encoderParameter.Type);
        Assert.Equal(expectedNumberOfValues, encoderParameter.NumberOfValues);
    }

    public static IEnumerable<object[]> Ctor_Encoder_Byte_TestData
    {
        get
        {
            yield return new object[] { Encoder.ChrominanceTable, byte.MinValue };
            yield return new object[] { Encoder.ColorDepth, byte.MinValue };
            yield return new object[] { Encoder.Compression, byte.MinValue };
            yield return new object[] { Encoder.LuminanceTable, byte.MinValue };
            yield return new object[] { Encoder.Quality, byte.MinValue };
            yield return new object[] { Encoder.RenderMethod, byte.MinValue };
            yield return new object[] { Encoder.SaveFlag, byte.MinValue };
            yield return new object[] { Encoder.ScanMethod, byte.MinValue };
            yield return new object[] { Encoder.Transformation, byte.MinValue };
            yield return new object[] { Encoder.Version, byte.MinValue };
            yield return new object[] { new Encoder(Guid.NewGuid()), byte.MinValue };
            yield return new object[] { new Encoder(Guid.NewGuid()), 1 };
            yield return new object[] { new Encoder(Guid.NewGuid()), byte.MaxValue };
        }
    }

    [Theory]
    [MemberData(nameof(Ctor_Encoder_Byte_TestData))]
    public void Ctor_Encoder_Byte(Encoder encoder, byte value)
    {
        using EncoderParameter ep = new(encoder, value);
        CheckEncoderParameter(ep, encoder, EncoderParameterValueType.ValueTypeByte, 1);
    }

    [Theory]
    [InlineData(false, EncoderParameterValueType.ValueTypeByte)]
    [InlineData(true, EncoderParameterValueType.ValueTypeUndefined)]
    public void Ctor_Encoder_ByteValue_Bool(bool undefined, EncoderParameterValueType expected)
    {
        EncoderParameter ep = new(s_anyEncoder, 0, undefined);
        CheckEncoderParameter(ep, s_anyEncoder, expected, 1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    public void Ctor_Encoder_Short(short value)
    {
        using EncoderParameter ep = new(s_anyEncoder, value);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeShort, 1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public void Ctor_Encoder_Long(long value)
    {
        using EncoderParameter ep = new(s_anyEncoder, value);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeLong, 1);
    }

    [Theory]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(10, 5)]
    [InlineData(-10, -5)]
    public void Ctor_Encoder_Numerator_Denominator(int numerator, int denominator)
    {
        using EncoderParameter ep = new(s_anyEncoder, numerator, denominator);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeRational, 1);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4)]
    public void Ctor_Encoder_Numerator1_Denominator1_Numerator2_Denominator2(int numerator1, int denominator1, int numerator2, int denominator2)
    {
        using EncoderParameter ep = new(s_anyEncoder, numerator1, denominator1, numerator2, denominator2);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeRationalRange, 1);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void Ctor_Encoder_RangeBegin_RangeEnd(long rangeBegin, long rangeEnd)
    {
        using EncoderParameter ep = new(s_anyEncoder, rangeBegin, rangeEnd);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeLongRange, 1);
    }

    [Theory]
    [InlineData("someStringValue")]
    [InlineData("")]
    public void Ctor_Encoder_String(string value)
    {
        using EncoderParameter ep = new(s_anyEncoder, value);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeAscii, value.Length);
    }

    [Theory]
    [InlineData(new byte[] { })]
    [InlineData(new byte[] { 0, 1, 2, 3 })]
    public void Ctor_Encoder_ByteArray(byte[] value)
    {
        using EncoderParameter ep = new(s_anyEncoder, value);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeByte, value.Length);
    }

    [Theory]
    [InlineData(new byte[] { 1, 2 }, false, EncoderParameterValueType.ValueTypeByte)]
    [InlineData(new byte[] { 1, 2 }, true, EncoderParameterValueType.ValueTypeUndefined)]
    public void Ctor_Encoder_ByteArray_Bool(byte[] value, bool undefined, EncoderParameterValueType expected)
    {
        using EncoderParameter ep = new(s_anyEncoder, value, undefined);
        CheckEncoderParameter(ep, s_anyEncoder, expected, value.Length);
    }

    [Theory]
    [InlineData(new short[] { })]
    [InlineData(new short[] { 0, 1, 2, 3 })]
    public void Ctor_Encoder_ShortArray(short[] value)
    {
        using EncoderParameter ep = new(s_anyEncoder, value);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeShort, value.Length);
    }

    [Theory]
    [InlineData(new long[] { })]
    [InlineData(new long[] { 0, 1, 2, 3 })]
    public void Ctor_Encoder_LongArray(long[] value)
    {
        using EncoderParameter ep = new(s_anyEncoder, value);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeLong, value.Length);
    }

    [Theory]
    [InlineData(new int[] { 0, 1, 2, 3 }, new int[] { 5, 6, 7, 8 })]
    public void Ctor_Encoder_NumeratorArray_DenominatorArray(int[] numerator, int[] denominator)
    {
        using EncoderParameter ep = new(s_anyEncoder, numerator, denominator);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeRational, numerator.Length);
    }

    [Theory]
    [InlineData(new long[] { 0, 1, 2, 3 }, new long[] { 5, 6, 7, 8 })]
    public void Ctor_Encoder_RangeBeginArray_RangeEndArray(long[] rangeBegin, long[] rangeEnd)
    {
        using EncoderParameter ep = new(s_anyEncoder, rangeBegin, rangeEnd);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeLongRange, rangeBegin.Length);
    }

    [Theory]
    [InlineData(new int[] { 0, 1, 2, 3 }, new int[] { 4, 5, 6, 7 }, new int[] { 8, 9, 10, 11 }, new int[] { 12, 13, 14, 15 })]
    public void Ctor_Encoder_Numerator1Array_Denominator1Array_Numerator2Array_Denominator2Array(int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2)
    {
        using EncoderParameter ep = new(s_anyEncoder, numerator1, denominator1, numerator2, denominator2);
        CheckEncoderParameter(ep, s_anyEncoder, EncoderParameterValueType.ValueTypeRationalRange, numerator1.Length);
    }

    public static IEnumerable<object[]> Encoder_NumberOfValues_TestData
    {
        get
        {
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeAscii, IntPtr.Zero };
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeByte, IntPtr.Zero };
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeLong, IntPtr.Zero };
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeLongRange, IntPtr.Zero };
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeRational, IntPtr.Zero };
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeRationalRange, IntPtr.Zero };
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeShort, IntPtr.Zero };
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeUndefined, IntPtr.Zero };
            yield return new object[] { 0, EncoderParameterValueType.ValueTypeUndefined, IntPtr.Zero };
        }
    }

    [Theory]
    [MemberData(nameof(Encoder_NumberOfValues_TestData))]
    public void Ctor_Encoder_NumberOfValues_Type_Value(int numberOfValues, EncoderParameterValueType type, IntPtr value)
    {
        using EncoderParameter ep = new(s_anyEncoder, numberOfValues, type, value);
        CheckEncoderParameter(ep, s_anyEncoder, type, numberOfValues);
    }

    [Fact]
    public void Encoder_ReturnsExpecetd()
    {
        Encoder encoder = new(Guid.NewGuid());
        using EncoderParameter ep = new(s_anyEncoder, 0)
        {
            Encoder = encoder
        };
        Assert.Equal(encoder.Guid, ep.Encoder.Guid);
    }

    [Fact]
    public void Ctor_Encoder_NumberOfValues_NotExistingType_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => new EncoderParameter(s_anyEncoder, 1, (EncoderParameterValueType)999, IntPtr.Zero));
    }

    [Theory]
    [InlineData(new int[] { 1, 2 }, new int[] { 1 }, new int[] { 1 }, new int[] { 1 }, typeof(ArgumentException))]
    [InlineData(new int[] { 1 }, new int[] { 1, 2 }, new int[] { 1 }, new int[] { 1 }, typeof(ArgumentException))]
    [InlineData(null, new int[] { 1 }, new int[] { 1 }, new int[] { 1 }, typeof(NullReferenceException))]
    [InlineData(new int[] { 1 }, null, new int[] { 1 }, new int[] { 1 }, typeof(NullReferenceException))]
    [InlineData(new int[] { 1 }, new int[] { 1 }, null, new int[] { 1 }, typeof(NullReferenceException))]
    [InlineData(new int[] { 1 }, new int[] { 1 }, new int[] { 1 }, null, typeof(NullReferenceException))]
    public void Ctor_Encoder_Numerator1Array_Denominator1Array_Numerator2Array_Denominator2Array_InvalidParameters_ThrowsExpected(int[]? numerator1, int[]? denominator1, int[]? numerator2, int[]? denominator2, Type expected)
    {
        Assert.Throws(expected, () => new EncoderParameter(s_anyEncoder, numerator1, denominator1, numerator2, denominator2));
    }

    [Fact]
    public void Encoder_Null_ThrowsNullReferenceException()
    {
        using EncoderParameter ep = new(s_anyEncoder, 0);
        Assert.Throws<NullReferenceException>(() => ep.Encoder = null);
    }

    [Theory]
    [InlineData(new int[] { 0 }, new int[] { 0, 1 }, typeof(ArgumentException))]
    [InlineData(new int[] { 0, 1 }, new int[] { 0 }, typeof(ArgumentException))]
    [InlineData(new int[] { 0, 1 }, null, typeof(NullReferenceException))]
    [InlineData(null, new int[] { 0, 1 }, typeof(NullReferenceException))]
    public void Ctor_Numerator_Denominator_IvalidValues_ThrowsExpected(int[]? numerator, int[]? denominator, Type expected)
    {
        Assert.Throws(expected, () => new EncoderParameter(s_anyEncoder, numerator, denominator));
    }

    [Theory]
    [InlineData(new long[] { 0 }, new long[] { 0, 1 }, typeof(ArgumentException))]
    [InlineData(new long[] { 0, 1 }, new long[] { 0 }, typeof(ArgumentException))]
    [InlineData(new long[] { 0, 1 }, null, typeof(NullReferenceException))]
    [InlineData(null, new long[] { 0, 1 }, typeof(NullReferenceException))]
    public void Ctor_RangeBegin_RangeEnd_InvalidValues_ThrowsExpected(long[]? rangeBegin, long[]? rangeEnd, Type expected)
    {
        Assert.Throws(expected, () => new EncoderParameter(s_anyEncoder, rangeBegin, rangeEnd));
    }

    [Fact]
    public void Ctor_Encoder_NullString_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(s_anyEncoder, (string)null));
    }

    [Fact]
    public void Ctor_Encoder_ByteArray_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(s_anyEncoder, (byte[])null));
    }

    public static IEnumerable<object[]> EncoderParameterCtor_NullEncoder_TestData
    {
        get
        {
            yield return new object[] { new Action(() => new EncoderParameter(null, 0)) };
            yield return new object[] { new Action(() => new EncoderParameter(null, 0, false)) };
            yield return new object[] { new Action(() => new EncoderParameter(null, (short)0)) };
            yield return new object[] { new Action(() => new EncoderParameter(null, numerator: 0, denominator: 0)) };
            yield return new object[] { new Action(() => new EncoderParameter(null, rangebegin: 0, rangeend: 0)) };
            yield return new object[] { new Action(() => new EncoderParameter(null, 0, 0, 0, 0)) };
            yield return new object[] { new Action(() => new EncoderParameter(null, "anyString")) };
            yield return new object[] { new Action(() => new EncoderParameter(null, new byte[] { })) };
            yield return new object[] { new Action(() => new EncoderParameter(null, [], false)) };
            yield return new object[] { new Action(() => new EncoderParameter(null, new short[] { })) };
            yield return new object[] { new Action(() => new EncoderParameter(null, new long[] { })) };
            yield return new object[] { new Action(() => new EncoderParameter(null, new int[] { }, [])) };
            yield return new object[] { new Action(() => new EncoderParameter(null, new long[] { }, [])) };
        }
    }

    [Fact]
    public void Ctor_NullEncoder_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, 0));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, 0, false));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, (short)0));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, numerator: 0, denominator: 0));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, rangebegin: 0, rangeend: 0));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, 0, 0, 0, 0));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, "anyString"));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, new byte[] { }));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, [], false));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, new short[] { }));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, new long[] { }));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, new int[] { }, []));
        Assert.Throws<NullReferenceException>(() => new EncoderParameter(null, new long[] { }, []));
    }

    [Theory]
    [InlineData(EncoderParameterValueType.ValueTypeShort, (int.MaxValue / 2) + 1, typeof(OverflowException))]
    [InlineData(EncoderParameterValueType.ValueTypeLong, (int.MaxValue / 4) + 1, typeof(OverflowException))]
    [InlineData(EncoderParameterValueType.ValueTypeRational, (int.MaxValue / 8) + 1, typeof(OverflowException))]
    [InlineData(EncoderParameterValueType.ValueTypeLongRange, (int.MaxValue / 8) + 1, typeof(OverflowException))]
    [InlineData(EncoderParameterValueType.ValueTypeRationalRange, (int.MaxValue / 16) + 1, typeof(OverflowException))]
    public void Ctor_Encoder_TooBigNumberOfValues_Type_Value_AccessViolationException(EncoderParameterValueType type, int numberOfValues, Type expected)
    {
        Assert.Throws(expected, () => new EncoderParameter(s_anyEncoder, numberOfValues, type, IntPtr.Zero));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [SkipOnArchitecture(TestArchitectures.X86, "backwards compatibility on 32 bit platforms may not throw")]
    // This test may depend on amount of RAM and system configuration and load.
    public void Ctor_Encoder_NegativeNumberOfValues_Type_Value_OutOfMemoryException(int numberOfValues)
    {
        IntPtr anyValue = IntPtr.Zero;
        EncoderParameterValueType anyType = EncoderParameterValueType.ValueTypeAscii;
        Assert.Throws<OutOfMemoryException>(() => new EncoderParameter(s_anyEncoder, numberOfValues, anyType, anyValue));
    }
}
