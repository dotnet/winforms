// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Globalization;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This serializer replaces the property serializer for properties when we're
///  in localization mode.
/// </summary>
internal class ResourcePropertyMemberCodeDomSerializer : MemberCodeDomSerializer
{
    private readonly CodeDomLocalizationModel _model;
    private readonly MemberCodeDomSerializer _serializer;
    private CultureInfo? _localizationLanguage;

    internal ResourcePropertyMemberCodeDomSerializer(MemberCodeDomSerializer serializer, CodeDomLocalizationModel model)
    {
        _serializer = serializer;
        _model = model;
    }

    /// <summary>
    ///  This method actually performs the serialization. When the member is serialized
    ///  the necessary statements will be added to the statements collection.
    /// </summary>
    public override void Serialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
    {
        // We push the localization model to indicate that our serializer is in control. Our
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

    private CultureInfo? GetLocalizationLanguage(IDesignerSerializationManager manager)
    {
        if (_localizationLanguage is null)
        {
            // Check to see if our base component's localizable prop is true
            if (manager.TryGetContext(out RootContext? rootCtx))
            {
                object comp = rootCtx.Value;
                PropertyDescriptor? prop = TypeDescriptor.GetProperties(comp)["LoadLanguage"];
                _localizationLanguage = prop?.GetValue<CultureInfo>(comp);
            }
        }

        return _localizationLanguage;
    }

    private void OnSerializationComplete(object? sender, EventArgs e)
    {
        // we do the cleanup here and clear out the cache of the localizedlanguage
        _localizationLanguage = null;

        // unhook the event
        IDesignerSerializationManager? manager = sender as IDesignerSerializationManager;
        Debug.Assert(manager is not null, "manager should not be null!");

        if (manager is not null)
        {
            manager.SerializationComplete -= OnSerializationComplete;
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
                        if (_localizationLanguage is null)
                        {
                            manager.SerializationComplete += OnSerializationComplete;
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
                    if (!manager.TryGetContext(out InheritanceAttribute? inheritance) && !TypeDescriptorHelper.TryGetAttribute(value, out inheritance))
                    {
                        inheritance = InheritanceAttribute.NotInherited;
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
