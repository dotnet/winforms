// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Windows.Forms.IntegrationTests.Common;
using static System.Windows.Forms.ComboBox;
using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms.Tests;

public class ComboBox_ComboBoxObjectCollectionTests
{
    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Add_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

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

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentException>(() => comboBoxObjectCollection.Add("a"));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Sorted_Add_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.Sorted = true;
        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "b",
            "c",
            "a"
        };

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
        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");

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
        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Person person1 = new(1, "Name 1");
        Person person2 = new(3, "Name 3");

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.Sorted = true;
        ObjectCollection comboBoxObjectCollection = new(comboBox);

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");

        ObjectCollection comboBoxObjectCollection = new(comboBox);
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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.Sorted = true;
        ObjectCollection comboBoxObjectCollection = new(comboBox);

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
        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");

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
        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Person person1 = new(1, "Name 1");
        Person person2 = new(3, "Name 3");

        comboBoxObjectCollection.AddRange(["Name 2", person1, person2]);

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
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

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentException>(() => comboBoxObjectCollection.AddRange(new string[] { "a", "b" }));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Clear_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
        comboBoxObjectCollection.AddRange(new string[] { "c", "a", "b" });

        Assert.Equal(3, comboBoxObjectCollection.Count);
        Assert.Equal(3, comboBoxObjectCollection.InnerList.Count);

        comboBoxObjectCollection.Clear();

        Assert.Empty(comboBoxObjectCollection);
        Assert.Empty(comboBoxObjectCollection.InnerList);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxObjectCollection_Clear_Invoke_ClearItemAccessibleObjects()
    {
        using ComboBox comboBox = new();
        comboBox.CreateControl();
        ObjectCollection comboBoxObjectCollection = comboBox.Items;
        comboBoxObjectCollection.AddRange(new string[] { "a", "b" });
        ComboBoxAccessibleObject accessibleObject = GetComboBoxAccessibleObject(comboBox);

        // Adding ComboBoxItemAccessibleObject to the "ItemAccessibleObjects" list
        ComboBoxItemAccessibleObject firstAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[0]);
        ComboBoxItemAccessibleObject secondAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[1]);

        Assert.Equal(2, comboBoxObjectCollection.InnerList.Count);
        Assert.Equal(2, accessibleObject.ItemAccessibleObjects.Count);

        comboBoxObjectCollection.Clear();

        Assert.Empty(comboBoxObjectCollection);
        Assert.Empty(accessibleObject.ItemAccessibleObjects);
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

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentException>(comboBoxObjectCollection.Clear);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Contains_Invoke_ReturnExpected(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");

        comboBoxObjectCollection.AddRange([person1, person2]);

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentNullException>("destination", () => comboBoxObjectCollection.CopyTo(null, 1));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_CopyTo_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentOutOfRangeException>("arrayIndex", () => comboBoxObjectCollection.CopyTo(new object[1], -1));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_CopyTo_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentOutOfRangeException>("arrayIndex", () => comboBoxObjectCollection.CopyTo(new object[1], 3));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_CopyTo_InvokeEmpty_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
        object[] array = ["1", "2", "3"];

        comboBoxObjectCollection.CopyTo(array, 1);

        Assert.Equal(["1", "2", "3"], array);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_CopyTo_InvokeNotEmpty_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            1,
            2
        };
        object[] array = ["1", "2", "3"];

        comboBoxObjectCollection.CopyTo(array, 1);

        Assert.Equal(["1", 1, 2], array);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Insert_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            person1,
            person2
        };

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.Sorted = true;
        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };

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
        ObjectCollection comboBoxObjectCollection = new(comboBox);
        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");
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
        ObjectCollection comboBoxObjectCollection = new(comboBox);
        Person person1 = new(1, "Name 1");
        Person person2 = new(3, "Name 3");
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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentNullException>("item", () => comboBoxObjectCollection.Insert(0, null));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Insert_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.Insert(-1, 1));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Insert_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

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

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentException>(() => comboBoxObjectCollection.Insert(3, 1));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_RemoveAt_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };

        comboBoxObjectCollection.RemoveAt(0);

        Assert.Single(comboBoxObjectCollection);
        Assert.Equal("b", comboBoxObjectCollection[0]);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxObjectCollection_RemoveAt_Invoke_RemoveItemAccessibleObject()
    {
        using ComboBox comboBox = new();
        comboBox.CreateControl();

        comboBox.CreateControl();
        ObjectCollection comboBoxObjectCollection = comboBox.Items;
        comboBoxObjectCollection.Add("a");
        ComboBoxAccessibleObject accessibleObject = GetComboBoxAccessibleObject(comboBox);

        // Adding ComboBoxItemAccessibleObject to the "ItemAccessibleObjects" list
        ComboBoxItemAccessibleObject firstAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[0]);
        Assert.Single(accessibleObject.ItemAccessibleObjects);

        comboBoxObjectCollection.RemoveAt(0);

        Assert.Empty(comboBoxObjectCollection);
        Assert.Empty(accessibleObject.ItemAccessibleObjects);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_RemoveAt_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.RemoveAt(-1));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_RemoveAt_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

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

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentException>(() => comboBoxObjectCollection.RemoveAt(0));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Remove_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };

        comboBoxObjectCollection.Remove("a");

        Assert.Single(comboBoxObjectCollection);
        Assert.Equal("b", comboBoxObjectCollection[0]);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Remove_Invoke_NonExistingItem_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };

        comboBoxObjectCollection.Remove("c");

        Assert.Equal(2, comboBoxObjectCollection.Count);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Remove_Object_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");
        comboBoxObjectCollection.Add(person1);
        comboBoxObjectCollection.Add(person2);

        // Remove non-existing object
        comboBoxObjectCollection.Remove(person3);

        Assert.Equal(2, comboBoxObjectCollection.Count);

        // Remove existing object
        comboBoxObjectCollection.Remove(person1);

        Assert.Single(comboBoxObjectCollection);
        Assert.Equal(person2, comboBoxObjectCollection[0]);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Remove_Entry_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };

        comboBoxObjectCollection.Remove(comboBoxObjectCollection[0]);

        Assert.Single(comboBoxObjectCollection);
        Assert.Equal("b", comboBoxObjectCollection[0]);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxObjectCollection_Remove_Invoke_RemoveItemAccessibleObject()
    {
        using ComboBox comboBox = new();
        comboBox.CreateControl();

        comboBox.CreateControl();
        ObjectCollection comboBoxObjectCollection = comboBox.Items;
        comboBoxObjectCollection.Add("a");
        ComboBoxAccessibleObject accessibleObject = GetComboBoxAccessibleObject(comboBox);

        // Adding ComboBoxItemAccessibleObject to the "ItemAccessibleObjects" list
        ComboBoxItemAccessibleObject firstAccessibleObjectItem = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBoxObjectCollection.InnerList[0]);
        Assert.Single(accessibleObject.ItemAccessibleObjects);

        comboBoxObjectCollection.Remove("a");

        Assert.Empty(comboBoxObjectCollection);
        Assert.Empty(accessibleObject.ItemAccessibleObjects);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_Remove_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a"
        };
        comboBox.DataSource = new string[] { "b" };

        Assert.Throws<ArgumentException>(() => comboBoxObjectCollection.Remove("a"));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_SetItemInternal_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };
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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");
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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a"
        };

        Assert.Throws<ArgumentNullException>("value", () => comboBoxObjectCollection.SetItemInternal(0, null));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_SetItemInternal_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.SetItemInternal(-1, 1));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_SetItemInternal_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection.SetItemInternal(3, 1));
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_IndexOf_ReturnExpected(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a",
            "b"
        };

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");
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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a"
        };
        comboBoxObjectCollection[0] = "b";

        Assert.Equal("b", comboBoxObjectCollection[0]);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_SetItem_Object_Invoke_Success(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);
        Person person1 = new(1, "Name 1");
        Person person2 = new(2, "Name 2");
        Person person3 = new(3, "Name 3");
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
        using ComboBox comboBox = new();
        comboBox.CreateControl();
        ComboBoxAccessibleObject accessibleObject = GetComboBoxAccessibleObject(comboBox);
        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a"
        };

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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a"
        };
        Assert.Throws<ArgumentNullException>("value", () => comboBoxObjectCollection[0] = null);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_SetItem_Invoke_NegativeIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection[-1] = 1);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_SetItem_Invoke_TooMoreIndex_ThrowsArgumentNullException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => comboBoxObjectCollection[3] = 1);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_SetItem_Invoke_DataSourceExists_ThrowsArgumentException(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection comboBoxObjectCollection = new(comboBox)
        {
            "a"
        };
        comboBox.DataSource = new string[] { "b" };

        Assert.Throws<ArgumentException>(() => comboBoxObjectCollection[0] = 1);
        Assert.Equal(createControl, comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComboBoxObjectCollection_GetEnumerator_Invoke_Empty(bool createControl)
    {
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection collection = new(comboBox);
        IEnumerator enumerator = collection.GetEnumerator();

        ObjectCollection comboBoxObjectCollection = new(comboBox);
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
        using ComboBox comboBox = new();
        if (createControl)
        {
            comboBox.CreateControl();
        }

        ObjectCollection collection = new(comboBox)
        {
            2
        };
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

    [WinFormsFact]
    public void ComboBox_Item_RemoveAt_DoesNotCreateAccessibilityObject()
    {
        using ComboBox comboBox = new();

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.Items.Remove(1);

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_Item_Insert_DoesNotCreateAccessibilityObject()
    {
        using ComboBox comboBox = new();

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.Items.Insert(1, "Item3");

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBox_Item_Clear_DoesNotCreateAccessibilityObject()
    {
        using ComboBox comboBox = new();

        comboBox.Items.Add("item1");
        comboBox.Items.Add("item2");
        comboBox.Items.Clear();

        Assert.False(comboBox.IsAccessibilityObjectCreated);
        Assert.False(comboBox.IsHandleCreated);
    }

    private static ComboBoxAccessibleObject GetComboBoxAccessibleObject(ComboBox comboBox)
    {
        return (ComboBoxAccessibleObject)comboBox.AccessibilityObject;
    }
}
