using System;
using System.Collections.Generic;
using FluentValidation.Results;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services.Validation;

namespace Forestry.Eapc.External.Web.Services
{
    public class ValidationProvider
    {
        public List<ValidationFailure> Validate(Application application)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            
            var validator = new ApplicationValidator();
            var result = validator.Validate(application);
            return result.Errors;
        }

        public List<ValidationFailure> ValidateSection(Application application, string sectionAction)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            if (string.IsNullOrEmpty(sectionAction)) throw new ArgumentException(nameof(sectionAction));

            var validator = new ApplicationValidator(sectionAction);
            var result = validator.Validate(application);
            return result.Errors;
        }
    }
}
