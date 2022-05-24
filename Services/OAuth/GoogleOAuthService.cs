using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using MailKit.Security;

namespace PopWithOAuth2Wpf.Services.OAuth
{
    public class GoogleOAuthConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string[] Scopes { get; set; }
        public string Account { get; set; }
    }

    public class GoogleOAuthService : IOAuthService
    {
        private readonly GoogleOAuthConfig _Config;
        private UserCredential UserCredential { get; set; }

        public GoogleOAuthService(GoogleOAuthConfig config)
        {
            _Config = config;
        }

        public async Task AuthorizeAsync()
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = _Config.ClientId,
                ClientSecret = _Config.ClientSecret
            };

            var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                DataStore = new FileDataStore("CredentialCacheFolder", false),
                Scopes = new[] { "https://mail.google.com/" },
                ClientSecrets = clientSecrets
            });

            // Note: For a web app, you'll want to use AuthorizationCodeWebApp instead.
            var codeReceiver = new LocalServerCodeReceiver();
            var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

            var credential = await authCode.AuthorizeAsync(_Config.Account, CancellationToken.None);

            if (credential.Token.IsExpired(SystemClock.Default))
                await credential.RefreshTokenAsync(CancellationToken.None);

            UserCredential = credential;
        }

        public SaslMechanismOAuth2 CreateMimekitSasl()
        {
            return new SaslMechanismOAuth2(_Config.Account, UserCredential.Token.AccessToken);
        }
    }
}
