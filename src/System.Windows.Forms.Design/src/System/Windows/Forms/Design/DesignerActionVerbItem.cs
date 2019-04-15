// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    internal class DesignerActionVerbItem : DesignerActionMethodItem
    {
        private readonly DesignerVerb _targetVerb;

        public DesignerActionVerbItem(DesignerVerb verb) : base(null, null, null)
        {
            _targetVerb = verb ?? throw new ArgumentNullException(nameof(verb));
        }

        public override string Category => "Verbs";

        public override string DisplayName => _targetVerb.Text;

        public override void Invoke() => _targetVerb.Invoke();
    }
}
