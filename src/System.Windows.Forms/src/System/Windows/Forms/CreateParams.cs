// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace System.Windows.Forms
{
    public class CreateParams
    {
        /// <summary>
        ///  Name of the window class to subclass. The default value for this field
        ///  is null, indicating that the window is not a subclass of an existing
        ///  window class. To subclass an existing window class, store the window
        ///  class name in this field. For example, to subclass the standard edit
        ///  control, set this field to "EDIT".
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        ///  The initial caption your control will have.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        ///  Window style bits. This must be a combination of WS_XXX style flags and
        ///  other control-specific style flags.
        /// </summary>
        public int Style { get; set; }

        /// <summary>
        ///  Extended window style bits. This must be a combination of WS_EX_XXX
        ///  style flags.
        /// </summary>
        public int ExStyle { get; set; }

        /// <summary>
        ///  Class style bits. This must be a combination of CS_XXX style flags. This
        ///  field is ignored if the className field is not null.
        /// </summary>
        public int ClassStyle { get; set; }

        /// <summary>
        ///  The left portion of the initial proposed location.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///  The top portion of the initial proposed location.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        ///  The initially proposed width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///  The initially proposed height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///  The controls parent.
        /// </summary>
        public IntPtr Parent { get; set; }

        /// <summary>
        ///  Any extra information that the underlying handle might want.
        /// </summary>
        public object Param { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder(64);
            builder.Append("CreateParams {'");
            builder.Append(ClassName);
            builder.Append("', '");
            builder.Append(Caption);
            builder.Append("', 0x");
            builder.Append(Convert.ToString(Style, 16));
            builder.Append(", 0x");
            builder.Append(Convert.ToString(ExStyle, 16));
            builder.Append(", {");
            builder.Append(X);
            builder.Append(", ");
            builder.Append(Y);
            builder.Append(", ");
            builder.Append(Width);
            builder.Append(", ");
            builder.Append(Height);
            builder.Append('}');
            builder.Append('}');
            return builder.ToString();
        }
    }
}
