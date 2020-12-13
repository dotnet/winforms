// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.IntegrationTests.Common;
using Xunit;
using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_Get_Not_ThrowsException()
        {
            using (new NoAssertContext())
            {
                using ComboBox control = new ComboBox();

                HashNotImplementedObject item1 = new();
                HashNotImplementedObject item2 = new();
                HashNotImplementedObject item3 = new();

                control.Items.AddRange(new[] { item1, item2, item3 });

                ComboBox.ComboBoxAccessibleObject comboBoxAccessibleObject = (ComboBox.ComboBoxAccessibleObject)control.AccessibilityObject;

                bool exceptionThrown = false;

                try
                {
                    ComboBox.ComboBoxItemAccessibleObject item1AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(control.Items.InnerList[0]);
                    ComboBox.ComboBoxItemAccessibleObject item2AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(control.Items.InnerList[1]);
                    ComboBox.ComboBoxItemAccessibleObject item3AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(control.Items.InnerList[2]);
                }
                catch
                {
                    exceptionThrown = true;
                }

                Assert.False(exceptionThrown, "Getting accessible object for ComboBox item has thrown an exception.");
            }
        }

        public class HashNotImplementedObject
        {
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_DataBoundAccessibleName()
        {
            using (new NoAssertContext())
            {
                // Regression test for https://github.com/dotnet/winforms/issues/3549
                using ComboBox control = new ComboBox()
                {
                    DataSource = TestDataSources.GetPersons(),
                    DisplayMember = TestDataSources.PersonDisplayMember
                };

                ComboBox.ComboBoxAccessibleObject accessibleObject = Assert.IsType<ComboBox.ComboBoxAccessibleObject>(control.AccessibilityObject);

                foreach (Person person in TestDataSources.GetPersons())
                {
                    ComboBox.ComboBoxItemAccessibleObject item = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(new Entry(person));
                    AccessibleObject itemAccessibleObject = Assert.IsType<ComboBox.ComboBoxItemAccessibleObject>(item);
                    Assert.Equal(person.Name, itemAccessibleObject.Name);
                }
            }
        }

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_SeveralSameItems_FragmentNavigate_NextSibling_ReturnExpected()
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox();
                comboBox.CreateControl();

                comboBox.Items.AddRange(new[] { "aaa", "aaa", "aaa" });
                ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
                Assert.Equal("aaa", comboBoxItem1.Name);

                // FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling) should return accessible object for second "aaa" item
                ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
                Assert.NotEqual(comboBoxItem1, comboBoxItem2);
                Assert.Equal("aaa", comboBoxItem2.Name);

                // FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling) should return accessible object for third "aaa" item
                ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
                Assert.NotEqual(comboBoxItem3, comboBoxItem2);
                Assert.NotEqual(comboBoxItem3, comboBoxItem1);
                Assert.Equal("aaa", comboBoxItem3.Name);

                Assert.True(comboBox.IsHandleCreated);
            }
        }

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_SeveralSameItems_FragmentNavigate_PreviousSibling_ReturnExpected()
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox();
                comboBox.CreateControl();

                comboBox.Items.AddRange(new[] { "aaa", "aaa", "aaa" });
                ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild);

                // FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling) should return accessible object for second "aaa" item
                ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem3
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling);
                Assert.NotEqual(comboBoxItem2, comboBoxItem3);
                Assert.Equal("aaa", comboBoxItem2.Name);

                // FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling) should return accessible object for first "aaa" item
                ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling);
                Assert.NotEqual(comboBoxItem1, comboBoxItem2);
                Assert.NotEqual(comboBoxItem1, comboBoxItem3);
                Assert.Equal("aaa", comboBoxItem1.Name);

                // FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling) should return null for first "aaa" item
                ComboBox.ComboBoxItemAccessibleObject comboBoxItemPrevious = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling);
                Assert.Null(comboBoxItemPrevious);
                Assert.True(comboBox.IsHandleCreated);
            }
        }
    }
}
