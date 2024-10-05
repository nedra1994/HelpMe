using Duende.IdentityServer.Models;
using Duende.IdentityServer;

using System.Collections.Generic;

namespace HelpMe.Identity.Data.IdentityServerConfig
{
    public class Config
    {
        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {

                new ApiResource("account", "Account Service"),
                new ApiResource("gateway", "Gateway Service"),
                new ApiResource("orders", "Orders Service"),
                new ApiResource("ticketing", "Ticketing Service"),
                 new ApiResource("ged", "GED Service"),
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {

                new Client
        {
            ClientId = "ExtranetResellerSPA",

            AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
            AccessTokenType = AccessTokenType.Jwt,
            AccessTokenLifetime = 60*60*4, //86400,
            IdentityTokenLifetime = 60*60*4, //86400,
            UpdateAccessTokenClaimsOnRefresh = false,
           // SlidingRefreshTokenLifetime = 30,
            AllowOfflineAccess = true,
            RefreshTokenExpiration = TokenExpiration.Absolute,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
            AlwaysSendClientClaims = true,
            Enabled = true,
            ClientSecrets=  new List<Secret> { new Secret("RTtYf&8/5a$qAnH$frph/zu-u@ZE".Sha256()) },
            AllowedScopes = {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                //IdentityServerConstants.StandardScopes.OfflineAccess,
                 "orders","account" ,"gateway" , "ticketing","ged"
            }
        },
//                //profile  openId basket orders
//                new Client
//{
//    ClientId = "jsc",
//    ClientName = "JavaScript Client",
//    ClientUri = "http://identityserver.io",

//    AllowedGrantTypes = GrantTypes.Implicit,
//    AllowAccessTokensViaBrowser = true,

//    RedirectUris =           { "http://localhost:7017/index.html" },
//    PostLogoutRedirectUris = { "http://localhost:7017/index.html" },
//    AllowedCorsOrigins =     { "http://localhost:7017" },

//    AllowedScopes =
//    {
//        IdentityServerConstants.StandardScopes.OpenId,
//        IdentityServerConstants.StandardScopes.Profile,
//        IdentityServerConstants.StandardScopes.Email,

//        "basket", "orders"
//    }
//},
//                // JavaScript Client
//                new Client
//                {
//                    ClientId = "js",
//                    ClientName = "eShop SPA OpenId Client",
//                    AllowedGrantTypes = GrantTypes.Implicit,
//                    AllowAccessTokensViaBrowser = true,
//                    RedirectUris =           { $"{clientsUrl["Spa"]}/" },

//                    RequireConsent = false,
//                    PostLogoutRedirectUris = { $"{clientsUrl["Spa"]}/" },
//                    AllowedCorsOrigins =     { $"{clientsUrl["Spa"]}" },
//                    AllowedScopes =
//                    {
//                        IdentityServerConstants.StandardScopes.OpenId,
//                        IdentityServerConstants.StandardScopes.Profile,
//                        "orders",
//                        "basket",
//                        "locations",
//                        "marketing",
//                        "webshoppingagg",
//                        "orders.signalrhub",
//                        "webhooks"
//                    },
//                },

//                new Client
//                {
//                    ClientId = "mvc",
//                    ClientName = "MVC Client",
//                    ClientSecrets = new List<Secret>
//                    {
//                        new Secret("secret".Sha256())
//                    },
//                    ClientUri = $"{clientsUrl["Mvc"]}",                             // public uri of the client
//                    AllowedGrantTypes = GrantTypes.Hybrid,
//                    AllowAccessTokensViaBrowser = false,
//                    RequireConsent = false,
//                    AllowOfflineAccess = true,
//                    AlwaysIncludeUserClaimsInIdToken = true,
//                    RedirectUris = new List<string>
//                    {
//                        $"{clientsUrl["Mvc"]}/signin-oidc"
//                    },
//                    PostLogoutRedirectUris = new List<string>
//                    {
//                        $"{clientsUrl["Mvc"]}/signout-callback-oidc"
//                    },
//                    AllowedScopes = new List<string>
//                    {
//                        IdentityServerConstants.StandardScopes.OpenId,
//                        IdentityServerConstants.StandardScopes.Profile,
//                        IdentityServerConstants.StandardScopes.OfflineAccess,
//                        "orders",
//                        "basket",
//                        "locations",
//                        "marketing",
//                        "webshoppingagg",
//                        "orders.signalrhub",
//                        "webhooks"
//                    },
//                    AccessTokenLifetime = 60*60*2, // 2 hours
//                    IdentityTokenLifetime= 60*60*2 // 2 hours
//                },

//                new Client
//                {
//                    ClientId = "mvctest",
//                    ClientName = "MVC Client Test",
//                    ClientSecrets = new List<Secret>
//                    {
//                        new Secret("secret".Sha256())
//                    },
//                    ClientUri = $"{clientsUrl["Mvc"]}",                             // public uri of the client
//                    AllowedGrantTypes = GrantTypes.Hybrid,
//                    AllowAccessTokensViaBrowser = true,
//                    RequireConsent = false,
//                    AllowOfflineAccess = true,
//                    RedirectUris = new List<string>
//                    {
//                        $"{clientsUrl["Mvc"]}/signin-oidc"
//                    },
//                    PostLogoutRedirectUris = new List<string>
//                    {
//                        $"{clientsUrl["Mvc"]}/signout-callback-oidc"
//                    },
//                    AllowedScopes = new List<string>
//                    {
//                        IdentityServerConstants.StandardScopes.OpenId,
//                        IdentityServerConstants.StandardScopes.Profile,
//                        IdentityServerConstants.StandardScopes.OfflineAccess,
//                        "orders",
//                        "basket",
//                        "locations",
//                        "marketing",
//                        "webshoppingagg",
//                        "webhooks"
//                    },
//                },


            };
        }
    }
}