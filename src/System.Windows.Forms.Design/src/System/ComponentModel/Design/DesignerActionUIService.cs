// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.ComponentModel.Design
{
    public sealed class DesignerActionUIService : IDisposable
    {
        private DesignerActionUIStateChangeEventHandler _designerActionUIStateChangedEventHandler;
        private readonly IServiceProvider _serviceProvider; //standard service provider
        private readonly DesignerActionService _designerActionService;

        internal DesignerActionUIService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            if (serviceProvider != null)
            {
                _serviceProvider = serviceProvider;
                IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
                host.AddService(typeof(DesignerActionUIService), this);
                _designerActionService = serviceProvider.GetService(typeof(DesignerActionService)) as DesignerActionService;
                Debug.Assert(_designerActionService != null, "we should have created and registered the DAService first");
            }
        }

        /// <summary>
        ///  Disposes all resources and unhooks all events.
        /// </summary>
        public void Dispose()
        {
            if (_serviceProvider != null)
            {
                IDesignerHost host = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    host.RemoveService(typeof(DesignerActionUIService));
                }
            }
        }

        /// <summary>
        ///  This event is thrown whenever a request is made to show/hide the UI.
        /// </summary>
        public event DesignerActionUIStateChangeEventHandler DesignerActionUIStateChange
        {
            add => _designerActionUIStateChangedEventHandler += value;
            remove => _designerActionUIStateChangedEventHandler -= value;
        }

        public void HideUI(IComponent component)
        {
            OnDesignerActionUIStateChange(new DesignerActionUIStateChangeEventArgs(component, DesignerActionUIStateChangeType.Hide));
        }

        public void ShowUI(IComponent component)
        {
            OnDesignerActionUIStateChange(new DesignerActionUIStateChangeEventArgs(component, DesignerActionUIStateChangeType.Show));
        }

        /// <summary>
        ///  Refreshes the <see cref="DesignerActionGlyph">designer action glyph</see> as well as <see cref="DesignerActionPanel">designer action panels</see>.
        /// </summary>
        public void Refresh(IComponent component)
        {
            OnDesignerActionUIStateChange(new DesignerActionUIStateChangeEventArgs(component, DesignerActionUIStateChangeType.Refresh));
        }

        /// <summary>
        ///  This fires our DesignerActionsChanged event.
        /// </summary>
        private void OnDesignerActionUIStateChange(DesignerActionUIStateChangeEventArgs e)
        {
            _designerActionUIStateChangedEventHandler?.Invoke(this, e);
        }

        public bool ShouldAutoShow(IComponent component)
        {
            // Check the designer options...
            if (_serviceProvider != null)
            {
                if (_serviceProvider.GetService(typeof(DesignerOptionService)) is DesignerOptionService opts)
                {
                    PropertyDescriptor p = opts.Options.Properties["ObjectBoundSmartTagAutoShow"];
                    if (p != null && p.PropertyType == typeof(bool) && !(bool)p.GetValue(null))
                    {
                        return false;
                    }
                }
            }
            if (_designerActionService != null)
            {
                DesignerActionListCollection coll = _designerActionService.GetComponentActions(component);
                if (coll != null && coll.Count > 0)
                {
                    for (int i = 0; i < coll.Count; i++)
                    {
                        if (coll[i].AutoShow)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
