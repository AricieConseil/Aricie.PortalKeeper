using System;
using System.Linq;
using System.Web.Http.Validation;
using Aricie.ComponentModel;

namespace Aricie.PortalKeeper.DNN7.WebAPI
{
    public class BodyModelValidator : IBodyModelValidator
    {
        private IBodyModelValidator _defaultValidator;

        public BodyModelValidator (IBodyModelValidator originalValidator)
        {
            if ( originalValidator != null)
            {
                _defaultValidator = originalValidator;
            }
            else
            {
                _defaultValidator = new DefaultBodyModelValidator();
            }
            
        }

        public bool Validate(object model, Type type, System.Web.Http.Metadata.ModelMetadataProvider metadataProvider, System.Web.Http.Controllers.HttpActionContext actionContext, string keyPrefix)
        {
            if (model.GetType().GetCustomAttributes(true).Contains(new SkipModelValidationAttribute()))
            {
                return true;
            }
            else {
                return _defaultValidator.Validate(model, type, metadataProvider, actionContext, keyPrefix);
            }
        }
    }
}