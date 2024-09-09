// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class ControlDesignerDesignerControlCollectionTests : IDisposable
{
    private readonly Control _control;
    private readonly ControlDesigner.DesignerControlCollection _collection;

    public void Dispose() => _collection.Clear();

    public ControlDesignerDesignerControlCollectionTests()
    {
        _control = new();
        _collection = new ControlDesigner.DesignerControlCollection(_control);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenControlIsNull()
    {
        Action act = () => new ControlDesigner.DesignerControlCollection(null);
        act.Should().Throw<ArgumentNullException>();
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
        Control controlToAdd = new();
        ((IList)_collection).Add(controlToAdd);
        _control.Controls.Cast<Control>().Should().Contain(controlToAdd);
    }

    [Fact]
    public void AddRange_ShouldAddMultipleControlsToCollection()
    {
        Control[] controlsToAdd = { new(), new() };
        _collection.AddRange(controlsToAdd);
        _control.Controls.Cast<Control>().Should().Contain(controlsToAdd);
    }

    [Fact]
    public void Contains_ShouldReturnTrueIfControlExists()
    {
        Control controlToAdd = new();
        ((IList)_collection).Add(controlToAdd);
        ((IList)_collection).Contains(controlToAdd).Should().BeTrue();
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
        Control controlToAdd = new();
        ((IList)_collection).Add(controlToAdd);
        ((IList)_collection).IndexOf(controlToAdd).Should().Be(_control.Controls.IndexOf(controlToAdd));
    }

    [Fact]
    public void Insert_ShouldInsertControlAtCorrectIndex()
    {
        Control controlToInsert = new();
        Control anotherControl = new();
        ((IList)_collection).Add(anotherControl);
        ((IList)_collection).Add(controlToInsert);
        _collection.SetChildIndex(controlToInsert, 0);
        _control.Controls[0].Should().Be(controlToInsert);
    }

    [Fact]
    public void Remove_ShouldRemoveControlFromCollection()
    {
        Control controlToRemove = new();
        ((IList)_collection).Add(controlToRemove);
        ((IList)_collection).Remove(controlToRemove);
        _control.Controls.Contains(controlToRemove).Should().BeFalse();
    }

    [Fact]
    public void RemoveAt_ShouldRemoveControlAtSpecifiedIndex()
    {
        Control controlToRemove = new();
        ((IList)_collection).Add(controlToRemove);
        ((IList)_collection).RemoveAt(0);
        _control.Controls.Contains(controlToRemove).Should().BeFalse();
    }

    [Fact]
    public void GetChildIndex_ShouldReturnCorrectIndex()
    {
        Control child = new();
        _collection.Add(child);
        _collection.GetChildIndex(child, false).Should().Be(_control.Controls.GetChildIndex(child, false));
    }

    [Fact]
    public void SetChildIndex_ShouldSetCorrectIndex()
    {
        Control child = new();
        _collection.Add(child);
        _collection.SetChildIndex(child, 0);
        _control.Controls.GetChildIndex(child).Should().Be(0);
    }

    [Fact]
    public void Clear_ShouldRemoveAllControlsFromCollection()
    {
        Control child = new();
        child.Site = new DummySite { Component = child, DesignMode = true };
        _collection.Add(child);
        _collection.Clear();
        _control.Controls.Contains(child).Should().BeFalse();
    }

    private class DummySite : ISite
    {
        public IComponent Component { get; set; }
        public IContainer Container { get; set; }
        public bool DesignMode { get; set; }
        public string Name { get; set; }
        public object GetService(Type serviceType) => null;
    }
}
