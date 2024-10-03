// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class ControlDesignerDesignerControlCollectionTests : IDisposable
{
    private readonly Control _control;
    private readonly ControlDesigner.DesignerControlCollection _collection;

    public ControlDesignerDesignerControlCollectionTests()
    {
        _control = new();
        _collection = new(_control);
    }

    public void Dispose()
    {
        _collection.Clear();
        _control.Dispose();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenControlIsNull()
    {
        Action action = () => new ControlDesigner.DesignerControlCollection(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SyncRoot_ShouldReturnSelf()
    {
        ((ICollection)_collection).SyncRoot.Should().Be(_collection);
    }

    [Fact]
    public void IsSynchronized_ShouldReturnFalse()
    {
        ((ICollection)_collection).IsSynchronized.Should().BeFalse();
    }

    [Fact]
    public void IsFixedSize_ShouldReturnFalse()
    {
        ((IList)_collection).IsFixedSize.Should().BeFalse();
    }

    [Fact]
    public void IsReadOnly_ShouldMatchControlIsReadOnly()
    {
        _collection.IsReadOnly.Should().Be(_control.Controls.IsReadOnly);
    }

    [Fact]
    public void Add_ShouldAddControlToCollection()
    {
        using Control control = new();
        ((IList)_collection).Add(control);
        _control.Controls.Cast<Control>().Should().Contain(control);
    }

    [Fact]
    public void AddRange_ShouldAddMultipleControlsToCollection()
    {
        using Control control1 = new();
        using Control control2 = new();
        Control[] controls = [control1, control2];
        _collection.AddRange(controls);
        _control.Controls.Cast<Control>().Should().Contain(controls);
    }

    [Fact]
    public void Contains_ShouldReturnTrueIfControlExists()
    {
        using Control control = new();
        ((IList)_collection).Add(control);
        ((IList)_collection).Contains(control).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnTrueWhenCollectionsAreEqual()
    {
        var other = _control.Controls;
        _collection.Equals(other).Should().BeTrue();
    }

    [Fact]
    public void GetEnumerator_ShouldReturnCorrectEnumerator()
    {
        _collection.GetEnumerator().Should().BeEquivalentTo(_control.Controls.GetEnumerator());
    }

    [Fact]
    public void GetHashCode_ShouldMatchControlHashCode()
    {
        _collection.GetHashCode().Should().Be(_control.Controls.GetHashCode());
    }

    [Fact]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        using Control control = new();
        ((IList)_collection).Add(control);
        ((IList)_collection).IndexOf(control).Should().Be(_control.Controls.IndexOf(control));
    }

    [Fact]
    public void Insert_ShouldInsertControlAtCorrectIndex()
    {
        using Control control = new();
        using Control anotherControl = new();
        ((IList)_collection).Add(anotherControl);
        ((IList)_collection).Add(control);
        _collection.SetChildIndex(control, 0);
        _control.Controls[0].Should().Be(control);
    }

    [Fact]
    public void Remove_ShouldRemoveControlFromCollection()
    {
        using Control control = new();
        ((IList)_collection).Add(control);
        ((IList)_collection).Remove(control);
        _control.Controls.Contains(control).Should().BeFalse();
    }

    [Fact]
    public void RemoveAt_ShouldRemoveControlAtSpecifiedIndex()
    {
        using Control control = new();
        ((IList)_collection).Add(control);
        ((IList)_collection).RemoveAt(0);
        _control.Controls.Contains(control).Should().BeFalse();
    }

    [Fact]
    public void GetChildIndex_ShouldReturnCorrectIndex()
    {
        using Control control = new();
        _collection.Add(control);
        _collection.GetChildIndex(control, throwException: false).Should().Be(_control.Controls.GetChildIndex(control, throwException: false));
    }

    [Fact]
    public void SetChildIndex_ShouldSetCorrectIndex()
    {
        using Control control = new();
        _collection.Add(control);
        _collection.SetChildIndex(control, 0);
        _control.Controls.GetChildIndex(control).Should().Be(0);
    }

    [Fact]
    public void Clear_ShouldRemoveAllControlsFromCollection()
    {
        using Control control = new();
        control.Site = new MockSite(control, designMode: true);
        _collection.Add(control);
        _control.Controls.Contains(control).Should().BeTrue();

        _collection.Clear();
        _control.Controls.Contains(control).Should().BeFalse();
    }

    private class MockSite : ISite, IDisposable
    {
        public IComponent Component { get; }
        public IContainer Container { get; init; } = new Container();
        public bool DesignMode { get; }
        public string? Name { get; set; }

        public MockSite(IComponent component, bool designMode)
        {
            Component = component;
            DesignMode = designMode;
        }

        public object? GetService(Type serviceType) => null;

        public void Dispose() => Container?.Dispose();
    }
}
