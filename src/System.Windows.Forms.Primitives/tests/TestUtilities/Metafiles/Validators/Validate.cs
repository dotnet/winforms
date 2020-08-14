// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal static class Validate
    {
        internal static IEmfValidator TextOut(
            string text,
            Gdi32.MM mapMode = Gdi32.MM.TEXT,
            Gdi32.BKMODE backgroundMode = Gdi32.BKMODE.TRANSPARENT,
            string? fontFace = null,
            TextOutValidator.Flags validate = default) => new TextOutValidator(
                text,
                textColor: Color.Empty,
                mapMode,
                backgroundMode,
                fontFace,
                validate);

        internal static IEmfValidator TextOut(
            string text,
            Color textColor,
            Gdi32.MM mapMode = Gdi32.MM.TEXT,
            Gdi32.BKMODE backgroundMode = Gdi32.BKMODE.TRANSPARENT,
            string? fontFace = null,
            TextOutValidator.Flags validate = default) => new TextOutValidator(
                text,
                textColor,
                mapMode,
                backgroundMode,
                fontFace,
                validate);

        internal static IEmfValidator LineTo(
            EasyPoint from,
            EasyPoint to,
            Color penColor,
            int penWidth = 1,
            Gdi32.PS penStyle = default,
            Gdi32.R2 rop2Mode = Gdi32.R2.COPYPEN,
            Gdi32.BKMODE backgroundMode = Gdi32.BKMODE.TRANSPARENT,
            LineToValidator.Flags validate = default) => new LineToValidator(
                from,
                to,
                penColor,
                penWidth,
                penStyle,
                rop2Mode,
                backgroundMode,
                validate);

        /// <summary>
        ///  Simple wrapper to allow doing an arbitrary action for a given <paramref name="recordType"/>.
        /// </summary>
        internal static IEmfValidator Action(Gdi32.EMR recordType, ProcessRecordDelegate action)
            => new ActionValidator(recordType, action);

        /// <summary>
        ///  Simple wrapper to allow doing an arbitrary action for a given <paramref name="recordType"/>.
        /// </summary>
        internal static IEmfValidator Action(Gdi32.EMR recordType, ProcessRecordWithStateDelegate action)
            => new ActionValidator(recordType, action);

        /// <summary>
        ///  Skip all records from this point. Use when you don't care about any further records.
        /// </summary>
        internal static IEmfValidator SkipAll() => SkipAllValidator.Instance;

        /// <summary>
        ///  Skip all output records that the given <paramref name="validator"/> does not validate.
        /// </summary>
        internal static IEmfValidator SkipTo(IEmfValidator validator) => new SkipToValidator(validator);

        /// <summary>
        ///  Skips the next record of the given type.
        /// </summary>
        internal static IEmfValidator SkipType(Gdi32.EMR type) => new SkipTypesValidator(type);

        /// <summary>
        ///  Skip the next record if it matches any of the given types.
        /// </summary>
        internal static IEmfValidator SkipTypes(params Gdi32.EMR[] types) => new SkipTypesValidator(types);

        /// <summary>
        ///  Repeat the given validation <paramref name="count"/> number of times.
        /// </summary>
        internal static IEmfValidator Repeat(IEmfValidator validator, int count) => new RepeatValidator(validator, count);
    }
}
