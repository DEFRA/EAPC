using FluentValidation;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Validation
{
    public class ApplicationValidator : AbstractValidator<Application>
    {
        public ApplicationValidator()
        {
            RuleFor(x => x.Identifier).NotEmpty();
            RuleFor(x => x.ReferenceIdentifier).NotEmpty();
            RuleFor(x => x.CreationDate).NotNull();
            RuleFor(x => x.Section1).NotNull().SetValidator(new Section1Validator());
            RuleFor(x => x.Section2).NotNull().SetValidator(new Section2Validator());
            RuleFor(x => x.Section3).NotNull().SetValidator(new Section3Validator());
            RuleFor(x => x.Section4).NotNull().SetValidator(new Section4Validator());
            RuleFor(x => x.Section5).NotNull().SetValidator(new Section5Validator());
            RuleFor(x => x.Section6).NotNull().SetValidator(new Section6Validator());
            RuleFor(x => x.Section7).NotNull().SetValidator(new Section7Validator());
            RuleFor(x => x.Applicant).NotNull().SetValidator(new ApplicantValidator());
            RuleFor(x => x.SupportingDocumentsSection).NotNull()
                .SetValidator(new SupportingInformationSectionValidator());

            CheckKeyDatesSection3();
            CheckKeyDatesSection5();
        }

        public ApplicationValidator(string sectionAction)
        {
            switch (sectionAction)
            {
                case nameof(Applicant):
                    RuleFor(x => x.Applicant).NotNull().SetValidator(new ApplicantValidator());
                    break;
                case nameof(Section1):
                    RuleFor(x => x.Section1).NotNull().SetValidator(new Section1Validator());
                    break;
                case nameof(Section2):
                    RuleFor(x => x.Section2).NotNull().SetValidator(new Section2Validator());
                    break;
                case nameof(Section3):
                    RuleFor(x => x.Section3).NotNull().SetValidator(new Section3Validator());
                    CheckKeyDatesSection3();
                    break;
                case nameof(Section4):
                    RuleFor(x => x.Section4).NotNull().SetValidator(new Section4Validator());
                    break;
                case nameof(Section5):
                    RuleFor(x => x.Section5).NotNull().SetValidator(new Section5Validator());
                    CheckKeyDatesSection5();
                    break;
                case nameof(Section6):
                    RuleFor(x => x.Section6).NotNull().SetValidator(new Section6Validator());
                    break;
                case nameof(Section7):
                    RuleFor(x => x.Section7).NotNull().SetValidator(new Section7Validator());
                    break;
                case nameof(SupportingDocumentsSection):
                    RuleFor(x => x.SupportingDocumentsSection).NotNull().SetValidator(new SupportingInformationSectionValidator());
                    break;
            }
        }

        private void CheckKeyDatesSection3()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section3));

            RuleFor(x => x.Section3.DateOfExport!.Value.Date)
                .GreaterThan(x => x.Section5.DateOfTreatment!.Value.Date)
                .When(x => x.Section5.DateOfTreatment.HasValue && x.Section3.DateOfExport.HasValue)
                .WithMessage("The date of export should occur after the date of treatment.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }

        private void CheckKeyDatesSection5()
        {
            var appModel = ApplicationFormSectionsMetaModel.GetSectionItem(nameof(Section5));

            RuleFor(x => x.Section5.DateOfTreatment!.Value.Date)
                .LessThan(x => x.Section3.DateOfExport!.Value.Date)
                .When(x => x.Section5.DateOfTreatment.HasValue && x.Section3.DateOfExport.HasValue)
                .WithMessage("The date of treatment should occur before the date of export.")
                .WithState(_ => new CustomValidationState(appModel.SectionDisplayName, appModel.SectionAction));
        }
    }
}
