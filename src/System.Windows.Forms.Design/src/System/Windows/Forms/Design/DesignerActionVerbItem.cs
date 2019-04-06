// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    internal class DesignerActionVerbItem : DesignerActionMethodItem
    {
        private readonly DesignerVerb _targetVerb;

        public DesignerActionVerbItem(DesignerVerb verb)
        {
            _targetVerb = verb ?? throw new ArgumentNullException();
        }

        public override string Category
        {
            get => "Verbs";
        }

        public override string Description { get; }

        public override string DisplayName
        {
            get => _targetVerb.Text;
        }

        public override string MemberName
        {
            get => null;
        }

        public override bool IncludeAsDesignerVerb
        {
            get => false;
        }

        public override void Invoke() => _targetVerb.Invoke();
    }
}
