// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.System.Ole;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public partial class Control
{
    private class AxSourcingSite : ISite
    {
        private readonly AgileComPointer<IOleClientSite> _clientSite;
        private string? _name;
        private HtmlShimManager? _shimManager;

        internal AxSourcingSite(IComponent component, AgileComPointer<IOleClientSite> clientSite, string? name)
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
        public IContainer? Container => null;

        public unsafe object? GetService(Type service)
        {
            if (service == typeof(HtmlDocument))
            {
                using var clientSite = _clientSite.GetInterface();
                using ComScope<IOleContainer> container = new(null);
                clientSite.Value->GetContainer(container);
                using var document = container.TryQuery<IHTMLDocument>(out HRESULT hr);
                if (hr.Succeeded)
                {
                    _shimManager ??= new HtmlShimManager();
                    return new HtmlDocument(_shimManager, document);
                }
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
        public string? Name
        {
            get => _name;

            [RequiresUnreferencedCode(TrimmingConstants.SiteNameMessage)]
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
