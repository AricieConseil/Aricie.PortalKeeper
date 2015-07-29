using System;
using System.Web.Http.Filters;
using Aricie.DNN.Modules.PortalKeeper;
using Aricie.PortalKeeper.DNN7.WebAPI;
using System.Linq;
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


    }
}