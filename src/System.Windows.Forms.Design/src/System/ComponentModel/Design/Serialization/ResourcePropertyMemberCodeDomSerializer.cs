// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Diagnostics;
using System.Globalization;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This serializer replaces the property serializer for properties when we're
    ///  in localization mode.
    /// </summary>
    internal class ResourcePropertyMemberCodeDomSerializer : MemberCodeDomSerializer
    {
        private CodeDomLocalizationModel _model;
        private MemberCodeDomSerializer _serializer;
        private CodeDomLocalizationProvider.LanguageExtenders _extender;
        private CultureInfo localizationLanguage;

        internal ResourcePropertyMemberCodeDomSerializer(MemberCodeDomSerializer serializer, CodeDomLocalizationProvider.LanguageExtenders extender, CodeDomLocalizationModel model)
        {
            Debug.Assert(extender != null, "Extender should have been created by now.");

            _serializer = serializer;
            _extender = extender;
            _model = model;
        }

        /// <summary>
        ///  This method actually performs the serialization.  When the member is serialized
        ///  the necessary statements will be added to the statements collection.
        /// </summary>
        public override void Serialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
        {
            // We push the localization model to indicate that our serializer is in control.  Our
            // serialization provider looks for this and decides what type of resource serializer
            // to give us.
            manager.Context.Push(_model);

            try
            {
                _serializer.Serialize(manager, value, descriptor, statements);
            }
            finally
            {
                manager.Context.Pop();
            }
        }

        private CultureInfo GetLocalizationLanguage(IDesignerSerializationManager manager)
        {
            if (localizationLanguage is null)
            {
                // Check to see if our base component's localizable prop is true
                RootContext rootCxt = manager.Context[typeof(RootContext)] as RootContext;

                if (rootCxt != null)
                {
                    object comp = rootCxt.Value;
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(comp)["LoadLanguage"];

                    if (prop != null && prop.PropertyType == typeof(CultureInfo))
                    {
                        localizationLanguage = (CultureInfo)prop.GetValue(comp);
                    }
                }
            }

            return localizationLanguage;
        }

        private void OnSerializationComplete(object sender, EventArgs e)
        {
            // we do the cleanup here and clear out the cache of the localizedlanguage
            localizationLanguage = null;

            //unhook the event
            IDesignerSerializationManager manager = sender as IDesignerSerializationManager;
            Debug.Assert(manager != null, "manager should not be null!");

            if (manager != null)
            {
                manager.SerializationComplete -= new EventHandler(OnSerializationComplete);
            }
        }

        /// <summary>
        ///  This method returns true if the given member descriptor should be serialized,
        ///  or false if there is no need to serialize the member.
        /// </summary>
        public override bool ShouldSerialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor)
        {
            bool shouldSerialize = _serializer.ShouldSerialize(manager, value, descriptor);

            if (!shouldSerialize && !descriptor.Attributes.Contains(DesignOnlyAttribute.Yes))
            {
                switch (_model)
                {
                    case CodeDomLocalizationModel.PropertyReflection:
                        if (!shouldSerialize)
                        {
                            // hook up the event the first time to clear out our cache at the end of the serialization
                            if (localizationLanguage is null)
                            {
                                manager.SerializationComplete += new EventHandler(OnSerializationComplete);
                            }
                            if (GetLocalizationLanguage(manager) != CultureInfo.InvariantCulture)
                            {
                                shouldSerialize = true;
                            }
                        }
                        break;

                    case CodeDomLocalizationModel.PropertyAssignment:
                        // If this property contains its default value, we still want to serialize it if we are in
                        // localization mode if we are writing to the default culture, but only if the object
                        // is not inherited.
                        InheritanceAttribute inheritance = (InheritanceAttribute)manager.Context[typeof(InheritanceAttribute)];

                        if (inheritance is null)
                        {
                            inheritance = (InheritanceAttribute)TypeDescriptor.GetAttributes(value)[typeof(InheritanceAttribute)];
                            if (inheritance is null)
                            {
                                inheritance = InheritanceAttribute.NotInherited;
                            }
                        }

                        if (inheritance.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
                        {
                            shouldSerialize = true;
                        }

                        break;

                    default:
                        break;
                }
            }

            return shouldSerialize;
        }
    }
}
