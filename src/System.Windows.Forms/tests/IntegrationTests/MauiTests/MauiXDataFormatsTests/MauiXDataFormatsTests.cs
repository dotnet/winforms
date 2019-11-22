// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Log;

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

        protected override Type Class => typeof(DataFormats);

        protected override Object CreateObject(TParams p) => null;

        [Scenario(true)]
        public ScenarioResult Get_Format_By_String(TParams p)
        {
            DataFormats.Format formatObj;

            string strFormat = Get_DataFormat_String(p);
            p.log.WriteLine("Format type : " + strFormat);
            formatObj = DataFormats.GetFormat(strFormat);

            return new ScenarioResult(Verify_Format_Type(formatObj.Name, strFormat), "Unexpected format type : " + formatObj.Name);
        }

        [Scenario(true)]
        public ScenarioResult Get_Format_By_Id(TParams p)
        {
            DataFormats.Format formatObj;

            int iFormat = Get_DataFormat_Int(p);
            p.log.WriteLine("Format type number : " + iFormat);
            formatObj = DataFormats.GetFormat(iFormat);

            return new ScenarioResult(formatObj.Id == iFormat, "Unexpected format type : " + formatObj.Id);
        }

        private string Get_DataFormat_String(TParams p)
        {
            string[] strFormats = new string[] { "UnicodeText", "Text", "Bitmap", "MetafilePict", "EnhancedMetafile", "DIF",
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

        private Boolean Verify_Format_Type(string strExp, string strOrg)
        {
            if (String.Compare(strExp.Substring(strExp.IndexOf(":") + 1), strOrg, true) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
