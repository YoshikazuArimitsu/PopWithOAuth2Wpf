using MailKit.Security;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopWithOAuth2Wpf.Services.OAuth
{
    /// <summary>
    /// Exchange OAuth2設定
    /// </summary>
    public class ExchangeOAtuhConfig
    {
        /// <summary>
        /// クライアントID
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// テナントID
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// 要求スコープ
        /// </summary>
        public string[] Scopes { get; set; }

        /// <summary>
        /// Exchangeアカウント
        /// </summary>
        public string Account { get; set; }
    }

    /// <summary>
    /// Exchange OAuth2 認証処理
    /// </summary>
    public class ExchangeOAuthService : IOAuthService
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly ExchangeOAtuhConfig _Config;

        /// <summary>
        /// 認証結果
        /// </summary>
        private AuthenticationResult AuthenticationResult { get; set; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="config">OAuth2設定</param>
        public ExchangeOAuthService(ExchangeOAtuhConfig config)
        {
            _Config = config;   
        }

        /// <summary>
        /// 認証処理
        /// </summary>
        /// <returns></returns>
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
                // トークンキャッシュからトークン取得
                result = await publicClientApplication.AcquireTokenSilent(
                    _Config.Scopes,
                    _Config.Account).ExecuteAsync();
                AuthenticationResult = result;
            }
            catch (MsalUiRequiredException ex)
            {
                // 対話認証実行
                result = await publicClientApplication.AcquireTokenInteractive(_Config.Scopes).ExecuteAsync();
                AuthenticationResult = result;
            }

        }

        /// <summary>
        /// MailKit用 認可オブジェクト取得
        /// </summary>
        /// <returns>SaslMechanismOAuth2</returns>
        public SaslMechanismOAuth2 CreateMimekitSasl()
        {
            return new SaslMechanismOAuth2(AuthenticationResult.Account.Username, AuthenticationResult.AccessToken);
        }
    }
}
