using System;
using System.ComponentModel.Design;
using System.Reflection;

namespace DesignSurfaceExt
{
    public class TabOrderHooker
    {
        private const string _Name_ = "TabOrderHooker";

        private object _tabOrder;

        //- Enables/Disables visual TabOrder on the view.
        //- internal override
        public void HookTabOrder(IDesignerHost host)
        {
            //- the TabOrder must be called AFTER the DesignSurface has been loaded
            //- therefore we do a little check
            if (null == host.RootComponent)
                throw new Exception(_Name_ + "::HookTabOrder() - Exception: the TabOrder must be invoked after the DesignSurface has been loaded! ");

            try
            {
                System.Reflection.Assembly designAssembly = System.Reflection.Assembly.Load("System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                Type tabOrderType = designAssembly.GetType("System.Windows.Forms.Design.TabOrder");
                if (_tabOrder == null)
                {
                    //- call the ctor passing the IDesignerHost target object
                    _tabOrder = Activator.CreateInstance(tabOrderType, new object[] { host });
                }
                else
                {
                    DisposeTabOrder();
                }
            }//end_try
            catch (Exception ex)
            {
                throw new Exception(_Name_ + "::HookTabOrder() - Exception: (see Inner Exception)", ex);
            }//end_catch
        }

        //- Disposes the tab order
        public void DisposeTabOrder()
        {
            if (null == _tabOrder)
                return;
            try
            {
                System.Reflection.Assembly designAssembly = System.Reflection.Assembly.Load("System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                Type tabOrderType = designAssembly.GetType("System.Windows.Forms.Design.TabOrder");
                tabOrderType.InvokeMember("Dispose", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, _tabOrder, new object[] { true });
                _tabOrder = null;
            }
            catch (Exception ex)
            {
                throw new Exception(_Name_ + "::DisposeTabOrder() - Exception: (see Inner Exception)", ex);
            }//end_catch
        }
    }//end_class
}//end_namespace
