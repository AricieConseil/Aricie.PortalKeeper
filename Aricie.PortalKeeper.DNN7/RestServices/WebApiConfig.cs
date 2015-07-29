using System.Web.Http;
using System.Web.Http.Controllers;
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
                        foreach (DynamicRoute objRoute in objService.Routes.All)
                        {
                            if (objRoute.Enabled)
                            {
                                if (!objRoute.DNNRoute.Enabled)
                                {
                                    config.Routes.MapHttpRoute(
                                        name: objRoute.Name,
                                        routeTemplate: objRoute.Template /*,
                                    defaults: new { id = RouteParameter.Optional }*/
                                        );
                                }
                               
                            }
                        }
                    }

                }
            }
        }
    }
}