// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Diagnostics;
    using System;
    

    internal class HelpInfo {

        private string helpFilePath;
        private string keyword;
        private HelpNavigator navigator;
        private object param;
        private int option;

        public HelpInfo(string helpfilepath) {
            this.helpFilePath= helpfilepath;
            this.keyword     = "";
            this.navigator =  HelpNavigator.TableOfContents;
            this.param = null;
            this.option = NativeMethods.HLP_FILE;
        }

        public HelpInfo(string helpfilepath, string keyword) {
            this.helpFilePath= helpfilepath;
            this.keyword     = keyword;
            this.navigator =  HelpNavigator.TableOfContents;
            this.param = null;
            this.option = NativeMethods.HLP_KEYWORD;
        }

        public HelpInfo(string helpfilepath, HelpNavigator navigator) {
            this.helpFilePath= helpfilepath;
            this.keyword     = "";
            this.navigator =  navigator;
            this.param = null;
            this.option = NativeMethods.HLP_NAVIGATOR;
        }


        public HelpInfo(string helpfilepath, HelpNavigator navigator, object param) {
            this.helpFilePath= helpfilepath;
            this.keyword     = "";
            this.navigator =  navigator;
            this.param = param;
            this.option = NativeMethods.HLP_OBJECT;
        }

        public int Option {
            get {
                return option;
            }
        }

        public string HelpFilePath {
            get {
                return helpFilePath;
            }
        }
        
        public string Keyword {
            get {
                return keyword;
            }
        }

        public HelpNavigator Navigator {
            get {
                return navigator;
            }
        }

        public object Param {
            get {
                return param;
            }
        }
       

        public override string ToString() {
            return "{HelpFilePath=" + helpFilePath + ", keyword =" + keyword + ", navigator=" + navigator.ToString() + "}";
        }



    }
}
