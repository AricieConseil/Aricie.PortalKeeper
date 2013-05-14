using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aricie.DNN.Settings.Moles;
using Aricie.DNN.Settings;
using System.Collections;

namespace Aricie.DNN.Test.TestFramework.Services.Mocks
{
    public class SettingsControllerMocks
    {

        private static Hashtable _hostSettings = new Hashtable();
        private static Hashtable _moduleSettings = new Hashtable();
        private static Hashtable _portalSettings = new Hashtable();
        private static Hashtable _tabModuleSettings = new Hashtable();


        public static void Mock()
        {
            MSettingsController.FetchSettingsSettingsScopeInt32 = FetchSettings;
        }

        private static Hashtable FetchSettings(SettingsScope scope, int id)
        {

            switch (scope)
	        {
		        case SettingsScope.HostSettings:
                    return _hostSettings;
                case SettingsScope.PortalSettings:
                    return _portalSettings;
                case SettingsScope.ModuleSettings:
                default:
                    return _moduleSettings;
                case SettingsScope.TabModuleSettings:
                    return _tabModuleSettings;
	        }
        }
    }
}
