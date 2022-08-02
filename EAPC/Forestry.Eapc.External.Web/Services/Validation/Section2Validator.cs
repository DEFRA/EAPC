using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class Section2Validator : AbstractValidator<Section2>
    {
        public Section2Validator()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section2));

            RuleFor(x => x.GoodsInspectionAddress.Line1)
                .NotEmpty()
                .When(x =>x.InspectionNotRequired == false)
                .WithMessage("Line 1 of the inspection address must be provided when an inspection is required.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }
    }
}
