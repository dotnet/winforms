﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Layout;
namespace System.Windows.Forms {
    
    public abstract class LayoutSettings {
        private IArrangedElement _owner;

        protected LayoutSettings() {
        }
        
        internal LayoutSettings(IArrangedElement owner) {
            this._owner = owner;
        }
        
        public virtual LayoutEngine LayoutEngine {
            get { return null;}
        }

        internal IArrangedElement Owner {
            get { return _owner; }
        }
    }
}
