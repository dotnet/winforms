// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Globalization;
using System.Resources;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This is a serialization provider that provides a localization feature.  This provider
    ///  adds two properties to the root component:  Language and Localizable.  If Localizable
    ///  is set to true this provider will change the way that component properties are generated
    ///  and will route their values to a resource file.  Two localization models are
    ///  supported.
    /// </summary>
    public sealed partial class CodeDomLocalizationProvider : IDisposable, IDesignerSerializationProvider
    {
        private IExtenderProviderService _providerService;
        private readonly CodeDomLocalizationModel _model;
        private readonly CultureInfo[] _supportedCultures;
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
            ArgumentNullException.ThrowIfNull(provider);

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
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(supportedCultures);

            _model = model;
            _supportedCultures = (CultureInfo[])supportedCultures.Clone();
            Initialize(provider);
        }

        /// <summary>
        ///  Disposes this object.
        /// </summary>
        public void Dispose()
        {
            if (_providerService is not null && _extender is not null)
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

            if (_providerService is null)
            {
                throw new NotSupportedException(string.Format(SR.LocalizationProviderMissingService, nameof(IExtenderProviderService)));
            }

            _extender = new LanguageExtenders(provider, _supportedCultures);
            _providerService.AddExtenderProvider(_extender);
        }

        #region IDesignerSerializationProvider Members
        /// <summary>
        ///  Returns a code dom serializer
        /// </summary>
        private static object GetCodeDomSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            if (currentSerializer is null)
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
            // The second serializer is used only for localizing properties when the CodeDomLocalizationModel
            // is set to PropertyReflection

            // Compute a localization model based on the property, localization mode,
            // and what (if any) serializer already exists
            CodeDomLocalizationModel model = CodeDomLocalizationModel.None;
            object modelObj = manager.Context[typeof(CodeDomLocalizationModel)];

            if (modelObj is not null)
            {
                model = (CodeDomLocalizationModel)modelObj;
            }

            //Nifty, but this causes everything to be loc'd because our provider
            //comes in before the default one
            //if (model == CodeDomLocalizationModel.None && currentSerializer is null) {
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
            if (currentSerializer is null)
            {
                return null;
            }

            // If we've already provided this serializer, don't do it again
            if (currentSerializer is ResourcePropertyMemberCodeDomSerializer)
            {
                return null;
            }

            // We only care if we're localizable
            if (_extender is null || !_extender.GetLocalizable(null))
            {
                return null;
            }

            // Fish the property out of the context to see if the property is localizable.
            PropertyDescriptor serializingProperty = manager.Context[typeof(PropertyDescriptor)] as PropertyDescriptor;

            if (serializingProperty is null || !serializingProperty.IsLocalizable)
            {
                model = CodeDomLocalizationModel.None;
            }

            _memberSerializers ??= new Hashtable();

            _nopMemberSerializers ??= new Hashtable();

            object newSerializer;
            if (model == CodeDomLocalizationModel.None)
            {
                newSerializer = _nopMemberSerializers[currentSerializer];
            }
            else
            {
                newSerializer = _memberSerializers[currentSerializer];
            }

            if (newSerializer is null)
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
    }
}
