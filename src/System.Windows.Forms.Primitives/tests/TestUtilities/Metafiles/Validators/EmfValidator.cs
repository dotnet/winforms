// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Xunit.Sdk;

namespace System.Windows.Forms.Metafiles;

internal static partial class EmfValidator
{
    internal static void Validate(
        this EmfScope emf,
        DeviceContextState state,
        params IEmfValidator[] validationSteps)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(validationSteps);

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
                    catch (XunitException ex)
                    {
                        throw new WrappedXunitException(
                            $"\nValidator index {currentIndex}: {currentValidator!.GetType().Name} failed\n\n{emf.RecordsToString()}",
                            ex);
                    }
                }
                else
                {
                    Assert.False(IsRenderingRecord(record.Type), $"Got unexpected {record.Type}\n\n{emf.RecordsToString()}");
                }

                return true;
            },
            state);

        if (currentValidator is not null)
        {
            Assert.False(
                currentValidator.FailIfIncomplete,
                $"{currentValidator.GetType().Name} did not receive expected records\n\n{emf.RecordsToString()}");
        }
    }

    private static bool IsRenderingRecord(ENHANCED_METAFILE_RECORD_TYPE recordType)
        => recordType switch
        {
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYBEZIER => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINE => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYBEZIERTO => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINETO => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYLINE => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_SETPIXELV => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_ANGLEARC => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_ELLIPSE => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_RECTANGLE => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_ROUNDRECT => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_ARC => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_CHORD => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_PIE => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTFLOODFILL => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_LINETO => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_ARCTO => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYDRAW => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_CLOSEFIGURE => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_FILLPATH => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_STROKEANDFILLPATH => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_STROKEPATH => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_FILLRGN => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_FRAMERGN => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_INVERTRGN => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_PAINTRGN => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_BITBLT => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHBLT => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_MASKBLT => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_PLGBLT => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_SETDIBITSTODEVICE => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTTEXTOUTA => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTTEXTOUTW => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYBEZIER16 => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16 => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINE16 => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYBEZIERTO16 => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINETO16 => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYLINE16 => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16 => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYDRAW16 => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYTEXTOUTA => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYTEXTOUTW => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_ALPHABLEND => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_TRANSPARENTBLT => true,
            ENHANCED_METAFILE_RECORD_TYPE.EMR_GRADIENTFILL => true,
            _ => false
        };
}
