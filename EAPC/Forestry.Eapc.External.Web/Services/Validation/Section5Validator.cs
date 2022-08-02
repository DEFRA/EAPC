using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class Section5Validator : AbstractValidator<Section5>
    {
        public Section5Validator()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section5));

            RuleFor(x => x.Treatment)
                .NotNull()
                .WithMessage("Please select a treatment type, or select 'None' if no treatment is applicable.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.TreatmentOtherText)
                .NotEmpty()
                .When(x => x.Treatment == TreatmentType.Other)
                .WithMessage("Please enter the treatment method used when 'Other' has been chosen.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.Chemical)
                .NotEmpty()
                .WithMessage("Please enter a treatment chemical, or select 'None' if no treatment is applicable.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.ChemicalOtherText)
                .NotEmpty()
                .When(x => x.Chemical == TreatmentChemical.Other)
                .WithMessage("Please enter the treatment chemical used when 'Other' has been chosen.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }
    }
}
