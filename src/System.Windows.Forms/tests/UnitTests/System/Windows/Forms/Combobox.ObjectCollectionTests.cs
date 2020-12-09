﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;
using static System.Windows.Forms.ComboBox;
using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxObjectCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Add_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");

            // Check Entry values
            Assert.Equal("a", comboBoxObjectCollection[0]);
            Assert.Equal("b", comboBoxObjectCollection[1]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Add_Object_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");

            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add(person2);

            // Check Entry values
            Assert.Equal(person1, comboBoxObjectCollection[0]);
            Assert.Equal(person2, comboBoxObjectCollection[1]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Add_Invoke_ReturnExpected(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            int firstIndex = comboBoxObjectCollection.Add("a");
            int secondIndex = comboBoxObjectCollection.Add("a");

            Assert.Equal(0, firstIndex);
            Assert.Equal(1, secondIndex);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Add_Invoke_NullItem_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentNullException>("item", () => comboBoxObjectCollection.Add(null));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Add_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DataSource = Array.Empty<object>()
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentException>(null, () => comboBoxObjectCollection.Add("a"));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_Add_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            comboBoxObjectCollection.Add("b");
            comboBoxObjectCollection.Add("c");
            comboBoxObjectCollection.Add("a");

            // Check string values
            Assert.Equal("a", comboBoxObjectCollection[0]);
            Assert.Equal("b", comboBoxObjectCollection[1]);
            Assert.Equal("c", comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_Add_Object_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DisplayMember = TestDataSources.PersonDisplayMember
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");

            comboBoxObjectCollection.Add(person3);
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add(person2);

            // Check string values
            Assert.Equal(person1, comboBoxObjectCollection[0]);
            Assert.Equal(person2, comboBoxObjectCollection[1]);
            Assert.Equal(person3, comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_Add_ObjectAndString_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DisplayMember = TestDataSources.PersonDisplayMember
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            var person1 = new Person(1, "Name 1");
            var person2 = new Person(3, "Name 3");

            comboBoxObjectCollection.Add(person2);
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add("Name 2");

            // Check string values
            Assert.Equal(person1, comboBoxObjectCollection[0]);
            Assert.Equal("Name 2", comboBoxObjectCollection[1]);
            Assert.Equal(person2, comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_Add_Invoke_ReturnExpected(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            int firstIndex = comboBoxObjectCollection.Add("c");
            int secondIndex = comboBoxObjectCollection.Add("a");
            int thirdIndex = comboBoxObjectCollection.Add("b");

            Assert.Equal(0, firstIndex);
            Assert.Equal(0, secondIndex);
            Assert.Equal(1, thirdIndex);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_AddRange_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.AddRange(new string[] { "a", "b" });

            // Check string values
            Assert.Equal("a", comboBoxObjectCollection[0]);
            Assert.Equal("b", comboBoxObjectCollection[1]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_AddRange_Objects_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.AddRange(new Person[] { person1, person2, person3 });

            // Check string values
            Assert.Equal(person1, comboBoxObjectCollection[0]);
            Assert.Equal(person2, comboBoxObjectCollection[1]);
            Assert.Equal(person3, comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_AddRange_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            comboBoxObjectCollection.AddRange(new string[] { "c", "a", "b" });

            // Check string values
            Assert.Equal("a", comboBoxObjectCollection[0]);
            Assert.Equal("b", comboBoxObjectCollection[1]);
            Assert.Equal("c", comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_AddRange_Object_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DisplayMember = TestDataSources.PersonDisplayMember
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");

            comboBoxObjectCollection.AddRange(new Person[] { person3, person1, person2 });

            // Check string values
            Assert.Equal(person1, comboBoxObjectCollection[0]);
            Assert.Equal(person2, comboBoxObjectCollection[1]);
            Assert.Equal(person3, comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_AddRange_ObjectAndString_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DisplayMember = TestDataSources.PersonDisplayMember
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            var person1 = new Person(1, "Name 1");
            var person2 = new Person(3, "Name 3");

            comboBoxObjectCollection.AddRange(new object[] { "Name 2", person1, person2 });

            // Check string values
            Assert.Equal(person1, comboBoxObjectCollection[0]);
            Assert.Equal("Name 2", comboBoxObjectCollection[1]);
            Assert.Equal(person2, comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_AddRange_Invoke_NullItems_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            Assert.Throws<ArgumentNullException>("items", () => comboBoxObjectCollection.AddRange(null));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_AddRange_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DataSource = Array.Empty<object>()
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentException>(null, () => comboBoxObjectCollection.AddRange(new string[] { "a", "b" }));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Clear_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.AddRange(new string[] { "c", "a", "b" });

            Assert.Equal(3, comboBoxObjectCollection.Count);
            Assert.Equal(3, comboBoxObjectCollection.InnerList.Count);

            comboBoxObjectCollection.Clear();

            Assert.Equal(0, comboBoxObjectCollection.Count);
            Assert.Equal(0, comboBoxObjectCollection.InnerList.Count);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxObjectCollection_Clear_Invoke_ClearItemAccessibleObjects()
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.CreateControl();
            ObjectCollection comboBoxObjectCollection = comboBox.Items;
            comboBoxObjectCollection.AddRange(new string[] { "a", "b" });
            ComboBox.ComboBoxAccessibleObject accessibleObject = GetComboBoxAccessibleObject(comboBox);

            // Adding ComboBoxItemAccessibleObject to the "ItemAccessibleObjects" list
            ComboBoxItemAccessibleObject firstAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[0]);
            ComboBoxItemAccessibleObject secondAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[1]);

            Assert.Equal(2, comboBoxObjectCollection.InnerList.Count);
            Assert.Equal(2, accessibleObject.ItemAccessibleObjects.Count);

            comboBoxObjectCollection.Clear();

            Assert.Equal(0, comboBoxObjectCollection.Count);
            Assert.Equal(0, accessibleObject.ItemAccessibleObjects.Count);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Clear_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DataSource = Array.Empty<object>()
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentException>(null, () => comboBoxObjectCollection.Clear());
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Contains_Invoke_ReturnExpected(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.AddRange(new string[] { "a", "b" });

            Assert.True(comboBoxObjectCollection.Contains("a"));
            Assert.True(comboBoxObjectCollection.Contains("b"));
            Assert.False(comboBoxObjectCollection.Contains("c"));

            Assert.True(comboBoxObjectCollection.Contains(comboBoxObjectCollection.InnerList[0]));
            Assert.True(comboBoxObjectCollection.Contains(comboBoxObjectCollection.InnerList[1]));
            Assert.False(comboBoxObjectCollection.Contains(new Entry("a")));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Contains_Object_Invoke_ReturnExpected(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");

            comboBoxObjectCollection.AddRange(new object[] { person1, person2 });

            Assert.True(comboBoxObjectCollection.Contains(person1));
            Assert.True(comboBoxObjectCollection.Contains(person2));
            Assert.False(comboBoxObjectCollection.Contains(person3));

            Assert.True(comboBoxObjectCollection.Contains(comboBoxObjectCollection.InnerList[0]));
            Assert.True(comboBoxObjectCollection.Contains(comboBoxObjectCollection.InnerList[1]));
            Assert.False(comboBoxObjectCollection.Contains(new Entry(person3)));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_CopyTo_Invoke_NullArray_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentNullException>("destination", () => comboBoxObjectCollection.CopyTo(null, 1));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_CopyTo_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("arrayIndex", () => comboBoxObjectCollection.CopyTo(new object[1], -1));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_CopyTo_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("arrayIndex", () => comboBoxObjectCollection.CopyTo(new object[1], 3));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_CopyTo_InvokeEmpty_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            object[] array = new object[] { "1", "2", "3" };

            comboBoxObjectCollection.CopyTo(array, 1);

            Assert.Equal(new object[] { "1", "2", "3" }, array);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_CopyTo_InvokeNotEmpty_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add(1);
            comboBoxObjectCollection.Add(2);
            object[] array = new object[] { "1", "2", "3" };

            comboBoxObjectCollection.CopyTo(array, 1);

            Assert.Equal(new object[] { "1", 1, 2 }, array);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Insert_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");

            comboBoxObjectCollection.Insert(0, "c");

            Assert.Equal("c", comboBoxObjectCollection[0]);
            Assert.Equal("a", comboBoxObjectCollection[1]);
            Assert.Equal("b", comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Insert_Object_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add(person2);

            comboBoxObjectCollection.Insert(0, person3);

            Assert.Equal(person3, comboBoxObjectCollection[0]);
            Assert.Equal(person1, comboBoxObjectCollection[1]);
            Assert.Equal(person2, comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_Insert_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");

            comboBoxObjectCollection.Insert(0, "c");

            Assert.Equal("a", comboBoxObjectCollection[0]);
            Assert.Equal("b", comboBoxObjectCollection[1]);
            Assert.Equal("c", comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_Insert_Object_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DisplayMember = TestDataSources.PersonDisplayMember
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add(person2);

            comboBoxObjectCollection.Insert(0, person3);

            Assert.Equal(person1, comboBoxObjectCollection[0]);
            Assert.Equal(person2, comboBoxObjectCollection[1]);
            Assert.Equal(person3, comboBoxObjectCollection[2]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Sorted_Insert_ObjectAndString_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DisplayMember = TestDataSources.PersonDisplayMember
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.Sorted = true;
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            var person1 = new Person(1, "Name 1");
            var person2 = new Person(3, "Name 3");
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add("Name 2");

            comboBoxObjectCollection.Insert(0, person2);
            comboBoxObjectCollection.Insert(0, "Name 4");

            Assert.Equal(person1, comboBoxObjectCollection[0]);
            Assert.Equal("Name 2", comboBoxObjectCollection[1]);
            Assert.Equal(person2, comboBoxObjectCollection[2]);
            Assert.Equal("Name 4", comboBoxObjectCollection[3]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Insert_Invoke_NullItem_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentNullException>("item", () => comboBoxObjectCollection.Insert(0, null));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Insert_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.Insert(-1, 1));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Insert_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.Insert(3, 1));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Insert_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DataSource = Array.Empty<object>()
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentException>(null, () => comboBoxObjectCollection.Insert(3, 1));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_RemoveAt_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");

            comboBoxObjectCollection.RemoveAt(0);

            Assert.Equal(1, comboBoxObjectCollection.Count);
            Assert.Equal("b", comboBoxObjectCollection[0]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxObjectCollection_RemoveAt_Invoke_RemoveItemAccessibleObject()
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.CreateControl();

            comboBox.CreateControl();
            ObjectCollection comboBoxObjectCollection = comboBox.Items;
            comboBoxObjectCollection.Add("a");
            ComboBox.ComboBoxAccessibleObject accessibleObject = GetComboBoxAccessibleObject(comboBox);

            // Adding ComboBoxItemAccessibleObject to the "ItemAccessibleObjects" list
            ComboBoxItemAccessibleObject firstAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[0]);
            Assert.Equal(1, accessibleObject.ItemAccessibleObjects.Count);

            comboBoxObjectCollection.RemoveAt(0);

            Assert.Equal(0, comboBoxObjectCollection.Count);
            Assert.Equal(0, accessibleObject.ItemAccessibleObjects.Count);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_RemoveAt_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.RemoveAt(-1));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_RemoveAt_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.RemoveAt(3));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_RemoveAt_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox
            {
                DataSource = Array.Empty<object>()
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentException>(null, () => comboBoxObjectCollection.RemoveAt(0));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Remove_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");

            comboBoxObjectCollection.Remove("a");

            Assert.Equal(1, comboBoxObjectCollection.Count);
            Assert.Equal("b", comboBoxObjectCollection[0]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Remove_Invoke_NonExistingItem_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");

            comboBoxObjectCollection.Remove("c");

            Assert.Equal(2, comboBoxObjectCollection.Count);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Remove_Object_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add(person2);

            // Remove non-existing object
            comboBoxObjectCollection.Remove(person3);

            Assert.Equal(2, comboBoxObjectCollection.Count);

            // Remove existing object
            comboBoxObjectCollection.Remove(person1);

            Assert.Equal(1, comboBoxObjectCollection.Count);
            Assert.Equal(person2, comboBoxObjectCollection[0]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Remove_Entry_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");

            comboBoxObjectCollection.Remove(comboBoxObjectCollection[0]);

            Assert.Equal(1, comboBoxObjectCollection.Count);
            Assert.Equal("b", comboBoxObjectCollection[0]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxObjectCollection_Remove_Invoke_RemoveItemAccessibleObject()
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.CreateControl();

            comboBox.CreateControl();
            ObjectCollection comboBoxObjectCollection = comboBox.Items;
            comboBoxObjectCollection.Add("a");
            ComboBox.ComboBoxAccessibleObject accessibleObject = GetComboBoxAccessibleObject(comboBox);

            // Adding ComboBoxItemAccessibleObject to the "ItemAccessibleObjects" list
            ComboBoxItemAccessibleObject firstAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[0]);
            Assert.Equal(1, accessibleObject.ItemAccessibleObjects.Count);

            comboBoxObjectCollection.Remove("a");

            Assert.Equal(0, comboBoxObjectCollection.Count);
            Assert.Equal(0, accessibleObject.ItemAccessibleObjects.Count);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_Remove_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBox.DataSource = new string[] { "b" };

            Assert.Throws<ArgumentException>(null, () => comboBoxObjectCollection.Remove("a"));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItemInternal_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");
            comboBoxObjectCollection.SetItemInternal(0, "c");
            comboBoxObjectCollection.SetItemInternal(1, "b");

            Assert.Equal("c", comboBoxObjectCollection[0]);
            Assert.Equal("b", comboBoxObjectCollection[1]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItemInternal_Object_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add(person2);
            comboBoxObjectCollection.SetItemInternal(0, person3);
            comboBoxObjectCollection.SetItemInternal(1, "Name 4");

            Assert.Equal(person3, comboBoxObjectCollection[0]);
            Assert.Equal("Name 4", comboBoxObjectCollection[1]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItemInternal_Invoke_NullItem_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");

            Assert.Throws<ArgumentNullException>("value", () => comboBoxObjectCollection.SetItemInternal(0, null));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItemInternal_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.SetItemInternal(-1, 1));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItemInternal_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.SetItemInternal(3, 1));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_IndexOf_ReturnExpected(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection.Add("b");

            Assert.Equal(0, comboBoxObjectCollection.IndexOf("a"));
            Assert.Equal(1, comboBoxObjectCollection.IndexOf("b"));
            Assert.Equal(-1, comboBoxObjectCollection.IndexOf("c"));
            Assert.Equal(-1, comboBoxObjectCollection.IndexOf(null));

            Assert.Equal(0, comboBoxObjectCollection.IndexOf(comboBoxObjectCollection[0]));
            Assert.Equal(1, comboBoxObjectCollection.IndexOf(comboBoxObjectCollection[1]));
            Assert.Equal(-1, comboBoxObjectCollection.IndexOf(new Entry("a")));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_IndexOf_Object_ReturnExpected(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add(person2);

            Assert.Equal(0, comboBoxObjectCollection.IndexOf(person1));
            Assert.Equal(1, comboBoxObjectCollection.IndexOf(person2));
            Assert.Equal(-1, comboBoxObjectCollection.IndexOf(person3));
            Assert.Equal(-1, comboBoxObjectCollection.IndexOf("Name 1"));
            Assert.Equal(-1, comboBoxObjectCollection.IndexOf("Name 2"));
            Assert.Equal(-1, comboBoxObjectCollection.IndexOf(null));

            Assert.Equal(0, comboBoxObjectCollection.IndexOf(comboBoxObjectCollection[0]));
            Assert.Equal(1, comboBoxObjectCollection.IndexOf(comboBoxObjectCollection[1]));
            Assert.Equal(-1, comboBoxObjectCollection.IndexOf(new Entry(person1)));
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItem_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBoxObjectCollection[0] = "b";

            Assert.Equal("b", comboBoxObjectCollection[0]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItem_Object_Invoke_Success(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            var person1 = new Person(1, "Name 1");
            var person2 = new Person(2, "Name 2");
            var person3 = new Person(3, "Name 3");
            comboBoxObjectCollection.Add(person1);
            comboBoxObjectCollection.Add(person2);
            comboBoxObjectCollection[0] = person3;
            comboBoxObjectCollection[1] = "Name 4";

            Assert.Equal(person3, comboBoxObjectCollection[0]);
            Assert.Equal("Name 4", comboBoxObjectCollection[1]);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxObjectCollection_SetItem_DoesNotAffect_ItemsAccessibleObject()
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.CreateControl();
            ComboBox.ComboBoxAccessibleObject accessibleObject = GetComboBoxAccessibleObject(comboBox);
            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            ComboBoxItemAccessibleObject oldAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[0]);
            comboBoxObjectCollection[0] = "b";
            ComboBoxItemAccessibleObject newAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[0]);

            Assert.Equal(oldAccessibleObjectItem, newAccessibleObjectItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItem_Invoke_NullItem_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            Assert.Throws<ArgumentNullException>("value", () => comboBoxObjectCollection[0] = null);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItem_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection[-1] = 1);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItem_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection[3] = 1);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_SetItem_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            comboBoxObjectCollection.Add("a");
            comboBox.DataSource = new string[] { "b" };

            Assert.Throws<ArgumentException>(null, () => comboBoxObjectCollection[0] = 1);
            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_GetEnumerator_Invoke_Empty(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection collection = new ObjectCollection(comboBox);
            IEnumerator enumerator = collection.GetEnumerator();

            ObjectCollection comboBoxObjectCollection = new ObjectCollection(comboBox);
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // Move again.
                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // Reset.
                enumerator.Reset();
            }

            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComboBoxObjectCollection_GetEnumerator_Invoke_NotEmpty(bool createControl)
        {
            using ComboBox comboBox = new ComboBox();
            if (createControl)
            {
                comboBox.CreateControl();
            }

            ObjectCollection collection = new ObjectCollection(comboBox);
            collection.Add(2);
            IEnumerator enumerator = collection.GetEnumerator();

            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Equal(2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // Move again.
                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // Reset.
                enumerator.Reset();
            }

            Assert.Equal(createControl, comboBox.IsHandleCreated);
        }

        private ComboBox.ComboBoxAccessibleObject GetComboBoxAccessibleObject(ComboBox comboBox)
        {
            return (ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject;
        }
    }
}
