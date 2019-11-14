// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiLabelTests : ReflectBase
    {
        private readonly Label _lbl;

        public MauiLabelTests(String[] args) : base(args)
        {
            this.BringToForeground();
            _lbl = new Label();
            Controls.Add(_lbl);
        }

        public static void Main(String[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiLabelTests(args));
        }

        [Scenario(true)]
        public ScenarioResult AutoSize_Changes_Size_When_True(TParams p)
        {
            p.log.WriteLine("Make Sure label size is changed when the text changes and autosize is true");
            _lbl.AutoSize = true;

            _lbl.Size = new Size(10, 10);
            _lbl.Text = "Hello";
            Size oldSize = _lbl.Size;
            _lbl.Text = "Say Hello";
            Size newSize = _lbl.Size;
            return new ScenarioResult(newSize != oldSize);
        }

        [Scenario(true)]
        public ScenarioResult AutoSize_Does_Not_Change_Size_When_False(TParams p)
        {
            p.log.WriteLine("Make Sure label size is not changed when the text changes and autosize is false");
            _lbl.AutoSize = false;

            _lbl.Size = new Size(10, 10);
            _lbl.Text = "Hello";
            Size oldSize = _lbl.Size;
            _lbl.Text = "Say Hello";
            Size newSize = _lbl.Size;
            return new ScenarioResult(newSize == oldSize);
        }

        [Scenario(true)]
        public ScenarioResult Set_TextAlign_With_Enum_Values(TParams p)
        {
            p.log.WriteLine("Make Sure I can set all ContentAlignment via integral");
            _lbl.AutoSize = true;

            foreach (int value in Enum.GetValues(typeof(ContentAlignment)))
            {
                try
                {
                    _lbl.TextAlign = (ContentAlignment)value;
                }
                catch
                {
                    return new ScenarioResult(false, $"Failed to set ContentAlignment: {value}");
                }
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult Set_TextAlign_With_Enum_Names(TParams p)
        {
            p.log.WriteLine("Make Sure I can set all ContentAlignment via literals");
            _lbl.AutoSize = true;

            foreach (string value in Enum.GetNames(typeof(ContentAlignment)))
            {
                try
                {
                    _lbl.TextAlign = (ContentAlignment)Enum.Parse(typeof(ContentAlignment), value);
                }
                catch
                {
                    return new ScenarioResult(false, $"Failed to set ContentAlignment: {value}");
                }
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult Set_TextAlign_With_Invalid_Enum_Value(TParams p)
        {
            p.log.WriteLine("Make Sure I get Exceptions on -1 Align Enums via integral");
            _lbl.AutoSize = true;

            try
            {
                _lbl.TextAlign = (ContentAlignment)(-1);
            }
            catch (ArgumentException e)
            {
                p.log.WriteLine("exception was caught: " + e.Message);
                return ScenarioResult.Pass;
            }
            catch (Exception e)
            {
                p.log.WriteLine("exception was caught: " + e.Message);
                return new ScenarioResult(false, "FAILED: wrong exception was thrown", p.log);
            }
            return new ScenarioResult(false, "Failed to throw exception on -1 text alignment", p.log);
        }
    }
}
