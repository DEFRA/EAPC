using FluentValidation;
using FluentValidation.Results;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class ApplicantValidator: AbstractValidator<Applicant>
    {

        public ApplicantValidator()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Applicant));

            RuleFor(x => x.PersonName)
                .NotEmpty()
                .WithMessage("A contact name must be provided for this application.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage("A company name must be provided for this application.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.Region)
                .NotNull()
                .WithMessage("A region must be provided for this application.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("A contact email address must be provided for this application.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
                
            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("A valid contact email address must be provided for this application.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.Telephone)
                .NotEmpty()
                .WithMessage("A contact telephone number must be provided for this application.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.Telephone)
                .Custom((str, ctx) =>
                {
                    var isValid = EapcTelephoneAttribute.IsValidTelephoneNumber(str);
                    if (!isValid)
                    {
                        var failure = new ValidationFailure(ctx.PropertyName, "It looks like the provided telephone number may not be a valid phone number.");
                        failure.CustomState = new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction);
                        ctx.AddFailure(failure);
                    }
                })
                .When(x => !string.IsNullOrWhiteSpace(x.Telephone));


            RuleFor(x => x.ProfessionalOperatorNumber)
                .NotEmpty()
                .WithMessage("Your six-digit professional operator number must be provided.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.ProfessionalOperatorNumber)
                .Matches(DataValueConstants.ProfessionalOperatorNumberRegex)
                .When(x => !string.IsNullOrEmpty(x.ProfessionalOperatorNumber))
                .WithMessage("Your professional operator number should be six-digits.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));

            RuleFor(x => x.ExportStatus)
                .NotNull()
                .WithMessage("You must tell us if this is an application for a new or reforwarding export certificate.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }
    }
}
