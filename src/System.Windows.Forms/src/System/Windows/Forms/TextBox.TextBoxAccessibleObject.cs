using static Interop;

namespace System.Windows.Forms
{
    public partial class TextBox
    {
        internal class TextBoxAccessibleObject : TextBoxBaseAccessibleObject
        {
            public TextBoxAccessibleObject(TextBox owner) : base(owner)
            { }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.HelpTextPropertyId:
                        string? placeholderText = (Owner as TextBox)?.PlaceholderText;
                        return string.IsNullOrEmpty(placeholderText) ? base.GetPropertyValue(propertyID) : placeholderText;

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }
        }
    }
}
