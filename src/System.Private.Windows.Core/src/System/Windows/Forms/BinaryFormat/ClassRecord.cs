// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Base class for class records.
/// </summary>
/// <remarks>
///  <para>
///   Includes the values for the class (which trail the record)
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/c9bc3af3-5a0c-4b29-b517-1b493b51f7bb">
///    [MS-NRBF] 2.3
///   </see>.
///  </para>
/// </remarks>
internal abstract class ClassRecord : ObjectRecord
{
    internal ClassInfo ClassInfo { get; }
    public IReadOnlyList<object?> MemberValues { get; }
    public MemberTypeInfo MemberTypeInfo { get; }

    public string Name => ClassInfo.Name;
    public override Id ObjectId => ClassInfo.ObjectId;
    public virtual Id LibraryId => Id.Null;

    public IReadOnlyList<string> MemberNames => ClassInfo.MemberNames;

    public object? this[string memberName]
    {
        get
        {
            for (int i = 0; i < ClassInfo.MemberNames.Count; i++)
            {
                string current = ClassInfo.MemberNames[i];
                if (current == memberName)
                {
                    return MemberValues[i];
                }
            }

            throw new KeyNotFoundException();
        }
    }

    private protected ClassRecord(ClassInfo classInfo, MemberTypeInfo memberTypeInfo, IReadOnlyList<object?> memberValues)
    {
        ClassInfo = classInfo;
        MemberValues = memberValues;
        MemberTypeInfo = memberTypeInfo;
    }
}
