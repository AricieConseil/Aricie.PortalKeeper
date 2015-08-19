using Aricie.DNN.Modules.PortalKeeper;
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
                        foreach (DynamicRoute objRoute in objService.SpecificRoutes)
                        {
                            if (objRoute.Enabled)
                            {
                                if (objRoute.DNNRoute.Enabled)
                                {
                                    mapRouteManager.MapHttpRoute(objRoute.DNNRoute.Entity.FolderName, objRoute.Name, objRoute.Template, 
                                        objRoute.Defaults.EvaluateVariables(PortalKeeperContext<RequestEvent>.Instance, PortalKeeperContext<RequestEvent>.Instance), 
                                        objRoute.Constraints.EvaluateVariables(PortalKeeperContext<RequestEvent>.Instance, PortalKeeperContext<RequestEvent>.Instance), 
                                        objRoute.DNNRoute.Entity.Namespaces.ToArray());

                                }

                            }
                        }
                    }

                }
            }
        }
    }
}