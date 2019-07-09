// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools.AutoPME;
using ReflectTools;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiIndexerTests : ReflectBase
    {
        MauiIndexerTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        [STAThread]
        public static void Main(String[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiIndexerTests(args));
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        TParams gp;
        protected override void InitTest(TParams p)
        {
            gp = p;
            base.InitTest(p);
        }

        enum Place
        {
            BeforeFirst, First, BetweenFirstAndLast, Last, AfterLast
        }

        Form f = new Form();
        ArrayList al;

        [Scenario(true)]
        public ScenarioResult ElementDoesntExist(TParams p)
        {
            string str = p.ru.GetString(300);
            CreateCollection(str, 0, 20);
            return new ScenarioResult(f.Controls[str] == null, "Non-null!");
        }

        [Scenario(true)]
        public ScenarioResult ElementExistsSameCase(TParams p)
        {
            string str = p.ru.GetString(300);
            CreateCollection(str, 1, 20);
            return new ScenarioResult(f.Controls[str].Equals(f.Controls[(int)al[0]]), "Wrong value.");
        }

        [Scenario(true)]
        public ScenarioResult ElementExistsDifferentCase(TParams p)
        {
            string str = p.ru.GetString(300);
            CreateCollection(str, 1, 20);
            str = MakeRandomCase(str);
            return new ScenarioResult(f.Controls[str].Equals(f.Controls[(int)al[0]]), "Wrong value.");
        }

        [Scenario(true)]
        public ScenarioResult ElementDiffersByCaseInCurrentCulture(TParams p)
        {
            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            string str = p.ru.GetString(150) + "i" + p.ru.GetString(150);
            CreateCollection(str, 1, 20);
            int idx = str.IndexOf("i");
            str = str.Substring(0, idx) + "i".ToUpper(CultureInfo.CurrentCulture) + str.Substring(idx + 1, str.Length - idx - 1);
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            return new ScenarioResult(f.Controls[str] == null, "Wrong value.");
        }

        [Scenario(true)]
        public ScenarioResult ElementDiffersByCaseInInvariantCulture(TParams p)
        {
            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            string str = p.ru.GetString(150) + "i" + p.ru.GetString(150);
            CreateCollection(str, 1, 20);
            int idx = str.IndexOf("i");
            str = str.Substring(0, idx) + "i".ToUpper(CultureInfo.InvariantCulture) + str.Substring(idx + 1, str.Length - idx - 1);
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            return new ScenarioResult(f.Controls[str].Equals(f.Controls[(int)al[0]]), "Wrong value.");
        }

        [Scenario(true)]
        public ScenarioResult ElementDoesntExistAfterFound(TParams p)
        {
            string str = p.ru.GetString(300);
            CreateCollection(str, 0, 20);
            Control c = f.Controls[f.Controls[0].Name]; //find an element
            return new ScenarioResult(f.Controls[str] == null, "Non-null!");
        }

        [Scenario(true)]
        public ScenarioResult ElementExistsSameCaseAfterFound(TParams p)
        {
            string str = p.ru.GetString(300);
            CreateCollection(str, 2, 20);
            ScenarioResult sr = new ScenarioResult();
            Control c;
            int spot = GetNumber(Place.BeforeFirst);
            if (spot != -1)
            {
                c = f.Controls[f.Controls[spot].Name];
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.First)]), "Wrong value 1", p.log);
            }

            spot = GetNumber(Place.First);
            c = f.Controls[f.Controls[spot].Name];
            sr.IncCounters(f.Controls[str].Equals(f.Controls[spot]), "Wrong value 2", p.log);

            spot = GetNumber(Place.BetweenFirstAndLast);
            if (spot != -1)
            {
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.First)]), "Wrong value 3", p.log);
            }

            //This is the case where we find our cached value, and it's different from what we find by walking...
            spot = GetNumber(Place.Last);
            //To find the second occurrence, make sure the first occurrence doesn't match...
            f.Controls[GetNumber(Place.First)].Name += "1";
            c = f.Controls[f.Controls[spot].Name];
            //...and put it back when we're done
            string s = f.Controls[GetNumber(Place.First)].Name;
            f.Controls[GetNumber(Place.First)].Name = s.Substring(0, s.Length - 1);

            sr.IncCounters(f.Controls[str].Equals(f.Controls[spot]), "Wrong value 4", p.log);

            spot = GetNumber(Place.AfterLast);
            if (spot != -1)
            {
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.Last)]), "Wrong value 5", p.log);
            }

            return sr;
        }

        [Scenario(true)]
        public ScenarioResult ElementExistsDifferentCaseAfterFound(TParams p)
        {
            string str = p.ru.GetString(300);
            CreateCollection(str, 2, 20);
            str = MakeRandomCase(str);
            ScenarioResult sr = new ScenarioResult();
            Control c;
            int spot = GetNumber(Place.BeforeFirst);
            if (spot != -1)
            {
                c = f.Controls[f.Controls[spot].Name];
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.First)]), "Wrong value 1", p.log);
            }

            spot = GetNumber(Place.First);
            c = f.Controls[f.Controls[spot].Name];
            sr.IncCounters(f.Controls[str].Equals(f.Controls[spot]), "Wrong value 2", p.log);

            spot = GetNumber(Place.BetweenFirstAndLast);
            if (spot != -1)
            {
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.First)]), "Wrong value 3", p.log);
            }

            //This is the case where we find our cached value, and it's different from what we find by walking...
            spot = GetNumber(Place.Last);
            //To find the second occurrence, make sure the first occurrence doesn't match...
            f.Controls[GetNumber(Place.First)].Name += "1";
            c = f.Controls[f.Controls[spot].Name];
            //...and put it back when we're done
            string s = f.Controls[GetNumber(Place.First)].Name;
            f.Controls[GetNumber(Place.First)].Name = s.Substring(0, s.Length - 1);

            sr.IncCounters(f.Controls[str].Equals(f.Controls[spot]), "Wrong value 4", p.log);

            spot = GetNumber(Place.AfterLast);
            if (spot != -1)
            {
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.Last)]), "Wrong value 5", p.log);
            }

            return sr;
        }

        [Scenario(true)]
        public ScenarioResult ElementDiffersByCaseInCurrentCultureAfterFound(TParams p)
        {
            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            string str = p.ru.GetString(150) + "i" + p.ru.GetString(150);
            CreateCollection(str, 2, 20);
            int idx = str.IndexOf("i");
            str = str.Substring(0, idx) + "i".ToUpper(CultureInfo.CurrentCulture) + str.Substring(idx + 1, str.Length - idx - 1);
            ScenarioResult sr = new ScenarioResult();
            Control c;
            int spot = GetNumber(Place.BeforeFirst);
            if (spot != -1)
            {
                c = f.Controls[f.Controls[spot].Name];
                sr.IncCounters(f.Controls[str] == null, "Wrong value 1", p.log);
            }

            spot = GetNumber(Place.First);
            c = f.Controls[f.Controls[spot].Name];
            sr.IncCounters(f.Controls[str] == null, "Wrong value 2", p.log);

            spot = GetNumber(Place.BetweenFirstAndLast);
            if (spot != -1)
            {
                sr.IncCounters(f.Controls[str] == null, "Wrong value 3", p.log);
            }

            //This is the case where we find our cached value, and it's different from what we find by walking...
            spot = GetNumber(Place.Last);
            //To find the second occurrence, make sure the first occurrence doesn't match...
            f.Controls[GetNumber(Place.First)].Name += "1";
            c = f.Controls[f.Controls[spot].Name];
            //...and put it back when we're done
            string s = f.Controls[GetNumber(Place.First)].Name;
            f.Controls[GetNumber(Place.First)].Name = s.Substring(0, s.Length - 1);

            sr.IncCounters(f.Controls[str] == null, "Wrong value 4", p.log);

            spot = GetNumber(Place.AfterLast);
            if (spot != -1)
            {
                sr.IncCounters(f.Controls[str] == null, "Wrong value 5", p.log);
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            return sr;
        }

        [Scenario(true)]
        public ScenarioResult ElementDiffersByCaseInInvariantCultureAfterFound(TParams p)
        {
            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            string str = p.ru.GetString(150) + "i" + p.ru.GetString(150);
            CreateCollection(str, 1, 20);
            int idx = str.IndexOf("i");
            str = str.Substring(0, idx) + "i".ToUpper(CultureInfo.InvariantCulture) + str.Substring(idx + 1, str.Length - idx - 1);
            ScenarioResult sr = new ScenarioResult();
            Control c;
            int spot = GetNumber(Place.BeforeFirst);
            if (spot != -1)
            {
                c = f.Controls[f.Controls[spot].Name];
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.First)]), "Wrong value 1", p.log);
            }

            spot = GetNumber(Place.First);
            c = f.Controls[f.Controls[spot].Name];
            sr.IncCounters(f.Controls[str].Equals(f.Controls[spot]), "Wrong value 2", p.log);

            spot = GetNumber(Place.BetweenFirstAndLast);
            if (spot != -1)
            {
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.First)]), "Wrong value 3", p.log);
            }

            //This is the case where we find our cached value, and it's different from what we find by walking...
            spot = GetNumber(Place.Last);
            //To find the second occurrence, make sure the first occurrence doesn't match...
            f.Controls[GetNumber(Place.First)].Name += "1";
            c = f.Controls[f.Controls[spot].Name];
            //...and put it back when we're done
            string s = f.Controls[GetNumber(Place.First)].Name;
            f.Controls[GetNumber(Place.First)].Name = s.Substring(0, s.Length - 1);

            sr.IncCounters(f.Controls[str].Equals(f.Controls[spot]), "Wrong value 4", p.log);

            spot = GetNumber(Place.AfterLast);
            if (spot != -1)
            {
                sr.IncCounters(f.Controls[str].Equals(f.Controls[GetNumber(Place.Last)]), "Wrong value 5", p.log);
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            return sr;
        }

        [Scenario(true)]
        public ScenarioResult LookForNull(TParams p)
        {
            string str = p.ru.GetString(300);
            CreateCollection(str, 0, 20);
            return new ScenarioResult(f.Controls[null] == null, "Non-null!");
        }

        [Scenario(true)]
        public ScenarioResult EmptyStringDoesntExist(TParams p)
        {
            string str = "";
            CreateCollection(str, 0, 20);
            return new ScenarioResult(f.Controls[str] == null, "Non-null!");
        }

        [Scenario(true)]
        public ScenarioResult EmptyStringExists(TParams p)
        {
            string str = "";
            //we always return null, even when "" exists
            CreateCollection(str, 1, 20);
            return new ScenarioResult(f.Controls[str] == null, "Non-null!");
        }

        [Scenario(true)]
        public ScenarioResult LongStringDoesntExist(TParams p)
        {
            string str = p.ru.GetString(66000);
            CreateCollection(str, 0, 20);
            return new ScenarioResult(f.Controls[str] == null, "Non-null!");
        }

        [Scenario(true)]
        public ScenarioResult LongStringExists(TParams p)
        {
            string str = p.ru.GetString(66000);
            CreateCollection(str, 1, 20);
            return new ScenarioResult(f.Controls[str].Equals(f.Controls[(int)al[0]]), "Wrong value.");
        }

        [Scenario(true)]
        public ScenarioResult StringWithProblemChars(TParams p)
        {
            string str = "";
            while (str == "")
                str = p.ru.GetProbCharString(500, false, false);
            CreateCollection(str, 1, 20);
            return new ScenarioResult(f.Controls[str].Equals(f.Controls[(int)al[0]]), "Wrong value.");
        }

        //HELPER FUNCTIONS
        int GetNumber(Place p)
        {
            switch (p)
            {
                case Place.BeforeFirst:
                    //if the first matching item is at the first spot, there's nothing before the first match
                    if ((int)al[0] == 0)
                        return -1;
                    else
                        return gp.ru.GetRange(0, (int)al[0] - 1);
                case Place.First:
                    return (int)al[0];
                case Place.BetweenFirstAndLast:
                    //if the first and last matches are next to each other, there's nothing between them
                    if ((int)al[0] <= (int)al[al.Count - 1] - 1)
                        return -1;
                    else
                        return gp.ru.GetRange((int)al[0] + 1, (int)al[al.Count - 1] - 1);
                case Place.Last:
                    return (int)al[al.Count - 1];
                case Place.AfterLast:
                default:
                    //if the last matching item is at the last spot, there's nothing after the last match
                    if ((int)al[al.Count - 1] == f.Controls.Count - 1)
                        return -1;
                    return gp.ru.GetRange((int)al[al.Count - 1] + 1, f.Controls.Count - (int)al[al.Count - 1] - 1);
            }
        }

        string MakeRandomCase(string s)
        {
            string ret = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (gp.ru.GetBoolean())
                    ret += s.Substring(i, 1).ToUpper(CultureInfo.InvariantCulture);
                else
                    ret += s.Substring(i, 1).ToLower(CultureInfo.InvariantCulture);
            }
            return ret;
        }

        void CreateCollection(string stringToFind, int numberOfOccurrences, int totalItems)
        {
            al = new ArrayList();
            int newValue;
            string str;
            for (int i = 0; i < numberOfOccurrences; i++)
            {
                do
                {
                    newValue = gp.ru.GetRange(0, totalItems - 1);
                } while (al.Contains(newValue));
                al.Add(newValue);
            }
            al.Sort(); //Make life simple for First, Last, etc.
            f.Controls.Clear();
            for (int i = 0; i < totalItems; i++)
            {
                Control ctrl = null;
                int rnd = gp.ru.GetRange(0, 3);
                switch (rnd)
                {
                    case 0:
                        ctrl = new Button();
                        break;
                    case 1:
                        ctrl = new TextBox();
                        break;
                    case 2:
                        ctrl = new DomainUpDown();
                        break;
                    default:
                        ctrl = new Panel();
                        break;
                }
                if (al.Contains(i))
                {
                    ctrl.Name = stringToFind;
                }
                else
                {
                    //Loop here on the off chance that you find a string that matches what you're looking for
                    do
                    {
                        str = gp.ru.GetString(300);
                    } while (str.ToUpper(CultureInfo.InvariantCulture) == stringToFind.ToUpper(CultureInfo.InvariantCulture));
                    ctrl.Name = str;
                }
                f.Controls.Add(ctrl);
            }
        }
    }
}
