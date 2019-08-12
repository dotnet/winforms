// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Determines when changes to a data-bound control property get propagated
    ///  back to the corresponding data source property.
    /// </summary>
    public enum DataSourceUpdateMode
    {
        /// <summary>
        ///  Data source is updated when the control property is validated, ie.
        ///  during the Validating event. Typically this does not occur until the
        ///  input focus leaves the control. After validation, the value in the
        ///  control property will also be re-formatted.
        ///  This is the default update mode.
        /// </summary>
        OnValidation = 0,

        /// <summary>
        ///  Data source is updated whenever the control property changes (and also
        ///  updated again during validation). After validation, the value in the
        ///  control property will also be re-formatted.
        /// </summary>
        OnPropertyChanged = 1,

        /// <summary>
        ///  Data source is never updated. Binding is "read-only" with respect to
        ///  the data source. Values entered into the control are *not* parsed,
        ///  validated or re-formatted. To force the data source to be updated,
        ///  use the Binding.WriteValue method.
        /// </summary>
        Never = 2,
    }
}
