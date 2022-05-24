using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PopWithOAuth2Wpf.Services.OAuth
{
    public interface IOAuthService
    {
        public Task AuthorizeAsync();

        public SaslMechanismOAuth2 CreateMimekitSasl();
    }
}
