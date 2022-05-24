using MailKit.Security;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopWithOAuth2Wpf.Services.OAuth
{
    public class ExchangeOAtuhConfig
    {
        public string ClientId { get; set; }
        public string TenantId { get; set; }

        public string[] Scopes { get; set; }
        public string Account { get; set; }
    }


    public class ExchangeOAuthService : IOAuthService
    {
        private readonly ExchangeOAtuhConfig _Config;
        private AuthenticationResult AuthenticationResult { get; set; }

        public ExchangeOAuthService(ExchangeOAtuhConfig config)
        {
            _Config = config;   
        }

        public async Task AuthorizeAsync()
        {
            var options = new PublicClientApplicationOptions
            {
                ClientId = _Config.ClientId,
                TenantId = _Config.TenantId,
                RedirectUri = "http://localhost"
            };

            var publicClientApplication = PublicClientApplicationBuilder
                .CreateWithApplicationOptions(options)
                .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
                .Build();

            AuthenticationResult result;
            try
            {
                result = await publicClientApplication.AcquireTokenSilent(
                    _Config.Scopes,
                    _Config.Account).ExecuteAsync();
                AuthenticationResult = result;
            }
            catch (MsalUiRequiredException ex)
            {
                result = await publicClientApplication.AcquireTokenInteractive(_Config.Scopes).ExecuteAsync();
                AuthenticationResult = result;
            }

        }

        public SaslMechanismOAuth2 CreateMimekitSasl()
        {
            return new SaslMechanismOAuth2(AuthenticationResult.Account.Username, AuthenticationResult.AccessToken);
        }
    }
}
