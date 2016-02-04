using System.Net;
using System.Net.Http;
using Aricie.DNN.Modules.PortalKeeper;

namespace Aricie.PortalKeeper.DNN7
{
    public class DynamicController : DotNetNuke.Web.Api.DnnApiController
    {

        public const string ActionContextParameterName = "actionContext";

        public RestService Service { get; set; }

        public DynamicControllerInfo ControllerInfo { get; set; }


        public DynamicAction SelectedAction { get; set; }

        public HttpResponseMessage Get(PortalKeeperContext<SimpleEngineEvent> actionContext)
        {
            return this.Process(actionContext, WebMethod.Get);
        }

        public HttpResponseMessage Post(PortalKeeperContext<SimpleEngineEvent> actionContext)
        {
            return this.Process(actionContext, WebMethod.Post);
        }

        public HttpResponseMessage Put(PortalKeeperContext<SimpleEngineEvent> actionContext)
        {
            return this.Process(actionContext, WebMethod.Put);
        }

        public HttpResponseMessage Delete(PortalKeeperContext<SimpleEngineEvent> actionContext)
        {
            return this.Process(actionContext, WebMethod.Delete);
        }

        public HttpResponseMessage Head(PortalKeeperContext<SimpleEngineEvent> actionContext)
        {
            return this.Process(actionContext, WebMethod.Head);
        }

        public HttpResponseMessage Options(PortalKeeperContext<SimpleEngineEvent> actionContext)
        {
            return this.Process(actionContext, WebMethod.Options);
        }

        public virtual HttpResponseMessage Process(PortalKeeperContext<SimpleEngineEvent> actionContext, WebMethod verb )
        {

            var response = ProcessInternal(actionContext, verb);
            
            return response?.CreateResponse(actionContext);

        }

        public virtual CreateHttpResponseInfo ProcessInternal(PortalKeeperContext<SimpleEngineEvent> actionContext, WebMethod verb)
        {
            actionContext.SetVar("HttpVerb", verb);
            this.SelectedAction.ProcessRules(actionContext, SimpleEngineEvent.Run, true, true);
            var response = actionContext.GetResponse();
            if (response == null && this.SelectedAction.DefaultResponse.Enabled)
            {
                response = this.SelectedAction.DefaultResponse.Entity;
            }

            return response;
        }

    }
}