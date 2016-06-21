using System;
using Aricie.DNN.Modules.PortalKeeper;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;

namespace Aricie.PortalKeeper.DNN7
{
    public class DynamicServiceRouteMapper : IServiceRouteMapper
    {
        public DynamicServiceRouteMapper()
        {
        }

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            if (PortalKeeperConfig.Instance.RestServices.Enabled)
            {
                foreach (RestService objService in PortalKeeperConfig.Instance.RestServices.RestServices)
                {
                    if (objService.Enabled )
                    {

                        foreach (DynamicControllerInfo objController in objService.DynamicControllers)
                        {
                            if (objController.Enabled)
                            {
                                foreach (DynamicRoute objRoute in objController.SpecificRoutes)
                                {
                                    RegisterRoute(mapRouteManager, objRoute);
                                }
                            }
                        }
                        foreach (DynamicRoute objRoute in objService.SpecificRoutes)
                        {
                            RegisterRoute(mapRouteManager, objRoute);
                        }
                    }

                }

                foreach (DynamicRoute objRoute in PortalKeeperConfig.Instance.RestServices.GlobalRoutes)
                {
                    RegisterRoute(mapRouteManager, objRoute);
                }
            }
        }

        private static void RegisterRoute(IMapRoute mapRouteManager , DynamicRoute objRoute)
        {
            if (objRoute.Enabled)
            {
                if (objRoute.DNNRoute.Enabled)
                {
                    try
                    {
                        mapRouteManager.MapHttpRoute(objRoute.DNNRoute.Entity.FolderName, objRoute.Name, objRoute.Template,
                        objRoute.Defaults.EvaluateVariables(PortalKeeperContext<RequestEvent>.Instance, PortalKeeperContext<RequestEvent>.Instance),
                        objRoute.Constraints.EvaluateVariables(PortalKeeperContext<RequestEvent>.Instance, PortalKeeperContext<RequestEvent>.Instance),
                        objRoute.DNNRoute.Entity.Namespaces.ToArray());
                    }
                    catch (Exception ex)
                    {
                        
                        Exceptions.LogException(ex);
                    }
                    

                }

            }
        }

    }
}