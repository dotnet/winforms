﻿// Licensed to the .NET Foundation under one or more agreements.
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
        private PrinterUnit _fromUnit;
        private PrinterUnit _toUnit;

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
            double doubleValue;
            double doubleResultValue;

            ScenarioResult result = new ScenarioResult(true);

            doubleValue = p.ru.GetDouble();
            _fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            _toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + doubleValue.ToString());
            p.log.WriteLine("fromUnit=" + _fromUnit.ToString());
            p.log.WriteLine("toUnit=" + _toUnit.ToString());

            doubleResultValue = PrinterUnitConvert.Convert(doubleValue, _fromUnit, _toUnit);
            p.log.WriteLine("resultValue=" + doubleResultValue.ToString());

            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Int_Value(TParams p)
        {
            int intValue;
            int intResultValue;

            ScenarioResult result = new ScenarioResult(true);

            intValue = p.ru.GetInt();
            _fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            _toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + intValue.ToString());
            p.log.WriteLine("fromUnit=" + _fromUnit.ToString());
            p.log.WriteLine("toUnit=" + _toUnit.ToString());

            try
            {
                intResultValue = PrinterUnitConvert.Convert(intValue, _fromUnit, _toUnit);
                p.log.WriteLine("resultValue=" + intResultValue.ToString());
            }
            catch (Exception)
            {
                intResultValue = PrinterUnitConvert.Convert(intValue, _toUnit, _fromUnit);
                p.log.WriteLine("resultValue=" + intResultValue.ToString());
            }
            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Point_Value(TParams p)
        {
            Point pointValue;
            Point pointResultValue;

            ScenarioResult result = new ScenarioResult(true);

            pointValue = p.ru.GetPoint();
            _fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            _toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + pointValue.ToString());
            p.log.WriteLine("fromUnit=" + _fromUnit.ToString());
            p.log.WriteLine("toUnit=" + _toUnit.ToString());

            pointResultValue = PrinterUnitConvert.Convert(pointValue, _fromUnit, _toUnit);
            p.log.WriteLine("resultValue=" + pointResultValue.ToString());

            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Size_Value(TParams p)
        {
            Size sizeValue;
            Size sizeResultValue;

            ScenarioResult result = new ScenarioResult(true);

            sizeValue = p.ru.GetSize();
             _fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            _toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + sizeValue.ToString());
            p.log.WriteLine("fromUnit=" + _fromUnit.ToString());
            p.log.WriteLine("toUnit=" + _toUnit.ToString());

            try
            {
                sizeResultValue = PrinterUnitConvert.Convert(sizeValue, _fromUnit, _toUnit);
                p.log.WriteLine("resultValue=" + sizeResultValue.ToString());
            }
            catch (Exception)
            {
                sizeResultValue = PrinterUnitConvert.Convert(sizeValue, _toUnit, _fromUnit);
                p.log.WriteLine("resultValue=" + sizeResultValue.ToString());
            }

            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Rectangle_Value(TParams p)
        {
            Rectangle rectangleValue;
            Rectangle rectangleResultValue;

            ScenarioResult result = new ScenarioResult(true);

            rectangleValue = p.ru.GetRectangle();
            _fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            _toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + rectangleValue.ToString());
            p.log.WriteLine("fromUnit=" + _fromUnit.ToString());
            p.log.WriteLine("toUnit=" + _toUnit.ToString());

            rectangleResultValue = PrinterUnitConvert.Convert(rectangleValue, _fromUnit, _toUnit);
            p.log.WriteLine("resultValue=" + rectangleResultValue.ToString());

            return result;
        }

        [Scenario(true)]
        public ScenarioResult Convert_Margins_Value(TParams p)
        {
            Margins marginsValue;
            Margins marginsResultValue;

            ScenarioResult result = new ScenarioResult(true);

            marginsValue = new Margins(p.ru.GetRange(0, char.MaxValue), p.ru.GetRange(0, char.MaxValue), p.ru.GetRange(0, char.MaxValue), p.ru.GetRange(0, char.MaxValue));
            _fromUnit = p.ru.GetEnumValue<PrinterUnit>();
            _toUnit = p.ru.GetEnumValue<PrinterUnit>();

            p.log.WriteLine("value=" + marginsValue.ToString());
            p.log.WriteLine("fromUnit=" + _fromUnit.ToString());
            p.log.WriteLine("toUnit=" + _toUnit.ToString());

            marginsResultValue = PrinterUnitConvert.Convert(marginsValue, _fromUnit, _toUnit);
            p.log.WriteLine("resultValue=" + marginsResultValue.ToString());

            return result;
        }
    }
}

