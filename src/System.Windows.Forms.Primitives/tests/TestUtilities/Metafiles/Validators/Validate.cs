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
        /// <param name="bounds">Optional bounds to validate.</param>
        /// <param name="points">Optional points to validate.</param>
        /// <param name="stateValidators">Optional device context state validation to perform.</param>
        internal static IEmfValidator Polygon16(
            Rectangle? bounds = default,
            Point[]? points = default,
            params IStateValidator[] stateValidators) => new Polygon16Validator(
                bounds,
                points,
                stateValidators);

        /// <param name="bounds">Optional bounds to validate.</param>
        /// <param name="points">Optional points to validate.</param>
        /// <param name="stateValidators">Optional device context state validation to perform.</param>
        internal static IEmfValidator Polyline16(
            Rectangle? bounds = default,
            Point[]? points = default,
            params IStateValidator[] stateValidators) => new Polyline16Validator(
                bounds,
                points,
                stateValidators);

        /// <param name="bounds">Optional bounds to validate.</param>
        /// <param name="polyCount">Optional count of polygons to validate.</param>
        /// <param name="stateValidators">Optional device context state validation to perform.</param>
        internal static IEmfValidator PolyPolygon16(
            Rectangle? bounds = default,
            int? polyCount = default,
            params IStateValidator[] stateValidators) => new PolyPolygon16Validator(
                bounds,
                polyCount,
                stateValidators);

        /// <param name="bounds">Optional bounds to validate.</param>
        /// <param name="polyCount">Optional count of polygons to validate.</param>
        /// <param name="stateValidators">Optional device context state validation to perform.</param>
        internal static IEmfValidator PolyPolyline16(
            Rectangle? bounds = default,
            int? polyCount = default,
            params IStateValidator[] stateValidators) => new PolyPolyline16Validator(
                bounds,
                polyCount,
                stateValidators);

        /// <param name="text">Optional text to validate.</param>
        /// <param name="bounds">Optional bounds to validate.</param>
        /// <param name="stateValidators">Optional device context state validation to perform.</param>
        internal static IEmfValidator TextOut(
            string? text = default,
            Rectangle? bounds = default,
            params IStateValidator[] stateValidators) => new TextOutValidator(
                text,
                bounds,
                stateValidators);

        /// <param name="from">Optional source point to validate.</param>
        /// <param name="to">Optional destination point to validate.</param>
        /// <param name="stateValidators">Optional device context state validation to perform.</param>
        internal static IEmfValidator LineTo(
            EasyPoint? from,
            EasyPoint? to,
            params IStateValidator[] stateValidators) => new LineToValidator(
                from,
                to,
                stateValidators);

        /// <param name="bounds">Optional bounds to validate.</param>
        /// <param name="stateValidators">Optional device context state validation to perform.</param>
        internal static IEmfValidator Rectangle(
            RECT? bounds,
            params IStateValidator[] stateValidators) => new RectangleValidator(
                bounds,
                stateValidators);

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
