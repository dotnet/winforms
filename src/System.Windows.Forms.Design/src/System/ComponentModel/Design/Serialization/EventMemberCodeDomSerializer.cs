// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  A MemberCodeDomSerializer for events.
/// </summary>
internal sealed class EventMemberCodeDomSerializer : MemberCodeDomSerializer
{
    private static readonly CodeThisReferenceExpression s_thisRef = new();
    private static EventMemberCodeDomSerializer? s_default;

    internal static EventMemberCodeDomSerializer Default => s_default ??= new EventMemberCodeDomSerializer();

    /// <summary>
    ///  This method actually performs the serialization. When the member is serialized
    ///  the necessary statements will be added to the statements collection.
    /// </summary>
    public override void Serialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        if (descriptor is not EventDescriptor eventToSerialize)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        ArgumentNullException.ThrowIfNull(statements);

        try
        {
            IEventBindingService? eventBindings = manager.GetService<IEventBindingService>();

            // If the IEventBindingService is not available, we don't throw - we just don't do anything.
            if (eventBindings is not null)
            {
                PropertyDescriptor prop = eventBindings.GetEventProperty(eventToSerialize);
                string? methodName = (string?)prop.GetValue(value);

                if (methodName is not null)
                {
                    CodeExpression? eventTarget = SerializeToExpression(manager, value);
                    if (eventTarget is not null)
                    {
                        CodeTypeReference delegateTypeRef = new(eventToSerialize.EventType);
                        CodeDelegateCreateExpression delegateCreate = new(delegateTypeRef, s_thisRef, methodName);
                        CodeEventReferenceExpression eventRef = new(eventTarget, eventToSerialize.Name);
                        CodeAttachEventStatement attach = new(eventRef, delegateCreate);

                        attach.UserData[typeof(Delegate)] = eventToSerialize.EventType;
                        statements.Add(attach);
                    }
                }
            }
        }
        catch (Exception e)
        {
            // Since we usually go through reflection, don't show what our engine does, show what caused the problem.
            if (e is TargetInvocationException)
            {
                e = e.InnerException!;
            }

            manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerPropertyGenFailed, eventToSerialize.Name, e.Message), manager));
        }
    }

    /// <summary>
    ///  This method returns true if the given member descriptor should be serialized,
    ///  or false if there is no need to serialize the member.
    /// </summary>
    public override bool ShouldSerialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor) => true;
}
