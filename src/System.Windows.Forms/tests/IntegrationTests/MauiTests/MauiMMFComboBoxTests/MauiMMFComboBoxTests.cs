// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using Maui.Core;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiMMFComboBoxTests : ReflectBase
    {
        const int MAX_ITEM_COUNT = 50;  // TODO: Move back up to some silly-large number once test is running smoothly
        const int MAX_ITEM_NAME_LENGTH = 200;
        const int MAX_DROPDOWN_WIDTH = 300;
        const int MAX_DROPDOWN_HEIGHT = 300;
        const int MAX_SUGGESTIONS_TO_CHECK = 500;
        const int FILES_TO_CHECK = 15;  // used in AutoCompleteSource.FileSystem test
        const int SLEEP_TIME = 200;  //t-t: CHANGE IF NECESSARY

        // note: the URL string is alphabetically early so AllUrl test doesn't have to search far
        string URL_FOR_AUTOCOMPLETION = Utilities.TestFileShareUrl;

        int textUpdateCount = 0;
        int dropDownClosedCount = 0;

        string newTextForSuggestion = "";

        #region Testcase setup

        ComboBox cb;

        public MauiMMFComboBoxTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiMMFComboBoxTests(args));
        }

        protected override void InitTest(TParams p)
        {
            p.ru.Log = p.log;
            //p.ru.LogRandomValues = true;
            base.InitTest(p);
            cb = new ComboBox();

            // add items
            int itemsToAdd = p.ru.GetRange(0, MAX_ITEM_COUNT);

            for (int i = 0; i < itemsToAdd; i++)
            {
                cb.Items.Add(p.ru.GetString(MAX_ITEM_NAME_LENGTH));
            }

            this.Controls.Add(cb);
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios

        //@ TextUpdate event fires between text formatting and text display
        [Scenario(true)]
        public ScenarioResult TextUpdateEvent(TParams p)
        {

            SafeMethods.HideSecurityBubble(this);

            ScenarioResult sr = new ScenarioResult();

            cb.TextUpdate += new EventHandler(cb_TextUpdate);
            p.log.WriteLine("DropDownStyle: " + cb.DropDownStyle.ToString());

            int currentTextUpdateCount = textUpdateCount;

            SafeMethods.Focus(cb);
            Application.DoEvents();
            Thread.Sleep(2000);
            Application.DoEvents();

            // send single keystroke
            string st = p.ru.GetValidString(1);
            p.log.WriteLine("Sending string " + st);
            sendKeysAndWait(st);
            //sendKeysAndWait(p.ru.GetValidString(1));
            Thread.Sleep(2000);
            Application.DoEvents();

            // 1. Ensure that TextUpdate fired only once
            sr.IncCounters(++currentTextUpdateCount == textUpdateCount, "Expected " + currentTextUpdateCount + " but got " + textUpdateCount, p.log);

            sendKeysAndWait("{ENTER}");
            cb.TextUpdate -= cb_TextUpdate;

            return sr;
        }

        //@ DropDownClosed event fires when dropdown closes with different closing methods
        [Scenario(true)]
        public ScenarioResult DropDownClosedEvent(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            cb.DropDownClosed += new EventHandler(cb_DropDownClosed);

            // set up point to click (in dropdown arrow)
            int x, y;

            y = cb.Bounds.Top + (cb.Height / 2);
            if (cb.RightToLeft == RightToLeft.Yes)
                x = cb.Bounds.Left + (cb.Height / 2);
            else
                x = cb.Bounds.Right - (cb.Height / 2);

            Point point = new Point(x, y);

            point = cb.PointToScreen(point);

            int currentDropDownClosedCount = dropDownClosedCount;

            // 1. Drop down via click on arrow, click on arrow again should close box
            sendLeftClickAndWait(point);
            sendLeftClickAndWait(point);
            sr.IncCounters(++currentDropDownClosedCount == dropDownClosedCount, "FAIL: did not fire DropDownClosed event exactly once when closing box with click on arrow", p.log);

            // 2. Drop down via click on arrow, sending an ENTER should close box
            sendLeftClickAndWait(point);
            sendKeysAndWait("{ENTER}");
            sr.IncCounters(++currentDropDownClosedCount == dropDownClosedCount, "FAIL: did not fire DropDownClosed event exactly once when closing box with ENTER", p.log);

            // 3. Drop down via click on arrow, sending an ESC should close box
            sendLeftClickAndWait(point);
            sendKeysAndWait("{ESC}");
            sr.IncCounters(++currentDropDownClosedCount == dropDownClosedCount, "FAIL: did not fire DropDownClosed event exactly once when closing box with ESC", p.log);

            // 4. Drop down via click on arrow, clicking literally anywhere should close it
            // Note, this will fail if random point is on the scroll bar of the drop down.
            // Then the drop down will not close at all.
            sendLeftClickAndWait(point);
            p.log.WriteLine("DroppedDown: " + cb.DroppedDown + " Count " + dropDownClosedCount);
            // click anywhere *on form* (don't want to lose focus)--even on close or minimize is ok
            Rectangle formBounds = this.Bounds;   // don't have to do RectangleToScreen, already in screen coords

            Point pnt = p.ru.GetPoint(formBounds);
            p.log.WriteLine("This form bounds: " + formBounds);
            p.log.WriteLine("Clicking at: " + pnt);
            p.log.WriteLine("cb bounds: " + this.RectangleToScreen(cb.Bounds));

            sendLeftClickAndWait(pnt);
            p.log.WriteLine("Current was: " + currentDropDownClosedCount + " after click event count become "
                + dropDownClosedCount);
            p.log.WriteLine("After random click: DroppedDown: " + cb.DroppedDown + " Count " + dropDownClosedCount);
            sr.IncCounters(++currentDropDownClosedCount == dropDownClosedCount, "FAIL: did not fire DropDownClosed event exactly once when closing box with random click on form", p.log);
            cb.DropDownClosed -= cb_DropDownClosed;
            //Utilities.ActiveFreeze("after dd method");

            // move mouse away from suggestion menu's area--screws up tests that send down-arrow keystrokes
            Point nearBottomRightCorner = new Point(this.Bounds.Right - 5, this.Bounds.Bottom - 5);

            sendLeftClickAndWait(nearBottomRightCorner);
            return sr;
        }

        //	[Scenario("AutoComplete modes work properly: None, Append, Suggest, SuggestAppend")]
        //	public ScenarioResult AutoCompleteModes(TParams p)
        //	{
        //		ScenarioResult sr = new ScenarioResult();
        //
        //		// easiest to check against a custom source, so build it: 2 strings of at least 2 chars each
        //		cb.AutoCompleteCustomSource.Clear();
        //
        //		string[] source = p.ru.GetUniqueStrings(2, 2, MAX_ITEM_NAME_LENGTH);
        //
        //		// prepend unique characters to use for auto completion
        //		source[0] = "a" + source[0];
        //		source[1] = "b" + source[1];
        //		cb.AutoCompleteCustomSource.AddRange(source);
        //		cb.AutoCompleteSource = AutoCompleteSource.CustomSource;
        //		SafeMethods.Focus(cb);
        //		Application.DoEvents();
        //		sr.IncCounters(checkAppendAndSuggestForMode(AutoCompleteMode.None));
        //		sr.IncCounters(checkAppendAndSuggestForMode(AutoCompleteMode.Append));
        //		sr.IncCounters(checkAppendAndSuggestForMode(AutoCompleteMode.Suggest));
        //		sr.IncCounters(checkAppendAndSuggestForMode(AutoCompleteMode.SuggestAppend));
        //		sendKeysAndWait("{ENTER}");
        //
        //		return sr;
        //	}

        [Scenario(false)]
        private ScenarioResult checkAppendAndSuggestForMode(AutoCompleteMode ACMode)
        {
            ScenarioResult sr = new ScenarioResult();
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("1");
            cb.AutoCompleteMode = ACMode;
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("2");
            if (cb.AutoCompleteCustomSource.Count == 0)
                return ScenarioResult.Fail;

            scenarioParams.log.WriteLine("== checking appending and suggesting for mode: " + ACMode);

            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("3");
            string first = cb.AutoCompleteCustomSource[0];
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("4");
            string toSend = first.Substring(0, 1);
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("5");
            Point justBelowComboBox = new Point(cb.Bounds.Left + (cb.Width / 2), cb.Bounds.Bottom + 2);
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("6");

            cb.Text = "";
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("7");
            sendKeysAndWait(toSend);
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("8");
            sendKeysAndWait("{ENTER}");
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("9");

            // 1. Should append ONLY in Append or SuggestAppend
            if (ACMode == AutoCompleteMode.Append || ACMode == AutoCompleteMode.SuggestAppend)
            {
                MauiMMFComboBoxTests.scenarioParams.log.WriteLine("10");
                sr.IncCounters(cb.Text == first.Substring(0, cb.Text.Length), "(Append) Expected " + first + " but got " + cb.Text + ".  (Mode: " + ACMode + ")", scenarioParams.log);
            }
            else
            {
                MauiMMFComboBoxTests.scenarioParams.log.WriteLine("11");
                sr.IncCounters(cb.Text == toSend.Substring(0, cb.Text.Length), "(Else) Expected " + toSend + " but got " + cb.Text + ".  (Mode: " + ACMode + ")", scenarioParams.log);
            }
            cb.Text = "";
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("12");
            sendKeysAndWait(toSend);

            // 2. Should suggest ONLY in Suggest or SuggestAppend
            //		BeginSecurityCheck(new UIPermission(PermissionState.None));
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("13");
            Bitmap formBitmap = Utilities.GetBitmapOfControl(this);
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("14");
            //		EndSecurityCheck();
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("15");

            Color justBelowColor = formBitmap.GetPixel(justBelowComboBox.X, justBelowComboBox.Y);
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("16");

            if (ACMode == AutoCompleteMode.Suggest || ACMode == AutoCompleteMode.SuggestAppend)
            {
                MauiMMFComboBoxTests.scenarioParams.log.WriteLine("17");
                sr.IncCounters(justBelowColor.ToArgb() != this.BackColor.ToArgb(), "FAIL: suggestion box not dropped down", "Mode: " + ACMode + ", Expected not to find: " + this.BackColor, scenarioParams.log);
            }
            else
            {
                MauiMMFComboBoxTests.scenarioParams.log.WriteLine("18");
                sr.IncCounters(justBelowColor.ToArgb() == this.BackColor.ToArgb(), "FAIL: suggestion box dropped down in Mode: " + ACMode, this.BackColor, justBelowColor, scenarioParams.log);
            }

            sendKeysAndWait("{ENTER}");
            MauiMMFComboBoxTests.scenarioParams.log.WriteLine("19");
            return sr;
        }

        //@ Default value for AutoCompleteMode is None
        [Scenario(true)]
        public ScenarioResult AutoCompleteModeDefault(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            // make a new ComboBox
            ComboBox newCB = new ComboBox();

            this.Controls.Add(newCB);

            // 1. ComboBox's AutoCompleteMode should be None
            sr.IncCounters(newCB.AutoCompleteMode == AutoCompleteMode.None, "FAIL: default value for AutoCompleteMode was wrong", AutoCompleteMode.None, newCB.AutoCompleteMode, p.log);
            this.Controls.Remove(newCB);
            return sr;
        }

        //@ Default value for AutoCompleteSource is None
        [Scenario(true)]
        public ScenarioResult AutoCompleteSourceDefault(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // make a new ComboBox
            ComboBox newCB = new ComboBox();

            this.Controls.Add(newCB);

            // 1. ComboBox's AutoCompleteSource should be None
            sr.IncCounters(newCB.AutoCompleteSource == AutoCompleteSource.None, "FAIL: default value for AutoCompleteSource was wrong", AutoCompleteSource.None, newCB.AutoCompleteSource, p.log);
            this.Controls.Remove(newCB);
            return sr;
        }

        //	[Scenario("Required permissions are demanded for sources")]
        //	public ScenarioResult Scenario11(TParams p)
        //	{
        //		// t-t: NYI, waiting on bugs 75531 and 121696
        //	}

        //@ Unalpha lists show up alpha (incl. Turk. I, Nor. A and case-insens.
        [Scenario(true)]
        public ScenarioResult AlphabetizationOfSuggestList(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            p.log.WriteLine("CurrentCulture: " + CultureInfo.CurrentCulture);
            cb.AutoCompleteCustomSource.Clear();

            int sourceItemsToAdd = p.ru.GetRange(1, MAX_ITEM_COUNT);

            // prepare the three arrays used in IntlStrings.GetSortingProblemStrings
            string[] unsortedSource = new string[sourceItemsToAdd];
            string[] invariantSortedSource = new string[sourceItemsToAdd];
            string[] cultureSortedSource = new string[sourceItemsToAdd];
            Maui.Core.International.IntlStrings intlStrings = new Maui.Core.International.IntlStrings();
            p.log.WriteLine("array length: " + cultureSortedSource.Length);

            //t-t: potential problem--GetSortingProblemStrings sorts case-sensitively, I think
            //t-t: well, at least that isn't true for english...why the sorting problems then?
            intlStrings.GetSortingProblemStrings(ref unsortedSource, ref invariantSortedSource, ref cultureSortedSource, CultureInfo.CurrentCulture);

            for (int count = 0; count < cultureSortedSource.Length - 1; count++)
            {
                if (string.Compare(cultureSortedSource[count], cultureSortedSource[count + 1]) < 0 && !Utilities.IsWin9x)
                    p.log.WriteLine("less than 0!");
            }

            for (int counter = 0; counter < unsortedSource.Length; counter++)
            {
                p.log.WriteLine("Culture: " + cultureSortedSource[counter]);
            }

            for (int i = 0; i < unsortedSource.Length; i++)
            {
                // get rid of whacks because the escape sequences give probs (\ = alt+0092, [ = alt+0091)
                unsortedSource[i].Replace('\\', '[');

                // to each string, prepend a character so that typing it will bring up all AutoComplete suggestions
                unsortedSource[i] = "_" + unsortedSource[i];
            }

            // add the unsorted strings to AutoCompleteCustomSource, which should then sort based on CurrentCulture
            cb.AutoCompleteCustomSource.AddRange(unsortedSource);
            cb.AutoCompleteMode = p.ru.GetBoolean() ? AutoCompleteMode.Suggest : AutoCompleteMode.SuggestAppend;
            cb.AutoCompleteSource = AutoCompleteSource.CustomSource;

            //NOTE: For Turkish culture, the sorting's different for the dropdown than the .net sort:
            //just make sure the items are present
            List<string> unmatchedStrings = new List<string>();
            foreach (string s in cultureSortedSource)
                unmatchedStrings.Add("_" + s);

            SafeMethods.Focus(cb);
            sendKeysAndWait("_");

            for (int i = 0; i < cultureSortedSource.Length; i++)
            {
                p.log.WriteLine("iteration" + i);
                p.log.WriteLine("remained" + unmatchedStrings.Count);
                sendKeysAndWait("{DOWN}");  // should select the next item in the suggestion list
                if (this.ManualMode)
                {
                    p.log.WriteLine("cultureSortedSource[i]: " + cultureSortedSource[i]);
                    p.log.WriteLine("invariantSortedSource[i]: " + invariantSortedSource[i]);
                    p.log.WriteLine("cb.text: " + cb.Text);
                }

                // See if suggestions were correctly alphabetized
                //NOTE: Win9x chokes on Turkish I and some other chars, so can't
                //really compare the strings, even for equality...
                if (Utilities.IsWin9x)
                {
                    int count = sr.TotalCount;
                    for (int idx = 0; idx < unmatchedStrings.Count; idx++)
                        if (unmatchedStrings[idx].Length == cb.Text.Length)
                        {
                            unmatchedStrings.RemoveAt(idx);
                            p.log.WriteLine("Found string of correct length vs. " + cb.Text.Length);
                            sr.IncCounters(true, "Found string of correct length", p.log);
                            break;
                        }
                    if (sr.TotalCount == count)
                        sr.IncCounters(false, "Didn't find string of correct length", p.log);
                }
                else
                {
                    bool cont = unmatchedStrings.Contains(cb.Text);
                    if (cont)
                    {
                        p.log.WriteLine("contained " + cb.Text);
                        sr.IncCounters(true);
                    }
                    else
                    {
                        sr.IncCounters(false, "Fail: Didn't contain " + cb.Text, p.log);
                    }
                    unmatchedStrings.Remove(cb.Text);
                    //sr.IncCounters("_" + cultureSortedSource[i], cb.Text, p.log);
                }
            }

            sendKeysAndWait("{ENTER}");

            //        Utilities.ActiveFreeze();

            return sr;
        }

        //@ Expected strings are populated, suggested and appended for ACSource: AllUrl.
        [Scenario(true)]
        public ScenarioResult AllUrl(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            cb.AutoCompleteMode = AutoCompleteMode.None;
            p.log.WriteLine("Using new method to add string to history.");

            // Before the following 4 lines of code worked. Now we change it to the call to a method.
            // we'll force the AllUrl list to include a specified website
            //WebBrowser browser = new WebBrowser();
            //this.Controls.Add(browser);
            //NavigateTo(p, browser, URL_FOR_AUTOCOMPLETION, 10000);
            //Application.DoEvents();

            SafeMethods.AddUrlToHistory(URL_FOR_AUTOCOMPLETION);
            p.log.WriteLine("browsing to URL: " + URL_FOR_AUTOCOMPLETION);

            // New decision about autocomplete: it should not work for DropDownStyle = DropDownList
            cb.DropDownStyle = p.ru.GetBoolean() ? ComboBoxStyle.DropDown : ComboBoxStyle.Simple;
            p.log.WriteLine("cb.DropDownStyle is set to " + cb.DropDownStyle.ToString());
            cb.AutoCompleteMode = p.ru.GetBoolean() ? AutoCompleteMode.Suggest : AutoCompleteMode.SuggestAppend;
            p.log.WriteLine("cb.AutoCompleteMode is set to " + cb.AutoCompleteMode.ToString());

            //cb.AutoCompleteSource = AutoCompleteSource.AllUrl;
            SafeMethods.SetAutoCompleteSource(cb, AutoCompleteSource.AllUrl);
            SafeMethods.Focus(cb);
            Thread.Sleep(1000);
            Application.DoEvents();
            sendKeysAndWait(URL_FOR_AUTOCOMPLETION.Substring(0, URL_FOR_AUTOCOMPLETION.Length - 1));
            Thread.Sleep(1000);
            Application.DoEvents();

            string text = "initial string";
            string firstSuggested = cb.Text;

            p.log.WriteLine("firstSuggested: " + firstSuggested);
            //p.log.WriteLine("cb item count is: " + cb.Items.Count.ToString());
            int loopCount = 0;
            bool found = false;

            p.log.WriteLine("Looking for: " + URL_FOR_AUTOCOMPLETION.ToLower());
            for (loopCount = 0; loopCount < MAX_SUGGESTIONS_TO_CHECK; loopCount++)
            {
                sendKeysAndWait("{DOWN}");
                text = cb.Text;
                p.log.WriteLine(text);
                //if (text.Contains(URL_FOR_AUTOCOMPLETION) && text != firstSuggested)
                //    break;
                if (text.ToLower().Contains(URL_FOR_AUTOCOMPLETION.ToLower()))
                {
                    found = true;
                    break;
                }
            }
            p.log.WriteLine("loop iterations made: " + loopCount.ToString());

            // MAX_SUGGESTIONS_TO_CHECK is some big number, currently it is set to 500.
            if (loopCount == MAX_SUGGESTIONS_TO_CHECK)
                sr.IncCounters(false, "FAIL: reached max number of suggestions to check, consider increasing or bugging", p.log);
            sr.IncCounters(found, "Fail!!! Expected string was not found!", p.log);


            //sr.IncCounters(text.ToLower().Contains(URL_FOR_AUTOCOMPLETION.ToLower()), 
            //    "cb.Text was '" + text + "' but wanted '" + URL_FOR_AUTOCOMPLETION + "'.", p.log);
            //sendKeysAndWait("{ENTER}");

            // see above - now we are using SafeMethod AddUrlToHistory, so we do not need these two lines
            //browser.Hide();
            //this.Controls.Remove(browser);
            return sr;
        }

        //@ Expected strings are populated, suggested and appended for ACSource: FileSystem.
        [Scenario(true)]
        public ScenarioResult FileSystem(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            //cb.AutoCompleteSource = AutoCompleteSource.FileSystem;
            SafeMethods.SetAutoCompleteSource(cb, AutoCompleteSource.FileSystem);
            cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            // get directory information so we know what to expect
            string originaldir = SafeMethods.GetCurrentDirectory();
            BeginSecurityCheck(LibSecurity.UnrestrictedFileIO);     // TODO: Better to create SafeMethods.CreateDirectory
            DirectoryInfo tempfolder = Directory.CreateDirectory("MMFTemp");
            EndSecurityCheck();
            Directory.SetCurrentDirectory(tempfolder.ToString());
            FileInfo tempfile;

            string[] dwarves = { "Happy.txt", "Sleepy.txt", "Dopey.txt", "Sneezy.txt", "Grumpy.txt", "Bashful.txt", "Doc.txt" };
            foreach (string dwarf in dwarves)
            {
                tempfile = new FileInfo(dwarf);
                tempfile.Create();
            }

            p.log.WriteLine("directory to search: " + tempfolder.ToString());

            FileSystemInfo[] fsis = tempfolder.GetFileSystemInfos();
            List<string> filesAndDirs = new List<string>();

            foreach (FileSystemInfo fsi in fsis)
            {
                filesAndDirs.Add(fsi.FullName);
            }

            filesAndDirs.Sort();  // by default, does case-insensitive sorting (and so does Suggest)

            int numToCheck = (filesAndDirs.Count < FILES_TO_CHECK) ? filesAndDirs.Count : FILES_TO_CHECK;

            SafeMethods.Focus(cb);
            sendKeysAndWait("{ESC}");

            // now, send in directory path keystrokes.  CAN'T use sendKeys with whole path, messes up so
            //		must do one character at a time.
            for (int i = 0; i < tempfolder.FullName.Length; i++)
                sendKeysAndWait(char.ToString(tempfolder.FullName[i]));
            sendKeysAndWait("\\");

            for (int i = 0; i < numToCheck; i++)
            {
                sendKeysAndWait("{DOWN}");  // should select the next item in the suggestion list
                sr.IncCounters(cb.Text.ToLower() == filesAndDirs[i].ToLower(),
                    "Expected '" + filesAndDirs[i].ToLower() + "' but got '" + cb.Text.ToLower() + "'",
                    p.log);
            }

            sendKeysAndWait("{ENTER}");

            Directory.SetCurrentDirectory(originaldir);

            return sr;
        }

        //	[Scenario("Expected strings are populated, suggested and appended for ACSource: FileSystem")]
        //	public ScenarioResult FileSystem(TParams p)
        //	{
        //		// TODO: Really should create a directory structure for this to be nice and predictable
        //		ScenarioResult sr = new ScenarioResult();
        //
        //		cb.AutoCompleteSource = AutoCompleteSource.FileSystem;
        //		cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        //
        //		// get directory information so we know what to expect
        //		DirectoryInfo currentRoot = new DirectoryInfo(Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()));
        //
        //		p.log.WriteLine("directory to search: " + currentRoot);
        //
        //		FileSystemInfo[] fsis = currentRoot.GetFileSystemInfos();
        //
        //		List<string> filesAndDirs = new List<string>();
        //
        //		// get rid of hidden files (they don't show up in suggestion box)
        //		foreach (FileSystemInfo fsi in fsis)
        //		{
        ////			if ((fsi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
        //			filesAndDirs.Add(fsi.FullName);
        //		}
        //
        //		filesAndDirs.Sort();  // by default, does case-insensitive sorting (and so does Suggest)
        //
        //		// i used to have it check all the files in the directory, but on lab machines the
        //		//		d:\school directory was HUGE, and frequently caused time-outs
        //		int numToCheck = (filesAndDirs.Count < FILES_TO_CHECK) ? filesAndDirs.Count : FILES_TO_CHECK;
        //
        //		SafeMethods.Focus(cb);
        //		sendKeysAndWait("{ESC}");
        //
        //		// now, send in directory path keystrokes.  CAN'T use sendKeys with whole path, messes up so
        //		//		must do one character at a time.
        //		for (int i = 0; i < currentRoot.FullName.Length; i++)
        //			sendKeysAndWait(char.ToString(currentRoot.FullName[i]));
        //
        //		string expected;
        //		string actual;
        //
        //		for (int i = 0; i < numToCheck; i++)
        //		{
        //			sendKeysAndWait("{DOWN}");  // should select the next item in the suggestion list
        //			expected = filesAndDirs[i].ToLower();
        //			actual = cb.Text.ToLower();
        //			sr.IncCounters(actual == expected, "FAIL: ComboBox's auto suggestions in FileSystem mode were not correct (or correctly ordered)", expected, actual, p.log);
        //		}
        //
        //		sendKeysAndWait("{ENTER}");
        //		return sr;
        //	}

        //@ CustomSource (custom StringArray) -- null, dupe, empty, I, aA.
        [Scenario(true)]
        public ScenarioResult CustomSource(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // check to make sure duplicates only appear once in suggestion list
            string dupeString = "_" + p.ru.GetString(2, MAX_ITEM_NAME_LENGTH);

            p.log.WriteLine("Forced duplicate string: " + dupeString);
            cb.AutoCompleteCustomSource.Add(dupeString);
            cb.AutoCompleteCustomSource.Add(dupeString);
            cb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            cb.AutoCompleteMode = p.ru.GetBoolean() ? AutoCompleteMode.Suggest : AutoCompleteMode.SuggestAppend;
            SafeMethods.Focus(cb);
            cb.Text = "";
            sendKeysAndWait("_");

            int loopCount = 0;

            // find the first of the duplicates
            do
            {
                sendKeysAndWait("{DOWN}");
                loopCount++;
            } while (cb.Text != dupeString && loopCount < MAX_SUGGESTIONS_TO_CHECK);

            // 1. Check to make sure it is NOT duplicated in the suggestion box
            sendKeysAndWait("{DOWN}");
            sr.IncCounters(cb.Text != dupeString, "FAIL: text was duplicated in suggestion box", "duplicated text: " + dupeString, p.log);
            sendKeysAndWait("{ENTER}");

            // 2. Clear AutoCompleteCustomSource and make sure it has no elements
            cb.AutoCompleteCustomSource.Clear();
            sr.IncCounters(cb.AutoCompleteCustomSource.Count == 0, "FAIL: cleared AutoCompleteCustomSource did not have zero items", 0, cb.AutoCompleteCustomSource.Count, p.log);

            // 3. Now set AutoCompleteCustomSource to null and make sure it has no elements
            cb.AutoCompleteCustomSource.Add(p.ru.GetString(MAX_ITEM_NAME_LENGTH));
            cb.AutoCompleteCustomSource = null;
            sr.IncCounters(cb.AutoCompleteCustomSource.Count == 0, "FAIL: nulled AutoCompleteCustomSource did not have zero items", 0, cb.AutoCompleteCustomSource.Count, p.log);
            return sr;
        }

        //t-t: not sure how to test these next three.  they ostensibly depend on registry values.
        //	[Scenario("Expected strings are populated, suggested and appended for ACSource: AllSystemResources")]
        //	public ScenarioResult Scenario21(TParams p)
        //	{
        //		// TODO: Rename Scenario21 with a descriptive method name.
        //		return new ScenarioResult(false, "NYI", p.log);
        //	}
        //
        //	[Scenario("Expected strings are populated, suggested and appended for ACSource: HistoryList")]
        //	public ScenarioResult Scenario24(TParams p)
        //	{
        //		// TODO: Rename Scenario24 with a descriptive method name.
        //		return new ScenarioResult(false, "NYI", p.log);
        //	}
        //
        //	[Scenario("Expected strings are populated, suggested and appended for ACSource: RecentlyUsedList")]
        //	public ScenarioResult Scenario25(TParams p)
        //	{
        //		// TODO: Rename Scenario25 with a descriptive method name.
        //		return new ScenarioResult(false, "NYI", p.log);
        //	}
        //
        //	[Scenario("Expected strings are populated, suggested and appended for ACSource: None")]
        //	public ScenarioResult Scenario27(TParams p)
        //	{
        //		// TODO: Rename Scenario27 with a descriptive method name.
        //		return new ScenarioResult(false, "NYI", p.log);
        //	}
        //t-t: apparently, this scenario should not exist.  it is not expected behavior.
        //	[Scenario("Setting ACMode to any non-None value sets ACSource to AllSystemSources")]
        //	public ScenarioResult AutoCompleteModeSetsAutoCompleteSource(TParams p)
        //	{
        //		ScenarioResult sr = new ScenarioResult();
        //
        //		// reset the two relevant properties
        //		cb.AutoCompleteMode = AutoCompleteMode.None;
        //		cb.AutoCompleteSource = p.ru.GetDifferentEnumValue<AutoCompleteSource>(AutoCompleteSource.AllSystemSources);
        //
        //		cb.AutoCompleteMode = p.ru.GetDifferentEnumValue<AutoCompleteMode>(AutoCompleteMode.None);
        //
        //		// 1. Setting AutoCompleteMode to non-null value should set AutoCompleteSource to AllSystemSources
        //		sr.IncCounters(cb.AutoCompleteSource == AutoCompleteSource.AllSystemSources, "FAIL: setting AutoCompleteMode to non-null value did not set AutoCompleteSource to AllSystemSources", p.log);
        //
        //		return sr;
        //	}
        //
        //t-t: hmm, doesn't seem like a pri1, probably not even automation.
        //	[Scenario("15) Keyboard interface identical to IE")]
        //	public ScenarioResult Scenario15(TParams p)
        //	{
        //		return new ScenarioResult(false, "NYI", p.log);
        //	}
        //
        //t-t: also does not seem like a pri1.  will move to pri2...
        //	[Scenario("17) Works in MDI (context?)")]
        //	public ScenarioResult Scenario17(TParams p)
        //	{
        //		// TODO: Rename Scenario17 with a descriptive method name.
        //		return new ScenarioResult(false, "NYI", p.log);
        //	}

        #endregion

        #region Event Handlers
        private void cb_TextUpdate(object sender, EventArgs e)
        {
            scenarioParams.log.WriteLine("====[TextUpdate event fired]====");
            textUpdateCount++;
        }

        private void cb_DropDownClosed(object sender, EventArgs e)
        {
            scenarioParams.log.WriteLine("====[DropDownClosed event fired]====");
            dropDownClosedCount++;
        }

        #endregion

        #region Helper Functions
        private bool sendKeysAndWait(string keys)
        {
            SafeMethods.SendWait(keys);

            Application.DoEvents();
            Thread.Sleep(SLEEP_TIME);
            Application.DoEvents();
            return true;
        }

        private void sendLeftClickAndWait(Point point)
        {
            Mouse.Click(MouseFlags.LeftButton, point.X, point.Y);
            Application.DoEvents();
            Thread.Sleep(SLEEP_TIME);
            Application.DoEvents();
        }

        private void setACModeNone()
        {
            cb.AutoCompleteMode = AutoCompleteMode.None;
            scenarioParams.log.WriteLine("set AutoCompleteMode to None (on separate thread)");
        }

        private void addTextToCustomSource()
        {
            cb.AutoCompleteCustomSource.Add(newTextForSuggestion);
            scenarioParams.log.WriteLine("added text to custom source, text: " + newTextForSuggestion);
        }

        #endregion
    }
}
