using System;
using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class Section3Validator : AbstractValidator<Section3>
    {
        public Section3Validator()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section3));

            RuleFor(x => x.PortOfExport)
                .NotEmpty()
                .WithMessage("Please provide a port of export for the consignment")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.PortOfImport)
                .NotEmpty()
                .WithMessage("Please provide a port of import for the consignment.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.NameOfConsignee)
                .NotEmpty()
                .WithMessage("Please provide a consignee contact name.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
                
            RuleFor(x => x.AddressOfConsignee.Line1)
                .NotEmpty()
                .WithMessage("Please provide line 1 of the consignee contact address.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.AddressOfConsignee.Line2)
                .NotEmpty()
                .WithMessage("Please provide line 2 of the consignee contact address.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.AddressOfConsignee.Line3)
                .NotEmpty()
                .WithMessage("Please provide line 3 of the consignee contact address.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.CountryOfDestination)
                .NotEmpty()
                .WithMessage("Please provide a country of destination for the consignment.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.DateOfExport)
                .NotNull()
                .WithMessage("Please provide a date of export for the consignment")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
            
            RuleFor(x => x.DateOfExport)
                .Must(date => date.Value.Date >= DateTime.Today)
                .When(x => x.DateOfExport.HasValue)
                .WithMessage("The date of export for the consignment must be in the future")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }
    }
}
