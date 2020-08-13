// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2ICategorizePropertiesHandler : Com2ExtendedBrowsingHandler
    {
        public override Type Interface => typeof(VSSDK.ICategorizeProperties);

        private unsafe string GetCategoryFromObject(object obj, Ole32.DispatchID dispid)
        {
            if (!(obj is VSSDK.ICategorizeProperties catObj))
            {
                return null;
            }

            VSSDK.PROPCAT categoryID = 0;
            if (catObj.MapPropertyToCategory(dispid, &categoryID) != HRESULT.S_OK)
            {
                return null;
            }

            switch (categoryID)
            {
                case VSSDK.PROPCAT.Nil:
                    return string.Empty;
                case VSSDK.PROPCAT.Misc:
                    return SR.PropertyCategoryMisc;
                case VSSDK.PROPCAT.Font:
                    return SR.PropertyCategoryFont;
                case VSSDK.PROPCAT.Position:
                    return SR.PropertyCategoryPosition;
                case VSSDK.PROPCAT.Appearance:
                    return SR.PropertyCategoryAppearance;
                case VSSDK.PROPCAT.Behavior:
                    return SR.PropertyCategoryBehavior;
                case VSSDK.PROPCAT.Data:
                    return SR.PropertyCategoryData;
                case VSSDK.PROPCAT.List:
                    return SR.PropertyCategoryList;
                case VSSDK.PROPCAT.Text:
                    return SR.PropertyCategoryText;
                case VSSDK.PROPCAT.Scale:
                    return SR.PropertyCategoryScale;
                case VSSDK.PROPCAT.DDE:
                    return SR.PropertyCategoryDDE;
            }

            if (catObj.GetCategoryName(categoryID, Kernel32.GetThreadLocale(), out string categoryName) == HRESULT.S_OK)
            {
                return categoryName;
            }

            return null;
        }

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[] propDesc)
        {
            if (propDesc is null)
            {
                return;
            }
            for (int i = 0; i < propDesc.Length; i++)
            {
                propDesc[i].QueryGetBaseAttributes += new GetAttributesEventHandler(OnGetAttributes);
            }
        }

        private void OnGetAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            string cat = GetCategoryFromObject(sender.TargetObject, sender.DISPID);

            if (cat != null && cat.Length > 0)
            {
                attrEvent.Add(new CategoryAttribute(cat));
            }
        }
    }
}
