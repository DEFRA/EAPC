using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class SupportingInformationSectionValidator : AbstractValidator<SupportingDocumentsSection>
    {
        public SupportingInformationSectionValidator()
        {
            {
                var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(SupportingDocumentsSection));

                RuleFor(x => x.SupportingDocuments)
                    .NotEmpty()
                    .When(x => x.SupportingDocumentationNotRequired == false)
                    .WithMessage(
                        "Supporting documentation must be provided unless you have specified that it is not required.")
                    .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
            }
        }
    }
}
