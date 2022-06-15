// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Windows.Forms.Metafiles;
using static Interop;

namespace System
{
    internal interface IEmfValidator
    {
        /// <summary>
        ///  Returns true if the given <paramref name="recordType"/> should be validated by this validator.
        /// </summary>
        bool ShouldValidate(Gdi32.EMR recordType);

        /// <summary>
        ///  Validates the given <paramref name="record"/>.
        /// </summary>
        /// <param name="complete">
        ///  True if the validator is finished validating. If false the validator will remain in scope for handling
        ///  the next record.
        /// </param>
        void Validate(ref EmfRecord record, DeviceContextState state, out bool complete);

        /// <summary>
        ///  Return true if this validator is still in scope when all records have been processed.
        /// </summary>
        bool FailIfIncomplete => true;
    }
}
