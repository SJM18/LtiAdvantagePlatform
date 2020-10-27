using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;
using LtiAdvantage;

namespace AdvantagePlatform
{
    /// <summary>
    /// Configuration data required at first startup of Identity Server.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// LTI scopes.
        /// </summary>
        public static ICollection<string> LtiScopes => new[]
        {
            OidcConstants.StandardScopes.OpenId,
            Constants.LtiScopes.Ags.LineItem,
            Constants.LtiScopes.Ags.LineItemReadonly,
            Constants.LtiScopes.Ags.ResultReadonly,
            Constants.LtiScopes.Ags.Score,
            Constants.LtiScopes.Ags.ScoreReadonly,
            Constants.LtiScopes.Nrps.MembershipReadonly
        };

        /// <summary>
        /// Built-in clients.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // Client for Swagger UI
                new Client
                {
                    ClientId = "swagger",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = LtiScopes,
                    RedirectUris = new [] { "/swagger/oauth2-redirect.html" }
                }
            };
        }

        /// <summary>
        /// Identity resources that can be requested.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId()
            };
        }

        /// <summary>
        /// List of API's that are protected.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = Constants.ServiceEndpoints.Ags.LineItemsService,
                    DisplayName = "LTI Assignment and Grade Line Item Service",
                    Description = "Provides tools access to gradebook columns",

                    Scopes = new List<string>()
                    {
                        Constants.LtiScopes.Ags.LineItem,
                        Constants.LtiScopes.Ags.LineItemReadonly
                    }
                },
                new ApiResource
                {
                    Name = Constants.ServiceEndpoints.Ags.ResultsService,
                    DisplayName = "LTI Assignment and Grade Result Service",
                    Description = "Provides tools access to gradebook results",
                    Scopes = new List<string>()
                    {
                        Constants.LtiScopes.Ags.ResultReadonly
                    }
                },
                new ApiResource
                {
                    Name = Constants.ServiceEndpoints.Ags.ScoresService,
                    DisplayName = "LTI Assignment and Grade Score Service",
                    Description = "Provides tools access to gradebook scores",
                    Scopes = new List<string>()
                    {
                        Constants.LtiScopes.Ags.Score,
                        Constants.LtiScopes.Ags.ScoreReadonly
                    }
                },
                new ApiResource
                {
                    Name = Constants.ServiceEndpoints.Nrps.MembershipService,
                    DisplayName = "LTI Names and Role Provisioning Membership Service",
                    Description = "Provides tools access to course membership",
                    Scopes = new List<string>()
                    {
                         Constants.LtiScopes.Nrps.MembershipReadonly
                    }
                }
            };
        }
    }
}