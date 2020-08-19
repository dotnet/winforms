// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.IntegrationTests.Common;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_Get_Not_ThrowsException()
        {
            using (new NoAssertContext())
            {
                using var control = new ComboBox();

                var item1 = new HashNotImplementedObject();
                var item2 = new HashNotImplementedObject();
                var item3 = new HashNotImplementedObject();

                control.Items.AddRange(new[] { item1, item2, item3 });

                var comboBoxAccessibleObject = (ComboBox.ComboBoxAccessibleObject)control.AccessibilityObject;

                var exceptionThrown = false;

                try
                {
                    var item1AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects[item1];
                    var item2AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects[item2];
                    var item3AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects[item3];
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
                using var control = new ComboBox()
                {
                    DataSource = TestDataSources.GetPersons(),
                    DisplayMember = TestDataSources.PersonDisplayMember
                };

                ComboBox.ComboBoxAccessibleObject accessibleObject = Assert.IsType<ComboBox.ComboBoxAccessibleObject>(control.AccessibilityObject);

                foreach (Person person in TestDataSources.GetPersons())
                {
                    var item = accessibleObject.ItemAccessibleObjects[person];
                    AccessibleObject itemAccessibleObject = Assert.IsType<ComboBox.ComboBoxItemAccessibleObject>(item);
                    Assert.Equal(person.Name, itemAccessibleObject.Name);
                }
            }
        }
    }
}
