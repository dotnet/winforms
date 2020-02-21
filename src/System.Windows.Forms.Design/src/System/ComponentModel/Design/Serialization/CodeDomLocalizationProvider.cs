// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This is a serialization provider that provides a localization feature.  This provider
    ///  adds two properties to the root component:  Language and Localizable.  If Localizable
    ///  is set to true this provider will change the way that component properties are generated
    ///  and will route their values to a resource file.  Two localization models are 
    ///  supported.
    /// </summary>
    public sealed class CodeDomLocalizationProvider : IDisposable, IDesignerSerializationProvider
    {
        private IExtenderProviderService _providerService;
        private CodeDomLocalizationModel _model;
        private CultureInfo[] _supportedCultures;
        private LanguageExtenders _extender;
        private Hashtable _memberSerializers;
        private Hashtable _nopMemberSerializers;

        /// <summary>
        ///  Creates a new adapter and attaches it to the serialization manager.  This 
        ///  will add itself as a serializer for resources into the serialization manager, and, 
        ///  if not already added, will add itself as an extender provider to the roost component 
        ///  and provide the Language and Localizable properties.  The latter piece is only 
        ///  supplied if CodeDomLocalizationModel is not �none�.  
        /// </summary>
        public CodeDomLocalizationProvider(IServiceProvider provider, CodeDomLocalizationModel model)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            _model = model;
            Initialize(provider);
        }

        /// <summary>
        ///  Creates a new adapter and attaches it to the serialization manager.  This 
        ///  will add itself as a serializer for resources into the serialization manager, and, 
        ///  if not already added, will add itself as an extender provider to the roost component 
        ///  and provide the Language and Localizable properties.  The latter piece is only 
        ///  supplied if CodeDomLocalizationModel is not �none�.  
        /// </summary>
        public CodeDomLocalizationProvider(IServiceProvider provider, CodeDomLocalizationModel model, CultureInfo[] supportedCultures)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (supportedCultures == null)
            {
                throw new ArgumentNullException(nameof(supportedCultures));
            }

            _model = model;
            _supportedCultures = (CultureInfo[])supportedCultures.Clone();
            Initialize(provider);
        }

        /// <summary>
        ///  Disposes this object.
        /// </summary>
        public void Dispose()
        {
            if (_providerService != null && _extender != null)
            {
                _providerService.RemoveExtenderProvider(_extender);
                _providerService = null;
                _extender = null;
            }
        }

        /// <summary>
        ///  Adds our extended properties.
        /// </summary>
        private void Initialize(IServiceProvider provider)
        {
            _providerService = provider.GetService(typeof(IExtenderProviderService)) as IExtenderProviderService;

            if (_providerService == null)
            {
                throw new NotSupportedException(string.Format(SR.LocalizationProviderMissingService, typeof(IExtenderProviderService).Name));
            }

            _extender = new LanguageExtenders(provider, _supportedCultures);
            _providerService.AddExtenderProvider(_extender);
        }

        #region IDesignerSerializationProvider Members
        /// <summary>
        ///  Returns a code dom serializer
        /// </summary>
        private object GetCodeDomSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            if (currentSerializer == null)
            {
                return null;
            }

            // Always do default processing for the resource manager.
            if (typeof(ResourceManager).IsAssignableFrom(objectType))
            {
                return null;
            }

            // Here's how this works.  We have two different types of serializers to offer :  a 
            // serializer that writes out code like this:
            //
            //      this.Button1.Text = rm.GetString("Button1_Text");
            //
            // And one that writes out like this:
            //
            //      rm.ApplyResources(Button1, "Button1");
            //
            // The first serializer is used for serializable objects that have no serializer of their
            // own, and for localizable properties when the CodeDomLocalizationModel is set to PropertyAssignment.
            // The second serializer is used only for localizaing properties when the CodeDomLocalizationModel
            // is set to PropertyReflection

            // Compute a localization model based on the property, localization mode,
            // and what (if any) serializer already exists
            CodeDomLocalizationModel model = CodeDomLocalizationModel.None;
            object modelObj = manager.Context[typeof(CodeDomLocalizationModel)];

            if (modelObj != null)
            {
                model = (CodeDomLocalizationModel)modelObj;
            }

            //Nifty, but this causes everything to be loc'd because our provider
            //comes in before the default one
            //if (model == CodeDomLocalizationModel.None && currentSerializer == null) {
            //    model = CodeDomLocalizationModel.PropertyAssignment;
            //}

            if (model != CodeDomLocalizationModel.None)
            {
                return new LocalizationCodeDomSerializer(model, currentSerializer);
            }

            return null;
        }

        /// <summary>
        ///  Returns a code dom serializer for members.
        /// </summary>
        private object GetMemberCodeDomSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            CodeDomLocalizationModel model = _model;

            if (!typeof(PropertyDescriptor).IsAssignableFrom(objectType))
            {
                return null;
            }

            // Ok, we got a property descriptor.  If we're being localized
            // we provide a different type of serializer.  But, we only
            // do this if we were given a current serializer.  Otherwise
            // we don't know how to perform the serialization.
            // We can only provide a custom serializer if we have an existing one
            // to base off of.
            if (currentSerializer == null)
            {
                return null;
            }

            // If we've already provided this serializer, don't do it again
            if (currentSerializer is ResourcePropertyMemberCodeDomSerializer)
            {
                return null;
            }

            // We only care if we're localizable
            if (_extender == null || !_extender.GetLocalizable(null))
            {
                return null;
            }

            // Fish the property out of the context to see if the property is localizable.
            PropertyDescriptor serializingProperty = manager.Context[typeof(PropertyDescriptor)] as PropertyDescriptor;

            if (serializingProperty == null || !serializingProperty.IsLocalizable)
            {
                model = CodeDomLocalizationModel.None;
            }

            if (_memberSerializers == null)
            {
                _memberSerializers = new Hashtable();
            }

            if (_nopMemberSerializers == null)
            {
                _nopMemberSerializers = new Hashtable();
            }

            object newSerializer = null;

            if (model == CodeDomLocalizationModel.None)
            {
                newSerializer = _nopMemberSerializers[currentSerializer];
            }
            else
            {
                newSerializer = _memberSerializers[currentSerializer];
            }

            if (newSerializer == null)
            {
                newSerializer = new ResourcePropertyMemberCodeDomSerializer((MemberCodeDomSerializer)currentSerializer, _extender, model);

                if (model == CodeDomLocalizationModel.None)
                {
                    _nopMemberSerializers[currentSerializer] = newSerializer;
                }
                else
                {
                    _memberSerializers[currentSerializer] = newSerializer;
                }
            }

            return newSerializer;
        }

        /// <summary>
        ///  Returns an appropriate serializer for the object.  
        /// </summary>
        object IDesignerSerializationProvider.GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            if (serializerType == typeof(CodeDomSerializer))
            {
                return GetCodeDomSerializer(manager, currentSerializer, objectType, serializerType);
            }
            else if (serializerType == typeof(MemberCodeDomSerializer))
            {
                return GetMemberCodeDomSerializer(manager, currentSerializer, objectType, serializerType);
            }

            return null; // don't understand this type of serializer.
        }
        #endregion

        #region LanguageExtenders class
        /// <summary>
        ///  The design time language and localizable properties.
        /// </summary>
        [ProvideProperty("Language", typeof(IComponent))]
        [ProvideProperty("LoadLanguage", typeof(IComponent))]
        [ProvideProperty("Localizable", typeof(IComponent))]
        internal class LanguageExtenders : IExtenderProvider
        {
            private IServiceProvider _serviceProvider;
            private IDesignerHost _host;
            private IComponent _lastRoot;
            private TypeConverter.StandardValuesCollection _supportedCultures;
            private bool _localizable;
            private CultureInfo _language;
            private CultureInfo _loadLanguage;
            private CultureInfo _defaultLanguage;

            public LanguageExtenders(IServiceProvider serviceProvider, CultureInfo[] supportedCultures)
            {
                _serviceProvider = serviceProvider;
                _host = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
                _language = CultureInfo.InvariantCulture;

                if (supportedCultures != null)
                {
                    _supportedCultures = new TypeConverter.StandardValuesCollection(supportedCultures);
                }
            }

            /// <summary>
            ///  A collection of custom supported cultures.  This can be null, indicating that the
            ///  type converter should use the default set of supported cultures.
            /// </summary>
            internal TypeConverter.StandardValuesCollection SupportedCultures
            {
                get
                {
                    return _supportedCultures;
                }
            }

            /// <summary>
            ///  Returns the current default language for the thread.
            /// </summary>
            private CultureInfo ThreadDefaultLanguage
            {
                get
                {
                    if (_defaultLanguage == null)
                    {
                        _defaultLanguage = Application.CurrentCulture;
                    }
                    return _defaultLanguage;
                }
            }

            /// <summary>
            ///  Broadcasts a global change, indicating that all 
            ///  objects on the designer have changed.
            /// </summary>
            private void BroadcastGlobalChange(IComponent comp)
            {
                ISite site = comp.Site;

                if (site != null)
                {
                    IComponentChangeService cs = site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                    IContainer container = site.GetService(typeof(IContainer)) as IContainer;

                    if (cs != null && container != null)
                    {
                        foreach (IComponent c in container.Components)
                        {
                            cs.OnComponentChanging(c, null);
                            cs.OnComponentChanged(c, null, null, null);
                        }
                    }
                }
            }

            /// <summary>
            ///  This method compares the current root component
            ///  with the last one we saw.  If they don't match,
            ///  that means the designer has reloaded and we
            ///  should set all of our properties back to their
            ///  defaults.  This is more efficient than syncing
            ///  an event.
            /// </summary>
            private void CheckRoot()
            {
                if (_host != null && _host.RootComponent != _lastRoot)
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
            [SRDescriptionAttribute("LocalizationProviderLanguageDescr")]
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
                if (_loadLanguage == null)
                {
                    _loadLanguage = CultureInfo.InvariantCulture;
                }

                return _loadLanguage;
            }

            /// <summary>
            ///  Gets a value indicating whether the specified object supports design-time localization 
            ///  support.
            /// </summary>
            [DesignOnly(true)]
            [Category("Design")]
            [SRDescriptionAttribute("LocalizationProviderLocalizableDescr")]
            public bool GetLocalizable(IComponent o)
            {
                CheckRoot();

                return _localizable;
            }

            /// <summary>
            ///  Sets the language to use.  When the language is set the designer will be
            ///  reloaded.
            /// </summary>
            public void SetLanguage(IComponent o, CultureInfo language)
            {
                CheckRoot();

                if (language == null)
                {
                    language = CultureInfo.InvariantCulture;
                }

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

                if (_serviceProvider != null && _host != null)
                {
                    IDesignerLoaderService ls = _serviceProvider.GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;

                    // Only reload if we're not in the process of loading!
                    if (_host.Loading)
                    {
                        _loadLanguage = language;
                    }
                    else
                    {
                        bool reloadSuccessful = false;

                        if (ls != null)
                        {
                            reloadSuccessful = ls.Reload();
                        }

                        if (!reloadSuccessful)
                        {
                            IUIService uis = (IUIService)_serviceProvider.GetService(typeof(IUIService));

                            if (uis != null)
                            {
                                uis.ShowMessage(SR.LocalizationProviderManualReload);
                            }
                        }
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

                    if (_host != null && !_host.Loading)
                    {
                        BroadcastGlobalChange(o);
                    }
                }
            }

            /// <summary>
            ///  Gets a value indicating whether the specified object should have its design-time localization support persisted.
            /// </summary>
            private bool ShouldSerializeLanguage(IComponent o)
            {
                return (_language != null && _language != CultureInfo.InvariantCulture);
            }

            /// <summary>
            ///  Gets a value indicating whether the specified object should have its design-time localization support persisted.
            /// </summary>
            private bool ShouldSerializeLocalizable(IComponent o)
            {
                return (_localizable);
            }

            /// <summary>
            ///  Resets the localizable property to the 'defaultLocalizable' value.
            /// </summary>
            private void ResetLocalizable(IComponent o)
            {
                SetLocalizable(o, false);
            }

            /// <summary>
            ///  Resets the language for the specified object.
            /// </summary>
            private void ResetLanguage(IComponent o)
            {
                SetLanguage(o, CultureInfo.InvariantCulture);
            }

            /// <summary>
            ///  We only extend the root component.
            /// </summary>
            public bool CanExtend(object o)
            {
                CheckRoot();

                return (_host != null && o == _host.RootComponent);
            }
        }

        #region LanguageCultureInfoConverter 
        /// <summary>
        ///  This is a culture info converter that knows how to provide
        ///  a restricted list of cultures based on the SupportedCultures
        ///  property of the extender.  If the extender can't be found
        ///  or the SupportedCultures property returns null, this 
        ///  defaults to the stock implementation.
        /// </summary>
        internal sealed class LanguageCultureInfoConverter : CultureInfoConverter
        {
            /// <summary>
            ///  Retrieves the Name for a input CultureInfo.
            /// </summary>
            protected override string GetCultureName(CultureInfo culture)
            {
                return culture.DisplayName;
            }

            /// <summary>
            ///  Gets a collection of standard values collection for a System.Globalization.CultureInfo
            ///  object using the specified context.
            /// </summary>
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                StandardValuesCollection values = null;

                if (context.PropertyDescriptor != null)
                {
                    ExtenderProvidedPropertyAttribute attr = context.PropertyDescriptor.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;

                    if (attr != null)
                    {
                        LanguageExtenders provider = attr.Provider as LanguageExtenders;

                        if (provider != null)
                        {
                            values = provider.SupportedCultures;
                        }
                    }
                }

                if (values == null)
                {
                    values = base.GetStandardValues(context);
                }

                return values;
            }
        }
        #endregion
        #endregion
    }
}
