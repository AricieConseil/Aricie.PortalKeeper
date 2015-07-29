using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Aricie.DNN.Modules.PortalKeeper;

namespace Aricie.PortalKeeper.DNN7
{
    public class DynamicActionSelector : ApiControllerActionSelector
    {

        private readonly IHttpActionSelector _originalActionSelector;

        private readonly HttpConfiguration _configuration;


        public DynamicActionSelector(HttpConfiguration configuration)
            : base()
        {
            this._configuration = configuration;
        }

        public DynamicActionSelector(HttpConfiguration configuration, IHttpActionSelector originalSelector)
            : base()
        {
            this._configuration = configuration;
            this._originalActionSelector = originalSelector;

        }

        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            var dynController = controllerContext.Controller as DynamicController;
            if (dynController != null)
            {
                //We want to use http verb only from inherited selection.
                var tempData = controllerContext.RouteData;
                controllerContext.RouteData = new HttpRouteData(controllerContext.RouteData.Route);
                var reflectedCandidate = (ReflectedHttpActionDescriptor) base.SelectAction(controllerContext);
                controllerContext.RouteData = tempData;


                string actionName = null;
                bool hasActionRouteKey = controllerContext.RouteData.Values.TryGetValue<string>("action", ref actionName);

                var candidateActions = new List<DynamicAction>();
                foreach (DynamicAction candidateAction in dynController.ControllerInfo.DynamicActions)
                {
                    if (candidateAction.Enabled
                          && candidateAction.HttpVerbs.All.Contains(controllerContext.Request.Method.ToWebMethod())
                          && ((!hasActionRouteKey && !candidateAction.EnforceActionName) || (StringComparer.OrdinalIgnoreCase.Compare(candidateAction.Name, actionName) == 0)))
                    {
                        candidateActions.Add(candidateAction);
                    }
                }

                if (candidateActions.Count > 1)
                {
                    
                    IDictionary<string, object> values = controllerContext.RouteData.Values;
                    var strs = new HashSet<string>(values.Keys, StringComparer.OrdinalIgnoreCase);
                    strs.Remove("controller");
                    if (hasActionRouteKey)
                    {
                        strs.Remove("action");
                    }
                    if (controllerContext.Request.RequestUri != null && !string.IsNullOrEmpty(controllerContext.Request.RequestUri.Query))
                    {
                        foreach (KeyValuePair<string, string> queryNameValuePair in controllerContext.Request.GetQueryNameValuePairs())
                        {
                            strs.Add(queryNameValuePair.Key);
                        }
                    }
                    foreach (DynamicAction objMethod in new List<DynamicAction>(candidateActions))
                    {
                        foreach (string strRouteParam in strs)
                        {
                            if (!objMethod.ParametersDictionary.ContainsKey(strRouteParam))
                            {
                                candidateActions.Remove(objMethod);
                            }
                        }     
                    }
                    
                }
                if (candidateActions.Count > 1)
                {
                    candidateActions = candidateActions.GroupBy<DynamicAction, Int32>(method => method.Parameters.Count)
                        .OrderByDescending(g => g.Key)
                        .First()
                        .ToList();    
                }
                

                
                string templatedMessage;
                switch (candidateActions.Count)
                {
                    case 0:
                        templatedMessage= DotNetNuke.Services.Localization.Localization.GetString("ActionNotFound.Message", PortalKeeperContext<RequestEvent>.SharedResourceFile);
                        throw new HttpResponseException(controllerContext.Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            string.Format(templatedMessage, new object[3] { (object)controllerContext.Request.RequestUri.ToString(), controllerContext.ControllerDescriptor.ControllerName, actionName })));
                    case 1:
                        var candidateDynamicMethod = candidateActions[0]; 
                        var toReturn = new DynamicHttpActionDescriptor(candidateDynamicMethod, reflectedCandidate);
                        
                        return toReturn;
                    default:
                        templatedMessage = DotNetNuke.Services.Localization.Localization.GetString("AmbiguousAction.Message", PortalKeeperContext<RequestEvent>.SharedResourceFile);
                        throw new HttpResponseException(controllerContext.Request.CreateErrorResponse(HttpStatusCode.Ambiguous,
                            string.Format(templatedMessage, new object[3] { controllerContext.Request.RequestUri.ToString(), controllerContext.ControllerDescriptor.ControllerName, actionName })));
                }

            }
            else
            {
                if (_originalActionSelector != null)
                {
                    return _originalActionSelector.SelectAction(controllerContext);
                }
                return base.SelectAction(controllerContext);  
            }
            
        }

        //public static HttpActionDescriptor GetActionDescriptor( DynamicAction dynamicMethod, ReflectedHttpActionDescriptor originalDescriptor)
        //{
        //    var toReturn = new DynamicHttpActionDescriptor(dynamicMethod, originalDescriptor);


        //    return toReturn;
        //}

    
    }
}