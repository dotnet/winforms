// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.LinkLabel.Link;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class LinkLabel_Link_LinkAccessibleObjectTests
    {
        [WinFormsFact]
        public void LinkAccessibleObject_Ctor_OwnerLinkCannotBeNull()
        {
            using LinkLabel linkLabel = new();

            Assert.Throws<ArgumentNullException>(() => new LinkAccessibleObject(link: null, linkLabel));
        }

        [WinFormsFact]
        public void LinkAccessibleObject_Ctor_OwnerLinkLabelCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LinkAccessibleObject(new Link(), owner: null));
        }

        [WinFormsFact]
        public void LinkAccessibleObject_CurrentIndex_IsExpected()
        {
            using LinkLabel linkLabel = new();

            for (int index = 0; index < 4; index++)
            {
                linkLabel.Links.Add(new());
            }

            for (int index = 0; index < 4; index++)
            {
                LinkAccessibleObject linkAccessibleObject = linkLabel.Links[index].AccessibleObject;
                int actual = linkAccessibleObject.TestAccessor().Dynamic.CurrentIndex;

                Assert.Equal(index, actual);
            }

            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("TestDescription")]
        [InlineData(null)]
        public void LinkAccessibleObject_Description_IsExpected(string description)
        {
            using LinkLabel linkLabel = new();

            Link link = linkLabel.Links[0];
            link.Description = description;

            Assert.Equal(description, link.AccessibleObject.Description);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_FragmentNavigate_NextSibling_IsExpected()
        {
            using LinkLabel linkLabel = new();

            for (int index = 0; index < 4; index++)
            {
                linkLabel.Links.Add(new());
            }

            AccessibleObject linkLabelAccessibleObject1 = linkLabel.Links[0].AccessibleObject;
            AccessibleObject linkLabelAccessibleObject2 = linkLabel.Links[1].AccessibleObject;
            AccessibleObject linkLabelAccessibleObject3 = linkLabel.Links[2].AccessibleObject;
            AccessibleObject linkLabelAccessibleObject4 = linkLabel.Links[3].AccessibleObject;

            Assert.Equal(linkLabelAccessibleObject2, linkLabelAccessibleObject1.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(linkLabelAccessibleObject3, linkLabelAccessibleObject2.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(linkLabelAccessibleObject4, linkLabelAccessibleObject3.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(linkLabelAccessibleObject4.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_FragmentNavigate_PreviousSibling_IsExpected()
        {
            using LinkLabel linkLabel = new();

            for (int index = 0; index < 4; index++)
            {
                linkLabel.Links.Add(new());
            }

            AccessibleObject linkLabelAccessibleObject1 = linkLabel.Links[0].AccessibleObject;
            AccessibleObject linkLabelAccessibleObject2 = linkLabel.Links[1].AccessibleObject;
            AccessibleObject linkLabelAccessibleObject3 = linkLabel.Links[2].AccessibleObject;
            AccessibleObject linkLabelAccessibleObject4 = linkLabel.Links[3].AccessibleObject;

            Assert.Null(linkLabelAccessibleObject1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(linkLabelAccessibleObject1, linkLabelAccessibleObject2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(linkLabelAccessibleObject2, linkLabelAccessibleObject3.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(linkLabelAccessibleObject3, linkLabelAccessibleObject4.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void LinkAccessibleObject_FragmentNavigate_Parent_IsExpected(int linkIndex)
        {
            using LinkLabel linkLabel = new();

            AccessibleObject expected = linkLabel.AccessibilityObject;

            for (int index = 0; index < 4; index++)
            {
                linkLabel.Links.Add(new());
            }

            LinkAccessibleObject linkAccessibleObject = linkLabel.Links[linkIndex].AccessibleObject;
            UiaCore.IRawElementProviderFragment actual = linkAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);

            Assert.Equal(expected, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_Parent_IsExpected_IfNoOneLinkWasAdded()
        {
            using LinkLabel linkLabel = new();
            LinkAccessibleObject linkAccessibleObject = linkLabel.Links[0].AccessibleObject;

            AccessibleObject expected = linkLabel.AccessibilityObject;
            AccessibleObject actual = linkAccessibleObject.Parent;

            Assert.Equal(linkLabel.AccessibilityObject, linkAccessibleObject.Parent);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_Role_IsLink()
        {
            using LinkLabel linkLabel = new();
            LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

            AccessibleRole actual = accessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(((int)UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(((int)UiaCore.UIA.IsInvokePatternAvailablePropertyId))]
        public void LinkAccessibleObject_GetPropertyValue_IsPatternSupported(int propertyId)
        {
            using LinkLabel linkLabel = new();
            LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

            bool actual = (bool)accessibleObject.GetPropertyValue((UiaCore.UIA)propertyId);

            Assert.True(actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_GetPropertyValue_RuntimeIdNotNull()
        {
            using LinkLabel linkLabel = new();
            LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.RuntimeIdPropertyId);

            Assert.NotNull(actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_GetPropertyValue_RuntimeId_ReturnsExpected()
        {
            using LinkLabel linkLabel = new();
            LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

            object actual = accessibleObject.GetPropertyValue(UIA.RuntimeIdPropertyId);

            Assert.Equal(accessibleObject.RuntimeId, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_GetPropertyValue_Name_IsExpected_ForOneLink()
        {
            using LinkLabel linkLabel = new();
            string testName = "TestNameLink";
            linkLabel.Text = testName;

            LinkAccessibleObject linkAccessibleObject = linkLabel.Links[0].AccessibleObject;
            object actual = linkAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);

            Assert.Equal(testName, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_Name_IsExpected_ForSeveralLinks()
        {
            using LinkLabel linkLabel = new();
            string[] names = { "Home", "About", "Help", "Details" };
            linkLabel.Text = string.Join(' ', names);
            int start = 0;

            foreach (string name in names)
            {
                linkLabel.Links.Add(new(start, name.Length));
                start += name.Length + 1;
            }

            for (int index = 0; index < linkLabel.Links.Count; index++)
            {
                string actual = linkLabel.Links[index].AccessibleObject.Name;

                Assert.Equal(names[index], actual);
            }

            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_GetPropertyValue_HelpText_ReturnsExpected()
        {
            using LinkLabel linkLabel = new();
            LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

            object actual = accessibleObject.GetPropertyValue(UIA.HelpTextPropertyId);

            Assert.Equal(accessibleObject.Help ?? string.Empty, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkAccessibleObject_GetPropertyValue_IsOffscreen_ReturnsFalse()
        {
            using LinkLabel linkLabel = new();
            LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

            var actual = (bool)accessibleObject.GetPropertyValue(UIA.IsOffscreenPropertyId);

            Assert.False(actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(false, ((int)UIA.IsExpandCollapsePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsGridItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsGridPatternAvailablePropertyId))]
        [InlineData(true, ((int)UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsMultipleViewPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsScrollItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsScrollPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsSelectionItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsSelectionPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTableItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTablePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTextPattern2AvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTextPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTogglePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsValuePatternAvailablePropertyId))]
        public void LinkAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
        {
            using LinkLabel linkLabel = new();
            LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

            Assert.Equal(expected, accessibleObject.GetPropertyValue((UIA)propertyId) ?? false);
            Assert.False(linkLabel.IsHandleCreated);
        }
    }
}
