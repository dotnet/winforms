// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.UI.IntegrationTests.Infra;
using WFCTestLib.Util;
using Xunit;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class ComboBoxTests
    {
        private const int NumItems = 200;

        private RandomUtil _randomUtil;

        private int _numEvents;

        public ComboBoxTests()
        {
            _randomUtil = new();
        }

        private bool InitializeItems(ComboBox comboBox)
        {
            var szItems = CreateItems(NumItems);
            return AddItems(comboBox, szItems, NumItems);
        }

        private string[] CreateItems(int numItems)
        {
            var items = new string[numItems];
            for (var i = 0; i < numItems; i++)
            {
                items[i] = "item" + i;
            }

            return items;
        }

        bool AddItems(ComboBox c, string[] items, int numToAdd)
        {
            for (var i = 0; i < numToAdd; i++)
            {
                try
                {
                    c.Items.Add(items[i]);
                }
                catch
                {
                    return false;
                }
            }

            if (c.Items.Count != numToAdd)
            {
                return false;
            }

            for (var j = 0; j < numToAdd; j++)
            {
                if (!items[j].Equals(c.Items[j]))
                {
                    return false;
                }
            }

            return true;
        }

        bool RemoveItems(ComboBox comboBox, bool isByIndex, int numToRemove)
        {
            for (var i = 0; i < numToRemove; i++)
            {
                var count = comboBox.Items.Count;
                var index = _randomUtil.GetRange(0, count - 1);
                var item = (string)((index < count - 1) ? comboBox.Items[index + 1] : comboBox.Items[index]);

                if (isByIndex)
                {
                    comboBox.Items.RemoveAt(index);
                }
                else
                {
                    comboBox.Items.Remove(comboBox.Items[index]);
                }

                if (comboBox.Items.Count != count - 1)
                {
                    return false;
                }

                if (index < count - 1)
                {
                    if (!item.Equals(comboBox.Items[index]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        bool FindItem(ComboBox comboBox, bool isExactMatch)
        {
            var count = comboBox.Items.Count;
            var index = _randomUtil.GetRange(0, count - 1);
            var expectedMatchingIndex = -1;
            var expectedValue = comboBox.Items[index].ToString();

            int matchingIndex;
            if (isExactMatch)
            {
                matchingIndex = comboBox.FindStringExact(expectedValue, -1);
            }
            else
            {
                matchingIndex = comboBox.FindString(expectedValue, -1);
            }

            for (var i = 0; i < count; i++)
            {
                if (comboBox.GetItemText(comboBox.Items[i]).Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    expectedMatchingIndex = i;
                    break;
                }
            }

            return (matchingIndex == expectedMatchingIndex);
        }

        bool SelectItemWithKeyboard(ComboBox comboBox, string key, int numKeyPresses)
        {
            var selectedIndex = (key.Equals("{DOWN}")) ? 0 : comboBox.Items.Count - 1;
            comboBox.SelectedIndex = (key.Equals("{DOWN}")) ? 0 : comboBox.Items.Count - 1;
            _numEvents = 0;
            comboBox.SelectedIndexChanged += (x, y) => _numEvents++;
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

        [StaFact]
        public void Add_Items_By_Index()
        {
            RunTest(comboBox =>
            {
                InitializeItems(comboBox);

                //add 5 items by index
                for (var i = 0; i < 5; i++)
                {
                    var count = comboBox.Items.Count;
                    var index = _randomUtil.GetRange(0, count - 1);
                    var item = "new item" + i;

                    comboBox.Items.Insert(index, item);
                    bool result = item.Equals(comboBox.Items[index]) && comboBox.Items.Count == ++count;

                    Assert.True(result);
                }
            });
        }

        [StaFact]
        public void Remove_Items_By_Index()
        {
            RunTest(comboBox =>
            {
                InitializeItems(comboBox);

                //remove 5 items by index
                bool result = RemoveItems(comboBox, true, 5);

                Assert.True(result);
            });
        }

        [StaFact]
        public void Remove_Items_By_Object()
        {
            RunTest(comboBox =>
            {
                InitializeItems(comboBox);

                //remove 5 items by object
                bool result = RemoveItems(comboBox, false, 5);

                Assert.True(result);
            });
        }

        [StaFact]
        public void Sorted_Sorts_By_Ascending()
        {
            RunTest(comboBox =>
            {
                var items = new string[NumItems];
                // all the items are between a - z
                for (var j = 0; j < NumItems; j++)
                {
                    var item = (_randomUtil.GetInt(true) % 26 + 97).ToString();
                    items[j] = item;
                    comboBox.Items.Add(item);
                }

                comboBox.Sorted = true;
                Array.Sort(items);

                for (int i = 0; i < NumItems; i++)
                {
                    bool result = items[i].Equals(comboBox.Items[i]);
                    Assert.True(result);
                }
            });
        }

        [StaFact]
        public void SelectedIndex_Changes_SelectedItem_And_Fires_OnSelectedIndexChanged()
        {
            RunTest(comboBox =>
            {
                InitializeItems(comboBox);

                comboBox.Sorted = false;
                int numEventsExpected = 0;
                _numEvents = 0;
                comboBox.SelectedIndexChanged += (x, y) => _numEvents++;

                //change SelectedIndex 10 times
                for (var i = 0; i < 10; i++)
                {
                    int selectedIndex = comboBox.SelectedIndex;
                    int index = _randomUtil.GetRange(0, NumItems - 1);

                    comboBox.SelectedIndex = index;
                    Assert.True(comboBox.SelectedIndex == index);

                    if (selectedIndex != index)
                        numEventsExpected++;

                    Assert.True(comboBox.Items[comboBox.SelectedIndex].Equals(comboBox.SelectedItem));
                }

                Assert.True(_numEvents == numEventsExpected);
            });
        }

        [StaFact]
        public void FindString()
        {
            RunTest(comboBox =>
            {
                InitializeItems(comboBox);

                Assert.True(FindItem(comboBox, false));
            });
        }

        [StaFact]
        public void FindStringExact()
        {
            RunTest(comboBox =>
            {
                InitializeItems(comboBox);

                Assert.True(FindItem(comboBox, true));
            });
        }

        [StaFact]
        public void Select_Item_By_Keyboard()
        {
            RunTest(comboBox =>
            {
                var numKeyPresses = _randomUtil.GetRange(1, comboBox.Items.Count);
                var szItems = CreateItems(NumItems);
                comboBox.SelectedIndexChanged += (x, y) => _numEvents++;

                Assert.True(AddItems(comboBox, szItems, NumItems));

                //scroll items using the down arrow key
                Assert.True(SelectItemWithKeyboard(comboBox, "{DOWN}", numKeyPresses));

                Assert.True(_numEvents == numKeyPresses);

                //scroll items using the up arrow key
                Assert.True(SelectItemWithKeyboard(comboBox, "{UP}", numKeyPresses));
                Assert.True(_numEvents == numKeyPresses);
            });
        }

        [StaFact]
        public void Verify_OnMeasureItem_receives_correct_arguments()
        {
            RunTestForDerivedComboBox(derivedComboBox =>
            {
                derivedComboBox.Items.AddRange(new object[]
                {
                    "One",
                    "Two",
                    "Three"
                });
                derivedComboBox.Location = new Point(0, 50);

                Assert.Equal(3, derivedComboBox.MeasureItemEventArgs.Count);

                for (int i = 0; i < 3; i++)
                {
                    MeasureItemEventArgs e = derivedComboBox.MeasureItemEventArgs[i];

                    Assert.NotNull(e.Graphics);
                    Assert.Equal(e.Index, i);
                    Assert.Equal(18, e.ItemHeight);
                    Assert.Equal(0, e.ItemWidth);
                }
            });
        }

        private class DerivedComboBox : ComboBox
        {
            public DerivedComboBox()
            {
                DrawMode = DrawMode.OwnerDrawVariable;
                FormattingEnabled = true;
            }

            public List<MeasureItemEventArgs> MeasureItemEventArgs { get; } = new List<MeasureItemEventArgs>();

            protected override void OnMeasureItem(MeasureItemEventArgs e)
            {
                MeasureItemEventArgs.Add(e);
            }
        }

        private void RunTest(Action<ComboBox> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    ComboBox comboBox = new()
                    {
                        Parent = form,
                    };
                    return comboBox;
                },
                runTestAsync: async comboBox =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(comboBox);
                });
        }

        private void RunTestForDerivedComboBox(Action<DerivedComboBox> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    DerivedComboBox derivedComboBox = new()
                    {
                        Parent = form,
                    };
                    return derivedComboBox;
                },
                runTestAsync: async derivedComboBox =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(derivedComboBox);
                });
        }
    }
}
