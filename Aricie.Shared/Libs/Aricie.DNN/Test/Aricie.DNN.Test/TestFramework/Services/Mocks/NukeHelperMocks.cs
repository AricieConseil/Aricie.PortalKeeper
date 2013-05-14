using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aricie.DNN.Services.Moles;
using DotNetNuke.Entities.Portals.Moles;
using System.Collections;

namespace Aricie.DNN.Test.TestFramework.Services.Mocks
{
    public class NukeHelperMocks
    {
        private static ArrayList _tabs;


        public static void Mock()
        {
            MPortalSettings ps = new MPortalSettings();
            MNukeHelper.PortalIdGet = () => 0;
            MNukeHelper.GetSSLEnabledInt32 = (portalId) => false;
            MNukeHelper.PortalSettingsGet = () => ps;

            ps.DesktopTabsGet = () => _tabs;
            ps.HomeTabIdGet = () => 1;

            _tabs = new ArrayList();
            _tabs.Add(DotNetNukeMocks.Home);
            _tabs.Add(DotNetNukeMocks.TestTab);
            _tabs.Add(DotNetNukeMocks.ParentTab);

        }
    }
}
