// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomLocalizationProvider
{
    /// <summary>
    ///  The design time language and localizable properties.
    /// </summary>
    [ProvideProperty("Language", typeof(IComponent))]
    [ProvideProperty("LoadLanguage", typeof(IComponent))]
    [ProvideProperty("Localizable", typeof(IComponent))]
    internal class LanguageExtenders : IExtenderProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDesignerHost? _host;
        private IComponent? _lastRoot;
        private bool _localizable;
        private CultureInfo _language;
        private CultureInfo? _loadLanguage;

        public LanguageExtenders(IServiceProvider serviceProvider, CultureInfo?[]? supportedCultures)
        {
            _serviceProvider = serviceProvider;
            _host = serviceProvider.GetService<IDesignerHost>();
            _language = CultureInfo.InvariantCulture;

            if (supportedCultures is not null)
            {
                SupportedCultures = new TypeConverter.StandardValuesCollection(supportedCultures);
            }
        }

        /// <summary>
        ///  A collection of custom supported cultures. This can be null, indicating that the
        ///  type converter should use the default set of supported cultures.
        /// </summary>
        internal TypeConverter.StandardValuesCollection? SupportedCultures { get; }

        /// <summary>
        ///  Broadcasts a global change, indicating that all objects on the designer have changed.
        /// </summary>
        private static void BroadcastGlobalChange(IComponent component)
        {
            ISite? site = component.Site;

            if (site.TryGetService(out IComponentChangeService? changeService)
                && site.TryGetService(out IContainer? container))
            {
                foreach (IComponent c in container.Components)
                {
                    changeService.OnComponentChanging(c);
                    changeService.OnComponentChanged(c);
                }
            }
        }

        /// <summary>
        ///  This method compares the current root component
        ///  with the last one we saw. If they don't match,
        ///  that means the designer has reloaded and we
        ///  should set all of our properties back to their
        ///  defaults. This is more efficient than syncing
        ///  an event.
        /// </summary>
        private void CheckRoot()
        {
            if (_host is not null && _host.RootComponent != _lastRoot)
            {
                _lastRoot = _host.RootComponent;
                _language = CultureInfo.InvariantCulture;
                _loadLanguage = null;
                _localizable = false;
            }
        }

        /// <summary>
        ///  Gets the language set for the specified object.
        /// </summary>
        [DesignOnly(true)]
        [TypeConverter(typeof(LanguageCultureInfoConverter))]
        [Category("Design")]
        [SRDescription("LocalizationProviderLanguageDescr")]
        public CultureInfo GetLanguage(IComponent o)
        {
            CheckRoot();
            return _language;
        }

        /// <summary>
        ///  Gets the language we'll use when re-loading the designer.
        /// </summary>
        [DesignOnly(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CultureInfo GetLoadLanguage(IComponent o)
        {
            CheckRoot();

            // If we never configured the load language, we're always invariant.
            _loadLanguage ??= CultureInfo.InvariantCulture;

            return _loadLanguage;
        }

        /// <summary>
        ///  Gets a value indicating whether the specified object supports design-time localization support.
        /// </summary>
        [DesignOnly(true)]
        [Category("Design")]
        [SRDescription("LocalizationProviderLocalizableDescr")]
        public bool GetLocalizable(IComponent? o)
        {
            CheckRoot();
            return _localizable;
        }

        /// <summary>
        ///  Sets the language to use. When the language is set the designer will be reloaded.
        /// </summary>
        public void SetLanguage(IComponent o, CultureInfo? language)
        {
            CheckRoot();

            language ??= CultureInfo.InvariantCulture;

            bool isInvariantCulture = (language.Equals(CultureInfo.InvariantCulture));

            if (_language.Equals(language))
            {
                return;
            }

            _language = language;

            if (!isInvariantCulture)
            {
                SetLocalizable(o, true);
            }

            if (_host is null)
            {
                return;
            }

            // Only reload if we're not in the process of loading!
            if (_host.Loading)
            {
                _loadLanguage = language;
            }
            else
            {
                bool reloadSuccessful = false;

                if (_serviceProvider.TryGetService(out IDesignerLoaderService? ls))
                {
                    reloadSuccessful = ls.Reload();
                }

                if (!reloadSuccessful)
                {
                    IUIService? uis = _serviceProvider.GetService<IUIService>();

                    uis?.ShowMessage(SR.LocalizationProviderManualReload);
                }
            }
        }

        /// <summary>
        ///  Sets a value indicating whether or not the specified object has design-time
        ///  localization support.
        /// </summary>
        public void SetLocalizable(IComponent o, bool localizable)
        {
            CheckRoot();

            if (localizable != _localizable)
            {
                _localizable = localizable;

                if (!localizable)
                {
                    SetLanguage(o, CultureInfo.InvariantCulture);
                }

                if (_host is not null && !_host.Loading)
                {
                    BroadcastGlobalChange(o);
                }
            }
        }

        /// <summary>
        ///  Gets a value indicating whether the specified object should have its design-time localization support persisted.
        /// </summary>
        private bool ShouldSerializeLanguage(IComponent o) => _language != CultureInfo.InvariantCulture;

        /// <summary>
        ///  Gets a value indicating whether the specified object should have its design-time localization support persisted.
        /// </summary>
        private bool ShouldSerializeLocalizable(IComponent o) => _localizable;

        /// <summary>
        ///  Resets the localizable property to the 'defaultLocalizable' value.
        /// </summary>
        private void ResetLocalizable(IComponent o) => SetLocalizable(o, false);

        /// <summary>
        ///  Resets the language for the specified object.
        /// </summary>
        private void ResetLanguage(IComponent o) => SetLanguage(o, CultureInfo.InvariantCulture);

        /// <summary>
        ///  We only extend the root component.
        /// </summary>
        public bool CanExtend(object o)
        {
            CheckRoot();

            return (_host is not null && o == _host.RootComponent);
        }
    }
}
