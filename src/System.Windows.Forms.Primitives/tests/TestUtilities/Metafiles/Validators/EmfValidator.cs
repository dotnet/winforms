// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Xunit;
using Xunit.Sdk;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal static partial class EmfValidator
    {
        internal static void Validate(
            this EmfScope emf,
            DeviceContextState state,
            params IEmfValidator[] validationSteps)
        {
            if (state is null)
                throw new ArgumentNullException(nameof(state));
            if (validationSteps is null)
                throw new ArgumentNullException(nameof(validationSteps));

            int currentIndex = 0;
            IEmfValidator? currentValidator = validationSteps[currentIndex];

            emf.EnumerateWithState((ref EmfRecord record, DeviceContextState state) =>
                {
                    if (currentValidator?.ShouldValidate(record.Type) ?? false)
                    {
                        try
                        {
                            currentValidator.Validate(ref record, state, out bool complete);

                            if (complete)
                            {
                                // Current validator doesn't want to look at any more records.
                                currentIndex++;
                                currentValidator = currentIndex < validationSteps.Length
                                    ? validationSteps[currentIndex]
                                    : null;
                            }
                        }
                        catch (XunitException e)
                        {
                            throw new WrappedXunitException(
                                $"\nValidator index {currentIndex}: {currentValidator!.GetType().Name} failed",
                                e);
                        }
                    }
                    else
                    {
                        Assert.False(IsRenderingRecord(record.Type), $"Got unexpected {record.Type}");
                    }

                    return true;
                },
                state);

            if (currentValidator != null)
            {
                Assert.False(
                    currentValidator.FailIfIncomplete,
                    $"{currentValidator.GetType().Name} did not receive expected records");
            }
        }

        private static bool IsRenderingRecord(Gdi32.EMR recordType) => recordType switch
        {
            Gdi32.EMR.POLYBEZIER               => true,
            Gdi32.EMR.POLYGON                  => true,
            Gdi32.EMR.POLYLINE                 => true,
            Gdi32.EMR.POLYBEZIERTO             => true,
            Gdi32.EMR.POLYLINETO               => true,
            Gdi32.EMR.POLYPOLYLINE             => true,
            Gdi32.EMR.POLYPOLYGON              => true,
            Gdi32.EMR.SETPIXELV                => true,
            Gdi32.EMR.ANGLEARC                 => true,
            Gdi32.EMR.ELLIPSE                  => true,
            Gdi32.EMR.RECTANGLE                => true,
            Gdi32.EMR.ROUNDRECT                => true,
            Gdi32.EMR.ARC                      => true,
            Gdi32.EMR.CHORD                    => true,
            Gdi32.EMR.PIE                      => true,
            Gdi32.EMR.EXTFLOODFILL             => true,
            Gdi32.EMR.LINETO                   => true,
            Gdi32.EMR.ARCTO                    => true,
            Gdi32.EMR.POLYDRAW                 => true,
            Gdi32.EMR.CLOSEFIGURE              => true,
            Gdi32.EMR.FILLPATH                 => true,
            Gdi32.EMR.STROKEANDFILLPATH        => true,
            Gdi32.EMR.STROKEPATH               => true,
            Gdi32.EMR.FILLRGN                  => true,
            Gdi32.EMR.FRAMERGN                 => true,
            Gdi32.EMR.INVERTRGN                => true,
            Gdi32.EMR.PAINTRGN                 => true,
            Gdi32.EMR.BITBLT                   => true,
            Gdi32.EMR.STRETCHBLT               => true,
            Gdi32.EMR.MASKBLT                  => true,
            Gdi32.EMR.PLGBLT                   => true,
            Gdi32.EMR.SETDIBITSTODEVICE        => true,
            Gdi32.EMR.STRETCHDIBITS            => true,
            Gdi32.EMR.EXTTEXTOUTA              => true,
            Gdi32.EMR.EXTTEXTOUTW              => true,
            Gdi32.EMR.POLYBEZIER16             => true,
            Gdi32.EMR.POLYGON16                => true,
            Gdi32.EMR.POLYLINE16               => true,
            Gdi32.EMR.POLYBEZIERTO16           => true,
            Gdi32.EMR.POLYLINETO16             => true,
            Gdi32.EMR.POLYPOLYLINE16           => true,
            Gdi32.EMR.POLYPOLYGON16            => true,
            Gdi32.EMR.POLYDRAW16               => true,
            Gdi32.EMR.POLYTEXTOUTA             => true,
            Gdi32.EMR.POLYTEXTOUTW             => true,
            Gdi32.EMR.ALPHABLEND               => true,
            Gdi32.EMR.TRANSPARENTBLT           => true,
            Gdi32.EMR.GRADIENTFILL             => true,
            _ => false
        };
    }
}
