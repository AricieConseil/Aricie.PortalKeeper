using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using Aricie.DNN.Modules.PortalKeeper;

namespace Aricie.PortalKeeper.DNN7
{
    public class DynamicHttpParameterDescriptor : HttpParameterDescriptor
    {

        private DynamicParameter _DynamicParameter;

        public DynamicHttpParameterDescriptor(DynamicParameter objDynamicParameter, HttpActionDescriptor actionDescriptor)
            : base(actionDescriptor)
        {
            _DynamicParameter = objDynamicParameter;
        }

        public override string ParameterName
        {
            get { return _DynamicParameter.Name; }
        }

        public override Type ParameterType
        {
            get { return _DynamicParameter.EditableType.GetDotNetType(); }
        }

        public override bool IsOptional
        {
            get
            {
                return _DynamicParameter.IsOptional;
            }
        }

        public override object DefaultValue
        {
            get
            {
               return _DynamicParameter.ResolvedDefaultValue;
            }
        }

        public override Collection<T> GetCustomAttributes<T>()
        {

            var toReturn = new List<T>(base.GetCustomAttributes<T>());
            toReturn.AddRange(from objAttribute in _DynamicParameter.DynamicAttributes.ItemsSurrogated where objAttribute is T select (T)(Object)objAttribute);
            return new Collection<T>(toReturn);
        }
    }
}