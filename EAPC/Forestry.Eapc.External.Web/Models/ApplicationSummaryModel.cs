using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Forestry.Eapc.External.Web.Models
{
    public class ApplicationSummaryModel
    {
        public Application.Application Application { get; }
        public List<ValidationFailure> ValidationFailures { get; }

        public ApplicationSummaryModel(
            Application.Application application,
            List<ValidationFailure> validationFailures)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
            ValidationFailures = validationFailures ?? new List<ValidationFailure>(0);
        }
    }
}
