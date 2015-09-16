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

        public override ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
        {
            var baseLookup = base.GetActionMapping(controllerDescriptor);

            var dynController = controllerDescriptor as DynamicControllerDescriptor;
            if (dynController != null)
            {
                var candidateActions = dynController.ControllerInfo.DynamicActions
                    .Where(candidateAction => candidateAction.Enabled);
                var toReturnList = new List<HttpActionDescriptor>();
                foreach (DynamicAction objMethod in candidateActions)
                {
                    foreach (string verb in objMethod.HttpVerbs.All.Select(objVerb => objVerb.ToString()))
                    {
                        if (baseLookup.Contains(verb))
                        {
                            ReflectedHttpActionDescriptor originalDescriptor = baseLookup[verb].First() as ReflectedHttpActionDescriptor;
                            if (originalDescriptor != null)
                            {
                                toReturnList.Add(new DynamicHttpActionDescriptor(dynController.Service,
                                    dynController.ControllerInfo, objMethod, originalDescriptor));
                                break;
                            }
                            
                            
                        }
                    }
                }

                return toReturnList.ToLookup<HttpActionDescriptor, string>(objActionDesc => objActionDesc.ActionName);
            }
            return baseLookup;

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

                var candidateActions = dynController.ControllerInfo.DynamicActions
                    .Where(candidateAction => candidateAction.Enabled && candidateAction.HttpVerbs.All.Contains(controllerContext.Request.Method.ToWebMethod()) 
                        && ((!hasActionRouteKey && !candidateAction.EnforceActionName) 
                        || (StringComparer.OrdinalIgnoreCase.Compare(candidateAction.Name, actionName) == 0)))
                    .ToList();

                if (candidateActions.Count > 1)
                {
                    
                    IDictionary<string, object> values = controllerContext.RouteData.Values;
                    var strRouteParams = new HashSet<string>(values.Keys, StringComparer.OrdinalIgnoreCase);
                    strRouteParams.Remove("controller");
                    if (hasActionRouteKey)
                    {
                        strRouteParams.Remove("action");
                    }
                    if (controllerContext.Request.RequestUri != null && !string.IsNullOrEmpty(controllerContext.Request.RequestUri.Query))
                    {
                        foreach (KeyValuePair<string, string> queryNameValuePair in controllerContext.Request.GetQueryNameValuePairs())
                        {
                            strRouteParams.Add(queryNameValuePair.Key);
                        }
                    }
                    foreach (DynamicAction objMethod in new List<DynamicAction>(candidateActions))
                    {
                        //removing actions with dynamic parameters not found in routeData
                        foreach (var pairActionDynamicParam in objMethod.ParametersDictionary)
                        {
                            if (!pairActionDynamicParam.Value.IsOptional)
                            {
                                if (!pairActionDynamicParam.Value.DynamicAttributes.Items.Any(objAttr => objAttr is FromBodyAttribute))
                                {
                                    if (!strRouteParams.Contains(pairActionDynamicParam.Key))
                                    {
                                        candidateActions.Remove(objMethod);
                                    }    
                                }
                                
                            }
                        }
                        if (candidateActions.Count>1)
                        {
                            Object objDefaultValue = null;
                            foreach (string strRouteParam in strRouteParams)
                            {
                                if (!objMethod.ParametersDictionary.ContainsKey(strRouteParam) 
                                        && (!controllerContext.RouteData.Route.Defaults.TryGetValue(strRouteParam,out objDefaultValue) 
                                             || objDefaultValue != RouteParameter.Optional))
                                {
                                    candidateActions.Remove(objMethod);
                                }
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
                            string.Format(templatedMessage, new object[3] { controllerContext.Request.RequestUri != null ? controllerContext.Request.RequestUri.ToString() : "", controllerContext.ControllerDescriptor.ControllerName, actionName })));
                    case 1:
                        var candidateDynamicMethod = candidateActions[0];
                        var toReturn = new DynamicHttpActionDescriptor(dynController.Service,dynController.ControllerInfo, candidateDynamicMethod, reflectedCandidate);
                        
                        return toReturn;
                    default:
                        templatedMessage = DotNetNuke.Services.Localization.Localization.GetString("AmbiguousAction.Message", PortalKeeperContext<RequestEvent>.SharedResourceFile);
                        throw new HttpResponseException(controllerContext.Request.CreateErrorResponse(HttpStatusCode.Ambiguous,
                            string.Format(templatedMessage, new object[3] {controllerContext.Request.RequestUri != null ? controllerContext.Request.RequestUri.ToString() : "", controllerContext.ControllerDescriptor.ControllerName, actionName })));
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