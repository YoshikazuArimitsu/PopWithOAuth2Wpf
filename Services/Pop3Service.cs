using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PopWithOAuth2Wpf.Services.OAuth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PopWithOAuth2Wpf.Services
{
    public class Pop3Config
    {
        public string Host { get; set; }
        public int Port { get; set; } = 995;
    }

    public class Pop3Service
    {
        public Pop3Config Pop3Config { get; set; }
        public IOAuthService OAuthService { get; set; }

        public async Task AuthorizeAsync()
        {
            await OAuthService.AuthorizeAsync();
        }

        public async Task<IEnumerable<string>> GetMailSubjectsAsync()
        {
            var subjects = new List<string>();
            var oauth2 = OAuthService.CreateMimekitSasl();

            //var logger = new ProtocolLogger("pop3.log");
            using (var client = new Pop3Client())
            {
                await client.ConnectAsync(Pop3Config.Host, Pop3Config.Port, SecureSocketOptions.SslOnConnect);

                await client.AuthenticateAsync(oauth2);

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
