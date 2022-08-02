using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Certificate;
using Forestry.Eapc.External.Web.Services.Repositories.DataVerse;
using Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator;
using Forestry.Eapc.External.Web.Services.Repositories.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NodaTime;
using SystemClock = NodaTime.SystemClock;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureAdB2CServices(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new AzureAdB2COptions();
            configuration.Bind("AzureAdB2C", settings);
            
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.Authority = settings.Authority;
                    options.ClientId = settings.ClientId;
                    options.ClientSecret = settings.ClientSecret;
                    options.UseTokenLifetime = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.ResponseMode = OpenIdConnectResponseMode.Query;
                    options.UsePkce = false;
                    options.AccessDeniedPath = new PathString("/Home");
                    options.Scope.Add("offline_access");
                    options.SaveTokens = true; // this causes the framework code to store access and refresh tokens as part of the auth cookie, making things just easier for us
                    options.ClaimActions.Add(new MapAllClaimsAction());
                    options.Events.OnAuthorizationCodeReceived = async context =>
                    {
                        // Microsoft-specific kind of nonsense for resolving a code to an access token
                        var code = context.ProtocolMessage.Code;
                        var request = context.HttpContext.Request;
                        string currentUri = UriHelper.BuildAbsolute(
                            request.Scheme,
                            request.Host,
                            request.PathBase,
                            options.CallbackPath);

                        IConfidentialClientApplication cca = ConfidentialClientApplicationBuilder.Create(options.ClientId)
                            .WithB2CAuthority(options.Authority)
                            .WithRedirectUri(currentUri)
                            .WithClientSecret(options.ClientSecret)
                            .Build();

                        try
                        {
                            AuthenticationResult result = await cca.AcquireTokenByAuthorizationCode(options.Scope, code).ExecuteAsync();
                            context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                        }
                        catch (Exception ex)
                        {
                            //TODO: Handle
                            throw;
                        }
                    };
                    
                    options.Events.OnTokenValidated = async context =>
                    {
                        var principal = context.Principal;
                        var userRepository = context.HttpContext.RequestServices.GetService<ILocalUserRepository>();

                        if (principal != null && userRepository != null)
                        {
                            await userRepository.HandleUserLoginAsync(principal);
                        }
                    };
                });

            return services;
        }

        public static IServiceCollection AddEapcServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IClock>(SystemClock.Instance);
            services.AddSingleton<CountryProvider>();
            services.AddSingleton<AdditionalDeclarationsProvider>();
            services.AddSingleton<BotanicalNamesProvider>();
            services.AddSingleton<CommodityTypesProvider>();
            services.AddSingleton<ValidationProvider>();
            services.AddSingleton<FileTypesProvider>();

            RegisterPowerappsIntegration(services, configuration);
            RegisterCertificatePreviewIntegration(services, configuration);

            RegisterUseCases(services, configuration);
            RegisterRepositories(services);

            services.Configure<ApiSecuritySettings>(configuration.GetSection("ApiSecurity"));

            return services;
        }

        private static void RegisterCertificatePreviewIntegration(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PhytoCertificatePreviewSettings>(configuration.GetSection("PhytoCertificatePreview"));
            services.AddHttpClient<CertificateGenerationProxy>((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<PhytoCertificatePreviewSettings>>();
                client.BaseAddress = new Uri(settings.Value.ApiUrl);
            });
        }

        private static void RegisterPowerappsIntegration(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EapcEnvironmentSettings>(configuration.GetSection("EapcEnvironment"));
            services.AddHttpClient("Dataverse",
                c => { c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); });
            services.AddSingleton<ConfidentialClientApplicationAuthentication>();

            services.Configure<ProfessionalOperatorRegistrationEnvironmentSettings>(configuration.GetSection("ProfessionalOperatorRegistrationEnvironment"));
            services.AddHttpClient("ProfessionalOperator",
                c => { c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); });
            services.AddSingleton<ProfessionalOperatorClientApplicationAuthentication>();
        }

        private static void RegisterUseCases(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SupportingDocumentsSettings>(configuration.GetSection("SupportingDocuments"));
            services.AddScoped<CreateNewApplicationUseCase>();
            services.AddScoped<EditApplicationUseCase>();
            services.AddScoped<WithdrawApplicationUseCase>();
            services.AddScoped<RetrieveApplicationsForUserUseCase>();
            services.AddScoped<StoreSupportingDocumentsUseCase>();
            services.AddScoped<RemoveSupportingDocumentUseCase>();
            services.AddScoped<SubmitApplicationUseCase>();
            services.AddScoped<PreviewExportCertificateUseCase>();
            services.AddScoped<ReplicateExistingApplicationUseCase>();
            services.AddScoped<SignUpToUseSystemUseCase>();
            services.AddScoped<ApproveAccountUseCase>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            //services.AddSingleton<IApplicationRepository, InMemoryApplicationRepository>();
            services.AddSingleton<IApplicationRepository, DataverseRepository>();
            services.AddSingleton<IApplicationUnattendedRepository, DataverseRepository>();
            services.AddSingleton<ISupportingDocumentRepository, DataverseRepository>();

            //services.AddSingleton<IProfessionalOperatorRepository, FakeProfessionalOperatorRepository>();
            services.AddSingleton<IProfessionalOperatorRepository, DataverseProfessionalOperatorRepository>();
            services.AddScoped<ILocalUserRepository, DataverseUserRepository>();
        }
    }
}
