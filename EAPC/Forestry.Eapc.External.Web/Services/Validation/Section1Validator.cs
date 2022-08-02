using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class Section1Validator : AbstractValidator<Section1>
    {
        public Section1Validator()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section1));

            RuleFor(x => x.ExporterName)
                .NotEmpty()
                .WithMessage("An exporter name must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.ExporterAddress.Line1)
                .NotEmpty()
                .WithMessage("Line 1 of your exporter address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.ExporterAddress.Line2)
                .NotEmpty()
                .WithMessage("Line 2 of your exporter address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.ExporterAddress.Line3)
                .NotEmpty()
                .WithMessage("Line 3 of your exporter address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.ExporterAddress.PostalCode)
                .NotEmpty()
                .WithMessage("The postal code of your exporter address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }
    }
}
