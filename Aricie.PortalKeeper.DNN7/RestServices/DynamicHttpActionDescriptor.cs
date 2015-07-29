using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Aricie.DNN.Modules.PortalKeeper;

namespace Aricie.PortalKeeper.DNN7
{
    public class DynamicHttpActionDescriptor : ReflectedHttpActionDescriptor
    {

        private DynamicAction _DynamicMethod;
        private ReflectedHttpActionDescriptor _OriginalDescriptor;
        private Collection<HttpParameterDescriptor> _Parameters;

        public DynamicHttpActionDescriptor(DynamicAction objMethod, ReflectedHttpActionDescriptor originalDescriptor)
            : base(originalDescriptor.ControllerDescriptor, originalDescriptor.MethodInfo)
        {
            _DynamicMethod = objMethod;
            _OriginalDescriptor = originalDescriptor;
            _Parameters = InitializeParameterDescriptors();
        }
        public override Task<object> ExecuteAsync(HttpControllerContext controllerContext,
            IDictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            var objContext = _DynamicMethod.InitContext(arguments);
            objContext.SetVar("Request", controllerContext.Request);
            objContext.SetVar("RouteData", controllerContext.RouteData);
            var newDico = new Dictionary<string, object>(1) {{DynamicController.ActionContextParameterName, objContext}};
            var dynController = controllerContext.Controller as DynamicController;
            if (dynController != null)
            {
                dynController.SelectedAction = _DynamicMethod;
            }
            else
            {
                throw new ApplicationException("DynamicActionDescriptor is tied to Dynamic Controllers");
            }

            return base.ExecuteAsync(controllerContext, newDico, cancellationToken);
        }

        public override Collection<HttpParameterDescriptor> GetParameters()
        {
            return _Parameters;
        }

        public override Collection<T> GetCustomAttributes<T>()
        {

            var toReturn = new List<T>(base.GetCustomAttributes<T>());
            toReturn.AddRange(from objAttribute in _DynamicMethod.DynamicAttributes.ItemsSurrogated where objAttribute is T select (T)(Object)objAttribute);
            return new Collection<T>(toReturn);
        }

        private Collection<HttpMethod> _SupportedHttpMethods;

        public override Collection<HttpMethod> SupportedHttpMethods
        {
            get
            {
                if (_SupportedHttpMethods == null)
                {
                    _SupportedHttpMethods =
                        new Collection<HttpMethod>(
                            _DynamicMethod.HttpVerbs.All.ConvertAll(objWebMethod => objWebMethod.ToHttpMethod()));
                }
                return _SupportedHttpMethods;
            }
        }


        public override string ActionName
        {
            get
            {
                return _DynamicMethod.Name;
            }
        }

        private Collection<HttpParameterDescriptor> InitializeParameterDescriptors()
        {
            var typedParams =
                _DynamicMethod.Parameters.Select(
                    objParam => (HttpParameterDescriptor)new DynamicHttpParameterDescriptor(objParam, _OriginalDescriptor)).ToList();
            return new Collection<HttpParameterDescriptor>(typedParams);
        }
        
    }
}