// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using static System.Windows.Forms.LinkLabel;

namespace System.Windows.Forms.Tests;
public class LinkCollectionTest
{
    public class LinkCollectionTests : IDisposable
    {
        private readonly LinkLabel _linkLabel;
        private readonly LinkLabel.LinkCollection _linkCollection;

        public LinkCollectionTests()
        {
            _linkLabel = new();
            _linkCollection = new(_linkLabel);
        }

        public void Dispose()
        {
            _linkLabel.Dispose();
        }

        [WinFormsFact]
        public void LinkCollection_Index()
        {
            Link link = new Link(_linkLabel) { Name = "TestLink1" };
            _linkCollection.Add(link);

            _linkCollection[0].Should().BeSameAs(link);
        }

        [WinFormsFact]
        public void LinkCollection_Index_Update()
        {
            Link link1 = new Link(_linkLabel) { Name = "TestLink1" };
            Link link2 = new Link(_linkLabel) { Name = "TestLink2" };
            _linkCollection.Add(link1);

            _linkCollection[0] = link2;

            _linkCollection[0].Should().BeSameAs(link2);
        }

        [WinFormsFact]
        public void LinkCollection_Index_ThrowException()
        {
            Link link = new Link(_linkLabel) { Name = "TestLink" };
            _linkCollection.Add(link);

            Action act = () => ((IList)_linkCollection)[0] = "InvalidLink";

            act.Should().Throw<ArgumentException>();
        }

        [WinFormsTheory]
        [InlineData("TestLink", true)]
        [InlineData("InvalidLink", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void LinkCollection_LinkOfKey(string key, bool expectedResult)
        {
            Link link = new Link(_linkLabel) { Name = "TestLink" };
            _linkCollection.Add(link);

            if (expectedResult)
            {
                _linkCollection[key].Should().BeSameAs(link);
            }
            else
            {
                _linkCollection[key].Should().BeNull();
            }

            _linkCollection.ContainsKey(key).Should().Be(expectedResult);
        }

        [WinFormsTheory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, true)]
        [InlineData(1, 0, false)]
        [InlineData(2, 3, true)]
        public void LinkCollection_Add(int start, int length, bool expectedResult)
        {
            Link link = _linkCollection.Add(start, length);

            link.Should().NotBeNull();
            link.Start.Should().Be(start);
            link.Length.Should().Be(length);
            _linkCollection.LinksAdded.Should().Be(expectedResult);
            _linkCollection.Contains(link).Should().BeTrue();
        }

        [WinFormsFact]
        public void LinkCollection_IndexOf()
        {
            Link validLink = _linkCollection.Add(0, 1);
            Link invalidLink = new Link(_linkLabel);

            _linkCollection.IndexOf(validLink).Should().BeGreaterThanOrEqualTo(0);
            _linkCollection.IndexOf(invalidLink).Should().Be(-1);
            _linkCollection.IndexOf(null).Should().Be(-1);
        }

        [WinFormsTheory]
        [InlineData("TestLink", 0)]
        [InlineData("InvalidLink", -1)]
        [InlineData(null, -1)]
        [InlineData("", -1)]
        public void LinkCollection_IndexOfKey(string key, int expectedResult)
        {
            Link link = new Link(_linkLabel) { Name = "TestLink" };
            _linkCollection.Add(link);

            _linkCollection.IndexOfKey(key).Should().Be(expectedResult);
        }

        [WinFormsFact]
        public void LinkCollection_RemoveByKey_KeyIsNull()
        {
            Link link = new Link(_linkLabel) { Name = "TestLink" };

            _linkCollection.Add(link);
            _linkCollection.RemoveByKey(null);

            _linkCollection.Contains(link).Should().BeTrue();
        }

        [WinFormsFact]
        public void LinkCollection_RemoveByKey_KeyIsInvalid()
        {
            Link link = new Link(_linkLabel) { Name = "TestLink" };

            _linkCollection.Add(link);
            _linkCollection.RemoveByKey("InvalidKey");

            _linkCollection.Contains(link).Should().BeTrue();
        }

        [WinFormsFact]
        public void LinkCollection_RemoveByKey_KeyIsValid()
        {
            Link link = new Link(_linkLabel) { Name = "TestLink" };

            _linkCollection.Add(link);
            _linkCollection.RemoveByKey("TestLink");

            _linkCollection.Contains(link).Should().BeFalse();
        }

        [WinFormsFact]
        public void LinkCollection_IList_ValueIsNotLink()
        {
            Link link = new Link(_linkLabel);
            _linkCollection.Add(link);
            ((IList)_linkCollection).Remove("Not a link");

            _linkCollection.Contains(link).Should().BeTrue();
        }

        [WinFormsFact]
        public void LinkCollection_IList_ValueIsLink()
        {
            Link link = new Link(_linkLabel);
            _linkCollection.Add(link);
            ((IList)_linkCollection).Remove(link);

            _linkCollection.Contains(link).Should().BeFalse();
        }
    }
}

