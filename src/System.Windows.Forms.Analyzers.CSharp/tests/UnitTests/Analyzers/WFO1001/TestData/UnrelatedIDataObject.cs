using System;
using System.Runtime.Versioning;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer.Data
{
    internal interface IDataObject
    {
        void DoStuff();
    }

    internal class UnrelatedIDataObject : IDataObject
    {
        public void DoStuff() => throw new NotImplementedException();
    }
}
