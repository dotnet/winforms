// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    public partial class Control
    {
        private class AxSourcingSite : ISite
        {
            private readonly UnsafeNativeMethods.IOleClientSite _clientSite;
            private string _name;
            private HtmlShimManager _shimManager;

            internal AxSourcingSite(IComponent component, UnsafeNativeMethods.IOleClientSite clientSite, string name)
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
                object retVal = null;

                if (service == typeof(HtmlDocument))
                {

                    int hr = _clientSite.GetContainer(out UnsafeNativeMethods.IOleContainer iOlecontainer);

                    if (NativeMethods.Succeeded(hr)
                            && (iOlecontainer is UnsafeNativeMethods.IHTMLDocument))
                    {
                        if (_shimManager == null)
                        {
                            _shimManager = new HtmlShimManager();
                        }

                        retVal = new HtmlDocument(_shimManager, iOlecontainer as UnsafeNativeMethods.IHTMLDocument);
                    }

                }
                else if (_clientSite.GetType().IsAssignableFrom(service))
                {
                    retVal = _clientSite;
                }

                return retVal;
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
                get { return _name; }
                set
                {
                    if (value == null || _name == null)
                    {
                        _name = value;
                    }
                }
            }
        }
    }
}
