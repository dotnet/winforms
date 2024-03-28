// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class BindingsCollectionTests
{
    [Fact]
    public void Ctor_Default()
    {
        SubBindingsCollection collection = new();
        Assert.Equal(0, collection.Count);
        Assert.Empty(collection.List);
        Assert.False(collection.ShouldSerializeMyAll());
    }

    [Fact]
    public void Add_Invoke_Success()
    {
        BindingsCollection collection = [];
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        Assert.Equal(1, collection.Count);
        Assert.Same(binding, collection[0]);
        Assert.True(collection.ShouldSerializeMyAll());
    }

    [Fact]
    public void Add_InvokeWithCollectionChanging_CallsHandler()
    {
        BindingsCollection collection = [];
        Binding binding = new(null, new object(), "member");

        int changingCallCount = 0;
        int changedCallCount = 0;
        CollectionChangeEventHandler changingHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Add, e.Action);
            Assert.Same(binding, e.Element);
            changingCallCount++;
            Assert.True(changingCallCount > changedCallCount);
        };
        CollectionChangeEventHandler changedHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Add, e.Action);
            Assert.Same(binding, e.Element);
            changedCallCount++;
            Assert.Equal(changingCallCount, changedCallCount);
        };
        collection.CollectionChanging += changingHandler;
        collection.CollectionChanged += changedHandler;

        collection.Add(binding);
        Assert.Equal(1, changingCallCount);
        Assert.Equal(1, changedCallCount);
        Assert.Equal(1, collection.Count);

        // Add again.
        collection.Add(binding);
        Assert.Equal(2, changingCallCount);
        Assert.Equal(2, changedCallCount);
        Assert.Equal(2, collection.Count);

        // Remove handler.
        collection.CollectionChanging -= changingHandler;
        collection.CollectionChanged -= changedHandler;
        collection.Add(binding);
        Assert.Equal(2, changingCallCount);
        Assert.Equal(2, changedCallCount);
        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void Add_NullDataBinding_ThrowsArgumentNullException()
    {
        BindingsCollection collection = [];
        Assert.Throws<ArgumentNullException>("dataBinding", () => collection.Add(null));
    }

    [Fact]
    public void AddCore_Invoke_Success()
    {
        SubBindingsCollection collection = new();
        Binding binding = new(null, new object(), "member");
        collection.AddCore(binding);

        Assert.Equal(1, collection.Count);
        Assert.Same(binding, collection[0]);
        Assert.True(collection.ShouldSerializeMyAll());
    }

    [Fact]
    public void AddCore_InvokeWithCollectionChanging_DoesNotCallHandler()
    {
        SubBindingsCollection collection = new();
        Binding binding = new(null, new object(), "member");

        int changingCallCount = 0;
        int changedCallCount = 0;
        CollectionChangeEventHandler changingHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Add, e.Action);
            Assert.Same(binding, e.Element);
            changingCallCount++;
            Assert.True(changingCallCount > changedCallCount);
        };
        CollectionChangeEventHandler changedHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Add, e.Action);
            Assert.Same(binding, e.Element);
            changedCallCount++;
            Assert.Equal(changingCallCount, changedCallCount);
        };
        collection.CollectionChanging += changingHandler;
        collection.CollectionChanged += changedHandler;

        collection.AddCore(binding);
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Equal(1, collection.Count);

        // Add again.
        collection.AddCore(binding);
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Equal(2, collection.Count);

        // Remove handler.
        collection.CollectionChanging -= changingHandler;
        collection.CollectionChanged -= changedHandler;
        collection.AddCore(binding);
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void AddCore_NullDataBinding_ThrowsArgumentNullException()
    {
        SubBindingsCollection collection = new();
        Assert.Throws<ArgumentNullException>("dataBinding", () => collection.AddCore(null));
    }

    [Fact]
    public void Clear_Invoke_Success()
    {
        BindingsCollection collection = [];
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        collection.Clear();
        Assert.Empty(collection);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
    }

    [Fact]
    public void Clear_InvokeWithCollectionChanging_CallsHandler()
    {
        BindingsCollection collection = [];
        Binding binding = new(null, new object(), "member");

        int changingCallCount = 0;
        int changedCallCount = 0;
        CollectionChangeEventHandler changingHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Refresh, e.Action);
            Assert.Null(e.Element);
            changingCallCount++;
            Assert.True(changingCallCount > changedCallCount);
        };
        CollectionChangeEventHandler changedHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Refresh, e.Action);
            Assert.Null(e.Element);
            changedCallCount++;
            Assert.Equal(changingCallCount, changedCallCount);
        };
        collection.Add(binding);
        collection.CollectionChanging += changingHandler;
        collection.CollectionChanged += changedHandler;

        collection.Clear();
        Assert.Equal(1, changingCallCount);
        Assert.Equal(1, changedCallCount);
        Assert.Empty(collection);

        // Add again.
        collection.Clear();
        Assert.Equal(2, changingCallCount);
        Assert.Equal(2, changedCallCount);
        Assert.Empty(collection);

        // Remove handler.
        collection.CollectionChanging -= changingHandler;
        collection.CollectionChanged -= changedHandler;

        collection.Clear();
        Assert.Equal(2, changingCallCount);
        Assert.Equal(2, changedCallCount);
        Assert.Empty(collection);
    }

    [Fact]
    public void ClearCore_Invoke_Success()
    {
        SubBindingsCollection collection = new();
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        collection.ClearCore();
        Assert.Empty(collection);
    }

    [Fact]
    public void ClearCore_InvokeWithCollectionChanging_DoesNotCallHandler()
    {
        SubBindingsCollection collection = new();
        Binding binding = new(null, new object(), "member");

        int changingCallCount = 0;
        int changedCallCount = 0;
        CollectionChangeEventHandler changingHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Refresh, e.Action);
            Assert.Null(e.Element);
            changingCallCount++;
            Assert.True(changingCallCount > changedCallCount);
        };
        CollectionChangeEventHandler changedHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Refresh, e.Action);
            Assert.Null(e.Element);
            changedCallCount++;
            Assert.Equal(changingCallCount, changedCallCount);
        };
        collection.Add(binding);
        collection.CollectionChanging += changingHandler;
        collection.CollectionChanged += changedHandler;

        collection.ClearCore();
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Empty(collection);

        // Add again.
        collection.ClearCore();
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Empty(collection);

        // Remove handler.
        collection.CollectionChanging -= changingHandler;
        collection.CollectionChanged -= changedHandler;

        collection.ClearCore();
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Empty(collection);
    }

    [Fact]
    public void RemoveAt_Invoke_Success()
    {
        BindingsCollection collection = [];
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        collection.RemoveAt(0);
        Assert.Empty(collection);
    }

    [Fact]
    public void Remove_Invoke_Success()
    {
        SubBindingsCollection collection = new();
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        collection.Remove(binding);
        Assert.Empty(collection);
    }

    [Fact]
    public void Remove_InvokeWithCollectionChanging_CallsHandler()
    {
        BindingsCollection collection = [];
        Binding binding = new(null, new object(), "member");

        int changingCallCount = 0;
        int changedCallCount = 0;
        CollectionChangeEventHandler changingHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Remove, e.Action);
            Assert.Same(binding, e.Element);
            changingCallCount++;
            Assert.True(changingCallCount > changedCallCount);
        };
        CollectionChangeEventHandler changedHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Remove, e.Action);
            Assert.Same(binding, e.Element);
            changedCallCount++;
            Assert.Equal(changingCallCount, changedCallCount);
        };
        collection.Add(binding);
        collection.CollectionChanging += changingHandler;
        collection.CollectionChanged += changedHandler;

        collection.Remove(binding);
        Assert.Equal(1, changingCallCount);
        Assert.Equal(1, changedCallCount);
        Assert.Empty(collection);

        // Add again.
        collection.CollectionChanging -= changingHandler;
        collection.CollectionChanged -= changedHandler;
        collection.Add(binding);
        collection.CollectionChanging += changingHandler;
        collection.CollectionChanged += changedHandler;

        collection.Remove(binding);
        Assert.Equal(2, changingCallCount);
        Assert.Equal(2, changedCallCount);
        Assert.Empty(collection);

        // Remove handler.
        collection.CollectionChanging -= changingHandler;
        collection.CollectionChanged -= changedHandler;
        collection.Add(binding);

        collection.Remove(binding);
        Assert.Equal(2, changingCallCount);
        Assert.Equal(2, changedCallCount);
        Assert.Empty(collection);
    }

    [Fact]
    public void Remove_NullDataBinding_Nop()
    {
        BindingsCollection collection = [];
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        collection.Remove(null);
        Assert.Same(binding, Assert.Single(collection));
    }

    [Fact]
    public void Remove_NoSuchDataBinding_Nop()
    {
        BindingsCollection collection = [];
        Binding binding1 = new(null, new object(), "member");
        Binding binding2 = new(null, new object(), "member");
        collection.Add(binding1);

        collection.Remove(binding2);
        Assert.Same(binding1, Assert.Single(collection));
    }

    [Fact]
    public void Remove_DataBindingFromOtherCollection_Nop()
    {
        BindingsCollection collection1 = [];
        BindingsCollection collection2 = [];
        Binding binding1 = new(null, new object(), "member");
        Binding binding2 = new(null, new object(), "member");
        collection1.Add(binding1);
        collection2.Add(binding2);

        collection2.Remove(binding1);
        Assert.Same(binding1, Assert.Single(collection1));
        Assert.Same(binding2, Assert.Single(collection2));
    }

    [Fact]
    public void RemoveCore_Invoke_Success()
    {
        SubBindingsCollection collection = new();
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        collection.RemoveCore(binding);
        Assert.Empty(collection);
    }

    [Fact]
    public void RemoveCore_InvokeWithCollectionChanging_DoesNotCallHandler()
    {
        SubBindingsCollection collection = new();
        Binding binding = new(null, new object(), "member");

        int changingCallCount = 0;
        int changedCallCount = 0;
        CollectionChangeEventHandler changingHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Remove, e.Action);
            Assert.Same(binding, e.Element);
            changingCallCount++;
            Assert.True(changingCallCount > changedCallCount);
        };
        CollectionChangeEventHandler changedHandler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Equal(CollectionChangeAction.Remove, e.Action);
            Assert.Same(binding, e.Element);
            changedCallCount++;
            Assert.Equal(changingCallCount, changedCallCount);
        };
        collection.Add(binding);
        collection.CollectionChanging += changingHandler;
        collection.CollectionChanged += changedHandler;

        collection.RemoveCore(binding);
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Empty(collection);

        // Add again.
        collection.CollectionChanging -= changingHandler;
        collection.CollectionChanged -= changedHandler;
        collection.Add(binding);
        collection.CollectionChanging += changingHandler;
        collection.CollectionChanged += changedHandler;

        collection.RemoveCore(binding);
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Empty(collection);

        // Remove handler.
        collection.CollectionChanging -= changingHandler;
        collection.CollectionChanged -= changedHandler;
        collection.Add(binding);

        collection.RemoveCore(binding);
        Assert.Equal(0, changingCallCount);
        Assert.Equal(0, changedCallCount);
        Assert.Empty(collection);
    }

    [Fact]
    public void RemoveCore_NullDataBinding_Nop()
    {
        SubBindingsCollection collection = new();
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        collection.RemoveCore(null);
        Assert.Same(binding, Assert.Single(collection));
    }

    [Fact]
    public void RemoveCore_NoSuchDataBinding_Nop()
    {
        SubBindingsCollection collection = new();
        Binding binding1 = new(null, new object(), "member");
        Binding binding2 = new(null, new object(), "member");
        collection.Add(binding1);

        collection.RemoveCore(binding2);
        Assert.Same(binding1, Assert.Single(collection));
    }

    [Fact]
    public void RemoveCore_DataBindingFromOtherCollection_Nop()
    {
        SubBindingsCollection collection1 = new();
        SubBindingsCollection collection2 = new();
        Binding binding1 = new(null, new object(), "member");
        Binding binding2 = new(null, new object(), "member");
        collection1.Add(binding1);
        collection2.Add(binding2);

        collection2.RemoveCore(binding1);
        Assert.Same(binding1, Assert.Single(collection1));
        Assert.Same(binding2, Assert.Single(collection2));
    }

    [Fact]
    public void OnCollectionChanging_Invoke_CallsHandler()
    {
        SubBindingsCollection collection = new();
        CollectionChangeEventArgs eventArgs = new(CollectionChangeAction.Refresh, null);
        int callCount = 0;
        CollectionChangeEventHandler handler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        collection.CollectionChanging += handler;
        collection.OnCollectionChanging(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        collection.CollectionChanging -= handler;
        collection.OnCollectionChanging(eventArgs);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void OnCollectionChanged_Invoke_CallsHandler()
    {
        SubBindingsCollection collection = new();
        CollectionChangeEventArgs eventArgs = new(CollectionChangeAction.Refresh, null);
        int callCount = 0;
        CollectionChangeEventHandler handler = (sender, e) =>
        {
            Assert.Same(collection, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        collection.CollectionChanged += handler;
        collection.OnCollectionChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        collection.CollectionChanged -= handler;
        collection.OnCollectionChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    private class SubBindingsCollection : BindingsCollection
    {
        public new ArrayList List => base.List;

        public new void AddCore(Binding dataBinding) => base.AddCore(dataBinding);

        public new void ClearCore() => base.ClearCore();

        public new void RemoveCore(Binding dataBinding) => base.RemoveCore(dataBinding);

        public new void OnCollectionChanging(CollectionChangeEventArgs e) => base.OnCollectionChanging(e);

        public new void OnCollectionChanged(CollectionChangeEventArgs e) => base.OnCollectionChanged(e);
    }
}
