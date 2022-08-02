using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class Section4Validator : AbstractValidator<Section4>
    {
        public Section4Validator()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section4));

            RuleFor(x => x.DescriptionOfProducts)
                .NotEmpty()
                .WithMessage("Please provide a description of the products being exported.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.CommodityType)
                .NotEmpty()
                .WithMessage("Please provide a commodity type category for the products being exported.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.BotanicalNames)
                .NotEmpty()
                .WithMessage("Please provide at least one common / botanical name for the products being exported.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.WhereGrowns)
                .NotEmpty()
                .WithMessage("Please provide at least one place of origin for the products being exported.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Please provide at least one quantity metric for the products being exported.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleForEach(x => x.Quantity)
                .ChildRules(quantities =>
                {
                    quantities.RuleFor(q => q.OtherText)
                        .NotEmpty()
                        .When(q => q.Unit == QuantityUnit.Other)
                        .WithMessage("Please enter the quantity unit when 'Other' has been chosen.")
                        .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

                    quantities.RuleFor(q => q.Amount)
                        .NotNull()
                        .WithMessage("Please indicate an amount for the quantity of the consignment")
                        .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction))
                        .GreaterThan(0)
                        .WithMessage("Quantity amount must be greater than zero")
                        .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

                    quantities.RuleFor(q => q.Unit)
                        .NotNull()
                        .WithMessage("Please indicate a unit for the quantity of the consignment")
                        .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
                });

            RuleFor(x => x.MeansOfConveyance)
                .NotEmpty()
                .WithMessage("Please provide the means of conveyance for the products being exported.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.MeansOfConveyanceOtherText)
                .NotEmpty()
                .When(x => x.MeansOfConveyance == TransportType.Other)
                .WithMessage("Please enter the means of conveyance for the products being exported when 'Other' has been chosen.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }
    }
}
