using System;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services
{
    /// <summary>
    /// Contains extensions to the <see cref="Application"/> class that ensures this sort of logic
    /// is separated out from the model class.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Sets various properties on the <see cref="Application.Applicant"/> model to reflect
        /// those read from the provided <see cref="ExternalUser"/>.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="user">The user whose details should be copied to the application.</param>
        public static void ImplantUser(this Application application, ExternalUser user)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            if (user == null) throw new ArgumentNullException(nameof(user));

            application.Applicant.ProfessionalOperatorNumber = user.ProfessionalOperatorNumber;
            application.Applicant.CompanyName = user.CompanyName;
            application.Applicant.Email = user.Email;
            application.Applicant.PersonName = user.FullName;
            application.Applicant.Region = user.HomeNation;
            application.Applicant.Telephone = user.Telephone;
            application.Section7.CustomerCreditNumber = user.CreditAccountReference;
        }
    }
}
