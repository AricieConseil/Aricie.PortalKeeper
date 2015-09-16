using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;
using Aricie.DNN.Modules.PortalKeeper;
using Aricie.DNN.Services;
using Aricie.PortalKeeper.DNN7.WebAPI;
using System.Linq;


namespace Aricie.PortalKeeper.DNN7
{
    public static class WebApiExtensions
    {


        public static HttpRequestMessage GetRequest(this PortalKeeperContext<SimpleEngineEvent> context)
        {
            return (HttpRequestMessage) context.GetVar("Request");
        }

        public static IHttpRouteData GetRouteData(this PortalKeeperContext<SimpleEngineEvent> context)
        {
            return (IHttpRouteData)context.GetVar("RouteData");
        }

        public static HttpResponseMessage GetResponse(this PortalKeeperContext<SimpleEngineEvent> context)
        {
            return (HttpResponseMessage) context.GetVar("Response");
        }

        public static void SetResponse(this PortalKeeperContext<SimpleEngineEvent> context, HttpResponseMessage objresponse)
        {
            context.SetVar("Response", objresponse);
        }

        public static HttpResponseMessage CreateResponse(this CreateHttpResponseInfo objCreateHttpResponseInfo, PortalKeeperContext<SimpleEngineEvent> objContext)
        {
            var request = objContext.GetRequest();
            HttpResponseMessage toReturn = request.CreateResponse(objCreateHttpResponseInfo.StatusCode);

            var objContent = objCreateHttpResponseInfo.EvaluateToReturn(objContext);

            var configuration = WebApiConfig.GetConfiguration();

            if (objContent!=null)
            {
                
                
                var objContentType = objContent.GetType();
                IEnumerable<MediaTypeFormatter> formatters = configuration.Formatters;

                MediaTypeFormatter selectedFormatter;
                MediaTypeHeaderValue mediaType = null; 
                if (objCreateHttpResponseInfo.CustomFormatter.Enabled)
                {
                    if (!objCreateHttpResponseInfo.CustomFormatter.Entity.KnownFormatter.IsNullOrEmpty())
                    {
                        selectedFormatter =
                            formatters.First(
                                objFormatter =>
                                    objFormatter.GetType().Name ==
                                    objCreateHttpResponseInfo.CustomFormatter.Entity.KnownFormatter);
                    }
                    else
                    {
                        selectedFormatter =
                            objCreateHttpResponseInfo.CustomFormatter.Entity.FormatterExpression.Evaluate(objContext, objContext, typeof (MediaTypeFormatter)) as
                                MediaTypeFormatter;
                    }

                }
                else
                {
                    IContentNegotiator contentNegotiator = configuration.Services.GetContentNegotiator();
                    if (contentNegotiator == null)
                    {
                        throw new InvalidOperationException("No Content Negotiator");
                    }
                    ContentNegotiationResult contentNegotiationResult = contentNegotiator.Negotiate(objContentType, request, formatters);
                    if (contentNegotiationResult == null)
                    {
                        throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotAcceptable);
                    }
                    selectedFormatter = contentNegotiationResult.Formatter;
                    mediaType = contentNegotiationResult.MediaType;
                }
                if (objCreateHttpResponseInfo.CustomMediaType.Enabled)
                {
                    var strMediaType = objCreateHttpResponseInfo.CustomMediaType.Entity.KnownHeader;

                    if (strMediaType.IsNullOrEmpty())
                    {
                        strMediaType =
                            objCreateHttpResponseInfo.CustomMediaType.Entity.CustomHeader.GetValue(objContext, objContext);
                    }
                    if (!strMediaType.IsNullOrEmpty())
                    {
                        mediaType =
                            formatters.SelectMany(form => form.SupportedMediaTypes)
                                .First(
                                    objHeader =>
                                        StringComparer.OrdinalIgnoreCase.Compare(objHeader.MediaType,
                                            objCreateHttpResponseInfo.CustomMediaType.Entity.KnownHeader) == 0) ??
                            new MediaTypeHeaderValue(strMediaType);
                    }
                    
                }
                
                toReturn.Content = new ObjectContent(objContentType, objContent, selectedFormatter,
                    mediaType);
            }
            if (objCreateHttpResponseInfo.CustomHttpHeaders.Enabled)
            {
                foreach (var objPair in objCreateHttpResponseInfo.CustomHttpHeaders.Entity.EvaluateVariables(objContext, objContext))
                {
                    if (objPair.Value==null)
                    {
                        throw new InvalidOperationException("Header \""+ objPair.Key +"\" has a null value");
                    }
                    var collectionCast = objPair.Value as IEnumerable<String>;
                    if (collectionCast != null)
                    {
                        toReturn.Headers.Add(objPair.Key, collectionCast);    
                    }
                    else toReturn.Headers.Add(objPair.Key, objPair.Value.ToString());    
                }
                
            }
            
            return toReturn;
        }

        public static HttpMethod ToHttpMethod(this WebMethod objWebMethod)
        {
            switch (objWebMethod)
            {

                case WebMethod.Get:
                    return HttpMethod.Get;
                case WebMethod.Post:
                    return HttpMethod.Post;
                case WebMethod.Put:
                    return HttpMethod.Put;
                case WebMethod.Delete:
                    return HttpMethod.Delete;
                case WebMethod.Options:
                    return HttpMethod.Options;
                case WebMethod.Head:
                    return HttpMethod.Head;
                default:
                    return HttpMethod.Get;
            }
        }


        public static WebMethod ToWebMethod(this HttpMethod objWebMethod)
        {
            switch (objWebMethod.Method)
            {

                case @"GET":
                    return WebMethod.Get;
                case @"POST":
                    return WebMethod.Post;
                case @"PUT":
                    return WebMethod.Put;
                case @"DELETE":
                    return WebMethod.Delete;
                case @"OPTIONS":
                    return WebMethod.Options;
                case @"HEAD":
                    return WebMethod.Head;
                default:
                    return WebMethod.Get;
            }
        }



    }

}