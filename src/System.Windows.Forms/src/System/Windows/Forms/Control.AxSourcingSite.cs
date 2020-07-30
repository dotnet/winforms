// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        private class AxSourcingSite : ISite
        {
            private readonly Ole32.IOleClientSite _clientSite;
            private string _name;
            private HtmlShimManager _shimManager;

            internal AxSourcingSite(IComponent component, Ole32.IOleClientSite clientSite, string name)
            {
                Component = component;
                _clientSite = clientSite;
                _name = name;
            }

            /// <summary>
            ///  The component sited by this component site.
            /// </summary>
            public IComponent Component { get; }

            /// <summary>
            ///  The container in which the component is sited.
            /// </summary>
            public IContainer Container => null;

            public object GetService(Type service)
            {
                if (service == typeof(HtmlDocument))
                {
                    if (_clientSite.GetContainer() is Mshtml.IHTMLDocument document)
                    {
                        _shimManager ??= new HtmlShimManager();
                        return new HtmlDocument(_shimManager, document);
                    }
                }
                else if (_clientSite.GetType().IsAssignableFrom(service))
                {
                    return _clientSite;
                }

                return null;
            }

            /// <summary>
            ///  Indicates whether the component is in design mode.
            /// </summary>
            public bool DesignMode => false;

            /// <summary>
            ///  The name of the component.
            /// </summary>
            public string Name
            {
                get => _name;
                set
                {
                    if (value is null || _name is null)
                    {
                        _name = value;
                    }
                }
            }
        }
    }
}
