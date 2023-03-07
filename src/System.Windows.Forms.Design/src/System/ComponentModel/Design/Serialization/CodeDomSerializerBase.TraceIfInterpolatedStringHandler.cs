﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.ComponentModel.Design.Serialization
{
    public abstract partial class CodeDomSerializerBase
    {
        /// <summary>
        ///  Provides an interpolated string handler for <see
        ///  cref="CodeDomSerializerBase.TraceIf(System.Diagnostics.TraceLevel,bool,ref System.ComponentModel.Design.Serialization.CodeDomSerializerBase.TraceIfInterpolatedStringHandler)"
        ///  /> that only performs formatting if the condition applies and tracing is set to verbose.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [InterpolatedStringHandler]
        internal struct TraceIfInterpolatedStringHandler
        {
            /// <summary>
            ///  The handler we use to perform the formatting.
            /// </summary>
            private StringBuilder.AppendInterpolatedStringHandler _stringBuilderHandler;

            /// <summary>
            ///  The underlying <see cref="StringBuilder"/> instance used by <see cref="_stringBuilderHandler"/>, if any.
            /// </summary>
            private StringBuilder? _builder;

            /// <summary>
            ///  Creates an instance of the handler..
            /// </summary>
            /// <param name="literalLength">The number of constant characters outside of interpolation expressions in
            /// the interpolated string.</param>
            /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
            /// <param name="condition">The condition Boolean passed to the <see cref="Debug"/> method.</param>
            /// <param name="level">The trace level of the message.</param>
            /// <param name="shouldAppend">A value indicating whether formatting should proceed.</param>
            /// <remarks>
            ///  This is intended to be called only by compiler-generated code. Arguments are not validated as they'd
            ///  otherwise be for members intended to be used directly.
            /// </remarks>
            public TraceIfInterpolatedStringHandler(int literalLength, int formattedCount, bool condition, TraceLevel level, out bool shouldAppend)
            {
                if (condition && traceSerialization.Level >= level)
                {
                    _builder = new StringBuilder();
                    _stringBuilderHandler = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount,
                        _builder);
                    shouldAppend = true;
                }
                else
                {
                    _stringBuilderHandler = default;
                    _builder = null;
                    shouldAppend = false;
                }
            }

            /// <summary>
            ///  Extracts the built string from the handler.
            /// </summary>
            internal string ToStringAndClear()
            {
                string s = _builder?.ToString() ?? string.Empty;
                _stringBuilderHandler = default;
                return s;
            }

            /// <summary>
            ///  Writes the specified string to the handler.
            /// </summary>
            /// <param name="value">The string to write.</param>
            public void AppendLiteral(string value) => _stringBuilderHandler.AppendLiteral(value);

            /// <summary>
            ///  Writes the specified value to the handler.
            /// </summary>
            /// <param name="value">The value to write.</param>
            /// <typeparam name="T">The type of the value to write.</typeparam>
            public void AppendFormatted<T>(T value) => _stringBuilderHandler.AppendFormatted(value);

            /// <summary>
            ///  Writes the specified value to the handler.
            /// </summary>
            /// <param name="value">The value to write.</param>
            /// <param name="format">The format string.</param>
            /// <typeparam name="T">The type of the value to write.</typeparam>
            public void AppendFormatted<T>(T value, string? format) => _stringBuilderHandler.AppendFormatted(value, format);

            /// <summary>
            ///  Writes the specified value to the handler.
            /// </summary>
            /// <param name="value">The value to write.</param>
            /// <param name="alignment">Minimum number of characters that should be written for this value. If the value
            /// is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
            /// <typeparam name="T">The type of the value to write.</typeparam>
            public void AppendFormatted<T>(T value, int alignment) => _stringBuilderHandler.AppendFormatted(value, alignment);

            /// <summary>
            ///  Writes the specified value to the handler.
            /// </summary>
            /// <param name="value">The value to write.</param>
            /// <param name="format">The format string.</param>
            /// <param name="alignment">Minimum number of characters that should be written for this value. If the value
            /// is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
            /// <typeparam name="T">The type of the value to write.</typeparam>
            public void AppendFormatted<T>(T value, int alignment, string? format) => _stringBuilderHandler.AppendFormatted(value, alignment, format);

            /// <summary>
            ///  Writes the specified character span to the handler.
            /// </summary>
            /// <param name="value">The span to write.</param>
            public void AppendFormatted(ReadOnlySpan<char> value) => _stringBuilderHandler.AppendFormatted(value);

            /// <summary>
            ///  Writes the specified string of chars to the handler.
            /// </summary>
            /// <param name="value">The span to write.</param>
            /// <param name="alignment">Minimum number of characters that should be written for this value. If the value
            /// is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
            /// <param name="format">The format string.</param>
            public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null) => _stringBuilderHandler.AppendFormatted(value, alignment, format);

            /// <summary>
            ///  Writes the specified value to the handler.
            /// </summary>
            /// <param name="value">The value to write.</param>
            public void AppendFormatted(string? value) => _stringBuilderHandler.AppendFormatted(value);

            /// <summary>
            ///  Writes the specified value to the handler.
            /// </summary>
            /// <param name="value">The value to write.</param>
            /// <param name="alignment">Minimum number of characters that should be written for this value. If the value
            /// is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
            /// <param name="format">The format string.</param>
            public void AppendFormatted(string? value, int alignment = 0, string? format = null) => _stringBuilderHandler.AppendFormatted(value, alignment, format);

            /// <summary>
            ///  Writes the specified value to the handler.
            /// </summary>
            /// <param name="value">The value to write.</param>
            /// <param name="alignment">Minimum number of characters that should be written for this value. If the value
            /// is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
            /// <param name="format">The format string.</param>
            public void AppendFormatted(object? value, int alignment = 0, string? format = null) => _stringBuilderHandler.AppendFormatted(value, alignment, format);
        }
    }
}
