// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using static System.Windows.Forms.LinkLabel;

namespace System.Windows.Forms.Tests;

public class LinkLabel_LinkCollectionTests : IDisposable
{
    private readonly LinkLabel _linkLabel;
    private readonly LinkCollection _linkCollection;
    private readonly Link _link;

    public LinkLabel_LinkCollectionTests()
    {
        _linkLabel = new();
        _linkCollection = new(_linkLabel);
        _link = new(_linkLabel) { Name = "TestLink" };
    }

    public void Dispose()
    {
        _linkLabel.Dispose();
    }

    [WinFormsFact]
    public void LinkCollection_Index()
    {
        _linkCollection.Add(_link);

        _linkCollection[0].Should().BeSameAs(_link);
    }

    [WinFormsFact]
    public void LinkCollection_Index_Update()
    {
        Link link2 = new(_linkLabel) { Name = "TestLink2" };
        _linkCollection.Add(_link);

        _linkCollection[0] = link2;

        _linkCollection[0].Should().BeSameAs(link2);
    }

    [WinFormsFact]
    public void LinkCollection_Index_ThrowException()
    {
        _linkCollection.Add(_link);

        Action act = () => ((IList)_linkCollection)[0] = "InvalidLink";

        act.Should().Throw<ArgumentException>();
    }

    [WinFormsTheory]
    [InlineData("TestLink", true, 0)]
    [InlineData("InvalidLink", false, -1)]
    [InlineData("", false, -1)]
    [InlineData(null, false, -1)]
    public void LinkCollection_LinkOfKey_IndexOfKey(string key, bool expectedContains, int expectedIndex)
    {
        _linkCollection.Add(_link);

        if (expectedContains)
        {
            _linkCollection[key].Should().BeSameAs(_link);
        }
        else
        {
            _linkCollection[key].Should().BeNull();
        }

        _linkCollection.ContainsKey(key).Should().Be(expectedContains);
        _linkCollection.IndexOfKey(key).Should().Be(expectedIndex);
    }

    [WinFormsTheory]
    [InlineData(0, 0, false)]
    [InlineData(0, 1, true)]
    [InlineData(1, 0, false)]
    [InlineData(2, 3, true)]
    public void LinkCollection_Add(int start, int length, bool expected)
    {
        Link link = _linkCollection.Add(start, length);

        link.Should().NotBeNull();
        link.Start.Should().Be(start);
        link.Length.Should().Be(length);
        _linkCollection.LinksAdded.Should().Be(expected);
        _linkCollection.Contains(link).Should().BeTrue();
    }

    [WinFormsFact]
    public void LinkCollection_IndexOfLink()
    {
        Link validLink = _linkCollection.Add(0, 1);

        _linkCollection.IndexOf(validLink).Should().Be(0);
        _linkCollection.IndexOf(_link).Should().Be(-1);
        _linkCollection.IndexOf(null).Should().Be(-1);
    }

    [WinFormsTheory]
    [InlineData(null, true)]
    [InlineData("InvalidKey", true)]
    [InlineData("TestLink", false)]
    public void LinkCollection_RemoveByKey(string key, bool expected)
    {
        _linkCollection.Add(_link);
        _linkCollection.RemoveByKey(key);

        _linkCollection.Contains(_link).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData("Not a link", true)]
    [InlineData(null, true)]
    public void LinkCollection_IList_Remove_Value(object value, bool expected)
    {
        _linkCollection.Add(_link);
        ((IList)_linkCollection).Remove(value);

        _linkCollection.Contains(_link).Should().Be(expected);
    }
}
