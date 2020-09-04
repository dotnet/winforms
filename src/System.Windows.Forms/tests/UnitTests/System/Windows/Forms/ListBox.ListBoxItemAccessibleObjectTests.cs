// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.IntegrationTests.Common;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListBox_ListBoxItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListBoxItemAccessibleObject_DataBoundAccessibleName()
        {
            // Regression test for https://github.com/dotnet/winforms/issues/3706

            using var form = new Form
            {
                BindingContext = new BindingContext()
            };

            using var control = new ListBox
            {
                Parent = form,
                DisplayMember = TestDataSources.PersonDisplayMember,
                DataSource = TestDataSources.GetPersons()
            };

            ListBox.ListBoxAccessibleObject accessibleObject =
                Assert.IsType<ListBox.ListBoxAccessibleObject>(control.AccessibilityObject);

            Collections.Generic.List<Person> persons = TestDataSources.GetPersons();
            Assert.Equal(persons.Count, accessibleObject.GetChildCount());

            for (int i = 0; i < persons.Count; i++)
            {
                Person person = persons[i];
                AccessibleObject itemAccessibleObject = accessibleObject.GetChild(i);

                Assert.IsType<ListBox.ListBoxItemAccessibleObject>(itemAccessibleObject);
                Assert.Equal(person.Name, itemAccessibleObject.Name);
            }

            Assert.False(control.IsHandleCreated);
        }
    }
}
