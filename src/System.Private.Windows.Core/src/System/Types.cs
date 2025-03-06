// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Text;

namespace System;

internal static class Types
{
    public const string BooleanType = "System.Boolean";
    public const string CharType = "System.Char";
    public const string StringType = "System.String";
    public const string SByteType = "System.SByte";
    public const string ByteType = "System.Byte";
    public const string Int16Type = "System.Int16";
    public const string UInt16Type = "System.UInt16";
    public const string Int32Type = "System.Int32";
    public const string UInt32Type = "System.UInt32";
    public const string Int64Type = "System.Int64";
    public const string DecimalType = "System.Decimal";
    public const string UInt64Type = "System.UInt64";
    public const string SingleType = "System.Single";
    public const string DoubleType = "System.Double";
    public const string TimeSpanType = "System.TimeSpan";
    public const string DateTimeType = "System.DateTime";

    public const string IntPtrType = "System.IntPtr";
    public const string UIntPtrType = "System.UIntPtr";

    public const string HashtableType = "System.Collections.Hashtable";
    public const string ArrayListType = "System.Collections.ArrayList";

    public const string IDictionaryType = "System.Collections.IDictionary";
    public const string ExceptionType = "System.Exception";
    public const string NotSupportedExceptionType = "System.NotSupportedException";

    internal const string ListName = "System.Collections.Generic.List`1";

    public const string BitmapType = "System.Drawing.Bitmap";
    public const string ColorType = "System.Drawing.Color";
    public const string PointType = "System.Drawing.Point";
    public const string PointFType = "System.Drawing.PointF";
    public const string RectangleType = "System.Drawing.Rectangle";
    public const string RectangleFType = "System.Drawing.RectangleF";
    public const string SizeType = "System.Drawing.Size";
    public const string SizeFType = "System.Drawing.SizeF";

    /// <inheritdoc cref="TypeName.Parse(ReadOnlySpan{char}, TypeNameParseOptions?)"/>
    /// <remarks>
    ///  <para>
    ///   This method allows efficient use of interpolated strings with
    ///   <see cref="TypeName.Parse(ReadOnlySpan{char}, TypeNameParseOptions?)"/>
    ///  </para>
    /// </remarks>
    public static TypeName ToTypeName(ref ValueStringBuilder builder)
    {
        using (builder)
        {
            return TypeName.Parse(builder.AsSpan());
        }
    }
}
