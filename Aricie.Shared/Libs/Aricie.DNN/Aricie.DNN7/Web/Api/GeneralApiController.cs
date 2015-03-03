using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;


namespace Aricie.DNN7.Web.Api
{
    public abstract class GeneralApiControllerBase<TResource,TId>:DotNetNuke.Web.Api.DnnApiController
    {

        public virtual HttpResponseMessage GetAll()
        {
            IEnumerable<TResource> values = this.GetList();
            return Request.CreateResponse<IEnumerable<TResource>>(HttpStatusCode.OK, values);
        }

        public abstract IEnumerable<TResource> GetList();

        public virtual  HttpResponseMessage Get(TId id)
        {
            TResource value = this.GetByKey(id);
            if (value == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return this.Request.CreateResponse<TResource>(HttpStatusCode.OK, value);
        }

        public abstract TResource GetByKey(TId id);

        public abstract void Post(TResource item);

        public abstract void Put(TResource item);

        public abstract void Delete(TResource item);

    }
}
