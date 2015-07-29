using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using Aricie.DNN.Modules.PortalKeeper;

namespace Aricie.PortalKeeper.DNN7
{
    public class DynamicControllerDescriptor : HttpControllerDescriptor
    {

        private DynamicControllerInfo _ControllerInfo;
        private RestService _RestService;

        public DynamicControllerDescriptor(HttpConfiguration configuration, DynamicControllerInfo objControllerInfo, RestService objService)
            : base(configuration, objControllerInfo.Name, typeof(DynamicController))
        {
            _ControllerInfo = objControllerInfo;
            _RestService = objService;
        }

        public DynamicControllerDescriptor()
            : base()
        {
        }

        public override System.Collections.ObjectModel.Collection<T> GetCustomAttributes<T>()
        {

            var toReturn = new List<T>( base.GetCustomAttributes<T>());
            toReturn.AddRange(from objAttribute in _ControllerInfo.DynamicAttributes.ItemsSurrogated where (objAttribute is T ) select (T) (Object) objAttribute);
            return new Collection<T> (toReturn);
        }


        public override IHttpController CreateController(System.Net.Http.HttpRequestMessage request)
        {
            var dynController = new DynamicController {Service = _RestService, ControllerInfo = _ControllerInfo};

            return dynController;

        }


    }
}