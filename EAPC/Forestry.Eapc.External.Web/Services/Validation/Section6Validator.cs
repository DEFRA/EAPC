using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class Section6Validator : AbstractValidator<Section6>
    {
        public Section6Validator()
        {
            {
                var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section6));

                RuleFor(x => x.AdditionalDeclarations)
                    .NotEmpty()
                    .When(x => x.AdditionalDeclarationsNotRequired == false)
                    .WithMessage(
                        "Additional Declaration(s) must be provided unless you have specified they are not required.")
                    .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
            }
        }
    }
}
