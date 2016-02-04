using System;
using System.Collections.Generic;
using System.Web.Http.Filters;
using Aricie.DNN.Modules.PortalKeeper;
using Aricie.PortalKeeper.DNN7.WebAPI;
using System.Linq;
using System.Net.Http;
using Aricie.DNN.Services;
using DotNetNuke.Web.Api;

namespace Aricie.PortalKeeper.DNN7
{
    public class Dnn7ObsoleteDotNetProvider : ObsoleteDotNetProvider
    {

        public override void RegisterWebAPI()
        {
            WebApiConfig.RegisterWebHosted();
        }

        public override System.Collections.Generic.List<string> GetFormatterNames()
        {
            var config = WebApiConfig.GetConfiguration();
            return config.Formatters.Select(form => form.GetType().Name).Distinct().ToList();

        }

        public override System.Collections.Generic.List<string> GetMediaTypeHeaders()
        {
            var config = WebApiConfig.GetConfiguration();
            return config.Formatters.SelectMany(form => form.SupportedMediaTypes.Select(map => map.MediaType)).Distinct().ToList();

        }

        
        public override System.Collections.Generic.IEnumerable<System.Type> GetWebAPIAttributes()
        {
            var systemNetAssembly = typeof (FilterAttribute).Assembly;
            var dnnWebAssembly = typeof(DnnAuthorizeAttribute).Assembly;
            var toReturn = systemNetAssembly.GetTypes().Where(IsValidAttributeType)
                .Union(dnnWebAssembly.GetTypes().Where(IsValidAttributeType))
                .Union(new Type[] {typeof (SupportedModulesSurrogateAttribute)});
            return toReturn;
        }


        private bool IsValidAttributeType(Type objType)
        {
            return objType.IsPublic 
                && typeof (Attribute).IsAssignableFrom(objType) 
                && !objType.IsAbstract 
                && objType.GetConstructor(Type.EmptyTypes) != null;
        }

        public override System.Collections.Generic.IEnumerable<string> GetSampleRoutes(string basePath, DynamicControllerInfo controller)
        {
            return WebApiConfig.GetSampleRoutes(basePath, controller);
        }

        public override object RunAction(DynamicAction objAction, DynamicControllerInfo controller, RestService objService, WebMethod verb, 
            IDictionary<string, object> arguments)
        {
            var dynController = new DynamicController
            {
                Service = objService,
                ControllerInfo = controller,
                SelectedAction = objAction
            };

            var objContext = objAction.InitContext(arguments);
            objContext.InitParams(objService.GlobalParameters);
            objContext.InitParams(controller.GlobalParameters);
            objContext.SetVar("Controller", dynController);
            foreach (DynamicParameter parameter in objAction.Parameters)
            {
                object val = null;
                if (parameter.IsOptional && parameter.DefaultValue.Enabled && !arguments.TryGetValue(parameter.Name, out val))
                {
                    objContext.SetVar(parameter.Name, parameter.ResolvedDefaultValue);
                }
            }
            var response = dynController.ProcessInternal(objContext, verb);
            return response?.EvaluateToReturn(objContext);
        }
    }
}