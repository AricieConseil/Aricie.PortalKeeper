using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Aricie.DNN.Modules.PortalKeeper;
using Aricie.DNN.Services;
using System.Linq;
using Aricie.Services;

namespace Aricie.PortalKeeper.DNN7
{
    public class DynamicControllerSelector:System.Web.Http.Dispatcher.DefaultHttpControllerSelector
    {

        private readonly IHttpControllerSelector _originalControllerSelector ;

        private readonly HttpConfiguration _configuration;

        public DynamicControllerSelector(HttpConfiguration configuration) : base(configuration)
        {
            this._configuration = configuration;
        }

        public DynamicControllerSelector(HttpConfiguration configuration, IHttpControllerSelector originalSelector)
            : base(configuration)
        {
            this._configuration = configuration;
            this._originalControllerSelector = originalSelector;

        }

        //private Dictionary<string, HttpControllerDescriptor> _ControllerDescriptors;

        public Dictionary<string, HttpControllerDescriptor> ControllerDescriptors
        {
            get
            {
                var controllerDescriptors =
                    Aricie.Services.CacheHelper.GetGlobal<Dictionary<string, HttpControllerDescriptor>>();
                if (controllerDescriptors == null)
                {
                     controllerDescriptors =
                        new Dictionary<string, HttpControllerDescriptor>( StringComparer.OrdinalIgnoreCase);
                    foreach (var objService in PortalKeeperConfig.Instance.RestServices.RestServices)
                    {
                        foreach (var dynController in objService.DynamicControllers)
                        {
                            controllerDescriptors.Add(dynController.Name,
                            new DynamicControllerDescriptor(this._configuration, dynController, objService ));    
                        }

                        
                    }
                    CacheHelper.SetCacheDependant<Dictionary<string, HttpControllerDescriptor>>(controllerDescriptors, 
                                                                        PortalKeeperConfig.GetFilePath(true), Constants.Cache.GlobalExpiration);
                }
                return controllerDescriptors;
            }
        }


        public override IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            IDictionary<string, HttpControllerDescriptor> toReturn = _originalControllerSelector != null ? _originalControllerSelector.GetControllerMapping() : base.GetControllerMapping();
            foreach (var objPair in ControllerDescriptors)
            {
                toReturn.Add(objPair.Key, objPair.Value);
            }

            return toReturn;
        }

        public override System.Web.Http.Controllers.HttpControllerDescriptor SelectController(System.Net.Http.HttpRequestMessage request)
        {
            HttpControllerDescriptor httpControllerDescriptor = null;
            if (request != null)
            {
                string controllerName = this.GetControllerName(request);
                if (! string.IsNullOrEmpty(controllerName))
                {
                    ControllerDescriptors.TryGetValue(controllerName,out httpControllerDescriptor);
                }
            }
            if (httpControllerDescriptor != null)
            {
                return httpControllerDescriptor;
            }
            if (_originalControllerSelector != null)
            {
                return _originalControllerSelector.SelectController(request);
            }
            return base.SelectController(request);
        }
    }
}

