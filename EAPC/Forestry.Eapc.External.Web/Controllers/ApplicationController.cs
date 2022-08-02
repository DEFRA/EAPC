using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Infrastructure.Display;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Forestry.Eapc.External.Web.Controllers
{
    [Authorize]
    [RequireCompletedProfile]
    [AutoValidateAntiforgeryToken]
    [Route("{controller}/{identifier}/{action}")]
    public class ApplicationController : Controller
    {
        private const string SaveFailureErrorMessage = "Sorry, there was a problem saving your application, please try again.";
        private readonly EditApplicationUseCase _editApplication;
        private readonly ValidationProvider _validationProvider;
        private static readonly List<SectionNavigationLinkModel> ApplicationFormSectionsModel = ApplicationFormSectionsMetaModel.Model; 

        public ApplicationController(EditApplicationUseCase editApplication, ValidationProvider validationProvider)
        {
            _editApplication = editApplication ?? throw new ArgumentNullException(nameof(editApplication));
            _validationProvider = validationProvider ?? throw new ArgumentNullException(nameof(validationProvider));
        }

        [HttpGet]
        public async Task<IActionResult> Applicant(
            string identifier,
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            this.AddViewBagFlagIfSectionErrorsResolved();

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Applicant));

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Applicant(
            string identifier,
            [Bind(Prefix = nameof(Applicant))] Applicant model,
            [FromForm(Name = PageConstants.NavigationButtonName)]
            NavigationDirection direction,
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var whereToGoNext = GetRedirectAction(direction, null, nameof(Applicant), nameof(Section1));
            var application = await FindApplicationAsync(identifier, cancellationToken);

            ApplyViewData(application);

            if (!IsDraft(application))
            {
                // do not save data, off we go to the next page
                return BuildRedirect(whereToGoNext);
            }

            // if we got here then we need to do the data edit bits
            application.Applicant = model; // we always set this here as it will be needed even in the event of invalid model state

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Applicant));

            if (!ModelState.IsValid && !showErrors)
            {
                return View(application);
            }

            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(application, cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(whereToGoNext, 
                    displaySaveDataMessage: direction == NavigationDirection.Save, 
                    showErrors:stillShowErrors) 
                : View(application);
        }
        
        [HttpGet]
        public async Task<IActionResult> Section1(string identifier, 
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);
            
            this.AddViewBagFlagIfSectionErrorsResolved();

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section1));

           
            
            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Section1(
            string identifier,
            [Bind(Prefix = nameof(Models.Application.Section1))] Section1 model,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction, 
            bool showErrors,
            CancellationToken cancellationToken)
        {

            var whereToGoNext = GetRedirectAction(direction, nameof(Applicant), nameof(Section1), nameof(Section2));
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            if (!IsDraft(application))
            {
                // do not save data, off we go to the next page
                return BuildRedirect(whereToGoNext);
            }

            application.Section1 = model;

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section1));

            if (!ModelState.IsValid && !showErrors)
            {
                return View(application);
            }

            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(
                application, 
                cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(whereToGoNext, 
                    displaySaveDataMessage: direction == NavigationDirection.Save, 
                    showErrors: stillShowErrors) 
                : View(application);
        }
        
        [HttpGet]
        public async Task<IActionResult> Section2(
            string identifier, 
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            this.AddViewBagFlagIfSectionErrorsResolved();

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section2));

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Section2(
            string identifier,
            [Bind(Prefix = nameof(Models.Application.Section2))] Section2 model,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction, 
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var whereToGoNext = GetRedirectAction(direction, nameof(Section1), nameof(Section2), nameof(Section3));
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            if (!IsDraft(application))
            {
                // do not save data, off we go to the next page
                return BuildRedirect(whereToGoNext);
            }

            application.Section2 = model;

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section2));

            if (!ModelState.IsValid && !showErrors)
            {
                return View(application);
            }

            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(
                application, 
                cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(whereToGoNext, 
                    displaySaveDataMessage: direction == NavigationDirection.Save,
                    showErrors:stillShowErrors)
                : View(application);
        }

        [HttpGet]
        public async Task<IActionResult> Section3(
            string identifier, 
            bool showErrors, 
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            this.AddViewBagFlagIfSectionErrorsResolved();

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section3));

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Section3(
            string identifier,
            [Bind(Prefix = nameof(Models.Application.Section3))] Section3 model,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction, 
            bool showErrors,
            CancellationToken cancellationToken)
        {                        
            var application = await FindApplicationAsync(identifier, cancellationToken);

            if (!ModelState.IsValid && !showErrors)
            {
                return View(application);
            }

            ApplyViewData(application);

            var whereToGoNext = GetRedirectAction(direction, nameof(Section2), nameof(Section3), nameof(Section4));

            if (!IsDraft(application))
            {
                // do not save data, off we go to the next page
                return BuildRedirect(whereToGoNext);
            }

            application.Section3 = model;

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section3));

            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(
                application, 
                cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(whereToGoNext, 
                    displaySaveDataMessage: direction == NavigationDirection.Save,
                    showErrors: stillShowErrors) 
                : View(application);
        }
        
        [HttpGet]
        public async Task<IActionResult> Section4(
            string identifier, 
            bool showErrors, 
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            this.AddViewBagFlagIfSectionErrorsResolved();

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section4));

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Section4(
            string identifier,
            [Bind(Prefix = nameof(Models.Application.Section4))] Section4 model,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction, 
            bool showErrors,
            CancellationToken cancellationToken)
        {                        
            var application = await FindApplicationAsync(identifier, cancellationToken);

            var quantityCount = model.Quantity.Count;
            model.Quantity.RemoveAll(x => x.Amount == 0 || !x.Unit.HasValue);
            
            ApplyViewData(application);

            if (!ModelState.IsValid && !showErrors)
            {
                //if we have removed invalid quantity objects in the POSTed model, then we also need to remove the validation errors.
                if (model.Quantity.Count < quantityCount)
                {
                    foreach (var propertyKey in ModelState.Keys.Where(x=>x.Contains(nameof(Application.Section4.Quantity))))
                    {
                        if (ModelState.GetValidationState(propertyKey) != ModelValidationState.Invalid) continue;

                        ModelState.Remove(propertyKey.EndsWith("Amount")
                            ? propertyKey.Replace("Amount", "Unit")
                            : propertyKey.Replace("Unit", "Amount"));

                        ModelState.Remove(propertyKey);
                    }
                }
                return View(application);
            }

            var whereToGoNext = GetRedirectAction(direction, nameof(Section3), nameof(Section4), nameof(Section5));

            if (!IsDraft(application))
            {
                // do not save data, off we go to the next page
                return BuildRedirect(whereToGoNext);
            }

            application.Section4 = model;

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section4));

            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(
                application, 
                cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(whereToGoNext, 
                    displaySaveDataMessage: direction == NavigationDirection.Save,
                    showErrors: stillShowErrors) 
                : View(application);
        }

        [HttpGet]
        public async Task<IActionResult> Section5(
            string identifier, 
            bool showErrors, 
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            this.AddViewBagFlagIfSectionErrorsResolved();

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section5));

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Section5(
            string identifier,
            [Bind(Prefix = nameof(Models.Application.Section5))] Section5 model,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction, 
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var whereToGoNext = GetRedirectAction(direction, nameof(Section4), nameof(Section5), nameof(Section6));
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            if (!IsDraft(application))
            {
                // do not save data, off we go to the next page
                return BuildRedirect(whereToGoNext);
            }

            application.Section5 = model;
         
            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section5));

            if (!ModelState.IsValid && !showErrors)
            {
                return View(application);
            }

            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(
                application, 
                cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(whereToGoNext, 
                    displaySaveDataMessage: direction == NavigationDirection.Save,
                    showErrors:stillShowErrors) 
                : View(application);
        }

    
        [HttpGet]
        public async Task<IActionResult> Section6(
            string identifier, 
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            this.AddViewBagFlagIfSectionErrorsResolved();
            
            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section6));

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Section6(
            string identifier,
            [Bind(Prefix = nameof(Models.Application.Section6))] Section6 model,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction, 
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);
            var whereToGoNext = GetRedirectAction(direction, nameof(Section5), nameof(Section6), nameof(Section7));

            if (!IsDraft(application))
            {
                // do not save data, off we go to the next page
                return BuildRedirect(whereToGoNext);
            }

            application.Section6 = model;
         
            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section6));

            if (!ModelState.IsValid && !showErrors)
            {
                return View(application);
            }

            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(
                application, 
                cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(whereToGoNext, 
                    displaySaveDataMessage: direction == NavigationDirection.Save,
                    showErrors:stillShowErrors) 
                : View(application);
        }

        [HttpGet]
        public async Task<IActionResult> Section7(
            string identifier, 
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);

            this.AddViewBagFlagIfSectionErrorsResolved();

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section7));

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Section7(
            string identifier,
            [Bind(Prefix = nameof(Models.Application.Section7))] Section7 model,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction, 
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);
            var whereToGoNext = GetRedirectAction(direction, nameof(Section6), nameof(Section7), nameof(SupportingDocumentsSection));

            if (!IsDraft(application))
            {
                // do not save data, off we go to the next page
                return BuildRedirect(whereToGoNext);
            }

            application.Section7 = model;
         
            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(Models.Application.Section7));

            if (!ModelState.IsValid && !showErrors)
            {
                return View(application);
            }

            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(
                application, 
                cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(whereToGoNext, 
                    displaySaveDataMessage: direction == NavigationDirection.Save,
                    showErrors:stillShowErrors) 
                : View(application);
        }
        
        [HttpGet]
        public async Task<IActionResult> SupportingDocumentsSection(string identifier, string? error, CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application, error);
            return View(application);
        }
        
        [HttpPost]
        public async Task<IActionResult> SupportingDocumentsSection(
            string identifier,
            [FromServices] RemoveSupportingDocumentUseCase useCase,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction,
            [FromForm(Name = "SupportingDocumentsSection.SupportingDocumentationNotRequired")] bool supportingDocumentationNotRequired,
            bool showErrors,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);

            if (showErrors)
                ApplySectionValidationModelErrors(application, nameof(SupportingDocumentsSection));

            if (!ModelState.IsValid && !showErrors)
            {
                return View(application);
            }

            if (supportingDocumentationNotRequired)
            {
                application.SupportingDocumentsSection.SupportingDocumentationNotRequired = true;

                if (application.SupportingDocumentsSection.SupportingDocuments.Any())
                {
                    foreach (var supportingDocument in application.SupportingDocumentsSection.SupportingDocuments)
                    {
                        await useCase.RemoveSupportingDocument(supportingDocument.Identifier, cancellationToken);  
                    }
                    
                    application.SupportingDocumentsSection.SupportingDocuments = new List<SupportingDocument>();
                }
            }

            application.SupportingDocumentsSection.SupportingDocumentationNotRequired =
                supportingDocumentationNotRequired;
           
            var stillShowErrors = this.AddTempDataFlagIfSectionErrorsNowResolved(showErrors);

            var saveResult = await SaveChangesAsync(
                application, 
                cancellationToken);

            return saveResult.IsSuccess 
                ? BuildRedirect(GetRedirectAction(direction, nameof(Section7), nameof(SupportingDocumentsSection), nameof(CertificatePreview)), 
                    displaySaveDataMessage: direction == NavigationDirection.Save,
                    showErrors:stillShowErrors) 
                : View(application);
        }

        /// <summary>
        /// This is an action method to explicitly support the uploading of file attachments from the <see cref="SupportingDocumentsSection(string,string?,System.Threading.CancellationToken)"/> view.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AttachSupportingDocumentation(
            string identifier,
            IFormFileCollection supportingDocumentationFiles,
            [FromServices] StoreSupportingDocumentsUseCase useCase,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            var user = new ExternalUser(User);

            // only process for an uploaded document if one has been sent in POST
            if (supportingDocumentationFiles.Any())
            {
                var storeResult = await useCase.StoreSupportingDocuments(
                    application,
                    user,
                    supportingDocumentationFiles,
                    cancellationToken);

                if (storeResult.IsSuccess)
                {
                    application.SupportingDocumentsSection.SupportingDocumentationNotRequired = false;
                    await SaveChangesAsync(application, cancellationToken);
                }
                return BuildRedirect(nameof(SupportingDocumentsSection), storeResult.IsFailure ? storeResult.Error : null);
            }

            return BuildRedirect(nameof(SupportingDocumentsSection));
        }

        /// <summary>
        /// This is an action method to explicitly support the deleting of uploaded supporting documents from the <see cref="SupportingDocumentation(string,string?,CancellationToken)"/> view.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveSupportingDocumentation(
            string identifier,
            string documentIdentifier,
            [FromServices] RemoveSupportingDocumentUseCase useCase,
            CancellationToken cancellationToken)
        {
            var removeResult = await useCase.RemoveSupportingDocument(documentIdentifier, cancellationToken);
            return BuildRedirect(nameof(SupportingDocumentsSection), removeResult.IsFailure ? removeResult.Error : null);
        }

        [HttpGet]
        public async Task<IActionResult> CertificatePreview(
            string identifier,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);

            ApplyViewData(application);
            return View(application);
        }

        
        [HttpPost]
        public async Task<IActionResult> CertificatePreview(
            string identifier,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction, 
            CancellationToken cancellationToken)
        {
            var whereToGoNext = GetRedirectAction(direction, nameof(SupportingDocumentsSection), nameof(CertificatePreview), nameof(Summary));
            return BuildRedirect(whereToGoNext);
        }

        [HttpGet]
        public async Task<IActionResult> Preview(
            string identifier, 
            [FromServices] PreviewExportCertificateUseCase useCase,
            CancellationToken cancellationToken)
        {
            var responseStream = await useCase.GetCertificateAsync(identifier, new ExternalUser(User), cancellationToken);
            return File(responseStream, "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> Summary(
            string identifier, 
            [FromServices] ValidationProvider validationProvider,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            ApplyViewData(application);
            
            var validationFailures = validationProvider.Validate(application);

            ViewBag.ValidationFailures = validationFailures;

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Summary(
            string identifier, 
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction,
            CancellationToken cancellationToken)
        {
            return BuildRedirect(GetRedirectAction(direction, nameof(CertificatePreview), null, nameof(Confirmation)));
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(
            string identifier,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);

            if (_validationProvider.Validate(application).Any())
            {
                ModelState.AddModelError("Application","Application Has Errors");
            }

            ApplyViewData(application);

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmation(
            string identifier,
            [Bind(Prefix = nameof(Models.Application.Confirmation))] Confirmation model,
            [FromForm(Name = PageConstants.NavigationButtonName)] NavigationDirection direction,
            [FromServices] SubmitApplicationUseCase useCase,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);

            ApplyViewData(application);
          
            Result saveResult = Result.Success();

            if (direction == NavigationDirection.Previous)
            {
                // handle the user selecting previous
                if (application.State == ApplicationState.Draft)
                {
                    application.Confirmation = model;
                    saveResult = await SaveChangesAsync(application, cancellationToken);
                }
                
                return saveResult.IsSuccess 
                    ? BuildRedirect(nameof(Summary)) 
                    : View(application);
            }
            
            // failsafe - stop a user submitting at the confirmation screen by editing the browser URL
            // while the application remains invalid.
            var isInvalid = _validationProvider.Validate(application).Any();

            if (isInvalid)
                return RedirectToAction("Confirmation", new { identifier });

            // handle the user selecting Submit
            if (model.AcceptTermsAndConditions == false)  //double-check that the user hasn't somehow been able to click Submit without checking the box
            {
                ModelState.AddModelError(string.Empty, "You must accept the terms and conditions before submitting your application.");
                ApplyViewData(application);
                return View(application);
            }
            
            // only carry this out if the application is in a draft state, else ignore the Save Changes request
            if (application.State == ApplicationState.Draft)
            {
                application.Confirmation = model;
                saveResult = await useCase.SubmitAsync(application, new ExternalUser(User), cancellationToken);
            }

            if (saveResult.IsFailure)
            {
                ViewBag.ErrorText = SaveFailureErrorMessage;
                return View(application);
            }

            TempData["ApplicationReferenceId"] = application.ReferenceIdentifier;
            return RedirectToAction("Completed", "Applications");
        }

        private static bool IsDraft(Application application) => application.State == ApplicationState.Draft;

        private static string GetRedirectAction(NavigationDirection direction, string? previous, string? current, string? next)
        {
            if (!string.IsNullOrEmpty(previous) && direction == NavigationDirection.Previous)
                return previous;

            if (!string.IsNullOrEmpty(current) && direction == NavigationDirection.Save)
                return current;

            if (!string.IsNullOrEmpty(next) && direction == NavigationDirection.Next)
                return next;

            if (direction == NavigationDirection.Summary)
                return "Summary";

            throw new ArgumentOutOfRangeException(nameof(direction));
        }

        private async Task<Application> FindApplicationAsync(string identifier, CancellationToken cancellationToken)
        {
            var user = new ExternalUser(User);
            return await _editApplication.GetByIdAsync(identifier, user, cancellationToken);
        }

        private async Task<Result> SaveChangesAsync(
            Application application,
            CancellationToken cancellationToken = default)
        {
            // only carry this out if the application is in a draft state, else ignore the Save Changes request
            if (!IsDraft(application))
            {
                return Result.Failure(
                    $"Cannot save changes to application with identifier {application.Identifier} as the application is in a {application.State} state. To be updated an application must be in a Draft state.");
            }

            var result = await _editApplication.SaveChangesAsync(application, new ExternalUser(User), cancellationToken);

            if (result.IsFailure)
            {
                ViewBag.ErrorText = SaveFailureErrorMessage;
            }

            return result;
        }
        
        private void ApplyViewData(
            Application application,
            string? error = null)
        {
            var sectionNavigationLinkModel = ApplicationFormSectionsModel.Single(x => x.SectionAction == ControllerContext.ActionDescriptor.ActionName);
            ViewBag.HeadingText = sectionNavigationLinkModel.SectionDisplayName;
            ViewBag.ProgressBarStep = sectionNavigationLinkModel.SectionNumber;
            if (!string.IsNullOrWhiteSpace(error))
            {
                ViewBag.ErrorText = error;
            }

            var distinctErroringSectionActions  = GetApplicationSectionsHavingErrors(application);
            ViewData[EapcConstants.ErroringSectionsViewDataKey] = distinctErroringSectionActions;
        }
        
        private string[] GetApplicationSectionsHavingErrors(Application application)
        {
            var validationFailures = _validationProvider.Validate(application);

            if (!validationFailures.Any()) return Array.Empty<string>();

            var erroringSectionActions = 
                validationFailures.Select(item => item.CustomState as CustomValidationState)
                    .Select(cs => cs.SectionAction).ToList();

            return erroringSectionActions.Distinct().ToArray();
        }
        
        private IActionResult BuildRedirect(string action, string? error = null, bool displaySaveDataMessage = false, bool showErrors = false)
        {
            var identifier = RouteData.Values["identifier"];
            var redirectResult =  !showErrors ? RedirectToAction(action, new {identifier, error}) : RedirectToAction(action, new {identifier, error, showErrors = true});
            TempData["ShowSaveConfirmation"] = displaySaveDataMessage;
            return redirectResult;
        }

        private void ApplySectionValidationModelErrors(Application application, string sectionAction)
        {
            var sectionValidationErrors = _validationProvider.ValidateSection(application, sectionAction);

            if (!sectionValidationErrors.Any()) return;

            foreach (var validationFailure in sectionValidationErrors)
            {
                // Handle fact that dates fields do not have property names stored in the validator result
                // which match the name of the field in the DOM.
                // Will not get visual styling to show validation issue to user otherwise)
                if (validationFailure.PropertyName.EndsWith(".Value.Date"))
                {
                    ModelState.AddModelError(validationFailure.PropertyName.Split(".Value.Date")[0],
                        validationFailure.ErrorMessage);
                    continue;
                }

                if (validationFailure.PropertyName.Equals("Section4.Quantity"))
                {
                    ModelState.AddModelError("Section4.Quantity[0].Unit", validationFailure.ErrorMessage);
                    continue;
                }

                ModelState.AddModelError(validationFailure.PropertyName, validationFailure.ErrorMessage);
            }
        }
    }
}
