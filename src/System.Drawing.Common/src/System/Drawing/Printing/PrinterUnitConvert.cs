// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
///  Specifies a series of conversion methods that are useful when interoperating with the raw Win32 printing API.
/// </summary>
public sealed class PrinterUnitConvert
{
    private PrinterUnitConvert()
    {
    }

    /// <summary>
    ///  Converts the value, in fromUnit units, to toUnit units.
    /// </summary>
    public static double Convert(double value, PrinterUnit fromUnit, PrinterUnit toUnit)
    {
        double fromUnitsPerDisplay = UnitsPerDisplay(fromUnit);
        double toUnitsPerDisplay = UnitsPerDisplay(toUnit);
        return value * toUnitsPerDisplay / fromUnitsPerDisplay;
    }

    /// <summary>
    ///  Converts the value, in fromUnit units, to toUnit units.
    /// </summary>
    public static int Convert(int value, PrinterUnit fromUnit, PrinterUnit toUnit) =>
        (int)Math.Round(Convert((double)value, fromUnit, toUnit));

    /// <summary>
    ///  Converts the value, in fromUnit units, to toUnit units.
    /// </summary>
    public static Point Convert(Point value, PrinterUnit fromUnit, PrinterUnit toUnit) =>
        new(Convert(value.X, fromUnit, toUnit), Convert(value.Y, fromUnit, toUnit));

    /// <summary>
    ///  Converts the value, in fromUnit units, to toUnit units.
    /// </summary>
    public static Size Convert(Size value, PrinterUnit fromUnit, PrinterUnit toUnit) =>
        new(Convert(value.Width, fromUnit, toUnit), Convert(value.Height, fromUnit, toUnit));

    /// <summary>
    ///  Converts the value, in fromUnit units, to toUnit units.
    /// </summary>
    public static Rectangle Convert(Rectangle value, PrinterUnit fromUnit, PrinterUnit toUnit) => new(
        Convert(value.X, fromUnit, toUnit),
        Convert(value.Y, fromUnit, toUnit),
        Convert(value.Width, fromUnit, toUnit),
        Convert(value.Height, fromUnit, toUnit));

    /// <summary>
    ///  Converts the value, in fromUnit units, to toUnit units.
    /// </summary>
    public static Margins Convert(Margins value, PrinterUnit fromUnit, PrinterUnit toUnit) => new()
    {
        DoubleLeft = Convert(value.DoubleLeft, fromUnit, toUnit),
        DoubleRight = Convert(value.DoubleRight, fromUnit, toUnit),
        DoubleTop = Convert(value.DoubleTop, fromUnit, toUnit),
        DoubleBottom = Convert(value.DoubleBottom, fromUnit, toUnit)
    };

    private static double UnitsPerDisplay(PrinterUnit unit)
    {
        double result;
        switch (unit)
        {
            case PrinterUnit.Display:
                result = 1.0;
                break;
            case PrinterUnit.ThousandthsOfAnInch:
                result = 10.0;
                break;
            case PrinterUnit.HundredthsOfAMillimeter:
                result = 25.4;
                break;
            case PrinterUnit.TenthsOfAMillimeter:
                result = 2.54;
                break;
            default:
                Debug.Fail($"Unknown PrinterUnit {unit}");
                result = 1.0;
                break;
        }

        return result;
    }
}
