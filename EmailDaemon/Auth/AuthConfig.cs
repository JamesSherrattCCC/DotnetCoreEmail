using EmailDaemon.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace EmailDaemon.Auth
{
    class AuthConfig : AConfig<AuthConfig>
    {
        [Required(AllowEmptyStrings = false)]
        public string Instance { get; set; }

        public string Tenant { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public IEnumerable<string> Scopes { get; set; }

        public string ClientSecret { get; set; }

        public string CertificateName { get; set; }

        public static AuthConfig Config { get; }  = AuthConfig.ReadFromJsonFile("authsettings.json");
    }
}
