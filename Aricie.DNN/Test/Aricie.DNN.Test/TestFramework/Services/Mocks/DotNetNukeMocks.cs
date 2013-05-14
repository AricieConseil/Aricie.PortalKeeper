using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Security.Roles.Moles;
using DotNetNuke.Framework.Moles;
using Aricie.DNN.Test.TestFramework.Services.FakesProviders;
using DotNetNuke.Entities.Portals.Moles;
using System.Collections;
using DotNetNuke.Entities.Tabs;

namespace Aricie.DNN.Test.TestFramework.Services.Mocks
{
    public class DotNetNukeMocks
    {
        private static DnnRoleProvider _dnnRoleProvider;
        private static TabInfo _ParentTab;
        private static TabInfo _home;
        private static TabInfo _testTab;

        public static TabInfo ParentTab
        {
            get
            {
                if (_ParentTab == null)
                {
                    _ParentTab = new TabInfo() { TabID = 90, TabName = "parent", };
                }

                return _ParentTab;
            }
        }


        public static TabInfo Home
        {
            get
            {
                if (_home == null)
                {
                    _testTab = new TabInfo() { TabID = 1, TabName = "Home"};
                }

                return _testTab;
            }
        }

        public static TabInfo TestTab
        {
            get
            {
                if (_testTab == null)
                {
                    _testTab = new TabInfo() { TabID = 100, TabName = "Test", ParentId = ParentTab.TabID};
                }

                return _testTab;
            }
        }

        public static void Mock()
        {
            MReflection.CreateObjectString = CreateObject;
        }
        
        private static object CreateObject(string type)
        {
            if (_dnnRoleProvider == null)
                _dnnRoleProvider = new DnnRoleProvider();

            switch (type)
            {
                case "roles":
                    return _dnnRoleProvider;

                default:
                    throw new NotImplementedException(string.Format("CreateObject for {0}", type));
            }
        }

    }
}
