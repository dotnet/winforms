// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    internal partial class OleDragDropHandler
    {
        protected class ComponentDataObject : IDataObject
        {
            public ComponentDataObject(IOleDragClient dragClient, IServiceProvider sp, object[] comps, int x, int y)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public ComponentDataObject(IOleDragClient dragClient, IServiceProvider sp, object serializationData)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public object[] Components => throw new NotImplementedException(SR.NotImplementedByDesign);

            public virtual object GetData(string format)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public virtual object GetData(string format, bool autoConvert)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public virtual object GetData(Type t)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///  If the there is data store in the data object associated with
            ///  format this will return true.
            /// </summary>
            public bool GetDataPresent(string format, bool autoConvert)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///  If the there is data store in the data object associated with
            ///  format this will return true.
            /// </summary>
            public bool GetDataPresent(string format)
            {
                return GetDataPresent(format, false);
            }

            /// <summary>
            ///  If the there is data store in the data object associated with
            ///  format this will return true.
            /// </summary>
            public bool GetDataPresent(Type format)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///  Retrieves a list of all formats stored in this data object.
            /// </summary>
            public string[] GetFormats(bool autoConvert)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///  Retrieves a list of all formats stored in this data object.
            /// </summary>
            public string[] GetFormats()
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///  Sets the data to be associated with the specific data format. For
            ///  a listing of predefined formats see System.Windows.Forms.DataFormats.
            /// </summary>
            public void SetData(string format, bool autoConvert, object data)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///  Sets the data to be associated with the specific data format. For
            ///  a listing of predefined formats see System.Windows.Forms.DataFormats.
            /// </summary>
            public void SetData(string format, object data)
            {
                throw new Exception(SR.DragDropSetDataError);
            }

            /// <summary>
            ///  Sets the data to be associated with the specific data format.
            /// </summary>
            public void SetData(Type format, object data)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///  Stores data in the data object. The format assumed is the
            ///  class of data
            /// </summary>
            public void SetData(object data)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public void Deserialize(IServiceProvider serviceProvider, bool removeCurrentComponents)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
        }
    }
}
