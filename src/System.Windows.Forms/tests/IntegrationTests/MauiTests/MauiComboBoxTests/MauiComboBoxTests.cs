// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiComboBoxTests : ReflectBase
    {
        private const int NumItems = 200;
        private readonly ComboBox _comboBox;
        private int _numEvents = 0;

        public MauiComboBoxTests(string[] args) : base(args)
        {
            this.BringToForeground();
            _comboBox = new ComboBox();
            _comboBox.SelectedIndexChanged += (x,y) => _numEvents++;
            Controls.Add(_comboBox);
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiComboBoxTests(args));
        }

        [Scenario(true)]
        public ScenarioResult Add_Items_By_Index(TParams p)
        {
            if (!InitializeItems(_comboBox, p))
                return new ScenarioResult(false, "failed to initialize combobox items");

            //add 5 items by index
            for (var i = 0; i < 5; i++)
            {
                var count = _comboBox.Items.Count;
                var index = p.ru.GetRange(0, count - 1);
                var item = "new item" + i;

                _comboBox.Items.Insert(index, item);
                if (!item.Equals(_comboBox.Items[index]) || _comboBox.Items.Count != ++count)
                    return new ScenarioResult(false, "failed to add items by index.");
            }

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult Remove_Items_By_Index(TParams p)
        {
            if (!InitializeItems(_comboBox, p))
                return new ScenarioResult(false, "failed to initialize combobox items");

            //remove 5 items by index
            if (!RemoveItems(p, _comboBox, true, 5))
                return new ScenarioResult(false, "failed to remove items by index.");

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult Remove_Items_By_Object(TParams p)
        {
            if (!InitializeItems(_comboBox, p))
                return new ScenarioResult(false, "failed to initialize combobox items");

            //remove 5 items by object
            if (!RemoveItems(p, _comboBox, false, 5))
                return new ScenarioResult(false, "failed to remove items by object.");

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult Sorted_Sorts_By_Ascending(TParams p)
        {
            _comboBox.Items.Clear();

            var items = new string[NumItems];
            // all the items are between a - z
            for (var j = 0; j < NumItems; j++)
            {
                var item = (p.ru.GetInt(true) % 26 + 97).ToString();
                items[j] = item;
                _comboBox.Items.Add(item);
            }

            _comboBox.Sorted = true;
            Array.Sort(items);

            for (int i = 0; i < NumItems; i++)
            {
                if (!items[i].Equals(_comboBox.Items[i]))
                    return new ScenarioResult(false, i.ToString() + "th items does not match.");
            }

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult SelectedIndex_Changes_SelectedItem_And_Fires_OnSelectedIndexChanged(TParams p)
        {
            _comboBox.Sorted = false;
            var numEventsExpected = 0;
            _numEvents = 0;

            if (!InitializeItems(_comboBox, p))
                return new ScenarioResult(false, "failed to initialize combobox items");

            //change SelectedIndex 10 times
            for (var i = 0; i < 10; i++)
            {
                int selectedIndex = _comboBox.SelectedIndex;
                int index = p.ru.GetRange(0, NumItems - 1);

                _comboBox.SelectedIndex = index;
                if (_comboBox.SelectedIndex != index)
                    return new ScenarioResult(false, "failed to set SelectedIndex");

                if (selectedIndex != index)
                    numEventsExpected++;

                if (!_comboBox.Items[_comboBox.SelectedIndex].Equals(_comboBox.SelectedItem))
                    return new ScenarioResult(false, "failed to sync up SelectedItem with SelectedIndex");
            }

            if (_numEvents != numEventsExpected)
                return new ScenarioResult(false, "failed to handle OnSelectedIndexChanged event properly.");

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult FindString(TParams p)
        {
            if (!InitializeItems(_comboBox, p))
                return new ScenarioResult(false, "failed to initialize combobox items");

            if (!FindItem(p, _comboBox, false))
                return new ScenarioResult(false, "failed to find item.");

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult FindStringExact(TParams p)
        {
            if (!InitializeItems(_comboBox, p))
                return new ScenarioResult(false, "failed to initialize combobox items");

            if (!FindItem(p, _comboBox, true))
                return new ScenarioResult(false, "failed to find exact item.");

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult Select_Item_By_Keyboard(TParams p)
        {
            var sr = new ScenarioResult();
            _comboBox.Items.Clear();

            var numKeyPresses = p.ru.GetRange(1, _comboBox.Items.Count);
            var szItems = CreateItems(NumItems);

            sr.IncCounters(AddItems(p, _comboBox, szItems, NumItems), "failed to add items to ObjectCollection", p.log);

            //scroll items using the down arrow key
            sr.IncCounters(SelectItemWithKeyboard(_comboBox, "{DOWN}", numKeyPresses), "failed to select items with the DOWN arrow key", p.log);
            sr.IncCounters(_numEvents == numKeyPresses, "failed to handle SelectedIndexChanged event properly going down; nEvents was " + _numEvents + ", nKeys was " + numKeyPresses, p.log);

            //scroll items using the up arrow key
            sr.IncCounters(SelectItemWithKeyboard(_comboBox, "{UP}", numKeyPresses), "failed to select items with the UP arrow key", p.log);
            sr.IncCounters(_numEvents == numKeyPresses, "failed to handle SelectedIndexChanged event properly going up; nEvents was " + _numEvents + ", nKeys was " + numKeyPresses, p.log);

            return sr;
        }

        private bool InitializeItems(ComboBox comboBox, TParams p)
        {
            comboBox.Items.Clear();
            var szItems = CreateItems(NumItems);
            return AddItems(p, comboBox, szItems, NumItems);
        }

        string[] CreateItems(int numItems)
        {
            var items = new string[numItems];
            for (var i = 0; i < numItems; i++)
                items[i] = "item" + i;
            return items;
        }

        bool AddItems(TParams p, ComboBox c, string[] items, int numToAdd)
        {
            for (var i = 0; i < numToAdd; i++)
            {
                try
                {
                    c.Items.Add(items[i]);
                }
                catch (Exception e)
                {
                    p.log.WriteLine("failed: unexpected exception thrown.");
                    p.log.WriteLine(e.Message);
                    return false;
                }
            }

            if (c.Items.Count != numToAdd)
            {
                p.log.WriteLine("failed: incorrect ObjectCollection.Count.");
                return false;
            }

            for (var j = 0; j < numToAdd; j++)
            {
                if (!items[j].Equals(c.Items[j]))
                {
                    p.log.WriteLine("failed: " + j.ToString() + "th items do not match.");
                    return false;
                }
            }
            return true;
        }

        bool RemoveItems(TParams p, ComboBox comboBox, bool isByIndex, int numToRemove)
        {
            for (var i = 0; i < numToRemove; i++)
            {
                var count = comboBox.Items.Count;
                var index = p.ru.GetRange(0, count - 1);
                var item = (string)((index < count - 1) ? comboBox.Items[index + 1] : comboBox.Items[index]);

                if (isByIndex)
                    comboBox.Items.RemoveAt(index);
                else
                    comboBox.Items.Remove(comboBox.Items[index]);

                if (comboBox.Items.Count != count - 1)
                    return false;

                if (index < count - 1)
                {
                    if (!item.Equals(comboBox.Items[index]))
                        return false;
                }
            }

            return true;
        }
        bool FindItem(TParams p, ComboBox comboBox, bool isExactMatch)
        {
            var count = comboBox.Items.Count;
            var index = p.ru.GetRange(0, count - 1);
            var expectedMatchingIndex = -1;
            var expectedValue = comboBox.Items[index].ToString();

            int matchingIndex;
            if (isExactMatch)
                matchingIndex = comboBox.FindStringExact(expectedValue, -1);
            else
                matchingIndex = comboBox.FindString(expectedValue, -1);

            for (var i = 0; i < count; i++)
            {
                if (comboBox.Items[i].ToString().Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    expectedMatchingIndex = i;
                    break;
                }
            }

            return (matchingIndex == expectedMatchingIndex);
        }
        bool SelectItemWithKeyboard(ComboBox comboBox, string key, int numKeyPresses)
        {
            Activate();
            var selectedIndex = (key.Equals("{DOWN}")) ? 0 : comboBox.Items.Count - 1;
            comboBox.SelectedIndex = (key.Equals("{DOWN}")) ? 0 : comboBox.Items.Count - 1;
            _numEvents = 0;
            for (var i = 0; i < numKeyPresses; i++)
            {
                if (key.Equals("{DOWN}"))
                    selectedIndex++;
                else
                    selectedIndex--;

                comboBox.Focus();
                Application.DoEvents();
                SendKeys.SendWait(key);
                Application.DoEvents();
                Thread.Sleep(500);
                if (comboBox.SelectedIndex != selectedIndex || comboBox.SelectedItem != comboBox.Items[comboBox.SelectedIndex])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
