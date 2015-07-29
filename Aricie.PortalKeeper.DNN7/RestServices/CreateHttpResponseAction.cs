using System.ComponentModel;
using System.Net.Http;
using Aricie.DNN.Modules.PortalKeeper;

namespace Aricie.PortalKeeper.DNN7.Providers.ActionProviders
{
    [DisplayName("Create Http Response")]
    [Description("This provider allows to prepare an Http Response to be returned by a Web API Service.")]
    public class CreateHttpResponseAction :
        Aricie.DNN.Modules.PortalKeeper.AsyncEnabledActionProvider<SimpleEngineEvent>
    {

        public CreateHttpResponseAction()
        {
            CreateResponseInfo = new CreateHttpResponseInfo();
        }

        public CreateHttpResponseInfo CreateResponseInfo { get; set; }
        protected override bool Run(PortalKeeperContext<SimpleEngineEvent> actionContext, bool aSync)
        {
            var objResponse = this.CreateResponseInfo.CreateResponse(actionContext);
            actionContext.SetResponse(objResponse);
            return true;
        }
    }
}