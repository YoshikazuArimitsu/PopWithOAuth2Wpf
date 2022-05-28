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
    /// <summary>
    /// Google OAuth2設定
    /// </summary>
    public class GoogleOAuthConfig
    {
        /// <summary>
        /// クライアントID
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// クライアントシークレット
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// 要求スコープ
        /// </summary>
        public string[] Scopes { get; set; }

        /// <summary>
        /// Gmailアカウント
        /// </summary>
        public string Account { get; set; }
    }

    /// <summary>
    /// Google OAuth2 認証処理
    /// </summary>
    public class GoogleOAuthService : IOAuthService
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly GoogleOAuthConfig _Config;

        /// <summary>
        /// 認証結果
        /// </summary>
        private UserCredential UserCredential { get; set; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="config">OAuth2設定</param>
        public GoogleOAuthService(GoogleOAuthConfig config)
        {
            _Config = config;
        }

        /// <summary>
        /// 認証処理
        /// </summary>
        /// <returns></returns>
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
            var codeReceiver = new LocalServerCodeReceiver();
            var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

            // 認証実行(キャッシュに入っている場合はそちらから取得)
            var credential = await authCode.AuthorizeAsync(_Config.Account, CancellationToken.None);

            if (credential.Token.IsExpired(SystemClock.Default))
                await credential.RefreshTokenAsync(CancellationToken.None);

            UserCredential = credential;
        }

        /// <summary>
        /// MailKit用 認可オブジェクト取得
        /// </summary>
        /// <returns>SaslMechanismOAuth2</returns>
        public SaslMechanismOAuth2 CreateMimekitSasl()
        {
            return new SaslMechanismOAuth2(_Config.Account, UserCredential.Token.AccessToken);
        }
    }
}
