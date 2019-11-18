// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Printing;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Log;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiPrintUnitConvertTests : XObject
    {
        private double _doubleValue;
        private int _intValue;
        private Point _pointValue;
        private Size _sizeValue;
        private Rectangle _rectangleValue;
        private Margins _marginsValue;
        private double _doubleResultValue;
        private int _intResultValue;
        private Point _pointResultValue;
        private Size _sizeResultValue;
        private Rectangle _rectangleResultValue;
        private Margins _marginsResultValue;

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiPrintUnitConvertTests(args));
        }

        public MauiPrintUnitConvertTests(string[] args) : base(args)
        { 
            this.BringToForeground(); 
        }

        protected override Type Class
        {
            get
            { 
                { 
                    return typeof(PrinterUnitConvert);
                } 
            }
        }

        protected override Object CreateObject(TParams p)
        {
            return null;
        }

        protected PrinterUnitConvert GetPrinterUnitConvert(TParams p)
        {
            if (p.target is PrinterUnitConvert) 
            { 
                return (PrinterUnitConvert)p.target;
            }
            else
            {
                p.log.WriteLine("target !instanceof System.Drawing.Printing.PrinterUnitConvert.");
                return null;
            }
        }

        [Scenario(true)]
        public ScenarioResult Convert_Double_value(TParams p)
        {
            ScenarioResult result = new ScenarioResult(true);

            _doubleValue = p.ru.GetDouble();
            PrinterUnit test_fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            PrinterUnit test_toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + _doubleValue.ToString());
            p.log.WriteLine("fromUnit=" + test_fromUnit.ToString());
            p.log.WriteLine("toUnit=" + test_toUnit.ToString());

            _doubleResultValue = PrinterUnitConvert.Convert(_doubleValue, test_fromUnit, test_toUnit);
            p.log.WriteLine("resultValue=" + _doubleResultValue.ToString());

            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Int_Value(TParams p)
        {
            ScenarioResult result = new ScenarioResult(true);

            _intValue = p.ru.GetInt();
            PrinterUnit test_fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            PrinterUnit test_toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + _intValue.ToString());
            p.log.WriteLine("fromUnit=" + test_fromUnit.ToString());
            p.log.WriteLine("toUnit=" + test_toUnit.ToString());

            try
            {
                _intResultValue = PrinterUnitConvert.Convert(_intValue, test_fromUnit, test_toUnit);
                p.log.WriteLine("resultValue=" + _intResultValue.ToString());
            }
            catch (Exception)
            {
                _intResultValue = PrinterUnitConvert.Convert(_intValue, test_toUnit, test_fromUnit);
                p.log.WriteLine("resultValue=" + _intResultValue.ToString());
            }
            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Point_Value(TParams p)
        {
            ScenarioResult result = new ScenarioResult(true);

            _pointValue = p.ru.GetPoint();
            PrinterUnit test_fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            PrinterUnit test_toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + _pointValue.ToString());
            p.log.WriteLine("fromUnit=" + test_fromUnit.ToString());
            p.log.WriteLine("toUnit=" + test_toUnit.ToString());

            _pointResultValue = PrinterUnitConvert.Convert(_pointValue, test_fromUnit, test_toUnit);
            p.log.WriteLine("resultValue=" + _pointResultValue.ToString());

            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Size_Value(TParams p)
        {
            ScenarioResult result = new ScenarioResult(true);

            _sizeValue = p.ru.GetSize();
            PrinterUnit test_fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            PrinterUnit test_toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + _sizeValue.ToString());
            p.log.WriteLine("fromUnit=" + test_fromUnit.ToString());
            p.log.WriteLine("toUnit=" + test_toUnit.ToString());

            try
            {
                _sizeResultValue = PrinterUnitConvert.Convert(_sizeValue, test_fromUnit, test_toUnit);
                p.log.WriteLine("resultValue=" + _sizeResultValue.ToString());
            }
            catch (Exception)
            {
                _sizeResultValue = PrinterUnitConvert.Convert(_sizeValue, test_toUnit, test_fromUnit);
                p.log.WriteLine("resultValue=" + _sizeResultValue.ToString());
            }

            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Rectangle_Value(TParams p)
        {
            ScenarioResult result = new ScenarioResult(true);

            _rectangleValue = p.ru.GetRectangle();
            PrinterUnit test_fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            PrinterUnit test_toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + _rectangleValue.ToString());
            p.log.WriteLine("fromUnit=" + test_fromUnit.ToString());
            p.log.WriteLine("toUnit=" + test_toUnit.ToString());

            _rectangleResultValue = PrinterUnitConvert.Convert(_rectangleValue, test_fromUnit, test_toUnit);
            p.log.WriteLine("resultValue=" + _rectangleResultValue.ToString());

            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Margins_Value(TParams p)
        {
            ScenarioResult result = new ScenarioResult(true);

            _marginsValue = new Margins(p.ru.GetRange(0, char.MaxValue), p.ru.GetRange(0, char.MaxValue), p.ru.GetRange(0, char.MaxValue), p.ru.GetRange(0, char.MaxValue));
            PrinterUnit test_fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            PrinterUnit test_toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + _marginsValue.ToString());
            p.log.WriteLine("fromUnit=" + test_fromUnit.ToString());
            p.log.WriteLine("toUnit=" + test_toUnit.ToString());

            _marginsResultValue = PrinterUnitConvert.Convert(_marginsValue, test_fromUnit, test_toUnit);
            p.log.WriteLine("resultValue=" + _marginsResultValue.ToString());

            return result;
        }
    }
}

