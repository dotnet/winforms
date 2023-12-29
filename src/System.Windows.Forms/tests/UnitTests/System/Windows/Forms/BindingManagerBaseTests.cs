// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class BindingManagerBaseTests
{
    [Fact]
    public void Bindings_Get_ReturnsExpected()
    {
        PropertyManager manager = new();
        Assert.Empty(manager.Bindings);
        Assert.Same(manager.Bindings, manager.Bindings);
    }

    [Fact]
    public void Bindings_Add_Success()
    {
        PropertyManager manager = new();
        BindingsCollection collection = manager.Bindings;
        Binding binding = new(null, new object(), "member");

        collection.Add(binding);
        Assert.Same(binding, Assert.Single(collection));
        Assert.Same(manager, binding.BindingManagerBase);
    }

    [Fact]
    public void Bindings_AddNull_ThrowsArgumentNullException()
    {
        PropertyManager manager = new();
        BindingsCollection collection = manager.Bindings;
        Assert.Throws<ArgumentNullException>("dataBinding", () => collection.Add(null));
    }

    [Fact]
    public void Bindings_AddAlreadyInSameManager_ThrowsArgumentException()
    {
        PropertyManager manager = new();
        BindingsCollection collection = manager.Bindings;
        Binding binding = new(null, new object(), "member");

        collection.Add(binding);
        Assert.Throws<ArgumentException>("dataBinding", () => collection.Add(binding));
    }

    [Fact]
    public void Bindings_AddAlreadyInDifferentManager_ThrowsArgumentException()
    {
        PropertyManager manager1 = new();
        PropertyManager manager2 = new();
        BindingsCollection collection1 = manager1.Bindings;
        BindingsCollection collection2 = manager2.Bindings;
        Binding binding = new(null, new object(), "member");

        collection1.Add(binding);
        Assert.Throws<ArgumentException>("dataBinding", () => collection2.Add(binding));
    }

    [Fact]
    public void Bindings_Clear_Success()
    {
        PropertyManager manager = new();
        BindingsCollection collection = manager.Bindings;
        Binding binding = new(null, new object(), "member");

        collection.Add(binding);
        Assert.Same(binding, Assert.Single(collection));
        Assert.Same(manager, binding.BindingManagerBase);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(binding.BindingManagerBase);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
    }

    [Fact]
    public void Bindings_Remove_Success()
    {
        PropertyManager manager = new();
        BindingsCollection collection = manager.Bindings;
        Binding binding = new(null, new object(), "member");

        collection.Add(binding);
        Assert.Same(binding, Assert.Single(collection));
        Assert.Same(manager, binding.BindingManagerBase);

        collection.Remove(binding);
        Assert.Empty(collection);
        Assert.Null(binding.BindingManagerBase);
    }

    [Fact]
    public void Bindings_RemoveNullDataBinding_ThrowsArgumentNullException()
    {
        PropertyManager manager = new();
        BindingsCollection collection = manager.Bindings;
        Binding binding = new(null, new object(), "member");
        collection.Add(binding);

        Assert.Throws<ArgumentNullException>("dataBinding", () => collection.Remove(null));
        Assert.Same(binding, Assert.Single(collection));
    }

    [Fact]
    public void Bindings_RemoveNoSuchDataBinding_ThrowsArgumentException()
    {
        PropertyManager manager = new();
        BindingsCollection collection = manager.Bindings;
        Binding binding1 = new(null, new object(), "member");
        Binding binding2 = new(null, new object(), "member");
        collection.Add(binding1);

        Assert.Throws<ArgumentException>("dataBinding", () => collection.Remove(binding2));
        Assert.Same(binding1, Assert.Single(collection));
    }

    [Fact]
    public void Bindings_RemoveDataBindingFromOtherCollection_ThrowsArgumentException()
    {
        PropertyManager manager1 = new();
        PropertyManager manager2 = new();
        BindingsCollection collection1 = manager1.Bindings;
        BindingsCollection collection2 = manager2.Bindings;
        Binding binding1 = new(null, new object(), "member");
        Binding binding2 = new(null, new object(), "member");
        collection1.Add(binding1);
        collection2.Add(binding2);

        Assert.Throws<ArgumentException>("dataBinding", () => collection2.Remove(binding1));
        Assert.Same(binding1, Assert.Single(collection1));
        Assert.Same(binding2, Assert.Single(collection2));
    }

    [Fact]
    public void Bindings_RemoveAt_Success()
    {
        PropertyManager manager = new();
        BindingsCollection collection = manager.Bindings;
        Binding binding = new(null, new object(), "member");

        collection.Add(binding);
        Assert.Same(binding, Assert.Single(collection));
        Assert.Same(manager, binding.BindingManagerBase);

        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Null(binding.BindingManagerBase);
    }
}
