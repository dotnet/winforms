﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

public partial class CollectionEditor
{
    internal class PropertyGridSite : ISite
    {
        private readonly IServiceProvider? _sp;
        private bool _inGetService;

        public PropertyGridSite(IServiceProvider? sp, IComponent comp)
        {
            _sp = sp;
            Component = comp;
        }

        public IComponent Component { get; }

        public IContainer? Container => null;

        public bool DesignMode => false;

        public string? Name
        {
            get => null;
            set { }
        }

        public object? GetService(Type t)
        {
            if (!_inGetService && _sp is not null)
            {
                try
                {
                    _inGetService = true;
                    return _sp.GetService(t);
                }
                finally
                {
                    _inGetService = false;
                }
            }

            return null;
        }
    }
}
