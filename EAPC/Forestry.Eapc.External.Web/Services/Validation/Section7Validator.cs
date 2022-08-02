using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class Section7Validator : AbstractValidator<Section7>
    {
        public Section7Validator()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section7));

            RuleFor(x => x.CertificateDeliveryAddress.ContactName)
                .NotEmpty()
                .WithMessage("A contact name for your certificate delivery address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.CertificateDeliveryAddress.Line1)
                .NotEmpty()
                .WithMessage("Line 1 of your certificate delivery address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.CertificateDeliveryAddress.Line2)
                .NotEmpty()
                .WithMessage("Line 2 of your certificate delivery address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.CertificateDeliveryAddress.Line3)
                .NotEmpty()
                .WithMessage("Line 3 of your certificate delivery address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.CertificateDeliveryAddress.PostalCode)
                .NotEmpty()
                .WithMessage("The postal code of your certificate delivery address must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.CustomerCreditNumber)
                .NotEmpty()
                .WithMessage("Your customer credit number must be provided.")
                .WithState(x => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }
    }
}
