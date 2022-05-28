using MailKit;
using MailKit.Net.Pop3;
using MailKit.Security;
using PopWithOAuth2Wpf.Services.OAuth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PopWithOAuth2Wpf.Services
{
    /// <summary>
    /// POP3サーバ設定
    /// </summary>
    public class Pop3Config
    {
        /// <summary>
        /// ホスト
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// ポート番号
        /// </summary>
        public int Port { get; set; } = 995;
    }

    /// <summary>
    /// POP3サービス
    /// </summary>
    public class Pop3Service
    {
        /// <summary>
        /// POP3サーバ設定
        /// </summary>
        public Pop3Config Pop3Config { get; set; }

        /// <summary>
        /// Google/Exchange 認証サービス
        /// </summary>
        public IOAuthService OAuthService { get; set; }


        /// <summary>
        /// 認証
        /// </summary>
        /// <returns></returns>
        public async Task AuthorizeAsync()
        {
            await OAuthService.AuthorizeAsync();
        }

        /// <summary>
        /// POP3サーバへのアクセス
        /// </summary>
        /// <remarks>
        /// ログインし、メールボックス先頭10件の件名を取得する。
        /// 先に AuthorizeAsync() を呼び出し、認可を受けているものとする
        /// </remarks>
        /// <returns>件名のリスト</returns>
        public async Task<IEnumerable<string>> GetMailSubjectsAsync()
        {
            var subjects = new List<string>();
            var oauth2 = OAuthService.CreateMimekitSasl();

            using (var client = new Pop3Client())
            {
                // POP3サーバ接続＆OAuth2認証
                await client.ConnectAsync(Pop3Config.Host, Pop3Config.Port, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(oauth2);

                // 先頭10メールの件名取得
                var count = await client.GetMessageCountAsync();
                count = count > 10 ? 10 : count;
                for(int i = 0; i < count; i++)
                {
                    var message = await client.GetMessageAsync(i);
                    subjects.Add(message.Subject);
                }

                await client.DisconnectAsync(true);
            }

            return subjects;
        }
    }
}
