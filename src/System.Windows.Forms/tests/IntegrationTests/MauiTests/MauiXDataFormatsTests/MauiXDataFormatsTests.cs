// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using WFCTestLib.Log;
using ReflectTools;
using ReflectTools.AutoPME;
using System;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiXDataFormatsTests : XObject
    {
        public MauiXDataFormatsTests(String[] args) : base(args)
        {
            this.BringToForeground();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiXDataFormatsTests(args));
        }

        protected override Type Class
        {
            get { return typeof(DataFormats); }
        }

        protected override Object CreateObject(TParams p)
        {
            return null;
        }

        //========================================
        // Test Methods
        //========================================
        [Scenario(true)]
        public ScenarioResult Get_Format_By_String(TParams p)
        {

            DataFormats.Format formatObj1;

            String strFormat = Get_DataFormat_String(p);
            p.log.WriteLine("Format type : " + strFormat);
            formatObj1 = DataFormats.GetFormat(strFormat);

            return new ScenarioResult(Verify_Format_Type(formatObj1.Name, strFormat), "Unexpected format type : " + formatObj1.Name);
        }

        [Scenario(true)]
        public ScenarioResult Get_Format_By_Id(TParams p)
        {
            DataFormats.Format formatObj1;

            int iFormat = Get_DataFormat_Int(p);
            p.log.WriteLine("Format type number : " + iFormat);
            formatObj1 = DataFormats.GetFormat(iFormat);

            return new ScenarioResult(formatObj1.Id == iFormat, "Unexpected format type : " + formatObj1.Id);
        }

        private String Get_DataFormat_String(TParams p)
        {
            String[] strFormats = new String[] { "UnicodeText", "Text", "Bitmap", "MetafilePict", "EnhancedMetafile", "DIF",
                                     "TIFF", "OEMText", "DIB", "Palette", "PenData", "RIFF", "WaveAudio", "SymbolicLink",
                                     "FileDrop", "Locale","Test1", "Test2", "Test3" };

            int iRandNum = p.ru.GetRange(0, strFormats.Length - 1);
            return strFormats[iRandNum];
        }

        private int Get_DataFormat_Int(TParams p)
        {
            int[] iFormatNum = new int[] {  win.CF_UNICODETEXT, win.CF_TEXT, win.CF_BITMAP, win.CF_METAFILEPICT, win.CF_ENHMETAFILE,
                         win.CF_DIF, win.CF_TIFF, win.CF_OEMTEXT, win.CF_DIB, win.CF_PALETTE, win.CF_PENDATA, win.CF_RIFF,
                         win.CF_WAVE, win.CF_SYLK, win.CF_HDROP, win.CF_LOCALE, 100, 200, 300 };

            int iRandNum = p.ru.GetRange(0, iFormatNum.Length - 1);
            return iFormatNum[iRandNum];
        }

        private Boolean Verify_Format_Type(String strExp, String strOrg)
        {
            if (String.Compare(strExp.Substring(strExp.IndexOf(":") + 1), strOrg, true) == 0)
                return true;
            else
                return false;
        }
    }
}
