// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;

    /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject"]/*' />
    /// <devdoc>
    ///    <para>Provides a format-independent mechanism for transferring
    ///       data.</para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IDataObject {
        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.GetData"]/*' />
        /// <devdoc>
        ///    <para>Retrieves the data associated with the specified data 
        ///       format, using autoConvert to determine whether to convert the data to the
        ///       format. </para>
        /// </devdoc>
        object GetData(string format, bool autoConvert);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.GetData1"]/*' />
        /// <devdoc>
        ///    <para>Retrieves the data associated with the specified data format.</para>
        /// </devdoc>
        object GetData(string format);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.GetData2"]/*' />
        /// <devdoc>
        ///    <para>Retrieves the data associated with the specified class 
        ///       type format.</para>
        /// </devdoc>
        object GetData(Type format);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.SetData"]/*' />
        /// <devdoc>
        ///    <para>Stores the specified data and its associated format in 
        ///       this instance, using autoConvert to specify whether the data can be converted
        ///       to another format.</para>
        /// </devdoc>
        void SetData(string format, bool autoConvert, object data);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.SetData1"]/*' />
        /// <devdoc>
        ///    <para>Stores the specified data and its associated format in this
        ///       instance.</para>
        /// </devdoc>
        void SetData(string format, object data);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.SetData2"]/*' />
        /// <devdoc>
        ///    <para>Stores the specified data and its associated class type in this
        ///       instance.</para>
        /// </devdoc>
        void SetData(Type format, object data);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.SetData3"]/*' />
        /// <devdoc>
        ///    <para>Stores the specified data in this instance, using the class of the
        ///       data for the format.</para>
        /// </devdoc>
        void SetData(object data);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.GetDataPresent"]/*' />
        /// <devdoc>
        ///    <para>Determines whether data stored in this instance is 
        ///       associated with the specified format, using autoConvert to determine whether to
        ///       convert the data to the format.</para>
        /// </devdoc>
        bool GetDataPresent(string format, bool autoConvert);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.GetDataPresent1"]/*' />
        /// <devdoc>
        ///    <para>Determines whether data stored in this instance is associated with, or can be
        ///       converted to, the specified format.</para>
        /// </devdoc>
        bool GetDataPresent(string format);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.GetDataPresent2"]/*' />
        /// <devdoc>
        ///    <para>Determines whether data stored in this instance is associated with, or can be converted to, the specified format.</para>
        /// </devdoc>
        bool GetDataPresent(Type format);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.GetFormats"]/*' />
        /// <devdoc>
        ///    <para>Gets a list of all formats that data stored in this 
        ///       instance is associated with or can be converted to, using autoConvert to determine whether to
        ///       retrieve all formats that the data can be converted to or only native data
        ///       formats.</para>
        /// </devdoc>
        string[] GetFormats(bool autoConvert);

        /// <include file='doc\IDataObject.uex' path='docs/doc[@for="IDataObject.GetFormats1"]/*' />
        /// <devdoc>
        ///    <para>Gets a list of all formats that data stored in this instance is associated
        ///       with or can be converted to.</para>
        /// </devdoc>
        string[] GetFormats();
    }
}
