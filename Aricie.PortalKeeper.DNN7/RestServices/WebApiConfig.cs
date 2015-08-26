using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Dispatcher;
using Aricie.DNN.Modules.PortalKeeper;

namespace Aricie.PortalKeeper.DNN7.WebAPI
{
    public static class WebApiConfig
    {

        public static HttpConfiguration GetConfiguration()
        {
            return System.Web.Http.GlobalConfiguration.Configuration;
        }


        public static void RegisterWebHosted()
        {
            WebApiConfig.Register(GetConfiguration());
        }

        public static void Register(HttpConfiguration config)
        {
            if (PortalKeeperConfig.Instance.RestServices.Enabled)
            {
                var originalControllerSelector = config.Services.GetHttpControllerSelector();
                config.Services.Replace(typeof(IHttpControllerSelector), new DynamicControllerSelector(config, originalControllerSelector));
                var originalActionSelector = config.Services.GetActionSelector();
                config.Services.Replace(typeof(IHttpActionSelector), new DynamicActionSelector(config, originalActionSelector));
                config.Formatters.Add(new BrowserJsonFormatter());

                foreach (RestService objService in PortalKeeperConfig.Instance.RestServices.RestServices)
                {
                    if (objService.Enabled)
                    {
                        foreach (DynamicRoute objRoute in objService.SpecificRoutes)
                        {
                            if (objRoute.Enabled)
                            {
                                if (!objRoute.DNNRoute.Enabled)
                                {
                                    config.Routes.MapHttpRoute(
                                        name: objRoute.Name,
                                        routeTemplate: objRoute.Template,
                                        defaults:  objRoute.Defaults.EvaluateVariables(PortalKeeperContext<RequestEvent>.Instance, PortalKeeperContext<RequestEvent>.Instance), 
                                        constraints: objRoute.Constraints.EvaluateVariables(PortalKeeperContext<RequestEvent>.Instance, PortalKeeperContext<RequestEvent>.Instance)
                                        );
                                }
                               
                            }
                        }
                    }

                }
            }
        }

        public static IEnumerable<string> GetSampleRoutes(string basePath, DynamicControllerInfo controller)
        {
            var toReturn = new List<string>();
            IApiExplorer apiExplorer = GetConfiguration().Services.GetApiExplorer();

            foreach (ApiDescription api in apiExplorer.ApiDescriptions)
            {
                if (DotNetNuke.Web.Api.RouteExtensions.GetName(api.Route).StartsWith(PortalKeeperContext<SimpleEngineEvent>.MODULE_NAME, StringComparison.OrdinalIgnoreCase)
                    && api.ActionDescriptor.ControllerDescriptor.ControllerName == controller.Name)
                {
                    toReturn.Add(api.ActionDescriptor.ActionName + " : "
                        + api.HttpMethod.ToString() + " : "
                        + basePath + "/" + api.RelativePath);
                }
            }
            return toReturn;
        }


    }
}